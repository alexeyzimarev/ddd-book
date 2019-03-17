<template>
    <v-card>
        <v-img
                :src="image"
                height="200px"
        >
            <v-container
                    fill-height
                    fluid
                    pa-2
            >
                <v-layout fill-height>
                    <v-flex xs12 align-end flexbox>
                        <span class="headline white--text"
                              v-text="title"></span>
                    </v-flex>
                </v-layout>
            </v-container>
        </v-img>
        <v-card-title primary-title>
            <div>
                <div class="headline">{{ title }}</div>
                <span class="grey--text">{{ price }}</span>
            </div>
        </v-card-title>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn icon>
                <v-icon>favorite</v-icon>
            </v-btn>
            <v-btn icon>
                <v-icon>bookmark</v-icon>
            </v-btn>
            <v-btn icon>
                <v-icon>share</v-icon>
            </v-btn>
        </v-card-actions>
    </v-card>

</template>

<script>
    export default {
        name: "",
        props: {
            image: String,
            title: String,
            price: Number
        }
    }
</script>

<style scoped>

</style>