using System.Collections.Generic;
using System.Reflection;

namespace ULIB
{
	/// <summary>
	/// Base class for ULIB managers.
	/// Contains methods and fields for add/remove targets and members
	/// </summary>
	public abstract class BaseManager
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
		protected static readonly Dictionary<string, object> Targets = new Dictionary<string, object>();
        /// <summary>
        /// 
        /// </summary>
		protected static readonly Dictionary<string, MemberInfo> Members = new Dictionary<string, MemberInfo>();

        #endregion

        #region Targets and Members

        /// <summary>
		/// Add object to TargetList and all allowed members from object to MemberList. 
		/// Members must contains AccessAllow attribute
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
			FillMembers(targetName,newTarget);
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

        }

        /// <summary>
        /// Add object to TargetList and member by name from object to MemberList. 
        /// If target exists - previos members not removed.
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="targetObject"></param>
        /// <param name="memberName"></param>
        public static void AddMember(string targetName, object targetObject, string memberName)
        {
            if (Targets.ContainsKey(targetName))
                Targets[targetName] = targetObject;
            else
                Targets.Add(targetName, targetObject);
            var member = targetObject.GetType().GetMember(memberName)[0];
            if (member != null)
                Members.Add(targetName + "." + memberName, member);
            else
                ULog.Log("Member "+memberName+" not find in "+targetName+" ("+targetObject+")");
        }

        /// <summary>
        /// Remove member from list.
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="memberName"></param>
        public static void RemoveMember(string targetName, string memberName)
        {
            if (Members.ContainsKey(targetName + "." + memberName))
                lock (Members)
                    Members.Remove(targetName + "." + memberName);
        }

        /// <summary>
        /// Remove all members from list by target name.
        /// </summary>
        /// <param name="targetName"></param>
        public static void RemoveMembers(string targetName)
        {
            var removedMembers = new List<string>();
            foreach (var member in Members)
                if (member.Key == targetName + "." + member.Value.Name)
                    removedMembers.Add(targetName + "." + member.Value.Name);
            /*var removedMembers = (from member in Members
                                  where member.Key == targetName + "." + member.Value.Name
                                  select targetName + "." + member.Value.Name).ToList();*/
            lock (Members)
                foreach (var removedMember in removedMembers)
                    Members.Remove(removedMember);
        }
        
        /*
		/// <summary>
		/// Return true if taget by name exists in TargetList
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		public static bool TargetExists(string targetName)
		{
			return Targets.ContainsKey(targetName);
		}*/

        /// <summary>
        /// Fill member from Fields and Properties.
        /// Member must be with attribute 'AccessAllow' and 'AccessAllow=true'
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="target"></param>
		protected static void FillMembers(string targetName,object target)
		{
			//var tp = target.GetType();
            var members = target.GetType().GetMembers();
            foreach (var member in members)
                if (member.GetCustomAttributes(typeof (AccessAllow), false).Length > 0 &&
                    ((AccessAllow) member.GetCustomAttributes(typeof (AccessAllow), false)[0]).Allow &&
                    (member is FieldInfo ||
                     (member is PropertyInfo && ((PropertyInfo) member).CanWrite && ((PropertyInfo) member).CanRead)))
                    Members.Add(targetName + "." + member.Name, member);
            /*if (member is FieldInfo || (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead))
                    Members.Add(targetName + "." + member.Name, member);*/


			/*foreach (var member in members.Where(member => member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
												((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow))
				Members.Add(targetName + "." + member.Name, member);*/
        }

        #endregion
    }
}
