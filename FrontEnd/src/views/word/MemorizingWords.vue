<template>
  <div class="margin60Auto memContainer">
    单词记录数 ≥
    <el-input-number
      v-model="recordTimes"
      @change="handleChange"
      :min="1"
      label="描述文字"
    ></el-input-number>

    共 {{ allData.length }}个

    <div style="margin: 16px 0px">
      记录数:
      <el-button
        type="primary"
        @click="changeRecordTimes(-1)"
        @keyup.space.native.prevent="fun"
        plain
        >-</el-button
      >
      <el-button
        type="primary"
        @click="changeRecordTimes(1)"
        @keyup.space.native.prevent="fun"
        plain
        >+</el-button
      >
    </div>
    <div class="wordBall" @click="isShowTranslate = !isShowTranslate">
      {{ currentItem.word }}
    </div>
    <div v-show="isShowTranslate">
      {{ currentItem.translate }}
    </div>
    <el-button style="margin: 19px 61px" @click="setRandomOne" type="primary"
      >换一个</el-button
    >
  </div>
</template>

  <script>
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
    allData() {
      this.setRandomOne();
    },
  },
  mounted() {
    var that = this;
    this.recordTimes = window.localStorage.getItem("recordTime");
    if (!this.recordTimes) {
      this.recordTimes = 2;
    }
    this.getList();
    //全局键盘事件
    document.onkeydown = function (e) {
      switch (e.code) {
        case "Space":
          that.setRandomOne();
          break;
        case "Enter":
          // that.changeShowType();
          break;
      }
    };
  },
  methods: {
    fun() {
      console.log("空格");
    },
    //更改单词数
    changeRecordTimes(nums) {
      this.currentItem.recordTimes += nums;
      var that = this;
      that.$http.post("/api/Word/OnSaveWord", this.currentItem).then((res) => {
        console.log("OnSaveWord", res);
        that.$message.success("更改成功");
        that.getList();
      });
    },
    getList() {
      var that = this;
      that.$http
        .get("/api/Word/GetGreaterThanRecordWords/" + that.recordTimes)
        .then((res) => {
          console.log("GetGreaterThanRecordWords", res);
          that.allData = res.data;
        });
    },
    //获取随机一个
    setRandomOne() {
      if (this.allData.length == 0) {
        this.currentItem = {};
        return;
      }
      // Math.round(Math.random()*10)
      //均衡获取 0~ length-1的下标
      var rndIndex = Math.floor(Math.random() * this.allData.length);
      var rndItem = this.allData[rndIndex];
      //如果重复
      if (this.currentItem == rndItem) {
        rndIndex++;
        rndIndex = rndIndex >= this.allData.length ? 0 : rndIndex;
        this.currentItem = this.allData[rndIndex];
      } else {
        this.currentItem = rndItem;
      }
    },
    //单词记录数更改
    handleChange(value) {
      console.log(value);
      window.localStorage.setItem("recordTime", value);
      this.getList();
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