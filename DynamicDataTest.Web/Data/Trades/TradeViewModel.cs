using System.Collections.ObjectModel;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace DynamicDataTest.Web.Data.Trades;

public sealed class TradeViewModel : AbstractNotifyPropertyChanged
{
	readonly ReadOnlyObservableCollection<TradeProxy> _data; 
	readonly TradeClient                       _tradeClient;

	public TradeViewModel(TradeClient tradeClient)
	{
		this._tradeClient = tradeClient;

		var loader = this._tradeClient.Trades
		                 .Connect(trade => trade.Status == TradeStatus.Live)
		                 .RefCount()
		                 .Transform(trade => new TradeProxy(trade))
		                 .Sort(SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
		                 .ObserveOn(RxApp.MainThreadScheduler)
		                 .Bind(out this._data)
		                 .DisposeMany()
		                 .Subscribe();


	}

	public ReadOnlyObservableCollection<TradeProxy> Data        => this._data;

}