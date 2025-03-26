namespace InveneTakeHomeChallenge.LabOrderProcessor.Features.SanitizeLabOrder;

public interface ISanitizeLabOrderHandler
{
    Task SanitizeLabOrder(IFormFile file);
}