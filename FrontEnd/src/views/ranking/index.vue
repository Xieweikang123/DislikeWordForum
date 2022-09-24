<template>
  <div class="container">
    <!-- <div>
      <el-button type="primary">今日排行</el-button>
    </div> -->

    <div class="rowmargin disAlignCenter rankingBar">
      <div class="disAlignCenter">
        <!-- <span class="notxt">{{ index + 1 }}、</span> -->
        <!-- <el-avatar :src="getAvatorUrl(item.avatar)"></el-avatar> -->
        用户名
      </div>
      背单词数量
    </div>
    <div
      class="rowmargin disAlignCenter"
      v-for="(item, index) in todayData"
      :key="item.belongUserId"
    >
      <div class="disAlignCenter">
        <span class="notxt">{{ index + 1 }}、</span>
        <el-avatar :src="getAvatorUrl(item.avatar)"></el-avatar>
        {{ item.nickName }}
      </div>
      {{ item.sum }}
    </div>

    <!-- <el-row class="rowmargin"  :align="center" v-for="item in todayData" :key="item.belongUserId">
      <el-col :span="10">
        <el-avatar :src="getAvatorUrl(item.avatar)"></el-avatar>
        {{ item.nickName }}</el-col
      >
      <el-col :span="8">{{ item.sum }}</el-col>
    </el-row> -->
  </div>
</template>

<script>
export default {
  data() {
    return {
      todayData: [],
    };
  },
  computed: {
    // AvatorUrl() {
    //   if (!this.form || !this.form.avatar || this.form.avatar.length == 0) {
    //     return "";
    //   }
    //   return process.env.VUE_APP_BASE_API + this.form.avatar;
    // },
  },
  mounted() {
    var that = this;
    that.$http.post("/api/Ranking/GetTodayRanking", {}).then((res) => {
      console.log("GetTodayRanking", res);
      that.todayData = res.data;
      // //关闭面板
      // if (res.succeeded) {
      //   that.$message.success("删除成功");
      //   that.isShowDrawer = false;
      //   that.$emit("RefreshData");
      // }
    });
  },
  methods: {
    //拼接头像前缀
    getAvatorUrl(url) {
      return process.env.VUE_APP_BASE_API + url;
    },
  },
};
</script>
<style scoped>
.rankingBar {
    box-shadow: 2px 8px 11px 3px #f1fcff;
    padding: 6px 77px;
}
.notxt {
  font-size: 22px;
  color: #145e7e;
  text-shadow: 3px 1px 4px white;
}
.rowmargin {
  margin-top: 10px;
  width: 41%;
  margin: 13px auto;
  justify-content: space-between;
  /* display: flex;
  justify-content: center;
  align-items: center; */
}
.container {
  text-align: center;
  width: 60%;
  margin: 0 auto;
}
</style>
