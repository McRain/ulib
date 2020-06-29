/// <summary>
/// 
/// </summary>
public class UModel:ULabelModel
{
	/// <summary>
	///
	/// </summary>
	public string target = "";
	/// <summary>
	/// 
	/// </summary>
	public string member = "";
	/// <summary>
	/// 
	/// </summary>
	public object[] parameters = new object[0];

	/// <summary>
	/// 
	/// </summary>
	public UModel()
	{
		value = new object();
	}

	/// <summary>
	/// 
	/// </summary>
	public object value { get; set; }
}

