using System.Text;
using InveneTakeHomeChallenge.LabOrderProcessor.Features.SanitizeLabOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InveneTakeHomeChallenge.LabOrderProcessor.Tests.Features.SanitizeLabOrder;

public class SanitizeLabOrderControllerTests
{
    private readonly Mock<ISanitizeLabOrderHandler> _sanitizeLabOrderHandlerMock = new();
    
    [Fact]
    public async Task SanitizeLabOrder_When_FileIsNull_Then_ReturnBadRequest()
    {
        // Arrange
        var controller = new SanitizeLabOrderController(_sanitizeLabOrderHandlerMock.Object);
        
        var expectedErrorMessage = "No file uploaded or file is empty.";
        
        // Act
        var result = await controller.SanitizeLabOrder(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedErrorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task SanitizeLabOrder_When_FileIsEmpty_Then_ReturnBadRequest()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        
        var controller = new SanitizeLabOrderController(_sanitizeLabOrderHandlerMock.Object);

        var expectedErrorMessage = "No file uploaded or file is empty.";

        // Act
        var result = await controller.SanitizeLabOrder(fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedErrorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task SanitizeLabOrder_When_FileTypeIsNotTxt_Then_ReturnBadRequest()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.FileName).Returns("document.pdf");
        
        var controller = new SanitizeLabOrderController(_sanitizeLabOrderHandlerMock.Object);

        var expectedErrorMessage = "Only .txt files are allowed.";

        // Act
        var result = await controller.SanitizeLabOrder(fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedErrorMessage, badRequestResult.Value);
    }
    
    [Fact]
    public async Task SanitizeLabOrder_When_ProcessingIsSuccessful_Then_ReturnsOk()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Patient name: John Doe";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.FileName).Returns("labOrder.txt");
        
        var controller = new SanitizeLabOrderController(_sanitizeLabOrderHandlerMock.Object);

        // Act
        var result = await controller.SanitizeLabOrder(fileMock.Object);

        // Assert
        Assert.IsType<OkResult>(result);
        
        _sanitizeLabOrderHandlerMock.Verify(h => h.SanitizeLabOrder(fileMock.Object), Times.Once);
    }
    
    [Fact]
    public async Task SanitizeLabOrder_When_ExceptionOccurs_Then_ReturnsInternalServerError()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Patient name: John Doe";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.FileName).Returns("labOrder.txt");
        
        _sanitizeLabOrderHandlerMock.Setup(h => h.SanitizeLabOrder(It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception("Unexpected error"));
        
        var controller = new SanitizeLabOrderController(_sanitizeLabOrderHandlerMock.Object);
        
        var httpContextMock = new DefaultHttpContext();
        httpContextMock.Request.Path = "/SanitizeLabOrder";
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContextMock
        };

        // Act
        var result = await controller.SanitizeLabOrder(fileMock.Object);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
        Assert.Contains("Error Processing Lab Order", ((ProblemDetails)problemResult.Value).Title);
    }
}