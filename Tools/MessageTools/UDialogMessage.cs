namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public class UDialogMessage
	{
		private UMessage _msg;
		private bool _showed;
		private float _showTime = 5.0f;

		/// <summary>
		/// 
		/// </summary>
		//public List<MessageEvent> events = new List<MessageEvent>();

		/// <summary>
		/// 
		/// </summary>
		public void Show()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="umsg"></param>
		public void Show(UMessage umsg)
		{
			_msg = umsg;
			_showed = true;
			/*if (_msg.type != DialogMessageType.QUERY)
				_showTime = 2.5f;*/
		}

		
	}
}
