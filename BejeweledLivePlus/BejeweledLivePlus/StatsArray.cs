namespace BejeweledLivePlus
{
	public class StatsArray
	{
		public int[] mStats = new int[40];

		public StatsArray()
		{
			Clear();
		}

		public StatsArray(StatsArray obj)
		{
			for (int i = 0; i < mStats.Length; i++)
			{
				mStats[i] = obj.mStats[i];
			}
		}

		public void Clear()
		{
			for (int i = 0; i < mStats.Length; i++)
			{
				mStats[i] = 0;
			}
		}

		public void CopyFrom(StatsArray rhs)
		{
			for (int i = 0; i < mStats.Length; i++)
			{
				mStats[i] = rhs.mStats[i];
			}
		}

		public void CopyToArray(int[] dstArr)
		{
			if (dstArr != null)
			{
				for (int i = 0; i < mStats.Length; i++)
				{
					dstArr[i] = mStats[i];
				}
			}
		}

		public void CopyFromArray(int[] srcArr)
		{
			if (srcArr != null)
			{
				for (int i = 0; i < mStats.Length; i++)
				{
					mStats[i] = srcArr[i];
				}
			}
		}
	}
}
