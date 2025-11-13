// ============================================================================
// ASP.NET Core MVC Client-Side Validation (Pure JavaScript - No Dependencies)
// Mimics jQuery Unobtrusive Validation patterns with data-val-* attributes
// ============================================================================

(function() {
    'use strict';
    
    console.log('[VAL] validation.js loading...');
    
    // ============================================================================
    // PURE JAVASCRIPT VALIDATORS (No v8n dependency)
    // ============================================================================
    
    const validators = {
        required: (value) => {
            if (typeof value === 'boolean') return value;
            if (Array.isArray(value)) return value.length > 0;
            return value !== null && value !== undefined && String(value).trim() !== '';
        },
        
        email: (value) => {
            if (!value) return true;
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(String(value));
        },
        
        minlength: (value, min) => {
            if (!value) return true;
            return String(value).length >= parseInt(min || 0, 10);
        },
        
        maxlength: (value, max) => {
            if (!value) return true;
            return String(value).length <= parseInt(max || Infinity, 10);
        },
        
        length: (value, min, max) => {
            if (!value) return true;
            const len = String(value).length;
            const minVal = min ? parseInt(min, 10) : 0;
            const maxVal = max ? parseInt(max, 10) : Infinity;
            return len >= minVal && len <= maxVal;
        },
        
        range: (value, min, max) => {
            if (!value) return true;
            const num = parseFloat(value);
            if (isNaN(num)) return false;
            const minVal = min ? parseFloat(min) : -Infinity;
            const maxVal = max ? parseFloat(max) : Infinity;
            return num >= minVal && num <= maxVal;
        },
        
        url: (value) => {
            if (!value) return true;
            try {
                new URL(value);
                return true;
            } catch {
                return /^https?:\/\/.+/.test(value);
            }
        },
        
        phone: (value) => {
            if (!value) return true;
            const cleaned = String(value).replace(/\D/g, '');
            return cleaned.length >= 10;
        },
        
        creditcard: (value) => {
            if (!value) return true;
            const cardNumber = String(value).replace(/\D/g, '');
            if (cardNumber.length < 13 || cardNumber.length > 19) return false;
            
            // Luhn algorithm
            let sum = 0;
            let shouldDouble = false;
            
            for (let i = cardNumber.length - 1; i >= 0; i--) {
                let digit = parseInt(cardNumber[i], 10);
                if (shouldDouble) {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
                shouldDouble = !shouldDouble;
            }
            
            return (sum % 10) === 0;
        },
        
        regex: (value, pattern) => {
            if (!value) return true;
            if (!pattern) return true;
            return new RegExp(pattern).test(String(value));
        },
        
        equalto: (value, element, otherFieldName) => {
            if (!value) return true;
            if (!otherFieldName) return true;
            
            const form = element.closest('form');
            if (!form) return false;
            
            const cleanName = otherFieldName.replace(/^\*\./, '');
            const otherField = form.querySelector(`[name="${cleanName}"]`);
            if (!otherField) return false;
            
            return String(value) === String(otherField.value);
        },
        
        // Remote validation - returns a Promise
        remote: async (value, element, url, additionalFields) => {
            if (!value) return true;
            if (!url) return true;
            
            try {
                // Build form data
                const formData = new FormData();
                formData.append(element.name, value);
                
                // Add additional fields if specified
                if (additionalFields) {
                    const form = element.closest('form');
                    if (form) {
                        const fieldNames = additionalFields.split(',').map(f => f.trim().replace(/^\*\./, ''));
                        fieldNames.forEach(fieldName => {
                            const field = form.querySelector(`[name="${fieldName}"]`);
                            if (field && field !== element) {
                                formData.append(fieldName, field.value || '');
                            }
                        });
                    }
                }
                
                // Make POST request to validation endpoint
                const response = await fetch(url, {
                    method: 'POST',
                    body: formData
                });
                
                if (!response.ok) {
                    return false;
                }
                
                const result = await response.json();
                
                // Handle different response formats
                // ASP.NET Core Remote returns: true, false, or "error message"
                if (typeof result === 'boolean') {
                    return result;
                }
                if (typeof result === 'string') {
                    return result === 'true' || result === '';
                }
                if (result && typeof result === 'object') {
                    // Handle { valid: true/false } format
                    return result.valid === true;
                }
                
                return false;
            } catch (error) {
                console.error('[VAL] Remote validation error:', error);
                return true; // On error, pass validation (server will validate)
            }
        }
    };
    
    // ============================================================================
    // VALIDATION SERVICE
    // ============================================================================
    
    const validationService = {
        schemas: new Map(),
        boundElements: new WeakSet()
    };
    
    function parseValidationRules(element) {
        const rules = {};
        const attrs = Array.from(element.attributes);
        
        attrs.forEach(attr => {
            if (!attr.name.startsWith('data-val-')) return;
            
            const parts = attr.name.split('-');
            if (parts.length < 3) return;
            
            const ruleName = parts[2];
            const paramName = parts[3];
            
            if (!paramName) {
                if (!rules[ruleName]) {
                    rules[ruleName] = { message: attr.value, params: {} };
                } else {
                    rules[ruleName].message = attr.value;
                }
            } else {
                if (!rules[ruleName]) {
                    rules[ruleName] = { message: '', params: {} };
                }
                rules[ruleName].params[paramName] = attr.value;
            }
        });
        
        return rules;
    }
    
    function isHidden(element) {
        if (element.type === 'hidden') return true;
        
        const shouldValidate = element.getAttribute('data-should-validate');
        if (shouldValidate === 'false') return true;
        
        if (!element.offsetParent) return true;
        
        const style = window.getComputedStyle(element);
        return style.display === 'none' || style.visibility === 'hidden';
    }
    
    function getFieldValue(element) {
        if (element.type === 'checkbox') {
            return element.checked;
        }
        if (element.type === 'radio') {
            const form = element.closest('form');
            const checked = form?.querySelector(`[name="${element.name}"]:checked`);
            return checked?.value || '';
        }
        return element.value || '';
    }
    
    function findErrorContainer(element) {
        const name = element.getAttribute('name') || element.id;
        if (!name) return null;
        
        const form = element.closest('form');
        const scope = form || document;
        const containers = scope.querySelectorAll(`[data-valmsg-for="${name}"]`);
        
        return containers.length > 0 ? containers[0] : null;
    }
    
    function validateElementSync(element) {
        if (isHidden(element)) {
            return [];
        }
        
        const rules = parseValidationRules(element);
        const value = getFieldValue(element);
        const errors = [];
        
        // Check required first
        if (rules.required && !validators.required(value)) {
            errors.push(rules.required.message);
            return errors;
        }
        
        // Check conditional validators (requiredif, requiredunless) - BEFORE early return
        if (rules.requiredif && validators.requiredif) {
            const dependentProperty = rules.requiredif.params.dependentproperty;
            const expectedValue = rules.requiredif.params.expectedvalue;
            if (!validators.requiredif(value, element, dependentProperty, expectedValue)) {
                errors.push(rules.requiredif.message);
                return errors;
            }
        }
        
        if (rules.requiredunless && validators.requiredunless) {
            const dependentProperty = rules.requiredunless.params.dependentproperty;
            const expectedValue = rules.requiredunless.params.expectedvalue;
            if (!validators.requiredunless(value, element, dependentProperty, expectedValue)) {
                errors.push(rules.requiredunless.message);
                return errors;
            }
        }
        
        // If empty and not required, skip other validations
        if (!value && element.type !== 'checkbox') {
            return [];
        }
        
        // Check other rules (skip remote - it's async)
        for (const [ruleName, ruleData] of Object.entries(rules)) {
            if (ruleName === 'required' || ruleName === 'remote' || ruleName === 'requiredif' || ruleName === 'requiredunless') continue;
            
            let isValid = true;
            
            switch (ruleName) {
                case 'length':
                    isValid = validators.length(value, ruleData.params.min, ruleData.params.max);
                    break;
                case 'minlength':
                    isValid = validators.minlength(value, ruleData.params.min);
                    break;
                case 'maxlength':
                    isValid = validators.maxlength(value, ruleData.params.max);
                    break;
                case 'range':
                    isValid = validators.range(value, ruleData.params.min, ruleData.params.max);
                    break;
                case 'equalto':
                    isValid = validators.equalto(value, element, ruleData.params.other);
                    break;
                case 'regex':
                    isValid = validators.regex(value, ruleData.params.pattern);
                    break;
                default:
                    if (validators[ruleName]) {
                        isValid = validators[ruleName](value);
                    }
            }
            
            if (!isValid) {
                errors.push(ruleData.message);
                break;
            }
        }
        
        return errors;
    }
    
    async function validateElementAsync(element) {
        // First run synchronous validation
        const syncErrors = validateElementSync(element);
        if (syncErrors.length > 0) {
            return syncErrors;
        }
        
        // Then check remote validation if present
        const rules = parseValidationRules(element);
        if (rules.remote) {
            const value = getFieldValue(element);
            if (value) {
                const isValid = await validators.remote(
                    value,
                    element,
                    rules.remote.params.url,
                    rules.remote.params.additionalfields
                );
                
                if (!isValid) {
                    return [rules.remote.message];
                }
            }
        }
        
        return [];
    }
    
    function displayFieldErrors(element, errors) {
        const container = findErrorContainer(element);
        
        if (errors.length > 0) {
            element.classList.add('input-validation-error');
            element.classList.remove('input-validation-valid');
            element.setAttribute('data-validation-error', errors[0]);
            
            if (container) {
                container.textContent = errors[0];
                container.style.display = 'block';
                container.classList.add('field-validation-error');
                container.classList.remove('field-validation-valid');
            }
        } else {
            element.classList.remove('input-validation-error');
            element.classList.add('input-validation-valid');
            element.removeAttribute('data-validation-error');
            
            if (container) {
                container.textContent = '';
                container.style.display = 'none';
                container.classList.remove('field-validation-error');
                container.classList.add('field-validation-valid');
            }
        }
    }
    
    function clearFieldErrors(element) {
        displayFieldErrors(element, []);
    }
    
    function attachValidationToElement(element) {
        if (validationService.boundElements.has(element)) {
            return;
        }
        
        const rules = parseValidationRules(element);
        const hasRemote = rules.remote !== undefined;
        
        const validate = async () => {
            let errors;
            
            if (hasRemote) {
                // Use async validation for fields with remote validation
                errors = await validateElementAsync(element);
            } else {
                // Use sync validation for regular fields
                errors = validateElementSync(element);
            }
            
            displayFieldErrors(element, errors);
            return errors.length === 0;
        };
        
        // Attach blur event (fires when element loses focus)
        element.addEventListener('blur', async (e) => {
            await validate();
        }, false);
        
        // Attach input event (fires on text changes)
        element.addEventListener('input', async () => {
            const container = findErrorContainer(element);
            if (container && container.classList.contains('field-validation-error')) {
                await validate();
            }
        });
        
        // Attach change event for checkboxes and radios
        if (element.type === 'checkbox' || element.type === 'radio') {
            element.addEventListener('change', validate);
        }
        
        element._validator = validate;
        validationService.boundElements.add(element);
    }
    
    function attachValidationToForm(form) {
        if (form.hasAttribute('data-validation-attached')) {
            console.log('[VAL] Form already has validation attached:', form.id);
            return;
        }
        
        console.log('[VAL] Attaching validation to form:', form.id || form.name);
        const fields = form.querySelectorAll('[data-val]');
        console.log('[VAL] Found', fields.length, 'fields with data-val in form');
        
        fields.forEach((field, index) => {
            console.log(`[VAL] Processing field ${index + 1}/${fields.length}:`, field.name || field.id);
            attachValidationToElement(field);
        });
        
        // Add form submit handler to validate before submission
        // This ensures conditional fields are properly validated
        form.addEventListener('submit', function(evt) {
            console.log('[VAL] Form submit event, validating all visible fields');
            let hasErrors = false;
            
            // Validate all fields that should be validated
            form.querySelectorAll('[data-val="true"]').forEach(field => {
                // Check if field should be validated (for conditional validation)
                const shouldValidate = field.getAttribute('data-should-validate');
                if (shouldValidate === 'false') {
                    // Skip validation for fields that are conditionally hidden
                    console.log('[VAL] Skipping validation for conditionally hidden field:', field.name);
                    return;
                }
                
                // Run validation
                const errors = validateElementSync(field);
                displayFieldErrors(field, errors);
                
                if (errors.length > 0) {
                    console.log('[VAL] Validation errors found on field:', field.name, errors);
                    hasErrors = true;
                }
            });
            
            if (hasErrors) {
                console.log('[VAL] Form submission prevented due to validation errors');
                evt.preventDefault();
                evt.stopPropagation();
                return false;
            }
            
            console.log('[VAL] Form validation passed, allowing submission');
        });
        
        form.setAttribute('data-validation-attached', 'true');
        console.log('[VAL] Form validation attachment complete');
    }
    
    function attachValidation(root) {
        const element = root instanceof HTMLElement ? root : document;
        
        // Attach to forms
        const forms = element.querySelectorAll ? element.querySelectorAll('form') : [];
        forms.forEach(form => {
            attachValidationToForm(form);
        });
        
        // If root is a form, attach to it
        if (element.tagName === 'FORM') {
            attachValidationToForm(element);
        }
    }
    
    // ============================================================================
    // EXPORTS
    // ============================================================================
    
    // ============================================================================
    // PUBLIC API - Exports for external use
    // ============================================================================
    
    window.validators = validators;
    window.validationService = {
        ...validationService,
        parseValidationRules,
        isHidden,
        getFieldValue,
        findErrorContainer,
        clearFieldErrors
    };
    window.validateFieldSync = validateElementSync;
    window.validateFieldAsync = validateElementAsync;
    window.displayFieldErrors = displayFieldErrors;
    window.attachValidation = attachValidation;
    window.attachValidationToForm = attachValidationToForm;
    window.attachValidationToElement = attachValidationToElement;
    
    console.log('[VAL] Pure JavaScript validation with remote support ready!');
})();

// ============================================================================
// ALPINE.JS CONDITIONAL VALIDATION DIRECTIVE
// Enables conditional validation based on Alpine.js reactivity
// 
// USAGE EXAMPLES:
// 
// 1. Simple checkbox condition:
//    <input x-validate-when="acceptTerms" />
// 
// 2. Complex expression:
//    <input x-validate-when="userType === 'premium' && age >= 18" />
// 
// 3. Multiple conditions:
//    <input x-validate-when="isVisible && isEnabled && hasPermission" />
// 
// 4. Inverted condition:
//    <input x-validate-when="!isDisabled" />
// 
// 5. With select dropdown:
//    <select x-model="country">...</select>
//    <input x-validate-when="country === 'USA'" placeholder="State" />
// 
// 6. Computed property:
//    x-data="{ items: [], get hasItems() { return items.length > 0 } }"
//    <input x-validate-when="hasItems" />
// ============================================================================

document.addEventListener('alpine:init', () => {
    // Custom directive to mark fields for conditional validation
    // The field will only be validated when the Alpine expression evaluates to true
    Alpine.directive('validate-when', (el, { expression }, { evaluateLater, effect }) => {
        const getCondition = evaluateLater(expression);
        
        // Set initial state immediately (not validated by default)
        el.setAttribute('data-should-validate', 'false');
        
        // Track previous state to detect changes
        let previousShouldValidate = false;
        
        // Use Alpine's reactive effect to watch the condition
        effect(() => {
            getCondition(value => {
                const shouldValidate = Boolean(value);
                const changed = shouldValidate !== previousShouldValidate;
                
                el.setAttribute('data-should-validate', shouldValidate ? 'true' : 'false');
                
                if (!shouldValidate) {
                    // Field should NOT be validated (hidden/disabled)
                    // Clear any existing validation errors immediately
                    if (window.validationService?.clearFieldErrors) {
                        window.validationService.clearFieldErrors(el);
                    }
                    // Clear the field value to prevent server-side validation
                    if (el.value) {
                        el.value = '';
                    }
                } else if (changed && shouldValidate) {
                    // Field just became visible/enabled
                    // Trigger validation if field already has a value
                    if (el.value && window.validateFieldSync) {
                        // Use setTimeout to ensure Alpine has finished updating the DOM
                        setTimeout(() => {
                            const errors = window.validateFieldSync(el);
                            if (window.displayFieldErrors) {
                                window.displayFieldErrors(el, errors);
                            }
                        }, 0);
                    }
                }
                
                previousShouldValidate = shouldValidate;
            });
        });
    });
    
    // Magic function for manual validation control
    // Usage: $validateWhen.check(element) or $validateWhen.clear(element)
    Alpine.magic('validateWhen', () => {
        return {
            check: (el) => {
                const shouldValidate = el.getAttribute('data-should-validate');
                return shouldValidate === 'true';
            },
            clear: (el) => {
                if (window.validationService?.clearFieldErrors) {
                    window.validationService.clearFieldErrors(el);
                }
            },
            // Force validation on demand
            validate: (el) => {
                if (window.validateFieldSync && window.displayFieldErrors) {
                    const errors = window.validateFieldSync(el);
                    window.displayFieldErrors(el, errors);
                }
            }
        };
    });
    
    console.log('[Alpine Validation] Conditional validation directive registered');
});

// ============================================================================
// HOW TO ADD A CUSTOM VALIDATOR
// ============================================================================
/*

1. ADD THE VALIDATOR FUNCTION
   Add your custom validator to the validators object at the top of this file:

   validators: {
       required: (value) => { ... },
       email: (value) => { ... },
       
       // Add your custom validator here
       zipcode: (value) => {
           if (!value) return true; // Empty is valid unless also required
           return /^\d{5}(-\d{4})?$/.test(value);
       },
       
       ssn: (value) => {
           if (!value) return true;
           return /^\d{3}-\d{2}-\d{4}$/.test(value);
       }
   }

2. ADD A CASE IN THE SWITCH STATEMENT
   In the validateElementSync function, add a case for your validator:

   switch (ruleName) {
       case 'length':
           isValid = validators.length(value, ruleData.params.min, ruleData.params.max);
           break;
       
       // Add your custom validator case
       case 'zipcode':
           isValid = validators.zipcode(value);
           break;
   }

3. CREATE SERVER-SIDE VALIDATION ATTRIBUTE (Optional)
   Create a C# ValidationAttribute that generates the data-val-* attributes:

   public class ZipCodeAttribute : ValidationAttribute
   {
       protected override ValidationResult IsValid(object value, ValidationContext context)
       {
           if (value == null || string.IsNullOrEmpty(value.ToString()))
               return ValidationResult.Success;
           
           var regex = new Regex(@"^\d{5}(-\d{4})?$");
           if (regex.IsMatch(value.ToString()))
               return ValidationResult.Success;
           
           return new ValidationResult(ErrorMessage ?? "Invalid ZIP code");
       }
   }
   
   // Client-side adapter
   public class ZipCodeAttributeAdapter : AttributeAdapterBase<ZipCodeAttribute>
   {
       public ZipCodeAttributeAdapter(ZipCodeAttribute attribute, IStringLocalizer stringLocalizer)
           : base(attribute, stringLocalizer) { }
       
       public override void AddValidation(ClientModelValidationContext context)
       {
           MergeAttribute(context.Attributes, "data-val", "true");
           MergeAttribute(context.Attributes, "data-val-zipcode", GetErrorMessage(context));
       }
       
       public override string GetErrorMessage(ModelValidationContextBase validationContext)
       {
           return Attribute.ErrorMessage ?? "Invalid ZIP code";
       }
   }

4. OR USE REGULAR EXPRESSION ATTRIBUTE
   If you don't need a custom attribute, use RegularExpression:

   [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZIP code")]
   public string ZipCode { get; set; }
   
   This will automatically generate:
   <input data-val="true" 
          data-val-regex="Invalid ZIP code"
          data-val-regex-pattern="^\d{5}(-\d{4})?$" />
   
   The 'regex' case in the switch statement will handle it automatically!

5. EXAMPLE: CUSTOM VALIDATOR WITH PARAMETERS
   For validators that need parameters (like min/max):

   // In validators object
   between: (value, min, max) => {
       if (!value) return true;
       const num = parseFloat(value);
       return num >= parseFloat(min) && num <= parseFloat(max);
   }
   
   // In switch statement
   case 'between':
       isValid = validators.between(value, ruleData.params.min, ruleData.params.max);
       break;
   
   // Server-side generates:
   // data-val-between="Value must be between 1 and 100"
   // data-val-between-min="1"
   // data-val-between-max="100"

6. EXAMPLE: ASYNC CUSTOM VALIDATOR
   For validators that need to call an API:

   // In validators object (return a Promise)
   checkAvailability: async (value, element, url) => {
       if (!value) return true;
       
       try {
           const response = await fetch(url + '?value=' + encodeURIComponent(value));
           const result = await response.json();
           return result.available === true;
       } catch (error) {
           console.error('Availability check failed:', error);
           return true; // Pass on error, server will validate
       }
   }
   
   // In validateElementAsync function (not validateElementSync!)
   if (rules.checkavailability) {
       const value = getFieldValue(element);
       if (value) {
           const isValid = await validators.checkAvailability(
               value,
               element,
               rules.checkavailability.params.url
           );
           if (!isValid) {
               return [rules.checkavailability.message];
           }
       }
   }

7. TESTING YOUR CUSTOM VALIDATOR
   Test in browser console:

   // Test the validator function directly
   window.validators.zipcode('12345')      // Should return true
   window.validators.zipcode('12345-6789') // Should return true
   window.validators.zipcode('invalid')    // Should return false
   
   // Test on a form field
   const field = document.querySelector('#ZipCode');
   window.validateFieldSync(field);

That's it! Your custom validator is now integrated with the framework.

*/

