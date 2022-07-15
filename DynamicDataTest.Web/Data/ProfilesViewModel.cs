using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using DynamicDataTest.Contracts;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DynamicDataTest.Web.Data;

public sealed class ProfilesViewModel : ReactiveObject
{
    readonly IProfilesCache _profilesCache;

    public ProfilesViewModel(IProfilesCache profilesCache)
    {
        this._profilesCache = profilesCache;

        this._profilesCache
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(x => !this.omittedGuids.Contains(x.Id))
            .Bind(out profiles)
            .DisposeMany()
            .Subscribe(_ => {}, RxApp.DefaultExceptionHandler.OnNext);

        this.DeleteProfile = ReactiveCommand.Create((Guid id) => this.omittedGuids.Add(id), default,RxApp.MainThreadScheduler);
    }
    
    [Reactive]
    ObservableCollection<Guid> omittedGuids { get; set; } = new ObservableCollection<Guid>();
    
    readonly ReadOnlyObservableCollection<ProfileDto> profiles;
    public ReadOnlyObservableCollection<ProfileDto> Profiles => profiles;

    public IObservable<Unit>           ExecuteInitialize() => this._profilesCache.Load();
    [Reactive]
    public ReactiveCommand<Guid, Unit> DeleteProfile       { get; private set; }
}
