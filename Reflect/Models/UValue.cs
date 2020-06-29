/// <summary>
/// 
/// </summary>
public class UValue:UModel
{
    public string key;
	/// <summary>
	/// The minimum value
	/// </summary>
	public object min;
	/// <summary>
	/// The maximum value
	/// </summary>
	public object max;
	/// <summary>
	/// if value changes
	/// </summary>
	public bool changed;

	/// <summary>
	/// Return copy
	/// </summary>
	/// <returns></returns>
	public object GetCopy()
	{
		return MemberwiseClone();
	}
}