namespace BejeweledLivePlus
{
	public class BadgePlatinum : Badge
	{
		public BadgePlatinum()
		{
			mIdx = 3;
			mGPoints = 10;
			mUnlockRequirement = 750000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_DIAMOND_MINE;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_DIAMONDMINE;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "Platinum";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is DigBoard)
			{
				result = ((DigBoard)mBoard).mPoints;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Mine Platinum", 4015);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 750 000 points in Diamond Mine mode", 4055);
		}
	}
}
