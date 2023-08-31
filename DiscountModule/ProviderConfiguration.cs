using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountModule
{
	public class ProviderConfiguration
	{
		private static ProviderConfiguration _instance;
		public List<Provider> Providers { get; private set; }

		private ProviderConfiguration()
		{
			Providers = new List<Provider>
			{
				new (ProviderType.LP, new Dictionary<PackageSizeType, decimal>
				{
					{ PackageSizeType.S, 1.50m },
					{ PackageSizeType.M, 4.90m },
					{ PackageSizeType.L, 6.90m }
				}),

				new (ProviderType.MR, new Dictionary<PackageSizeType, decimal>
				{
					{ PackageSizeType.S, 2.00m },
					{ PackageSizeType.M, 3.00m },
					{ PackageSizeType.L, 4.00m }
				})
			};
		}

		public static ProviderConfiguration Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ProviderConfiguration();

				return _instance;
			}
		}
	}
}
