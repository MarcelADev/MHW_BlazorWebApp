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
using MHWApp.Api.Provider;

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

// SignalR uses this service automatically, behind the scenes
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();

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
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Results.Unauthorized();

        var playerId = int.Parse(userIdClaim.Value);

        var playerSessions = await dbContext.Sessions
            .Where(s =>
                dbContext.Participators.Any(p => p.PlayerId == playerId && p.SessionId == s.Id))
            .ToListAsync();

        return Results.Ok(playerSessions);
    }
).RequireAuthorization(); ;

app.MapPost("/sessions", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, CreateSessionDTO createSessionDTO) =>
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Results.Unauthorized();

        if (createSessionDTO.Participators is null || !createSessionDTO.Participators.Any()) return Results.BadRequest();

        await dbContext.Sessions.AddAsync(createSessionDTO.NewSession);
        await dbContext.SaveChangesAsync();

        foreach(var participator in createSessionDTO.Participators)
        {
            if (participator.PlayerId == 0) participator.PlayerId = int.Parse(userIdClaim.Value);
            participator.SessionId = createSessionDTO.NewSession.Id;
            await dbContext.Participators.AddAsync(participator);
        }

        await dbContext.SaveChangesAsync();

        var userIds = createSessionDTO.Participators
            .Select(p => p.PlayerId.ToString())
            .Distinct();

        foreach (var userId in userIds)
        {
            await hubContext.Clients.User(userId)
                .SendAsync("ReceiveNewSession", createSessionDTO.NewSession);
        }

        return Results.Created($"/sessions/{createSessionDTO.NewSession.Id}", createSessionDTO.NewSession);
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
        .Include(c => c.GameExtension)
        .ToListAsync();

    return Results.Ok(categories);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapGet("/categories/inventoryentries", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var inventoryCategories = await dbContext.InventoryEntryCategories
    .Include(ic => ic.Category)
    .ToListAsync();

    return Results.Ok(inventoryCategories);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, CategoryDTO categoryDTO) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.Categories.AnyAsync(c => c.Name == categoryDTO.NewCategory.Name);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.Categories.AddAsync(categoryDTO.NewCategory);
    await dbContext.SaveChangesAsync();

    await AddSubCategories(dbContext, categoryDTO);
    
    if (categoryDTO.NewCategory.GameExtension == null)
        categoryDTO.NewCategory.GameExtension = await dbContext.GameExtensions
            .FirstOrDefaultAsync(ge => ge.Id == categoryDTO.NewCategory.GameExtensionId);
    

    await hubContext.Clients.All.SendAsync("ReceiveNewCategory", categoryDTO.NewCategory);

    return Results.Created($"/categories/{categoryDTO.NewCategory.Id}", categoryDTO.NewCategory);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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

app.MapPost("/categories/inventoryentries", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, InventoryEntryCategory inventoryEntryCategory) => 
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.InventoryEntryCategories.AnyAsync(ic => ic.CategoryId == inventoryEntryCategory.CategoryId);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.InventoryEntryCategories.AddAsync(inventoryEntryCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewInventoryCategory", inventoryEntryCategory);

    return Results.Created($"/categories/inventoryentries/{inventoryEntryCategory.Id}", inventoryEntryCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories/weapons", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, WeaponCategory weaponCategory) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.WeaponCategories.AnyAsync(wc => wc.CategoryId == weaponCategory.CategoryId);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.WeaponCategories.AddAsync(weaponCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewWeaponCategory", weaponCategory);

    return Results.Created($"/categories/weapons/{weaponCategory.Id}", weaponCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories/armors", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, ArmorCategory armorCategory) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.ArmorCategories.AnyAsync(ac => ac.CategoryId == armorCategory.CategoryId);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    await dbContext.ArmorCategories.AddAsync(armorCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewArmorCategory", armorCategory);

    return Results.Created($"/categories/armors/{armorCategory.Id}", armorCategory);

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPut("/categories/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, Category updatedCategory, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingCategory = await dbContext.Categories.FindAsync(id);
    if (existingCategory == null) return Results.NotFound();

    existingCategory.Name = updatedCategory.Name;
    existingCategory.GameExtension = updatedCategory.GameExtension;
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveUpdatedCategory", existingCategory);

    return Results.NoContent();

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/categories/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var category = await dbContext.Categories.FindAsync(id);
    if (category == null) return Results.NotFound();

    dbContext.Categories.Remove(category);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedCategory", id);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/categories/inventoryentries/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var inventoryCategory = await dbContext.InventoryEntryCategories.FindAsync(id);
    if (inventoryCategory == null) return Results.NotFound();

    int categoryId = inventoryCategory.CategoryId;

    dbContext.InventoryEntryCategories.Remove(inventoryCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedInventoryCategory", categoryId);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/categories/weapons/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var weaponCategory = await dbContext.WeaponCategories.FindAsync(id);
    if (weaponCategory == null) return Results.NotFound();

    int categoryId = weaponCategory.CategoryId;

    dbContext.WeaponCategories.Remove(weaponCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedWeaponCategory", categoryId);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/categories/armors/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var armorCategory = await dbContext.ArmorCategories.FindAsync(id);
    if (armorCategory == null) return Results.NotFound();

    int categoryId = armorCategory.CategoryId;

    dbContext.ArmorCategories.Remove(armorCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedArmorCategory", categoryId);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));
#endregion

#region Endpoint: Game Extension

app.MapGet("/gameextensions", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var gameExtensions = await dbContext.GameExtensions.ToListAsync();

    return Results.Ok(gameExtensions);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

#endregion

#region Endpoint: Inventory Entry

app.MapGet("/inventoryEntries", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var inventoryEntries = await dbContext.InventoryEntries
        .Include(i => i.InventoryEntryCategories)
            .ThenInclude(ic => ic.Category)
        .ToListAsync();

    return Results.Ok(inventoryEntries); ;
}
).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/inventoryEntries", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, InventoryEntry newInventoryEntry) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.InventoryEntries.AnyAsync(ie => ie.Name == newInventoryEntry.Name);
    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

    if (newInventoryEntry.InventoryEntryCategories != null && newInventoryEntry.InventoryEntryCategories.Any())
    {
        var validCategoryIds = newInventoryEntry.InventoryEntryCategories.Select(c => c.Id).ToList();

        // Replace the incoming categories with tracked entities from DB
        newInventoryEntry.InventoryEntryCategories = await dbContext.InventoryEntryCategories
            .Where(iec => validCategoryIds.Contains(iec.Id))
            .Include(iec => iec.Category)
            .ToListAsync();
    }

    await dbContext.InventoryEntries.AddAsync(newInventoryEntry);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewInventoryEntry", newInventoryEntry);

    return Results.Created($"/inventoryEntries/{newInventoryEntry.Id}", newInventoryEntry);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPut("/inventoryEntries/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, InventoryEntry updatedInventoryEntry, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingInventoryEntry = await dbContext.InventoryEntries
        .Include(ie => ie.InventoryEntryCategories)
            .ThenInclude(iec => iec.Category)
        .FirstOrDefaultAsync(ie => ie.Id == id);
    if (existingInventoryEntry == null) return Results.NotFound();

    existingInventoryEntry.Name = updatedInventoryEntry.Name;

    var updatedCategoryIds = updatedInventoryEntry.InventoryEntryCategories
        .Select(c => c.Id)
        .ToHashSet();

    existingInventoryEntry.InventoryEntryCategories.RemoveAll(c => !updatedCategoryIds.Contains(c.Id));

    var existingCategoryIds = existingInventoryEntry.InventoryEntryCategories.Select(c => c.Id).ToHashSet();
    var missingCategoryIds = updatedCategoryIds.Except(existingCategoryIds);

    foreach (var categoryId in missingCategoryIds)
    {
        var inventoryEntryCategory = await dbContext.InventoryEntryCategories
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (inventoryEntryCategory != null)
        {
            existingInventoryEntry.InventoryEntryCategories.Add(inventoryEntryCategory);
        }
    }

    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveUpdatedInventoryEntry", existingInventoryEntry);

    return Results.NoContent();

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/inventoryEntries/{id}", async (AppDbContext dbContext, IHubContext<DataBaseUpdatesHub> hubContext, ClaimsPrincipal user, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var inventoryEntry = await dbContext.InventoryEntries.FindAsync(id);
    if (inventoryEntry == null) return Results.NotFound();

    dbContext.InventoryEntries.Remove(inventoryEntry);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedInventoryEntry", id);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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