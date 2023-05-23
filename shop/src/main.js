import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const globalApi={
    data(){
        return{
            base_url:'https://localhost:7133/api/'
        }
    }
}
const app=createApp(App).use(router)
app.mixin(globalApi)
app.mount('#app')
