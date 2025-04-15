namespace BejeweledLivePlus
{
	public class BadgeBonanzaBronze : Badge
	{
		public BadgeBonanzaBronze()
		{
			mIdx = 12;
			mGPoints = 5;
			mUnlockRequirement = 4;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BUTTERFLY_BONANZA;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BTF_BONANZA;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "Bonanza_bronze";
		}

		public override int GetStat()
		{
			return mBoard.mGameStats[29];
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Bonanza Bronze", 4024);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Collect 4 butterflies in one move in Butterfly", 4060);
		}
	}
}
