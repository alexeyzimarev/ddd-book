import ApiService from "../common/api.service";
import {
    CreateAd,
    RenameAd,
    UpdateAdText
} from "./actions.type";
import {
    AdCreated, 
    AdRenamed,
    AdTextUpdated
} from "./mutation.type";

const state = {
    errors: null,
    ad: {},
    notification: {}
};

const getters = {
    currentAd: (state) => state.ad,
    adNotification: (state) => state.notification
};

const actions = {
    async [CreateAd](context, uuid) {
        await ApiService.post("/ad", { id: uuid });
        context.commit(AdCreated, uuid);
    },
    async [RenameAd](context, title) {
        if (context.state.ad && title === context.state.ad.title) return;
        await ApiService.put(
            "/ad/name", 
            {id: context.state.ad.id, title: title});
        context.commit(AdRenamed, title);
    },
    async [UpdateAdText](context, description) {
        if (description === context.state.ad.description) return;
        await ApiService.put(
            "/ad/text",
            {id: context.state.id, title: description});
        context.commit(AdTextUpdated, description);
    }
};

const mutations = {
    [AdCreated](state, id) {
        state.ad = { id: id };
    },
    [AdRenamed](state, title) {
        state.ad.title = title;
        state.notification.show = true;
        state.notification.text = "Title updated";
    },
    [AdTextUpdated](state, description) {
        state.ad.description = description;
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
