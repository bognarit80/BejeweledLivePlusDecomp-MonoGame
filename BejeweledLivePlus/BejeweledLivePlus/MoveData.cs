using BejeweledLivePlus.Misc;

namespace BejeweledLivePlus
{
	public class MoveData
	{
		public int mUpdateCnt;

		public int mSelectedId;

		public int mSwappedRow;

		public int mSwappedCol;

		public Serialiser mPreSaveBuffer = new Serialiser();

		public bool mPartOfReplay;

		public int mMoveCreditId;

		public int[] mStats = new int[40];

		public MoveData()
		{
			mUpdateCnt = 0;
			mSelectedId = -1;
			mSwappedRow = 0;
			mSwappedCol = 0;
			mPartOfReplay = false;
		}

		public void CopyFrom(MoveData data)
		{
			mUpdateCnt = data.mUpdateCnt;
			mSelectedId = data.mSelectedId;
			mSwappedRow = data.mSwappedRow;
			mSwappedCol = data.mSwappedCol;
			mPartOfReplay = data.mPartOfReplay;
			mMoveCreditId = data.mMoveCreditId;
			mPreSaveBuffer.Copyfrom(data.mPreSaveBuffer);
			for (int i = 0; i < 40; i++)
			{
				mStats[i] = data.mStats[i];
			}
		}
	}
}
