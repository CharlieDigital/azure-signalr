import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    https: false,
    cors: {
      origin: "http://localhost:3000",
      methods: "GET,POST,PUT,PATCH,DELETE,OPTIONS"
    }
  },
})
