namespace BejeweledLivePlus
{
	public class BadgeBonanzaPlatinum : Badge
	{
		public BadgeBonanzaPlatinum()
		{
			mIdx = 13;
			mGPoints = 10;
			mUnlockRequirement = 10;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BUTTERFLY_BONANZA;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BTF_BONANZA;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "Bonanza_Platinum";
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
			return GlobalMembers._ID("Bonanza Platinum", 4025);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Collect 10 butterflies in one move in Butterfly", 4061);
		}
	}
}
