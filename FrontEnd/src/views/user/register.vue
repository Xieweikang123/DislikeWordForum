<template>
  <el-dialog
    title="注册"
    :visible.sync="dialogFormVisible"
    :close-on-click-modal="false"
    width="30%"
  >
    <el-form
      ref="elForm"
      :model="formData"
      :rules="rules"
      size="medium"
      label-width="80px"
    >
      <el-form-item label="账号" prop="UserName">
        <el-input
          v-model="formData.UserName"
          placeholder="请输入账号"
          clearable
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="Password">
        <el-input
          v-model="formData.Password"
          placeholder="请输入密码"
          clearable
          show-password
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="Password">
        <el-input
          v-model="formData.PasswordAgain"
          placeholder="请再次输入密码"
          clearable
          show-password
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
      <el-form-item label-width="0">
        <el-button type="primary" @click="submitForm">注册</el-button>
        <!-- <el-button type="primary" @click="test">ces</el-button> -->
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
            message: "请输入邮箱",
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
    test(){
      var that=this
       that.$http
          .post("/api/user/Test")
          .then((response) => {
            console.log("Test response", response);

            // if(response.data.)
          });
    },
    submitForm() {
      var that = this;

      that.formData.account = "zzzz";
      console.log("submit");
      this.$refs["elForm"].validate((valid) => {
        if (!valid) return;
        // TODO 提交表单
        that.$http
          .post("/api/user/register", that.formData)
          .then((response) => {
            console.log("response", response);
            if (!response.succeeded) {
              that.$message.error(response.errors);
              return;
            }
            if (response.succeeded) {
              //成功
              that.$message.success("注册成功");
              window.localStorage.setItem("token", response.data.data);
            }

            // if(response.data.)
          });
      });
    },
    resetForm() {
      this.$refs["elForm"].resetFields();
    },
    show() {
      this.dialogFormVisible = true;
    },
  },
};
</script>