<template>
  <div>
    <el-drawer
      size="50%"
      :title="dynamicTitle"
      custom-class="drawerStyle"
      :visible.sync="isShowDrawer"
      direction="rtl"
      :modal="false"
      :before-close="handleClose"
    >
      <el-form
        ref="form"
        :model="editRow"
        @submit.native.prevent
        label-width="80px"
      >
        <el-input
          ref="contentIpt"
          @paste.native="onContentPaste"
          type="textarea"
          :rows="18"
          v-model="editRow.sayContent"
        ></el-input>
        <el-form-item label="标签:">
          <el-tag
            :key="titem.id"
            v-for="titem in dynamicTags"
            closable
            :disable-transitions="false"
            @close="handleCloseTag(titem)"
          >
            {{ titem.tagName }}
          </el-tag>
          <el-input
            class="input-new-tag"
            v-if="inputVisible"
            v-model="inputValue"
            ref="saveTagInput"
            size="small"
            @keyup.enter.native="handleInputConfirm"
            @blur="handleInputConfirm"
          >
          </el-input>
          <el-button
            v-else
            class="button-new-tag"
            size="small"
            @click="showInput"
            >+ New Tag</el-button
          >
        </el-form-item>
      </el-form>
      <el-row type="flex" class="row-bg" justify="center">
        <el-col :span="6"
          ><el-button type="primary" @click="onSubmit">保存</el-button></el-col
        >
        <el-col :span="2"
          ><div class="grid-content bg-purple-light"></div
        ></el-col>
        <el-col :span="6">
          <el-popconfirm @confirm="confirmDel" title="确定删除吗？">
            <el-button slot="reference" type="danger">删除</el-button>
          </el-popconfirm></el-col
        >
      </el-row>
    </el-drawer>
    <div v-if="isShowDrawer" class="leftPreview">
      <div class="contentLinePreview" v-html="editRow.sayContent"></div>
    </div>
  </div>
</template>

 <script>
export default {
  data() {
    return {
      dynamicTags: ["标签一", "标签二", "标签三"],
      inputVisible: false,
      inputValue: "",
      editRow: {},

      isShowDrawer: false,
    };
  },
  computed: {
    dynamicTitle() {
      return "内容修改";
    },
  },
  methods: {
    // 保存
    onSubmit() {
      var that = this;
      console.log("that.editRow save", that.editRow);
      that.$http.post("/api/Note/SaveNoteWithTag", that.editRow).then((res) => {
        // //关闭面板
        if (res.succeeded) {
          that.isShowDrawer = false;
          that.$message.success("保存成功");
          that.$emit("RefreshData");
        }
      });
    },
    show(row) {
      this.isShowDrawer = true;

      this.editRow = JSON.parse(JSON.stringify(row));
      console.log("show data", this.editRow);
      this.dynamicTags = this.editRow.noteTags;

      //监听内容粘贴事件
      // this.$refs.contentIpt.addEventListener("paste", this.onContentPaste);
    },
    //向字符串指定下标插入字符串
    //index 插入的下标
    insertStr(str, index, insertStr) {
      return str.substring(0, index) + insertStr + str.substring(index);
    },
    onContentPaste(e) {
      var that = this;
      var file = e.clipboardData.files[0];
      var txtAreaEl = e.srcElement;
      var oriSelectionStart = txtAreaEl.selectionStart;
      //有文件、图片
      if (file) {
        let formData = new FormData();
        formData.append("file", file);
        that.$http.post("/api/File/UploadImg", formData).then((res) => {
          console.log("uploadimg res", res);
          if (res.succeeded) {
            var insertImgSrc = `<img src="${process.env.VUE_APP_BASE_API}${res.data.url}" alt="">`;
            txtAreaEl.value = that.insertStr(
              txtAreaEl.value,
              oriSelectionStart,
              insertImgSrc
            );
            txtAreaEl.selectionStart = oriSelectionStart + insertImgSrc.length;
            txtAreaEl.selectionEnd = txtAreaEl.selectionStart;
            that.editRow.sayContent = txtAreaEl.value;
          }
        });
      }
    },
    //关闭标签
    handleCloseTag(tagItem) {
      // this.dynamicTags.splice(this.dynamicTags.indexOf(tag), 1);
      this.dynamicTags.splice(
        this.dynamicTags.findIndex((x) => x == tagItem),
        1
      );
    },

    showInput() {
      this.inputVisible = true;
      this.$nextTick((_) => {
        this.$refs.saveTagInput.$refs.input.focus();
      });
    },
    //确认添加标签
    handleInputConfirm() {
      let inputValue = this.inputValue;
      if (inputValue) {
        this.dynamicTags.push({ tagName: inputValue });
      }
      this.inputVisible = false;
      this.inputValue = "";
    },
    //确认删除
    confirmDel() {
      var that = this;
      that.$http
        .post("/api/Word/OnDelWord", { id: that.editRow.id })
        .then((res) => {
          //关闭面板
          if (res.succeeded) {
            that.$message.success("删除成功");
            that.isShowDrawer = false;
            that.$emit("RefreshData");
          }
        });
    },

    handleClose(done) {
      done();
      //   this.$confirm("确认关闭？")
      //     .then((_) => {
      //       done();
      //     })
      //     .catch((_) => {});
    },
  },
};
</script>
<style >
.contentLinePreview {
  padding: 35px 44px;
  line-height: 26px;
  white-space: pre-wrap;
}
.leftPreview {
  position: fixed;
  left: 0;
  top: 0;
  background: #fefefe;
  width: 50%;
  height: 100%;
  overflow: scroll;
  /* padding: 38px 0 0 23px; */
  /* transition: all 2s; */
  z-index: 9999;
}
.el-tag + .el-tag {
  margin-left: 10px;
}
.button-new-tag {
  margin-left: 10px;
  height: 32px;
  line-height: 30px;
  padding-top: 0;
  padding-bottom: 0;
}
.input-new-tag {
  width: 90px;
  margin-left: 10px;
  vertical-align: bottom;
}
.drawerStyle {
  /* padding:0 1px; */

  padding: 14px 27px;
}
</style>
