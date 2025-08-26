using Domain.Models.Exceptions;
using LMS.Presentation.Controllers;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;

namespace LMS.Tests.UnitTests;

public class ModuleControllerUnitTests
{

    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly ModuleController _controller;


    public ModuleControllerUnitTests()
    {
        _serviceManagerMock = new Mock<IServiceManager>();
        _controller = new ModuleController(_serviceManagerMock.Object);

        // Needed for Response.Headers
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task GetAllModulesAsync_ShouldReturnsOk_WithModules()
    {
        // Arrange

        int courseId = 1;
        var parameters = new ModuleRequestParams();
        var modules = SeedData.GetModuleDtos();
        var metadata = new MetaData(1, 1, 5, 2);

        _serviceManagerMock.Setup(s => s.ModuleService.GetAllModulesAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleRequestParams>(),
            It.IsAny<bool>()))
            .ReturnsAsync((modules, metadata));

        // Act
        var result = await _controller.GetModules(courseId, parameters);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ModuleDto>>(okResult.Value);
        _serviceManagerMock.Verify(service => service.ModuleService.GetAllModulesAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleRequestParams>(),
            It.IsAny<bool>()), Times.Once());
        Assert.Equal(2, returnValue.Count());
        Assert.Collection(returnValue,
        module => Assert.Equal("C# Fundamentals", module.Name),
        module => Assert.Equal("Web Development with React", module.Name));
    }

    

}
