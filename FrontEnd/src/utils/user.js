

const userJs = {
    //退出登录
    logout() {
        window.localStorage.removeItem("token")
        window.localStorage.setItem("userInfo", null);
        setTimeout(() => {
            window.location.href = '/'
        }, 2000);
    }

}

export default userJs