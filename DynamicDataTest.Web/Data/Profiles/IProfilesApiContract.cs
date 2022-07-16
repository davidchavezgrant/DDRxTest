using DynamicDataTest.Contracts;

using Refit;

namespace DynamicDataTest.Web.Data.Profiles;

interface IProfilesApiContract
{
    [Get("/profiles")]
    IObservable<IEnumerable<ProfileDto>> GetProfiles();
    
    [Get("/profiles/{id}")]
    IObservable<ProfileDto> GetProfile(Guid id);

    [Post("/profiles")]
    IObservable<ProfileDto> CreateProfile([Body] ProfileDto profile);
}
