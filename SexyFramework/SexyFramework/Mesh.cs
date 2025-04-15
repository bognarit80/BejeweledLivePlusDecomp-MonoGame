using System;
using System.Collections.Generic;

namespace SexyFramework
{
	public class Mesh : IDisposable
	{
		public string mFileName;

		public MeshListener mListener;

		public object mUserData;

		public List<MeshPiece> mPieces = new List<MeshPiece>();

		public Mesh()
		{
			mListener = null;
			mUserData = null;
			GlobalMembers.gSexyAppBase.mGraphicsDriver.AddMesh(this);
		}

		public void Dispose()
		{
			if (mListener != null)
			{
				mListener.MeshPreDeleted(this);
			}
			Cleanup();
		}

		public virtual void Cleanup()
		{
			List<MeshPiece>.Enumerator enumerator = mPieces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current?.Dispose();
			}
			mPieces.Clear();
		}

		public virtual void SetListener(MeshListener theListener)
		{
			mListener = theListener;
		}
	}
}
