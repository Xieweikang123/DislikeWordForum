<template>
  <div id="app">
    <!-- <el-container> -->
    <Header v-if="!$route.meta.hideHeader"></Header>
    <!--       
      <el-header v-if="!$route.meta.hideHeader">
        <Header ></Header>
      </el-header> -->
    <el-main>
      <!-- 组件占位符-->
      <router-view /></el-main>
    <!-- </el-container> -->
  </div>
</template>

<script>
import Header from "@/components/layout/header.vue";

export default {
  components: {
    Header,
  },
};
</script>

<style lang="less">
@media (max-width: 768px) {
  .el-dialog {
    width: fit-content !important;
  }

  .margin60Auto {
    width: auto !important;
  }

  .disFlex div {
    margin-left: 11px;
  }
}

.margin60Auto {
  width: 60%;
  margin: 0 auto;
}

.disAlignCenter {
  display: flex;
  align-items: center;
}

body {
  // 设置最小宽度, 以免出现布局混乱
  // min-width: 1800px;
  margin: 0;
  padding: 0;
  border: 0;
}

#app {
  // 设置字体
  // font-family: Avenir, Helvetica, Arial, sans-serif;
  font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB",
    "Microsoft YaHei", "微软雅黑", Arial, sans-serif;

  // 让字体更平滑, 抗锯齿
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  // 内容居中
  // text-align: center;
  // 顶部不留白
  margin-top: 0;
}
</style>
