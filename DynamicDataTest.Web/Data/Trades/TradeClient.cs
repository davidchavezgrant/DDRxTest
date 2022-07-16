using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Kernel;

namespace DynamicDataTest.Web.Data.Trades;




public class TradeClient 
{
	readonly SchedulerProvider _schedulerProvider;
	readonly TradeGenerator    _tradeGenerator;
	readonly IObservableCache<Trade, long> _tradesCache;

	public TradeClient(TradeGenerator tradeGenerator, SchedulerProvider schedulerProvider)
	{
		this._tradeGenerator    = tradeGenerator;
		this._schedulerProvider = schedulerProvider;
		var          tradesData = GenerateTradesAndMaintainCache().Publish().RefCount();

		//create a derived cache  
		this._tradesCache = tradesData.Filter(trade => trade.Status == TradeStatus.Live).Filter(x => x.Id > 5000).AsObservableCache();
	}

	/// <inheritdoc />
	public IObservableCache<Trade, long> Trades => this._tradesCache; 

	IObservable<IChangeSet<Trade, long>> GenerateTradesAndMaintainCache()
	{
		//construct an cache datasource specifying that the primary key is Trade.Id
		return ObservableChangeSet.Create<Trade, long>(subscribe: cache =>
		                                               {
			                                               /*
			                                                   The following code emulates an external trade provider. 
			                                                   Alternatively you can use "new SourceCacheTrade, long>(t=>t.Id)" and manually maintain the cache.
                                               
			                                                   For examples of creating a observable change sets, see https://github.com/RolandPheasant/DynamicData.Snippets
			                                               */

			                                               //bit of code to generate trades
			                                               var random = new Random();

			                                               //initally load some trades 
			                                               cache.AddOrUpdate(this._tradeGenerator.Generate(5_000, true));

			                                               TimeSpan RandomInterval() => TimeSpan.FromMilliseconds(random.Next(2500, 5000));


			                                               // create a random number of trades at a random interval
			                                               var tradeGenerator = this._schedulerProvider.Background
			                                                                        .ScheduleRecurringAction(RandomInterval,
			                                                                                                 action: () =>
			                                                                                                 {
				                                                                                                 var number = random.Next(1, 5);
				                                                                                                 var trades = this._tradeGenerator.Generate(number);
				                                                                                                 cache.AddOrUpdate(trades);
			                                                                                                 });

			                                               // close a random number of trades at a random interval
			                                               var tradeCloser = this._schedulerProvider.Background
			                                                                     .ScheduleRecurringAction(RandomInterval,
			                                                                                              action: () =>
			                                                                                              {
				                                                                                              var number = random.Next(1, 2);

				                                                                                              cache.Edit(innerCache =>
				                                                                                              {
					                                                                                              var trades = innerCache.Items
					                                                                                                                     .Where(trade => trade.Status == TradeStatus.Live)
					                                                                                                                     .OrderBy(t => Guid.NewGuid())
					                                                                                                                     .Take(number)
					                                                                                                                     .ToArray();

					                                                                                              var toClose = trades.Select(trade => new Trade(trade, TradeStatus.Closed));

					                                                                                              cache.AddOrUpdate(toClose);
				                                                                                              });
			                                                                                              });

			                                               //expire closed items from the cache to avoid unbounded data
			                                               var expirer = cache
			                                                             .ExpireAfter(timeSelector: t => t.Status == TradeStatus.Closed?
				                                                                                             TimeSpan.FromMinutes(1) :
				                                                                                             null,
			                                                                          TimeSpan.FromMinutes(1),
			                                                                          this._schedulerProvider.Background)
			                                                             .Subscribe(x => Console.WriteLine("{0} filled trades have been removed from memory", x.Count()));

			                                               return new CompositeDisposable(tradeGenerator, tradeCloser, expirer);
		                                               },
		                                               keySelector: trade => trade.Id);
	}
}