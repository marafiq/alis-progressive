# Alis.Progressive.TagHelpers Development Rules

## Core Principles

### RULE 1: Test-First Development
- ALL tests must pass before moving to next component
- No new code without tests
- Run full test suite after every change

### RULE 2: Component Progression
1. Phase 1: Foundation (Scripts, Styles, Base CSS)
2. Phase 2: Container Components (Layout building blocks)
3. Phase 3: Grid Components (Responsive layouts)
4. Phase 4: Form Components (Deep Alpine/HTMX integration)
5. Phase 5: Validation Integration (Complete validation system)

### RULE 3: Deep Alpine/HTMX Integration
- All components designed to work with Alpine directives
- HTMX attributes supported where applicable
- Composable and declarative

### RULE 4: Incremental Building
- Prove each component in isolation
- Test manually + unit tests + E2E tests
- ALL tests pass before next component

### RULE 5: Breaking Changes
- If a test fails, fix immediately
- Never commit broken tests
- Rollback if necessary

## Test Progression

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

1. AlisScriptsTagHelper
2. AlisStylesTagHelper
3. AlisContainerTagHelper
4. AlisGridTagHelper
5. AlisGridItemTagHelper
6. AlisFormTagHelper
7. AlisFormGroupTagHelper
8. AlisInputTagHelper
9. AlisIslandTagHelper
10. Remaining samples + validation

Each fully tested before moving to next!

## Design Inspiration

- shadcn/ui: https://ui.shadcn.com/
- Composition patterns
- Utility-first Tailwind approach
- Accessible by default
