<template>
  <el-header class="headerStyle">
    <el-row :gutter="20">
      <el-col :span="16" class="disFlex">
        <div v-for="item in menuList" @click="jumpTo(item.url)" :key="item.name">
          <span :class="item.url == activeUrl ? 'menuActive' : ''">
            {{ item.name }}</span>
        </div>
      </el-col>

      <el-col class="rightTop" v-if="userInfo && userInfo.id" :span="8">
        <el-dropdown ref="dropdown" trigger="click">
          <span class="el-dropdown-link disFlexSingle">
            <div class="divFlexAlignCenter marginright15">
              <el-avatar v-if="AvatorUrl.length > 0" :key="AvatorUrl" shape="square" :src="AvatorUrl"></el-avatar>
              <el-avatar v-else shape="square">{{
                userInfo.userName
              }}</el-avatar>
            </div>
            <!-- 非移动端，显示昵称 -->
            <div v-if="!$store.getters.isMobile">
              {{ userInfo.nickName
              }}<i class="el-icon-arrow-down el-icon--right"></i>
            </div>
          </span>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item @click.native="jumpTo('/selfInfo')">个人信息</el-dropdown-item>
            <el-dropdown-item divided @click.native="onLogout">退出</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </el-col>
      <el-col v-else :span="8">
        <span @click="onLogin" class="hand" style="margin-right: 10px">登录</span>
        <span @click="onRegister" class="hand">注册 </span>
      </el-col>
    </el-row>

    <Login ref="login"></Login>
    <Register ref="register"></Register>
  </el-header>
</template>
   
   
<script>
import Login from "@/views/user/login.vue";
import Register from "@/views/user/register.vue";
import commonJs from '@/utils/commonJs'

export default {
  components: {
    Login,
    Register,
  },
  data() {
    return {
      dropdownVisible: false,// 控制dropdown的显示状态
      userInfo: {
        avatar: ''
      },
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
        {
          name: "统计",
          url: "/ranking",
        },
        {
          name: "笔记",
          url: "/note",
        },
      ],
      isLogin: false,
    };
  },

  computed: {
    // isMobile() {
    //   return commonJs.checkMobile()
    // },
    AvatorUrl() {
      if (
        !this.userInfo ||
        !this.userInfo.avatar ||
        this.userInfo.avatar.length == 0
      ) {
        return "";
      }
      var s1 = process.env.VUE_APP_BASE_API + this.userInfo.avatar
      console.log('s1', s1)
      return process.env.VUE_APP_BASE_API + this.userInfo.avatar;
    },
  },
  mounted() {
    // 获取登录状态
    // this.updateLoginStatus();
    this.activeUrl = window.location.pathname;

    // console.log('cheack mobile', this.$store)
    // const userAgentInfo = navigator.userAgent;
    // console.log('userAgentInfo', userAgentInfo)

    this.userInfo = JSON.parse(window.localStorage.getItem("userInfo"));
    console.log("header  this.userInfo", this.userInfo);

    this.listenUserInfo();
  },
  methods: {
    //监听用户信息变化
    listenUserInfo() {
      var that = this;
      this.$eventBus.$off("userInfoChange");
      this.$eventBus.$on("userInfoChange", function (data) {
        console.log("监听到变化 userInfoChange", data);

        window.localStorage.setItem("userInfo", JSON.stringify(data));
        that.userInfo = {
          ...data,
        };
        that.$set(that.userInfo, 'avatar', data.avatar)
      });
    },
    //跳转
    jumpTo(url) {
      //链接一样，不跳转
      if (this.activeUrl == url) {
        return
      }
      this.activeUrl = url;

      this.$router.push(url);
      // this.$nextTick(() => {
      //   setTimeout(() => {
      //     this.$router.push(url);
      //     // this.$router.push(path);
      //   }, 1000);

      // });

    },

    // // 更新登录状态
    // updateLoginStatus() {
    //   this.isLogin = this.$Global.user.isLogin();
    // },
    //退出登录
    onLogout() {
      this.$message.success("退出成功");
      //登录过期
      window.localStorage.removeItem("token");
      //清空缓存
      window.localStorage.removeItem("userInfo");
      // // 获取登录状态
      // this.updateLoginStatus();

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
@media (max-width: 768px) {
  .rightTop {
    display: flex;
    justify-content: center;
    align-items: center;
    height: inherit;
  }

  .headerStyle .el-row {
    height: 60px;
  }

  .el-dropdown-link {
    /* height: 30px; */
  }

  .el-header {
    padding: 0;
  }

  .headerStyle {
    overflow: auto;
  }
}

.menuActive {
  color: #409eff;
}

.disFlexSingle,
.divFlexAlignCenter {
  display: flex;
}

.divFlexAlignCenter {
  align-items: center;
}

.marginright15 {
  margin-right: 10px;
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

/* .el-popper {
  top: 45px !important;
} */
.hand {
  cursor: pointer;
}

.hand:hover {
  color: blue;
}

.el-header {
  box-shadow: 3px 3px 17px 6px #f9f9f999;
}

.el-header,
.el-footer {
  /* background-color: #b3c0d1; */
  color: #333;
  text-align: center;
  line-height: 60px;
}

body>.el-container {
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


body>.el-container {
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