<script setup lang="ts">
import { ref } from 'vue'

interface Config {
    projects: {
        name: string,
        connections: { active: boolean, type: string, host: string }[],
        templates: { name: string }[]
    }[]
}

const config = ref<Config>()
config.value = JSON.parse(localStorage.getItem('config') || "{}") as Config

</script>

<template>
    <el-table :data="config?.projects">
        <el-table-column width="80">
            <template #default="scope">
                <span v-if="scope.row.active" style="color: red;">
                    启用中
                </span>
            </template>
        </el-table-column>
        <el-table-column label="名称" width="100" prop="name" />
        <el-table-column label="类型" width="100">
            <template #default="scope">
                {{ scope.row.connections.filter(c => c.active)[0].type }}
            </template>
        </el-table-column>
        <el-table-column label="地址" width="100">
            <template #default="scope">
                {{ scope.row.connections.filter(c => c.active)[0].host }}
            </template>
        </el-table-column>
        <el-table-column label="模板" :show-overflow-tooltip="true">
            <template #default="scope">
                {{ scope.row.templates.map(t => t.name).join(',') }}
            </template>
        </el-table-column>
    </el-table>
</template>

<style scoped></style>
