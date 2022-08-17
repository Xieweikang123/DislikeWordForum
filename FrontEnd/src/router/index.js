import Vue from 'vue'
import VueRouter from 'vue-router'
import HelloWorld from '@/components/HelloWorld'
// 注册路由插件
Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'home',
    meta: {
      // 页面标题title
      title: '读书论坛'
    },
    component: () => import('@/views/home/index')
  },
  // 路由重定向
  {
    path: '/HelloWorld',
    name: 'HelloWorld',
    component: HelloWorld
  },
  // 只有用户访问到, 才会被加载渲染(惰性加载)
  {
    path: '/error',
    name: 'error',
    component: () => import('@/components/Error')
  },
]

const router = new VueRouter({
  // 此模式下, 不会有 # 在 URL 中
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})
// 路由前置守卫
router.beforeEach((to, from, next) => {
  // 通过
  next()
})
// 将路由对象暴露出去
export default router
