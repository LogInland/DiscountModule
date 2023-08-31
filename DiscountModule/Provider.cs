namespace DiscountModule
{
	public class Provider
	{
		public string Name { get; private set; }
		public Dictionary<PackageSizeType, decimal> Pricing { get; private set; }

		public Provider(string name, Dictionary<PackageSizeType, decimal> pricing)
		{
			Name = name;
			Pricing = pricing;
		}

		public decimal GetPrice(PackageSizeType sizeType)
		{
			if (Pricing.TryGetValue(sizeType, out decimal price))
			{
				return price;
			}
			else
			{
				throw new Exception("Price for the given sizeType not found.");
			}
		}
	}
}
