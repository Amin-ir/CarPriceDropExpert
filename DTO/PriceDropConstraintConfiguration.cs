
using oft_winform.Utilities;
using System.Configuration;
using System.Text.Json;

namespace oft_winform.DTO;

public class PriceDropConstraint
{
    public int MaxAge { get; set; }
    public long ConventionalCarPrice { get; set; }
    public int MaxCalculacatableDamagePercentage { get; set; }
}
public static class PriceDropConstraintConfiguration
{
    private const string CONFIG_PATH = "Resources/Data/PriceDropConstraintConfiguration.json";
    private static PriceDropConstraint _Instance;
    public static PriceDropConstraint Instance
    {
        get
        {
            if (_Instance == null)
                Load();
            return _Instance;
        }
    }

    private static void Load()
    {
        try
        {
            string path = "";
            if (EnvironmentInfo.GetEnvironmentType() == EnvironmentType.Development)
            {
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
                path = Path.Combine(projectDirectory, CONFIG_PATH).Replace("\\", "/");
            }
            else
            {
                path = Path.Combine(Environment.CurrentDirectory, CONFIG_PATH);
            }

            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration path not found!");

            string jsonContent = File.ReadAllText(path);
            _Instance = JsonSerializer.Deserialize<PriceDropConstraint>(jsonContent);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading configuration: {ex.Message}", "Configuration Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            _Instance = new PriceDropConstraint();
        }
    }
}
