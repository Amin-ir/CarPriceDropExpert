using oft_winform.Utilities;
using System.Reflection;
using System.Text.Json;

namespace oft_winform.DTO;

public class DamageItemConfig
{
	public string Persian { get; set; }
	public int Level_1 { get; set; }
	public int Level_2 { get; set; }
	public int Level_3 { get; set; }

	public int GetLevelValue(int level)
	{
		switch (level)
		{
			case 1: return 0; // 1 = فاقد خسارت
			case 2: return Level_1;
			case 3: return Level_2;
			case 4: return Level_3;
			default: throw new ArgumentException("invalid level");
		}
	}

	public class DamageCoefficientConfiguration
	{
		private Dictionary<string, DamageItemConfig> _configurations;
		private static DamageCoefficientConfiguration _instance;
		private const string CONFIG_PATH = "Resources/Data/DamageCoefficientConfiguration.json";

		public static DamageCoefficientConfiguration Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DamageCoefficientConfiguration();
				return _instance;
			}
		}

		private DamageCoefficientConfiguration()
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
				_configurations = JsonSerializer.Deserialize<Dictionary<string, DamageItemConfig>>(jsonContent);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading configuration: {ex.Message}", "Configuration Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				_configurations = new Dictionary<string, DamageItemConfig>();
			}
		}

		public DamageItemConfig GetItem(string key)
		{
			return _configurations.TryGetValue(key, out var item) ? item : null;
		}
	}
}