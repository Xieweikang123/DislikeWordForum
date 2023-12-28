<template>
  <div class="row1" style="">
    <RankingCard :isLoading="isLoading" :title="'今日-用户背单词排行'" :rdata="todayData" :cols="['用户名', '背单词数量']"
      :dataPropertys="['sum']">
    </RankingCard>
    <RankingCard :isLoading="isTotalLoading" :title="'全部-背单词统计'" :rdata="totalDayData" :cols="['用户名', '背单词数量']"
      :dataPropertys="['sum']">
    </RankingCard>
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
      totalDayData: [],
      isTotalLoading: true,
      isLoading: true,
      todayData: [],
    };
  },
  computed: {},
  mounted() {
    this.getTodayRankingData()
    this.getAllUserWordViews()
  },
  methods: {

    // 获取所有用户单词浏览记录
    getAllUserWordViews() {
      this.$http.get("/api/Ranking/GetAllUserWordViews", {}).then((res) => {
        console.log('GetAllUserWordViews', res)
        this.totalDayData = res.data;
        this.isTotalLoading = false;
      });
    },
    // 获取今日背单词数据
    getTodayRankingData() {
      this.$http.post("/api/Ranking/GetTodayRanking", {}).then((res) => {
        this.todayData = res.data;
        this.isLoading = false;
      });
    }
    // //拼接头像前缀
    // getAvatorUrl(url) {
    //   return process.env.VUE_APP_BASE_API + url;
    // },
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
