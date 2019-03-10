import Vue from "vue"
import Vuex from "vuex"

import auth from "./auth.module";
import ad from "./ad.module";

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        ad
    }
})
