﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Collections
{
	using StringSegmentEnumerator = StringSegment.Enumerator;

	// #TODO how is this better or different compared to StringSegment Microsoft.Extensions.Primitives.dll?
	[SuppressMessage("Microsoft.Design", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public partial struct StringSegment
		: IReadOnlyList<char>
		, IEquatable<StringSegment>
		, IList<char>
	{
		readonly string mData;
		public string Data { get { return mData; } }
		readonly int mOffset;
		public int Offset { get { return mOffset; } }
		readonly int mCount;
		public int Count { get { return mCount; } }

		#region Ctor
		public StringSegment(string data)
		{
			Contract.Requires<ArgumentNullException>(data != null);
			mData = data;
			mOffset = 0;
			mCount = data.Length;
		}
		public StringSegment(string data, int offset, int count)
		{
			Contract.Requires<ArgumentNullException>(data != null);
			Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
			Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
			Contract.Requires<ArgumentException>(count < (data.Length - offset));

			mData = data;
			mOffset = offset;
			mCount = count;
		}
		#endregion

		void VerifyData()
		{
			if (mData == null)
				throw new InvalidOperationException("String data is null");
		}

		public int IndexOf(char value)
		{
			VerifyData();

			int index = mData.IndexOf(value, mOffset, mCount);
			if (index < 0)
				return TypeExtensions.kNone;

			return index - mOffset;
		}

		public bool Contains(char value)
		{
			VerifyData();

			return mData.IndexOf(value, mOffset, mCount) >= 0;
		}

		public void CopyTo(char[] array, int arrayIndex)
		{
			VerifyData();

			mData.CopyTo(mOffset, array, arrayIndex, mCount);
		}

		#region IReadOnlyList<char> Members
		public char this[int index] { get {
			VerifyData();

			return mData[mOffset + index];
		} }

		char IList<char>.this[int index] {
			get { return this[index]; }
			set { throw new NotImplementedException(); }
		}
		#endregion

		#region IEnumerable<char> Members
		public StringSegmentEnumerator GetEnumerator()
		{
			VerifyData();

			return new StringSegmentEnumerator(this);
		}
		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{ return GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{ return GetEnumerator(); }
		#endregion

		#region NotImplemented IList<char> Members
		void IList<char>.Insert(int index, char item) { throw new NotImplementedException(); }
		void IList<char>.RemoveAt(int index) { throw new NotImplementedException(); }
		#endregion

		#region NotImplemented ICollection<char> Members
		void ICollection<char>.Add(char item) { throw new NotImplementedException(); }
		void ICollection<char>.Clear() { throw new NotImplementedException(); }
		bool ICollection<char>.IsReadOnly { get { return true; } }
		bool ICollection<char>.Remove(char item) { throw new NotImplementedException(); }
		#endregion

		#region Equatable Members
		public bool Equals(StringSegment other)
		{
			return other.mData == mData && other.mOffset == mOffset && other.mCount == mCount;
		}
		public override bool Equals(object obj)
		{
			return obj is StringSegment && Equals((StringSegment)obj);
		}

		public static bool operator ==(StringSegment lhs, StringSegment rhs)
		{
			return lhs.Equals(rhs);
		}
		public static bool operator !=(StringSegment lhs, StringSegment rhs)
		{
			return !lhs.Equals(rhs);
		}
		#endregion

		public override int GetHashCode()
		{
			if (mData != null)
				return (mData.GetHashCode() ^ mOffset) ^ mCount;

			return 0;
		}
	};
}
