using System.Collections.Generic;
using SexyFramework.Drivers.Leaderboard;

namespace SexyFramework
{
	public abstract class LeaderboardReadContext : IAsyncTask
	{
		protected uint mStartRank;

		protected uint mNumEntries;

		protected uint mTotalNumEntries;

		protected List<LeaderboardEntry> mData = new List<LeaderboardEntry>();

		public override void Dispose()
		{
			base.Dispose();
		}

		public uint GetStartRow()
		{
			return mStartRank;
		}

		public uint GetNumRows()
		{
			return mNumEntries;
		}

		public uint GetTotalNumRows()
		{
			return mTotalNumEntries;
		}

		public virtual int GetUserRow()
		{
			return -1;
		}

		public LeaderboardEntry GetRow(int index)
		{
			return mData[index];
		}
	}
}
