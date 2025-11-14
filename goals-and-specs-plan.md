<!-- e727baf2-ce80-4cbb-8d7c-8635f4167751 b54e2e8a-b92f-4a8e-b92d-b4ab23113716 -->
# Alis.Progressive.TagHelpers Solution

## Core Principles (RULES.md)

**RULE 1: Test-First Development**

- ALL tests must pass before moving to next component
- No new code without tests
- Run full test suite after every change

**RULE 2: Component Progression**

- Phase 1: Foundation (Scripts, Styles, Base CSS)
- Phase 2: Container Components (Layout building blocks)
- Phase 3: Grid Components (Responsive layouts)
- Phase 4: Form Components (Deep Alpine/HTMX integration)
- Phase 5: Validation Integration (Complete validation system)

**RULE 3: Deep Alpine/HTMX Integration**

- All components designed to work with Alpine directives
- HTMX attributes supported where applicable
- Composable and declarative

**RULE 4: Incremental Building**

- Prove each component in isolation
- Test manually + unit tests + E2E tests
- ALL tests pass before next component

## Solution Structure

```
alis-progressive/
├── RULES.md
├── src/
│   ├── Alis.Progressive.TagHelpers/
│   │   ├── client/
│   │   │   ├── src/
│   │   │   │   ├── js/
│   │   │   │   │   ├── validation.js
│   │   │   │   │   ├── conditional-validators.js
│   │   │   │   │   ├── framework-extension.js
│   │   │   │   │   └── main.js
│   │   │   │   └── css/
│   │   │   │       ├── design-system.css
│   │   │   │       └── alis.css
│   │   │   ├── vite.config.js
│   │   │   ├── tailwind.config.js
│   │   │   └── package.json
│   │   ├── TagHelpers/
│   │   │   ├── Core/
│   │   │   │   ├── AlisScriptsTagHelper.cs
│   │   │   │   └── AlisStylesTagHelper.cs
│   │   │   ├── Layout/
│   │   │   │   ├── AlisContainerTagHelper.cs
│   │   │   │   ├── AlisGridTagHelper.cs
│   │   │   │   └── AlisGridItemTagHelper.cs
│   │   │   └── Forms/
│   │   │       ├── AlisFormTagHelper.cs
│   │   │       ├── AlisFormGroupTagHelper.cs
│   │   │       ├── AlisInputTagHelper.cs
│   │   │       └── AlisIslandTagHelper.cs
│   │   ├── Validation/
│   │   │   ├── ConditionalValidationAttribute.cs
│   │   │   ├── RequiredIfAttribute.cs
│   │   │   ├── RequiredUnlessAttribute.cs
│   │   │   └── ConditionalValidationAttributeAdapter.cs
│   │   └── Alis.Progressive.TagHelpers.csproj
│   └── Alis.Progressive.SandboxApp/
│       └── ...
└── tests/
    ├── Alis.Progressive.TagHelpers.Tests/
    └── Alis.Progressive.TagHelpers.Playwright/
```

## Design System (shadcn/ui inspired)

**client/tailwind.config.js:**

```js
export default {
  content: ['../**/*.cs', './src/**/*.{js,css}'],
  theme: {
    container: {
      center: true,
      padding: '2rem',
      screens: {
        '2xl': '1400px'
      }
    },
    extend: {
      colors: {
        border: 'hsl(214.3 31.8% 91.4%)',
        input: 'hsl(214.3 31.8% 91.4%)',
        ring: 'hsl(222.2 84% 4.9%)',
        background: 'hsl(0 0% 100%)',
        foreground: 'hsl(222.2 84% 4.9%)',
        primary: {
          DEFAULT: 'hsl(222.2 47.4% 11.2%)',
          foreground: 'hsl(210 40% 98%)'
        },
        destructive: {
          DEFAULT: 'hsl(0 84.2% 60.2%)',
          foreground: 'hsl(210 40% 98%)'
        }
      },
      borderRadius: {
        lg: '0.5rem',
        md: 'calc(0.5rem - 2px)',
        sm: 'calc(0.5rem - 4px)'
      }
    }
  }
};
```

**client/src/css/design-system.css:**

```css
@layer base {
  * {
    @apply border-border;
  }
  body {
    @apply bg-background text-foreground;
  }
}

@layer components {
  /* Container Components */
  .alis-container {
    @apply container mx-auto px-4 sm:px-6 lg:px-8;
  }
  
  .alis-container-fluid {
    @apply w-full px-4 sm:px-6 lg:px-8;
  }
  
  /* Grid Components */
  .alis-grid {
    @apply grid gap-4;
  }
  
  .alis-grid-cols-1 { @apply grid-cols-1; }
  .alis-grid-cols-2 { @apply grid-cols-2; }
  .alis-grid-cols-3 { @apply grid-cols-3; }
  .alis-grid-cols-4 { @apply grid-cols-4; }
  
  /* Form Components */
  .alis-form {
    @apply space-y-6;
  }
  
  .alis-form-group {
    @apply space-y-2;
  }
  
  .alis-label {
    @apply text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70;
  }
  
  .alis-input {
    @apply flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm
           ring-offset-background
           placeholder:text-muted-foreground
           focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring
           disabled:cursor-not-allowed disabled:opacity-50;
  }
  
  .alis-error {
    @apply text-sm font-medium text-destructive;
  }
}
```

## Component Development Phases

### PHASE 0: Foundation Setup

**0.1: Create RULES.md**

**0.2: Create solution structure (4 projects)**

**0.3: Setup client build (package.json, vite, tailwind)**

**0.4: Copy core JavaScript from Jamidon**

**0.5: Copy validation attributes**

**CHECKPOINT: `dotnet build` must succeed**

### PHASE 1: Core Tag Helpers

**1.1: AlisScriptsTagHelper**

- Unit test: Renders `<script src="~/lib/alis/alis.bundle.js"></script>`
- Implementation
- Test: Unit test passes

**1.2: AlisStylesTagHelper**

- Unit test: Renders `<link rel="stylesheet" href="~/lib/alis/alis.bundle.css">`
- Implementation
- Test: Unit test passes

**1.3: Setup SandboxApp Base**

- _Layout.cshtml with `<alis-scripts />` and `<alis-styles />`
- _ViewImports.cshtml
- Program.cs with friendly routes
- Index page
- Test: App runs, bundles load

**CHECKPOINT: All unit tests pass, SandboxApp runs**

### PHASE 2: Container Components (Layout Foundation)

**2.1: AlisContainerTagHelper**

Design:

```html
<!-- Basic container -->
<alis-container>
    Content here
</alis-container>

<!-- Fluid container -->
<alis-container fluid="true">
    Full width content
</alis-container>

<!-- Alpine integration -->
<alis-container x-data="{ visible: true }" x-show="visible">
    Conditionally shown
</alis-container>
```

Properties:

- `Fluid` (bool): Full width or constrained
- Passes through all Alpine/HTMX attributes (x-*, hx-*)

Unit Tests:

- Renders div with alis-container class
- Fluid mode adds alis-container-fluid
- Preserves Alpine attributes
- Preserves HTMX attributes

Implementation Steps:

1. Write unit tests
2. Implement tag helper
3. Create Sample: Container showcase
4. Manual browser test
5. Create Playwright test
6. ALL TESTS MUST PASS

**2.2: Create Container Sample**

- Views/Samples/Container.cshtml
- Show basic, fluid, with Alpine
- Route: /samples/container

**CHECKPOINT: All tests pass including new Playwright test**

### PHASE 3: Grid Components (Layout System)

**3.1: AlisGridTagHelper**

Design:

```html
<!-- Basic grid -->
<alis-grid cols="3">
    <div>Item 1</div>
    <div>Item 2</div>
    <div>Item 3</div>
</alis-grid>

<!-- Responsive grid -->
<alis-grid cols="1" sm-cols="2" lg-cols="4">
    Content
</alis-grid>

<!-- With gap -->
<alis-grid cols="2" gap="lg">
    Content
</alis-grid>

<!-- Alpine integration -->
<alis-grid cols="3" x-data="{ items: [] }">
    <template x-for="item in items">
        <div x-text="item"></div>
    </template>
</alis-grid>
```

Properties:

- `Cols` (int): Default columns (1-12)
- `SmCols`, `MdCols`, `LgCols` (int): Responsive breakpoints
- `Gap` (string): xs, sm, md, lg, xl
- Alpine/HTMX passthrough

**3.2: AlisGridItemTagHelper**

Design:

```html
<alis-grid cols="4">
    <alis-grid-item span="2">
        Spans 2 columns
    </alis-grid-item>
    <alis-grid-item>
        Default span
    </alis-grid-item>
</alis-grid>
```

Properties:

- `Span` (int): Column span (1-12)
- `Order` (int): Visual order

Implementation Steps (for each):

1. Write comprehensive unit tests
2. Implement tag helper
3. Create Grid sample showcasing all features
4. Manual browser test
5. Playwright E2E test
6. ALL TESTS MUST PASS

**3.3: Create Grid Sample**

- Views/Samples/Grid.cshtml
- Show responsive grids, spans, Alpine integration
- Route: /samples/grid

**CHECKPOINT: All tests pass (unit + E2E for Container + Grid)**

### PHASE 4: Form Components (Deep Integration)

**4.1: AlisFormTagHelper**

Design:

```html
<!-- HTMX form -->
<alis-form hx-post="/api/submit" hx-target="#result">
    Form content
</alis-form>

<!-- With validation -->
<alis-form hx-post="/api/submit" validate="true">
    Form content
</alis-form>

<!-- Alpine state -->
<alis-form hx-post="/api/submit" x-data="{ loading: false }">
    Form content
</alis-form>
```

Properties:

- `HxMethod`: post, get, put, delete, patch
- `HxAction`: URL
- `HxTarget`: CSS selector
- `HxSwap`: innerHTML, outerHTML, etc.
- `Validate`: Enable client-side validation
- `HxBefore`, `HxSuccess`, `HxError`: Behavior hooks
- Alpine/HTMX passthrough

**4.2: AlisFormGroupTagHelper**

Design:

```html
<alis-form-group>
    <label asp-for="Email"></label>
    <input asp-for="Email" />
    <span asp-validation-for="Email"></span>
</alis-form-group>
```

Renders:

```html
<div class="alis-form-group">
    <label class="alis-label">Email</label>
    <input class="alis-input" />
    <span class="alis-error"></span>
</div>
```

**4.3: AlisInputTagHelper**

Design:

```html
<!-- Enhances standard input -->
<alis-input asp-for="Email" />

<!-- With Alpine -->
<alis-input asp-for="Email" x-model="email" />

<!-- With HTMX -->
<alis-input asp-for="Search" 
            hx-get="/api/search" 
            hx-trigger="keyup changed delay:500ms" 
            hx-target="#results" />
```

Auto-adds:

- alis-input class
- Validation attributes from model
- Integrates with validation system

Implementation Steps (for each):

1. Write comprehensive unit tests
2. Implement with Alpine/HTMX integration
3. Create Form sample with all features
4. Manual browser test
5. Playwright E2E test covering validation
6. ALL TESTS MUST PASS

**4.4: Create Form Samples**

- Views/Samples/Forms.cshtml (basic forms)
- Views/Samples/FormValidation.cshtml (with validation)
- Views/Samples/FormHtmx.cshtml (HTMX integration)
- Routes: /samples/forms, /samples/form-validation, /samples/form-htmx

**CHECKPOINT: All tests pass (Container + Grid + Forms)**

### PHASE 5: Island Component (Advanced)

**5.1: AlisIslandTagHelper**

Copy from Jamidon/TagHelpers/IslandTagHelper.cs:

- Rename to AlisIslandTagHelper
- Update to alis-island
- Deep Alpine integration (store injection)
- HTMX support for island re-rendering

Design:

```html
<alis-island>
    <script type="application/json" data-store>
    { "count": 0 }
    </script>
    <div x-data="islandStore">
        <span x-text="count"></span>
        <button @click="count++">Increment</button>
    </div>
</alis-island>
```

**5.2: Create Island Sample**

- Views/Samples/Island.cshtml
- Route: /samples/island

**CHECKPOINT: All tests pass**

### PHASE 6: Validation Integration

**6.1: Copy remaining samples from Jamidon**

- Update to use new component structure
- One sample at a time
- Test after each

**6.2: Validation unit tests**

- RequiredIfAttribute
- RequiredUnlessAttribute

**6.3: Conditional validation E2E tests**

**FINAL CHECKPOINT: Run complete test suite**

- Unit tests: 30+ (all components + validation)
- E2E tests: 15+ (each sample)
- ALL MUST PASS

## Test Progression Rules

After each component:

```bash
# 1. Unit tests for new component
dotnet test tests/Alis.Progressive.TagHelpers.Tests --filter "ClassName~NewComponent"

# 2. All unit tests
dotnet test tests/Alis.Progressive.TagHelpers.Tests

# 3. Manual test in browser
cd src/Alis.Progressive.SandboxApp && dotnet run

# 4. E2E test for new sample
dotnet test tests/Alis.Progressive.TagHelpers.Playwright --filter "TestName~NewSample"

# 5. All E2E tests
dotnet test tests/Alis.Progressive.TagHelpers.Playwright

# ALL MUST PASS BEFORE NEXT COMPONENT
```

## Component Priority Order

1. AlisScriptsTagHelper ✓
2. AlisStylesTagHelper ✓
3. AlisContainerTagHelper ← START HERE
4. AlisGridTagHelper
5. AlisGridItemTagHelper
6. AlisFormTagHelper
7. AlisFormGroupTagHelper
8. AlisInputTagHelper
9. AlisIslandTagHelper
10. Remaining samples + validation

Each fully tested before moving to next!

### To-dos

- [ ] Research jQuery unobtrusive validation patterns and v8n API
- [ ] Download v8n library locally and add to project structure
- [ ] Create comprehensive validation.js using v8n with proper patterns from jQuery unobtrusive
- [ ] Update all views to load v8n synchronously in head
- [ ] Ensure framework-extension.js properly integrates with new validation
- [ ] ALL 36 TESTS PASSING - Production-grade validation with v8n complete
- [ ] Fix Sample 12 conditional visibility validation
- [ ] Document the final validation architecture in code comments
- [ ] 31/36 tests passing - 5 tests failing (Sample 4 Problem Details, Sample 7 MinSelection, Sample 12 conditional visibility x3)
- [ ] All CSS fixes applied to sample views
- [ ] Fix Sample 4: Problem Details handler not showing errors from server
- [ ] Fix Sample 12: Hidden field validation and form submission
- [ ] Replace v8n with pure JavaScript validation - COMPLETED! 30/36 tests passing
- [ ] Fix Alpine validation event attachment - COMPLETED with pure JS
- [ ] Verify Alpine validation blur/input handlers are firing correctly - COMPLETED
- [ ] Remote validation implemented successfully! 34/36 tests passing
- [ ] Pure JavaScript validation working great - 89% pass rate
- [ ] MinSelection validation removed from codebase per requirements
- [ ] Fixed Password multi-validator test - issue was HTML5 maxlength preventing test scenario
- [ ] Fixed regex validator - now explicitly handled in switch statement
- [ ] 🎉 ALL 35/35 TESTS PASSING (excluding MinSelection per requirements)
- [ ] ✅ Created ValidationReadMe.md showcasing pure JavaScript power