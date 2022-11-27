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
        当前标签:{{ currentTagName }}
      </div>
      <el-input
        type="textarea"
        :rows="3"
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
            <span class="contentLine">
              {{ item.sayContent }}
            </span>

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

    //发送闪念
    sendFun() {
      var that = this;

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
            that.getNoteList();
          } else {
            that.$message.error(res.errors);
          }
        });
    },
  },
};
</script>
  
  <style scoped>
.tagItemStyle {
  margin: 8px 9px;
  cursor: pointer;
}
.leftTagContainer {
  width: 20%;
  position: fixed;
}
.contentLine {
  line-height: 26px;
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