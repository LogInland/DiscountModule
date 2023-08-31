using System.Globalization;

namespace DiscountModule
{
	public static class FileService
	{
		public static IEnumerable<string> ReadLines(string fileName)
		{
			StreamReader fileReader = new StreamReader(System.IO.Path.GetFullPath(fileName));
			string line;

			while ((line = fileReader.ReadLine()) != null)
			{
				yield return line;
			}
		}
	}

	public static class ShipmentParser
	{
		public static Shipment ParseLine(string line)
		{
			string[] parts = line.Split(' ');

			if (parts.Length != 3)
			{
				Console.WriteLine($"{line} Ignored");
				return null;
			}

			try
			{
				if (!DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
				{
					Console.WriteLine($"Error while parsing date from line: {line}");
					return null;
				}

				PackageSizeType size = Enum.TryParse(parts[1], out PackageSizeType parsedSize)
					? parsedSize
					: PackageSizeType.Unknown;
				ProviderType provider = Enum.TryParse(parts[2], out ProviderType parseProviderType)
					? parseProviderType
					: ProviderType.Unknown;

				if (size != PackageSizeType.Unknown && provider != ProviderType.Unknown)
				{
					return new Shipment(date, size, provider);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while parsing {ex}");
			}

			return null;
		}

		public static string ConvertToLine(Shipment shipment)
		{
			return $"{shipment.Date:yyy-MM-dd} {shipment.PackageSize} {shipment.ProviderType} {shipment.Price} {shipment.Discount}";
		}
	}
}
