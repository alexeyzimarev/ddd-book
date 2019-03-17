<template>
    <v-card>
        <v-card-title primary-title>
            <v-container grid-list-md>
                <div class="headline">New Classified Ad</div>
                <span>Fill out the details and then click the Add button.</span>
            </v-container>
        </v-card-title>
        <v-container grid-list-md>
            <v-layout wrap>
                <v-form>
                    <v-flex md12>
                        <AdTitle v-bind:ad-title="title"/>
                        <AdText v-bind:ad-text="text"/>
                        <AdPrice v-bind:ad-price="price"/>
                        <v-image-input
                                v-model="imageData"
                                :image-quality="0.85"
                                clearable
                                image-format="jpeg"
                        />
                    </v-flex>
                    <v-flex md12>
                        <v-btn color="primary" @click="add">Add</v-btn>
                        <v-btn to="/">Cancel</v-btn>
                    </v-flex>
                </v-form>
            </v-layout>
        </v-container>
        <v-snackbar bottom v-model="snackBar"/>
    </v-card>
</template>

<script>
    import VImageInput from 'vuetify-image-input';
    import {mapGetters} from "vuex";
    import {uuid} from "vue-uuid";
    import {CreateAd, DeleteAdIfEmpty} from "../store/actions.type";
    import AdTitle from "../components/AdTitle";
    import AdText from "../components/AdText";
    import AdPrice from "../components/AdPrice";
    import store from "../store";

    export default {
        components: {
            [VImageInput.name]: VImageInput,
            AdTitle,
            AdText,
            AdPrice
        },
        data: () => ({
            title: null,
            text: null,
            price: null,
            imageData: "",
            snackBar: {}
        }),
        computed: {
            ...mapGetters(["currentAd"])
        },
        methods: {
            add() {
                
            }
        },
        async beforeRouteEnter(to, from, next) {
            let adId = uuid.v1();
            await store.dispatch(CreateAd, adId);
            return next();
        },
        async beforeRouteLeave(to, from, next) {
            await store.dispatch(DeleteAdIfEmpty, this.currentAd.id);
            next();
        }
    }
</script>

<style scoped>

</style>