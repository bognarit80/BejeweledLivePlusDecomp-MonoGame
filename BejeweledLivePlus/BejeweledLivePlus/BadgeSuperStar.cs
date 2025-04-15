namespace BejeweledLivePlus
{
	public class BadgeSuperStar : Badge
	{
		public BadgeSuperStar()
		{
			mIdx = 17;
			mGPoints = 10;
			mUnlockRequirement = 1;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_SUPERSTAR;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_SUPERSTAR;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Superstar";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mStats.MaxStat(30);
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Superstar", 4029);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Line up 6 gems in a row", 4065);
		}
	}
}
