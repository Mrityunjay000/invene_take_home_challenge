using System.Text;
using InveneTakeHomeChallenge.LabOrderProcessor.Features.SanitizeLabOrder;
using Microsoft.AspNetCore.Http;
using Moq;

namespace InveneTakeHomeChallenge.LabOrderProcessor.Tests.Features.SanitizeLabOrder;

public class SanitizeLabOrderHandlerTests
{
    private readonly Mock<ILabOrderRepository> _labOrderRepositoryMock = new();

    [Fact]
    public async Task SanitizeLabOrder_When_LabOrderProcessedSuccessfully_Then_SavesSanitizedLabOrderWithExpectedName()
    {
        // Arrange
        var mockLabOrder = new Mock<IFormFile>();
        var content = "Patient name: John Doe";
        var fileName = "testLabOrder.txt";
        var expectedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testLabOrder_sanitized.txt");
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        mockLabOrder.Setup(f => f.OpenReadStream()).Returns(ms);
        mockLabOrder.Setup(f => f.FileName).Returns(fileName);
        
        var handler = new SanitizeLabOrderHandler(_labOrderRepositoryMock.Object);
        
        // Act
        await handler.SanitizeLabOrder(mockLabOrder.Object);

        // Assert
        _labOrderRepositoryMock.Verify(repo => repo.SaveSanitizedLabOrder(
            It.Is<string>(s => s.Equals(expectedFileName)), 
            It.IsAny<string>()
        ), Times.Once);
    }
    
    [Theory]
    [InlineData("Patient name: John Doe", "Patient name: [REDACTED]\n")]
    [InlineData("Date of Birth: 01/23/1980", "Date of Birth: [REDACTED]\n")]
    [InlineData("sSn: 123-45-6789", "sSn: [REDACTED]\n")]
    [InlineData("Address: 123 Main St, Anytown, USA", "Address: [REDACTED]\n")]
    [InlineData("Email: test@example.com", "Email: [REDACTED]\n")]
    [InlineData("phone Number: (123) 456-7890", "phone Number: [REDACTED]\n")]
    [InlineData("Medical Record Number: MRN-0012345", "Medical Record Number: [REDACTED]\n")]
    public async Task SanitizeLabOrder_When_KnowPhiKeysPresent_Then_RedactsPhiValues(string line, string expectedRedactedLine)
    {
        // Arrange
        var mockLabOrder = new Mock<IFormFile>();
        var content = line;
        var fileName = "testLabOrder.txt";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        mockLabOrder.Setup(f => f.OpenReadStream()).Returns(ms);
        mockLabOrder.Setup(f => f.FileName).Returns(fileName);
        
        var handler = new SanitizeLabOrderHandler(_labOrderRepositoryMock.Object);

        // Act
        await handler.SanitizeLabOrder(mockLabOrder.Object);

        // Assert
        _labOrderRepositoryMock.Verify(repo => repo.SaveSanitizedLabOrder(
            It.IsAny<string>(), 
            It.Is<string>(s => s.Equals(expectedRedactedLine))
        ), Times.Once);
    }
    
    [Theory]
    [InlineData("bad phi: 123-45-6789", "bad phi: [REDACTED]\n")]
    [InlineData("bad phi: 01/23/1980", "bad phi: [REDACTED]\n")]
    [InlineData("bad phi: test@example.com", "bad phi: [REDACTED]\n")]
    [InlineData("bad phi: (123) 456-7890", "bad phi: ([REDACTED]\n")]
    [InlineData("bad phi: MRN-0012345", "bad phi: [REDACTED]\n")]
    public async Task SanitizeLabOrder_When_PhiRegexPatternPresentt_Then_RedactsPhiValues(string line, string expectedRedactedLine)
    {
        // Arrange
        var mockLabOrder = new Mock<IFormFile>();
        var content = line;
        var fileName = "testLabOrder.txt";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        mockLabOrder.Setup(f => f.OpenReadStream()).Returns(ms);
        mockLabOrder.Setup(f => f.FileName).Returns(fileName);
        
        var handler = new SanitizeLabOrderHandler(_labOrderRepositoryMock.Object);

        // Act
        await handler.SanitizeLabOrder(mockLabOrder.Object);

        // Assert
        _labOrderRepositoryMock.Verify(repo => repo.SaveSanitizedLabOrder(
            It.IsAny<string>(), 
            It.Is<string>(s => s.Equals(expectedRedactedLine))
        ), Times.Once);
    }
}