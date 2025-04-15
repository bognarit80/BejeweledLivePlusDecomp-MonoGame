namespace BejeweledLivePlus
{
	public class BadgeChromaticGold : Badge
	{
		public BadgeChromaticGold()
		{
			mIdx = 14;
			mGPoints = 20;
			mUnlockRequirement = 400;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_CHROMATIC;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_CHROMATIC;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Chromatic_Gold";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mStats.MaxStat(14);
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Chromatic", 4026);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Clear 400 hypercubes", 4062);
		}
	}
}
