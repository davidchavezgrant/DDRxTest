using DynamicDataTest.Contracts;
using System.Reactive.Linq;
namespace DynamicDataTest.Web.Data;

sealed class ProfilesApiClient : IProfilesApiClient
{
    readonly IProfilesApiContract _api;
    public ProfilesApiClient(IProfilesApiContract contract) => _api = contract;
    public IObservable<ProfileDto> GetProfile(Guid id, bool forceUpdate = false) =>
        Observable.Create<ProfileDto>(observer => _api.GetProfile(id).Subscribe(observer));
    public IObservable<IEnumerable<ProfileDto>> GetProfiles(bool forceUpdate = false) =>
        Observable.Create<IEnumerable<ProfileDto>>(observer => _api.GetProfiles().Subscribe(observer));
}
