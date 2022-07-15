using DynamicDataTest.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace DynamicDataTest.Api.DAL;

sealed class ProfilesDbContext : DbContext
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public ProfilesDbContext(DbContextOptions options) : base(options)
    {
    }
}
