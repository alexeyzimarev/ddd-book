<template>
    <v-app>
        <v-toolbar app>
            <v-toolbar-title class="headline text-uppercase">
                <span>Marketplace</span>
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn
                    flat
                    href="https://github.com/vuetifyjs/vuetify/releases/latest"
                    target="_blank"
            >
                <span class="mr-2">Domain-Driven Design</span>
            </v-btn>
        </v-toolbar>

        <v-content>
            <v-container fluid>
                <router-view></router-view>
                <HelloWorld></HelloWorld>
            </v-container>
        </v-content>
    </v-app>
</template>

<script>
    import HelloWorld from './components/HelloWorld'

    export default {
        name: 'App',
        components: {
            HelloWorld
        },
        data() {
            return {
                //
            }
        }
    }
</script>
