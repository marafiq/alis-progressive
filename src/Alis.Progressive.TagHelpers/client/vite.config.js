import { defineConfig } from 'vite';
import { resolve } from 'path';

export default defineConfig({
  build: {
    lib: {
      entry: resolve(__dirname, 'src/js/main.js'),
      name: 'Alis',
      fileName: 'alis.bundle',
      formats: ['iife']
    },
    outDir: '../Resources/js',
    emptyOutDir: false,
    minify: 'terser',
    rollupOptions: {
      output: {
        assetFileNames: '../css/[name].bundle[extname]'
      }
    }
  }
});

