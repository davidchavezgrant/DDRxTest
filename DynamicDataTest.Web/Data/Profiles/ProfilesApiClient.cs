using System.Reactive.Linq;

using DynamicDataTest.Contracts;

namespace DynamicDataTest.Web.Data.Profiles;

sealed class ProfilesApiClient : IProfilesApiClient
{
    readonly IProfilesApiContract _api;
    public ProfilesApiClient(IProfilesApiContract contract) => this._api = contract;
    public IObservable<ProfileDto> GetProfile(Guid id, bool forceUpdate = false) =>
        Observable.Create<ProfileDto>(observer => this._api.GetProfile(id).Subscribe(observer));
    public IObservable<IEnumerable<ProfileDto>> GetProfiles(bool forceUpdate = false) =>
        Observable.Create<IEnumerable<ProfileDto>>(observer => this._api.GetProfiles().Subscribe(observer));
}
