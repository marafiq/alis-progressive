# Master Plan: Progressive .NET 10 MVC Framework with HTMX

## ✅ Proven Implementation Status

**ALL 22 PLAYWRIGHT TESTS PASSING** - The framework has been validated with a real .NET 10 MVC implementation (Jamidon project) with automated end-to-end tests.

### Working .NET 10 Implementation

The Jamidon project in this repository demonstrates all framework concepts with real ASP.NET Core MVC:

1. **Sample 1: Basic Island** - Counter with Alpine.js state management
2. **Sample 2: Login Form** - Automatic `data-val-*` validation with server integration
3. **Sample 3: HTMX Behavior Hooks** - Multiple comma-separated lifecycle hooks
4. **Sample 4: Problem Details** - Server 400 errors mapped to form fields
5. **Sample 5: Server State Update** - Dynamic content with state payload
6. **Sample 6: Registration Form** - Cross-field validation (password confirmation)
7. **Sample 7: Comprehensive Validators** - All built-in validators
8. **Sample 8: Island Re-render** - HTMX form submission and island re-render
9. **Sample 9: Remote Validation (HTMX)** - Async validation using HTMX native features
10. **Sample 10: Remote Validation ([Remote])** - Async validation using ASP.NET `[Remote]` attribute
11. **Sample 11: Price Validation** - Decimal validation with range
12. **Sample 12: Conditional Validation** - RequiredIf/RequiredUnless with Alpine.js visibility

### Key Validated Features

✅ **HTMX Behavior Hooks** - `data-hx-before`, `data-hx-success`, `data-hx-error` with comma-separated functions  
✅ **Automatic Validation** - Pure JavaScript `data-val-*` validation, no manual code per form  
✅ **Problem Details Handling** - RFC 7807 format, automatic field mapping on 400 errors  
✅ **Server State Updates** - `updateFromServer(payload)` pattern for dynamic state  
✅ **Cross-Field Validation** - `data-val-equalto-other` for password confirmation  
✅ **Alpine.js Integration** - Direct Alpine state management in islands  
✅ **Remote Validation** - Both HTMX native and ASP.NET `[Remote]` attribute patterns  
✅ **Conditional Validation** - Client and server-side RequiredIf/RequiredUnless attributes  
✅ **Production-Grade Code** - 22 passing Playwright tests, clean architecture

---

## Executive Summary

This framework provides a progressive enhancement approach to building modern web applications using .NET 10's Static Rendering MVC Views with HTMX for declarative client-server communication. The architecture emphasizes simplicity, declarative patterns, and minimal JavaScript.

### Core Philosophy

- **Progressive Enhancement**: Start with static HTML, add interactivity only where needed
- **Declarative Over Imperative**: Use HTML attributes to define behavior, not JavaScript
- **Server-First**: Leverage .NET 10's static rendering capabilities
- **Simple State Management**: Alpine.js for reactive components, no additional libraries
- **Bidirectional Validation**: Client-side validation for UX, server-side validation for security
- **Pure JavaScript**: No external validation libraries, production-grade vanilla JS

## Architecture Goals

### 1. Static Rendering with Selective Interactivity

- Leverage .NET 10's static rendering for initial page load performance
- Use Island Architecture pattern for isolated interactive components
- Minimize JavaScript bundle size by using declarative HTMX attributes
- Maintain SEO benefits of server-rendered content

### 2. Declarative Client-Server Communication

- All client-server communication through HTMX attributes
- Use built-in HTMX methods: `hx-get`, `hx-post`, `hx-put`, `hx-delete`, `hx-patch`
- No middleware needed - HTMX handles methods natively
- Custom behavior hooks for lifecycle management (comma-separated)

### 3. Simple State Management with Alpine.js

- Alpine.js for reactive state management within components
- Use `x-data` for component-local state
- Declarative reactivity with Alpine directives (`x-show`, `x-text`, `x-model`, etc.)
- No additional state management libraries needed

### 4. Unified Validation System

- Client-side validation using `data-val-*` attributes (pure JavaScript)
- Simple, spot-on validation - do not validate hidden fields
- Server-side validation returning 400 with Problem Details (.NET 10 feature)
- Conditional validation with RequiredIf/RequiredUnless attributes
- Unified error display from both validation sources

### 5. Developer Experience

- Type-safe server-side code with C#
- Declarative HTML with minimal JavaScript
- Tag Helpers for common patterns
- Composable component architecture

## Technical Specifications

### HTMX Integration

#### Core Features

- **Declarative Attributes**: All interactions defined via HTML attributes
  - `hx-get`, `hx-post`, `hx-put`, `hx-delete`, `hx-patch` for HTTP methods (built-in)
  - `hx-target` for response targeting
  - `hx-swap` for content replacement strategies
  - `hx-trigger` for event customization

- **Custom Behavior Hooks via HTMX Extension**: Lifecycle management via data attributes
  - `data-hx-before`: Execute before HTMX request (supports multiple comma-separated functions)
  - `data-hx-success`: Execute on successful response (supports multiple comma-separated functions)
  - `data-hx-error`: Execute on error response (supports multiple comma-separated functions)

- **Response Handling**: 
  - Partial view responses for HTMX requests
  - Problem Details for 400 validation responses (RFC 7807)

#### HTMX Extension

Custom HTMX extension (`framework-extension.js`) provides:
1. Behavior hooks (before, success, error) with multiple function support
2. Automatic Problem Details handling
3. Automatic validation attachment after DOM swaps
4. Prevention of form submission when client-side validation fails

### Alpine.js Components

Alpine.js provides reactive components with simple `x-data` for state management:

**Usage** (in Razor view):
```html
<div x-data="{ count: 0, step: 1 }">
    <p>Count: <span x-text="count"></span></p>
    <p>Step: <span x-text="step"></span></p>
    <button @click="count += step">+</button>
    <button @click="count -= step">-</button>
</div>
```

**For Conditional Visibility**:
```html
<form x-data="{ acceptTerms: false }">
    <input type="checkbox" asp-for="AcceptTerms" x-model="acceptTerms" />
    
    <div x-show="acceptTerms" x-transition>
        <input asp-for="PhoneNumber" 
               x-bind:data-should-validate="acceptTerms" />
        <span asp-validation-for="PhoneNumber"></span>
    </div>
</form>
```

### Validation System

#### Pure JavaScript Validation

**Client-Side**:
- `validation.js` - Pure JavaScript implementation
- Follows jQuery unobtrusive validation patterns
- Supports all ASP.NET Core `data-val-*` attributes
- Extensible with `addRule()` function
- Automatic validation attachment
- Smart field detection (skips hidden fields)

**Server-Side**:
- ASP.NET Core DataAnnotations
- Custom conditional validation attributes (RequiredIf, RequiredUnless)
- Problem Details (RFC 7807) for validation errors
- Automatic client-side metadata generation via adapters

#### Conditional Validation

**Server-Side Attributes**:
```csharp
public class RegisterViewModel
{
    [Required]
    public bool AcceptTerms { get; set; }
    
    [RequiredIf(nameof(AcceptTerms), true, ErrorMessage = "Phone is required when terms are accepted")]
    [Phone]
    public string? PhoneNumber { get; set; }
}
```

**Client-Side**:
- `conditional-validators.js` implements `requiredif` and `requiredunless` validators
- Integrated with `validation.js`
- Works with Alpine.js `x-show` for dynamic visibility
- Prevents form submission when validation fails

#### Validation Flow

```
User Input
    ↓
Client-Side Validation (data-val-*, pure JS)
    - Skip hidden fields
    - Check conditional requirements
    ↓ (if valid)
HTMX Submit
    ↓
Server-Side Validation
    ↓ (if invalid)
400 Problem Details Response
    ↓
HTMX Error Handler
    ↓
Display Errors in Form
```

### Cross-Component Communication

Alpine.js components can communicate via:

1. **Shared State** (Alpine Store):
```javascript
Alpine.store('global', {
    message: ''
});
```

2. **Custom Events**:
```html
<button @click="$dispatch('custom-event', { data: 'Hello' })">Publish</button>

<div @custom-event="message = $event.detail.data" x-text="message"></div>
```

3. **Server-Side Coordination** (Preferred):
Use HTMX to coordinate via server when possible.

## Usage Examples

### Example 1: Basic Form with Validation

```html
<form hx-post="/Account/Login" hx-target="#result" hx-ext="framework">
    <div class="form-group">
        <label asp-for="Email"></label>
        <input asp-for="Email" type="email" />
        <span asp-validation-for="Email"></span>
    </div>
    
    <div class="form-group">
        <label asp-for="Password"></label>
        <input asp-for="Password" type="password" />
        <span asp-validation-for="Password"></span>
    </div>
    
    <button type="submit">Login</button>
    <div id="result"></div>
</form>
```

### Example 2: Conditional Validation

```csharp
// Model
public class ConditionalViewModel
{
    [Required]
    public bool AcceptTerms { get; set; }
    
    [RequiredIf(nameof(AcceptTerms), true)]
    [Phone]
    public string? PhoneNumber { get; set; }
}
```

```html
<!-- View -->
<form x-data="{ acceptTerms: false }">
    <input type="checkbox" asp-for="AcceptTerms" x-model="acceptTerms" />
    
    <div x-show="acceptTerms">
        <input asp-for="PhoneNumber" 
               x-bind:data-should-validate="acceptTerms" />
        <span asp-validation-for="PhoneNumber"></span>
    </div>
    
    <button type="submit">Submit</button>
</form>
```

## Implementation Guidelines

### Validation Guidelines

- **.NET DataAnnotations**: Server generates `data-val-*` attributes
- **Pure JavaScript Validation**: Simple, production-grade implementation
- **Skip Hidden Fields**: Do not validate hidden fields
- **Conditional Validation**: Use RequiredIf/RequiredUnless attributes
- **Server Always Validates**: Never trust client-side validation alone
- **Problem Details for Errors**: Use .NET 10 Problem Details for 400 responses
- **Unified Error Display**: Same UI for client and server errors

### HTMX Guidelines

- **Declarative Attributes**: Use HTML attributes, not JavaScript
- **Built-in Methods**: Use `hx-put`, `hx-delete`, `hx-patch` directly
- **Behavior Hooks Extension**: Multiple comma-separated functions supported
- **Response Handling**: Return partial views or Problem Details
- **Extension Handles**: Problem Details parsing, error display, validation attachment

### Alpine.js Guidelines

- **Use x-data**: Component-local state with simple JavaScript objects
- **Declarative Directives**: `x-show`, `x-text`, `x-model`, `x-bind`, etc.
- **Conditional Visibility**: Use `x-show` with `x-transition` for smooth animations
- **Dynamic Attributes**: Use `x-bind:data-should-validate` for validation control
- **Event Handling**: Use `@click`, `@submit`, `$dispatch` for events
- **Keep It Simple**: No complex stores unless truly needed

## JavaScript Files

```
wwwroot/js/
├── validation.js              # Pure JavaScript validation (25KB)
├── conditional-validators.js  # RequiredIf/RequiredUnless (3KB)
└── framework-extension.js     # HTMX extension (9KB)
```

## Summary

This framework provides a modern, progressive approach to building web applications with .NET 10 MVC. By combining:

- **Static Rendering** for performance
- **HTMX** for declarative client-server communication
- **Alpine.js** for reactive components (no additional state libraries)
- **Pure JavaScript Validation** with conditional support (no external validation libraries)
- **Problem Details** for unified error handling
- **Simple Patterns** - no unnecessary complexity

Developers can build interactive applications with minimal JavaScript while maintaining excellent performance, SEO, and developer experience.

**Key Principles**:
- Declarative over imperative
- Simple over complex
- Server-first approach
- Use tools as-is, don't reinvent
- Production-grade quality
- All tests must pass

## Lessons Learned from Jamidon

1. **Test-Driven Development**: All 22 Playwright tests passing before considering work complete
2. **No External Validation Libraries**: Pure JavaScript validation is more maintainable than v8n or jQuery Validate
3. **No State Management Libraries**: Alpine.js `x-data` is sufficient, no need for Zustand or Redux
4. **Conditional Validation**: Required both client and server implementations with proper attribute adapters
5. **Alpine.js Integration**: Use `x-bind:data-should-validate` for dynamic field visibility
6. **HTMX Form Prevention**: Check validation in `htmx:configRequest` to prevent submission
7. **Problem Details DOM**: Preserve validation message structure when rendering server errors
8. **Layout Consolidation**: Single `_Layout.cshtml` with all scripts reduces duplication
9. **Production Mindset**: No compromises on test quality, scalability, or code cleanliness
10. **Keep It Simple**: Three JavaScript files (37KB total) handle entire framework - no bloat

