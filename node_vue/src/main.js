// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import axios from 'axios'
import Vueaxios from 'vue-axios'
import App from './App'
import router from './router'
import '../node_modules/bootstrap/dist/js/bootstrap.min'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'

Vue.config.productionTip = false
Vue.use(Vueaxios, axios)

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
