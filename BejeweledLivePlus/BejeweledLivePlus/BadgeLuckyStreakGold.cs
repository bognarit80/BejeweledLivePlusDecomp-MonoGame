namespace BejeweledLivePlus
{
	public class BadgeLuckyStreakGold : Badge
	{
		public BadgeLuckyStreakGold()
		{
			mIdx = 19;
			mGPoints = 10;
			mUnlockRequirement = 250000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_LUCKY_STREAK;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_LUCKYSTREAK;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Lucky_Streak_Gold";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mLast3MatchScoreManager.GetLowerScore();
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Lucky Streak", 4031);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Finish 3 games in a row over 250 000 points", 4067);
		}
	}
}
