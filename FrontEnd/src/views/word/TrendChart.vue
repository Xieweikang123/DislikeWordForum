<template>
  <div class="margin60Auto memContainer">
    <div id="rec7Days" style="width: 500px; height: 300px"></div>
    <div id="rec30Days" style="width: 800px; height: 300px"></div>
    <div id="rec180Days" style="width: 900px; height: 300px"></div>
    <div id="rec360Days" style="width: 1000px; height: 300px"></div>
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

    // that.getRecLineData();
    that.drawLineChart(7, "rec7Days", "最近7天背单词趋势图");
    that.drawLineChart(30, "rec30Days", "最近30天背单词趋势图");
    that.drawLineChart(180, "rec180Days", "最近180天背单词趋势图");
    that.drawLineChart(360, "rec360Days", "最近360天背单词趋势图");
    // that.getrec30Days();

    // option && myChart.setOption(option);
  },
  methods: {
    // //最近30天
    // getrec30Days() {
    //   var that = this;
    //   that.$http.get("/api/Word/GetRecentWordChartData/30").then((res) => {
    //     console.log("GetRecentWordChartData", res);
    //     // that.allData = res.data;
    //     // 基于准备好的dom，初始化echarts实例
    //     var myChart = echarts.init(document.getElementById("rec30Days"));
    //     console.log("mounted", myChart);
    //     // 绘制图表
    //     myChart.setOption({
    //       title: {
    //         text: "最近30天背单词趋势图",
    //       },
    //       tooltip: {},
    //       xAxis: {
    //         data: res.data.recDays,
    //       },
    //       yAxis: {},
    //       series: [
    //         {
    //           name: "单词数",
    //           type: "line",
    //           data: res.data.wordCounts,
    //         },
    //       ],
    //     });
    //   });
    // },
    drawLineChart(howManayDays, chartId, charTitle) {
      var that = this;
      that.$http
        .get("/api/Word/GetRecentWordChartData/" + howManayDays)
        .then((res) => {
          console.log("GetRecentWordChartData", res);
          // that.allData = res.data;
          // 基于准备好的dom，初始化echarts实例
          var myChart = echarts.init(document.getElementById(chartId));
          console.log("mounted", myChart);
          // 绘制图表
          myChart.setOption({
            title: {
              text: charTitle,
            },
            tooltip: {
              // 鼠标悬浮提示框显示 X和Y 轴数据
              trigger: "axis",
              backgroundColor: "rgba(32, 33, 36,.7)",
              borderColor: "rgba(32, 33, 36,0.20)",
              borderWidth: 1,
              textStyle: {
                // 文字提示样式
                color: "#fff",
                fontSize: "12",
              },
            },
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
    // getRecLineData() {
    //   var that = this;
    //   that.$http.get("/api/Word/GetRecentWordChartData/7").then((res) => {
    //     console.log("GetRecentWordChartData", res);
    //     // that.allData = res.data;
    //     // 基于准备好的dom，初始化echarts实例
    //     var myChart = echarts.init(document.getElementById("main"));
    //     console.log("mounted", myChart);
    //     // 绘制图表
    //     myChart.setOption({
    //       title: {
    //         text: "最近七天背单词趋势图",
    //       },
    //       tooltip: {},
    //       xAxis: {
    //         data: res.data.recDays,
    //       },
    //       yAxis: {},
    //       series: [
    //         {
    //           name: "单词数",
    //           type: "line",
    //           data: res.data.wordCounts,
    //         },
    //       ],
    //     });
    //   });
    // },
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