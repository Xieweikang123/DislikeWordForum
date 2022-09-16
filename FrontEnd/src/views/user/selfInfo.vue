<template>
  <el-row :gutter="20">
    <el-col :span="12" :offset="6">
      <el-form ref="form" :model="form" label-width="80px">
        <el-form-item label="账号">
          <!-- <el-input v-model="form.name"></el-input> -->
          <span>{{ form.name }}</span>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="onSubmit">立即创建</el-button>
          <el-button>取消</el-button>
        </el-form-item>
      </el-form>
    </el-col>
  </el-row>
</template>
 
 
 <script>
export default {
  data() {
    return {
      form: {
        // name: "",
        // region: "",
        // date1: "",
        // date2: "",
        // delivery: false,
        // type: [],
        // resource: "",
        // desc: "",
      },
    };
  },
  computed: {},
  mounted() {
    this.getMyInfo();
  },

  methods: {
    //获取个人信息
    getMyInfo() {
      console.log("getMyInfo!");
      var that = this;
      that.$http.get("/api/user/GetUserInfo").then((res) => {
        console.log("GetUserInfo ", res);
        // if (!res.succeeded) {
        //   that.$message.error(res.errors);
        //   return;
        // }
        // that.$message.success("登录成功");
        // window.localStorage.setItem("token", res.data.data);

        // that.hide();
        // that.$emit("CallBack");
      });
    },

    onSubmit() {
      console.log("submit!");
    },
  },
};
</script>

<style>
</style>