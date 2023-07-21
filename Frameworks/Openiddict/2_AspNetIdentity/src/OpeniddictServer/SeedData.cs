using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpeniddictServer.Data;
using Serilog;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpeniddictServer;

public class SeedData
{
    public static async void EnsureSeedDataAsync(IApplicationBuilder app)
    {
         await using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateAsyncScope();
    
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        
        await RegisterApplicationsAsync(scope.ServiceProvider);
        //await RegisterScopesAsync(scope.ServiceProvider);
        await RegisterUserAsync(scope.ServiceProvider);

        static async Task RegisterApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

            // API
            if (await manager.FindByClientIdAsync("client") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "client",
                    ClientSecret = "secret",
                    DisplayName = "My client application",

                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "api1"
                    }
                });
            }
            
            // interactive ASP.NET Core Web App
            if (await manager.FindByClientIdAsync("web") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "web",
                    ClientSecret = "secret",
                    DisplayName = "My web application",
                    
                    RedirectUris =
                    {
                        new Uri("https://localhost:5002/signin-oidc")
                    },
                    
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:5002/signout-callback-oidc")
                    },
                    
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        //Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Authorization,
                        
                        Permissions.GrantTypes.AuthorizationCode,
                        //ResponseTypes.Code,
                        
                        Permissions.Scopes.Profile,
                        Permissions.Prefixes.Scope + "verification"
                    }
                });
            }
        }
        
        static async Task RegisterScopesAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

            if (await manager.FindByNameAsync("api1") is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Dantooine API access",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("fr-FR")] = "Accès à l'API de démo"
                    },
                    Name = "api1",
                    Resources =
                    {
                        //Assign the aud value to the resource parameter.
                        "resource_server_1"
                    }
                });
            }
        }

        static async Task RegisterUserAsync(IServiceProvider provider)
        {
            var userMgr = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = await userMgr.FindByNameAsync("alice");
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = await userMgr.CreateAsync(alice, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
        
                result = await userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(Claims.Name, "Alice Smith"),
                            new Claim(Claims.GivenName, "Alice"),
                            new Claim(Claims.FamilyName, "Smith"),
                            new Claim(Claims.Website, "http://alice.com"),
                        });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }
        
            var bob = await userMgr.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result =await userMgr.CreateAsync(bob, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
        
                result = await userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(Claims.Name, "Bob Smith"),
                            new Claim(Claims.GivenName, "Bob"),
                            new Claim(Claims.FamilyName, "Smith"),
                            new Claim(Claims.Website, "http://bob.com"),
                            new Claim("location", "somewhere")
                        });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }
    }
}