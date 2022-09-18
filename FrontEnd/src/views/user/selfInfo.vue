<template>
  <el-row :gutter="20">
    <el-col :span="12" :offset="6">
      <el-form ref="form" :model="form" label-width="80px">
        <el-form-item label="头像:">
          <el-upload
            class="avatar-uploader"
            action="https://jsonplaceholder.typicode.com/posts/"
            :show-file-list="false"
            :on-success="handleAvatarSuccess"
            :before-upload="beforeAvatarUpload"
          >
            <img v-if="form.avatar" :src="form.avatar" class="avatar" />
            <i v-else class="el-icon-plus avatar-uploader-icon"></i>
          </el-upload>
        </el-form-item>
        <el-form-item label="账号:">
          <span>{{ form.userName }}</span>
        </el-form-item>
        <el-form-item label="昵称:">
          <el-input v-model="form.nickName"></el-input>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="onSubmit">保存</el-button>
          <!-- <el-button>取消</el-button> -->
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
    handleAvatarSuccess(res, file) {
      this.form.avatar = URL.createObjectURL(file.raw);
    },
    beforeAvatarUpload(file) {
      const isJPG = file.type === "image/jpeg";
      const isLt2M = file.size / 1024 / 1024 < 2;

      if (!isJPG) {
        this.$message.error("上传头像图片只能是 JPG 格式!");
      }
      if (!isLt2M) {
        this.$message.error("上传头像图片大小不能超过 2MB!");
      }
      return isJPG && isLt2M;
    },
    //获取个人信息
    getMyInfo() {
      console.log("getMyInfo!");
      var that = this;
      that.$http.get("/api/user/GetUserInfo").then((res) => {
        console.log("GetUserInfo ", res);
        var data = res.data;
        // that.form.userName=data.userName
        // that.form.userSex=data.userSex
        that.form = {
          ...data,
        };
      });
    },
    // 保存信息
    onSubmit() {
      console.log("submit!");
      var that = this;
      that.$http
        .post("/api/user/SaveUserInfo", that.form)
        // .get("/api/walker/description")
        .then((res) => {
          console.log("SaveUserInfo ", res);
          that.$message.success("保存成功");
        });
    },
  },
};
</script>

<style>
:root {
  --widthPx: 78px;
}

.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.avatar-uploader .el-upload:hover {
  border-color: #409eff;
}
.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: var(--widthPx);
  height: var(--widthPx);
  line-height: var(--widthPx);
  text-align: center;
}
.avatar {
  width: var(--widthPx);
  height: var(--widthPx);
  display: block;
}
</style>