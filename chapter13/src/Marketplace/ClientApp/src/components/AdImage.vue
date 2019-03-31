<template>
    <v-image-input
            v-model="imageData"
            :image-quality="0.85"
            clearable
            image-format="jpeg"
    />
    <v-btn icon @click="test">
        <v-icon>upload</v-icon>
    </v-btn>
</template>

<script>
    import VImageInput from 'vuetify-image-input';
    import {mapActions} from "vuex";
    import {UploadAdImage} from "../store/modules/ads/actions.type";
    
    export default {
        name: "",
        components: {
            [VImageInput.name]: VImageInput
        },
        data: function () {
            return {
                imageData: {}
            }
        },
        methods: {
            test() {
                console.log(this.imageData);
            },
            async uploadImage() {
                try {
                    await this.uploadAdImage(this.imageData);
                } catch (e) {
                    console.log(JSON.stringify(e));
                }
            },
            ...mapActions("ad", {
                uploadAdImage: UploadAdImage
            })
        }
    }
</script>

<style scoped>

</style>