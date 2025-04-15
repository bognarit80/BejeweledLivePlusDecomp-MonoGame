using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class Last3MatchScoreManager
	{
		private int[] mScoreHistory = new int[3];

		private int mMatchCount;

		public Last3MatchScoreManager()
		{
			mMatchCount = 0;
			for (int i = 0; i < 3; i++)
			{
				mScoreHistory[i] = 0;
			}
		}

		public void Update(int iLatestScore)
		{
			mScoreHistory[mMatchCount] = iLatestScore;
			if (mMatchCount < 2)
			{
				mMatchCount++;
			}
			else
			{
				mMatchCount = 0;
			}
		}

		public int GetLowerScore()
		{
			int num = mScoreHistory[0];
			for (int i = 1; i < 3; i++)
			{
				if (mScoreHistory[i] < num)
				{
					num = mScoreHistory[i];
				}
			}
			return num;
		}

		public void Clear()
		{
			mMatchCount = 0;
			for (int i = 0; i < 3; i++)
			{
				mScoreHistory[i] = 0;
			}
		}

		public bool Save(Buffer theBuffer)
		{
			int num = mScoreHistory.Length;
			theBuffer.WriteInt32(mMatchCount);
			theBuffer.WriteInt32(num);
			for (int i = 0; i < num; i++)
			{
				theBuffer.WriteInt32(mScoreHistory[i]);
			}
			return true;
		}

		public bool Load(Buffer theBuffer)
		{
			mMatchCount = theBuffer.ReadInt32();
			int num = theBuffer.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				mScoreHistory[i] = theBuffer.ReadInt32();
			}
			return true;
		}
	}
}
