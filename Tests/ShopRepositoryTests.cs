using Xunit;
using Moq;
using System.Threading.Tasks;
using EMS.BACKEND.API.Repositories;
using EMS.BACKEND.API.DTOs.Shop;
using EMS.BACKEND.API.Models;
using EMS.BACKEND.API.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EMS.BACKEND.API.Tests.Repositories
{
    public class ShopRepositoryTests
    {
       [Fact]
        public async Task CreateAsync_UserIsVendor_ReturnsAlreadyVendorMessage()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                           .ReturnsAsync(new[] { "vendor" });

            var mockConfiguration = new Mock<IConfiguration>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();

            var repository = new ShopRepository(mockUserManager.Object, mockConfiguration.Object,
                                                mockServiceScopeFactory.Object, mockCloudProvider.Object, mockTokenService.Object);

            var userId = "user@example.com";
            var entity = new ShopCreateDTO();

            // Act
            var result = await repository.CreateAsync(userId, entity);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User is already a vendor", result.Message);
        }
        
     /*  [Fact]
      public async Task CreateAsync_UserIsNotVendor_SuccessfullyCreatesShop()
     {
    // Arrange
    var mockUserManager = MockUserManager<ApplicationUser>();
    mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                   .ReturnsAsync(new List<string>());

    var mockConfiguration = new Mock<IConfiguration>();
    var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
    var mockCloudProvider = new Mock<ICloudProviderRepository>();
    var mockTokenService = new Mock<ITokenService>();

    var repository = new ShopRepository(mockUserManager.Object, mockConfiguration.Object,
                                        mockServiceScopeFactory.Object, mockCloudProvider.Object, mockTokenService.Object);

    var userId = "user@example.com";
    var entity = new ShopCreateDTO();

    // Act
    var result = await repository.CreateAsync(userId, entity);

    // Assert
    Assert.True(result.Flag); // Expect result.Flag to be True
    Assert.Equal("Shop created successfully", result.Message); // Adjust message if necessary
    }*/

        
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
