<template>
    <v-container>
        <v-layout text-xs-center wrap>
            <v-flex mb-4>
                <h1 class="display-2 font-weight-bold mb-3">
                    Welcome to Marketplace
                </h1>
                <p class="subheading font-weight-regular">
                    There are no published ads to see<br>
                    <router-link v-if="isAuthenticated" :to="{name: 'new'}">Publish your first ad now!</router-link>
                    <router-link v-else :to="{name: 'register'}">Register and publish your first ad now!</router-link>
                </p>
            </v-flex>
            <v-flex xs12>
                <v-img
                    :src="require('../assets/list-is-empty.png')"
                    class="my-3" contain height="400"
                ></v-img>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
    import {mapGetters} from "vuex";

    export default {
        name: "",
        computed: {
            ...mapGetters(["isAuthenticated"])
        }
    };
</script>

<style scoped>

</style>