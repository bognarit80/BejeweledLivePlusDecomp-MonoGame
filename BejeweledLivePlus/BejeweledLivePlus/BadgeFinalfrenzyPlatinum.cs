namespace BejeweledLivePlus
{
	public class BadgeFinalfrenzyPlatinum : Badge
	{
		public BadgeFinalfrenzyPlatinum()
		{
			mIdx = 7;
			mGPoints = 10;
			mUnlockRequirement = 60000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_ELECTRIFIER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_ELECTRIFIER;
			mLevel = BadgeLevel.BADGELEVEL_PLATINUM;
			mName = "Final_frenzy_Platinum";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is SpeedBoard)
			{
				result = GlobalMembers.gApp.mBoard.mPoints - ((SpeedBoard)mBoard).mPreHurrahPoints;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return false;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Final frenzy Platinum", 4019);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score over 60 000 points during a Last Hurrah in Lightning mode", 4069);
		}
	}
}
