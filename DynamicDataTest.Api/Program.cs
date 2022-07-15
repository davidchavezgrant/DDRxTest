using DynamicDataTest.Api.DAL;
using DynamicDataTest.Api.Models;
using DynamicDataTest.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProfilesDbContext>(options => options.UseSqlite("Data Source=profiles.db;"));
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGet("/api/profiles", (ProfilesDbContext dbContext) => 
        {
            var profiles = dbContext.Profiles.ToList();
            return profiles;
        });
app.MapGet("/api/profiles/{id}", (Guid id, ProfilesDbContext dbContext) => 
        {
            var profile = dbContext.Profiles.FirstOrDefault(p => p.Id == id);
            return profile;
        });
app.MapPost("/api/profiles", (ProfileDto dto, ProfilesDbContext dbContext) => 
        {
            var profile = Profile.FromDto(dto);
            dbContext.Profiles.Add(profile);
            dbContext.SaveChanges();
            return profile;
        });
app.Run();
