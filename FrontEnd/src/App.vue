<template>
  <div id="app">
    <!-- 组件占位符 -->
    <router-view/>
  </div>
</template>

<style lang="less">
body {
  // 设置最小宽度, 以免出现布局混乱
  // min-width: 1800px;
  margin: 0;
  padding: 0;
  border: 0;
}
#app {
  // 设置字体
  font-family: Avenir, Helvetica, Arial, sans-serif;
  // 让字体更平滑, 抗锯齿
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  // 内容居中
  text-align: center;
  // 顶部不留白
  margin-top: 0;
}
</style>
