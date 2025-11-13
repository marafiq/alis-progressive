// ============================================================================
// CONDITIONAL VALIDATORS - Client-Side Support
// Works with RequiredIf and RequiredUnless attributes
// ============================================================================

(function() {
    'use strict';
    
    console.log('[Conditional Validators] Loading...');
    
    // Add conditional validators to the global validators object
    if (window.validators) {
        // RequiredIf validator
        window.validators.requiredif = function(value, element, dependentProperty, expectedValue) {
            // Get the dependent field value
            const form = element.closest('form');
            if (!form) return true;
            
            const dependentField = form.querySelector(`[name="${dependentProperty}"]`);
            if (!dependentField) {
                console.warn(`[RequiredIf] Dependent field '${dependentProperty}' not found`);
                return true;
            }
            
            // Get dependent field value
            let dependentValue;
            if (dependentField.type === 'checkbox') {
                dependentValue = dependentField.checked;
            } else if (dependentField.type === 'radio') {
                const checked = form.querySelector(`[name="${dependentProperty}"]:checked`);
                dependentValue = checked ? checked.value : null;
            } else {
                dependentValue = dependentField.value;
            }
            
            // Normalize expected value
            const normalizedExpected = expectedValue?.toString().toLowerCase();
            const normalizedDependent = dependentValue?.toString().toLowerCase();
            
            // Check if condition is met
            let conditionMet = false;
            if (normalizedExpected === 'true') {
                conditionMet = dependentValue === true || normalizedDependent === 'true';
            } else if (normalizedExpected === 'false') {
                conditionMet = dependentValue === false || normalizedDependent === 'false';
            } else {
                conditionMet = normalizedDependent === normalizedExpected;
            }
            
            // If condition is not met, field is not required
            if (!conditionMet) {
                return true;
            }
            
            // Condition is met, check if value is provided
            if (value === null || value === undefined) {
                return false;
            }
            
            if (typeof value === 'string' && value.trim() === '') {
                return false;
            }
            
            return true;
        };
        
        // RequiredUnless validator (inverse of RequiredIf)
        window.validators.requiredunless = function(value, element, dependentProperty, expectedValue) {
            // Get the dependent field value
            const form = element.closest('form');
            if (!form) return true;
            
            const dependentField = form.querySelector(`[name="${dependentProperty}"]`);
            if (!dependentField) {
                console.warn(`[RequiredUnless] Dependent field '${dependentProperty}' not found`);
                return true;
            }
            
            // Get dependent field value
            let dependentValue;
            if (dependentField.type === 'checkbox') {
                dependentValue = dependentField.checked;
            } else if (dependentField.type === 'radio') {
                const checked = form.querySelector(`[name="${dependentProperty}"]:checked`);
                dependentValue = checked ? checked.value : null;
            } else {
                dependentValue = dependentField.value;
            }
            
            // Normalize expected value
            const normalizedExpected = expectedValue?.toString().toLowerCase();
            const normalizedDependent = dependentValue?.toString().toLowerCase();
            
            // Check if condition is met
            let conditionMet = false;
            if (normalizedExpected === 'true') {
                conditionMet = dependentValue === true || normalizedDependent === 'true';
            } else if (normalizedExpected === 'false') {
                conditionMet = dependentValue === false || normalizedDependent === 'false';
            } else {
                conditionMet = normalizedDependent === normalizedExpected;
            }
            
            // UNLESS: If condition IS met, field is NOT required
            if (conditionMet) {
                return true;
            }
            
            // Condition is NOT met, check if value is provided
            if (value === null || value === undefined) {
                return false;
            }
            
            if (typeof value === 'string' && value.trim() === '') {
                return false;
            }
            
            return true;
        };
        
        console.log('[Conditional Validators] RequiredIf and RequiredUnless validators registered');
    } else {
        console.error('[Conditional Validators] window.validators not found!');
    }
    
    // No need to extend validateElementSync anymore - it's now built into validation.js
    // The validators are registered and will be called automatically
    
    console.log('[Conditional Validators] Ready!');
})();

