namespace oft_winform.Utilities;

public class EnvironmentInfo
{
	public static EnvironmentType GetEnvironmentType()
	{
		#if DEBUG 
			return EnvironmentType.Development;
		#else
			return EnvironmentType.Production;
		#endif
	}
}

public enum EnvironmentType { Development, Production }