import { createApp } from 'vue'
import App from './App.vue'
import { signalRService } from './SignalRClient'

const app = createApp(App)

app.mount('#app')

signalRService.connect('asdf');
