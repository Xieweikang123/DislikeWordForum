<template>
  <div class="container">
    <el-row class="rankingBar">
      <el-col :span="12"> 用户名 </el-col>
      <el-col :span="12"> 背单词数量 </el-col>
    </el-row>
    <el-skeleton
      :loading="isLoading"
      class="rowmargin"
      style="width: 50%"
      animated
    >
      <template slot="template">
        <el-skeleton-item
          v-for="item in [0, 1, 2, 3]"
          :key="item"
          variant="text"
          style="margin-top: 20px"
        />
      </template>
    </el-skeleton>

    <el-row
      class="rowmargin disAlignCenter"
      v-for="(item, index) in todayData"
      :key="item.belongUserId"
    >
      <el-col class="disAlignCenter" :span="8" :offset="4">
        <span class="notxt">{{ index + 1 }}、</span>
        <el-avatar :src="getAvatorUrl(item.avatar)"></el-avatar
        >{{ item.nickName }}
      </el-col>
      <el-col :span="12"> {{ item.sum }} </el-col>
      <!-- 
      <div class="disAlignCenter">
        <span class="notxt">{{ index + 1 }}、</span>
        <el-avatar :src="getAvatorUrl(item.avatar)"></el-avatar>
        {{ item.nickName }}
      </div>
      {{ item.sum }} -->
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isLoading: true,
      todayData: [],
    };
  },
  computed: {
  },
  mounted() {
    var that = this;

    that.$http.post("/api/Ranking/GetTodayRanking", {}).then((res) => {
      console.log("GetTodayRanking", res);
      that.todayData = res.data;
      that.isLoading = false;
      if (that.todayData.length == 0) {
        that.$message.info("今日暂无人背单词");
      }
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
  padding: 13px 0;
  color: #82b1d3;
  font-weight: 600;
}
.notxt {
  font-size: 16px;
  color: #145e7e;
  text-shadow: 3px 1px 4px white;
}
.rowmargin {
  margin-top: 10px;
  /* width: 41%; */
  margin: 13px auto;
  justify-content: space-between;
}
.container {
  text-align: center;
  width: 60%;
  margin: 0 auto;
}
</style>
