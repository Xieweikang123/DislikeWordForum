<template>
  <el-row :gutter="20">
    <el-col :span="24" :offset="0">
      <el-form ref="form" :model="form"  @submit.native.prevent label-width="80px">
        <el-form-item label="头像:">
          <el-upload
            class="avatar-uploader"
            :headers="upHeader"
            :action="ActionUrl"
            :show-file-list="false"
            :on-success="handleAvatarSuccess"
            :before-upload="beforeAvatarUpload"
          >
            <img v-if="form.avatar" :src="AvatorUrl" class="avatar" />
            <i v-else class="el-icon-plus avatar-uploader-icon"></i>
          </el-upload>
        </el-form-item>
        <el-form-item label="账号:">
          <span>{{ form.userName }}</span>
        </el-form-item>
        <el-form-item label="昵称:">
          <el-input v-model="form.nickName"></el-input>
        </el-form-item>
        <el-form-item label="签名:">
          <el-input
            type="textarea"
            :rows="2"
            v-model="form.personalSignature"
          ></el-input>
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
      upHeader: {},
      form: {},
    };
  },
  computed: {
    AvatorUrl() {
      if (!this.form || !this.form.avatar || this.form.avatar.length == 0) {
        return "";
      }
      return process.env.VUE_APP_BASE_API + this.form.avatar;
    },
    ActionUrl() {
      return process.env.VUE_APP_BASE_API + "api/File/UploadImg";
    },
  },
  mounted() {
    var token = localStorage.getItem("token");
    this.upHeader = {
      Authorization: `Bearer ${token}`,
    };
    this.getMyInfo();
  },

  methods: {
    handleAvatarSuccess(res, file) {
      console.log("handleAvatarSuccess", res, file);
      this.form.avatar = res.data.url;
    },
    beforeAvatarUpload(file) {
      // const isJPG = file.type === "image/jpeg";
      const isLt2M = file.size / 1024 / 1024 < 2;

      // if (!isJPG) {
      //   this.$message.error("上传头像图片只能是 JPG 格式!");
      // }
      if (!isLt2M) {
        this.$message.error("上传头像图片大小不能超过 2MB!");
      }
      return isLt2M;
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
        //获取个人信息的时候
        that.storageUserInfo(that.form);
      });
    },
    //储存个人信息
    storageUserInfo(data) {
      //获取个人信息的时候
      window.localStorage.setItem("userInfo", JSON.stringify(data));
    },
    // 保存信息
    onSubmit() {
      console.log("submit!", this.$parent);
      var that = this;
      that.$http
        .post("/api/user/SaveUserInfo", that.form)
        // .get("/api/walker/description")
        .then((res) => {
          console.log("SaveUserInfo ", res);
          if (res.succeeded) {
            that.$message.success("保存成功");
            //保存个人信息的时候
            that.storageUserInfo(that.form);

            this.$eventBus.$emit("userInfoChange", that.form);
          }
        });
    },
  },
};
</script>

<style>
:root {
  --widthPx: 78px;
}

.el-input{
  width: 72%;
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