using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nba_mvc.Data;
using nba_mvc.Hubs;
using nba_mvc.Repositories.ActionEvent;
using nba_mvc.Repositories.Arena;
using nba_mvc.Repositories.Coach;
using nba_mvc.Repositories.Game;
using nba_mvc.Repositories.Player;
using nba_mvc.Repositories.Referee;
using nba_mvc.Repositories.Team;
using nba_mvc.Services.ActionEvent;
using nba_mvc.Services.Arena;
using nba_mvc.Services.Auth;
using nba_mvc.Services.Coach;
using nba_mvc.Services.Game;
using nba_mvc.Services.Image;
using nba_mvc.Services.Player;
using nba_mvc.Services.Referee;
using nba_mvc.Services.Simulation;
using nba_mvc.Services.Stats;
using nba_mvc.Services.Team;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT token below (no need to type 'Bearer ' prefix)."
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

// Repositories
builder.Services.AddScoped<IArenaRepository, ArenaRepository>();
builder.Services.AddScoped<IRefereeRepository, RefereeRepository>();
builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IActionEventRepository, ActionEventRepository>();

// Services
builder.Services.AddScoped<IArenaService, ArenaService>();
builder.Services.AddScoped<IRefereeService, RefereeService>();
builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IActionEventService, ActionEventService>();
builder.Services.AddScoped<IGameStatsService, GameStatsService>();
builder.Services.AddScoped<IStandingsService, StandingsService>();

// SignalR
builder.Services.AddSignalR();

// Images
builder.Services.AddHttpContextAccessor();

// Game Simulation
builder.Services.AddSingleton<IGameSimulationStateStore, GameSimulationStateStore>();
builder.Services.AddScoped<IGameSimulationEngine, GameSimulationEngine>();
builder.Services.AddHostedService<GameSimulationBackgroundService>();

var imageStorage = builder.Configuration["ImageStorage"];
if (imageStorage == "Cloudinary")
{
    builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
    builder.Services.AddScoped<IImageUploader, CloudinaryImageUploader>();
}
else
{
    builder.Services.AddScoped<IImageUploader, LocalImageUploader>();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRoles(roleManager);

    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

app.Run();