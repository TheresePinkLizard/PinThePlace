using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PinThePlace.Controllers;
using PinThePlace.DAL;
using PinThePlace.Models;
using PinThePlace.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;


namespace PinThePlace.Test.Controllers;

//   <---------- Unit test of FavoriteController ----------->
public class FavoriteControllerTests
{

    // Positiv test of Tabel(). 
    // Checks if result is of type ViewResult, ViewData.Model contains PinViewModel object and PinsViewModel matches the FavoriteList.
    [Fact]
    public async Task TestTable()
    {
        var favoriteList = new List<Favorite>()
        {
            new Favorite
                {   
                    FavoriteId = 1,
                    Category = "Resturant",
                    MadeBy = "Muncher",
                    PinId = 2,
                    UserId = "20"
                },

            new Favorite
                {
                    FavoriteId = 2,
                    Category = "Resturant",
                    MadeBy = "TheStudent",
                    PinId = 4,
                    UserId = "22"
                }
        };
        var pinList = new List<Pin>()
        {
            new Pin 
                {
                    Name = "Cafe",
                    Rating = 4.0m,
                    Comment = "Great cafe!",
                    Latitude = 59.91731919136782,
                    Longitude = 10.727738688356991,
                    UserName = "TheStudent",
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
        mockPinRepository.Setup(repo => repo.GetAllFavorites()).ReturnsAsync(favoriteList);
        mockPinRepository.Setup(repo => repo.GetAll()).ReturnsAsync(pinList);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null!,null!, null!, null!);

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object,mockUserManager.Object, mockLogger.Object);

        var result = await favoriteController.Table();

        var viewResult = Assert.IsType<ViewResult>(result); 
        var pinsViewModel = Assert.IsAssignableFrom<PinsViewModel>(viewResult.ViewData.Model); 
        Assert.Equal(2, pinsViewModel.Favorites.Count()); 
        Assert.Equal(2, pinsViewModel.Pins.Count()); 
        Assert.Equal(favoriteList, pinsViewModel.Favorites); 
        Assert.Equal(pinList, pinsViewModel.Pins); 
    }
    
    // Negative test of Tabel(). 
    // Checks if result is NotFound if list of Favorites is empty
    [Fact]
    public async Task TestTableNotOk()
    {
    
    var emptyFavoriteList = new List<Favorite>();

    var pinList = new List<Pin>
    {
        new Pin
        {
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "TheStudent",
            UserId = "20",
            ImageUrl = "/images/Cafe.png"
        }
    };

    var mockPinRepository = new Mock<IPinRepository>();
    mockPinRepository.Setup(repo => repo.GetAllFavorites()).ReturnsAsync(emptyFavoriteList);
    mockPinRepository.Setup(repo => repo.GetAll()).ReturnsAsync(pinList);

    var userStoreMock = new Mock<IUserStore<User>>();
    var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

    var mockLogger = new Mock<ILogger<FavoriteController>>();
    var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

    // Act
    var result = await favoriteController.Table();

    // Assert
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result); 
    Assert.Equal("Favorite list not found", notFoundResult.Value); 
    }

    // Positive test for [GET] AddToFavorites()
    // Checks if results is of type viewResults and with a favorite pin correspoinding with the favorized pin.
    [Fact]
    public async Task TestAddToFavorites_Get()
    {
        // arrange
        var testPin = new Pin
        {
            PinId = 2,
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "TheStudent",
            UserId = "20",
            ImageUrl = "/images/Cafe.png"
        };

        var testFavorite = new Favorite
        {
            PinId = testPin.PinId,
            UserId = "2",
            MadeBy = testPin.UserName
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(testPin.PinId)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("2");

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.AddToFavorites(testPin.PinId);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Favorite>(viewResult.Model); 

        Assert.Equal(testFavorite.PinId, model.PinId);
        Assert.Equal(testFavorite.UserId, model.UserId);
        Assert.Equal(testFavorite.MadeBy, model.MadeBy);
        Assert.Equal("CreateFavorite", viewResult.ViewName);
        }

    // Positive test for [Post] AddToFavorites
    // Checks if results is of type redirectResults and if it redirects to the correct view Table.
    [Fact]
    public async Task TestAddToFavorites_Post()
    {
        // arrange
        var testPin = new Pin
        {
            PinId = 2,
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "TheStudent",
            UserId = "20",
            ImageUrl = "/images/Cafe.png"
        };

        var testFavorite = new Favorite
        {
            PinId = testPin.PinId,
            UserId = "2",
            MadeBy = testPin.UserName
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(testFavorite.PinId)).ReturnsAsync(testPin);
        mockPinRepository.Setup(repo => repo.SaveFavorite(It.IsAny<Favorite>())).ReturnsAsync(true);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.FindByIdAsync(testFavorite.UserId)).ReturnsAsync(new User { Id = testFavorite.UserId });

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.AddToFavorites(testFavorite);

        // assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(PinController.Table), redirectResult.ActionName); 
        Assert.Equal("Pin", redirectResult.ControllerName); 
    }

    // Negativ test for AddToFavoritesNotOk_Post
    // Checks if the results is View "CreateFavorite" and the with the corresponding if saveFavorite does not work.  

    [Fact]
    public async Task TestAddToFavoritesNotOk_Post()
    {
        // arrange
        var testPin = new Pin
        {
            PinId = 2,
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "TheStudent",
            UserId = "20",
            ImageUrl = "/images/Cafe.png"
        };

        var testFavorite = new Favorite
        {
            PinId = testPin.PinId,
            UserId = "2",
            MadeBy = testPin.UserName
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetItemById(testFavorite.PinId)).ReturnsAsync(testPin);
        mockPinRepository.Setup(repo => repo.SaveFavorite(testFavorite)).ReturnsAsync(false); 

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,null!, null!, null!, null!, null! ,null!, null!, null!);
        mockUserManager.Setup(um => um.FindByIdAsync(testFavorite.UserId)).ReturnsAsync(new User { Id = testFavorite.UserId });

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        
        // act
        var result = await favoriteController.AddToFavorites(testFavorite);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result); 
        Assert.Equal("CreateFavorite", viewResult.ViewName); 
        var model = Assert.IsAssignableFrom<Favorite>(viewResult.Model); 
        Assert.Equal(testFavorite.PinId, model.PinId); 
        Assert.Equal(testFavorite.UserId, model.UserId); 
    }

    // Positive test for [Get] Update()
    // Checks if the Update method (GET) returns a ViewResult with the correct Pin model when the pin exists.

    [Fact]
    public async Task TestUpdateFavorite_Get()
    {
        // arrange
        var testFavorite = new Favorite
        {
            FavoriteId = 1,
            PinId = 2,
            UserId = "20",
            MadeBy = "Muncher"
        };

        var testPin = new Pin
        {
            PinId = 2,
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "Muncher",
            UserId = "19",
            ImageUrl = "/images/Cafe.png"
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetFavoriteById(testFavorite.FavoriteId)).ReturnsAsync(testFavorite);
        mockPinRepository.Setup(repo => repo.GetItemById(testFavorite.PinId)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheStudent");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("20");

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.UpdateFavorite(testFavorite.FavoriteId);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result); 
        var model = Assert.IsAssignableFrom<Favorite>(viewResult.Model); 

        Assert.Equal(testFavorite.FavoriteId, model.FavoriteId);
        Assert.Equal(testFavorite.PinId, model.PinId);
        Assert.Equal(testFavorite.UserId, model.UserId);
        Assert.Equal("Muncher", model.MadeBy); 
    }

    // Negative test for [Get] UpdateFavorite
    // Checks if the result is Unauthorized when the logged-in user is not the owner of the Favorite.

    [Fact]
    public async Task TestUpdateFavoriteNotOk_Get()
    {
        // arrange
        var testFavorite = new Favorite
        {
            FavoriteId = 1,
            PinId = 2,
            UserId = "20", // Owner of the favorite
            MadeBy = "Muncher"
        };

        var testPin = new Pin
        {
            PinId = 2,
            Name = "Cafe",
            Rating = 4.0m,
            Comment = "Great cafe!",
            Latitude = 59.91731919136782,
            Longitude = 10.727738688356991,
            UserName = "Muncher",
            UserId = "19",
            ImageUrl = "/images/Cafe.png"
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetFavoriteById(testFavorite.FavoriteId)).ReturnsAsync(testFavorite);
        mockPinRepository.Setup(repo => repo.GetItemById(testFavorite.PinId)).ReturnsAsync(testPin);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("21"); // Id not the same as the owner of favorite

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.UpdateFavorite(testFavorite.FavoriteId);

        // assert
        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result); 
    }

    // Positive test for [POST] UpdateFavorite
    // Checks if the result is a redirection to the Table action in the Pin controller when the favorite is successfully updated.
    [Fact]
    public async Task TestUpdateFavorite_Post()
    {
        // arrange
        var testFavorite = new Favorite
        {
            FavoriteId = 1,
            PinId = 2,
            UserId = "20",
            MadeBy = "Muncher"
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.UpdateFavorite(testFavorite)).ReturnsAsync(true); 

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.UpdateFavorite(testFavorite);

        // assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result); 
        Assert.Equal(nameof(PinController.Table), redirectResult.ActionName); 
        Assert.Equal("Pin", redirectResult.ControllerName); 
    }

    // Positive test for [GET] DeleteFavorite
    // Checks if the result is a ViewResult displaying the correct Favorite model for confirmation when the Favorite exists.
    [Fact]
    public async Task TestDeleteFavorite_Get()
    {
        // arrange
        var testFavorite = new Favorite
        {
            FavoriteId = 1,
            PinId = 2,
            UserId = "20",
            MadeBy = "Muncher"
        };

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetFavoriteById(testFavorite.FavoriteId)).ReturnsAsync(testFavorite); 

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("TheStudent");
        mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("20");

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.DeleteFavorite(testFavorite.FavoriteId);

        // assert
        var viewResult = Assert.IsType<ViewResult>(result); 
        var model = Assert.IsAssignableFrom<Favorite>(viewResult.Model); 
        Assert.Equal(testFavorite.FavoriteId, model.FavoriteId); 
    }

    // Negative test for [GET] DeleteFavorite
    // Checks if the result is a NotFoundObjectResult with an appropriate error message when the Favorite does not exist.
    [Fact]
    public async Task TestDeleteFavoriteNotOk_Get()
    {
        // arrange
        int favoriteId = 2;

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.GetFavoriteById(favoriteId)).ReturnsAsync(() => null);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.DeleteFavorite(favoriteId);

        // assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Favorite not found for the FavoriteId", notFoundResult.Value);
    }

    // Positive test for [POST] DeleteConfirmed
    // Checks if the result is a redirection to the Table action when the Favorite is successfully deleted.
    [Fact]
    public async Task TestDeleteConfirmed_Post()
    {
        // arrange
        int favoriteId = 1;

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.DeleteFavorite(favoriteId)).ReturnsAsync(true);

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.DeleteConfirmed(favoriteId);

        // assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result); 
        Assert.Equal(nameof(PinController.Table), redirectResult.ActionName); 
        Assert.Equal("Pin", redirectResult.ControllerName); 
    }

    // Negative test for [POST] DeleteConfirmed
    // Checks if the result is a BadRequestObjectResult with an appropriate error message when deletion fails.
    [Fact]
    public async Task TestDeleteConfirmedNotOk_Post()
    {
        // arrange
        int favoriteId = 1;

        var mockPinRepository = new Mock<IPinRepository>();
        mockPinRepository.Setup(repo => repo.DeleteFavorite(favoriteId)).ReturnsAsync(false); 

        var userStoreMock = new Mock<IUserStore<User>>();
        var mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<FavoriteController>>();
        var favoriteController = new FavoriteController(mockPinRepository.Object, mockUserManager.Object, mockLogger.Object);

        // act
        var result = await favoriteController.DeleteConfirmed(favoriteId);

        // assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); 
        Assert.Equal("Favorite deletion failed", badRequestResult.Value); 
    }
}