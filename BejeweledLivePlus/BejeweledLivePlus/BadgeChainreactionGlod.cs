namespace BejeweledLivePlus
{
	public class BadgeChainreactionGlod : Badge
	{
		public BadgeChainreactionGlod()
		{
			mIdx = 18;
			mGPoints = 15;
			mUnlockRequirement = 7;
			mUnlocked = GlobalMembers.gApp.mProfile.mBadgeStatus[mIdx];
			mSmallIcon = GlobalMembersResourcesWP.IMAGE_BADGES_SMALL_CHAIN_REACTION;
			mSmallIconGray = GlobalMembersResourcesWP.IMAGE_BADGES_GRAY_ICON_CHAINREACTION;
			mLevel = BadgeLevel.BADGELEVEL_GOLD;
			mName = "Chain_reaction_Gold";
		}

		public override int GetStat()
		{
			return GlobalMembers.gApp.mProfile.mStats.MaxStat(38);
		}

		public override bool WantsMidGameCalc()
		{
			return true;
		}

		public override string GetTooltipHeader()
		{
			return GlobalMembers._ID("Chain reaction", 4030);
		}

		public override string GetTooltipBody()
		{
			return GlobalMembers._ID("Detonate 7 special gems in a single move", 4066);
		}
	}
}
