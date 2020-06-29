using System;
using System.Collections.Generic;
using System.Reflection;

namespace ULIB
{
	/// <summary>
	/// Manager for compare values
	/// </summary>
	public sealed class CompareManager:BaseManager
	{
		/// <summary>
		/// Instance of CompareManager
		/// </summary>
		private static CompareManager _instance;

		/*static readonly Dictionary<string, object> Targets = new Dictionary<string, object>();
		static readonly Dictionary<string, MemberInfo> Members = new Dictionary<string, MemberInfo>();*/
		
		private static Dictionary<int, UCompare> _compares = new Dictionary<int, UCompare>();

        /*
		/// <summary>
		/// Add object to TargetList and members from object to MemberList. 
		/// Members of target must contains AccessAllow attribute
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="newTarget"></param>
		public static void AddTarget(string targetName, object newTarget)
		{
			if (!Targets.ContainsKey(targetName))
				Targets.Add(targetName, newTarget);
			else
			{
				RemoveMembers(targetName);
				Targets[targetName] = newTarget;
			}
			FillMembers(targetName, newTarget);
		}

		/// <summary>
		/// Remove target and members from lists
		/// </summary>
		/// <param name="targetName"></param>
		public static void RemoveTarget(string targetName)
		{
			if (Targets.ContainsKey(targetName))
			{
				RemoveMembers(targetName);
				Targets.Remove(targetName);
			}

		}*/

		/// <summary>
		/// Return true if target find in list by name 
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		public static bool TargetExists(string targetName)
		{
			return Targets.ContainsKey(targetName);
		}

		/*static void FillMembers(string targetName, object target)
		{
			//Debug.Log("FillMembers " + targetName+":"+target);

			var tp = target.GetType();
			var members = tp.GetMembers();
			foreach (var memberInfo in members)
			{
				if (memberInfo.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
					((AccessAllow)memberInfo.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow)
				{
					Members.Add(targetName + "." + memberInfo.Name, memberInfo);
				}
			}

			/*foreach (var member in members.Where(member => member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
												((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow))
			{
				//Debug.Log("Add " + targetName + "." + member.Name+"."+member);
				
			}*/
		/*}

		static void RemoveMembers(string targetName)
		{
			var removedMembers = new List<string>();
			foreach (var member in Members)
				if (member.Key == targetName + "." + member.Value.Name)
					removedMembers.Add(targetName + "." + member.Value.Name);
			lock (Members)
				foreach (var removedMember in removedMembers)
					Members.Remove(removedMember);
		}*/

		#region Instance
		/// <summary>
		/// Return instance of CompareManager
		/// </summary>
		public static CompareManager Instance
		{
			get { return _instance ?? (_instance = new CompareManager()); }
		}

		/// <summary>
		/// Return instance of CompareManager
		/// </summary>
		public static CompareManager GetInstance()
		{
			return _instance ?? (_instance = new CompareManager());
		}
		#endregion

		#region Compares

		/// <summary>
		/// Adds UCompare into the internal array in CompareManager
		/// </summary>
		/// <param name="uCompare"></param>
		public static void AddCompare(UCompare uCompare)
		{
			if (!_compares.ContainsKey(uCompare.id))
				_compares.Add(uCompare.id, uCompare);
			else
				_compares[uCompare.id] = uCompare;
		}

		/// <summary>
		/// Adds array of UCompare into the internal array in CompareManager.<br/> 
		/// If replace is true then newCompares replace compare if exists
		/// </summary>
		/// <param name="newCompares"></param>
		/// <param name="replace"></param>
		public static void AddCompares(Dictionary<int, UCompare> newCompares, bool replace)
		{
			if (replace)
				_compares = newCompares;
			else
				foreach (var newCompare in newCompares)
					_compares.Add(newCompare.Key, newCompare.Value);
		}


		///<summary>
		/// Returns the presence of compare in list
		///</summary>
		///<param name="compareId"></param>
		///<returns></returns>
		public static bool CompareExists(int compareId)
		{
			return _compares.ContainsKey(compareId);
		}

		/// <summary>
		/// Remove compare with id from list
		/// </summary>
		/// <param name="compareId"></param>
		public static void RemoveCompare(int compareId)
		{
			if (_compares.ContainsKey(compareId))
				lock (_compares)
					_compares.Remove(compareId);
		}


		#endregion

		#region Compare
		/// <summary>
		/// Returns the result of the comparison
		/// </summary>
		/// <param name="compareModel"></param>
		/// <returns></returns>
		public static bool Compare(UCompare compareModel)
		{
			var result = false;
			if (Targets.ContainsKey(compareModel.target))
			{
				var targetObject = Targets[compareModel.target];
				if (Members.ContainsKey(compareModel.member))
				{
					var targetMember = Members[compareModel.member];

					if (targetMember is FieldInfo)
						result = Comparison(((FieldInfo) targetMember).GetValue(targetObject), compareModel.value, compareModel.condition);
					else if (targetMember is PropertyInfo)
						result = Comparison(((PropertyInfo) targetMember).GetValue(targetObject, null), compareModel.value,
						                    compareModel.condition);
					else
					{
						try
						{
							result = Comparison(((MethodInfo) targetMember).Invoke(targetObject, compareModel.parameters), compareModel.value,
							                    compareModel.condition);
						}
						catch (Exception e)
						{
							ULog.Log("CompareManager:Compare:Method. \n" + e.Message, ULogType.Error);
							//InvokeCompareError(compareModel,CompareEventType.Error,e.Message);
							//Debug.LogError("CompareManager:Compare method " + compareModel.member + " error: " + e.Message);
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a list of UCompare for which the comparison is false
		/// </summary>
		/// <param name="compareIds">id compare from compareList</param>
		/// <returns></returns>
		public static List<UCompare> Compare(int[] compareIds)
		{
			var result = new List<UCompare>();
			if (compareIds != null)
			{
				foreach (var compareId in compareIds)
				{
					if(_compares.ContainsKey(compareId))
					{
						var passed = Compare(_compares[compareId]);
						if(!passed)
							result.Add(_compares[compareId]);
					}
				}
			}
				/*result.AddRange(compareIds.Where(
					compareId => _compares.ContainsKey(compareId)).Select(
						compareId => _compares[compareId]).Where(
							compareModel => !Compare(compareModel)));*/
			return result;
		}

		/// <summary>
		/// Returns a list of UCompare for which the comparison is false
		/// </summary>
		/// <param name="compares"></param>
		/// <returns></returns>
		public static List<UCompare> Compare(List<UCompare> compares)
		{
			var result = new List<UCompare>();
			if (compares != null)
			{
				foreach (var uCompare in compares)
				{
					var passed = Compare(uCompare);
					if(!passed)
						result.Add(uCompare);
				}
			}
			//result.AddRange(compares.Where(compareModel => !Compare(compareModel)));
			return result;
		}

		private static bool Comparison(object fieldValue, object comparedValue, string condition)
		{
			var result = false;
			try
			{
				if (fieldValue is Int32)
					fieldValue = BitConverter.ToSingle(BitConverter.GetBytes((int)fieldValue), 0);
				if (comparedValue is Int32)
					comparedValue = BitConverter.ToSingle(BitConverter.GetBytes((int)comparedValue), 0);

				switch (condition)
				{
					case "<":
						if ((float)fieldValue < (float)comparedValue)
							result = true;
						break;
					case "<=":
						if ((float)fieldValue <= (float)comparedValue)
							result = true;
						break;
					case "==":
						if (fieldValue.Equals(comparedValue))
						{
							result = true;
						}
						break;
					case ">=":
						if ((float)fieldValue >= (float)comparedValue)
							result = true;
						break;
					case ">":
						if ((float)fieldValue > (float)comparedValue)
							result = true;
						break;
					case "!=":
						if (!fieldValue.Equals(comparedValue))
							result = true;
						break;
					/*case "contains":
					{
						//string stringArray = comparedValue.ToString();
						var values = (IList)comparedValue;//stringArray.Split(new char[] { ';' });
						foreach (var val in values.Cast<object>().Where(val => ((IList)fieldValue).Contains(val) == true))
						{
							result = true;
						}
					}
					break;
				case "!contains":
					{
						//string stringArray = comparedValue.ToString();
						var values = (IList)comparedValue; //stringArray.Split(new char[] { ';' });
						foreach (var val in values.Cast<object>().Where(val => ((IList)fieldValue).Contains(val) == true))
						{
							result = true;
						}
					}
					break;*/
					/*case "exists":
					{
						var values = (IList)comparedValue;
						if (values.Contains(fieldValue))
						{
							result = true;
						}
					}
					break;
				case "!exists":
					{
						result = true;
						var values = (IList)comparedValue;
						if (values.Contains(fieldValue))
						{
							result = false;
						}
					}
					break;*/
				}
			}
			catch (Exception e)
			{
				ULog.Log("CompareManager:Comparison:Error \n"+e.Message,ULogType.Error);
				//InvokeCompareError(new[]{fieldValue,comparedValue,condition}, CompareEventType.Error, e.Message);
				//Debug.LogError("CompareManager:Comparison error - " + e.Message);
			}
			return result;
		}
		#endregion
	}
}