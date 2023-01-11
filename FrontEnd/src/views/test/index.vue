<template>
  <div>
   
    233
  </div>
</template>
   <script>

export default {
  
  data() {
    return {
      isDataLoading: false,
      searchContent: "",
      // tagColorArray: ["#fdf6ec", "#d3ffdb", "#ffdfb2", "#cee6ff",'rgb(24 37 113)'],
      tagColorArray: ["rgb(245 248 254)", "rgb(242 255 255)"],
      allTags: [],
      userInfo: {},
      dataList: [],
      pageInfo: {
        pageNumber: 1,
        pageSize: 15,
        totalCount: 0,
        searchKeyValues: [
          {
            key: "tagName",
            value: "",
          },
          {
            key: "sayContent",
            value: "",
          },
        ],
      },
      sendContent: "",
      count: 0,
    };
  },
  watch: {
    "pageInfo.pageNumber": {
      handler(nVal) {
        this.getNoteList();
      },
    },
  },
  computed: {
    //当前标签
    currentTagName() {
      return this.pageInfo.searchKeyValues[0].value;
    },
  },
  mounted() {
    var that = this;
    // console.log("process env", process.env);

   
    
  },
  methods: {
    //搜索
    onSearch() {
      var that = this;
      // console.log("onSearch", this.searchContent);
      this.getNoteList();
    },
    //发送内容
    sendFun() {
      var that = this;
      that.sendContent = contentInput.innerHTML;

      if (!that.$Global.user.isLogin()) {
        that.$message.info("请先登录");
        return;
      }
      if (that.sendContent.length == 0) {
        that.$message.error("请输入发表内容");
        return;
      }
      that.$http
        .post("/api/Note/SendAContent", {
          sayContent: that.sendContent,
          tagName: that.currentTagName,
        })
        .then((res) => {
          //关闭面板
          if (res.succeeded) {
            that.$message.success("发表成功");
            that.sendContent = "";
            contentInput.innerHTML = "";
            that.getNoteList();
          } else {
            that.$message.error(res.errors);
          }
        });
    },

    //设置搜索条件的标签
    setTag(tagName) {
      this.pageInfo.searchKeyValues[0].value = tagName;
      //清除搜索内容
      this.pageInfo.searchKeyValues[1].value = '';
      //搜索
      this.getNoteList();
    },
    //获取闪念分页列表
    getNoteList() {
      var that = this;
      that.isDataLoading = true;
      that.dataList = [];
      //获取标签
      that.getAllTags();

      // setTimeout(() => {
      that.$http.post("/api/Note/GetContentList", that.pageInfo).then((res) => {
        that.isDataLoading = false;

        that.dataList = res.data.list;
        that.pageInfo.totalCount = res.data.totalNumber.value;
      });
      // }, 1000);
    },
    //编辑完笔记
    editOver() {
      //重新获取笔记和标签
      this.getNoteList();
      this.getAllTags();
    },
    //获取所有去重之后的标签
    getAllTags() {
      var that = this;
      that.$http.post("/api/Note/GetMyNoteTagList").then((res) => {
        if (res.succeeded) {
          that.allTags = res.data;
          // that.$message.success("删除成功");
          // that.getNoteList();
        }
      });
    },
    // 编辑笔记
    onEditTag(item) {
      this.$refs.editForm.show(item);
    },
    //确定删除
    confirmDel(el) {
      var that = this;
      that.$http.post("/api/Note/DelAContent", el).then((res) => {
        if (res.succeeded) {
          that.$message.success("删除成功");
          that.getNoteList();
        }
      });
    },
    //页码改变时
    changePageNumber(curPage) {
      var that = this;
      that.pageInfo.pageNumber = curPage;
    },
  },
};
</script>
  
  <style >
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
.leftTagContainer {
  width: 16%;
  position: fixed;
}
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