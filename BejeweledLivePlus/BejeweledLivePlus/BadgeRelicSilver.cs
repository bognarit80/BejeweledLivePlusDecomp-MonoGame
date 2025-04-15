namespace BejeweledLivePlus
{
	public class BadgeRelicSilver : Badge
	{
		public BadgeRelicSilver()
		{
			mIdx = 4;
			mGPoints = 5;
			mUnlockRequirement = 8;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RELIC_HUNTER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_RELICHUNTER;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "Relic_Silver";
		}

		public override int GetStat()
		{
			return mBoard.mGameStats[36];
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Relic Hunter Bronze", 4016);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Collect 8 artifacts in Diamond Mine mode", 4056);
		}
	}
}
