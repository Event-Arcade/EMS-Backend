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
using EMS.BACKEND.API.Hubs;
using EMS.BACKEND.API.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

//aws s3 configuration
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

//Db configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        //get connection string from appsettings.json
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDbConnection") ??
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
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
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

// 
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithHeaders(HeaderNames.ContentType);
                      });
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(MyAllowSpecificOrigins);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<PersonalChatHub>("/personalChatHub");

//seeding user-roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
        Email = "admin@gmail.com",
        UserName = "admin@gmail.com",
        Street = "admin street",
        City = "admin city",
        PostalCode = "admin postal code",
        Province = "admin province",
        Longitude = 0,
        Latitude = 0
    };
    if (await userManager.FindByEmailAsync(adminUser.Email) == null)
    {
        var response = await userManager.CreateAsync(adminUser, "Admin@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }

    //seeding 7 vendor user
    for(int i=1;i<=7;i++){
        string email = "vendor"+i.ToString()+"@gmail.com";
        var vendorUser = new ApplicationUser
    {
        FirstName = "vendor",
        LastName = "vendor",
        UserName =email,
        Email = email,
        Street = "vendor street",
        City = "vendor city",
        PostalCode = "vendor postal code",
        Province = "vendor province",
        Longitude = 0,
        Latitude = 0
    };
    if (await userManager.FindByEmailAsync(vendorUser.Email) == null)
    {
        var response = await userManager.CreateAsync(vendorUser, "Vendor@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(vendorUser, "vendor");
        }
    }
    }

    //seeding client user
    var clientUser = new ApplicationUser
    {
        FirstName = "client",
        LastName = "client",
        UserName = "client@gmail.com",
        Email = "client@gmail.com",
        Street = "client street",
        City = "client city",
        PostalCode = "client postal code",
        Province = "client province",
        Longitude = 0,
        Latitude = 0
    };
    if (await userManager.FindByEmailAsync(clientUser.Email) == null)
    {
        var response = await userManager.CreateAsync(clientUser, "Client@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(clientUser, "client");
        }
    }
}



app.Run();
