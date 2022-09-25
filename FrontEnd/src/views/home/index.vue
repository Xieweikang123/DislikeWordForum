<template>
  <div class="margin60Auto">
    <el-input
      type="textarea"
      :rows="2"
      placeholder="请输入内容"
      v-model="sendContent"
    >
    </el-input>
    <div style="text-align: right">
      <el-button @click="sendFun" style="margin-top: 10px" type="primary"
        >发表</el-button
      >
    </div>

    <div>
      <div
        v-for="item in dataList"
        :key="item.id"
        style="border-bottom: 1px solid #e8e6e6"
      >
        <div class="disAlignCenter userHead" style="">
          <el-avatar
            style="margin-right: 10px"
            :src="$Global.user.getAvatorUrl(item.avatar)"
          ></el-avatar>
          {{ item.nickName }}:
        </div>

        <!-- {{ $Global.user.getAvatorUrl(item.avatar) }} -->

        <div class="contentLine" style="">
          {{ item.sayContent }}

          <span style="float: right; font-size: 13px">
            {{ $Global.Common.formatTTime(item.createTime) }}
          </span>
        </div>
      </div>

      <el-pagination
        layout="prev, pager, next"
        :page-size="pageInfo.pageSize"
        @current-change="changePageNumber"
        :total="pageInfo.totalCount"
      >
      </el-pagination>
    </div>
  </div>
</template>
 
 
 <script>
export default {
  data() {
    return {
      dataList: [],
      pageInfo: {
        pageNumber: 1,
        pageSize: 15,
        totalCount: 0,
      },
      sendContent: "",
      count: 0,
    };
  },
  watch: {
    "pageInfo.pageNumber": {
      handler(nVal) {
        this.getFlashContentList();
      },
    },
  },
  computed: {},
  mounted() {
    this.getFlashContentList();
  },
  methods: {
    //页码改变时
    changePageNumber(curPage) {
      var that = this;
      that.pageInfo.pageNumber = curPage;
    },
    //获取闪念分页列表
    getFlashContentList() {
      var that = this;
      that.$http
        .post("/api/FlashContent/GetContentList", that.pageInfo)
        .then((res) => {
          console.log("GetContentList", res);
          that.dataList = res.data.list;
          that.pageInfo.totalCount = res.data.totalNumber.value;
        });
    },
    //发送闪念
    sendFun() {
      var that = this;
      that.$http
        .post("/api/FlashContent/SendAContent", {
          sayContent: that.sendContent,
        })
        .then((res) => {
          console.log("SendAContent", res);
          //关闭面板
          if (res.succeeded) {
            that.$message.success("发表成功");
            that.sendContent = "";
            that.getFlashContentList();
          }
        });
    },
  },
};
</script>

<style scoped>
.contentLine {
  padding: 18px 40px;
  line-height: 26px;
}
.userHead:hover {
  opacity: 1;
}
.userHead {
  opacity: 40%;
  transition: opacity 0.7s;
}
</style>