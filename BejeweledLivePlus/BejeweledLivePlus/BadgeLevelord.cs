namespace BejeweledLivePlus
{
	public class BadgeLevelord : Badge
	{
		public BadgeLevelord()
		{
			mIdx = 0;
			mGPoints = 10;
			mUnlockRequirement = 10;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_LEVELORD;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_LEVELORD;
			mLevel = BadgeLevel.BADGELEVEL_BRONZE;
			mName = "Levelord";
		}

		public override int GetStat()
		{
			int result = 0;
			if (mBoard is ClassicBoard)
			{
				result = ((ClassicBoard)mBoard).mLevel + 1;
			}
			return result;
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Levelord", 4012);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Reach level 10 in classic mode", 4052);
		}
	}
}
