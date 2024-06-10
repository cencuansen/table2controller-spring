<script setup lang="ts">
import { ref, shallowRef } from 'vue'
import Project from './components/Project.vue'
import Database from './components/Database.vue'
import Template from './components/Template.vue'
import Generate from './components/Generate.vue'

const hostObject = window.chrome.webview.hostObjects
async function getConfig() {
    const con: string = await hostObject.hostObjectOne.GetConfig()
    localStorage.setItem('config', con)
}
getConfig()

const menus = ref<{ name: string; color: string; component: any }[]>([
    { name: "工程", color: "red", component: shallowRef(Project) },
    { name: "数据库", color: "inherit", component: shallowRef(Database) },
    { name: "模板", color: "inherit", component: shallowRef(Template) },
    { name: "生成", color: "inherit", component: shallowRef(Generate) },
])
const current = shallowRef(Project)

async function switchMenu(name: string) {
    const old = menus.value.find((x) => x.color === "red")
    old!.color = "inherit"
    const target = menus.value.find((x) => x.name === name)
    target!.color = "red"
    current.value = target?.component
}
</script>

<template>
    <div class="main">
        <div class="left">
            <div v-for="menu in menus" @click="switchMenu(menu.name)">
                <span class="menu-name" :style="{ 'color': menu.color }">{{ menu.name }}</span>
            </div>
        </div>
        <div class="right">
            <component :is="current"></component>
        </div>
    </div>
</template>

<style scoped>
.main {
    width: 100%;
    height: 100%;
    display: flex;
    background-color: #eee;
}

.left {
    width: 150px;
    height: 100%;
    border-right: #ccc 1px solid;
    text-align: center;
    user-select: none;
}

.left div {
    margin-top: 10px;
    cursor: pointer;
    width: 150px;
}

.right {
    flex-grow: 1;
    overflow: scroll;
    scrollbar-width: none;
}
</style>
