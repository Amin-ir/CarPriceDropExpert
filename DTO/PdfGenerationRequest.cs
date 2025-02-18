using oft_winform.Utilities;

namespace oft_winform.DTO;

public class PdfGenerationRequest
{
    public Dictionary<string, DamageLevel> SelectedDamages { get; set; }
    public string CarType { get; set; }
    public string ChassisNumber { get; set; }
    public string EngineNumber { get; set; }
    public string PlateNumber { get; set; }
    public long ConventionalCarPrice { get; set; }
    public int YearManufactured { get; set; }
    public long CarPrice { get; set; }
    public float EstimatedCarPriceDrop => AppFormulas.GetPriceDrop(SelectedDamages, CarPrice, YearManufactured, ConventionalCarPrice);
    public string OutputPath { get; internal set; }
    public string Date { get; set; }
}
