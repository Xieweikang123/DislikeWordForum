import Vue from 'vue'
// import Vuex from 'vuex'
import App from './App.vue'
import router from './router'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import axios from 'axios'
import Global from '@/utils/global'
// import commonJs from '@/utils/commonJs'

import moment from 'moment'

// 全局引入vue-codemirror
// import {codemirror} from 'vue-codemirror';
// 引入主题 可以从 codemirror/theme/ 下引入多个
// import 'codemirror/theme/idea.css'
// // 引入语言模式 可以从 codemirror/mode/ 下引入多个
// import 'codemirror/mode/sql/sql.js';


//请求配置
import request from '@/config/request';
Vue.use(request);

// Vue.use(Vuex)



// // 配置模板编译器
// Vue.compile = function (template) {
//   const { render, staticRenderFns } = compiler.compile(template)
//   return { render, staticRenderFns }
// }

// 注册全局组件
Vue.use(ElementUI)
// 将提醒关掉
Vue.config.productionTip = false

// Vue.component('vue-ace', VueAce)

import commonJs from './utils/commonJs'

Vue.prototype.$commonJs = commonJs
Vue.prototype.$Global = Global
Vue.prototype.$moment = moment

Vue.prototype.$eventBus = new Vue()


// 设置axios请求的基础路径
axios.defaults.baseURL = process.env.VUE_APP_BASE_API
// 设置axios别名
Vue.prototype.$axios = axios
// 设置axios携带的Token
axios.defaults.headers.common['Authorization'] = localStorage.getItem('token')

router.beforeEach((to, from, next) => {
  /* 路由发生变化修改页面title */
  if (to.meta.title) {
    document.title = to.meta.title
  }
  next()
})


import store from './store'  // 引入刚才创建的 store


new Vue({
  // 挂载路由对象(相当于 router:router)
  router,
  render: h => h(App),
  store, // 将 store 选项注入到 Vue 实例中
  mounted() {
  }
}).$mount('#app')
