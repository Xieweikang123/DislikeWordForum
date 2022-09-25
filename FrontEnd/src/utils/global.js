
var Common = {

  isLogin: false,
  formatTTime(time) {
    return time.replace("T", " ");
  }
}
var user = {
  UserInfo: {},
  //拼接头像前缀
  getAvatorUrl(url) {
    return process.env.VUE_APP_BASE_API + url;
  },
  isLogin() {
    var token = window.localStorage.getItem("token");
    console.log("islogin", window.localStorage.getItem("token"));
    if (token) {
      return true;
    }
    return false;
  }
}


// 暴露出这些属性和方法
export default {
  user,
  Common
}