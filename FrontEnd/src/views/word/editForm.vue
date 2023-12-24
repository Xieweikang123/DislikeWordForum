<template>
  <div>
    <el-drawer :title="dynamicTitle" custom-class="drawerStyle" :visible.sync="isShowDrawer" direction="rtl"
      :before-close="handleClose">
      <el-form ref="form" :model="editRow" label-width="80px">
        <el-form-item label="单词:">
          <el-input v-model="editRow.word"></el-input>
        </el-form-item>
        <el-form-item label="翻译:">
          <el-input type="textarea" :rows="2" v-model="editRow.translate"></el-input>
        </el-form-item>
        <el-form-item label="记录次数:">
          <!-- <el-input v-model="editRow.recordTimes"></el-input> -->
          <el-input-number v-model="editRow.recordTimes" :min="0" :step="1" label="次数"></el-input-number>
        </el-form-item>
      </el-form>
      <el-row type="flex" class="row-bg" justify="center">
        <el-col :span="6"><el-button type="primary" @click="onSubmit">保存</el-button></el-col>
        <el-col :span="2">
          <div class="grid-content bg-purple-light"></div>
        </el-col>
        <el-col :span="6">
          <el-popconfirm @confirm="confirmDel" title="确定删除吗？">
            <el-button slot="reference" type="danger">删除</el-button>
          </el-popconfirm></el-col>
      </el-row>
    </el-drawer>
  </div>
</template>

<script>
import WordJS from '@/utils/word'
export default {
  data() {
    return {
      editRow: {},

      isShowDrawer: false,
    };
  },
  computed: {
    dynamicTitle() {
      return this.editRow.word + "修改";
    },
  },
  methods: {
    //确认删除
    confirmDel() {
      var that = this;
      that.$http
        .post("/api/Word/OnDelWord", { id: that.editRow.id })
        .then((res) => {
          console.log("OnDelWord", res, that.$parent);
          //关闭面板
          if (res.succeeded) {
            that.$message.success("删除成功");
            that.isShowDrawer = false;
            that.$emit("RefreshData");
          }
        });
    },
    onSubmit() {
      console.log("submit! wordJS", WordJS);
      var that = this;
      WordJS.SaveWord(that, that.editRow).then(res => {
        console.log("OnSaveWord", res);
        // //关闭面板
        if (res.succeeded) {
          that.isShowDrawer = false;
          that.$message.success("保存成功");
          that.$emit("RefreshData");
        }

      })
    },
    show(row) {
      this.isShowDrawer = true;

      this.editRow = JSON.parse(JSON.stringify(row));
      console.log("show", row);
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
.drawerStyle {
  /* padding:0 1px; */
  width: min-content !important;
  padding: 14px 27px;
}
</style>
