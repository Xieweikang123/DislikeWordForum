<template>
  <div class="margin60Auto memContainer">
    <div id="main" style="width: 500px; height: 300px"></div>
  </div>
</template>

  <script>
import * as echarts from "echarts";

export default {
  components: {},
  data() {
    return {
      isShowTranslate: false,
      allData: [],
      currentItem: {},
      recordTimes: 4, //单词数大于几
    };
  },
  watch: {
    // allData() {
    //   this.setRandomOne();
    // },
  },
  mounted() {
    var that = this;

    that.getRecLineData();

    // option && myChart.setOption(option);
  },
  methods: {
    getRecLineData() {
      var that = this;
      that.$http.get("/api/Word/GetRecentWordChartData").then((res) => {
        console.log("GetRecentWordChartData", res);
        // that.allData = res.data;
        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById("main"));
        console.log("mounted", myChart);
        // 绘制图表
        myChart.setOption({
          title: {
            text: "最近七天背单词趋势图",
          },
          tooltip: {},
          xAxis: {
            data: res.data.recDays,
          },
          yAxis: {},
          series: [
            {
              name: "单词数",
              type: "line",
              data: res.data.wordCounts,
            },
          ],
        });
      });
    },
  },
};
</script>
<style scoped>
.memContainer {
  position: relative;
  /* left: -12%; */
  text-align: center;
}
.wordBall {
  margin: 22px auto;
  width: 200px;
  background: #82bfff;
  border-radius: 50%;
  height: 200px;
  line-height: 200px;
  text-align: center;
  font-size: 24px;
  cursor: pointer;
  box-shadow: inset 0px 0px 14px 3px #ffffff;
}
</style>