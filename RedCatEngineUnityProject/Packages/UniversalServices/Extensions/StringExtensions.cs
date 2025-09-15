namespace RedCatEngine.CommonServices.Extensions
{
	public static class StringExtensions
	{
		public static string FormatWith(this string value, params object[] args)
			=> string.Format(value, args);

		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool IsNullOrWhitespace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool IsNotNullOrEmpty(this string value)
		{
			return !IsNullOrEmpty(value);
		}

		public static int SizeOfMemory(this string value)
		{
			return 26 + 2 * value.Length;
		}
		public static float SizeOfMemoryKb(this string value)
		{
			return (float)value.SizeOfMemory() / 1024;
		}
		public static float SizeOfMemoryMb(this string value)
		{
			return value.SizeOfMemoryKb() / 1024;
		}
	}
}