using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

namespace DynamicDataTest.Web.Data.Trades;

class SearchHints : AbstractNotifyPropertyChanged, IDisposable
{
	readonly IDisposable                          _cleanUp;
	readonly ReadOnlyObservableCollection<string> _hints;
	string                                        _searchText;

	public SearchHints(TradeClient tradeService, SchedulerProvider schedulerProvider)
	{
		//build a predicate when SearchText changes
		var filter = this.WhenValueChanged(t => t.SearchText)
		                 .Throttle(TimeSpan.FromMilliseconds(250))
		                 .Select(BuildFilter);

		//share the connection
		var shared = tradeService.Trades.Connect()
		                         .Publish();

		//distinct observable of customers
		var customers = shared.DistinctValues(trade => trade.Customer);

		//distinct observable of currency pairs
		var currencypairs = shared.DistinctValues(trade => trade.CurrencyPair);

		//observe customers and currency pairs using OR operator, and bind to the observable collection
		var loader = customers.Or(currencypairs)
		                      .Filter(filter)//filter strings
		                      .Sort(SortExpressionComparer<string>.Ascending(str => str))
		                      .ObserveOn(schedulerProvider.MainThread)
		                      .Bind(out this._hints)//bind to hints list
		                      .Subscribe();

		this._cleanUp = new CompositeDisposable(loader, shared.Connect());
	}

	public ReadOnlyObservableCollection<string> Hints => this._hints;

	public string SearchText
	{
		get => this._searchText;
		set => SetAndRaise(ref this._searchText, value);
	}

	public void Dispose() { this._cleanUp.Dispose(); }

	Func<string, bool> BuildFilter(string searchText)
	{
		if (string.IsNullOrEmpty(searchText)) return trade => true;

		return str => str.Contains(searchText, StringComparison.OrdinalIgnoreCase) || str.Contains(searchText, StringComparison.OrdinalIgnoreCase);
	}
}