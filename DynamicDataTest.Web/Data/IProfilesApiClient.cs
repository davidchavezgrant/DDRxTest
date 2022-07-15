using DynamicDataTest.Contracts;
namespace DynamicDataTest.Web.Data;

interface IProfilesApiClient
{
    IObservable<ProfileDto> GetProfile(Guid id, bool forceUpdate = false);
    IObservable<IEnumerable<ProfileDto>> GetProfiles(bool forceUpdate = false);
}
