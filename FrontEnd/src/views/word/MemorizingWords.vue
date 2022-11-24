<template>
  <div>
    <div class="rightStyle">
      <el-switch v-model="configEntity.isOpenVoice" active-text="开启朗读声音">
      </el-switch>
    </div>
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
      <div class="wordBall" @click="wordClick()">
        {{ currentItem.word }}
        <audio
          controls
          ref="audio"
          :src="audioSrc"
          style="display: none"
        ></audio>
      </div>
      <div v-show="isShowTranslate">
        {{ currentItem.translate }}
      </div>
      <el-button style="margin: 19px 61px" @click="setRandomOne" type="primary"
        >换一个</el-button
      >
    </div>
  </div>
</template>

  <script>
export default {
  components: {},
  data() {
    return {
      intervalId: "",
      configEntity: {
        isOpenVoice: false,
      },
      audioSrc: "https://dict.youdao.com/dictvoice?audio=individual&type=2",
      isShowTranslate: false,
      allData: [],
      currentItem: {},
      recordTimes: 4, //单词数大于几
    };
  },
  watch: {
    //配置改变，储存
    configEntity: {
      handler(nVal) {
        console.log("watch configEntity");
        localStorage.setItem("configEntity", JSON.stringify(nVal));
      },
      deep: true,
    },
    "currentItem.word": {
      handler(nVal) {
        var that = this;
        //开启朗读
        if (that.configEntity.isOpenVoice) {
          this.$refs.audio.load();
          this.audioSrc = `https://dict.youdao.com/dictvoice?audio=${this.currentItem.word}&type=2`;
          this.$refs.audio.addEventListener("canplay", function (res) {
            console.log("can play", res);
            that.$refs.audio.play();
          });
        }
        //关闭翻译
        this.isShowTranslate = false;
        //取消之前的定时
        clearInterval(this.intervalId);
        //两秒之后，点击
        this.intervalId = setTimeout(() => {
          that.wordClick();
        }, 2500);
        // this.$refs.audio.play();
      },
    },
    allData() {
      this.setRandomOne();
    },
  },
  mounted() {
    var that = this;

    //  localStorage.setItem("configEntity", JSON.stringify(nVal));
    //储存配置
    var configEntityStorage = localStorage.getItem("configEntity");
    console.log("configEntityStorage", configEntityStorage);
    if (configEntityStorage) {
      that.configEntity = JSON.parse(configEntityStorage);
    }

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
    // //请求并播放音频
    // playAudio() {
    //   var that = this;
    //   that.$http
    //     .get(
    //       `https://dict.youdao.com/dictvoice?audio=${this.currentItem.word}&type=2`,
    //       {
    //         headers: {
    //           "Access-Control-Allow-Origin": "*",
    //         },
    //       }
    //     )
    //     .then((response) => response.blob())
    //     .then((blob) => {
    //       this.$refs.audio.srcObject = blob;
    //       this.$refs.audio.play();
    //     });
    // },
    playAudio() {},
    //点击单词
    wordClick() {
      console.log("click");
      this.isShowTranslate = !this.isShowTranslate;
      // // this.audioSrc = `https://dict.youdao.com/dictvoice?audio=${this.currentItem.word}&type=2`;
      // // this.$refs.audio.load();
      //开启朗读
      if (this.configEntity.isOpenVoice) {
        this.$refs.audio.play();
      }

      // var playPromise = this.$refs.audio.play();
      // if (playPromise !== undefined) {
      //   playPromise
      //     .then((_) => {
      //       // Automatic playback started!
      //       // Show playing UI.
      //       // We can now safely pause video...
      //       // video.pause();
      //       this.$refs.audio.play();
      //     })
      //     .catch((error) => {
      //       console.log("play error", error);
      //       // Auto-play was prevented
      //       // Show paused UI.
      //     });
      // }
    },
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
.rightStyle {
  position: absolute;
  right: 7%;
}
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