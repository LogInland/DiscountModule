namespace DiscountModule
{
	public class DiscountPool
	{
		private decimal _remainingDiscount;
		private readonly decimal _monthlyDiscountLimit;
		private DateTime _lastShipmentDate = DateTime.MinValue;

		public DiscountPool(decimal monthlyDiscountLimit)
		{
			_remainingDiscount = monthlyDiscountLimit;
			_monthlyDiscountLimit = monthlyDiscountLimit;
		}

		public decimal WithdrawDiscount(decimal requestedDiscount)
		{
			if (requestedDiscount < 0) return 0;

			if (requestedDiscount > _remainingDiscount)
			{
				decimal tmp = _remainingDiscount;

				_remainingDiscount = 0;
				
				return tmp;
			}

			_remainingDiscount -= requestedDiscount;

			return requestedDiscount;
		}

		public void ProcessShipment(Shipment shipment)
		{
			if (IsDifferentMonth(shipment))
			{
				_remainingDiscount = _monthlyDiscountLimit;
			}

			_lastShipmentDate = shipment.Date;
		}

		private bool IsDifferentMonth(Shipment shipment)
		{
			return _lastShipmentDate.IsDifferentMonthFrom(shipment.Date);
		}
	}

	internal static class DateExtensions
	{
		public static bool IsDifferentMonthFrom(this DateTime self, DateTime other)
		{
			return self.Year != other.Year || self.Month != other.Month;
		}
	}
}
