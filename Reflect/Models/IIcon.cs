using UnityEngine;

namespace ULIB
{
	interface IIcon
	{
		/// <summary>
		/// Return icon texture. For change use IconName
		/// </summary>
		Texture2D Icon { get; }

		/// <summary>
		/// Change icon name. If ResourceManager contains not icon in cache - texture will be load
		/// </summary>
		string IconName { get; set; }
	}
}
