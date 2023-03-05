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
      title: '我不爱背单词UnForum'
    },
    component: () => import('@/views/home/index')
  },
  // 路由重定向
  {
    path: '/HelloWorld',
    name: 'HelloWorld',
    component: HelloWorld
  },
  {
    path: '/word',
    name: 'word',
    meta: {
      // 页面标题title
      title: '单词-UnForum'
    },
    component: () => import('@/views/word/index')
  },
  {
    path: '/ranking',
    meta: {
      // 页面标题title
      title: '排行-UnForum'
    },
    component: () => import('@/views/ranking/index')
  },
  {
    path: '/note',
    meta: {
      // 页面标题title
      title: '笔记'
    },
    component: () => import('@/views/note/index')
  },
  {
    path: '/note2',
    meta: {
      // 页面标题title
      title: '笔记2-编辑'
    },
    component: () => import('@/views/note/note2')
  },
  {
    path: '/test',
    meta: {
      // 页面标题title
      title: '兔小巢消息通知'
    },
    component: () => import('@/views/test/index')
  },
  {
    path: '/selfInfo',
    name: 'selfInfo',
    meta: {
      // 页面标题title
      title: '个人信息-UnForum'
    },
    component: () => import('@/views/user/selfInfo.vue')
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
