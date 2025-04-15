namespace BejeweledLivePlus
{
	public class BadgeFinalfrenzyBronze : Badge
	{
		public BadgeFinalfrenzyBronze()
		{
			mIdx = 6;
			mGPoints = 5;
			mUnlockRequirement = 20000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_ELECTRIFIER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_ELECTRIFIER;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "Final_frenzy_Bronze";
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
			return GlobalMembers._ID("Final frenzy Bronze", 4018);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score over 20 000 points during a Last Hurrah in Lightning mode", 4068);
		}
	}
}
