namespace BejeweledLivePlus
{
	public class BadgeBejGold : Badge
	{
		public BadgeBejGold()
		{
			mIdx = 1;
			mGPoints = 10;
			mUnlockRequirement = 300000;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_BEJEWELER;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_BEJEWELER;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Bejeweler_Gold";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is ClassicBoard)
			{
				result = ((ClassicBoard)mBoard).mPoints;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Bejeweler Gold", 4013);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Score 300 000 points in classic mode", 4053);
		}
	}
}
