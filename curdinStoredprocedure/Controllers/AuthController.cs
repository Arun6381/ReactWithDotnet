using curdinStoredprocedure.DataAccessLayer;
using curdinStoredprocedure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly UserDataAccess _userDataAccess;

    public AuthController(JwtTokenService jwtTokenService, UserDataAccess userDataAccess)
    {
        _jwtTokenService = jwtTokenService;
        _userDataAccess = userDataAccess;
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        UserModel user = _userDataAccess.GetUserByEmail(request.email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid credentials" });

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            return Unauthorized(new { Message = "Invalid credentials" });

        var token = _jwtTokenService.GenerateToken(user.EmailAddress, user.Roles);  

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };
        HttpContext.Session.SetString("UserRole", user.Roles);

        Response.Cookies.Append("jwtToken", token, cookieOptions);
        Response.Cookies.Append("UserName", user.FirstName, cookieOptions);
        Response.Cookies.Append("Role", user.Roles, cookieOptions);

        return Ok(new { Message = "Login successful", token, user.FirstName, user.UserID, user.Roles });
    }


    [HttpGet("glogin")]
    public async Task Glogin()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return NotFound();
        }

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var oAuthUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier); 

        var existingUser = _userDataAccess.GetUserByEmail(email);

        if (existingUser == null)
        {
            _userDataAccess.InsertOAuthUser(name, string.Empty, email, "Google", oAuthUserId);
            existingUser = new UserModel { EmailAddress = email, FirstName = name, Roles = "user" }; 
        }


        var token = _jwtTokenService.GenerateToken(email,existingUser.Roles);
        HttpContext.Session.SetString("UserRole", existingUser.Roles);

        var redirectUrl = $"http://localhost:3000/auth/callback?token={token}&Roles={existingUser.Roles}&UserId={existingUser.UserID}&userName={Uri.EscapeDataString(name)}";
        return Redirect(redirectUrl);
    }


    [HttpGet("githublogin")]
    public async Task GitHubLogin()
    {
        await HttpContext.ChallengeAsync("GitHub", new AuthenticationProperties
        {
            RedirectUri = Url.Action("GitHubResponse")
        });
    }

    [HttpGet("GitHubResponse")]
    public async Task<IActionResult> GitHubResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return NotFound();
        }

        //var email = result.Principal?.FindFirstValue(ClaimTypes.Email);
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            email = "arunkumar6381@gmail.com";  
        }

        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var oAuthUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier); 

        var existingUser = _userDataAccess.GetUserByEmail(email);

        if (existingUser == null)
        {
            _userDataAccess.InsertOAuthUser(name, string.Empty, email, "GitHub", oAuthUserId);
            existingUser = new UserModel { EmailAddress = email, FirstName = name, Roles = "user" };
        }

        var token = _jwtTokenService.GenerateToken(email,existingUser.Roles);
        HttpContext.Session.SetString("UserRole", existingUser.Roles);

        var redirectUrl = $"http://localhost:3000/auth/callback?token={token}&Roles={existingUser.Roles}&UserId={existingUser.UserID}&userName={Uri.EscapeDataString(name)}";
        return Redirect(redirectUrl);
    }

    [HttpGet("azureadlogin")]
    public async Task AzureAdLogin()
    {
        await HttpContext.ChallengeAsync("AzureAd", new AuthenticationProperties
        {
            RedirectUri = Url.Action("AzureAdResponse")
        });
    }

    [HttpGet("AzureAdResponse")]
    public async Task<IActionResult> AzureAdResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
            return Unauthorized("Authentication failed.");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var oAuthUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(oAuthUserId))
            return BadRequest("Invalid user data.");

        // Check or insert user into the database
        var user = _userDataAccess.GetUserByEmail(email) ?? new UserModel
        {
            EmailAddress = email,
            FirstName = name,
            Roles = "user"
        };

        if (user.UserID == 0)
            _userDataAccess.InsertOAuthUser(name, string.Empty, email, "AzureAD", oAuthUserId);

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(email, user.Roles);

        // Redirect to frontend with token
        var redirectUrl = $"http://localhost:4200/auth/callback?token={token}&roles={user.Roles}&name={Uri.EscapeDataString(name)}";
        return Redirect(redirectUrl);
    }

    [HttpGet("b2clogin")]
    public async Task AzureB2CLogin()
    {
        try
        {
            await HttpContext.ChallengeAsync("AzureAdB2C", new AuthenticationProperties
            {
                RedirectUri = Url.Action("AzureB2CResponse")
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during Azure AD B2C login: {ex.Message}");
            throw;
        }
    }


    [HttpGet("AzureB2CResponse")]
    public async Task<IActionResult> AzureB2CResponse()
    {
        try
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return NotFound("Authentication failed or user is not authenticated.");
            }

            // Retrieve user claims from Azure AD B2C
            var email = result.Principal.FindFirstValue(ClaimTypes.Email)
                         ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var name = result.Principal.FindFirstValue(ClaimTypes.Name);
            var oAuthUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(oAuthUserId))
            {
                return BadRequest("Invalid user data retrieved from Azure AD B2C.");
            }

            // Check if the user exists in the database
            var existingUser = _userDataAccess.GetUserByEmail(email);
            if (existingUser == null)
            {
                _userDataAccess.InsertOAuthUser(name, string.Empty, email, "AzureAdB2C", oAuthUserId);
                existingUser = new UserModel { EmailAddress = email, FirstName = name, Roles = "user" };
            }

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(email, existingUser.Roles);
            HttpContext.Session.SetString("UserRole", existingUser.Roles);

            // Redirect to the frontend with the token and user details
            var redirectUrl = $"http://localhost:3000/auth/callback?token={token}&Roles={existingUser.Roles}&UserId={existingUser.UserID}&userName={Uri.EscapeDataString(name)}";
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during Azure AD B2C authentication response: {ex.Message}");
            return StatusCode(500, "Internal server error during Azure AD B2C authentication.");
        }
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Delete cookies with path and expiration set explicitly
        var cookieOptions = new CookieOptions
        {
            Path = "/", // Adjust if your cookies were set with a different path
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true // Ensure this matches how the cookies were set
        };

        Response.Cookies.Delete("jwtToken", cookieOptions);
        Response.Cookies.Delete("UserRole", cookieOptions);
        Response.Cookies.Delete("UserName", cookieOptions);
        Response.Cookies.Delete("Role", cookieOptions);

        return Ok(new { Message = "Logged out successfully" });
    }



}

