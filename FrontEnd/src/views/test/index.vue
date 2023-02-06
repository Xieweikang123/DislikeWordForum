<template>
  <div>
    cookie:
    <el-input v-model="curCookie"> </el-input>
    <el-button @click="hasRead">消息已读</el-button>
    <div v-for="item in dataList" :key="item.reply_id">
      {{ item.post_user_nick }}:

      <div style="text-indent: 2em">
        {{ item.reply_text }} -------------- {{ item.created_at }}
      </div>
    </div>
  </div>
</template>
   <script>
export default {
  data() {
    return {
      curCookie: "",
      curNewRepId: "",
      dataList: [],
      oriTitle: "",
      curIntervalId: 0,
      getRepIntervalId: 0,
    };
  },
  watch: {
    //当前id变化，储存
    curNewRepId(nVal) {
      localStorage.setItem("curNewRepId", nVal);
    },
    curCookie(nVal) {
      console.log("watch cok", nVal);
      localStorage.setItem("cookie", nVal);
    },
    dataList: {
      handler(nVal) {
        console.log("watch datalist");
        if (nVal && nVal.length > 0) {
          //如果新旧id不一样
          if (this.curNewRepId != nVal[0].reply_id) {
            this.startNotice();
          }

          this.curNewRepId = nVal[0].reply_id;
        }
      },
      deep: true,
    },
  },
  computed: {},
  mounted() {
    var that = this;
    that.oriTitle = document.title;
    //读取cookie
    that.curCookie = localStorage.getItem("cookie");
    that.curNewRepId = localStorage.getItem("curNewRepId");

    that.getRepIntervalId = setInterval(() => {
      that.getNewData();
    }, 5000);
  },
  destroyed() {
    console.log("destroyed");
    clearInterval(this.curIntervalId);
    clearInterval(this.getRepIntervalId);
  },
  methods: {
    //消息已读
    hasRead() {
      this.stopNotice();
    },
    //获取消息
    getNewData() {
      var that = this;
      //新消息提醒中，不获取
      if (that.curIntervalId != 0) {
        return;
      }

      that.$http
        .post("/api/Notify/GetTuReplyMsg", {
          cookie: that.curCookie,
        })
        .then((res) => {
          console.log("get tu rep", res);

          console.log("d1", JSON.parse(res.data));
          that.dataList = JSON.parse(res.data).data;
        });
    },
    //关闭消息提示
    stopNotice() {
      //恢复标题
      document.title = this.oriTitle;
      clearInterval(this.curIntervalId);
      this.curIntervalId = 0;
    },
    // 开启消息提示
    startNotice() {
      var that = this;
      console.log("start notice");
      this.curIntervalId = setInterval(function () {
        var title = document.title;
        if (title == that.oriTitle) {
          document.title = "【你有新消息】";
        } else {
          document.title = that.oriTitle;
        }
      }, 500);
    },
  },
};
</script>
  
  <style scoped>
.tagAllStyle {
  background-color: #447154;
  border-color: #e1f3d8;
  color: #ffffff !important;
}
.mixMode {
  /* mix-blend-mode: difference;
  color: white; */
  color: #022023;
}
.topTagContainer {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}
.contentInput img {
  width: 100px;
}
.contentInput {
  min-height: 150px;
  height: auto;
  border: 1px solid #dadada;
  padding: 2px 4px;
}
.tagItemStyle {
  color: #363f4e;
  margin: 6px 2px;
  cursor: pointer;
}
/* .leftTagContainer {
  width: 16%;
  position: fixed;
} */
.contentLine img {
  width: 50%;
}
.contentLine {
  line-height: 26px;
  white-space: pre-wrap;
}
.userHead:hover {
  opacity: 1;
}
.userHead {
  opacity: 90%;
  /* margin-top: 12px; */
  transition: opacity 0.7s;
}
</style>