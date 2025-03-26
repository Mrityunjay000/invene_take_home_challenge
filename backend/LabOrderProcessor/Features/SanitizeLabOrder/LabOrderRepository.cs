namespace LabOrderProcessor.Features.SanitizeLabOrder;

public class LabOrderRepository : ILabOrderRepository
{
    public async Task SaveSanitizedLabOrder(string filePath, string fileContent)
    {
        await File.WriteAllTextAsync(filePath, fileContent);
    }
}