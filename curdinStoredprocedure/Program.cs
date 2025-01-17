using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using curdinStoredprocedure.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<ProductCategoryData>();
builder.Services.AddTransient<ProductItemsData>();
builder.Services.AddTransient<UserDataAccess>();
builder.Services.AddTransient<AddToartData>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddDistributedMemoryCache(); // In-memory session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // For GDPR compliance
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:3000", "https://localhost:7217/api/auth/azureadlogin")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; 
})
.AddCookie() 
.AddGoogle(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ClientId = builder.Configuration["Google:ClientID"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
})
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["GitHub:ClientId"];
    options.ClientSecret = builder.Configuration["GitHub:ClientSecret"];
    options.CallbackPath = new PathString("/signin-github");

    //options.Scope.Add("user:email");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var email = context.Identity.FindFirst("urn:github:email")?.Value ?? string.Empty;
            var name = context.Identity.FindFirst("urn:github:name")?.Value ?? string.Empty;

            context.Identity.AddClaim(new System.Security.Claims.Claim("name", name));
            context.Identity.AddClaim(new System.Security.Claims.Claim("email", email));
        }
    };
})
.AddJwtBearer("JwtScheme", options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }
            return Task.CompletedTask;
        }
    };
})
.AddOpenIdConnect("AzureAd", options =>
 {
     options.ClientId = builder.Configuration["AzureAd:ClientId"];
     options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}";
     options.ResponseType = "id_token";
     options.SaveTokens = true;
     options.Scope.Add("openid");
     options.Scope.Add("profile");
     options.Scope.Add("email");
     options.TokenValidationParameters = new TokenValidationParameters
     {
         NameClaimType = "name",
         RoleClaimType = "roles"
     };
 })
    .AddOpenIdConnect("AzureAdB2C", options =>
    {
        var azureAdB2C = builder.Configuration.GetSection("AzureAdB2C");

        options.ClientId = azureAdB2C["ClientId"];
        options.Authority = $"{azureAdB2C["Instance"]}{azureAdB2C["Domain"]}/{azureAdB2C["SignUpSignInPolicyId"]}/v2.0";
        options.ResponseType = "id_token";
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add(azureAdB2C["Scopes"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "roles"
        };
    });


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Error Handling and HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductCategory}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
