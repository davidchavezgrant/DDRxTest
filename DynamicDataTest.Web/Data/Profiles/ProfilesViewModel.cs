using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using DynamicDataTest.Contracts;

using ReactiveUI;

namespace DynamicDataTest.Web.Data.Profiles;

public sealed class ProfilesViewModel : ReactiveObject
{
    readonly IProfilesCache _profilesCache;

    public ProfilesViewModel(IProfilesCache profilesCache)
    {
        this._profilesCache = profilesCache;

        this._profilesCache
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out this.profiles)
            .DisposeMany()
            .Subscribe(_ => {}, RxApp.DefaultExceptionHandler.OnNext);

    }
    
    
    readonly ReadOnlyObservableCollection<ProfileDto> profiles;
    public ReadOnlyObservableCollection<ProfileDto> Profiles => this.profiles;

    public IObservable<Unit>           ExecuteInitialize() => this._profilesCache.Load();
}
