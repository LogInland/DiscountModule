using DiscountModule;

namespace DiscountModuleTests
{
	public class ProviderTest
	{
		Provider testProvider;

		public ProviderTest()
		{
			testProvider = new Provider("Provider1", new Dictionary<PackageSizeType, decimal>
			{
				{ PackageSizeType.S, 1.50m },
				{ PackageSizeType.M, 5.00m },
				{ PackageSizeType.L, 6.77m }
			});
		}

		[Fact]
		public void GetPrice_ReturnMediumSizePrice_WhenMediumSizeRequested()
		{
			//Arrange
			decimal expectedPrice = 5.00m;

			//Act
			decimal actual = testProvider.GetPrice(PackageSizeType.M);

			//Assert
			Assert.Equal(expectedPrice, actual);
		}

		[Fact]
		public void GetPrice_ThrowsException_WhenSizeTypeNotFound()
		{
			//Assert
			Assert.Throws<Exception>( () => testProvider.GetPrice(PackageSizeType.Unknown));
		}
	}
}