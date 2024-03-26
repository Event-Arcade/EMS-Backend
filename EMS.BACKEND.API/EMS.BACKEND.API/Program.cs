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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//aws s3 configuration
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

//Db configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("default") ??
            throw new InvalidOperationException("Connection string is not found"));
    });

//Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(config =>
    {
        //todo : implement email verification
        //config.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<IdentityRole>();


//Access the http content
builder.Services.AddHttpContextAccessor();

//JWT
builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;}).AddJwtBearer(options =>{
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
builder.Services.AddScoped<IShopServiceRepository, ShopServiceRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICloudProviderRepository, CloudProviderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithHeaders(HeaderNames.ContentType);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

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
        FirstName = "admin1",
        LastName = "admin2",
        PasswordHash = "Admin@123",
        UserName = "admin@gmail.com",
        Email = "admin@gmail.com"
    };
    if (await userManager.FindByEmailAsync(adminUser.Email) == null)
    {
        var response = await userManager.CreateAsync(adminUser, "Admin@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }

    //seeding client user
    var clientUser = new ApplicationUser
    {
        FirstName = "client",
        LastName = "client",
        PasswordHash = "Client@123",
        UserName = "client@gmail.com",
        Email = "client@gmail.com"
    };
    if (await userManager.FindByEmailAsync(clientUser.Email) == null)
    {
        var response = await userManager.CreateAsync(clientUser, "Client@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(clientUser, "client");
        }
    }

    //seeding vendor user
    var vendorUser = new ApplicationUser
    {
        FirstName = "vendor",
        LastName = "vendor",
        PasswordHash = "Vendor@123",
        UserName = "vendor@gmail.com",
        Email = "vendor@gmail.com"
    };
    if (await userManager.FindByEmailAsync(vendorUser.Email) == null)
    {
        var response = await userManager.CreateAsync(vendorUser, "Vendor@123");
        if (response.Succeeded)
        {
            await userManager.AddToRoleAsync(vendorUser, "vendor");
        }
    }

    //seeding categories
    if (!dbContext.Categories.Any())
    {
        var categories = new[] { "Entertainment", "Decoration", "Catering", "Transport" };
        foreach (var category in categories)
        {
            dbContext.Categories.Add(new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = category
            });
        }
        dbContext.SaveChanges();
    }

    //seeding shop for vendorUser
    if (!dbContext.Shops.Any())
    {
        var shop = await dbContext.Shops.Where(s => s.Name == "shop1").FirstOrDefaultAsync();
        try
        {
            var exsistedVendor = await userManager.FindByEmailAsync("vendor@gmail.com");
            var newShop = new Shop()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "shop1",
                Description = "shop1 description",
                Rating = 4,
                OwnerId = exsistedVendor.Id
            };

            await dbContext.Shops.AddAsync(newShop);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurd when seeeding a new shop: {ex}");
        }
    }


    //seeding services
    if (!dbContext.Services.Any())
    {
        var transportCategory = dbContext.Categories.FirstOrDefault(c => c.Name == "Transport");
        var shop1 = await dbContext.Shops.Where(s => s.Name == "shop1").FirstOrDefaultAsync();
        try
        {
            var services = new List<Service>
                    {
                        new Service
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Service1",
                            Price = 1000,
                            ShopId = shop1.Id,
                            CategoryId = transportCategory.Id
                        },
                        new Service
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Service2",
                            Price = 2000,
                            ShopId =    shop1.Id,
                            CategoryId = transportCategory.Id
                        },
                        new Service
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Service3",
                            Price = 3000,
                            ShopId = shop1.Id,
                            CategoryId = transportCategory.Id
                        }
                    };

            dbContext.Services.AddRange(services);
            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


}

app.Run();
