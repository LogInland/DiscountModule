using DiscountModule;

namespace DiscountModuleTests
{
	public class MockDiscountPool : DiscountPool
	{
		private const decimal MONTHLY_DISCOUNT_LIMIT = 15.00m;

		public MockDiscountPool() : base(MONTHLY_DISCOUNT_LIMIT)
		{
		}

		public decimal WithdrawDiscount(decimal amount) => amount;
	}

	public class SmallPackageLowestPriceRuleTests
	{
		[Fact]
		public void ApplyDiscount_GivesDiscount_WhenPackageIsSmallAndPriceIsHigher()
		{
			// Arrange
			decimal price = 2.00m;
			decimal expectedLowestPrice = 1.50m;

			var mockProviders = new List<Provider>
			{
				new Provider ("LP", new Dictionary<PackageSizeType, decimal>
				{
					{PackageSizeType.S, expectedLowestPrice},
				}),

				new Provider ("MR", new Dictionary<PackageSizeType, decimal>
				{
					{PackageSizeType.S, price},
				})
				
			};

			var rule = new SmallPackageLowestPriceRule(mockProviders);
			var shipmentDate = DateTime.Now;
			var shipmentSize = PackageSizeType.S;
			var shipmentProvider = mockProviders.Last(); // Provider "MR"
			var discountPool = new MockDiscountPool();
			var shipment = new Shipment(shipmentDate, shipmentSize, shipmentProvider);

			shipment.Price = mockProviders.Find(p => p.Name == shipment.ShipmentProvider.Name).GetPrice(shipment.PackageSize);
			
			// Act
			rule.ApplyDiscount(shipment, discountPool);

			// Assert
			Assert.Equal(price - expectedLowestPrice, shipment.Discount);
		}
	}

	public class MonthlyThirdLargeShipmentRuleTests
	{
		[Fact]
		public void ThirdLargeShipmentFromLP_ReceivesDiscount()
		{
			// Arrange
			var mockProvider = new Provider("LP", new Dictionary<PackageSizeType, decimal>
			{
				{ PackageSizeType.L, 5.00m }
			});
			var discountPool = new MockDiscountPool();
			var rule = new MonthlyThirdLargeShipmentRule();

			var firstShipment = new Shipment(DateTime.Now, PackageSizeType.L, mockProvider);
			var secondShipment = new Shipment(DateTime.Now, PackageSizeType.L, mockProvider);
			var thirdShipment = new Shipment(DateTime.Now, PackageSizeType.L, mockProvider);

			firstShipment.Price = mockProvider.GetPrice(PackageSizeType.L);
			secondShipment.Price = mockProvider.GetPrice(PackageSizeType.L);
			thirdShipment.Price = mockProvider.GetPrice(PackageSizeType.L);

			// Act
			rule.ApplyDiscount(firstShipment, discountPool);
			rule.ApplyDiscount(secondShipment, discountPool);
			rule.ApplyDiscount(thirdShipment, discountPool);

			// Assert
			Assert.Equal(0.00m, firstShipment.Discount);
			Assert.Equal(0.00m, secondShipment.Discount);
			Assert.Equal(5.00m, thirdShipment.Discount);  // Full price discount
		}

		[Fact]
		public void MonthlyDiscountLimit_Reached_ApplyPartialDiscountForLastShipment()
		{
			// Arrange
			var mockProvider = new Provider("LP", new Dictionary<PackageSizeType, decimal>
			{
				{ PackageSizeType.L, 16.00m }
			});
			var discountPool = new MockDiscountPool();
			var rule = new MonthlyThirdLargeShipmentRule();

			var shipmentSize = PackageSizeType.L;
			var shipments = new List<Shipment>();

			for (int i = 1; i < 4; i++) // Creating 3 large shipments for provider LP. Last one will get discount.
			{
				var shipmentDate = new DateTime(2023, 8, i);

				var shipment = new Shipment(shipmentDate, shipmentSize, mockProvider);
				shipment.Price = mockProvider.GetPrice(PackageSizeType.L);
				shipments.Add(shipment);
			}

			// Act
			foreach (var shipment in shipments)
			{
				rule.ApplyDiscount(shipment, discountPool);
			}

			// Assert
			Assert.Equal(15.00m, shipments.Sum(s => s.Discount)); // Check if total discounts equal the monthly limit.
			Assert.True(shipments.Last().Discount == 15); // Check if the last shipment has a partial discount.
		}
	}

	

}
