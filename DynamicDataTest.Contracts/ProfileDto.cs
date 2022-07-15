using System.Reactive;

using ReactiveUI;

namespace DynamicDataTest.Contracts;

public sealed record ProfileDto
{
	public Guid   Id       { get; init; }
	public string Username { get; init; }
	public string Bio      { get; init; }

	public ProfileDto(Guid id, string username, string bio)
	{
		this.Id       = id;
		this.Username = username;
		this.Bio      = bio;
		this.RemoveCommand = ReactiveCommand.Create(() => this.Id);
	}
	
	public ReactiveCommand<Unit, Guid> RemoveCommand { get; private set; }
}
