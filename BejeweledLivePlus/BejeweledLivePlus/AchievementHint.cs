using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;

namespace BejeweledLivePlus
{
	public class AchievementHint
	{
		private enum HintStage
		{
			FadeIn,
			Display,
			FadeOut,
			HoldToRemove
		}

		private HintStage mStage;

		private Color mColor;

		private string mText;

		private int mElapsed;

		private event AchievementHintFinishedHandler OnFinished;

		public AchievementHint(string achievementName, AchievementHintFinishedHandler handler)
		{
			mStage = HintStage.FadeIn;
			mText = GlobalMembers._ID("You have earned the", 3488) + " " + achievementName;
			mColor = new Color(255, 255, 255, 0);
			OnFinished += handler;
			mElapsed = 0;
		}

		public void Update()
		{
			switch (mStage)
			{
			case HintStage.FadeIn:
				if (mElapsed >= 1000)
				{
					mStage = HintStage.Display;
					mElapsed = 0;
					break;
				}
				mElapsed += GlobalMembers.gApp.ElapsedTime;
				mColor.mAlpha = (int)(255f * ((float)mElapsed / 1000f));
				if (mColor.mAlpha > 255)
				{
					mColor.mAlpha = 255;
				}
				break;
			case HintStage.FadeOut:
				if (mElapsed >= 1000)
				{
					mStage = HintStage.HoldToRemove;
					mElapsed = 0;
					break;
				}
				mElapsed += GlobalMembers.gApp.ElapsedTime;
				mColor.mAlpha = (int)(255f * ((float)(1000 - mElapsed) / 1000f));
				if (mColor.mAlpha < 0)
				{
					mColor.mAlpha = 0;
				}
				break;
			case HintStage.Display:
				mColor.mAlpha = 255;
				if (mElapsed > 7000)
				{
					mElapsed = 0;
					mStage = HintStage.FadeOut;
				}
				else
				{
					mElapsed += GlobalMembers.gApp.ElapsedTime;
				}
				break;
			case HintStage.HoldToRemove:
				if (mElapsed > 3000)
				{
					this.OnFinished(this);
				}
				else
				{
					mElapsed += GlobalMembers.gApp.ElapsedTime;
				}
				break;
			}
		}

		public void Draw(Graphics g)
		{
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			int num = (int)ConstantsWP.DEVICE_WIDTH_F / 2;
			int num2 = ConstantsWP.BOARD_UI_HINT_BTN_Y + ConstantsWP.DIALOGBOX_BUTTON_MEASURE_HEIGHT + ConstantsWP.DIALOGBOX_EXTRA_HEIGHT - g.GetFont().GetAscent() / 2;
			Utils.SetFontLayerColor((ImageFont)g.GetFont(), 0, mColor);
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			g.SetScale(1f, 1f, num, num2);
			g.WriteString(mText, num, num2);
			g.mScaleX = mScaleX;
			g.mScaleY = mScaleY;
		}
	}
}
