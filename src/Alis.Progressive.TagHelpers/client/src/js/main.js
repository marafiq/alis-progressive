// Alis.Progressive.TagHelpers - Main Entry Point
// This file bundles HTMX, Alpine.js, and all Alis framework code into a single bundle

// Import from npm packages
import 'htmx.org';
import Alpine from 'alpinejs';

// Initialize Alpine
window.Alpine = Alpine;
Alpine.start();

// Import Alis framework modules
import './validation.js';
import './conditional-validators.js';
import './framework-extension.js';

// Import Tailwind CSS
import '../css/alis.css';

// Export public API (optional - for direct JavaScript access)
export { validators, addRule } from './validation.js';

console.log('[Alis] Framework loaded successfully');

