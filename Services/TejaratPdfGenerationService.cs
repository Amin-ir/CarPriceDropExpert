using oft_winform.DTO;
using oft_winform.TemplateMethods;
using oft_winform.Utilities;
using System.Text.RegularExpressions;

namespace oft_winform.Services;

public class TejaratPdfGenerationService : PDFGenerationTemplate
{
    protected override string ReadHtmlTemplate()
    {
        string basePath = GetBasePath();

        string templatePath = Path.Combine(basePath, "Resources/PdfTemplates/TejaratTemplate.html");

        string htmlTemplate = File.ReadAllText(templatePath);
        return htmlTemplate;
    }

    protected override string ReplaceValues(string htmlTemplate, PdfGenerationRequest request)
    {
        byte[] logoBytes = GetLogo();
        string logoBase64 = Convert.ToBase64String(logoBytes);
        string imageTag = $"<img style=\"width: 60%;\" src='data:image/png;base64,{logoBase64}' />";
        htmlTemplate = htmlTemplate.Replace("[LOGO]", imageTag);

        // Replace date
        htmlTemplate = htmlTemplate.Replace("[DATE]", request.Date)
                                   .Replace("[CarType]", request.CarType)
                                   .Replace("[ChassisNumber]", request.ChassisNumber)
                                   .Replace("[EngineNumber]", request.EngineNumber)
                                   .Replace("[PlateNumber]", request.PlateNumber)
                                   .Replace("[ConventionalCarPrice]", request.ConventionalCarPrice.ToString())
                                   .Replace("[ManufactureYear]", request.YearManufactured.ToString())
                                   .Replace("[CarPrice]", request.CarPrice.ToString())
                                   .Replace("[CarPriceDrop]", request.EstimatedCarPriceDrop.ToString());

        foreach(var item in request.SelectedDamages)
        {
            string pattern = $@"\[*{item.Key}_Level[1-4]\]+";

            // Find matches
            MatchCollection matches = Regex.Matches(htmlTemplate, pattern);

            // Print matches
            foreach (Match match in matches)
            {
                int matchLevel = (int)Char.GetNumericValue(match.Value[match.Value.Length - 2]);
                if (matchLevel == (int)item.Value)
                    htmlTemplate = htmlTemplate.Replace(match.Value, "✓");
                else
                    htmlTemplate = htmlTemplate.Replace(match.Value, "-");
            }
        }


        return htmlTemplate;
    }

    private byte[] GetLogo()
    {
        string basePath = GetBasePath();

        string logoPath = Path.Combine(basePath, "Resources/Tejarat-No-Insurance-Company-Logo.png");

        return File.ReadAllBytes(logoPath);
    }

    private string GetBasePath()
    {
        if (EnvironmentInfo.GetEnvironmentType() == EnvironmentType.Development)
            return Directory.GetParent(Environment.CurrentDirectory)!.Parent.Parent.FullName;
        else
            return Environment.CurrentDirectory;
    }
}
