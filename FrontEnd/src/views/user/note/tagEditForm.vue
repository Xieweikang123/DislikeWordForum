<template>
  <div>
    <el-drawer
      :title="dynamicTitle"
      custom-class="drawerStyle"
      :visible.sync="isShowDrawer"
      direction="rtl"
      :before-close="handleClose"
    >
      <el-form
        ref="form"
        :model="editRow"
        @submit.native.prevent
        label-width="80px"
      >
        <el-form-item label="内容:">
          <el-input
            type="textarea"
            :rows="4"
            v-model="editRow.sayContent"
          ></el-input>
        </el-form-item>
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
