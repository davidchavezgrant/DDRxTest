using System.Reactive.Linq;

using DynamicData.Kernel;

namespace DynamicDataTest.Web.Data.Trades;

public class MarketDataService
{
	readonly Dictionary<string, IObservable<MarketData>> _prices = new();

	public MarketDataService(StaticData staticData)
	{
		foreach (var item in staticData.CurrencyPairs)
		{
			this._prices[item.Code] = GenerateStream(item)
			                          .Replay(1)
			                          .RefCount();
		}
	}

	IObservable<MarketData> GenerateStream(CurrencyPair currencyPair)
	{
		return Observable.Create<MarketData>(observer =>
		{
			var spread  = currencyPair.DefaultSpread;
			var midRate = currencyPair.InitialPrice;
			var bid     = midRate - (spread * currencyPair.PipSize);
			var offer   = midRate + (spread * currencyPair.PipSize);
			var initial = new MarketData(currencyPair.Code, bid, offer);

			var currentPrice = initial;
			observer.OnNext(initial);

			var random = new Random();


			//for a given period, move prices by up to 5 pips
			return Observable.Interval(TimeSpan.FromSeconds(1 / (double)currencyPair.TickFrequency))
			                 .Select(_ => random.Next(1, 5))
			                 .Subscribe(pips =>
			                 {
				                 //move up or down between 1 and 5 pips
				                 var adjustment = Math.Round(pips * currencyPair.PipSize, currencyPair.DecimalPlaces);

				                 currentPrice = random.NextDouble() > 0.5?
					                                currentPrice + adjustment :
					                                currentPrice - adjustment;

				                 observer.OnNext(currentPrice);

			                 });
		});
	}

	public IObservable<MarketData> Watch(string currencyPair)
	{
		if (currencyPair == null) throw new ArgumentNullException(nameof(currencyPair));

		return this._prices.Lookup(currencyPair)
		           .ValueOrThrow(() => new Exception(currencyPair + " is an unknown currency pair"));
	}
}