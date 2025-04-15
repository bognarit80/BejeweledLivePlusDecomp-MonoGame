namespace BejeweledLivePlus
{
	public class BadgeBronze : Badge
	{
		public BadgeBronze()
		{
			mIdx = 2;
			mGPoints = 5;
			mUnlockRequirement = 100000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_DIAMOND_MINE;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_DIAMONDMINE;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "Bronze";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is DigBoard)
			{
				result = ((DigBoard)mBoard).mLevelPointsTotal;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Mine Bronze", 4014);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 100 000 points in Diamond Mine mode", 4054);
		}
	}
}
