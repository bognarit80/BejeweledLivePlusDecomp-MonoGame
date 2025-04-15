namespace BejeweledLivePlus
{
	public class BadgeRelicPlatinum : Badge
	{
		public BadgeRelicPlatinum()
		{
			mIdx = 5;
			mGPoints = 10;
			mUnlockRequirement = 15;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_RELIC_HUNTER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_RELICHUNTER;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "Relic_Platinum";
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
			return GlobalMembers._ID("Relic Hunter Platinum", 4017);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Collect 15 artifacts in Diamond Mine mode", 4057);
		}
	}
}
