<template>
    <v-layout row wrap>
        <v-flex xs6>
            <v-image-input
                    v-model="imageData"
                    :image-quality="0.85"
                    clearable
                    image-format="jpeg"
            ></v-image-input>
            <br/>
            <v-btn
                    color="primary darken-1"
                    :disabled="!imageData"
                    @click="uploadImage">
                Upload
            </v-btn>
        </v-flex>
        <v-flex xs6>
            <v-carousel>
                <v-carousel-item
                        v-for="image in currentAd.images"
                        :key="image.key"
                        :src="image.image"
                ></v-carousel-item>
            </v-carousel>
        </v-flex>
    </v-layout>
</template>

<script>
    import VImageInput from 'vuetify-image-input';
    import {mapActions, mapGetters} from "vuex";
    import {UploadAdImage} from "../store/modules/ads/actions.type";

    export default {
        name: "",
        components: {
            [VImageInput.name]: VImageInput
        },
        data: function () {
            return {
                imageData: null,
            }
        },
        computed: {
            ...mapGetters("ad", {
                currentAd: "currentAd"
            }),
        },
        methods: {
            async uploadImage() {
                await this.uploadAdImage(this.imageData);
            },
            ...mapActions("ad", {
                uploadAdImage: UploadAdImage
            })
        }
    }
</script>

<style scoped>

</style>