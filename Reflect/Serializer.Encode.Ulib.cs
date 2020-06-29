using System.Collections.Generic;

namespace ULIB
{
	public partial class Serializer
	{
		private static readonly Dictionary<string, byte> CompareToByte = new Dictionary<string, byte>
		                                                                 	{ {"<",0x1}, {"<=",0x2},{"==",0x3},
		                                                                 	  {">=",0x4},{">",0x5},{"!=",0x6} };

		/*private byte[] EncodeULabelModel(ULabelModel inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;
			var result = new byte[4+labelLength];
			Buffer.BlockCopy(EncodeInteger(inObject.id),0,result,0,4);
			Buffer.BlockCopy(labelBytes,0,result,4,labelLength);
			return result;
		}

		private byte[] EncodeUModel(UModel inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;

			var targetBytes = EncodeString(inObject.target);
			var targetLength = targetBytes.Length;

			var memberBytes = EncodeString(inObject.member);
			var memberLength = memberBytes.Length;

			var paramBytes = Encoding(inObject.parameters);
			var paramLength = paramBytes.Length;

			var valueBytes = Encoding(inObject.value);
			var valueLength = valueBytes.Length;

			var result = new byte[4 + labelLength + targetLength + memberLength + paramLength + valueLength];

			Buffer.BlockCopy(EncodeInteger(inObject.id), 0, result, 0, 4);
			var pos = 4;

			Buffer.BlockCopy(labelBytes, 0, result, pos, labelLength);
			pos += labelLength;

			Buffer.BlockCopy(targetBytes, 0, result, pos, targetLength);
			pos += targetLength;

			Buffer.BlockCopy(memberBytes, 0, result, pos, memberLength);
			pos += memberLength;

			Buffer.BlockCopy(paramBytes, 0, result, pos, paramLength);
			pos += paramLength;

			Buffer.BlockCopy(valueBytes, 0, result, pos, valueLength);

			return result;
		}

		private byte[] EncodeUCompare(UCompare inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;

			var targetBytes = EncodeString(inObject.target);
			var targetLength = targetBytes.Length;

			var memberBytes = EncodeString(inObject.member);
			var memberLength = memberBytes.Length;

			var paramBytes = Encoding(inObject.parameters);
			var paramLength = paramBytes.Length;

			var valueBytes = Encoding(inObject.value);
			var valueLength = valueBytes.Length;

			var condBytes = new[] {CompareToByte[inObject.condition]};
			
			var result = new byte[4 + labelLength + targetLength + memberLength + paramLength + valueLength + 1];

			Buffer.BlockCopy(EncodeInteger(inObject.id), 0, result, 0, 4);
			var pos = 4;

			Buffer.BlockCopy(labelBytes, 0, result, pos, labelLength);
			pos += labelLength;

			Buffer.BlockCopy(targetBytes, 0, result, pos, targetLength);
			pos += targetLength;

			Buffer.BlockCopy(memberBytes, 0, result, pos, memberLength);
			pos += memberLength;

			Buffer.BlockCopy(paramBytes, 0, result, pos, paramLength);
			pos += paramLength;

			Buffer.BlockCopy(valueBytes, 0, result, pos, valueLength);
			pos += valueLength;

			Buffer.BlockCopy(condBytes, 0, result, pos, 1);

			return result;
		}

		private byte[] EncodeUCommand(UCommand inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;

			var targetBytes = EncodeString(inObject.target);
			var targetLength = targetBytes.Length;

			var memberBytes = EncodeString(inObject.member);
			var memberLength = memberBytes.Length;

			var paramBytes = Encoding(inObject.parameters);
			var paramLength = paramBytes.Length;

			var valueBytes = Encoding(inObject.value);
			var valueLength = valueBytes.Length;

			var ispreviosByte = EncodeBoolean(inObject.IsPrevios);

			var result = new byte[4 + labelLength + targetLength + memberLength + paramLength + valueLength+1];

			Buffer.BlockCopy(EncodeInteger(inObject.id), 0, result, 0, 4);
			var pos = 4;

			Buffer.BlockCopy(labelBytes, 0, result, pos, labelLength);
			pos += labelLength;

			Buffer.BlockCopy(targetBytes, 0, result, pos, targetLength);
			pos += targetLength;

			Buffer.BlockCopy(memberBytes, 0, result, pos, memberLength);
			pos += memberLength;

			Buffer.BlockCopy(paramBytes, 0, result, pos, paramLength);
			pos += paramLength;

			Buffer.BlockCopy(valueBytes, 0, result, pos, valueLength);
			pos += valueLength;

			Buffer.BlockCopy(ispreviosByte, 0, result, pos, 1);

			return result;
		}

		private byte[] EncodeUValue(UValue inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;

			var targetBytes = EncodeString(inObject.target);
			var targetLength = targetBytes.Length;

			var memberBytes = EncodeString(inObject.member);
			var memberLength = memberBytes.Length;

			var paramBytes = Encoding(inObject.parameters);
			var paramLength = paramBytes.Length;

			var valueBytes = Encoding(inObject.value);
			var valueLength = valueBytes.Length;

			var minBytes = Encoding(inObject.min);
			var minBytesLength = minBytes.Length;

			var maxBytes = Encoding(inObject.max);
			var maxBytesLength = maxBytes.Length;

			var changedBytes = EncodeBoolean(inObject.changed);

			var result = new byte[4 + labelLength + targetLength + memberLength + paramLength + valueLength + minBytesLength+maxBytesLength+1];

			Buffer.BlockCopy(EncodeInteger(inObject.id), 0, result, 0, 4);
			var pos = 4;

			Buffer.BlockCopy(labelBytes, 0, result, pos, labelLength);
			pos += labelLength;

			Buffer.BlockCopy(targetBytes, 0, result, pos, targetLength);
			pos += targetLength;

			Buffer.BlockCopy(memberBytes, 0, result, pos, memberLength);
			pos += memberLength;

			Buffer.BlockCopy(paramBytes, 0, result, pos, paramLength);
			pos += paramLength;

			Buffer.BlockCopy(valueBytes, 0, result, pos, valueLength);
			pos += valueLength;

			Buffer.BlockCopy(minBytes, 0, result, pos, minBytesLength);
			pos += minBytesLength;

			Buffer.BlockCopy(maxBytes, 0, result, pos, maxBytesLength);
			pos += maxBytesLength;

			Buffer.BlockCopy(changedBytes, 0, result, pos, 1);

			return result;
		}

		private byte[] EncodeUMenu(UMenu inObject)
		{
			var labelBytes = EncodeString(inObject.label);
			var labelLength = labelBytes.Length;

			var parentBytes = EncodeInteger(inObject.parent);
			var parentLength = parentBytes.Length;

			var comIdsBytes = Encoding(inObject.commandIds);
			var comIdsLength = comIdsBytes.Length;

			var comBytes = Encoding(inObject.commands);
			var comLength = comBytes.Length;

			var result = new byte[4 + labelLength + parentLength+comIdsLength+comLength];

			Buffer.BlockCopy(EncodeInteger(inObject.id), 0, result, 0, 4);
			
			Buffer.BlockCopy(labelBytes, 0, result, 4, labelLength);
			var pos = 4+labelLength;

			Buffer.BlockCopy(comIdsBytes, 0, result, pos, comIdsLength);
			pos += comIdsLength;

			Buffer.BlockCopy(comBytes, 0, result, pos, comLength);

			return result;
		}*/
		
	}
}
