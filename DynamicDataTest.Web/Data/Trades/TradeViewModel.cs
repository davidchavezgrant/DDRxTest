using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

namespace DynamicDataTest.Web.Data.Trades;

sealed class TradeViewModel
{
	readonly TradeService      _service;
	readonly MarketDataService _marketDataService;

	public TradeViewModel(TradeService service, MarketDataService marketDataService)
	{
		this._service           = service;
		this._marketDataService = marketDataService;

		var stream = service.Trades
		                    .Connect()
		                    .Filter(trade => trade.Status == TradeStatus.Live)// get live streams
		                    .Group(trade => trade.CurrencyPair)               // group into streams by currency pair
		                    .SubscribeMany(grouping =>                        // action to apply to group
		                    {
			                    var locker = new object();

			                    decimal latestPrice = 0;

			                    var priceHasChanged = ObservePrice(grouping.Key)
			                                          .Synchronize(locker)
			                                          .Subscribe(price =>

			                                          {

				                                          latestPrice = price.Bid;

				                                          TradeViewModel.UpdateTradesWithPrice(grouping.Cache.Items, latestPrice);
			                                          });

			                    var dataHasChanged = grouping.Cache.Connect()
			                                                 .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
			                                                 .Synchronize(locker)
			                                                 .Subscribe(changes => TradeViewModel.UpdateTradesWithPrice(changes.Select(change => change.Current), latestPrice));

			                    return new CompositeDisposable(priceHasChanged, dataHasChanged);
		                    })
		                    .Subscribe();

	}

	IObservable<MarketData> ObservePrice(string currencyPair) => this._marketDataService.Watch(currencyPair);

	static void UpdateTradesWithPrice(IEnumerable<Trade> trades, decimal price)
	{
		foreach (var trade in trades)
		{
			trade.Price = price;
		}
	}
}