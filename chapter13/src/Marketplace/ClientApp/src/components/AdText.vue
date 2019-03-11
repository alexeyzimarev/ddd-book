<template>
    <v-textarea
            v-model="description"
            label="Description"
            required
            :error-messages="validateText"
            @blur="$v.description.$touch(); updateText();"
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
        validations: {
            description: {required, minLength: minLength(10)}
        },
        data: () => ({
            description: ""
        }),
        computed: {
            validateText() {
                const errors = [];
                if (!this.$v.description.$dirty) return errors;
                !this.$v.description.minLength && errors.push("Please put at least 10 characters");
                !this.$v.description.required && errors.push("Ad text is required.");
                return errors;
            },
        },
        methods: {
            async updateText() {
                if (this.validateText.length > 0) return;
                try {
                    await store.dispatch(UpdateAdText, this.description);
                } catch (e) {
                    console.log(JSON.stringify(e));
                }
            }
        },
    }
</script>

<style scoped>

</style>