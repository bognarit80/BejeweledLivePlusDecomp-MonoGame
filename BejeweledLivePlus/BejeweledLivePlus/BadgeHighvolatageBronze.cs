namespace BejeweledLivePlus
{
	public class BadgeHighvolatageBronze : Badge
	{
		public BadgeHighvolatageBronze()
		{
			mIdx = 8;
			mGPoints = 5;
			mUnlockRequirement = 100000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_HIGH_VOLTAGE;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_HIGHVOLTAGE;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "High_volatage_Bronze";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is SpeedBoard)
			{
				result = ((SpeedBoard)mBoard).mPoints;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("High voltage Bronze", 4020);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 100 000 points in Lightning mode", 4070);
		}
	}
}
