namespace BejeweledLivePlus
{
	public class BadgeHighvolatagePlatinum : Badge
	{
		public BadgeHighvolatagePlatinum()
		{
			mIdx = 9;
			mGPoints = 10;
			mUnlockRequirement = 750000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_HIGH_VOLTAGE;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_HIGHVOLTAGE;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "High_voltage_Platinum";
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
			return GlobalMembers._ID("High voltage Platinum", 4021);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 750 000 points in Lightning mode", 4071);
		}
	}
}
