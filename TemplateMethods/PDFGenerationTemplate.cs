using oft_winform.DTO;
using SelectPdf;

namespace oft_winform.TemplateMethods;

public abstract class PDFGenerationTemplate
{
    public void GeneratePdf(PdfGenerationRequest request)
    {
        string htmlTemplate = ReadHtmlTemplate();
        htmlTemplate = ReplaceValues(htmlTemplate, request);
        ConvertHtmlToPdf(request, htmlTemplate);
    }

    private void ConvertHtmlToPdf(PdfGenerationRequest request, string htmlTemplate)
    {
        // Convert to PDF
        HtmlToPdf converter = new HtmlToPdf();
        converter.Options.PdfPageSize = PdfPageSize.A4;
        converter.Options.MarginTop = 20;
        converter.Options.MarginBottom = 20;

        PdfDocument doc = converter.ConvertHtmlString(htmlTemplate);
        doc.Save(request.OutputPath);
        doc.Close();
    }

    protected abstract string ReplaceValues(string htmlTemplate, PdfGenerationRequest request);
    protected abstract string ReadHtmlTemplate();
}
