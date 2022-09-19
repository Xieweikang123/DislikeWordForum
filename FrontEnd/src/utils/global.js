
var Global = {

    isLogin: false,

}
var user={
    UserInfo:{},
    isLogin(){
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
    Global
}