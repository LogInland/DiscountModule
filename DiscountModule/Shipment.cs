namespace DiscountModule
{
	public class Shipment
	{
		public DateTime Date { get; set; }
		public PackageSizeType PackageSize { get; set; }
		public Provider ShipmentProvider { get; set; }
		public decimal Price { get; set; }
		private decimal _discount;
		public decimal Discount
		{
			get => _discount;
			set
			{
				_discount = value;
				Price -= value;

				if (Price < 0) { Price = 0; }
			}
		}

		public Shipment(DateTime date, PackageSizeType packageSize, Provider provider)
		{
			Date = date;
			PackageSize = packageSize;
			ShipmentProvider = provider;
		}
	}
}
