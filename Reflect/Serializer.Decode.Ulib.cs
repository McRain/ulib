namespace ULIB
{
	public partial class Serializer
	{
		/*private ULabelModel DecodeULabelModel(byte[] inBytes, ref int startPos)
		{
			var result = new ULabelModel
			{
				id = DecodeInteger(inBytes, ref startPos),
				label = DecodeString(inBytes, ref startPos)
			};
			return result;
		}

		private UModel DecodeUModel(byte[] inBytes, ref int startPos)
		{
			var result = new UModel
			             	{
			             		id = DecodeInteger(inBytes, ref startPos),
			             		label = DecodeString(inBytes, ref startPos),
			             		target = DecodeString(inBytes, ref startPos),
			             		member = DecodeString(inBytes, ref startPos),
			             		parameters = (object[]) Decoding(inBytes, ref startPos),
			             		value = Decoding(inBytes, ref startPos)
			             	};
			return result;
		}

		private UCommand DecodeUCommand(byte[] inBytes, ref int startPos)
		{
			var result = new UCommand
			{
				id = DecodeInteger(inBytes, ref startPos),
				label = DecodeString(inBytes, ref startPos),
				target = DecodeString(inBytes, ref startPos),
				member = DecodeString(inBytes, ref startPos),
				parameters = (object[])Decoding(inBytes, ref startPos),
				value = Decoding(inBytes, ref startPos),
				IsPrevios = DecodeBoolean(inBytes,ref startPos)
			};
			return result;
		}

		private static readonly Dictionary<byte, string> ByteToCompare = new Dictionary<byte, string>
					{ {0x1,"<"}, {0x2,"<="},{0x3,"=="},
						{0x4,">="},{0x5,">"},{0x6,"!="} };

		private UCompare DecodeUCompare(byte[] inBytes, ref int startPos)
		{
			var result = new UCompare
			{
				id = DecodeInteger(inBytes, ref startPos),
				label = DecodeString(inBytes, ref startPos),
				target = DecodeString(inBytes, ref startPos),
				member = DecodeString(inBytes, ref startPos),
				parameters = (object[])Decoding(inBytes, ref startPos),
				value = Decoding(inBytes, ref startPos),
				condition = ByteToCompare[inBytes[startPos]] 
			};
			startPos++;
			return result;
		}

		private UValue DecodeUValue(byte[] inBytes, ref int startPos)
		{
			var result = new UValue
			{
				id = DecodeInteger(inBytes, ref startPos),
				label = DecodeString(inBytes, ref startPos),
				target = DecodeString(inBytes, ref startPos),
				member = DecodeString(inBytes, ref startPos),
				parameters = (object[])Decoding(inBytes, ref startPos),
				value = Decoding(inBytes, ref startPos),
				min = Decoding(inBytes,ref startPos),
				max = Decoding(inBytes, ref startPos),
				changed = DecodeBoolean(inBytes,ref startPos)
			};
			return result;
		}

		private UMenu DecodeUMenu(byte[] inBytes, ref int startPos)
		{
			var result = new UMenu
			{
				id = DecodeInteger(inBytes, ref startPos),
				label = DecodeString(inBytes, ref startPos),
				parent = DecodeInteger(inBytes,ref startPos),
				commandIds = (List<int>)Decoding(inBytes,ref startPos),
				commands = (List<UCommand>)Decoding(inBytes, ref startPos)
			};
			return result;
		}*/

	}
}
