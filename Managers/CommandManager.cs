using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ULIB
{
	/// <summary>
	/// Represents a tools for execute commands
	/// </summary>
	public sealed class CommandManager
	{
		private static CommandManager _instance;

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, object> Targets = new Dictionary<string, object>();
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, MemberInfo> Members = new Dictionary<string, MemberInfo>();
        /// <summary>
        /// List of command 
        /// </summary>
        private static Dictionary<int, UCommand> _commands = new Dictionary<int, UCommand>();

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
            FillMembers(targetName, newTarget);
        }

        /// <summary>
        /// Remove target and members from lists
        /// </summary>
        /// <param name="targetName"></param>
        public static void RemoveTarget(string targetName)
        {
            if (!Targets.ContainsKey(targetName)) return;
            RemoveMembers(targetName);
            Targets.Remove(targetName);
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
                ULog.Log("Member " + memberName + " not find in " + targetName + " (" + targetObject + ")");
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
            var removedMembers = (from member in Members where member.Key == targetName + "." + member.Value.Name select targetName + "." + member.Value.Name).ToList();
            /*var removedMembers = new List<string>();
            foreach (var member in Members)
                if (member.Key == targetName + "." + member.Value.Name)
                    removedMembers.Add(targetName + "." + member.Value.Name);*/
            /*var removedMembers = (from member in Members
                                  where member.Key == targetName + "." + member.Value.Name
                                  select targetName + "." + member.Value.Name).ToList();*/
            lock (Members)
                foreach (var removedMember in removedMembers)
                    Members.Remove(removedMember);
        }

        /// <summary>
        /// Return true if taget by name exists in TargetList
        /// </summary>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static bool TargetExists(string targetName)
        {
            return Targets.ContainsKey(targetName);
        }

        /// <summary>
        /// Fill member from Fields and Properties.
        /// Member must be with attribute 'AccessAllow' and 'AccessAllow=true'
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="target"></param>
        private static void FillMembers(string targetName, object target)
        {
            //var tp = target.GetType();
            var members = target.GetType().GetMembers();
            foreach (var member in members.Where(member => member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
                                                           ((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow &&
                                                           (member is FieldInfo ||
                                                            (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead))))
                Members.Add(targetName + "." + member.Name, member);
            /*foreach (var member in members)
                if (member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
                    ((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow &&
                    (member is FieldInfo ||
                     (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead)))
                    Members.Add(targetName + "." + member.Name, member);*/
            /*if (member is FieldInfo || (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead))
                    Members.Add(targetName + "." + member.Name, member);*/


            /*foreach (var member in members.Where(member => member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
                                                ((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow))
                Members.Add(targetName + "." + member.Name, member);*/
        }

	    #endregion
        
        #region Instance

        /// <summary>
		/// Return instance of CommandManager
		/// </summary>
		public static CommandManager Instance
		{
			get { return _instance ?? (_instance = new CommandManager()); }
		}

		/// <summary>
		/// Return instance of CommandManager
		/// </summary>
		public static CommandManager GetInstance()
		{
			return _instance ?? (_instance = new CommandManager());
		}

		#endregion
		
		#region Command

		/// <summary>
		/// Add one command to list
		/// If the list contains the command with some ID - it will be overwritten
		/// </summary>
		/// <param name="newCommand"></param>
		public static void AddCommand(UCommand newCommand)
		{
			if (!_commands.ContainsKey(newCommand.id))
				_commands.Add(newCommand.id, newCommand);
			else
				_commands[newCommand.id] = newCommand;
		}

		/// <summary>
		/// Set or add range of command to commandlist
		/// </summary>
		/// <param name="newCommands">The added range</param>
		/// <param name="replace">If true - old command list will be overwritten. If false - command will be add to commandlist </param>
		public static void AddCommands(Dictionary<int, UCommand> newCommands, bool replace)
		{
			if (!replace)
				foreach (var newCommand in newCommands)
					_commands.Add(newCommand.Key, newCommand.Value);
			else
				_commands = newCommands;
		}

		///<summary>
		/// Returns the presence of command in list
		///</summary>
		///<param name="commandId"></param>
		///<returns></returns>
		public static bool CommandExists(int commandId)
		{
			return _commands.ContainsKey(commandId);
		}

		/// <summary>
		/// Remove command with id from list
		/// </summary>
		/// <param name="commandId"></param>
		public static void RemoveCommand(int commandId)
		{
			lock (_commands)
				if (_commands.ContainsKey(commandId))
					_commands.Remove(commandId);
		}

		#endregion

		#region Execute
		/// <summary>
		/// Returns the result of execution of command 
		/// </summary>
		/// <param name="command">Command for run</param>
		/// <returns>Returns null or result</returns>
		public static object Execute(UCommand command)
		{
			object result = null;

			if (Targets.ContainsKey(command.target))
			{
				try
				{
					/*if (command.IsPrevios)
						command.value = result;*/

					var targetObject = Targets[command.target];
					if (Members.ContainsKey(string.Format("{0}.{1}", command.target, command.member)))
					{
						var targetMember = Members[string.Format("{0}.{1}", command.target, command.member)];

						if (targetMember is FieldInfo)
							((FieldInfo)targetMember).SetValue(targetObject, command.value);
						else if (targetMember is PropertyInfo)
							((PropertyInfo)targetMember).SetValue(targetObject, command.value, null);
						else
						{
							try
							{
								/*if (evt.IsPrevios && evt.parameters.Length > evt.parameters.Length - 1)//???? >0 ???
								evt.parameters[evt.parameters.Length - 1] = previosResult;*/
								result = ((MethodInfo)targetMember).Invoke(targetObject, command.parameters);
							}
							catch (Exception e)
							{
								ULog.Log("CommandManager:Execute:Method \n" + e.Message, ULogType.Error);
								//InvokeCommandError(command, CommandEventType.Error, e);
								//Debug.Log("Target is " + targetObject);
								//Debug.LogError("Unable to execute method " + eventTargetM.Name + " in target " + command.target);
							}
						}
						//InvokeCommandError(command, CommandEventType.NotFindMemebr);
						//Debug.LogWarning("Not find Field/Method : " + command.member + " in target " + command.target);
					}
					else
					{
					    foreach (var member in Members)
                            ULog.Log("CommandManager:Execute:Member " + member.Key + ":" + member.Value.Name, ULogType.Warning);
					    ULog.Log("CommandManager:Execute:Member Not Find " + command.member, ULogType.Warning);
					}
				}
				catch (Exception e)
				{
					ULog.Log("CommandManager:Execute:Error \n" + e.Message, ULogType.Error);
					//InvokeCommandError(command,CommandEventType.Error,e);
					//Console.WriteLine("CommandManager: Execute error {0}", e.Message);
				}
			}
			else
				ULog.Log("CommandManager:Execute:Target Not Find " + command.target, ULogType.Warning);
			//InvokeCommandError(command, CommandEventType.NotFindTarget);
			//Debug.LogWarning("Not find target : " + command.target);

			return result;
		}

		/// <summary>
		/// Returns the result of execution of command found in the commandlist by ID
		/// </summary>
		/// <param name="commandId">ID of task</param>
		/// <returns>NULL or result</returns>
		public static object Execute(int commandId)
		{
			if (_commands.ContainsKey(commandId))
				return Execute(_commands[commandId]);
			ULog.Log("CommandManager:Execute:Command id "+commandId+" Not Find ", ULogType.Warning);
			//InvokeCommandError(commandId, CommandEventType.NotFindCommand);
			return null;
		}

		/// <summary>
		/// Returns the results of execution of the commandlist
		/// </summary>
		/// <param name="inCommand">List of command</param>
		/// <returns>Returns the result of the last command or null</returns>
		public static object[] Execute(List<UCommand> inCommand)
		{
			var results = new object[inCommand.Count];
			for (var i = 0; i < inCommand.Count; i++)
				results[i] = Execute(inCommand[i]);
			return results;
		}

		/// <summary>
		/// Returns the result of execution of the last command,from founded in the commandlist by ID
		/// </summary>
		/// <param name="inCommandIds">Array of command id</param>
		/// <returns>Returns the result of the last command or null</returns>
		public static object[] Execute(int[] inCommandIds)
		{
			var results = new object[inCommandIds.Length];
			for (var i = 0; i < inCommandIds.Length; i++)
				results[i] = Execute(inCommandIds[i]);
			return results;
		}

        /// <summary>
        /// Returns the result of execution of the last command,from founded in the commandlist by ID
        /// </summary>
        /// <param name="inCommandIds">List of command id</param>
        /// <returns>Returns the result of the last command or null</returns>
        public static object[] Execute(List<int> inCommandIds)
        {
            var count = inCommandIds.Count;
            var results = new object[count];
            for (var i = 0; i < count; i++)
                results[i] = Execute(inCommandIds[i]);
            return results;
        }

		#endregion

        #region Interpretation

        /// <summary>
        /// 
        /// </summary>
        public static void Parse()
        {
            
        }

        #endregion

    }
}