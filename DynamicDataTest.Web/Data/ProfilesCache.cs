using DynamicDataTest.Contracts;
using DynamicData;
using System.Reactive;
using System.Reactive.Linq;
namespace DynamicDataTest.Web.Data;

interface IProfilesCache : IObservable<IChangeSet<ProfileDto, Guid>>
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
        _client = client;
        _recordChangeSet = _profileCache.Connect().RefCount();
    }
    public IDisposable Subscribe(IObserver<IChangeSet<ProfileDto, Guid>> observer) => _recordChangeSet.Subscribe(observer);
    public IObservable<Unit> Load() => Observable.Create<Unit>(observer => _client.GetProfiles().Subscribe(profiles => {
                _profileCache.AddOrUpdate(profiles);
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
                })  );
}
