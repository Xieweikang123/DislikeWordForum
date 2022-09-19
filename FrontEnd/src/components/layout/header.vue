<template>
  <el-header>
    <el-row :gutter="20">
      <el-col :span="16" class="disFlex">
        <div
          v-for="item in menuList"
          @click="jumpTo(item.url)"
          :key="item.name"
        >
          <span :class="item.url == activeUrl ? 'menuActive' : ''">
            {{ item.name }}</span
          >
        </div>
      </el-col>

      <el-col :span="8" v-if="!isLogin">
        <span @click="onLogin" class="hand" style="margin-right: 10px"
          >登录</span
        >
        <span @click="onRegister" class="hand">注册 </span>
      </el-col>
      <el-col :span="8" v-else>
        <el-dropdown>
          <span class="el-dropdown-link">
            已登录<i class="el-icon-arrow-down el-icon--right"></i>
          </span>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item @click.native="jumpTo('/selfInfo')"
              >个人信息</el-dropdown-item
            >
            <el-dropdown-item divided @click.native="onLogout"
              >退出</el-dropdown-item
            >
          </el-dropdown-menu>
        </el-dropdown>
      </el-col>
    </el-row>

    <Login ref="login" @CallBack="updateLoginStatus"></Login>
    <Register ref="register"></Register>
  </el-header>
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
  data() {
    return {
      activeUrl: "/",
      menuList: [
        {
          name: "首页",
          url: "/",
        },
        {
          name: "单词",
          url: "/word",
        },
      ],
      isLogin: false,
    };
  },
  // watch: {
  //   "$Global.user.UserInfo"() {
  //     console.log("watch userinfo");
  //   },
  // },
  computed: {
    // activeUrl() {
    //   console.log("computed this.$route.currentRoute", this.$router.currentRoute.fullPath);
    //   return  window.location.pathname
    // },
  },
  mounted() {
    console.log("header mounted");
    // 获取登录状态
    this.updateLoginStatus();

    this.activeUrl = window.location.pathname;
  },
  methods: {
    //跳转
    jumpTo(url) {
      console.log("jump url", url);
      this.activeUrl = url;
      this.$router.push(url);
    },

    // 更新登录状态
    updateLoginStatus() {
      this.isLogin = this.$Global.user.isLogin();
    },
    //退出登录
    onLogout() {
      this.$message.success("退出成功");
      //登录过期
      window.localStorage.removeItem("token");

      // 获取登录状态
      this.updateLoginStatus();

      //跳转首页
      window.location.href = "/";
    },
    //注册
    onRegister() {
      this.$refs.register.show();
    },
    //登录
    onLogin() {
      this.$refs.login.show();
    },
  },
};
</script>
  
  <style>
.menuActive {
  color: #409eff;
}
.disFlex {
  display: flex;
  justify-content: center;
}
.disFlex div {
  margin-left: 30px;
  cursor: pointer;
}
.disFlex div:hover {
  color: blue;
}
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

/* .el-main {
  background-color: #e9eef3;
  color: #333;
  text-align: center;
  line-height: 160px;
} */

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

/* .el-main {
  background-color: #e9eef3;
  color: #333;
  text-align: center;
  line-height: 160px;
} */

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