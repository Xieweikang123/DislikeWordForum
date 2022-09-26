<template>
  <div class="container">
    <el-row class="rankingBar">
      <el-col :span="12"> 用户名 </el-col>
      <el-col :span="12"> 背单词数量 </el-col>
    </el-row>

    <div class="rowmargin">
      <el-skeleton :loading="isLoading" animated>
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
        class="disAlignCenter"
        style="    margin-top: 12px;"
        v-for="(item, index) in todayData"
        :key="item.belongUserId"
      >
        <el-col class="disAlignCenter" :span="8" :offset="4">
          <div style="position: relative; margin-right: 16px">
            <el-avatar :src="getAvatorUrl(item.avatar)"> </el-avatar>
            <span class="notxt">{{ index + 1 }}</span>
          </div>
          {{ item.nickName }}
        </el-col>
        <el-col :span="12"> {{ item.sum }} </el-col>
      </el-row>

      <span v-if="todayData.length == 0">今日暂无人背单词</span>
    </div>
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
  computed: {},
  mounted() {
    var that = this;

    that.$http.post("/api/Ranking/GetTodayRanking", {}).then((res) => {
      console.log("GetTodayRanking", res);
      that.todayData = res.data;
      that.isLoading = false;
      if (that.todayData.length == 0) {
        // that.$message.info("今日暂无人背单词");
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
  font-size: 12px;
  position: absolute;
  bottom: 0;
  background: #fff3b3;
  font-weight: 900;
  width: 19px;
  border-radius: 12px;
  line-height: 19px;
  right: -11px;
  color: #914b4b;
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
