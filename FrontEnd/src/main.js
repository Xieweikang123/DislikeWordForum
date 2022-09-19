import Vue from 'vue'
import router from './router'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import App from './App.vue'
import axios from 'axios'
import Global from '@/utils/global'

//请求配置
import request from '@/config/request';
Vue.use(request);

// 注册全局组件
Vue.use(ElementUI)
// 将提醒关掉
Vue.config.productionTip = false


Vue.prototype.$Global = Global



console.log('env', process.env)


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

new Vue({
  // 挂载路由对象(相当于 router:router)
  router,
  render: h => h(App),
  mounted() {
  }
}).$mount('#app')
