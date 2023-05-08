
var WordJS = {
    SaveWord(thatObj,item) {
        console.log("SaveWord!", item);
        // var that = this;
        return new Promise((resolve, reject) => {
            thatObj.$http.post("/api/Word/OnSaveWord", item).then((res) => {
                resolve(res); // 返回异步操作的结果
            });
        })
    },
}

// 暴露出这些属性和方法
export default WordJS