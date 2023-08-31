namespace DiscountModule
{
	public interface IDiscountRule
	{
		void ApplyDiscount(Shipment shipment, DiscountPool discountPool);
	}

	public class SmallPackageLowestPriceRule : IDiscountRule
	{
		private readonly decimal _lowestPrice;

		public SmallPackageLowestPriceRule(List<Provider> providers)
		{
			_lowestPrice = providers.Min(p => p.GetPrice(PackageSizeType.S));
		}

		public void ApplyDiscount(Shipment shipment, DiscountPool discountPool)
		{
			if (shipment.PackageSize == PackageSizeType.S)
			{
				decimal currentPrice = shipment.Price;

				if (currentPrice > _lowestPrice)
				{
					shipment.Discount = discountPool.WithdrawDiscount(currentPrice - _lowestPrice);
				}
			}
		}
	}

	public class MonthlyThirdLargeShipmentRule : IDiscountRule
	{
		private const int _DISCOUNTED_SHIPMENT = 3;

		private int _shipmentCount = 0;
		private DateTime _lastDiscountDate;

		public void ApplyDiscount(Shipment shipment, DiscountPool discountPool)
		{
			try
			{
				if (_lastDiscountDate.IsDifferentMonthFrom(shipment.Date))
				{
					_shipmentCount = 0;
				}

				if (shipment.PackageSize == PackageSizeType.L && shipment.ShipmentProvider.Name == "LP")
				{
					_shipmentCount++;

					if (_shipmentCount == _DISCOUNTED_SHIPMENT)
					{
						shipment.Discount = discountPool.WithdrawDiscount(shipment.Price);
					}
				}
			}
			finally
			{
				_lastDiscountDate = shipment.Date;
			}
		}
	}

	public class RuleEngine
	{
		private List<IDiscountRule> rules = new();

		public void RegisterRule(IDiscountRule rule)
		{
			rules.Add(rule);
		}

		public void ApplyRule(Shipment shipment, DiscountPool discountPool)
		{
			foreach (var rule in rules)
			{
				rule.ApplyDiscount(shipment, discountPool);
			}
		}
	}
}
