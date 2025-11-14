# Alis.Progressive.TagHelpers

A production-ready .NET 10 NuGet package providing progressive enhancement tag helpers with deep Alpine.js and HTMX integration.

## 🏗️ Architecture

### Core Stack (Proven in Jamidon)
- **HTMX** for declarative server communication
- **Alpine.js** for reactive components (no additional state management libraries)
- **Pure JavaScript Validation** (no external validation libraries like v8n or jQuery Validate)
- **Tailwind CSS** for design system
- **Tag Helpers** for declarative ASP.NET Core MVC components

### Build Tools
- **Vite** for JavaScript bundling
- **.NET 10** for server-side rendering and validation

## 🚀 Getting Started

```bash
# Build client assets
cd src/Alis.Progressive.TagHelpers/client
npm install
npm run build

# Run SandboxApp
cd ../Alis.Progressive.SandboxApp
dotnet run

# Run tests
dotnet test
```

## 📦 Components (Planned)

### Core Tag Helpers
- \<alis-scripts>\ - Framework scripts loader (HTMX + Alpine + Validation)
- \<alis-styles>\ - Tailwind CSS bundle loader

### Layout Components
- \<alis-container>\ - Responsive container
- \<alis-grid>\ - Grid system
- \<alis-grid-item>\ - Grid items

### Form Components
- \<alis-form>\ - HTMX-enabled forms with automatic validation
- \<alis-form-group>\ - Form field groups
- \<alis-input>\ - Enhanced inputs with validation
- \<alis-textarea>\ - Enhanced textareas
- \<alis-select>\ - Enhanced select dropdowns

## 🧪 Development Rules

See [RULES.md](RULES.md) for complete development guidelines.

## 📄 License

MIT - Medtelligent

## 🔗 Inspiration

Design system inspired by [shadcn/ui](https://ui.shadcn.com/)
