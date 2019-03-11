<template>
    <v-text-field
            v-model="price"
            label="Price"
            required
            :error-messages="validatePrice"
            @blur="$v.price.$touch(); updatePrice();"
    />
</template>

<script>
    import {validationMixin} from "vuelidate";
    import {required, numeric} from "vuelidate/lib/validators";
    import {UpdateAdPrice} from "../store/actions.type";
    import store from "../store";
    
    export default {
        name: "",
        mixins: [validationMixin],
        validations: {
            price: {required, numeric}
        },
        data: () => ({
            price: null
        }),
        computed: {
            validatePrice() {
                const errors = [];
                if (!this.$v.price.$dirty) return errors;
                !this.$v.price.numeric && errors.push("Price must be a valid number");
                !this.$v.price.required && errors.push("Ad must have a price");
                return errors;
            },
        },
        methods: {
            async updatePrice() {
                if (this.validatePrice.length > 0) return;
                try {
                    await store.dispatch(UpdateAdPrice, this.price);
                } catch (e) {
                    console.log(JSON.stringify(e));
                }
            }
        },
    }
</script>

<style scoped>

</style>