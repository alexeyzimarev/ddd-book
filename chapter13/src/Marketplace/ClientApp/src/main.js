import Vue from 'vue'
import './plugins/vuetify'
import App from './App.vue'
import router from './router'
import store from './store'
import UUID from 'vue-uuid';
import 'roboto-fontface/css/roboto/roboto-fontface.css'
import 'material-design-icons-iconfont/dist/material-design-icons.css'
import {CheckAuth} from "./store/actions.type";
import ApiService from "./common/api.service";

Vue.config.productionTip = false;
Vue.use(UUID);

ApiService.init();

router.beforeEach(async (to, from, next) => {
    await store.dispatch(CheckAuth);
    next();
});

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app');
