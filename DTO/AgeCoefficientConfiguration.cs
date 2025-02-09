using oft_winform.Utilities;
using System.Reflection;
using System.Text.Json;

namespace oft_winform.DTO;

public class AgeCoefficientConfiguration
{
	private Dictionary<int, float> _configurations;
	private static AgeCoefficientConfiguration _instance;
	private const string CONFIG_PATH = "Resources/Data/AgeCoefficientConfiguration.json";

	public static AgeCoefficientConfiguration Instance
	{
		get
		{
			if (_instance == null)
				_instance = new AgeCoefficientConfiguration();
			return _instance;
		}
	}

	private AgeCoefficientConfiguration()
	{
		LoadConfiguration();
	}

	private void LoadConfiguration()
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
			_configurations = JsonSerializer.Deserialize<Dictionary<int, float>>(jsonContent);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error loading configuration: {ex.Message}", "Configuration Error",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			_configurations = new Dictionary<int, float>();
		}
	}

	public bool TryGetItem(int key, out float value)
	{
		if (_configurations.TryGetValue(key, out value))
			return true;
		return false;
	}
}