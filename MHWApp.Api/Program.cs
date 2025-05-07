using MHWApp.Api.Data;
using MHWApp.Api.Hubs;
using MHWApp.Library;
using static MHWApp.Library.StaticVariables;
using MHWApp.Library.DTO;
using MHWApp.Library.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region CUSTOM CODE
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


// JSON Serialization ignores cylces
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

// add services to the container
builder.Services.AddEndpointsApiExplorer();

// add NSwag services
builder.Services.AddOpenApiDocument();

// add SignalR services
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        // JSON Serialization ignores cylces also for SignalR
        options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.PayloadSerializerOptions.WriteIndented = true;
    });

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

#endregion

var app = builder.Build();

// Use Response Compression Middleware at the top of the processing pipeline's configuration
app.UseResponseCompression();

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

#region Endpoint: Logout
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
#endregion

app.UseHttpsRedirection();

#region Endpoint: Roles
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
#endregion

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
    if (userIdClaim == null) return Results.Unauthorized();

    var categories = await dbContext.Categories
        .Include(c => c.InventoryEntryCategory)
        .Include(c => c.WeaponCategory)
        .Include(c => c.ArmorCategory)
        .ToListAsync();

    return Results.Ok(categories);
});

app.MapPost("/categories", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, CategoryDTO categoryDTO) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.Categories.AnyAsync(ge => ge.Name == categoryDTO.NewCategory.Name);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.Categories.AddAsync(categoryDTO.NewCategory);
    await dbContext.SaveChangesAsync();

    await AddSubCategories(dbContext, categoryDTO);

    await hubContext.Clients.All.SendAsync("ReceiveNewCategory", categoryDTO.NewCategory);

    return Results.Created($"/categories/{categoryDTO.NewCategory.Id}", categoryDTO.NewCategory);
}
).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

async Task AddSubCategories(AppDbContext dbContext, CategoryDTO categoryDTO)
{
    if (categoryDTO.IsInventoryEntry)
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
            Order = ORDER_DEFAULT_VALUE
        });
    }
    await dbContext.SaveChangesAsync();
}

app.MapPost("/categories/inventoryentries", async (AppDbContext dbContext, ClaimsPrincipal user, InventoryEntryCategory inventoryEntryCategory) => 
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    await dbContext.InventoryEntryCategories.AddAsync(inventoryEntryCategory);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categories/inventoryentries/{inventoryEntryCategory.Id}", inventoryEntryCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories/weapons", async (AppDbContext dbContext, ClaimsPrincipal user, WeaponCategory weaponCategory) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    await dbContext.WeaponCategories.AddAsync(weaponCategory);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categories/weapons/{weaponCategory.Id}", weaponCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories/armors", async (AppDbContext dbContext, ClaimsPrincipal user, ArmorCategory armorCategory) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    await dbContext.ArmorCategories.AddAsync(armorCategory);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/categories/armors/{armorCategory.Id}", armorCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPut("/categories/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, Category updatedCategory, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingCategory = await dbContext.Categories.FindAsync(id);
    if (existingCategory == null) return Results.NotFound();

    existingCategory.Name = updatedCategory.Name;
    await dbContext.SaveChangesAsync();

    //await hubContext.Clients.All.SendAsync("ReceiveUpdatedGameExtension", existingGameExtension);

    return Results.NoContent();

}).RequireAuthorization();

app.MapDelete("/categories/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var category = await dbContext.Categories.FindAsync(id);
    if (category == null) return Results.NotFound();

    dbContext.Categories.Remove(category);
    await dbContext.SaveChangesAsync();

    //await hubContext.Clients.All.SendAsync("ReceiveDeletedGameExtension", id);

    return Results.NoContent();
}).RequireAuthorization();

#endregion

#region Endpoint: Game Extension

app.MapGet("/gameextensions", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var gameExtensions = await dbContext.GameExtensions.ToListAsync();

    return Results.Ok(gameExtensions);
}).RequireAuthorization();

app.MapPost("/gameextensions", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, GameExtension newGameExtension) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.GameExtensions.AnyAsync(ge => ge.Name == newGameExtension.Name);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.GameExtensions.AddAsync(newGameExtension);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewGameExtension", newGameExtension);

    return Results.Created($"/gameextensions/{newGameExtension.Id}", newGameExtension);
}).RequireAuthorization();

app.MapPut("/gameextensions/{id}", async (AppDbContext dbContext, IHubContext<DataBaseUpdatesHub> hubContext, ClaimsPrincipal user, GameExtension updatedGameExtension, int id) => 
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingGameExtension = await dbContext.GameExtensions.FindAsync(id);
    if (existingGameExtension == null) return Results.NotFound();

    existingGameExtension.Name = updatedGameExtension.Name;
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveUpdatedGameExtension", existingGameExtension);

    return Results.NoContent();

}).RequireAuthorization();

app.MapDelete("/gameextensions/{id}", async (AppDbContext dbContext, IHubContext<DataBaseUpdatesHub> hubContext, ClaimsPrincipal user, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var gameExtension = await dbContext.GameExtensions.FindAsync(id);
    if (gameExtension == null) return Results.NotFound();
    
    dbContext.GameExtensions.Remove(gameExtension);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedGameExtension", id);

    return Results.NoContent();
}).RequireAuthorization();

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

#region Endpoint: Weatherforecast
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
#endregion

#region Endpoint: Hubs
// Add an endpoint for the hub immediately before the line that runs the app (app.Run();):
app.MapHub<ChatHub>("/chathub");

app.MapHub<DataBaseUpdatesHub>("/databaseupdateshub");

#endregion

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