namespace BejeweledLivePlus
{
	public class BadgeStellarGold : Badge
	{
		public BadgeStellarGold()
		{
			mIdx = 15;
			mGPoints = 20;
			mUnlockRequirement = 400;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_STELLAR;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_STELLAR;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Stellar_Gold";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mStats.MaxStat(13);
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Chromatic", 4027);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Clear 400 star gems", 4063);
		}
	}
}
