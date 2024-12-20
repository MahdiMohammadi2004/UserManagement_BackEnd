using Moq;
using Microsoft.AspNetCore.Mvc;
using User_Managment.Controllers;
using User_Managment.Services;
using User_Managment.Models;
using System.Threading.Tasks;
using Xunit;

public class UserControllerTests
{
    private readonly ILogger<UserController> _logger;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object, _logger);
    }



    [Fact]
    public async Task GetAllUsers_ShouldReturnOkResult_WhenUsersExist()
    {
        // Arrange
        var users = new[] { new User { Username = "user1" }, new User { Username = "user2" } };
        _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userController.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<User[]>(okResult.Value);
        Assert.Equal(2, returnValue.Length);
    }
}
