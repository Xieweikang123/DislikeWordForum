import axios from 'axios'
import { MessageBox, Message } from 'element-ui'

// create an axios instance
const http = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // send cookies when cross-domain requests
  timeout: 30000, // request timeout
})

// request interceptor
http.interceptors.request.use(
  config => {
    const token = window.localStorage.getItem("token")
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config
  },
  error => {
    // do something with request error
    console.log("请求有问题请查看")
    console.log("error", error) // for debug
    if (!!error) {
      return Promise.reject(error)
    }
  }
)

// http request 拦截器 每次发送请求前判断vuex里有没有token如果有请求头里添加token
http.interceptors.response.use(
  res => {
    if (
      res &&
      res.data &&
      res.data.code == -11
    ) {
      auth.removeToken()
      window.location.href = "/Login"
    } else if (res && res.headers && res.headers.authorization) {
      auth.removeToken()
    }
    return res.data;
  },
  error => {
    // console.log("error",JSON.stringify(error.response.status))
    if (error.response) {
      switch (error.response.status) {
        case 401:
          //登录过期
          window.localStorage.removeItem("token")

          Message({
            message:"请重新登录",
            type: 'error',
            duration: 5 * 1000
          })

          return Promise.resolve({ succeeded: false, errors: "登录过期，请重新登录" })
        // auth.removeToken()
        // window.location.href = "/Login"
      }
      //return Promise.reject(error.response.data);
    }
    console.log("-----系统错误-----");
    console.log(error);
    if (error) {
      // return Promise.reject(error);
      return Promise.resolve({ succeeded: false, errors: error.message })
    }

    // let errorMsg = ''
    // if (error.message.includes('timeout')) {
    //     errorMsg = '请求超时!'
    // } else {
    //     errorMsg = '请求异常!'
    // }
    // return Promise.resolve({ Success: false, Msg: errorMsg })
  }
)
export default {
  install(Vue) {
    Object.defineProperty(Vue.prototype, '$http', { value: http })
  }
}

// function convertRequestParam(param) {
//   let result = "", newParam = null;

//   if (isObject(param)) {
//     newParam = {}
//     handleRequestParam(param, newParam);
//   }
//   for (let k in newParam) {
//     if (newParam.hasOwnProperty(k) === true) {
//       result +=
//         encodeURIComponent(k) +
//         "=" +
//         encodeURIComponent(newParam[k] || newParam[k] == 0 ? newParam[k] : "") +
//         "&";
//     }
//   }
//   return result;
// }

// function handleRequestParam(param, newParam) {
//   for (let key in param) {
//     if (Object.prototype.toString.call(param[key]) == "[object Object]") {
//       for (let k in param[key]) {
//         if (isObject(param[key][k])) {

//         } else {
//           newParam[key + "." + k] = param[key][k]
//         }
//       }
//     } else if (Object.prototype.toString.call(param[key]) == "[object Array]") {
//       for (let k in param[key]) {
//         if (isObject(param[key][k])) {
//           for (let k2 in param[key][k]) {
//             newParam[key + "[" + k + "]." + k2] = param[key][k][k2]
//           }
//         } else {
//           newParam[key + "[" + k + "]"] = param[key][k]
//         }
//       }
//     } else {
//       newParam[key] = param[key]
//     }
//   }
// }
// function isObject(value) {
//   return typeof value == "object"
// }
// function apiAxios(method, url, params, response) {
//   http({
//       method: method,
//       url: url,
//       data: method === "POST" || method === "PUT" ? params : null,
//       params: method === "GET" || method === "DELETE" ? params : null
//   })
//       .then(function (res) {
//           response(res);
//       })
//       .catch(function (err) {
//           response(err);
//       });
// }

// export default {
//   get: function (url, params, response) {
//       return apiAxios("GET", url, params, response);
//   },
//   post: function (url, params, response) {
//       return apiAxios("POST", url, params, response);
//   },
//   put: function (url, params, response) {
//       return apiAxios("PUT", url, params, response);
//   },
//   delete: function (url, params, response) {
//       return apiAxios("DELETE", url, params, response);
//   },

// };
