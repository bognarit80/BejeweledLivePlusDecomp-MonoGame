namespace BejeweledLivePlus
{
	public class BadgeBlasterGold : Badge
	{
		public BadgeBlasterGold()
		{
			mIdx = 16;
			mGPoints = 15;
			mUnlockRequirement = 60;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BLASTER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BLASTER;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Blaster_Gold";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mStats.MaxStat(33);
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Blaster", 4028);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Destroy 60 gems in 1 move", 4064);
		}
	}
}
