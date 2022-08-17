<template>
  <el-container>
    <el-header>
      <el-row :gutter="20">
        <el-col :span="6" :offset="6">首页</el-col>
        <el-col :span="6" :offset="6">
          <span @click="onLogin" class="hand">注册/登录</span>
        </el-col>
      </el-row>
    </el-header>
    <el-main>Main</el-main>

    <Login ref="login"></Login>
  </el-container>
</template>
 
 
 <script>
// import menu from '@/component/la'

import Menu from "@/components/menu/menu.vue";
import Login from "@/views/user/login.vue";

export default {
  components: {
    Menu,
    Login,
  },
  methods: {
    onLogin() {
      console.log("onLogin");
      this.$refs.login.show();
    },
  },
};
</script>

<style>
.hand {
  cursor: pointer;
}
.hand:hover {
  color: blue;
}
.el-header,
.el-footer {
  /* background-color: #b3c0d1; */
  color: #333;
  text-align: center;
  line-height: 60px;
}

.el-aside {
  background-color: #d3dce6;
  color: #333;
  text-align: center;
  line-height: 200px;
}

.el-main {
  background-color: #e9eef3;
  color: #333;
  text-align: center;
  line-height: 160px;
}

body > .el-container {
  margin-bottom: 40px;
}

.el-container:nth-child(5) .el-aside,
.el-container:nth-child(6) .el-aside {
  line-height: 260px;
}

.el-container:nth-child(7) .el-aside {
  line-height: 320px;
}
</style>