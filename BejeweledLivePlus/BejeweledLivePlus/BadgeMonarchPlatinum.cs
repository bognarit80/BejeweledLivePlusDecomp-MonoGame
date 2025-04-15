namespace BejeweledLivePlus
{
	public class BadgeMonarchPlatinum : Badge
	{
		public BadgeMonarchPlatinum()
		{
			mIdx = 11;
			mGPoints = 10;
			mUnlockRequirement = 750000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BUTTERFLY_MONARCH;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BTF_MONARCH;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "Monarch_Platinum";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is ButterflyBoard)
			{
				result = ((ButterflyBoard)mBoard).mPoints;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Monarch Platinum", 4023);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 750 000 points in Butterflies", 4059);
		}
	}
}
