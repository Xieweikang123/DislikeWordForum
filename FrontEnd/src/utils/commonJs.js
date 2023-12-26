
const commonJs = {
    //检查是否为移动设备
    checkMobile: function () {
        const userAgentInfo = navigator.userAgent;
        const mobileAgents = ['Android', 'iPhone', 'SymbianOS', 'Windows Phone', 'iPad', 'iPod'];
        return mobileAgents.some(x => userAgentInfo.includes(x))
    },
    //记录日志到后端
    writeLog(thisObj,msg) {
        thisObj.$http.post("/api/LogInfo/WriteLog", { Content: msg })
    }
}


export default commonJs