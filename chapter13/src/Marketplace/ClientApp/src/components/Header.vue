<template>
    <!--<v-toolbar app color="primary">-->
    <!--<v-toolbar-title class="headline text-uppercase">-->
    <!--<v-btn color="primary" icon :to="{ name: 'home' }">-->
    <!--<v-icon>home</v-icon>-->
    <!--</v-btn>-->
    <!--<span>Marketplace</span>-->
    <!--</v-toolbar-title>-->
    <!--<v-spacer></v-spacer>-->
    <!--<NewAdButton v-if="isAuthenticated"/>-->
    <!--<v-btn v-else color="primary darken-1" :to="{ name: 'login' }">-->
    <!--Sign-in-->
    <!--</v-btn>-->
    <!--<span class="headline">{{ currentUser.displayName }}</span>-->
    <!--<v-menu bottom left>-->
    <!--<template v-slot:activator="{ on }">-->
    <!--<v-btn v-on="on" icon>-->
    <!--<v-icon>more_vert</v-icon>-->
    <!--</v-btn>-->
    <!--</template>-->
    <!--<v-list>-->
    <!--<v-list-tile @click="">-->
    <!--<v-list-tile-title>Your ads</v-list-tile-title>-->
    <!--<v-list-tile-title>Your orders</v-list-tile-title>-->
    <!--</v-list-tile>-->
    <!--</v-list>-->
    <!--</v-menu>-->
    <!--</v-toolbar>-->
    <v-layout row>
        <v-flex xs12 sm6 offset-sm3>
            <v-card height="200px">
                <v-card-title class="blue white--text">
                    <span class="headline">Menu</span>

                    <v-spacer></v-spacer>

                    <v-menu bottom left>
                        <template v-slot:activator="{ on }">
                            <v-btn
                                    dark
                                    icon
                                    v-on="on"
                            >
                                <v-icon>more_vert</v-icon>
                            </v-btn>
                        </template>

                        <v-list>
                            <v-list-tile
                                    v-for="(item, i) in items"
                                    :key="i"
                                    @click=""
                            >
                                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                            </v-list-tile>
                        </v-list>
                    </v-menu>
                </v-card-title>

                <v-card-text>Lorem Ipsum</v-card-text>
            </v-card>
        </v-flex>
    </v-layout>
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
        },
        data: () => ({
            items: [
                {title: 'Click Me'},
                {title: 'Click Me'},
                {title: 'Click Me'},
                {title: 'Click Me 2'}
            ]
        })
    };
</script>

<style scoped>

</style>