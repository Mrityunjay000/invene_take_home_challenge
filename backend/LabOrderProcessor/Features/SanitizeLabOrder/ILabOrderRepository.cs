namespace LabOrderProcessor.Features.SanitizeLabOrder;

public interface ILabOrderRepository
{
    Task SaveSanitizedLabOrder(string filePath, string fileContent);
}