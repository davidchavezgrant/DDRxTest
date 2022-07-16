using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using DynamicDataTest.Contracts;

using ReactiveUI;

namespace DynamicDataTest.Web.Data.Profiles;

public interface IProfilesCache : IObservable<IChangeSet<ProfileDto, Guid>>
{
    IObservable<Unit> Load();
}

sealed class ProfilesCache : IProfilesCache
{
    readonly IProfilesApiClient _client;
    readonly IObservable<IChangeSet<ProfileDto, Guid>> _recordChangeSet;
    readonly SourceCache<ProfileDto, Guid> _profileCache = new SourceCache<ProfileDto, Guid>(p => p.Id);
    public ProfilesCache(IProfilesApiClient client)
    {
        this._client          = client;
        this._recordChangeSet = this._profileCache.Connect().DeferUntilLoaded().AutoRefresh().RefCount().ObserveOn(RxApp.MainThreadScheduler);
        this._recordChangeSet.MergeMany(x => x.RemoveCommand).Subscribe(RemoveItem);
    }

    private void        RemoveItem(Guid                                   id)       => this._profileCache.Remove(id);
    public  IDisposable Subscribe(IObserver<IChangeSet<ProfileDto, Guid>> observer) => this._recordChangeSet.Subscribe(observer);
    public IObservable<Unit> Load() => Observable.Create<Unit>(observer => this._client.GetProfiles().Subscribe(profiles => {
                this._profileCache.AddOrUpdate(profiles);
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
                })  );
}
