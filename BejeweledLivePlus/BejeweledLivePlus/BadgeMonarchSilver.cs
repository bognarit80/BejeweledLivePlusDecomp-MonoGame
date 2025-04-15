namespace BejeweledLivePlus
{
	public class BadgeMonarchSilver : Badge
	{
		public BadgeMonarchSilver()
		{
			mIdx = 10;
			mGPoints = 5;
			mUnlockRequirement = 300000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BUTTERFLY_MONARCH;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BTF_MONARCH;
			mLevel = BadgeLevel.BADGELEVEL_SILVER;
			mName = "Monarch_Silver";
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
			return GlobalMembers._ID("Monarch Bronze", 4022);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 300 000 points in Butterflies", 4058);
		}
	}
}
