using Xunit;
using Moq;
using System.Threading.Tasks;
using EMS.BACKEND.API.Repositories;
using EMS.BACKEND.API.DTOs.Account;
using EMS.BACKEND.API.Models;
using EMS.BACKEND.API.Contracts;
using Microsoft.AspNetCore.Identity;
using EMS.BACKEND.API.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EMS.BACKEND.API.Tests.Repositories
{
    public class AccountRepositoryTests
    {
        [Fact]
        public async Task CreateAccountAsync_UserAlreadyExists_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object,mockNotification.Object, mockCloudProvider.Object, null, null);
            var registerUser = new RegisterUserDTO { Email = "existinguser@gmail.com" };

            mockUserManager.Setup(m => m.FindByEmailAsync(registerUser.Email))
                           .ReturnsAsync(new ApplicationUser());

            // Act
            var result = await accountRepository.CreateAccountAsync(registerUser);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User already registered", result.Message);
        }

        [Fact]
        public async Task LoginAccountAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object,mockNotification.Object, mockCloudProvider.Object, null, null);
            var loginDTO = new LoginDTO { Email = "nonexistinguser@gmail.com", Password = "password" };

            mockUserManager.Setup(m => m.FindByEmailAsync(loginDTO.Email))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await accountRepository.LoginAccountAsync(loginDTO);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task UpdateAccountAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object, mockNotification.Object,mockCloudProvider.Object, null, null);
            var updateUserDTO = new UpdateUserDTO();
            var userId = "123";

            mockUserManager.Setup(m => m.FindByIdAsync(userId))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await accountRepository.UpdateAccountAsync(userId, updateUserDTO);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task GetAccountByIdAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object, mockNotification.Object,mockCloudProvider.Object, null, null);
            var userId = "123";

            mockUserManager.Setup(m => m.FindByIdAsync(userId))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await accountRepository.GetAccountByIdAsync(userId);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task DeleteAccountAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object, mockNotification.Object,mockCloudProvider.Object, null, null);
            var userId = "123";

            mockUserManager.Setup(m => m.FindByIdAsync(userId))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await accountRepository.DeleteAccountAsync(userId);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task UpdateAccountPasswordAsync_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var mockUserManager = MockUserManager<ApplicationUser>();
            var mockCloudProvider = new Mock<ICloudProviderRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockNotification = new Mock<INotificationRepository>();
            var accountRepository = new AccountRepository(mockTokenService.Object, mockUserManager.Object, mockNotification.Object,mockCloudProvider.Object, null, null);
            var updatePasswordDTO = new UpdatePasswordDTO();
            var userId = "123";

            mockUserManager.Setup(m => m.FindByIdAsync(userId))
                   .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await accountRepository.UpdateAccountPasswordAsync(userId, updatePasswordDTO);

            // Assert
            Assert.False(result.Flag);
            Assert.Equal("Invalid password", result.Message);
        }

        // Helper method to mock UserManager
        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null,null);
        }
    }
}
