using MHWApp.Api.Data;
using MHWApp.Library;
using MHWApp.Library.DTO;
using MHWApp.Library.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// -- CUSTOM CODE STARTS HERE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// establish cookie authentication
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

// Configure app cookie
//
// The default values, which are appropriate for hosting the Backend and
// BlazorWasmAuth apps on the same domain, are Lax and SameAsRequest. 
// For more information on these settings, see:
// https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#cross-domain-hosting-same-site-configuration
/*
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
*/

// configure authorization
builder.Services.AddAuthorizationBuilder();

// add identity and opt-in to endpoints
builder.Services.AddIdentityCore<Player>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// add a CORS policy for the client
builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:5001",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:5002"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

// add services to the container
builder.Services.AddEndpointsApiExplorer();

// add NSwag services
builder.Services.AddOpenApiDocument();

// -- CUSTOM CODE ENDS HERE

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseOpenApi();
    app.UseSwaggerUi();
}

// create routes for the identity endpoints
app.MapIdentityApi<Player>();

// activate the CORS policy
app.UseCors("wasm");

// Enable authentication and authorization after CORS Middleware
// processing (UseCors) in case the Authorization Middleware tries
// to initiate a challenge before the CORS Middleware has a chance
// to set the appropriate headers.
app.UseAuthentication();
app.UseAuthorization();

// provide an endpoint to clear the cookie for logout
//
// For more information on the logout endpoint and antiforgery, see:
// https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#antiforgery-support
app.MapPost("/logout", async (SignInManager<Player> signInManager, [FromBody] object empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }

    return Results.Unauthorized();
}).RequireAuthorization();

app.UseHttpsRedirection();

// provide an endpoint for user roles
app.MapGet("/roles", (ClaimsPrincipal user) =>
{
    if (user.Identity is not null && user.Identity.IsAuthenticated)
    {
        var identity = (ClaimsIdentity)user.Identity;
        var roles = identity.FindAll(identity.RoleClaimType)
            .Select(c =>
                new
                {
                    c.Issuer,
                    c.OriginalIssuer,
                    c.Type,
                    c.Value,
                    c.ValueType
                });

        return TypedResults.Json(roles);
    }

    return Results.Unauthorized();
}).RequireAuthorization();

#region Endpoint: Dummy-Example for Authorization
// provide an endpoint example that requires authorization
app.MapPost("/data-processing-1", ([FromBody] FormModel model) =>
    Results.Text($"{model.Message.Length} characters"))
        .RequireAuthorization();

// provide an endpoint example that requires authorization with a policy
app.MapPost("/data-processing-2", ([FromBody] FormModel model) =>
    Results.Text($"{model.Message.Length} characters"))
        .RequireAuthorization(policy => policy.RequireRole("Manager"));
#endregion

#region Endpoint: Session

app.MapGet("/sessions", async (AppDbContext dbContext, ClaimsPrincipal user) =>
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);  // Standard claim for user ID in most scenarios.
        if (userIdClaim == null)
        {
            return Results.Unauthorized();
        }

        var playerId = int.Parse(userIdClaim.Value);
        var playerSessions = await dbContext.Sessions
            .Where(s =>
                dbContext.Participators.Any(p => p.PlayerId == playerId && p.SessionId == s.Id))
            .ToListAsync();

        return Results.Ok(playerSessions);
    } 
);

app.MapPost("/sessions", async (Session newSession, AppDbContext dbContext, ClaimsPrincipal user) =>
    {
        await dbContext.Sessions.AddAsync(newSession);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/newSession/{newSession.Id}", newSession);
    }
);
#endregion

#region Endpoint: Player

app.MapGet("/players", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var playerId = int.Parse(userIdClaim.Value);

    var players = await dbContext.Players.Where(p => p.Id != playerId).ToListAsync();

    return Results.Ok(players);
}
);

#endregion

#region Endpoint: Participator

app.MapGet("/participators/{sessionId}", async (AppDbContext dbContext, ClaimsPrincipal user, int sessionId) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var playerId = int.Parse(userIdClaim.Value);

    var participator = await dbContext.Participators.Include(p => p.WeaponType).FirstOrDefaultAsync(p => p.PlayerId == playerId && p.SessionId == sessionId);

    return Results.Ok(participator);
}
);

app.MapPost("/participators", async (AppDbContext dbContext, ClaimsPrincipal user, Participator participator, bool isSelf) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    if (isSelf)
    {
        var playerId = int.Parse(userIdClaim.Value);
        participator.PlayerId = playerId;
    }
    
    await dbContext.Participators.AddAsync(participator);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/participators/{participator.Id}", participator);
}
).RequireAuthorization();

#endregion

#region Endpoint: Participator Inventory Entry

//app.MapGet("/participator/inventoryEntry", async (int sessionId, AppDbContext dbContext, ClaimsPrincipal user) => 
//    {
//        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

//        if (userIdClaim == null) return Results.Unauthorized();

//        var playerId = int.Parse(userIdClaim.Value);

//        var session = await dbContext.Sessions
//        .FirstOrDefaultAsync(s => s.Id == sessionId && dbContext.Participators.Any(p => p.PlayerId == playerId && p.SessionId == sessionId));

//        if (session == null) return Results.Unauthorized();

//        //TODO
//    }
//);

#endregion

#region Endpoint: Category

app.MapGet("/categories", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var categories = await dbContext.Categories
        .Include(c => c.InventoryEntryCategory)
        .ToListAsync();

    return Results.Ok(categories);
});

app.MapGet("/categories/inventoryentries", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var inventoryentryCategories = await dbContext.InventoryEntryCategories.ToListAsync();

    return Results.Ok(inventoryentryCategories);
});

app.MapGet("/categories/weapons", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var weaponCategories = await dbContext.WeaponCategories.ToListAsync();

    return Results.Ok(weaponCategories);
});

app.MapGet("/categories/armors", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var armorCategories = await dbContext.ArmorCategories.ToListAsync();

    return Results.Ok(armorCategories);
});

app.MapPost("/categories", async (CategoryDTO categoryDTO, AppDbContext dbContext) =>
{
    await dbContext.Categories.AddAsync(categoryDTO.NewCategory);

    await dbContext.SaveChangesAsync();

    if(categoryDTO.IsInventoryEntry)
    {
        await dbContext.InventoryEntryCategories.AddAsync(
        new InventoryEntryCategory
        {
            Id = 0,
            CategoryId = categoryDTO.NewCategory.Id,
            Order = StaticVariables.ORDER_DEFAULT_VALUE
        });
    }

    if (categoryDTO.IsWeapon)
    {
        await dbContext.WeaponCategories.AddAsync(
        new WeaponCategory
        {
            Id = 0,
            CategoryId = categoryDTO.NewCategory.Id,
            Order = StaticVariables.ORDER_DEFAULT_VALUE
        });
    }

    if (categoryDTO.IsArmor)
    {
        await dbContext.ArmorCategories.AddAsync(
        new ArmorCategory
        {
            Id = 0,
            CategoryId = categoryDTO.NewCategory.Id,
            Order = StaticVariables.ORDER_DEFAULT_VALUE
        });
    }

    await dbContext.SaveChangesAsync();

    return Results.Created($"/newcategory/{categoryDTO.NewCategory.Id}", categoryDTO.NewCategory);
}
).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

#endregion

#region Endpoint: Inventory Entry

app.MapGet("inventoryEntry", async (AppDbContext dbContext) =>
    {
        var inventoryEntries = await dbContext.InventoryEntries.ToListAsync();

        return Results.Ok(inventoryEntries); ;
    }
).RequireAuthorization(policy => policy.RequireRole("Manager", "Admin"));

app.MapPost("inventoryEntry", async (InventoryEntry newInventoryEntry, AppDbContext dbContext) =>
    {
        await dbContext.InventoryEntries.AddAsync(newInventoryEntry);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/newSession/{newInventoryEntry.Id}", newInventoryEntry);
    }
).RequireAuthorization(policy => policy.RequireRole("Manager", "Admin"));

#endregion

#region Endpoint: Weapon

app.MapGet("/weapontypes", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Results.Unauthorized();
    }

    var weaponTypes = await dbContext.WeaponTypes.ToListAsync();

    return Results.Ok(weaponTypes);
}).RequireAuthorization();

#endregion

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// example form model
class FormModel
{
    public string Message { get; set; } = string.Empty;
}