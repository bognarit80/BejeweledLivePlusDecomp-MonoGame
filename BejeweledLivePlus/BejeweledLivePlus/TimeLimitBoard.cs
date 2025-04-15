using BejeweledLivePlus.Bej3Graphics;
using SexyFramework;

namespace BejeweledLivePlus
{
	public class TimeLimitBoard : QuestBoard
	{
		public int mTimeLimit;

		public int mTimeBonus;

		public int mTimePenalty;

		public TimeLimitBoard()
		{
			mTimeBonus = 10;
			mTimePenalty = 3000;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void LoadContent(bool threaded)
		{
			base.LoadContent(threaded);
			BejeweledLivePlusApp.LoadContent("GamePlayQuest_TimeLimit");
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			BejeweledLivePlusApp.UnloadContent("GamePlayQuest_TimeLimit");
		}

		public override void NewGame()
		{
			NewGame(false);
		}

		public override void NewGame(bool restartingGame)
		{
			if (mParams.ContainsKey("Time"))
			{
				mTimeLimit = SexyFramework.GlobalMembers.sexyatoi(mParams["Time"]);
			}
			if (!mParams.ContainsKey("TimeBonus") || !int.TryParse(mParams["TimeBonus"], out mTimeBonus))
			{
				mTimeBonus = 0;
			}
			if (!mParams.ContainsKey("TimePenalty") || !int.TryParse(mParams["TimePenalty"], out mTimePenalty))
			{
				mTimePenalty = 0;
			}
			base.NewGame(restartingGame);
		}

		public override int GetTimeLimit()
		{
			return mTimeLimit;
		}

		public override bool WantsHideOnPause()
		{
			return true;
		}

		public override void DoQuestBonus()
		{
			DoQuestBonus(1f);
		}

		public override void DoQuestBonus(float iOpt_multiplier)
		{
			int num = (int)((float)mTimeBonus * iOpt_multiplier);
			mTimeLimit += num;
			TextNotifyEffect textNotifyEffect = ShowQuestText(string.Format(GlobalMembers._ID("+{0} SECOND BONUS", 499), num));
			textNotifyEffect.mFont = GlobalMembersResources.FONT_HEADER;
			textNotifyEffect.mY = GlobalMembers.M(1050);
		}

		public override void DoQuestPenalty()
		{
			DoQuestPenalty(1f);
		}

		public override void DoQuestPenalty(float iOpt_multiplier)
		{
			int num = (int)((float)mTimePenalty * iOpt_multiplier);
			mTimeLimit -= num;
			ShowQuestText(string.Format(GlobalMembers._ID("-{0} SECONDS PENALTY", 500), num));
		}

		public override void KeyChar(char theChar)
		{
		}
	}
}
