<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

const hostObject = window.chrome.webview.hostObjects

let configStr = localStorage.getItem('config') || "{}"
let config = JSON.parse(configStr)
const project = config['projects'].filter(p => p.active)[0]
const conn = project.connections.filter(c => c.active)[0]

const connection = ref()
connection.value = conn

const typeOptions = ref([
    { label: 'MYSQL', value: 'MYSQL' },
    { label: 'POSTGRE', value: 'POSTGRE' },
])

async function test() {
    const ok: string = await hostObject.hostObjectOne.TestConnect(JSON.stringify(connection.value))
    if (ok === 'True') {
        ElMessage('成功')
    } else {
        ElMessage('失败')
    }
}

async function save() {
    conn.type = connection.value.type
    conn.host = connection.value.host
    conn.port = connection.value.port
    conn.user = connection.value.user
    conn.password = connection.value.password
    const content = JSON.stringify(config)
    localStorage.setItem('config', content)
    const result = await hostObject.hostObjectOne.WriteConfig(content)
    if (result === 'True') {
        ElMessage('保存成功')
    } else {
        ElMessage('保存失败')
    }
}
</script>

<template>
    <div class="database-item">
        <div class="item">
            <el-select v-model="connection.type" style="width: 240px">
                <template #prefix>
                    类型
                </template>
                <el-option v-for="item in typeOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
        </div>
        <div class="item">
            <el-input type="text" v-model="connection.host">
                <template #prepend>服务</template>
            </el-input>
        </div>
        <div class="item">
            <el-input type="text" v-model="connection.port">
                <template #prepend>端口</template>
            </el-input>
        </div>
        <div class="item">
            <el-input type="text" v-model="connection.user">
                <template #prepend>用户</template>
            </el-input>
        </div>
        <div class="item">
            <el-input type="text" v-model="connection.password">
                <template #prepend>密码</template>
            </el-input>
        </div>
        <div class="item">
            <el-input type="text" v-model="connection.databaseName">
                <template #prepend>数据库</template>
            </el-input>
        </div>
        <div class="item test-button button" @click="test">测试</div>
        <div class="item save-button button" @click="save">保存</div>
    </div>
</template>

<style scoped>
.database-item {
    user-select: none;
    margin: 10px;
    padding: 10px;
    border-radius: 5px;
}

.item:nth-child(n+2) {
    margin-top: 10px;
}

.item span {
    text-align: right;
    display: inline-block;
    width: 70px;
}

.item input {
    padding: 5px;
    border: 1px solid #aaa;
    border-radius: 3px;
    outline: none;
}

.button {
    width: 50px;
    text-align: center;
    cursor: pointer;
    padding: 3px;
    border-radius: 3px;
    user-select: none;
    border: 1px solid #aaa;
}
</style>
