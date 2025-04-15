using BejeweledLivePlus.Misc;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class ButterflyGoal : QuestGoal
	{
		public int mNumButterflies;

		public int mButterflyCollectedDisplay;

		public int mButterflyCurrentScore;

		public bool mCountBeforeUpdate;

		public int mSavedCounter;

		public float mScoreScale;

		public bool mScoreIncSize;

		public bool mScoreDecSize;

		public ButterflyGoal(QuestBoard theQuestBoard)
			: base(theQuestBoard)
		{
			mButterflyCollectedDisplay = 0;
			mButterflyCurrentScore = 0;
			mCountBeforeUpdate = false;
			mSavedCounter = 0;
			mScoreScale = 1f;
			mScoreIncSize = false;
			mScoreDecSize = false;
		}

		public override Rect GetCountdownBarRect()
		{
			return new Rect(0, 0, 0, 0);
		}

		public override void DrawPieces(Graphics g, Piece[] pPieces, int numPieces, bool thePostFX)
		{
		}

		public override void Update()
		{
			if (mCountBeforeUpdate && mQuestBoard.mUpdateCnt - mSavedCounter > 0)
			{
				mCountBeforeUpdate = false;
				mSavedCounter = 0;
				mButterflyCollectedDisplay = mButterflyCurrentScore;
				mScoreIncSize = true;
			}
			if (mQuestBoard.mGameStats[28] > mButterflyCurrentScore)
			{
				mCountBeforeUpdate = true;
				mSavedCounter = mQuestBoard.mUpdateCnt;
				mButterflyCurrentScore = mQuestBoard.mGameStats[28];
			}
			if (mScoreIncSize)
			{
				mScoreScale += 0.08f;
				if (mScoreScale >= 3f)
				{
					mScoreIncSize = false;
					mScoreDecSize = true;
				}
			}
			else if (mScoreDecSize)
			{
				mScoreScale -= 0.08f;
				if (mScoreScale <= 1f)
				{
					mScoreScale = 1f;
					mScoreDecSize = false;
				}
			}
		}

		public override void NewGame()
		{
			if (mQuestBoard.mParams.ContainsKey("Butterflies"))
			{
				mNumButterflies = SexyFramework.GlobalMembers.sexyatoi(mQuestBoard.mParams["Butterflies"]);
			}
			mButterflyCollectedDisplay = (mButterflyCurrentScore = mQuestBoard.mGameStats[28]);
			mCountBeforeUpdate = false;
			mSavedCounter = 0;
			mScoreScale = 1f;
			mScoreIncSize = (mScoreDecSize = false);
		}

		public override void PieceTallied(Piece thePiece)
		{
			if (thePiece.IsFlagSet(128u) && (mQuestBoard.mGameOverCount == 0 || mQuestBoard.mIsPerpetual))
			{
				mQuestBoard.AddToStat(28, 1, thePiece.mMoveCreditId);
				if (!mQuestBoard.mIsPerpetual)
				{
					mQuestBoard.mPoints++;
					mQuestBoard.mLevelPointsTotal++;
				}
			}
			base.PieceTallied(thePiece);
		}

		public override int GetLevelPoints()
		{
			if (mQuestBoard.mIsPerpetual)
			{
				return 0;
			}
			return mNumButterflies;
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			int[] array = new int[3] { mNumButterflies, mButterflyCollectedDisplay, mButterflyCurrentScore };
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkButterflyGoal);
			for (int i = 0; i < array.Length; i++)
			{
				theBuffer.WriteInt32(array[i]);
			}
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			return base.SaveGameExtra(theBuffer);
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkButterflyGoal, header, out chunkBeginPos))
			{
				mNumButterflies = theBuffer.ReadInt32();
				mButterflyCollectedDisplay = theBuffer.ReadInt32();
				mButterflyCurrentScore = theBuffer.ReadInt32();
				base.LoadGameExtra(theBuffer);
			}
		}

		public override void DrawOverlay(Graphics g)
		{
			int bFLY_SCORE_DISPLAY_X = ConstantsWP.BFLY_SCORE_DISPLAY_X;
			int bFLY_SCORE_DISPLAY_Y = ConstantsWP.BFLY_SCORE_DISPLAY_Y;
			g.SetFont(GlobalMembersResources.FONT_DIALOG);
			g.SetColor(new Color(255, 255, 255, (int)(255f * mQuestBoard.GetAlpha())));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_DIALOG, 0, new Color(255, 255, 255, 255));
			g.WriteString(SexyFramework.Common.CommaSeperate(mQuestBoard.mGameStats[1]), bFLY_SCORE_DISPLAY_X, bFLY_SCORE_DISPLAY_Y, -1, 0);
			g.SetColor(Color.White);
		}

		public override bool DrawScoreWidget(Graphics g)
		{
			return !mQuestBoard.mIsPerpetual;
		}

		public override bool DrawScore(Graphics g)
		{
			string theString = SexyFramework.Common.CommaSeperate(mButterflyCollectedDisplay);
			g.StringWidth(theString);
			int nUM_BUTTERFLY_DISPLAY_X = ConstantsWP.NUM_BUTTERFLY_DISPLAY_X;
			int nUM_BUTTERFLY_DISPLAY_Y = ConstantsWP.NUM_BUTTERFLY_DISPLAY_Y;
			g.PushState();
			g.SetScale(mScoreScale, mScoreScale, nUM_BUTTERFLY_DISPLAY_X, nUM_BUTTERFLY_DISPLAY_Y - GlobalMembers.S(10));
			g.SetFont(GlobalMembersResources.FONT_SUBHEADER);
			g.SetColor(new Color(255, 255, 255, (int)(255f * mQuestBoard.GetAlpha())));
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 0, Bej3Widget.COLOR_SUBHEADING_4_STROKE);
			Utils.SetFontLayerColor((ImageFont)GlobalMembersResources.FONT_SUBHEADER, 1, Bej3Widget.COLOR_SUBHEADING_4_FILL);
			g.WriteString(SexyFramework.Common.CommaSeperate(mButterflyCollectedDisplay), nUM_BUTTERFLY_DISPLAY_X, nUM_BUTTERFLY_DISPLAY_Y, -1, 0);
			g.SetColor(Color.White);
			g.PopState();
			return !mQuestBoard.mIsPerpetual;
		}

		public override int GetSidebarTextY()
		{
			return GlobalMembers.M(80);
		}
	}
}
