<script setup lang="ts">
import { ref } from 'vue'
import { Project, Template } from '../dtos'
const hostObject = window.chrome.webview.hostObjects
let configStr = localStorage.getItem('config') || "{}"
let config = JSON.parse(configStr)
const project = config['projects'].filter((p: Project) => p.active)[0]
const templates = ref<Template[]>()
templates.value = project.templates

let templateNames: string[] = []
async function getTemplateNames() {
    const nameStr: string = await hostObject.hostObjectOne.GetTemplateNames(project.name)
    templateNames = JSON.parse(nameStr) as string[]
    for (let template of templates.value || []) {
        template.exists = templateNames.indexOf(template.name) > -1
    }
}
getTemplateNames()

const loading = ref(false)
const fileText = ref<Map<string, string>>()
const fileTextMap: Map<string, string> = new Map()
async function handleChange(name: string) {
    const key = `${project.name}_${name}`
    if (fileTextMap.has(key)) {
        return
    }
    if (templateNames.indexOf(name) === -1) {
        return
    }
    loading.value = true
    const text: string = await hostObject.hostObjectOne.GetTemplateText(project.name, name)
    fileTextMap.set(key, text)
    fileText.value = fileTextMap
    loading.value = false
}

function buildTitle(template: Template) {
    return template.exists ? template.name : `${template.name}(文件不存在)`
}
</script>

<template>
    <div class="container">
        <el-collapse @change="handleChange" accordion>
            <el-collapse-item v-loading="loading" v-for="template in templates" :title="buildTitle(template)"
                :name="template.name">
                <pre>
                    {{ fileText?.get(`${project.name}_${template.name}`) }}
                </pre>
            </el-collapse-item>
        </el-collapse>
    </div>
</template>

<style scoped>
.container {
    margin: 5px;
}

.el-collapse-item {}

::v-deep(.el-collapse-item button) {
    padding: 0 15px;
}

::v-deep(.el-collapse-item .el-collapse-item__wrap) {
    padding: 0 15px;
}
</style>
