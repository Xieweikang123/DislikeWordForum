
<template>
  <el-dialog title="标签更名" :visible.sync="dialogFormVisible">
    <el-form ref="ruleForm" :model="form" :rules="rules">
      <el-form-item
        label="原标签名"
        prop="oriTagName"
        :label-width="formLabelWidth"
      >
        <el-input v-model="form.oriTagName" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item
        label="新标签名"
        prop="newTagName"
        :label-width="formLabelWidth"
      >
        <el-input v-model="form.newTagName" autocomplete="off"></el-input>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="dialogFormVisible = false">取 消</el-button>
      <el-button type="primary" @click="onConfirmSave">确 定</el-button>
    </div>
  </el-dialog>
</template>

<script>
export default {
  data() {
    return {
      dialogFormVisible: false,
      form: {
        oriTagName: "",
        newTagName: "",
      },
      rules: {
        oriTagName: [
          { required: true, message: "请输入原标签名", trigger: "blur" },
        ],
        newTagName: [
          { required: true, message: "请输入现标签名", trigger: "blur" },
        ],
      },
      formLabelWidth: "120px",
    };
  },
  methods: {
    onConfirmSave() {
      var that = this;
      console.log("onConfirmSave", that.form);

      this.$refs.ruleForm.validate((valid) => {
        if (valid) {
          // alert("submit!");
          //新旧标签名一样
          if (that.form.newTagName == that.form.oriTagName) {
            that.$message.warning("新旧标签名不能一样");
            return;
          }

          that.$http.post("/api/Tag/ChangeTagName", that.form).then((res) => {
            //关闭面板
            if (res.succeeded) {
              that.$message.success("修改成功");
              that.form.newTagName=''

              that.dialogFormVisible = false;
              that.$emit("RefreshData");
            } else {
              that.$message.error(res.errors);
            }
          });
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
  },
};
</script>
<style >
.el-dialog__body {
  padding: 32px 23px 0px 0px;
}
</style>