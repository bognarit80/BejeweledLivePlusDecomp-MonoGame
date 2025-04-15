using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;
using SexyFramework.Drivers.Leaderboard;

namespace BejeweledLivePlus
{
	public class HighScoreTable
	{
		public enum HighScoreTableTime
		{
			TIME_RECENT,
			TIME_ALLTIME
		}

		public enum LRState
		{
			LR_Idle,
			LR_Loading,
			LR_Ready,
			LR_Error
		}

		public List<HighScoreEntryLive> mHighScoresLive = new List<HighScoreEntryLive>();

		private string mModeKey = string.Empty;

		public HighScoreMgr mManager;

		public bool CanPageUp;

		public bool CanPageDown;

		private object mLeaderBoardReadLock = new object();

		public LRState mLRState;

		// protected LeaderboardReader leaderboardReader;

		public int mMode { get; set; }

		public HighScoreTable(string modeKey)
		{
			mHighScoresLive = new List<HighScoreEntryLive>();
			mModeKey = modeKey;
			mMode = GlobalMembers.gApp.ModeHeadingToGameMode(mModeKey);
		}

		public bool Submit(string theName, int theValue, int thePicture)
		{
			SubmitHighScoreToXBLA(theValue);
			return false;
		}

		public void SubmitHighScoreToXBLA(int theScore)
		{
			try
			{
				// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
				// LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(LeaderboardKey.BestScoreRecent, mMode);
				// LeaderboardEntry leaderboard = signedInGamer.LeaderboardWriter.GetLeaderboard(leaderboardId);
				// leaderboard.Rating = theScore;
				// leaderboard.Columns.SetValue("TimeStamp", DateTime.Now);
				// leaderboard.Columns.SetValue("BestScore", theScore);
			}
			catch (Exception)
			{
			}
		}

		public void ReadLeaderboard(HighScoreTableTime t)
		{
			try
			{
				// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
				// LeaderboardKey key = ((t == HighScoreTableTime.TIME_RECENT) ? LeaderboardKey.BestScoreRecent : LeaderboardKey.BestScoreLifeTime);
				// LeaderboardIdentity leaderboardId = LeaderboardIdentity.Create(key, mMode);
				// LeaderboardReader.BeginRead(leaderboardId, signedInGamer, 10, LeaderboardReadCallback, signedInGamer);
				mLRState = LRState.LR_Loading;
				GlobalMembers.isLeaderboardLoading = true;
			}
			catch (Exception)
			{
				mLRState = LRState.LR_Error;
				GlobalMembers.isLeaderboardLoading = false;
			}
		}

		protected void LeaderboardReadCallback(IAsyncResult result)
		{
			lock (mLeaderBoardReadLock)
			{
				// SignedInGamer signedInGamer = result.AsyncState as SignedInGamer;
				// if (signedInGamer != null)
				// {
				// 	try
				// 	{
				// 		leaderboardReader = LeaderboardReader.EndRead(result);
				// 		CanPageUp = leaderboardReader.CanPageUp;
				// 		CanPageDown = leaderboardReader.CanPageDown;
				// 		CreateRankList();
				// 		mLRState = LRState.LR_Ready;
				// 	}
				// 	catch (Exception)
				// 	{
				// 		mLRState = LRState.LR_Error;
				// 	}
				// }
				// else
				// {
					mLRState = LRState.LR_Error;
				// }
				GlobalMembers.isLeaderboardLoading = false;
			}
		}

		protected void CreateRankList()
		{
			// mHighScoresLive.Clear();
			// LeaderboardReader leaderboardReader = this.leaderboardReader;
			// int count = leaderboardReader.Entries.Count;
			// for (int i = 0; i < count; i++)
			// {
			// 	LeaderboardEntry liveEntry = leaderboardReader.Entries[i];
			// 	HighScoreEntryLive highScoreEntryLive = new HighScoreEntryLive();
			// 	highScoreEntryLive.Init(liveEntry);
			// 	mHighScoresLive.Add(highScoreEntryLive);
			// }
		}
	}
}
