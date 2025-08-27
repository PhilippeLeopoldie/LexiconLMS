using Domain.Models.Exceptions;
using LMS.Presentation.Controllers;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Service.Contracts;
using System;

namespace LMS.Tests.UnitTests;

public class ModuleControllerUnitTests
{

    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly ModuleController _controller;
    private readonly Mock<IObjectModelValidator> _mockValidator;


    public ModuleControllerUnitTests()
    {
        _serviceManagerMock = new Mock<IServiceManager>();
        _controller = new ModuleController(_serviceManagerMock.Object);
        _mockValidator = new Mock<IObjectModelValidator>();

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
        var exception = await Record.ExceptionAsync(() => _controller.GetModules(courseId, parameters));
        Assert.IsAssignableFrom<NotFoundException>(exception);
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
        var exception = await Record.ExceptionAsync(() => _controller.GetModuleById(courseId, moduleId, true));
        Assert.IsAssignableFrom<NotFoundException>(exception);
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
        var exception = await Record.ExceptionAsync(() => _controller.PostModule(courseId, dto));
        Assert.IsAssignableFrom<NotFoundException>(exception);
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
        var exception = await Record.ExceptionAsync(() => _controller.PostModule(courseId, dto));
        Assert.IsAssignableFrom<ConflictException>(exception);
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.CreateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<ModuleCreateDto>()),
            Times.Once()
        );
    }


    [Fact]
    public async Task PutModule_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
        var dto = SeedData.GetModuleUpdateDto();

        _serviceManagerMock.Setup(service => service.ModuleService.UpdateModuleAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModuleUpdateDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.PutModule(courseId, moduleId, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceManagerMock.Verify(service => service.ModuleService.UpdateModuleAsync(courseId, moduleId, dto), Times.Once);
    }

    [Fact]
    public async Task PutModule_ShouldThrowException_whenModuleStartDateAfterEndDate()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
        var errorMessage = "Start date must be before end date.";
        var dto = SeedData.GetModuleUpdateDto();
        

        _serviceManagerMock.Setup(service => service.ModuleService.UpdateModuleAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModuleUpdateDto>()))
            .ThrowsAsync(new BadRequestException(errorMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _controller.PutModule(courseId, moduleId,dto));
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.UpdateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<ModuleUpdateDto>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task PutModule_ShouldThrowException_whenCourseNotFound()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
        var errorMessage = $"No Course with id: {courseId}  found!";
        var dto = SeedData.GetModuleUpdateDto();

        _serviceManagerMock.Setup(service => service.ModuleService.UpdateModuleAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModuleUpdateDto>()))
            .ThrowsAsync(new CourseNotFoundException(courseId));

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _controller.PutModule(courseId, moduleId,dto));
        Assert.IsAssignableFrom<NotFoundException>(exception);
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.UpdateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<ModuleUpdateDto>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task PutModule_ShouldThrowException_whenModuleOverlap()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
        var dto = SeedData.GetModuleUpdateDto();
        var updated = new ModuleDto
        {
            Id = 10,
            Name = dto.Name,
            Description = dto.Description,
            StartsAt = dto.EndsAt,
            EndsAt = dto.StartsAt,
        };
        var errorMessage = $"Module must be within course dates: {updated.StartsAt} - {updated.EndsAt}.";

        _serviceManagerMock.Setup(service => service.ModuleService.UpdateModuleAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ModuleUpdateDto>()))
            .ThrowsAsync(new ModuleOverlappingException(updated.StartsAt, updated.EndsAt));

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _controller.PutModule(courseId, moduleId , dto));
        Assert.IsAssignableFrom<ConflictException>(exception);
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.UpdateModuleAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<ModuleUpdateDto>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task PatchModule_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;

        var patchDoc = new JsonPatchDocument<ModuleUpdateDto>();
        patchDoc.Replace(m => m.Name, "Patched Module");

        var module = SeedData.GetFirstModule();
        var dto = SeedData.GetModuleUpdateDto();

        _serviceManagerMock.Setup(service => service.ModuleService.GetModuleForPatchAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((module, dto));

        _serviceManagerMock.Setup(service => service.ModuleService.ApplyModulePatchAsync(module, It.IsAny<ModuleUpdateDto>()))
            .Returns(Task.CompletedTask);

        _mockValidator.Setup(mockObjectModelValidator =>
           mockObjectModelValidator.Validate(It.IsAny<ActionContext>(), It.IsAny<ValidationStateDictionary>(), It.IsAny<string>(), It.IsAny<object>())
       );
        _controller.ObjectValidator = _mockValidator.Object;

        // Act
        var result = await _controller.PatchModuleAsync(courseId, moduleId, patchDoc);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _serviceManagerMock.Verify(service => service.ModuleService.GetModuleForPatchAsync(courseId, moduleId), Times.Once);
        _serviceManagerMock.Verify(service => service.ModuleService.ApplyModulePatchAsync(module, It.IsAny<ModuleUpdateDto>()), Times.Once);
    }

    [Fact]
    public async Task PatchModule_ShouldThrowException_whenModuleStartDateAfterEndDate()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
        var patchDoc = new JsonPatchDocument<ModuleUpdateDto>();
        patchDoc.Replace(m => m.Name, "Patched Module");
        var errorMessage = "Start date must be before end date.";
        var dto = SeedData.GetModuleUpdateDto();


        _serviceManagerMock.Setup(service => service.ModuleService.GetModuleForPatchAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new BadRequestException(errorMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _controller.PatchModuleAsync(courseId, moduleId, patchDoc));
        Assert.Equal(errorMessage, exception.Message);
        _serviceManagerMock.Verify(service => service.ModuleService.GetModuleForPatchAsync(
            It.IsAny<int>(),
            It.IsAny<int>()),
            Times.Once()
        );
    }
    
    [Fact]
    public async Task PatchModule_ShouldThrowException_whenCourseNotFound()
    {
        // Arrange
        int courseId = 1;
        int moduleId = 2;
    var patchDoc = new JsonPatchDocument<ModuleUpdateDto>();
        patchDoc.Replace(m => m.Name, "Patched Module");
        var errorMessage = $"No Course with id: {courseId}  found!";
        var dto = SeedData.GetModuleUpdateDto();

        _serviceManagerMock.Setup(service => service.ModuleService.GetModuleForPatchAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new CourseNotFoundException(courseId));

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _controller.PatchModuleAsync(courseId, moduleId, patchDoc));
        Assert.IsAssignableFrom<NotFoundException>(exception);
        Assert.Equal(errorMessage, exception.Message);
         _serviceManagerMock.Verify(service => service.ModuleService.GetModuleForPatchAsync(
            It.IsAny<int>(),
            It.IsAny<int>()),
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
