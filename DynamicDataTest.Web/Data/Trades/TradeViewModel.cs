using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace DynamicDataTest.Web.Data.Trades;

sealed class TradeViewModel
{
	readonly IObservableCollection<TradeProxy> _data = new ObservableCollectionExtended<TradeProxy>();
	readonly TradeClient                       _tradeClient;

	public TradeViewModel(TradeClient tradeClient, SearchHints searchHints)
	{
		this._tradeClient = tradeClient;
		this.SearchHints  = searchHints;

		var filter = this.SearchHints.WhenValueChanged(t => t.SearchText)
		                 .Select(BuildFilter);

		var loader = this._tradeClient.Trades
		                 .Connect(trade => trade.Status == TradeStatus.Live)
		                 .Filter(filter)
		                 .Transform(trade => new TradeProxy(trade))
		                 .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
		                 .ObserveOn(RxApp.TaskpoolScheduler)
		                 .Bind(this._data)
		                 .DisposeMany()
		                 .Subscribe();


	}

	public SearchHints SearchHints { get; }

	Func<Trade, bool> BuildFilter(string searchText)
	{
		if (string.IsNullOrEmpty(searchText)) return trade => true;

		return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase)
		         || t.Customer.Contains(searchText, StringComparison.OrdinalIgnoreCase);
	}
}