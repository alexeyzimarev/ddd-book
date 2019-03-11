import ApiService from "../common/api.service";
import {
    CreateAd, DeleteAdIfEmpty,
    RenameAd,
    UpdateAdPrice,
    UpdateAdText
} from "./actions.type";
import {
    AdCreated, 
    AdPriceUpdated,
    AdRenamed,
    AdTextUpdated,
    CurrentAdCleared
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
            {id: context.state.ad.id, text: description});
        context.commit(AdTextUpdated, description);
    },
    async [UpdateAdPrice](context, price) {
        if (price === context.state.ad.price) return;
        await ApiService.put(
            "/ad/price",
            {id: context.state.ad.id, price: price});
        context.commit(AdPriceUpdated, price);
    },
    async [DeleteAdIfEmpty](context, adId) {
        let {id, ...content} = context.state.ad;
        if (adId !== id) return;
        if (Object.keys(content).length === 0) {
            await ApiService.post("/ad/delete", {id});
        }
    }
};

const mutations = {
    [AdCreated](state, id) {
        state.ad = { id: id };
    },
    [AdRenamed](state, title) {
        state.ad.title = title;
    },
    [AdTextUpdated](state, description) {
        state.ad.description = description;
    },
    [AdPriceUpdated](state, price) {
        state.ad.price = price;
    },
    [CurrentAdCleared](state) {
        state.ad = {};
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
