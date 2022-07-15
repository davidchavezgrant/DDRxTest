using Refit;
using DynamicDataTest.Contracts;
namespace DynamicDataTest.Web.Data;

interface IProfilesApiContract
{
    [Get("/profiles")]
    IObservable<IEnumerable<ProfileDto>> GetProfiles();
    
    [Get("/profiles/{id}")]
    IObservable<ProfileDto> GetProfile(Guid id);

    [Post("/profiles")]
    IObservable<ProfileDto> CreateProfile([Body] ProfileDto profile);
}
