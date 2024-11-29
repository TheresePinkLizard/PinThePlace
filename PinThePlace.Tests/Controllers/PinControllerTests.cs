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

//   <---------- Unit test of PinController ----------->
public class PinControllerTests
{

    // Positiv test of Tabel(). 
    // Checks if result is of type ViewResult, ViewData.Model contains PinViewModel object and PinsViewModel matches the PinList.
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
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await pinController.Table();

        var viewResult = Assert.IsType<ViewResult>(result);
        var pinsViewModel = Assert.IsAssignableFrom<PinsViewModel>(viewResult.ViewData.Model);
        Assert.Equal(2, pinsViewModel.Pins.Count());
        Assert.Equal(pinList, pinsViewModel.Pins);
    }

    // Negative test of Tabel(). 
    // Checks if result is NotFound if list of Pins is Null.
    [Fact]
    public async Task TestTableNotOk()
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
        mockPinRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Pin>());
        
        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await pinController.Table();

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pin list not found", notFoundResult.Value);
    }

    // Positiv test for Details (int id)
    // Checks if result is of type ViewResult, ViewData.Model contains Pin object and that it matches the testPin.
    [Fact]
    public async Task TestDetails()
    {
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
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(testPin);
        
        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await pinController.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, model);
    }

    // Negative test for Details()
    // Checks that Detail() returns NotFound when pin = null and validates the error message.
    [Fact]
    public async Task TestDetailsNotOk()
    {
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(() =>null);
        
        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await pinController.Details(1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pin not found for the PinId", notFoundResult.Value);
    }
    
    // Positive test for Create()
    // Checks if results is of type redirectResults and if it redirects to the correct view Table.
    [Fact]
    public async Task TestCreate()
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
        mockPinRepository.Setup(repo => repo.Create(testPin)).ReturnsAsync(true);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TestUser");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Create(testPin);

        // assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(pinController.Table), redirectResult.ActionName);
    }
    
    // Negative test for Create()
    // Checks if the Create method returns Unauthorized when the user is not logged in.
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
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns((string)null!);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // Act
        var result = await pinController.Create(testPin);
        // Assert
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
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TestUser");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Create(testPin);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, viewPin);
    }

    // Positive test for [Get] Update()
    // Checks if the Update method (GET) returns a ViewResult with the correct Pin model when the pin exists.
    [Fact]
    public async Task TestUpdate_Get()
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
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Update(1);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, viewPin);
    }
    

    // Negative test for [Get] Update()
    // Checks if results is of type NotFound when pin = null and validates the error message
    [Fact]
    public async Task TestUpdate_Get_NotOk()
    {
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(() => null!);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Update(1);

        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pin not found for the pinId", notFoundResult.Value);
    }

    // Positive test for [Post] Update()
    // Checks if results is of type redirectResults and if it redirects to the correct view Table.
    [Fact]
    public async Task TestUpdate_Post()
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
        mockPinRepository.Setup(repo => repo.Update(testPin)).ReturnsAsync(true);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TestUser");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Update(testPin);

        // assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(pinController.Table), redirectResult.ActionName);
    }
 
    // Negative test for [Post] Update
    // Checks if result is of type ViewResult, ViewData.Model contains Pin Object that it matches the testPin.
    [Fact]
    public async Task TestUpdate_Post_NotOk()
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
        mockPinRepository.Setup(repo => repo.Update(testPin)).ReturnsAsync(false);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TestUser");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Update(testPin);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, viewPin);
    }

    // Positive test for [Get] Delete() 
    // Checks if results is of type redirectResults and if it redirects to the correct view Table.
    [Fact]
    public async Task TestDelete_Get()
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
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Delete(1);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewPin = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, viewPin);
    }
    

    // Negative test for [Get] Delete ()
    // Checks if results is of type NotFound when pin = null and validates the error message
    [Fact]
    public async Task TestDelete_Get_NotOk()
    {
        
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(() => null!);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("TestUserId");
        
        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        // act
        var result = await pinController.Delete(1);

        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pin not found for the PinId", notFoundResult.Value);
    }

    // Positive test for [Get] DeleteConfirmation()
    // Checks if DeleteConfirmation returns a ViewResult with the correct Pin model when the pin exists.
    [Fact]
    public async Task TestDeleteConfirmation_Get()
    {
        // Arrange
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
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // Act
        var result = await pinController.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Pin>(viewResult.ViewData.Model);
        Assert.Equal(testPin, model);
    }

    // Negative test for [Get] DeleteConfirmation()
    // Checks if DeleteConfirmation returns NotFound when the pin does not exist.
    [Fact]
    public async Task TestDeleteConfirmation_Get_NotOk()
    {
        // Arrange
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(1)).ReturnsAsync(() => null!);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheMermaid");

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // Act
        var result = await pinController.Delete(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Pin not found for the PinId", notFoundResult.Value);
    }

    // Positive test for [Post] DeleteConfirmation()
    // Checks if DeleteConfirmed returns a RedirectToActionResult and redirects to the Table view when deletion is successful.
    [Fact]
    public async Task TestDeleteConfirmed_Post()
    {
        // Arrange
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.Delete(1)).ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object, null!, mockLogger.Object);

        // Act
        var result = await pinController.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(pinController.Table), redirectResult.ActionName);
    }

    // Negative test for [Post] DeleteConfirmation()
    // Checks if DeleteConfirmed returns a BadRequestObjectResult with the correct error message when deletion fails.
    [Fact]
    public async Task TestDeleteConfirmed_Post_NotOk()
    {
        // Arrange
        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.Delete(1)).ReturnsAsync(false);

        var mockLogger = new Mock<ILogger<PinController>>();
        var pinController = new PinController(mockPinRepository.Object, null!, mockLogger.Object);

        // Act
        var result = await pinController.DeleteConfirmed(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Pin deletion failed", badRequestResult.Value);
    }
}
