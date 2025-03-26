using Microsoft.AspNetCore.Mvc;

namespace InveneTakeHomeChallenge.LabOrderProcessor.Features.SanitizeLabOrder;

[ApiController]
[Route("[controller]")]
public class SanitizeLabOrderController :  ControllerBase
{
    private readonly ISanitizeLabOrderHandler _sanitizeLabOrderHandler;

    public SanitizeLabOrderController(ISanitizeLabOrderHandler sanitizeLabOrderHandler)
    {
        _sanitizeLabOrderHandler = sanitizeLabOrderHandler;
    }
    
    /// <summary>
    /// API for sanitizing a Lab Order. Assuming only .txt files are allowed.
    /// </summary>
    /// <param name="labOrder">The lab order file to sanitize, provided as an IFormFile.</param>
    /// <returns>
    ///   <para>
    ///     <see cref="OkResult"/> (200 OK) if the file was successfully sanitized and processed.
    ///   </para>
    ///   <para>
    ///     <see cref="BadRequestObjectResult"/> (400 Bad Request) if no file was uploaded, the file was empty, or the file type is not .txt.
    ///   </para>
    ///   <para>
    ///     <see cref="ProblemDetails"/> (500 Internal Server Error) if an error occurs during file processing.
    ///   </para>
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> SanitizeLabOrder(IFormFile labOrder)
    {
        if (labOrder == null || labOrder.Length == 0)
            return BadRequest("No file uploaded or file is empty.");

        // Assuming only .txt files are valid input type
        if (Path.GetExtension(labOrder.FileName).ToLower() != ".txt")
            return BadRequest("Only .txt files are allowed.");
        
        try
        {
            await _sanitizeLabOrderHandler.SanitizeLabOrder(labOrder);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing file: {ex.Message}");
            
            return Problem(
                detail: ex.Message,
                statusCode: 500,
                title: "Error Processing Lab Order",
                instance: HttpContext.Request.Path
            );
        }
    }
}