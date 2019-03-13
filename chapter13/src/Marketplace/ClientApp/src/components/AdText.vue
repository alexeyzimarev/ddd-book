<template>
    <v-textarea
            v-model="text"
            label="Description"
            required
            :error-messages="validateText"
            @blur="$v.text.$touch(); updateText();"
    />
</template>

<script>
    import {validationMixin} from "vuelidate";
    import {required, minLength} from "vuelidate/lib/validators";
    import {UpdateAdText} from "../store/actions.type";
    import store from "../store";
    
    export default {
        name: "",
        mixins: [validationMixin],
        props:{
            adText: String
        },
        validations: {
            text: {required, minLength: minLength(10)}
        },
        data: () => ({
            text: this.adText
        }),
        computed: {
            validateText() {
                const errors = [];
                if (!this.$v.text.$dirty) return errors;
                !this.$v.text.minLength && errors.push("Please put at least 10 characters");
                !this.$v.text.required && errors.push("Ad text is required.");
                return errors;
            },
        },
        methods: {
            async updateText() {
                if (this.validateText.length > 0) return;
                try {
                    await store.dispatch(UpdateAdText, this.text);
                } catch (e) {
                    console.log(JSON.stringify(e));
                }
            }
        },
    }
</script>

<style scoped>

</style>