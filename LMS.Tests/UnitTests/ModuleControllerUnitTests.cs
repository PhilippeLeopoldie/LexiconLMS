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

    [Fact]
    public async Task GetAllModulesAsync_ShouldThrowCourseNotFoundException()
    {
        // Arrange

        int courseId = 99;
        var parameters = new ModuleRequestParams();

        _serviceManagerMock.Setup(s => s.ModuleService.GetAllModulesAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleRequestParams>(),
            It.IsAny<bool>()))
            .ThrowsAsync(new CourseNotFoundException(courseId));

        // Act & Assert
        await Assert.ThrowsAsync<CourseNotFoundException>(() => _controller.GetModules(courseId, parameters));

        _serviceManagerMock.Verify(service => service.ModuleService.GetAllModulesAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleRequestParams>(),
            It.IsAny<bool>()), Times.Once());

    }

    [Fact]
    public async Task GetModuleById_ReturnsOk_WithModule()
    {
        // Arrange
        int courseId = SeedData.GetCourse().Id;
        var module = SeedData.GetModuleDtos().First();

        _serviceManagerMock.Setup(s => s.ModuleService.GetModuleByIdAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<bool>()))
            .ReturnsAsync(module);

        // Act
        var result = await _controller.GetModuleById(courseId, module.Id, true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ModuleDto>(okResult.Value);
        Assert.Equal(module.Id, returnValue.Id);
    }

    [Fact]
    public async Task GetModuleById_ShouldThrowModuleNotFoundException()
    {
        // Arrange
        int courseId = SeedData.GetCourse().Id;
        var moduleId = 99;

        _serviceManagerMock.Setup(s => s.ModuleService.GetModuleByIdAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<bool>()))
            .ThrowsAsync(new ModuleNotFoundException(moduleId, courseId));

        // Act & Assert
        await Assert.ThrowsAsync<ModuleNotFoundException>(() => _controller.GetModuleById(courseId, moduleId, true));
        _serviceManagerMock.Verify(service => service.ModuleService.GetModuleByIdAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<bool>()), Times.Once());
    }

    [Fact]
    public async Task PostModule_ShouldReturnsCreatedAtAction()
    {
        // Arrange
        int courseId = 1;
        var dto = SeedData.GetModuleCreateDto();
        var created = new ModuleDto
        {
            Id = 10,
            Name = dto.Name,
            Description = dto.Description,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
        };

        _serviceManagerMock.Setup(s => s.ModuleService.CreateModuleAsync(It.IsAny<int>(), It.IsAny<ModuleCreateDto>()))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.PostModule(courseId, dto);

        // Assert
        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<ModuleDto>(createdAt.Value);
        Assert.Equal(10, returnValue.Id);
    }

    [Fact]
    public async Task PostModule_ShouldThrowException_whenModuleStartDateAfterEndDate ()
    {
        // Arrange
        int courseId = 1;
        var errorMessage = "Start date must be before end date.";
        var dto = SeedData.GetModuleCreateDto();
        var created = new ModuleDto
        {
            Id = 10,
            Name = dto.Name,
            Description = dto.Description,
            StartsAt = dto.EndsAt,
            EndsAt = dto.StartsAt,
        };

        _serviceManagerMock.Setup(s => s.ModuleService.CreateModuleAsync(It.IsAny<int>(), It.IsAny<ModuleCreateDto>()))
            .ThrowsAsync(new BadRequestException(errorMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _controller.PostModule(courseId, dto));
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.CreateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleCreateDto>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task PostModule_ShouldThrowException_whenCourseNotFound()
    {
        // Arrange
        int courseId = 1;
        var errorMessage = $"No Course with id: {courseId}  found!";
        var dto = SeedData.GetModuleCreateDto();
        var created = new ModuleDto
        {
            Id = 10,
            Name = dto.Name,
            Description = dto.Description,
            StartsAt = dto.EndsAt,
            EndsAt = dto.StartsAt,
        };

        _serviceManagerMock.Setup(s => s.ModuleService.CreateModuleAsync(It.IsAny<int>(), It.IsAny<ModuleCreateDto>()))
            .ThrowsAsync(new CourseNotFoundException(courseId));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CourseNotFoundException>(() => _controller.PostModule(courseId, dto));
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.CreateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleCreateDto>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task PostModule_ShouldThrowException_whenModuleOverlap()
    {
        // Arrange
        int courseId = 1;
        var dto = SeedData.GetModuleCreateDto();
        var created = new ModuleDto
        {
            Id = 10,
            Name = dto.Name,
            Description = dto.Description,
            StartsAt = dto.EndsAt,
            EndsAt = dto.StartsAt,
        };
        var errorMessage = $"Module must be within course dates: {created.StartsAt} - {created.EndsAt}.";

        _serviceManagerMock.Setup(s => s.ModuleService.CreateModuleAsync(It.IsAny<int>(), It.IsAny<ModuleCreateDto>()))
            .ThrowsAsync(new ModuleOverlappingException(created.StartsAt, created.EndsAt));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ModuleOverlappingException>(() => _controller.PostModule(courseId, dto));
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.CreateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleCreateDto>()),
            Times.Once()
        );
    }


    [Fact]
    public async Task DeleteModule_ShouldReturnsNoContent()
    {
        // Arrange
        int courseId = 1, id = 7;

        _serviceManagerMock.Setup(s => s.ModuleService.DeleteModuleAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteModuleAsync(courseId, id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

}
