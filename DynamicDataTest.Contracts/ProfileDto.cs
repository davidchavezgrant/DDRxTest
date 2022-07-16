using System.ComponentModel;
using System.Reactive;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DynamicDataTest.Contracts;

public sealed class ProfileDto : ReactiveObject 
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
	[Reactive]
	public ReactiveCommand<Unit, Guid> RemoveCommand { get; private set; }
}
