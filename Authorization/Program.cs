// Kinda raw implementation

//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.AspNetCore.Http;
//using System.Runtime.Intrinsics.Arm;
//using System.Security.Claims;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDataProtection();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<AuthService>();

//var app = builder.Build();

//app.Use((ctx, next) =>
//{
//    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//    var protector = idp.CreateProtector("auth-cookie");
//    var authCookie = ctx.Request.Headers.Cookie.FirstOrDefault(cookie => cookie.StartsWith("auth="));
//    var protectedPayload = authCookie.Split("=").Last();
//    var payload = protector.Unprotect(protectedPayload);
//    var parts = payload.Split(":");
//    var key = parts[0];
//    var value = parts[1];
//    var claims = new List<Claim>();

//    claims.Add(new Claim(key, value));

//    var identity = new ClaimsIdentity(claims);

//    ctx.User = new ClaimsPrincipal(identity);

//    return next();
//});

//app.MapGet("/username", (HttpContext httpContext, IDataProtectionProvider idp) =>
//{
//    return httpContext.User.FindFirst("user").Value;
//});

//app.MapGet("/login", (AuthService authService) =>
//{
//    authService.SignIn();
//    return "ok";
//});

//app.Run();

//public class AuthService
//{
//    private readonly IDataProtectionProvider _idp;
//    private readonly IHttpContextAccessor _accessor;

//    public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
//    {
//        _idp = idp;
//        _accessor = accessor;
//    }

//    public void SignIn()
//    {
//        var protector = _idp.CreateProtector("auth-cookie");
//        _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("user:bodya")}";
//    }
//}

// ================================================================================================================

// Using asp functionality

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie").AddCookie("cookie");

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/username", (HttpContext httpContext, IDataProtectionProvider idp) =>
{
    return httpContext.User.FindFirst("user").Value;
});

app.MapGet("/login", async (HttpContext httpContext) =>
{
    var claims = new List<Claim>();

    claims.Add(new Claim("user", "bodya"));

    var identity = new ClaimsIdentity(claims, "cookie");
    var user = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync("cookie", user);

    return "ok";
});

app.Run();
