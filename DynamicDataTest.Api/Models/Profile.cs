using DynamicDataTest.Contracts;
namespace DynamicDataTest.Api.Models;

class Profile
{
    public Guid Id {get; set;}
    public string Handle {get; set;}
    public DateTime Created {get; set;}
    public string Bio {get; set;}
    public static Profile FromDto(ProfileDto dto)
    {
        return new Profile 
        {
            Id = dto.Id,
            Handle =dto.Username,
            Bio = dto.Bio,
            Created = DateTime.Now
        };
    }
}
