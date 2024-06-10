<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Project, Template, Connection, Table, Variable } from '../dtos'
const hostObject = window.chrome.webview.hostObjects
let configStr = localStorage.getItem('config') || "{}"
let config = JSON.parse(configStr)
const project = ref<Project>(config['projects'].filter((p: Project) => p.active)[0])
const conn: Connection = project.value.connections.filter((c: Connection) => c.active)[0]
const templates = ref<Template[]>()
templates.value = project.value.templates
const tables = ref<Table[]>()
const checkMap = new Map<string, Map<string, boolean>>()
async function getAllTables() {
    const tableStr: string = await hostObject.hostObjectOne.GetAllTables(JSON.stringify(conn))
    tables.value = JSON.parse(tableStr)
}
getAllTables()

let templateNames = ref<string[]>()
async function getTemplateNames() {
    const nameStr: string = await hostObject.hostObjectOne.GetTemplateNames(project.value.name)
    templateNames.value = JSON.parse(nameStr)
}
getTemplateNames()

for (let table in tables.value) {
    let map = new Map<string, boolean>()
    for (let templateName in templateNames.value) {
        map.set(templateName, false)
    }
    checkMap.set(table, map)
}

const checkRef = ref(checkMap)

function selectRow(all: Table[], current: Table) {
    const check = all.some(a => a.tableName === current.tableName)
    if (!check) {
        checkRef.value.set(current.tableName, null)
    } else {
        const map = new Map<string, boolean>()
        for (let index = 0; index < (templateNames.value?.length || 0); index++) {
            let val = (templateNames.value || [])[index]
            map?.set(val, check)
        }
        checkRef.value.set(current.tableName, map)
    }
}

function updateChecked(event: Event, tableName: string, templateName: string) {
    if (checkRef.value.has(tableName)) {
        const row = checkRef.value.get(tableName)
        row?.set(templateName, event.target?.checked || false)
    } else {
        let map = new Map<string, boolean>()
        map.set(templateName, event.target?.checked || false)
        checkRef.value.set(tableName, map)
    }
}

async function selectFolder2(template: Template) {
    const folderPath: string = await hostObject.hostObjectOne.SelectFolder()
    template.targetPath = folderPath.replace(/\\\\/g, '/').replace(/\\/g, '/')
}

function tableName2BizName(tableName: string, tablePrefix: string) {
    if (tablePrefix) {
        return tableName.replace(tablePrefix, '')
    }
    return tableName
}

function buildFileName(tableName: string, tablePrefix: string, templateFileName: string, nameRule: string) {
    const bizName = tableName2BizName(tableName, tablePrefix)
    return `${upperFirstChar(bizName)}${templateFileName}`
}

function upperFirstChar(word: string) {
    return word.charAt(0).toUpperCase() + word.slice(1)
}

const ignoreField = ref()
function addIgnoreField() {
    if (project.value.ignoreFields.indexOf(ignoreField.value) > -1) {
        ignoreField.value = ''
        return
    }
    project.value.ignoreFields.push(ignoreField.value)
    ignoreField.value = ''
}

function closeTag(tag: string) {
    const index = project.value.ignoreFields.indexOf(tag)
    if (index === -1) {
        return
    }
    project.value.ignoreFields.splice(index, 1)
}

async function save() {
    const content = JSON.stringify(config)
    localStorage.setItem('config', content)
    debugger
    const result = await hostObject.hostObjectOne.WriteConfig(content)
    if (result === 'True') {
        ElMessage('保存成功')
    } else {
        ElMessage('保存失败')
    }
}

const generateInfo = ref<Template[]>()
async function generate() {
    const generateTemplates: Template[] = []
    const variables: Variable = {}
    for (let item of checkRef.value) {
        const map: Map<string, boolean> = item[1] || new Map()
        for (let t of map) {
            if (t[1]) {
                const template = templates.value?.filter(ts => ts.name === t[0])[0]
                const generateTemplate: Template = {} as Template
                generateTemplate.projectName = project.value?.name || ''
                generateTemplate.name = template?.name || ''
                generateTemplate.nameRule = template?.nameRule || ''
                generateTemplate.sourcePath = template?.sourcePath || ''
                generateTemplate.targetPath = template?.targetPath || ''
                generateTemplate.tableName = item[0]
                generateTemplate.finalFileName = buildFileName(generateTemplate.tableName, project.value.tablePrefix, generateTemplate.name, generateTemplate.nameRule)
                generateTemplates.push(generateTemplate)

                variables.tableName = item[0]
                variables.businessName = tableName2BizName(generateTemplate.tableName, project.value.tablePrefix)
                variables.author = ''
            }
        }
    }
    generateInfo.value = generateTemplates
    if (generateTemplates.length === 0) {
        ElMessage('数据为空')
        return
    }
    project.value.variables = variables
    project.value.templates = generateTemplates
    const result = await hostObject.hostObjectOne.Generate(JSON.stringify(project.value))
    if (result === 'True') {
        ElMessage('生成成功')
    } else {
        ElMessage(result)
    }
}

</script>

<template>
    <div class="select-table">
        <el-table :data="tables" :show-overflow-tooltip="true" table-layout="auto" @select="selectRow">
            <el-table-column type="selection" width="55" />
            <el-table-column label="名称" prop="tableName" width="150" />
            <el-table-column label="注释" prop="tableComment" width="150" />
            <el-table-column v-for="(templateName) in templateNames" :label="templateName">
                <template #default="scope">
                    <div style="text-align: center;">
                        <input v-if="checkRef.get(scope.row.tableName)" type="checkbox"
                            @change.native="updateChecked($event, scope.row.tableName, templateName)"
                            :checked="checkRef.get(scope.row.tableName)?.get(templateName)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <div class="info">
        <div class="row" v-for="template in templates">
            <el-input class="input-item" type="text" v-model="template.targetPath" size="small">
                <template #prepend>{{ template.name }}</template>
            </el-input>
            <el-button @click="selectFolder2(template)" size="small">选择文件夹</el-button>
            <el-input class="name-rule" v-model="template.nameRule" size="small" placeholder="文件名规则"></el-input>
        </div>
        <div class="row">
            <el-input class="input-item" type="text" size="small" v-model="ignoreField" @keyup.enter="addIgnoreField"
                placeholder="回车确认">
                <template #prepend>忽略公共字段</template>
            </el-input>
        </div>
        <div class="row">
            <el-tag class="tag-group" v-for="tag in project.ignoreFields" :key="tag" :closable="true" type="primary"
                @close="closeTag(tag)">
                {{ tag }}
            </el-tag>
        </div>
        <div class="row">
            <el-input class="input-item" type="text" size="small" v-model="project.tablePrefix">
                <template #prepend>表前缀</template>
            </el-input>
        </div>
        <div class="row">
            <el-input class="input-item" type="text" size="small" v-model="project.variables.author">
                <template #prepend>作者</template>
            </el-input>
        </div>
    </div>
    <div class="row">
        <el-button size="small" @click="save">保存</el-button>
        <el-button size="small" @click="generate">生成</el-button>
    </div>
    <div>
        {{ generateInfo }}
    </div>
</template>

<style scoped>
.select-table {
    max-height: 300px;
}

.row {
    display: flex;
    margin-top: 5px;
}

.name-rule {
    width: 200px;
}

.tag-group {
    margin-right: 5px;
}

::v-deep(.el-input__inner) {
    font-weight: bold;
}

::v-deep(.el-input-group__prepend) {
    text-align: right;
    min-width: 120px;
}

::v-deep(.el-input-group__append) {
    text-align: left;
    min-width: 120px;
}
</style>
