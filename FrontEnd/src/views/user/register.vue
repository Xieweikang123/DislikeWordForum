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
      <el-form-item label="邮箱" prop="Email">
        <el-input
          v-model="formData.Email"
          placeholder="请输入邮箱"
          clearable
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="UserPwd">
        <el-input
          v-model="formData.UserPwd"
          placeholder="请输入密码"
          clearable
          show-password
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
       <el-form-item label="确认密码" prop="UserPwd">
        <el-input
          v-model="formData.UserPwdAgain"
          placeholder="请再次输入密码"
          clearable
          show-password
          :style="{ width: '100%' }"
        ></el-input>
      </el-form-item>
      <el-form-item label-width="0">
        <el-button type="primary" @click="submitForm">注册</el-button>
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
        Email: undefined,
        UserPwd: undefined,
      },
      rules: {
        Email: [
          {
            required: true,
            message: "请输入邮箱",
            trigger: "blur",
          },
        ],
        UserPwd: [
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

      that.formData.account="zzzz"
      console.log("submit");
      this.$refs["elForm"].validate((valid) => {
        if (!valid) return;
        // TODO 提交表单
        that.$http
          .post("/api/user/register",that.formData)
          .then((response) => (this.info = response));
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