<template>
    <v-toolbar app color="primary">
        <v-toolbar-title class="headline text-uppercase">
            <v-btn color="primary" icon :to="{ name: 'home' }">
                <v-icon>home</v-icon>
            </v-btn>
            <span>Marketplace</span>
        </v-toolbar-title>
        <v-spacer></v-spacer>
        <NewAdButton v-if="isAuthenticated" />
        <v-btn v-else color="primary darken-1" :to="{ name: 'login' }">
            Sign-in
        </v-btn>
        <v-btn
                flat
                href="https://github.com/vuetifyjs/vuetify/releases/latest"
                target="_blank"
        >
            <span class="mr-2">{{ currentUser.displayName }}</span>
        </v-btn>
    </v-toolbar>
</template>

<script>
    import NewAdButton from "../components/NewAdButton";
    import {mapGetters} from "vuex";

    export default {
        name: "",
        components: {
            NewAdButton
        },
        computed: {
            ...mapGetters(["currentUser", "isAuthenticated"])
        }
    };
</script>

<style scoped>

</style>