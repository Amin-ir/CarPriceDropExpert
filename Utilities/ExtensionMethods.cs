namespace oft_winform.Utilities;

public static class ExtensionMethods
{
    public static T ToEnum<T>(this int value) where T : Enum
    => (T)Enum.ToObject(typeof(T), value);
}
