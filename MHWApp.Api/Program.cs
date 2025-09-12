using MHWApp.Api.Data;
using MHWApp.Api.Hubs;
using MHWApp.Api.Provider;
using MHWApp.Library;
using MHWApp.Library.DTO;
using MHWApp.Library.DTO.Create.Category_Armor_InventoryEntry;
using MHWApp.Library.DTO.Identity;
using MHWApp.Library.DTO.Images;
using MHWApp.Library.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static MHWApp.Library.StaticVariables;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region CUSTOM CODE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var enableRegister = builder.Configuration.GetValue<bool>("EnableRegister");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure app cookie
//
// The default values, which are appropriate for hosting the Backend and
// BlazorWasmAuth apps on the same domain, are Lax and SameAsRequest. 
// For more information on these settings, see:
// https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#cross-domain-hosting-same-site-configuration

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// configure authorization
builder.Services.AddAuthorizationBuilder();

// add identity and opt-in to endpoints
builder.Services.AddIdentity<Player, IdentityRole<int>>(options => 
    {
        // password requirements
        options.Password.RequireDigit = (bool)PASSWORD_REQUIREMENTS["RequireDigit"];
        options.Password.RequireLowercase = (bool)PASSWORD_REQUIREMENTS["RequireLowercase"];
        options.Password.RequireUppercase = (bool)PASSWORD_REQUIREMENTS["RequireUppercase"];
        options.Password.RequireNonAlphanumeric = (bool)PASSWORD_REQUIREMENTS["RequireNonAlphanumeric"];
        options.Password.RequiredLength = (int)PASSWORD_REQUIREMENTS["RequiredLength"];
        options.Password.RequiredUniqueChars = (int)PASSWORD_REQUIREMENTS["RequiredUniqueChars"];
        // username requirements
        options.User.AllowedUserNameCharacters = ALLOWED_USERNAME_CHARACTERS;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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
//app.MapIdentityApi<Player>();

// activate the CORS policy
app.UseCors("wasm");

// Enable authentication and authorization after CORS Middleware
// processing (UseCors) in case the Authorization Middleware tries
// to initiate a challenge before the CORS Middleware has a chance
// to set the appropriate headers.
app.UseAuthentication();
app.UseAuthorization();

#region Register
if (enableRegister)
{
    app.MapPost("/register", async (HttpContext httpContext, UserManager<Player> userManager, IdentityDTO dto) =>
    {
        if (Equals(dto.UserName.ToLower(), "prime vu"))
        {
            return Results.Problem(
                title: "Prime Vu",
                detail: $"The username '{dto.UserName}' can not be assigned. It is achieved.",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        if (BANNED_NAMES.Contains(dto.UserName.Replace(" ", "").ToLower()))
        {
            return Results.Problem(
                title: "Username not allowed",
                detail: $"The username '{dto.UserName}' is not permitted.",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        var existingUser = await userManager.FindByNameAsync(dto.UserName);

        if (existingUser is not null)
        {
            return Results.Problem(
                title: "Username already taken",
                detail: $"The username '{dto.UserName}' is already in use.",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        var player = new Player
        {
            UserName = dto.UserName,
            IsApproved = false
        };

        var result = await userManager.CreateAsync(player, dto.Password);

        return result.Succeeded ? Results.NoContent() : Results.BadRequest();
    });
}
#endregion

#region Login
app.MapPost("/login", async (
    HttpContext httpContext,
    [FromServices] SignInManager<Player> signInManager,
    [FromServices] UserManager<Player> userManager,
    [FromBody] IdentityDTO dto) =>
{
    var user = await userManager.FindByNameAsync(dto.UserName);
    if (user is null) return Results.Conflict();    // same HttpStatusCode for unknown user as for wrong password

    if (!user.IsApproved) return Results.Unauthorized();

    var result = await signInManager.PasswordSignInAsync(user, dto.Password, false, false);
    if (result.Succeeded)
    {
        return Results.Ok(); // Das Cookie wird automatisch gesetzt
    }
    else
    {
        return Results.Conflict();  // same HttpStatusCode for unknown user as for wrong password
    }
});
#endregion

#region Endpoint: Logout
// provide an endpoint to clear the cookie for logout
//
// For more information on the logout endpoint and antiforgery, see:
// https://learn.microsoft.com/aspnet/core/blazor/security/webassembly/standalone-with-identity#antiforgery-support
app.MapPost("/logout", async ([FromServices] SignInManager<Player> signInManager, [FromBody] object empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }

    return Results.Unauthorized();
}).RequireAuthorization();
#endregion

#region User Info
app.MapGet("/manage/info", (ClaimsPrincipal user) =>
{
    if (user.Identity is not null && user.Identity.IsAuthenticated)
    {
        var name = user.Identity.Name ?? "";
        var claims = user.Claims
            .ToDictionary(c => c.Type, c => c.Value);

        return TypedResults.Ok(new
        {
            Email = name,
            Claims = claims
        });
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

#region Endpoint: Images

app.MapGet("/images", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var imageEntities = await dbContext.Images.ToListAsync();

    var images = imageEntities.Select(img => new ImageDTO
    {
        Name = img.Name,
        ImageType = img.ImageType,
        Base64Data = Convert.ToBase64String(img.Data)
    }).ToList();

    return Results.Ok(images);
}).RequireAuthorization();

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
).RequireAuthorization();

app.MapPost("/sessions", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, SessionCreateDTO createSessionDTO) =>
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Results.Unauthorized();

        if (createSessionDTO.Participators is null || createSessionDTO.Participators.Count == 0) return Results.BadRequest();

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
).RequireAuthorization();

app.MapPut("/sessions/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id, SessionUpdateDTO updatedSessionDTO) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingSession = await dbContext.Sessions.FindAsync(id);
    if (existingSession == null) return Results.NotFound();

    var isAdminOrMaster = user.IsInRole(ADMIN_ROLE) || user.IsInRole(MASTER_ROLE);

    if (!string.IsNullOrWhiteSpace(updatedSessionDTO.UpdatedSession.Name)) existingSession.Name = updatedSessionDTO.UpdatedSession.Name;

    if (updatedSessionDTO.UpdatedSession.MaxDays != existingSession.MaxDays &&
        updatedSessionDTO.UpdatedSession.MaxDays >= 1)
    {
        existingSession.MaxDays = updatedSessionDTO.UpdatedSession.MaxDays;
    }

    if (updatedSessionDTO.UpdatedSession.DayCount != existingSession.DayCount &&
        updatedSessionDTO.UpdatedSession.DayCount >= 1 &&
        updatedSessionDTO.UpdatedSession.DayCount <= existingSession.MaxDays)
    {
        existingSession.DayCount = updatedSessionDTO.UpdatedSession.DayCount;
    }

    if (isAdminOrMaster)
    {
        // Remove Participators
        if (updatedSessionDTO.RemoveParticipators.Count > 0)
        {
            var idsToRemove = updatedSessionDTO.RemoveParticipators.Select(p => p.Id).ToList();
            var participatorsToRemove = await dbContext.Participators
                .Where(p => idsToRemove.Contains(p.Id))
                .ToListAsync();

            dbContext.Participators.RemoveRange(participatorsToRemove);
        }

        // Add Participators
        foreach (var participator in updatedSessionDTO.AddParticipators)
        {
            participator.SessionId = id;
            await dbContext.Participators.AddAsync(participator);
        }
    }

    await dbContext.SaveChangesAsync();

    if (isAdminOrMaster)
    {
        // Notify removed participators via SignalR
        var notifyRemovedTasks = updatedSessionDTO.RemoveParticipators
        .Where(p => p.PlayerId > 0)
        .Select(p => hubContext.Clients.User(p.PlayerId.ToString()).SendAsync("ReceiveDeletedParticipator", id));

        await Task.WhenAll(notifyRemovedTasks);
    }

    // Notify remaining and new participators via SignalR
    var participators = await dbContext.Participators
                            .Where(p => p.SessionId == id)
                            .ToListAsync();

    var notifyRemainingTasks = participators
    .Select(p => hubContext.Clients.User(p.PlayerId.ToString()).SendAsync("ReceiveUpdatedSession", existingSession));

    await Task.WhenAll(notifyRemainingTasks);

    return Results.NoContent();

}).RequireAuthorization();

app.MapDelete("/sessions/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var session = await dbContext.Sessions.FindAsync(id);
    if (session == null) return Results.NotFound();

    var userIds = await dbContext.Participators
        .Where(p => p.SessionId == id)
        .Select(p => p.PlayerId.ToString())
        .ToListAsync();

    dbContext.Sessions.Remove(session);
    await dbContext.SaveChangesAsync();

    foreach (var userId in userIds)
    {
        await hubContext.Clients.User(userId)
            .SendAsync("ReceiveDeletedSession", id);
    }

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));
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

app.MapGet("/participators/{sessionId}", async (AppDbContext dbContext, ClaimsPrincipal user, int sessionId, bool user_only) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    if (user_only)
    {
        var playerId = int.Parse(userIdClaim.Value);

        var participator = await dbContext.Participators
            .Include(p => p.WeaponType)
            .FirstOrDefaultAsync(p => p.PlayerId == playerId && p.SessionId == sessionId);

        return Results.Ok(participator);
    }
    else
    {
        var participators = await dbContext.Participators
            .Where(p => p.SessionId == sessionId)
            .Include(p => p.WeaponType)
            .Include (p => p.Player)
            .ToListAsync();

        return Results.Ok(participators);
    }
}
).RequireAuthorization();

//app.MapPost("/participators", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, Participator participator) =>
//{
//    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
//    if (userIdClaim == null) return Results.Unauthorized();

//    bool exists = await dbContext.Participators.AnyAsync(p => p.SessionId == participator.Id && p.PlayerId == participator.PlayerId);
//    if (exists) return Results.StatusCode(HTTP_STATUSCODE_CONFLICT);

//    await dbContext.Participators.AddAsync(participator);
//    await dbContext.SaveChangesAsync();

//    return Results.Created($"/participators/{participator.Id}", participator);  //No SignalR needed, as Participator is only seen by the user who created it
//}
//).RequireAuthorization();

app.MapDelete("/participators/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, int id) =>    // TO DO: MAKE 2ND DELETE
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var participator = await dbContext.Participators.FindAsync(id);
    if (participator == null) return Results.NotFound();

    var sessionId = participator.SessionId;

    dbContext.Participators.Remove(participator);
    await dbContext.SaveChangesAsync();

    var userId = userIdClaim.Value;

    if (!dbContext.Participators.Any(p => p.SessionId == sessionId))
    {
        var session = await dbContext.Sessions.FindAsync(sessionId);
        if (session != null)
        {
            dbContext.Sessions.Remove(session);
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.User(userId).SendAsync("ReceiveDeletedSession", sessionId);
        }
    }
    else
    {
        await hubContext.Clients.User(userId).SendAsync("ReceiveDeletedParticipator", sessionId);
    }

    return Results.NoContent();
}).RequireAuthorization();

#endregion

#region Endpoint: Participator Inventory Entry

//app.MapGet("/participator/inventoryEntry", async (int sessionId, AppDbContext dbContext, ClaimsPrincipal user) => 
//    {
//        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

//        if (userIdClaim == null) return Results.Unauthorized();

//        var userId = int.Parse(userIdClaim.Value);

//        var session = await dbContext.Sessions
//        .FirstOrDefaultAsync(s => s.Id == sessionId && dbContext.Participators.Any(p => p.PlayerId == userId && p.SessionId == sessionId));

//        if (session == null) return Results.Unauthorized();

//        //TODO
//    }
//);

#endregion

#region Endpoint: Category + Armor + InventoryEntry
app.MapPost("/categoriesArmorsInventoryEntries", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, CreateCategoryArmorInventoryEntryDTO dto) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool categoryExists = await dbContext.Categories.AnyAsync(c => c.Name == dto.Name);
    if (categoryExists) return Results.Conflict();

    Category newCategory = new()
    {
        Id = 0,
        Name = dto.Name
    };

    await dbContext.Categories.AddAsync(newCategory);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewCategory", newCategory);

    bool addArmorSet = dto.ArmorSet is not null && dto.ArmorSet.Count == 3 && dto.GeneralGameExtensionId is not null;

    // check if all 3 armor types are present and unique
    if (addArmorSet)
    {
        List<ArmorType> armorTypes = [];

        foreach (var armorModel in dto.ArmorSet!)
        {
            if (armorTypes.Contains(armorModel.ArmorType))
                addArmorSet = false;
            else
                armorTypes.Add(armorModel.ArmorType);
        }
    }
    
    if (addArmorSet)
    {
        ArmorCategory newArmorCategory = new()
        {
            Id = 0,
            CategoryId = newCategory.Id,
            Order = ORDER_DEFAULT_VALUE
        };
        await dbContext.ArmorCategories.AddAsync(newArmorCategory);
        await dbContext.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("ReceiveNewArmorCategory", newArmorCategory);

        foreach (var armorModel in dto.ArmorSet!)
        {
            Armor armor = new()
            {
                Id = 0,
                Name = armorModel.Name,
                //ArmorType = armorModel.ArmorType,
                ArmorCategoryId = newArmorCategory.Id,
                GameExtensionId = (int)dto.GeneralGameExtensionId!,
                Order = ORDER_DEFAULT_VALUE
            };

            await dbContext.Armors.AddAsync(armor);
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("ReceiveNewArmor", armor);
        }
    }

    bool addInventoryEntries = dto.InventoryEntries is not null && dto.InventoryEntries.Count > 0;

    if (addInventoryEntries)
    {
        InventoryEntryCategory newInventoryEntryCategory = new()
        {
            Id = 0,
            CategoryId = newCategory.Id,
            Order = ORDER_DEFAULT_VALUE
        };

        await dbContext.InventoryEntryCategories.AddAsync(newInventoryEntryCategory);
        await dbContext.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync("ReceiveNewInventoryCategory", newInventoryEntryCategory);

        foreach (InventoryEntryModel inventoryEntryModel in dto.InventoryEntries!)
        {
            if (await dbContext.InventoryEntries.AnyAsync(ie => ie.Name == inventoryEntryModel.Name))
            {
                var existingInventoryEntry = await dbContext.InventoryEntries.FirstAsync(ie => ie.Name == inventoryEntryModel.Name);
                existingInventoryEntry.InventoryEntryCategories.Add(newInventoryEntryCategory);

                await dbContext.SaveChangesAsync();

                await hubContext.Clients.All.SendAsync("ReceiveUpdatedInventoryEntry", existingInventoryEntry);
            }
            else
            { 
                InventoryEntry inventoryEntry = new()
                {
                    Id = 0,
                    Name = inventoryEntryModel.Name,
                    GameExtensionId = inventoryEntryModel.GameExtension.Id,
                    InventoryEntryCategories = [newInventoryEntryCategory],
                    Order = ORDER_DEFAULT_VALUE
                };

                await dbContext.InventoryEntries.AddAsync(inventoryEntry);
                await dbContext.SaveChangesAsync();

                await hubContext.Clients.All.SendAsync("ReceiveNewInventoryEntry", inventoryEntry);
            }
        }
    }

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));
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

app.MapGet("/categories/armors", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var armorCategories = await dbContext.ArmorCategories
    .Include(ac => ac.Category)
    .ToListAsync();

    return Results.Ok(armorCategories);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/categories", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, CategoryCreateDTO categoryDTO) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.Categories.AnyAsync(c => c.Name == categoryDTO.NewCategory.Name);
    if (exists) return Results.Conflict();

    await dbContext.Categories.AddAsync(categoryDTO.NewCategory);
    await dbContext.SaveChangesAsync();

    await AddSubCategories(dbContext, categoryDTO);    

    await hubContext.Clients.All.SendAsync("ReceiveNewCategory", categoryDTO.NewCategory);

    return Results.Created($"/categories/{categoryDTO.NewCategory.Id}", categoryDTO.NewCategory);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

async static Task AddSubCategories(AppDbContext dbContext, CategoryCreateDTO categoryDTO)
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
    if (exists) return Results.Conflict();

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
    if (exists) return Results.Conflict();

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
    if (exists) return Results.Conflict();

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
    if (exists) return Results.Conflict();

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
            .ThenInclude(iec => iec.Category)
        .Include(i => i.NamePrefix)
            .ThenInclude(iec => iec!.Category)
        .Include(i => i.GameExtension)
        .ToListAsync();

    return Results.Ok(inventoryEntries); ;
}
).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/inventoryEntries", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, InventoryEntryCreateDTO newInventoryEntryDTO) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    bool exists = await dbContext.InventoryEntries.AnyAsync(ie => ie.Name == newInventoryEntryDTO.Name);
    if (exists) return Results.Conflict();

    if (newInventoryEntryDTO.InventoryEntryCategoryIds == null || newInventoryEntryDTO.InventoryEntryCategoryIds.Count == 0)
        return Results.BadRequest("At least one category must be assigned.");

    if (newInventoryEntryDTO.NamePrefixId.HasValue)
    {
        bool prefixExists = await dbContext.InventoryEntryCategories
            .AnyAsync(c => c.Id == newInventoryEntryDTO.NamePrefixId.Value);
        if (!prefixExists) return Results.BadRequest("Provided NamePrefixId does not exist.");

        bool prefixIsInChosenCategories = newInventoryEntryDTO.InventoryEntryCategoryIds
            .Contains(newInventoryEntryDTO.NamePrefixId.Value);
        if (!prefixIsInChosenCategories) return Results.BadRequest("NamePrefixId wasn't in the selected Categories");
    }

    var newInventoryEntry = new InventoryEntry
    {
        Id = 0,
        Name = newInventoryEntryDTO.Name,
        Order = newInventoryEntryDTO.Order,
        InventoryEntryCategories = await dbContext.InventoryEntryCategories
                                        .Where((iec) => newInventoryEntryDTO.InventoryEntryCategoryIds!
                                        .Contains(iec.Id))
                                        .ToListAsync(),
        NamePrefixId = newInventoryEntryDTO.NamePrefixId,
        NamePrefix = newInventoryEntryDTO.NamePrefixId.HasValue ?
                        await dbContext.InventoryEntryCategories.FirstOrDefaultAsync(iec => iec.Id == newInventoryEntryDTO.NamePrefixId)
                        : null,
        GameExtensionId = newInventoryEntryDTO.GameExtensionId,
    };    

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

#region Endpoint: Armor

app.MapGet("/armors", async (AppDbContext dbContext, ClaimsPrincipal user) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var armors = await dbContext.Armors
        .Include(i => i.ArmorCategory!)
            .ThenInclude(ac => ac.Category)
        .ToListAsync();

    return Results.Ok(armors);
}
).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPost("/armors", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, Armor newArmor) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    if(newArmor.NamePrefix)
    {
        bool exists = await dbContext.Armors.AnyAsync(a => a.Name == newArmor.Name && a.NamePrefix == false);
        if (exists) return Results.Conflict();
    }
    else
    {
        bool exists = await dbContext.Armors.AnyAsync(a => a.Name == newArmor.Name && 
                                                           a.NamePrefix == true &&
                                                           a.ArmorCategoryId == newArmor.ArmorCategoryId);
        if (exists) return Results.Conflict();
    }

    bool armorCategoryExists = await dbContext.ArmorCategories
           .AnyAsync(ac => ac.Id == newArmor.ArmorCategoryId);
    if (!armorCategoryExists) return Results.BadRequest("Provided ArmorCategoryId does not exist.");

    newArmor.ArmorCategory = await dbContext.ArmorCategories
        .Include(ac => ac.Category)
        .FirstAsync(ac => ac.Id == newArmor.ArmorCategoryId);

    await dbContext.Armors.AddAsync(newArmor);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveNewArmor", newArmor);

    return Results.Created($"/armors/{newArmor.Id}", newArmor);
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapPut("/armors/{id}", async (AppDbContext dbContext, ClaimsPrincipal user, IHubContext<DataBaseUpdatesHub> hubContext, Armor updatedArmor, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var existingArmor = await dbContext.Armors
        .FirstOrDefaultAsync(a => a.Id == id);
    if (existingArmor == null) return Results.NotFound();

    existingArmor.Name = updatedArmor.Name;
    existingArmor.ArmorCategoryId = updatedArmor.ArmorCategoryId;
    existingArmor.NamePrefix = updatedArmor.NamePrefix;

    await dbContext.SaveChangesAsync();

    existingArmor = await dbContext.Armors
        .Include(a => a.ArmorCategory!)
            .ThenInclude(ac => ac.Category)
        .FirstOrDefaultAsync(a => a.Id == id);

    await hubContext.Clients.All.SendAsync("ReceiveUpdatedArmor", existingArmor);

    return Results.NoContent();

}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

app.MapDelete("/armors/{id}", async (AppDbContext dbContext, IHubContext<DataBaseUpdatesHub> hubContext, ClaimsPrincipal user, int id) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) return Results.Unauthorized();

    var armor = await dbContext.Armors.FindAsync(id);
    if (armor == null) return Results.NotFound();

    dbContext.Armors.Remove(armor);
    await dbContext.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveDeletedArmor", id);

    return Results.NoContent();
}).RequireAuthorization(policy => policy.RequireRole("Master", "Admin"));

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