<template>
    <v-card>
        <v-card-title primary-title>
            <v-container grid-list-md>
                <div class="headline">Increase visibility</div>
                <span>
                    That's how your app will look when published. 
                    You can increase its visibiliyty by choosing one of the services below.
                </span>
            </v-container>
        </v-card-title>
        <v-container grid-list-md>
            <AdListItem
                    :image="image"
                    :title="currentAd.title"
                    :price="currentAd.price"
                    v-bind="attributes">
            </AdListItem>
            <div v-for="service in services" :key="service.type">
                <v-switch 
                        v-model="service.enabled" 
                        :label="service.description"
                        @change="cbChange"
                />
            </div>
            <v-layout wrap>
                <v-flex md12>
                    <v-btn color="primary" @click="add">Proceed</v-btn>
                    <v-btn to="/">Cancel</v-btn>
                </v-flex>
            </v-layout>
        </v-container>
    </v-card>

</template>

<script>
    import {mapGetters, mapActions} from "vuex";
    import {FetchServices} from "../store/modules/services/actions.type";
    import AdListItem from "../components/AdListItem";

    export default {
        name: "",
        components: {
            AdListItem
        },
        async created() {
            await this.getServices();
        },
        computed: {
            ...mapGetters("ad", {
                currentAd: "currentAd"
            }),
            ...mapGetters("services", {
                services: "availableServices"
            })
        },
        data: function () {
            return {
                image: "https://cdn.vuetifyjs.com/images/cards/house.jpg",
                attributes: {
                    flex: 3
                }
            }
        },
        methods: {
            cbChange() {
                let enabled = this.services
                    .filter(x => x.enabled && x.attributes)
                    .map(x => x.attributes);
                if (enabled && enabled.length > 0) {
                    console.log(enabled);
                    let merged = Object.assign(...enabled);
                    console.log(merged);
                    this.attributes = merged;
                } else {
                    this.attributes = {flex: 3};
                }
            },
            add() {

            },
            ...mapActions("services", {
                getServices: FetchServices
            })
        }
    }
</script>

<style scoped>

</style>