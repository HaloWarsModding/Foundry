﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.IO
{
	/// <summary>Temporarily bookmarks a stream's <see cref="IKSoftStream.Owner"/></summary>
	[SuppressMessage("Microsoft.Design", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
	public struct IKSoftStreamOwnerBookmark : IDisposable
	{
		IKSoftStream mStream;
		readonly object mOldOwner;

		/// <summary>Saves the stream's owner so a new one can be specified, but is then later restored to the previous owner, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="newOwner"></param>
		public IKSoftStreamOwnerBookmark(IKSoftStream stream, object newOwner)
		{
			Contract.Requires(stream != null);

			mOldOwner = (mStream = stream).Owner;
			mStream.Owner = newOwner;
		}

		/// <summary>Returns the owner of the underlying stream to the previous owner</summary>
		public void Dispose()
		{
			if (mStream != null)
			{
				mStream.Owner = mOldOwner;
				mStream = null;
			}
		}
	};

	/// <summary>Temporarily bookmarks a stream's <see cref="IKSoftStream.UserData"/></summary>
	[SuppressMessage("Microsoft.Design", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
	public struct IKSoftStreamUserDataBookmark : IDisposable
	{
		IKSoftStream mStream;
		readonly object mOldUserData;

		/// <summary>Saves the stream's UserData so a new one can be specified, but is then later restored to the previous UserData, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="newUserData"></param>
		public IKSoftStreamUserDataBookmark(IKSoftStream stream, object newUserData)
		{
			Contract.Requires(stream != null);

			mOldUserData = (mStream = stream).UserData;
			mStream.UserData = newUserData;
		}

		/// <summary>Returns the UserData of the underlying stream to the previous UserData</summary>
		public void Dispose()
		{
			if (mStream != null)
			{
				mStream.UserData = mOldUserData;
				mStream = null;
			}
		}
	};

	/// <summary>Temporarily bookmarks a stream's <see cref="IKSoftStreamModeable.StreamMode"/></summary>
	[SuppressMessage("Microsoft.Design", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
	public struct IKSoftStreamModeBookmark : IDisposable
	{
		IKSoftStreamModeable mStream;
		readonly FileAccess mOldMode;

		/// <summary>Saves the stream's StreamMode so a new one can be specified, but is then later restored to the previous StreamMode, via <see cref="Dispose()"/></summary>
		/// <param name="stream">The underlying stream for this bookmark</param>
		/// <param name="newMode"></param>
		public IKSoftStreamModeBookmark(IKSoftStreamModeable stream, FileAccess newMode)
		{
			Contract.Requires(stream != null);
			Contract.Requires(newMode != 0, "New mode is unset!");

			mOldMode = (mStream = stream).StreamMode;
			mStream.StreamMode = newMode;

			if (mOldMode == newMode)
				mStream = null;
		}

		/// <summary>Returns the StreamMode of the underlying stream to the previous mode</summary>
		public void Dispose()
		{
			if (mStream != null)
			{
				mStream.StreamMode = mOldMode;
				mStream = null;
			}
		}
	};
}
