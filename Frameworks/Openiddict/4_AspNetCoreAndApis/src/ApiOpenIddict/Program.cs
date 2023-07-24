// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

//using OpenIddict.Validation.AspNetCore;

using IdentityModel.AspNetCore.AccessTokenValidation;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
     // Note: the validation handler uses OpenID Connect discovery
     // to retrieve the address of the introspection endpoint.
     options.SetIssuer("https://localhost:5001/");
     options.AddAudiences("web");
     
     // Configure the validation handler to use introspection and register the client
     // credentials used when communicating with the remote introspection endpoint.
     options.UseIntrospection()
         .SetClientId("web")
         .SetClientSecret("secret");
     
     // Register the System.Net.Http integration.
     options.UseSystemNetHttp();

     // Register the ASP.NET Core host.
     options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    })
);

builder.Services.AddScopeTransformation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("ApiScope");

app.Run();
