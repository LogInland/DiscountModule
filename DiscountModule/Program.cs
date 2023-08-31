// See https://aka.ms/new-console-template for more information

using DiscountModule;

const decimal MONTHLY_DISCOUNT_LIMIT = 10;

var resultLines = new List<string>();

var shipments = new List<Shipment>();

var ruleEngine = new RuleEngine();

var discountPool = new DiscountPool(MONTHLY_DISCOUNT_LIMIT);

var lines = FileService.ReadLines("Input.txt");

var providers = ProviderConfiguration.Instance.Providers;

ruleEngine.RegisterRule(new SmallPackageLowestPriceRule(providers));
ruleEngine.RegisterRule(new MonthlyThirdLargeShipmentRule());

foreach (var line in lines)
{
	var shipment = ShipmentParser.ParseLine(line, providers);

	if (shipment == null)
	{
		resultLines.Add($"{line} Ignored");
		continue;
	}

	shipment.Price = providers.Find(p => p.Name == shipment.ShipmentProvider.Name).GetPrice(shipment.PackageSize);

	shipments.Add(shipment);

	discountPool.ProcessShipment(shipment);

	ruleEngine.ApplyRule(shipment, discountPool);

	Console.WriteLine(ShipmentParser.ConvertToLine(shipment));
}
