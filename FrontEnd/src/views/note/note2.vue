<template>
  <div>
    2323
    <!-- <vue-ueditor-wrap v-model="content" editor-id="editor" :config="editorConfig"
      :editorDependencies="['ueditor.config.js', 'ueditor.all.js']" style="height:500px;" /> -->
  </div>
</template>
<script>
import NoteEditForm from "../note/noteEditForm";
import TagEditPop from "../note/tagEditPop";
import ShareCard from "../note/shareCard";
// import { CalendarHeatmap } from "vue-calendar-heatmap";
import CalendarHeatmap from "@/components/CalendarHeatmap/CalendarHeatmap.vue"

// import VueUeditorWrap from 'vue-ueditor-wrap';


export default {
  components: {
    // VueUeditorWrap,

  },
  data() {
    return {
      content: '<p>Hello UEditorPlus</p>',
      editorConfig: {
        serverUrl: '后端服务',
        //             // 配置UEditorPlus的惊天资源
        UEDITOR_HOME_URL: '/static/UEditorPlus'
      }

    };
  },
  watch: {


  },
  computed: {
    //当前标签
    currentTagName() {
      return this.pageInfo.searchKeyValues[0].value;
    },
  },
  mounted() {
    var that = this;

  },
  methods: {
    //关闭标签
    handleTagClose() {
      this.pageInfo.searchKeyValues[3].value = ''
    },
    onDayClick(day) {
      console.log('onDayClick', day)
      var timeStr = day.date.toISOString().slice(0, 10);
      var timeStr1 = this.$moment(day.date).format("YYYY-MM-DD")
      console.log('timeStr', timeStr)
      console.log('timeStr1', timeStr1)
      this.pageInfo.searchKeyValues[3].value = timeStr1

    },
    //获取闪念分页列表
    getNoteList() {
      var that = this;
      that.isDataLoading = true;
      that.dataList = [];
      //获取标签
      that.getAllTags();
      //重新获取热点图
      that.getCalendarHeatmapList()

      // setTimeout(() => {
      that.$http.post("/api/Note/GetContentList", that.pageInfo).then((res) => {
        that.isDataLoading = false;

        that.dataList = res.data.list;
        var searchKeyword = that.pageInfo.searchKeyValues[1].value;
        console.log("searchKeyword", searchKeyword);
        if (searchKeyword && searchKeyword.length > 0) {
          console.log("标红");
        }

        that.pageInfo.totalCount = res.data.totalNumber.value;
      });
      // }, 1000);
    },
    //搜索
    onSearch() {
      //清除搜索内容
      // this.pageInfo.searchKeyValues[1].value = "";
      //页数
      this.pageInfo.pageNumber = 1;
      //搜索
      this.getNoteList();
    },
    getCalendarHeatmapList() {
      var that = this;
      that.$http.post("/api/Note/GetCalendarHeatmapList", that.pageInfo).then((res) => {
        console.log('GetCalendarHeatmapList', res)
        that.timeValue = res.data.map((data, index) => {
          return { date: data.updateDate, count: data.noteCount }
        })

      });
    },
    // 处理 visibilitychange 事件的处理函数
    handleVisibilityChange() {
      if (document.visibilityState === 'visible') {
        // 网页处于可见状态，执行需要的操作
        //清除
        var allTooltips = document.querySelectorAll('.vue-tooltip-theme')
        console.log('allTooltips', allTooltips)
        if (allTooltips.length) {
          // Array.from(allTooltips).map((node) => node.attributes['aria-hidden'].nodeValue = 'true')
          Array.from(allTooltips).map((node) => node.style.display = 'none')
        }
      } else {
        // 网页处于不可见状态，执行需要的操作
      }
    },

    // 恢复
    onRecovery(item) {
      console.log('onRecovery', item)
      var that = this;
      that.$http.post("/api/Note/RecoveryAContent", item).then((res) => {
        if (res.succeeded) {
          that.$message.success("恢复成功");
          that.getNoteList();
        }
      });
    },
    onShare(item) {
      console.log("onShare", item);
      this.beforeImgScaleScrollTop = document.documentElement.scrollTop;
      this.$refs.shareCard.show(item);
    },
    // 点击图片回到顶部方法，加计时器是为了过渡顺滑
    backTop() {
      window.scrollTo({
        top: 0,
        behavior: "smooth",
      });
    },

    //移除笔记 的noteMask类
    removeNoteMask(item) {
      console.log("remove mask", item);
      var curEl = this.$refs["noteItem" + item.id][0];
      curEl.classList.remove("noteMask");
      //删除当前元素
      this.maskNoteEls = this.maskNoteEls.filter((el) => el.id != item.id);
    },
    //是否显示更多按钮
    isShowOpenMore(item) {
      var that = this;
      return that.maskNoteEls.some((x) => x.id == item.id);
    },

    //点击 标签更名
    onTagEditClick() {
      console.log("onTagEditClick", this.$refs.tagEditPop);
      this.$refs.tagEditPop.form.oriTagName = this.currentTagName;

      this.$refs.tagEditPop.dialogFormVisible = true;
    },
    tabFunc1() {
      // console.log("tabfunc1 ");
      // // this.insertInputTxt("contentInput", "\t");
      // return;
    },
    //编辑完笔记
    editOver() {
      //重新获取笔记和标签
      this.getNoteList();
      this.getAllTags();
      //回原处
      document.documentElement.scrollTop = this.beforeImgScaleScrollTop;
    },
    //注册鼠标点击图片放大事件
    listenImgScale() {
      var that = this;
      document
        .getElementById("noteContainer")
        .addEventListener("click", (e) => {
          var target = e.target;
          var docElement = document.documentElement;
          //如果图片正在放大，返回
          //如果点击图片
          if (target.localName == "img") {
            // target.className = target.className == "imgScale" ? "" : "imgScale";
            //当前点中的图片
            that.curImg = target;
            target.className = "imgScale";
            //图片首次点开
            if (!that.isMaskShow) {
              // target.style.transform = "scale(1.5)";
              //记录滚动条
              that.beforeImgScaleScrollTop = docElement.scrollTop;
              //左右居中
              var leftval = (docElement.clientWidth - target.width) / 2;
              target.style.left = leftval + "px";
              console.log("top");
              //上下居中
              target.style.top =
                (docElement.clientHeight - target.height) / 2 +
                docElement.scrollTop +
                "px";
            } else {
            }
            that.isMaskShow = true;
          }
        });

      //键盘 事件
      document.addEventListener("keydown", function (e) {
        //此处填写你的业务逻辑即可  esc
        if (e.keyCode == 27) {
          // 逻辑处理，如隐藏div，调用动画等
          that.clickMask();
          // that.$refs.editForm.isShowDrawer = false;
        }
      });
    },
    clickMask() {
      this.curImg.className = "";
      this.isMaskShow = false;
      document.documentElement.scrollTop = this.beforeImgScaleScrollTop;
    },

    //发送内容
    sendFun() {
      var that = this;
      that.sendContent = contentInput.innerHTML;
      // console.log('send contentInput',contentInput)
      // console.dir(contentInput)

      // return
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
      // //清除搜索内容
      // this.pageInfo.searchKeyValues[1].value = "";
      // //页数
      // this.pageInfo.pageNumber = 1;
      // //搜索
      // this.getNoteList();
    },


    //获取所有去重之后的标签
    getAllTags() {
      var that = this;
      that.$http.post("/api/Note/GetMyNoteTagList", { isRecycleBin: that.isRecycleBin }).then((res) => {
        if (res.succeeded) {
          that.allTags = res.data;
          // that.$message.success("删除成功");
          // that.getNoteList();
        }
      });
    },
    // 编辑笔记
    onEditTag(item) {
      this.beforeImgScaleScrollTop = document.documentElement.scrollTop;
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
      that.backTop();
    },
  },
};
</script>
  
<style >
.openMore {
  width: 100%;
  text-align: center;
  color: #205924;
  font-weight: bold;
  font-size: 32px;
  margin-top: -37px;
  cursor: pointer;
}

.contentLine {
  /* position: relative; */
  padding: 14px 40px;
  line-height: 26px;
}

.contentLine img {
  width: 50%;
}

/* .noteMask::after {
  content: "展开";
  position: absolute;
  bottom: 20px;
  color: red;
  font-size: 22px;
  width: 100%;
  background: #181859;
} */
.noteMask {
  max-height: 600px;
  overflow-y: hidden;
  background-image: linear-gradient(180deg,
      rgb(255 255 255 / 0%) 50%,
      #b0b0b0 100%);
}

.noteItemCls {
  border-bottom: 1px solid #e8e6e6;
  padding: 24px 0px;
}

.contentLine img {
  cursor: cell;
}

.medium-zoom-overlay {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  opacity: 0.8;
  transition: opacity 300ms;
  will-change: opacity;
  background: rgba(0, 0, 0, 0.8);
  z-index: 2;
}

.imgScale {
  position: absolute;
  transform: scale(1.5);
  z-index: 3;
}

/* .imgScale>div{
  width:200px
} */
img {
  transition: all 500ms cubic-bezier(0.2, 0, 0.2, 1);
}

.tagAllStyle {
  background-color: #447154 !important;
  border-color: #e1f3d8;
  color: #ffffff !important;
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
  position: absolute;
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