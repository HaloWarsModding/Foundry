﻿
namespace KSoft.IO
{
	partial class TagElementTextStream<TDoc, TCursor>
	{
		#region WriteElement impl
		protected override void WriteElement(TCursor n, char value)
		{
			WriteElement(n, new string(value, 1));
		}
		protected override void WriteElement(TCursor n, bool value)
		{
			WriteElement(n, value ? "true" : "false");
		}
		protected override void WriteElement(TCursor n, float value)
		{
			WriteElement(n, value.ToStringInvariant(this.SingleFormatSpecifier));
		}
		protected override void WriteElement(TCursor n, double value)
		{
			WriteElement(n, value.ToStringInvariant(this.DoubleFormatSpecifier));
		}

		protected override void WriteElement(TCursor n, byte value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, sbyte value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, ushort value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, short value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, uint value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, int value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, ulong value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		protected override void WriteElement(TCursor n, long value, NumeralBase toBase)
		{
			WriteElement(n, Numbers.ToString(value, toBase));
		}
		#endregion

		#region WriteAttribute
		public override void WriteAttribute(string name, string value)
		{
			CursorWriteAttribute(name, value);
		}
		public override void WriteAttribute(string name, char value)
		{
			CursorWriteAttribute(name, new string(value, 1));
		}
		public override void WriteAttribute(string name, bool value)
		{
			CursorWriteAttribute(name, value ? "true" : "false");
		}
		public override void WriteAttribute(string name, float value)
		{
			CursorWriteAttribute(name, value.ToStringInvariant(this.SingleFormatSpecifier));
		}
		public override void WriteAttribute(string name, double value)
		{
			CursorWriteAttribute(name, value.ToStringInvariant(this.DoubleFormatSpecifier));
		}

		public override void WriteAttribute(string name, byte value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, sbyte value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, ushort value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, short value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, uint value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, int value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, ulong value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		public override void WriteAttribute(string name, long value, NumeralBase toBase)
		{
			CursorWriteAttribute(name, Numbers.ToString(value, toBase));
		}
		#endregion
	};
}