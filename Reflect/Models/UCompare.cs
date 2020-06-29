/// <summary>
/// Used for compare value with any data from target object<br/>
/// <example><b>For example:</b> if(CompareManager.Compare(myUCompare)){gamePassed=true;}</example>
/// </summary>
public class UCompare:UModel
{
	/// <summary>
	/// Sign comparison <br/>
	/// You can use &lt;, &lt;=,==,!=,&gt;=,&gt;
	/// </summary>
	public string condition = "==";
}
