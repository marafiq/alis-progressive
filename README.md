# Alis.Progressive.TagHelpers

A production-ready .NET 10 NuGet package providing progressive enhancement tag helpers with deep Alpine.js and HTMX integration.

## 🏗️ Architecture

- **Vite** for JavaScript bundling
- **Tailwind CSS** for design system
- **Alpine.js** for reactive components
- **HTMX** for server interactions
- **Tag Helpers** for declarative ASP.NET Core MVC components

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

## 📦 Components

### Layout Components
- \<alis-container>\ - Responsive container
- \<alis-grid>\ - Grid system
- \<alis-grid-item>\ - Grid items

### Form Components
- \<alis-form>\ - HTMX-enabled forms
- \<alis-form-group>\ - Form field groups
- \<alis-input>\ - Enhanced inputs with validation
- \<alis-island>\ - Alpine.js reactive islands

## 🧪 Development Rules

See [RULES.md](RULES.md) for complete development guidelines.

## 📄 License

MIT - Medtelligent

## 🔗 Inspiration

Design system inspired by [shadcn/ui](https://ui.shadcn.com/)
