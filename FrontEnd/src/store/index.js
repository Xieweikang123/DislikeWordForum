// src/store/index.js
import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        // your state here
    },
    getters: {
        isMobile() {
            const userAgentInfo = navigator.userAgent;
            const mobileAgents = ['Android', 'iPhone', 'SymbianOS', 'Windows Phone', 'iPad', 'iPod'];
            return mobileAgents.some(x => userAgentInfo.includes(x))
        }
    },
    mutations: {
        // your mutations here
    },
    actions: {
        // your actions here
    }
});