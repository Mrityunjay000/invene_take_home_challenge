using System.Text;
using System.Text.RegularExpressions;

namespace LabOrderProcessor.Features.SanitizeLabOrder;

public class SanitizeLabOrderHandler : ISanitizeLabOrderHandler
{
    private const string REDACTED_TOKEN = "[REDACTED]";
    private readonly string[] _phiKeys =
    {
        "patient name", 
        "name",
        "date of birth", 
        "dob",
        "social security number", 
        "ssn",
        "address", 
        "home address",
        "phone number", 
        "number",
        "email address",
        "email",
        "medical record number"
    };
    
    private readonly ILabOrderRepository _labOrderRepository;

    public SanitizeLabOrderHandler(ILabOrderRepository labOrderRepository)
    {
        _labOrderRepository = labOrderRepository;
    }
    
    /// <summary>
    /// Sanitizes a Lab Order by redacting the PHI in it. This method assumes that PHIs will be present in a
    /// "key: value" pattern per line. It also tries to compensate for bad or unknown keys by using a regex for any
    /// remaining PHIs after the file has been redacted with known PHI keys. For simplicity, the regex patterns were
    /// limited to SSN, Date of Birth, Phone Number, Email, and MRN.
    /// </summary>
    /// <param name="labOrder">The lab order file to sanitize, provided as an IFormFile.</param>
    public async Task SanitizeLabOrder(IFormFile labOrder)
    {
        var fileName = Path.GetFileNameWithoutExtension(labOrder.FileName);

        var sanitizedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}_sanitized.txt");
        
        using var reader = new StreamReader(labOrder.OpenReadStream());
        StringBuilder stringBuilder = new StringBuilder();

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var sanitizedLine = RedactPhiWithKnownKeys(line);
            sanitizedLine = RedactPhiWithRegex(sanitizedLine);
            stringBuilder.AppendLine(sanitizedLine);
        }
        
        await _labOrderRepository.SaveSanitizedLabOrder(sanitizedFilePath,  stringBuilder.ToString());
    }
    
    /// <summary>
    /// Looks for a know PHI key and sanitizes the line if it finds one.
    /// </summary>
    /// <param name="line">The line to sanitize</param>
    /// <returns>Sanitized line with any known PHI redacted</returns>
    private string RedactPhiWithKnownKeys(string line)
    {
        var parts = line.Split(":", 2);

        if (parts.Length != 2)
            return line;

        var key = parts[0];

        if (_phiKeys.Contains(key.Trim().ToLower()))
            return $"{key}: {REDACTED_TOKEN}";

        return line;
    }

    /// <summary>
    /// Looks for a know PHI regex pattern (SSN, Date of Birth, Phone Number, Email, and MRN) and redacts
    /// the matched pattern(s).
    /// </summary>
    /// <param name="line">The line to sanitize</param>
    /// <returns>Sanitized line with any matched PHI pattenrs redacted</returns>
    private string RedactPhiWithRegex(string line)
    {
        var ssnPattern = @"\b(?!0{3}-0{2}-0{4})[0-9]{3}-[0-9]{2}-[0-9]{4}\b";
        var redactedLine = Regex.Replace(line, ssnPattern, REDACTED_TOKEN);

        var dobPattern = @"\b(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d\b";
        redactedLine = Regex.Replace(redactedLine, dobPattern, REDACTED_TOKEN);

        var phoneNumberPattern = @"\b(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}\b";
        redactedLine = Regex.Replace(redactedLine, phoneNumberPattern, REDACTED_TOKEN);

        var emailPattern = @"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\b";
        redactedLine = Regex.Replace(redactedLine, emailPattern, REDACTED_TOKEN);

        var mrnPattern = @"\bMRN-\d{7}\b";
        redactedLine = Regex.Replace(redactedLine, mrnPattern, REDACTED_TOKEN);

        return redactedLine;
    }
}