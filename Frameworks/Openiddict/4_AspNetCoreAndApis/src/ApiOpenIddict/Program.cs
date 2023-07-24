// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

//using OpenIddict.Validation.AspNetCore;

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
     
     // options.UseIntrospection()
     //     .SetClientId("web")
     //     .SetClientSecret("secret");
     
     options.AddAudiences("resource_server_2");

     // Register the System.Net.Http integration.
     options.UseSystemNetHttp();

     // Register the ASP.NET Core host.
     options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiResource", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("aud", "resource_server_2");
    })
);

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

app.MapControllers().RequireAuthorization("ApiResource");

app.Run();
