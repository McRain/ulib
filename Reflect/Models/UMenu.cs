using System.Collections.Generic;

/// <summary>
/// Class that represents a menu item
/// </summary>
public class UMenu:UChildModel
{
	/// <summary>
	/// List of commands id performed during the activation menu
	/// </summary>
	public List<int> commandIds = new List<int>();

	/// <summary>
	/// List of command performed during the activation menu
	/// </summary>
	public List<UCommand> commands = new List<UCommand>();
}