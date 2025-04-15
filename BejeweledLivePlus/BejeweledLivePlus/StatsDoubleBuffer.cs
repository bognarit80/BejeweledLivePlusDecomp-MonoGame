using System;

namespace BejeweledLivePlus
{
	public class StatsDoubleBuffer
	{
		private StatsArray[] mBufferStats = new StatsArray[2];

		private uint m_index;

		public int this[int index]
		{
			get
			{
				return mBufferStats[m_index].mStats[index];
			}
			set
			{
				mBufferStats[m_index].mStats[index] = value;
			}
		}

		public StatsDoubleBuffer()
		{
			mBufferStats[0] = new StatsArray();
			mBufferStats[1] = new StatsArray();
			m_index = 0u;
		}

		public void Swap()
		{
			Swap(false);
		}

		public void Swap(bool swapContents)
		{
			if (swapContents)
			{
				uint num = ((m_index == 0) ? 1u : 0u);
				mBufferStats[num].CopyFrom(mBufferStats[m_index]);
			}
			m_index = ((m_index == 0) ? 1u : 0u);
		}

		public void Clear()
		{
			mBufferStats[m_index].Clear();
		}

		public bool Clear(uint i)
		{
			if (i < 2)
			{
				mBufferStats[i].Clear();
				return true;
			}
			return false;
		}

		public void ClearAll()
		{
			mBufferStats[0].Clear();
			mBufferStats[1].Clear();
		}

		public void CopyTo(int[] dstArr)
		{
			mBufferStats[m_index].CopyToArray(dstArr);
		}

		public void CopyFrom(int[] srcArr)
		{
			mBufferStats[m_index].CopyFromArray(srcArr);
		}

		public int MaxStat(int i)
		{
			return Math.Max(mBufferStats[0].mStats[i], mBufferStats[1].mStats[i]);
		}
	}
}
