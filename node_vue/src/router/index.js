import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '@/components/HelloWorld'
import PricePanel from '@/components/PricePanel'
import RealTimeMarket from '@/components/RealTimeMarket'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'HelloWorld',
      component: HelloWorld
    },
    {
      path: '/PricePanel',
      name: 'PricePanel',
      component: PricePanel
    },
    {
      path: '/RealTimeMarket',
      name: 'RealTimeMarket',
      component: RealTimeMarket
    }
  ]
})
