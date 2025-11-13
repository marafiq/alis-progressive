// ============================================================================
// HTMX Extension: Framework Extension for .NET 10 MVC + Alpine Validation
// Provides: Behavior Hooks, Problem Details, Alpine Validation Integration
// ============================================================================
// Based on https://htmx.org/extensions/

// Initialize validation for initial page load forms
function initializeInitialPageValidation() {
    if (!window.attachValidation) {
        setTimeout(initializeInitialPageValidation, 50);
        return;
    }
    
    const forms = document.querySelectorAll('form:not([data-island-id])');
    forms.forEach(form => {
        const isHtmxForm = form.hasAttribute('hx-post') || form.hasAttribute('hx-get') || 
                          form.hasAttribute('hx-put') || form.hasAttribute('hx-delete') ||
                          form.hasAttribute('hx-patch');
        
        if (isHtmxForm) {
            const island = form.closest('[data-island-id]');
            if (!island && !form.hasAttribute('data-validation-attached')) {
                console.log('[FW-EXT] Calling attachValidation on form:', form.id);
                window.attachValidation(form);
            }
        }
    });
}

// Run initialization when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        setTimeout(initializeInitialPageValidation, 10);
    });
} else {
    setTimeout(initializeInitialPageValidation, 10);
}

htmx.defineExtension('framework', {
    
    // Called when htmx is about to make a request
    onEvent: async function (name, evt) {
        if (name === "htmx:configRequest") {
            // Execute before hooks
            callBehaviorHooks(evt.target, 'before', evt);
            
            // Validate form before HTMX request
            const form = evt.target.closest('form');
            if (form && form.hasAttribute('data-validation-attached')) {
                const island = form.closest('[data-island-id]');
                const scope = island || form;
                
                let hasErrors = false;
                
                const fields = [];
                const allElements = scope.querySelectorAll('input, select, textarea');
                const processedCheckboxGroups = new Set();
                
                for (const element of allElements) {
                    if (!form.contains(element)) continue;
                    if (element.type === 'hidden') continue;
                    
                    // Check if field should be validated (for conditional validation)
                    const shouldValidate = element.getAttribute('data-should-validate');
                    if (shouldValidate === 'false') {
                        console.log('[FW-EXT] Skipping validation for conditionally hidden field:', element.name);
                        continue; // Skip this field entirely
                    }
                    
                    const hasValidation = Array.from(element.attributes).some(attr => 
                        attr.name.startsWith('data-val-') || attr.name === 'data-val'
                    );
                    
                    if (hasValidation) {
                        if (element.type === 'checkbox') {
                            if (processedCheckboxGroups.has(element.name)) continue;
                            processedCheckboxGroups.add(element.name);
                        }
                        fields.push(element);
                    }
                }
                
                // Validate all fields that should be validated
                for (const field of fields) {
                    if (window.validateFieldSync && window.displayFieldErrors) {
                        const errors = window.validateFieldSync(field);
                        window.displayFieldErrors(field, errors);
                        if (errors.length > 0) {
                            console.log('[FW-EXT] Validation error on field:', field.name, errors);
                            hasErrors = true;
                        }
                    }
                }
                
                // Prevent HTMX request if validation fails
                if (hasErrors) {
                    console.log('[FW-EXT] Validation errors found, preventing HTMX request');
                    evt.preventDefault();
                    return false;
                } else {
                    console.log('[FW-EXT] No validation errors, allowing HTMX request');
                }
            }
        }
        
        if (name === "htmx:afterRequest") {
            // Check if request was successful (2xx status)
            const xhr = evt.detail.xhr;
            const status = xhr?.status;
            const isSuccess = xhr && status >= 200 && status < 300;
            
            if (isSuccess) {
                // Execute success hooks - pass the full HTMX event
                callBehaviorHooks(evt.target, 'success', evt);
            }
        }
        
        if (name === "htmx:afterSwap") {
            // Process new HTMX content
            if (evt.detail.target && typeof htmx !== 'undefined' && htmx.process) {
                htmx.process(evt.detail.target);
            }
            
            // Attach validation to forms in the swapped content
            if (evt.detail.target && window.attachValidation) {
                const forms = evt.detail.target.querySelectorAll('form');
                forms.forEach(function(form) {
                    if (!form.hasAttribute('data-validation-attached')) {
                        window.attachValidation(form);
                    }
                });
                window.attachValidation(evt.detail.target);
            }
        }
        
        if (name === "htmx:responseError") {
            // Execute error hooks
            const xhr = evt.detail.xhr;
            const status = xhr?.status;
            
            callBehaviorHooks(evt.target, 'error', evt);
            
            // Handle Problem Details (RFC 7807) for 400 errors
            if (xhr && status === 400) {
                try {
                    const problemDetails = JSON.parse(xhr.responseText);
                    if (problemDetails.errors) {
                        const form = evt.target.closest('form');
                        handleProblemDetails(problemDetails, form);
                    }
                } catch (e) {
                    // Silent fail - not valid Problem Details format
                }
            }
        }
    }
});

// Behavior hooks implementation
function callBehaviorHooks(element, hookType, event) {
    const hooksAttr = element.getAttribute(`data-hx-${hookType}`);
    if (!hooksAttr) return;
    
    const hooks = hooksAttr.split(',').map(h => h.trim());
    
    hooks.forEach(hookName => {
        if (typeof window[hookName] === 'function') {
            try {
                window[hookName](event);
            } catch (error) {
                console.error(`[Framework Extension] Error in hook ${hookName}:`, error);
            }
        } else {
            console.warn(`[Framework Extension] Hook function not found: ${hookName}`);
        }
    });
}

// Problem Details (RFC 7807) handler
function handleProblemDetails(problemDetails, form) {
    const island = form ? form.closest('[data-island-id]') : null;
    const scope = island || form || document;
    
    // First, clear all existing errors in the form
    if (form) {
        const allErrorContainers = form.querySelectorAll('[data-valmsg-for]');
        allErrorContainers.forEach(container => {
            container.textContent = '';
            container.style.display = 'none';
            container.classList.remove('field-validation-error');
            container.classList.add('field-validation-valid');
        });
        const allFields = form.querySelectorAll('.input-validation-error');
        allFields.forEach(field => {
            field.classList.remove('input-validation-error');
        });
    }
    
    // Then, display new errors from Problem Details
    Object.keys(problemDetails.errors).forEach(fieldName => {
        const errors = problemDetails.errors[fieldName];
        
        // Skip empty error arrays
        if (Array.isArray(errors) && errors.length === 0) return;
        
        // Find field within scope
        const field = scope.querySelector(`[name="${fieldName}"]`);
        const errorContainers = scope.querySelectorAll(`[data-valmsg-for="${fieldName}"]`);
        const errorContainer = errorContainers.length > 0 ? errorContainers[0] : null;
        
        if (errorContainer) {
            const errorMessage = Array.isArray(errors) ? errors[0] : errors;
            
            errorContainer.textContent = errorMessage;
            errorContainer.style.display = 'block';
            errorContainer.classList.remove('field-validation-valid');
            errorContainer.classList.add('field-validation-error');
            
            if (field) {
                field.classList.add('input-validation-error');
            }
        }
    });
}

// Framework Extension loaded - Use hx-ext="framework" on elements
