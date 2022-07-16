using System.Reactive.Linq;

using DynamicData.Kernel;

using DynamicDataTest.Web.Data.Trades;


public class TradeGenerator : IDisposable
{
	readonly IDisposable                     _cleanUp;
	readonly IDictionary<string, MarketData> _latestPrices = new Dictionary<string, MarketData>();
	readonly object                          _locker       = new();
	readonly Random                          _random       = new();
	readonly StaticData                      _staticData;
	int                                      _counter;

	public TradeGenerator(StaticData staticData, MarketDataService marketDataService)
	{
		this._staticData = staticData;

		//keep track of the latest price so we can generate trades are a reasonable distance from the market
		this._cleanUp = staticData.CurrencyPairs
		                          .Select(currencypair => marketDataService.Watch(currencypair.Code))
		                          .Merge()
		                          .Synchronize(this._locker)
		                          .Subscribe(md =>
		                          {
			                          this._latestPrices[md.Instrument] = md;
		                          });
	}

	public void Dispose() { this._cleanUp.Dispose(); }

	public IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad = false)
	{
		Trade NewTrade()
		{
			var id     = this._counter++;
			var bank   = this._staticData.Customers[this._random.Next(0,     this._staticData.Customers.Length)];
			var pair   = this._staticData.CurrencyPairs[this._random.Next(0, this._staticData.CurrencyPairs.Length)];
			var amount = (this._random.Next(1,                               2000) / 2) * (10 ^ this._random.Next(1, 5));

			var buySell = this._random.NextBoolean()?
				              BuyOrSell.Buy :
				              BuyOrSell.Sell;

			if (initialLoad)
			{
				var status = this._random.NextDouble() > 0.5?
					             TradeStatus.Live :
					             TradeStatus.Closed;

				var seconds = this._random.Next(1, 60 * 60 * 24);
				var time    = DateTime.Now.AddSeconds(-seconds);

				return new Trade(id,
				                 bank,
				                 pair.Code,
				                 status,
				                 buySell,
				                 GererateRandomPrice(pair, buySell),
				                 amount,
				                 timeStamp: time);
			}

			return new Trade(id,
			                 bank,
			                 pair.Code,
			                 TradeStatus.Live,
			                 buySell,
			                 GererateRandomPrice(pair, buySell),
			                 amount);
		}


		IEnumerable<Trade> result;

		lock (this._locker)
		{
			result = Enumerable.Range(1, numberToGenerate)
			                   .Select(_ => NewTrade())
			                   .ToArray();
		}

		return result;
	}

	decimal GererateRandomPrice(CurrencyPair currencyPair, BuyOrSell buyOrSell)
	{

		var price = this._latestPrices.Lookup(currencyPair.Code)
		                .ConvertOr(converter: md => md.Bid, fallbackConverter: () => currencyPair.InitialPrice);

		//generate percent price 1-100 pips away from the inital market
		var pipsFromMarket = this._random.Next(1, 100);
		var adjustment     = Math.Round(pipsFromMarket * currencyPair.PipSize, currencyPair.DecimalPlaces);

		return buyOrSell == BuyOrSell.Sell?
			       price + adjustment :
			       price - adjustment;
	}
}


static class RandomExtensions
{
	public static bool NextBoolean(this Random random) => random.Next() > (int.MaxValue / 2);
}