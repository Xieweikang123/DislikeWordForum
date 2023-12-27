<template>
  <div class="row1" style="">
    <RankingCard :title="'今日用户背单词统计'" :rdata="todayData" :cols="['用户名', '背单词数量']">
    </RankingCard>
    <el-card class="cardContainer">
      <el-row class="rankingBar">
        <el-col :span="12"> 用户名 </el-col>
        <el-col :span="12"> 背单词数量 </el-col>
      </el-row>
      <div class="rowmargin">
        <el-skeleton :loading="isLoading" animated>
          <template slot="template">
            <el-skeleton-item v-for="item in [0, 1, 2, 3]" :key="item" variant="text" style="margin-top: 20px" />
          </template>
        </el-skeleton>

        <el-row class="disAlignCenter" style="    margin-top: 12px;" v-for="(item, index) in todayData"
          :key="item.belongUserId">
          <el-col class="disAlignCenter" :span="12" :offset="1">
            <div style="position: relative; margin-right: 16px">
              <el-avatar :src="getAvatorUrl(item.avatar)"> </el-avatar>
              <span class="notxt">{{ index + 1 }}</span>
            </div>
            <span class="nickNames">
              {{ item.nickName }}
            </span>
          </el-col>
          <el-col :span="12"> {{ item.sum }} </el-col>
        </el-row>

        <span v-if="todayData.length == 0">今日暂无人背单词</span>
      </div>
    </el-card>
  </div>
</template>

<script>
import RankingCard from '@/components/RankingCard.vue';
export default {
  components: {
    RankingCard
  },
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
.row1 {
  display: flex;
  justify-content: center;
}

@media (max-width: 768px) {

  .row1 {

    flex-direction: column;
  }

}
</style>
