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

public class PinControllerTests
{
    [Fact]
    public async Task TestTable()
    {
        var pinList = new List<Pin>()
        
        {
            new Pin 
                {
                    Name = "Cafe",
                    Rating = 4.0m,
                    Comment = "Great cafe!",
                    Latitude = 59.91731919136782,
                    Longitude = 10.727738688356991,
                    UserName = "CoolKid",
                    UserId = "20",
                    ImageUrl = "/images/Cafe.png",
                },

            new Pin 
                {
                    Name = "SwimmingPool",
                    Rating = 5.0m,
                    Comment = "Refreshing, can recommend!",
                    Latitude = 59.921365321156706, 
                    Longitude = 10.733315263484577,
                    UserName = "TheMermaid",
                    UserId = "21",
                    ImageUrl = "/images/Pool.png",
                }   
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetAll()).ReturnsAsync(pinList);
        
        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null, null, null, null, null ,null, null, null);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await pinController.Table();

        var viewResult = Assert.IsType<ViewResult>(result);
        var pinsViewModel = Assert.IsAssignableFrom<PinsViewModel>(viewResult.ViewData.Model);
        Assert.Equal(2, pinsViewModel.Pins.Count());
        Assert.Equal(pinList, pinsViewModel.Pins);
    }

    [Fact]
    public async Task TestCreateNotLoggedIn()
    {
        // arrange
        var testPin = new Pin
        {
            Name = "SwimmingPool",
            Rating = 5.0m,
            Comment = "Refreshing, can recommend!",
            Latitude = 59.921365321156706, 
            Longitude = 10.733315263484577,
            UserName = "TheMermaid",
            UserId = "21",
            ImageUrl = "/images/Pool.png",
        };  

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.Create(testPin)).ReturnsAsync(false);


        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null, null, null, null, null ,null, null, null);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns((string)null);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Create(testPin);

        // assert
       // var viewResult = Assert.IsType<UnauthorizedResult>(result);
       // var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task TestCreateNotOk()
    {
        // arrange
        var testPin = new Pin
        {
            Name = "SwimmingPool",
            Rating = 5.0m,
            Comment = "Refreshing, can recommend!",
            Latitude = 59.921365321156706, 
            Longitude = 10.733315263484577,
            UserName = "TheMermaid",
            UserId = "21",
            ImageUrl = "/images/Pool.png",
        };  

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.Create(testPin)).ReturnsAsync(false);


        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null, null, null, null, null ,null, null, null);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TestUser");

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Create(testPin);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, viewPin);
    }
}


