<template>
  <div>
    <div class="leftTagContainer">
      <el-tag @click="setTag('')" class="tagItemStyle tagAllStyle">
        全部
      </el-tag>
      <el-tag
        v-for="(item, index) in allTags"
        :key="item.tagName"
        @click="setTag(item.tagName)"
        class="tagItemStyle"
        :style="{
          'background-color': tagColorArray[index % tagColorArray.length],
        }"
      >
        <span class="mixMode"
          >{{ item.tagName }}({{ item.count }})</span
        ></el-tag
      >
    </div>
    <div class="margin60Auto">
      <!-- <el-alert title="不可关闭的 alert" type="success" :closable="false">
      </el-alert> -->
      <div class="topTagContainer">
        <div>当前标签:</div>
        <!-- <span style="color: #7b5505"> {{ currentTagName }}</span> -->
        <el-input
          style="width: 200px"
          placeholder=""
          v-model="pageInfo.searchKeyValues[0].value"
        >
        </el-input>
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
      <el-row :gutter="20" style="margin-top: 13px">
        <el-col :span="22"
          ><el-input
            v-model="pageInfo.searchKeyValues[1].value"
            @keyup.enter.native="onSearch"
            placeholder="请输入内容"
          ></el-input
        ></el-col>
        <el-col :span="2"
          ><el-button icon="el-icon-search" @click="onSearch" circle></el-button
        ></el-col>
      </el-row>
      <el-skeleton
        v-if="isDataLoading"
        :rows="20"
        animated
        style="margin-top: 10px"
      />

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

    that.getNoteList();
    // setInterval(() => {
    //   that.getNoteList();
    // }, 15000);

    this.userInfo = JSON.parse(window.localStorage.getItem("userInfo"));
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