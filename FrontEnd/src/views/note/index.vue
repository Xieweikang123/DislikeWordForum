<template>
  <div>
    <div class="leftTagContainer">
      <el-tag @click="setTag('')" class="tagItemStyle" type="success">
        全部
      </el-tag>
      <el-tag
        v-for="item in allTags"
        :key="item.tagName"
        @click="setTag(item.tagName)"
        class="tagItemStyle"
        >{{ item.tagName }}({{ item.count }})</el-tag
      >
    </div>
    <div class="margin60Auto">
      <!-- <el-alert title="不可关闭的 alert" type="success" :closable="false">
      </el-alert> -->
      <div v-if="currentTagName.length > 0" style="margin-bottom: 15px">
        当前标签: <span style="color: #7b5505"> {{ currentTagName }}</span>
      </div>
      <!-- <el-input
        type="textarea"
        id="contentInput"
        :rows="3"
        placeholder="请输入内容"
        @paste.native="handlePaste($event)"
        v-model="sendContent"
      >
      </el-input> -->
      <div
        id="contentInput"
        class="contentInput"
        contenteditable
        placeholder="请输入内容"
      ></div>
      <!-- <div id="preview">
        <span>将图片按Ctrl+V 粘贴至此处</span>
      </div> -->
      <div style="text-align: right">
        <el-button @click="sendFun" style="margin-top: 10px" type="primary"
          >发表</el-button
        >
      </div>

      <div>
        <div
          v-for="item in dataList"
          :key="item.id"
          style="border-bottom: 1px solid #e8e6e6; padding: 13px 0px"
        >
          <div class="disAlignCenter userHead" style="">
            <!-- <el-avatar
              style="margin-right: 10px"
              :src="$Global.user.getAvatorUrl(item.avatar)"
            ></el-avatar>
            {{ item.nickName }}: -->
            <el-tag
              style="margin-right: 5px; cursor: pointer"
              @click="setTag(tagItem.tagName)"
              v-for="tagItem in item.noteTags"
              :key="tagItem.id"
              >{{ tagItem.tagName }}</el-tag
            >
          </div>

          <!-- {{ $Global.user.getAvatorUrl(item.avatar) }} -->

          <div style="padding: 14px 40px">
            <div class="contentLine" v-html="item.sayContent">
              <!-- {{ item.sayContent }} -->
            </div>

            <div>
              <span style="float: right; font-size: 13px">
                <el-link
                  @click="onEditTag(item)"
                  type="primary"
                  style="font-size: 12px; margin-right: 5px"
                  >编辑</el-link
                >
                <el-popconfirm
                  v-if="userInfo && userInfo.id == item.userId"
                  title="确定删除吗?"
                  @confirm="confirmDel(item)"
                >
                  <el-link
                    slot="reference"
                    type="danger"
                    style="font-size: 12px"
                    >删除</el-link
                  >
                </el-popconfirm>
                <span>
                  {{ $Global.Common.formatTTime(item.createTime) }}
                </span>
              </span>
            </div>
          </div>
        </div>

        <el-pagination
          v-if="pageInfo.totalCount > 0"
          layout="prev, pager, next"
          :page-size="pageInfo.pageSize"
          @current-change="changePageNumber"
          :total="pageInfo.totalCount"
        >
        </el-pagination>
        <TagEditForm ref="editForm" @RefreshData="editOver"></TagEditForm>
      </div>
    </div>
  </div>
</template>
   <script>
import TagEditForm from "../note/tagEditForm.vue";

export default {
  components: {
    TagEditForm,
  },
  data() {
    return {
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
    that.getNoteList();
    // setInterval(() => {
    //   that.getNoteList();
    // }, 15000);

    this.userInfo = JSON.parse(window.localStorage.getItem("userInfo"));
  },
  methods: {
    //发送内容
    sendFun() {
      var that = this;
      console.log("send", contentInput.innerHTML);
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
    // // 监听粘贴操作
    // handlePaste(event) {
    //   console.log("handle paste", event);
    //   const items = (event.clipboardData || window.clipboardData).items;
    //   let file = null;
    //   if (!items || items.length === 0) {
    //     this.$message.error("当前浏览器不支持本地");
    //     return;
    //   }
    //   // 搜索剪切板items
    //   for (let i = 0; i < items.length; i++) {
    //     if (items[i].type.indexOf("image") !== -1) {
    //       file = items[i].getAsFile();
    //       break;
    //     }
    //   }
    //   if (!file) {
    //     this.$message.error("粘贴内容非图片");
    //     return;
    //   }
    //   // 此时file就是我们的剪切板中的图片对象
    //   // 如果需要预览，可以执行下面代码
    //   const reader = new FileReader();
    //   reader.readAsDataURL(file);

    //   reader.onload = (event) => {
    //     // preview.innerHTML = `<img id="pase-img" src="${event.target.result}" style="width: 100%">`; // 添加style样式保证图片等比缩放
    //     var obj_img = document.createElement("img");
    //     obj_img.src = event.target.result;
    //     contentInput.appendChild(obj_img);

    //     // console.log("file 转 base64结果：" + reader.result);
    //   };

    //   console.log("file", file);
    //   this.pasteFile = file;
    // },
    //设置搜索条件的标签
    setTag(tagName) {
      this.pageInfo.searchKeyValues[0].value = tagName;
      //搜索
      this.getNoteList();
    },
    //获取闪念分页列表
    getNoteList() {
      var that = this;
      //获取标签
      that.getAllTags();

      that.$http.post("/api/Note/GetContentList", that.pageInfo).then((res) => {
        that.dataList = res.data.list;
        that.pageInfo.totalCount = res.data.totalNumber.value;
      });
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
        console.log("GetMyNoteTagList", res);
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
.contentInput img {
  width: 100px;
}
.contentInput {
  min-height: 150px;
  height: auto;
  border: 1px solid #dadada;
}
.tagItemStyle {
  margin: 8px 9px;
  cursor: pointer;
}
.leftTagContainer {
  width: 20%;
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