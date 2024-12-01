using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PinThePlace.Controllers;
using PinThePlace.DAL;
using PinThePlace.Models;
using PinThePlace.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System.Linq;
using System.Collections.Generic;


namespace PinThePlace.Test.Controllers;

public class UserControllerTests
{
    // Positive test of Tabel(). 
    // Checks if result is of type ViewResult, model is a List of Users and model matches the UserList.

    [Fact]
    public async Task TestTable()
    {
        var userList = new List<User>()
        {
            new User { UserName = "TheStudent", Email = "thestudent@gmail.com" },
            new User { UserName = "Muncher", Email = "muncher@gmail.com" }
        };

        var options = new DbContextOptionsBuilder<PinDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase_Table_Positive").Options;

        using var mockContext = new PinDbContext(options);
        mockContext.Users.AddRange(userList);
        mockContext.SaveChanges();

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("Admin");

        var mockLogger = new Mock<ILogger<UserController>>();
        var userController = new UserController(mockContext, mockUserManager.Object, mockLogger.Object);

        var result = await userController.Table();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
        Assert.Equal(2, model.Count);
        Assert.Equal(userList.Select(u => u.UserName), model.Select(u => u.UserName));
        Assert.Equal(userList.Select(u => u.Email), model.Select(u => u.Email));
    }
    
     // Negative test of Tabel(). 
    // Checks if user is not Admin it results in unauthorized.
    [Fact]
    public async Task TestTableNotAdmin()
    {
        var userList = new List<User>()
            {
                new User { UserName = "TheStudent", Email = "thestudent@gmail.com" },
                new User { UserName = "Muncher", Email = "muncher@gmail.com" }
            };

        var options = new DbContextOptionsBuilder<PinDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase_Table_Negative").Options;

        using var mockContext = new PinDbContext(options);
        mockContext.Users.AddRange(userList);
        mockContext.SaveChanges();

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheStudent");

        var mockLogger = new Mock<ILogger<UserController>>();
        var userController = new UserController(mockContext, mockUserManager.Object, mockLogger.Object);

        var result = await userController.Table();

        Assert.IsType<UnauthorizedResult>(result);
    }

    // Positiv test of MyPins(). 
    // Checks if result is of type ViewResult, model is a List of pins and model matches the UserList.
    [Fact]
    public async Task TestMyPins()
    {
        var pinList = new List<Pin>
        {
            new Pin { PinId = 1, Name = "Cafe", UserId = "1", UserName = "TheStudent" },
            new Pin { PinId = 2, Name = "Library", UserId = "1", UserName = "TheStudent" }
        };

        var user = new User { Id = "1", UserName = "TheStudent", Pins = pinList };

        var options = new DbContextOptionsBuilder<PinDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase_MyPins").Options;

        using var mockContext = new PinDbContext(options);
        mockContext.Users.Add(user);
        mockContext.Pins.AddRange(pinList);
        mockContext.SaveChanges();

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("1");

        var mockLogger = new Mock<ILogger<UserController>>();
        var userController = new UserController(mockContext, mockUserManager.Object, mockLogger.Object);

        var result = await userController.MyPins();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Pin>>(viewResult.Model);
        Assert.Equal(2, model.Count);
        Assert.Equal(pinList.Select(p => p.Name), model.Select(p => p.Name));
    }

    // Negative test for MyPins()
    // Checks if the result is NotFound when the user or their pins are not found in the database.
    [Fact]
    public async Task TestMyPinsNotOk()
    {
        // Arrange
        var user = new User { Id = "1", UserName = "TheStudent"};

        var options = new DbContextOptionsBuilder<PinDbContext>().UseInMemoryDatabase("TestDatabase").Options;

        // Create a mock DbContext with no users
        var mockPinDbContext = new PinDbContext(options);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id); 
       
        var mockLogger = new Mock<ILogger<UserController>>();

        var userController = new UserController(mockPinDbContext, mockUserManager.Object, mockLogger.Object);

        // Act
        var result = await userController.MyPins();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User with was not found.", notFoundResult.Value); 
    }
}

