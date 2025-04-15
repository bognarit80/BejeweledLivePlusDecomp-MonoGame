using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;

namespace BejeweledLivePlus
{
	public class BadgeManager
	{
		private static BadgeManager instance = null;

		private static object mAchievementsLockObject = new object();

		public Badge mBadge;

		public Badge[] mBadgeClass = new Badge[20];

		public Board mBoard;

		public static BadgeManager GetBadgeManagerInstance()
		{
			if (instance == null)
			{
				instance = new BadgeManager();
			}
			return instance;
		}

		public void LinkBoard(Board board)
		{
			mBoard = board;
			for (int i = 0; i < 20; i++)
			{
				if (mBadgeClass[i] != null)
				{
					mBadgeClass[i].mBoard = mBoard;
				}
			}
		}

		public BadgeManager()
		{
			ConfigBadges();
			SyncBadges();
			GetAchievementsFromXBLA();
		}

		public void Dispose()
		{
			for (int i = 0; i < 20; i++)
			{
				if (mBadgeClass[i] != null)
				{
					mBadgeClass[i].Dispose();
					mBadgeClass[i] = null;
				}
			}
		}

		public void SetBadge(int theIdx)
		{
			mBadge = mBadgeClass[theIdx];
		}

		public Badge GetBadge()
		{
			return mBadge;
		}

		public Badge GetBadgeByIndex(int theIdx)
		{
			return mBadgeClass[theIdx];
		}

		public Badge GetBadgeByType(Badges theType)
		{
			return GetBadgeByIndex((int)theType);
		}

		public void SyncBadges()
		{
			for (int i = 0; i < 20; i++)
			{
				if (mBadgeClass[i] != null)
				{
					mBadgeClass[i].mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[i];
				}
			}
			SyncAchievementsWithXBLA();
		}

		public void Update()
		{
			Badge[] array = mBadgeClass;
			foreach (Badge badge in array)
			{
				if (badge.CanUnlock() && !badge.mPending)
				{
					AwardAchievement(badge.mName);
					badge.mPending = true;
				}
				if (badge.mUnlocked)
				{
					badge.mPending = false;
				}
			}
		}

		public void AwardAchievement(string aName)
		{
			// SignedInGamer signedInGamer = Gamer.SignedInGamers[PlayerIndex.One];
			// if (signedInGamer == null)
			// {
			// 	return;
			// }
			// try
			// {
			// 	signedInGamer.BeginAwardAchievement(aName, AwardAchievementCallback, signedInGamer);
			// }
			// catch (GameUpdateRequiredException ex)
			// {
			// 	throw ex;
			// }
			// catch (Exception)
			// {
			// }
		}

		protected void AwardAchievementCallback(IAsyncResult result)
		{
			lock (mAchievementsLockObject)
			{
				try
				{
					// (result.AsyncState as SignedInGamer)?.EndAwardAchievement(result);
				}
				catch (Exception)
				{
				}
			}
		}

		public void SyncAchievementsWithXBLA()
		{
			// if (GlobalMembers.g_AchievementsXLive == null || GlobalMembers.g_AchievementsXLive.Count <= 0)
			// {
			// 	return;
			// }
			// List<string> list = new List<string>();
			// for (int i = 0; i < GlobalMembers.g_AchievementsXLive.Count; i++)
			// {
			// 	Achievement achievement = GlobalMembers.g_AchievementsXLive[i];
			// 	Badge badge = null;
			// 	Badge[] array = mBadgeClass;
			// 	foreach (Badge badge2 in array)
			// 	{
			// 		if (badge2.mName == achievement.Key)
			// 		{
			// 			badge = badge2;
			// 			break;
			// 		}
			// 	}
			// 	if (badge != null)
			// 	{
			// 		if (achievement.IsEarned)
			// 		{
			// 			badge.mUnlocked = true;
			// 		}
			// 		else if (badge.mUnlocked)
			// 		{
			// 			list.Add(badge.mName);
			// 		}
			// 	}
			// }
			// foreach (string item in list)
			// {
			// 	AwardAchievement(item);
			// }
			// for (int k = 0; k < 20; k++)
			// {
			// 	if (mBadgeClass[k] != null)
			// 	{
			// 		GlobalMembers.gApp.mProfile.mBadgeStatus[k] = mBadgeClass[k].mUnlocked;
			// 	}
			// }
		}

		public void GetAchievementsFromXBLA()
		{
			// SignedInGamer.SignedIn += GamerSignedInCallback;
		}

		// protected void GamerSignedInCallback(object sender, SignedInEventArgs args)
		// {
		// 	try
		// 	{
		// 		SignedInGamer gamer = args.Gamer;
		// 		gamer?.BeginGetAchievements(GetAchievementsCallback, gamer);
		// 	}
		// 	catch (Exception)
		// 	{
		// 	}
		// }

		protected void GetAchievementsCallback(IAsyncResult result)
		{
			// SignedInGamer signedInGamer = result.AsyncState as SignedInGamer;
			// if (signedInGamer == null)
			// {
			// 	return;
			// }
			// lock (mAchievementsLockObject)
			// {
			// 	try
			// 	{
			// 		GlobalMembers.g_AchievementsXLive = signedInGamer.EndGetAchievements(result);
			// 		SyncAchievementsWithXBLA();
			// 	}
			// 	catch (Exception)
			// 	{
			// 	}
			// }
		}

		private void ConfigBadges()
		{
			mBadgeClass[0] = new BadgeLevelord();
			mBadgeClass[1] = new BadgeBejGold();
			mBadgeClass[2] = new BadgeBronze();
			mBadgeClass[3] = new BadgePlatinum();
			mBadgeClass[4] = new BadgeRelicSilver();
			mBadgeClass[5] = new BadgeRelicPlatinum();
			mBadgeClass[6] = new BadgeFinalfrenzyBronze();
			mBadgeClass[7] = new BadgeFinalfrenzyPlatinum();
			mBadgeClass[8] = new BadgeHighvolatageBronze();
			mBadgeClass[9] = new BadgeHighvolatagePlatinum();
			mBadgeClass[10] = new BadgeMonarchSilver();
			mBadgeClass[11] = new BadgeMonarchPlatinum();
			mBadgeClass[12] = new BadgeBonanzaBronze();
			mBadgeClass[13] = new BadgeBonanzaPlatinum();
			mBadgeClass[14] = new BadgeChromaticGold();
			mBadgeClass[15] = new BadgeStellarGold();
			mBadgeClass[16] = new BadgeBlasterGold();
			mBadgeClass[17] = new BadgeSuperStar();
			mBadgeClass[18] = new BadgeChainreactionGlod();
			mBadgeClass[19] = new BadgeLuckyStreakGold();
		}
	}
}
