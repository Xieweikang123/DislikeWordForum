<template>
  <el-container>
    <el-header>
      <el-row :gutter="20">
        <el-col :span="6" :offset="6">首页</el-col>
        <el-col :span="6" :offset="6" v-if="!isLogin">
          <span @click="onLogin" class="hand" style="margin-right: 10px"
            >登录</span
          >
          <span @click="onRegister" class="hand">注册 </span>
        </el-col>
        <el-col :span="6" :offset="6" v-else>
          <el-dropdown @click="onLogout">
            <span class="el-dropdown-link">
              已登录<i class="el-icon-arrow-down el-icon--right"></i>
            </span>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>黄金糕</el-dropdown-item>
              <!-- <el-dropdown-item>狮子头</el-dropdown-item>
              <el-dropdown-item>螺蛳粉</el-dropdown-item> -->
              <!-- <el-dropdown-item disabled>双皮奶</el-dropdown-item> -->
              <el-dropdown-item divided>退出</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </el-col>
      </el-row>
    </el-header>
    <el-main>Main</el-main>

    <Login ref="login"></Login>
    <Register ref="register"></Register>
  </el-container>
</template>
 
 
 <script>
import Menu from "@/components/menu/menu.vue";
import Login from "@/views/user/login.vue";
import Register from "@/views/user/register.vue";

export default {
  components: {
    Menu,
    Login,
    Register,
  },
  computed: {
    isLogin() {
      var token = window.localStorage.getItem("token");
      console.log("islogin", window.localStorage.getItem("token"));
      if (token) {
        return true;
      }
      return false;
    },
  },
  methods: {
    //退出登录
    onLogout() {
      console.log("logout");
      //登录过期
      window.localStorage.removeItem("token");
    },
    //注册
    onRegister() {
      this.$refs.register.show();
    },
    //登录
    onLogin() {
      console.log("onLogin");
      this.$refs.login.show();
    },
  },
};
</script>

<style>
.el-dropdown-link {
  cursor: pointer;
  color: #409eff;
}
.el-popper {
  top: 45px !important;
}
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