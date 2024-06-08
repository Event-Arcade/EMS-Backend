using EMS.BACKEND.API.DbContext;
using EMS.BACKEND.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Net.Http.Headers;
using EMS.BACKEND.API.Repositories;
using SharedClassLibrary.Contracts;
using EMS.BACKEND.API.Contracts;
using Amazon.S3;
using Contracts;
using EMS.BACKEND.API.Controllers;
using EMS.BACKEND.API.Service;
using EMS.BACKEND.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

//aws s3 configuration with credentials
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
builder.Services.AddAWSService<IAmazonS3>();

//Db configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        //get connection string from appsettings.json
        options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")??
            throw new InvalidOperationException("Connection string is not found"));
    });

//Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(config =>
    {
        //TODO : implement email verification
        //config.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<IdentityRole>();


//Access the http content
builder.Services.AddHttpContextAccessor();


//JWT
builder.Services.AddAuthentication(options =>
{
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})

// Add google authentication
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"];
    options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"];
});

//Add authentication to Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddScoped<IUserAccountRepository, AccountRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IShopServiceRepository, ShopServiceRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICloudProviderRepository, CloudProviderRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IAdminStaticResourceRepository, AdminStaticResourceRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
            .SetIsOriginAllowed((x) => true)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
        .WithHeaders(HeaderNames.ContentType);
        });
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();
app.MapHub<EMSHub>("/personalChatHub");


//seeding user-roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    //roles in the application
    var roles = new[] { "vendor", "client", "admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    //seeding admin user
    var adminUser = new ApplicationUser
    {
        FirstName = "admin",
        LastName = "admin",
        Email = config["ADMIN_EMAIL"] ,
        UserName = config["ADMIN_EMAIL"] ,
        Street = "admin street",
        City = "admin city",
        PostalCode = "admin postal code",
        Province = "admin province",
        Longitude = 0,
        Latitude = 0
    };

if (await userManager.FindByEmailAsync(adminUser.Email) == null)
{
    var response = await userManager.CreateAsync(adminUser, config["ADMIN_PASSWORD"]);
    if (response.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "admin");
    }
}

// update the shopservice ratings according to the its feedbacks rating average
var shopServices = await dbContext.ShopServices.ToListAsync();
if (shopServices != null)
{
    foreach (var shopService in shopServices)
    {
        var feedbacks = await dbContext.FeedBacks.Where(f => f.ServiceId == shopService.Id).ToListAsync();
        if (feedbacks.Count > 0)
        {
            var rating = feedbacks.Average(f => f.Rating);
            shopService.Rating = rating;
            dbContext.ShopServices.Update(shopService);
            await dbContext.SaveChangesAsync();
        }
    }
}

// update the shop ratings according to the its shopService rating average
var shops = await dbContext.Shops.ToListAsync();
if (shops != null)
{
    foreach (var shop in shops)
    {
        var tempShopServices = await dbContext.ShopServices.Where(s => s.ShopId == shop.Id).ToListAsync();
        if (tempShopServices != null)
        {
            if (tempShopServices.Count > 0)
            {
                var rating = tempShopServices.Average(s => s.Rating);
                shop.Rating = rating;
                dbContext.Shops.Update(shop);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
}


app.Run();
