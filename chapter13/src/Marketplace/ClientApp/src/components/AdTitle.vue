<template>
    <v-text-field
            v-model="title"
            label="Title"
            required
            :error-messages="validateTitle"
            @blur="$v.title.$touch(); setTitle();"
    />
</template>

<script>
    import {validationMixin} from "vuelidate";
    import {required, minLength} from "vuelidate/lib/validators";
    import {RenameAd} from "../store/actions.type";
    import store from "../store";

    export default {
        name: "",
        mixins: [validationMixin],
        props: {
            adTitle : String
        },
        validations: {
            title: {required, minLength: minLength(10)}
        },
        data: function () {
            return {
                title: this.adTitle
            }
        },
        computed: {
            validateTitle() {
                const errors = [];
                if (!this.$v.title.$dirty) return errors;
                !this.$v.title.minLength && errors.push("Please put at least 10 characters");
                !this.$v.title.required && errors.push("Ad title is required.");
                return errors;
            },
        },
        methods: {
            async setTitle() {
                if (this.validateTitle.length > 0) return;
                try {
                    await store.dispatch(RenameAd, this.title);
                } catch (e) {
                    console.log(e.response.data.error);
                }
            }
        },
    }
</script>

<style scoped>

</style>