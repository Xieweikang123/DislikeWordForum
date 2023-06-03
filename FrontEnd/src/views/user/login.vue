<template>
  <el-dialog title="登录" :visible.sync="dialogFormVisible" :close-on-click-modal="false" width="30%">
    <el-form ref="elForm" :model="formData" :rules="rules" size="medium" label-width="80px">
      <el-form-item label="账号" prop="UserName">
        <el-input v-model="formData.UserName" placeholder="请输入账号" clearable :style="{ width: '100%' }"></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="Password">
        <el-input v-model="formData.Password" @keyup.enter.native="submitForm" placeholder="请输入密码" clearable show-password
          :style="{ width: '100%' }"></el-input>
      </el-form-item>
      <el-form-item label-width="0">
        <el-button type="primary" @click="submitForm">登录</el-button>
        <!-- <el-button @click="resetForm">重置</el-button> -->
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script>
export default {
  data() {
    return {
      formData: {
        UserName: undefined,
        Password: undefined,
      },
      rules: {
        UserName: [
          {
            required: true,
            message: "请输入账号",
            trigger: "blur",
          },
        ],
        Password: [
          {
            required: true,
            message: "请输入密码",
            trigger: "blur",
          },
        ],
      },
      dialogTableVisible: false,
      dialogFormVisible: false,
    };
  },
  methods: {
    submitForm() {
      var that = this;
      console.log("submit");
      this.$refs["elForm"].validate((valid) => {
        if (!valid) return;
        // TODO 提交表单
        that.$http
          .post("/api/user/Login", that.formData)
          // .get("/api/walker/description")
          .then((res) => {
            console.log("login ", res);
            if (!res.succeeded) {
              that.$message.error(res.errors);
              return;
            }
            that.$message.success("登录成功");
            window.localStorage.setItem("token", res.data.token);

            that.$eventBus.$emit("userInfoChange", res.data);
            that.hide();
            //跳转首页
            window.location.href = "/";
          });
      });
    },
    resetForm() {
      this.$refs["elForm"].resetFields();
    },
    //隐藏当前窗口
    hide() {
      this.dialogFormVisible = false;
    },
    show() {
      this.dialogFormVisible = true;
    },
  },
};
</script>

<style  >
.el-dialog__body {
  padding-top: 0;
}
</style>