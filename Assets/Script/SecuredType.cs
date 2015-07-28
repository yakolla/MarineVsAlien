using System;
namespace SecuredType
{
	public struct Util
	{
		public static int key = 0x7fc8a04f;

		public static int FloatToInt(float value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}
		
		public static float IntToFloat(int value)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
		}
	}

	public struct XInt
	{
		int m_value;
		public XInt(int d)
		{
			m_value = d ^ Util.key;
		}

		public static implicit operator XInt(int d)  // implicit digit to byte conversion operator
		{
			return new XInt(d);
		}

		public int Value
		{
			get{return m_value ^ Util.key;}
			set{m_value = value ^ Util.key;}
		}
	}

	public struct XFloat
	{
		int m_value;
		public XFloat(float d)
		{
			m_value = Util.FloatToInt(d) ^ Util.key;
		}
		
		public static implicit operator XFloat(float d)  // implicit digit to byte conversion operator
		{
			return new XFloat(d);
		}
		
		public float Value
		{
			get{return Util.IntToFloat(m_value ^ Util.key);}
			set{m_value = Util.FloatToInt(value) ^ Util.key;}
		}
	}
}

