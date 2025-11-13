using Microsoft.AspNetCore.Mvc;
using Jamidon.Models;

namespace Jamidon.Controllers;

public class SamplesController : Controller
{
    private bool IsHtmxRequest => Request.Headers.ContainsKey("HX-Request");
    
    private IActionResult ValidationProblemDetails()
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred",
            Status = 400
        };

        var errors = new Dictionary<string, string[]>();
        foreach (var keyValuePair in ModelState)
        {
            var key = keyValuePair.Key;
            var errorMessages = keyValuePair.Value.Errors
                .Select(e => e.ErrorMessage)
                .ToArray();
            
            if (errorMessages.Length > 0)
            {
                errors[key] = errorMessages;
            }
        }

        problemDetails.Extensions["errors"] = errors;

        return BadRequest(problemDetails);
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Sample1()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Sample2()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Sample2(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "Login successful!");
    }

    [HttpGet]
    public IActionResult Sample3()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Sample3Success()
    {
        return Content("<div class=\"success\">Success!</div>", "text/html");
    }

    [HttpGet]
    public IActionResult Sample3Error()
    {
        return StatusCode(500, "Internal Server Error");
    }

    [HttpGet]
    public IActionResult Sample4()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Sample4(LoginViewModel model)
    {
        // FOR TESTING: Always return 400 with Problem Details for HTMX requests
        // This allows us to test Problem Details handling even with valid input
        if (IsHtmxRequest)
        {
            // If model is invalid, return actual validation errors
            if (!ModelState.IsValid)
            {
                return ValidationProblemDetails();
            }
            
            // If model is valid, return a test Problem Details response anyway
            // This is for testing Problem Details handling
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred",
                Status = 400
            };

            var errors = new Dictionary<string, string[]>
            {
                ["Email"] = new[] { "Test error: Email validation failed" },
                ["Password"] = new[] { "Test error: Password validation failed" }
            };

            problemDetails.Extensions["errors"] = errors;
            return BadRequest(problemDetails);
        }

        // For non-HTMX requests, normal behavior
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Sample5()
    {
        return View();
    }

    [HttpPost]
    [Route("Samples/Sample5LoadItems")]
    public IActionResult Sample5LoadItems()
    {
        var response = new
        {
            html = "<ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul>",
            statePayload = new
            {
                items = new[] { "Item 1", "Item 2", "Item 3" },
                total = 3,
                loading = false
            }
        };

        return Json(response);
    }

    [HttpGet]
    public IActionResult Sample6()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public IActionResult Sample6(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "Registration successful! Passwords match.");
    }

    [HttpGet]
    public IActionResult Sample7()
    {
        return View(new ComprehensiveValidatorModel());
    }

    [HttpPost]
    public IActionResult Sample7(ComprehensiveValidatorModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "All validators passed! Form submitted successfully.");
    }

    [HttpGet]
    public IActionResult Sample8()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Sample8(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            // For non-HTMX requests (shouldn't happen in this framework, but handle gracefully)
            return View(model);
        }

        // Success - re-render island with success message
        ViewData["Success"] = "Login successful! Island re-rendered via HTMX.";
        
        if (IsHtmxRequest)
        {
            // Return the full view for HTMX to swap
            return View(model);
        }
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Sample9()
    {
        return View(new RemoteValidationViewModel());
    }

    [HttpPost]
    public IActionResult Sample9(RemoteValidationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "Registration successful! Username and email are available.");
    }

    [HttpPost]
    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckUsername([FromForm] string Username)
    {
        // ASP.NET Core Remote attribute expects JSON response: { valid: true/false }
        // Simulate checking if username is taken
        var takenUsernames = new[] { "admin", "test", "user" };
        var isValid = !takenUsernames.Contains(Username?.ToLower() ?? "");
        
        return Json(new { valid = isValid });
    }

    [HttpPost]
    [AcceptVerbs("GET", "POST")]
    public IActionResult CheckEmail([FromForm] string Email)
    {
        // ASP.NET Core Remote attribute expects JSON response: { valid: true/false }
        // Simulate checking if email is registered
        var registeredEmails = new[] { "test@example.com", "admin@example.com" };
        var isValid = !registeredEmails.Contains(Email?.ToLower() ?? "");
        
        return Json(new { valid = isValid });
    }

    [HttpGet]
    public IActionResult Sample10()
    {
        return View(new RemoteValidationViewModel());
    }

    [HttpPost]
    public IActionResult Sample10(RemoteValidationViewModel model)
    {
        // Server-side validation for Remote attributes
        // [Remote] attribute only validates client-side, so we need to manually validate on server
        var takenUsernames = new[] { "admin", "test", "user" };
        if (takenUsernames.Contains(model.Username?.ToLower() ?? ""))
        {
            ModelState.AddModelError(nameof(model.Username), "Username is already taken");
        }

        var registeredEmails = new[] { "test@example.com", "admin@example.com" };
        if (registeredEmails.Contains(model.Email?.ToLower() ?? ""))
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already registered");
        }

        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "Registration successful! Username and email are available.");
    }

    [HttpGet]
    public IActionResult Sample11()
    {
        return View(new PriceValidationViewModel());
    }

    [HttpPost]
    public IActionResult Sample11(PriceValidationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", $"Product '{model.ProductName}' saved successfully! Price: ${model.Price:F2}");
    }

    [HttpGet]
    public IActionResult Sample12()
    {
        return View(new SimpleConditionalViewModel());
    }

    [HttpPost]
    public IActionResult Sample12(SimpleConditionalViewModel model)
    {
        // No need to manually handle conditional validation anymore!
        // The RequiredIf attribute handles it automatically
        
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "✅ Form submitted successfully! All conditional validations passed.");
    }
    
    public IActionResult Sample12Comprehensive()
    {
        return View(new ConditionalValidationViewModel());
    }

    [HttpPost]
    public IActionResult Sample12Comprehensive(ConditionalValidationViewModel model)
    {
        // No need to manually handle conditional validation anymore!
        // The RequiredIf and RequiredUnless attributes handle everything automatically
        
        if (!ModelState.IsValid)
        {
            if (IsHtmxRequest)
            {
                return ValidationProblemDetails();
            }
            return View(model);
        }

        return PartialView("_SuccessMessage", "✅ All conditional validations passed! Form submitted successfully!");
    }
}

