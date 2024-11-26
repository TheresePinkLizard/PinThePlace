using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PinThePlace.Controllers;
using PinThePlace.DAL;
using PinThePlace.Models;
using PinThePlace.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace PinThePlace.Test.Controllers;

public class UserControllerTests
{
    // Positiv test of Tabel(). 
    // Checks if result is of type ViewResult, model is a List of Users and model matches the UserList.
    [Fact]
    public async Task TestTable()
    {
        // Arrange
        var users = new List<User>
            {
                new User {UserName = "TheStudent", Email="thestudent@gmail.com"} ,
                new User {UserName = "Muncher", Email="muncher@gmail.com"} ,
            };

    // Mock DbSet<User>
    var mockUserDbSet = new Mock<DbSet<User>>();
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userList.AsQueryable().Provider);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userList.AsQueryable().Expression);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userList.AsQueryable().ElementType);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userList.AsQueryable().GetEnumerator());

    // Mock DbContext
    var mockPinDbContext = new Mock<PinDbContext>(new DbContextOptions<PinDbContext>());
    mockPinDbContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);

    // Mock UserManager
    var userStoreMock = new Mock<IUserStore<User>>();
    var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
    mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("Admin");

    // Mock logger
    var mockLogger = new Mock<ILogger<UserController>>();

    // Create controller
    var userController = new UserController(mockUserManager.Object, mockPinDbContext.Object, mockLogger.Object);

    // Act
    var result = await userController.Table();

    // Assert
    var viewResult = Assert.IsType<ViewResult>(result); // Sjekk at resultatet er en ViewResult
    var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model); // Sjekk at modellen er en liste av brukere
    Assert.Equal(2, model.Count); // Sjekk at modellen inneholder 2 brukere
    Assert.Equal(userList, model); // Sjekk at listen matcher forventet data
    }
                    

    // Negative test of Tabel(). 
    // Checks if user is not Admin it results in unauthorized.
    [Fact]
    public async Task TestTableNotAdmin()
    {

    // Mock DbSet<User>
    var mockUserDbSet = new Mock<DbSet<User>>();
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userList.AsQueryable().Provider);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userList.AsQueryable().Expression);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userList.AsQueryable().ElementType);
    mockUserDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userList.AsQueryable().GetEnumerator());

    // Mock DbContext
    var mockPinDbContext = new Mock<PinDbContext>(new DbContextOptions<PinDbContext>());
    mockPinDbContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);

    // Mock UserManager
    var userStoreMock = new Mock<IUserStore<User>>();
    var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
    mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheStudent");

    // Mock logger
    var mockLogger = new Mock<ILogger<UserController>>();

    // Create controller
    var userController = new UserController(mockUserManager.Object, mockPinDbContext.Object, mockLogger.Object);

    // Act
    var result = await userController.Table();

    // Assert
    var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result); // Forvent UnauthorizedResult

    }

    // Positiv test of MyPins(). 
    // Checks if result is of type ViewResult, model is a List of Users and model matches the UserList.

    [Fact]
    public async Task MyPins_ReturnsView_WithUserPins()
    {
        // Arrange
        var pins = new List<Pin>
        {
            new Pin { Id = 1, Name = "Cafe", Comment = "Great spot!" },
            new Pin { Id = 2, Name = "Park", Comment = "Beautiful place!" }
        };
        var user = new User { userId = 1, UserName = "TheStudent", Pins = pins };

        // Mock DbSet<User>
        var mockUserDbSet = new Mock<DbSet<User>>();
        mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new List<User> { user }.AsQueryable().Provider);
        mockUserDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(new List<User> { user }.AsQueryable().Expression);
        mockUserDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(new List<User> { user }.AsQueryable().ElementType);
        mockUserDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(new List<User> { user }.AsQueryable().GetEnumerator());

        // Mock DbContext
        var mockPinDbContext = new Mock<PinDbContext>(new DbContextOptions<PinDbContext>());
        mockPinDbContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);

        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

        // Mock logger
        var mockLogger = new Mock<ILogger<UserController>>();

        // Create controller
        var userController = new UserController(mockUserManager.Object, mockPinDbContext.Object, mockLogger.Object);

        // Act
        var result = await userController.MyPins();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result); // Sjekk at resultatet er en ViewResult
        var model = Assert.IsAssignableFrom<List<Pin>>(viewResult.Model); // Sjekk at modellen er en liste av pins
        Assert.Equal(pins.Count, model.Count); // Sjekk at riktig antall pins returneres
        Assert.Equal(pins, model); // Sjekk at listen matcher forventet data
    }



    // Negative test of Tabel(). 
    // Checks if user is not Admin it results in unauthorized.








}

