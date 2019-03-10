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
                        <v-text-field
                                v-model="title"
                                label="Title"
                                @blur="setTitle"
                        />
                        <!--:rules="updateTitle"-->
                        <!--validate-on-blur-->
                        <v-textarea
                                v-model="description"
                                label="Description"
                                :rules="updateDescription"
                                validate-on-blur
                        />
                        <v-text-field v-model="title" label="Price"></v-text-field>
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
    import {mapGetters, mapState} from "vuex";
    import {uuid} from 'vue-uuid';
    import {CreateAd, RenameAd, UpdateAdText} from "../store/actions.type";
    import store from "../store";

    export default {
        components: {
            [VImageInput.name]: VImageInput,
        },
        data: () => ({
            title: null,
            description: null,
            imageData: null,
            snackBar: {},
            updateTitle: [(title) => {
                if (!title) return false;
            }],
            updateDescription: [
                // async (text) => await store.dispatch(UpdateAdText, text) 
            ]
        }),
        computed: {
            ...mapGetters(["currentAd"])
        },
        methods: {
            async setTitle() {
                await store.dispatch(RenameAd, title);
            },
            add() {
                
            }
        },
        async beforeRouteEnter(to, from, next) {
            let adId = uuid.v1();
            await store.dispatch(CreateAd, adId);
            return next();
        },
        beforeRouteUpdate(to, from, next) {
            console.log("enter");
            next();
        }
    }
</script>

<style scoped>

</style>