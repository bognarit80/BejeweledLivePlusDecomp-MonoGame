using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Common = SexyFramework.Common;

namespace BejeweledLivePlus
{
	public class DigGoal : QuestGoal
	{
		public enum EDigPieceType
		{
			eDigPiece_Artifact,
			eDigPiece_Block,
			eDigPiece_Goal,
			eDigPiece_COUNT
		}

		public enum DrawPiecesPass
		{
			DrawNormal,
			DrawAdditive,
			COUNT
		}

		public enum DrawPiecePrePass
		{
			DrawNormal,
			DrawAdditive,
			DrawShadow,
			COUNT
		}

		public enum DrawGameElementsPass
		{
			DrawCenter,
			DrawGrassTop,
			DrawEdgeShadows,
			DrawEdges,
			DrawGrassLR,
			COUNT
		}

		public class TileData
		{
			public Ref<int> mStrength;

			public Ref<int> mArtifactId;

			public Ref<int> mGoldOrDiamondValue;

			public int mStartStrength;

			public EDigPieceType mPieceType;

			public uint mRandCfg;

			public BlingParticleEffect mBlingPIFx;

			public PIEffect mGoldInnerFx;

			public bool mIsTopTile;

			public bool mIsDeleting;

			public DiamondEffect mDiamondFx;

			public TileData()
			{
				mIsDeleting = false;
				mStartStrength = 1;
				mPieceType = EDigPieceType.eDigPiece_Goal;
				mRandCfg = 0u;
				mBlingPIFx = null;
				mGoldInnerFx = null;
				mIsTopTile = false;
				mDiamondFx = null;
				mStrength = new Ref<int>(1);
				mArtifactId = mStrength;
				mGoldOrDiamondValue = mStrength;
			}

			public TileData(TileData rhs)
			{
				mIsDeleting = rhs.mIsDeleting;
				mStartStrength = rhs.mStartStrength;
				mPieceType = rhs.mPieceType;
				mRandCfg = rhs.mRandCfg;
				mBlingPIFx = rhs.mBlingPIFx;
				mGoldInnerFx = rhs.mGoldInnerFx;
				mIsTopTile = rhs.mIsTopTile;
				mDiamondFx = rhs.mDiamondFx;
				mStrength = new Ref<int>(rhs.mStrength.value);
				mArtifactId = mStrength;
				mGoldOrDiamondValue = mStrength;
			}

			public void SetAs(EDigPieceType theType, int theStr, Piece thePiece, DigGoal theGoal)
			{
				if (mDiamondFx != null)
				{
					mDiamondFx.Dispose();
				}
				mDiamondFx = null;
				if (mBlingPIFx != null)
				{
					mBlingPIFx.mDeleteMe = true;
					if (mBlingPIFx.mRefCount > 0)
					{
						mBlingPIFx.mRefCount--;
					}
					mBlingPIFx = null;
				}
				if (mGoldInnerFx != null)
				{
					mGoldInnerFx.Dispose();
					mGoldInnerFx = null;
				}
				mPieceType = theType;
				mStrength.value = theStr;
				mStartStrength = mStrength;
				mRandCfg = (uint)BejeweledLivePlus.Misc.Common.Rand();
				if (mPieceType == EDigPieceType.eDigPiece_Goal)
				{
					bool flag = IsDiamond();
					if (flag)
					{
						mDiamondFx = new DiamondEffect(Math.Min(4, mStrength.value - 3));
					}
					mBlingPIFx = BlingParticleEffect.fromPIEffectAndPieceId(flag ? GlobalMembersResourcesWP.PIEFFECT_DIAMOND_SPARKLES : GlobalMembersResourcesWP.PIEFFECT_GOLD_BLING, thePiece.mId);
					mBlingPIFx.mDoDrawTransform = true;
					mBlingPIFx.mX = thePiece.CX();
					mBlingPIFx.mY = thePiece.CY();
					mBlingPIFx.mPieceId = thePiece.mId;
					mBlingPIFx.mRefCount++;
					theGoal.mQuestBoard.mPostFXManager.AddEffect(mBlingPIFx);
				}
				else if (mPieceType == EDigPieceType.eDigPiece_Block)
				{
					if ((int)mStrength == 4 && theGoal.mBlackrockTipPieceId == -1)
					{
						theGoal.mBlackrockTipPieceId = thePiece.mId;
					}
				}
				else if (mPieceType == EDigPieceType.eDigPiece_Artifact)
				{
					if (theGoal.mArtifacts.Count > 0)
					{
						mStrength.value = Math.Max(0, Math.Min(theGoal.mArtifacts.Count - 1, mStrength));
					}
					mStartStrength = mStrength;
				}
			}

			public bool IsDiamond()
			{
				return (int)mGoldOrDiamondValue > 3;
			}
		}

		public class ArtifactData
		{
			public string mId;

			public string mName = string.Empty;

			public int mMinDepth;

			public int mValue;

			public int mImageId;

			public int mUnderlayImgId;

			public int mOverlayImgId;
		}

		public class OldPieceData
		{
			public int mFlags;

			public int mColor;
		}

		public class CheckPiece
		{
			public int mCol;

			public int mRow;

			public bool mIsHyperCube;

			public int mMoveCreditId;

			public CheckPiece(int col, int row, bool isHyperCube, int moveCreditId)
			{
				mCol = col;
				mRow = row;
				mIsHyperCube = isHyperCube;
				mMoveCreditId = moveCreditId;
			}
		}

		private struct MyPiece
		{
			public Piece piece;

			public TileData tile;
		}

		public const int COIN_SIZE = 70;

		public const int TRIGGER_LINE_AT = 6;

		public const int GOLD_COLLECTION_COUNT = 3;

		public const int MAX_MINE_STRENGTH = 7;

		public const int MAX_BLOCKER_STRENGTH = 5;

		public const int DARK_ROCK_STRENGTH = 4;

		public const int INFINITE_GOLD_STRENGTH = 9;

		internal int[] ARTIFACT_OVERLAY_IDS = new int[3] { 1284, 1285, 1286 };

		internal int[] ARTIFACT_UNDERLAY_IDS = new int[3] { 1287, 1288, 1289 };

		public List<ArtifactData> mArtifacts = new List<ArtifactData>();

		public CurvedVal mArtifactPossRange = new CurvedVal();

		public List<int> mArtifactPoss = new List<int>();

		public List<int> mCollectedArtifacts = new List<int>();

		public List<int> mPlacedArtifacts = new List<int>();

		public List<int> mTreasureRangeMin = new List<int>();

		public List<int> mTreasureRangeMax = new List<int>();

		public List<List<bool>> mVisited = new List<List<bool>>();

		public Dictionary<int, TileData> mIdToTileData = new Dictionary<int, TileData>();

		public int mInitPushAnimPixels;

		public CurvedVal mInitPushAnimCv = new CurvedVal();

		public int mBlackrockTipPieceId;

		public bool mDefaultNeverAllowCascades;

		public bool mSkipMoveCreditIdCheck;

		public CurvedVal mDigBarFlash = new CurvedVal();

		public int mGoldValue;

		public int mGemValue;

		public int mArtifactBaseValue;

		public int mDigBarFlashCount;

		public int mTargetCount;

		public int mTextFlashTicks;

		public int[] mTreasureEarnings = new int[3];

		public int mArtifactMinTiles;

		public int mArtifactMaxTiles;

		public int mNextArtifactTileCount;

		public List<int> mStartArtifactRow = new List<int>();

		public List<CheckPiece> mCheckPieces = new List<CheckPiece>();

		public List<Piece> mMovingPieces = new List<Piece>();

		public List<int> mHypercubes = new List<int>();

		public OldPieceData[,] mOldPieceData = new OldPieceData[2, 8];

		public bool mAllowDescent;

		public double mGridDepth;

		public int mRandRowIdx;

		public int mDigCountPerScroll;

		public GridData mGridData = new GridData();

		public CurvedVal mCvScrollY = new CurvedVal();

		public CurvedVal mCvShakey = new CurvedVal();

		public bool mForceScroll;

		public int mBoardOffsetXExtra;

		public int mBoardOffsetYExtra;

		public string mTutorialHeader = string.Empty;

		public string mTutorialText = string.Empty;

		public int mClearedAnimAtTick;

		public int mAllClearedAnimAtTick;

		public bool mClearedAnimPlayed;

		public bool mAllClearAnimPlayed;

		public bool mFirstChestPt;

		public bool mInMegaDig;

		public bool mDrawScoreWidget;

		public List<GoldCollectEffect> mGoldFx = new List<GoldCollectEffect>();

		public Dictionary<int, int> mIdToBlastCount = new Dictionary<int, int>();

		public CurvedVal mCvDarkRockFreq = new CurvedVal();

		public CurvedVal mCvMinBrickStr = new CurvedVal();

		public CurvedVal mCvMaxBrickStr = new CurvedVal();

		public CurvedVal mCvEdgeBrickStrPerLevel = new CurvedVal();

		public CurvedVal mCvMinMineStr = new CurvedVal();

		public CurvedVal mCvMaxMineStr = new CurvedVal();

		public SpreadCurve mBrickStrSpread = new SpreadCurve(500);

		public SpreadCurve mArtifactSpread = new SpreadCurve(500);

		public SpreadCurve mMineStrSpread = new SpreadCurve(500);

		public int mGoldRushTipPieceId;

		public int mNextBottomHypermixerWait;

		public int mArtifactSkipTileCount;

		public int mPowerGemThresholdDepth0;

		public int mPowerGemThresholdDepth20;

		public int mPowerGemThresholdDepth40;

		public CurvedVal mCvMineProb = new CurvedVal();

		public bool mWantTopFrame;

		public List<int> mArtifactVals = new List<int>();

		public EffectsManager mGoldPointsFXManager;

		public EffectsManager mDustFXManager;

		public EffectsManager mMessageFXManager;

		public bool mUsingRandomizers;

		public float mGameTicksF;

		public bool mLoadingGame;

		public bool mDigInProgress;

		private int[,] IsSpecialPieceUnlocked_neighbors = new int[4, 2]
		{
			{ 1, 0 },
			{ -1, 0 },
			{ 0, 1 },
			{ 0, -1 }
		};

		private static MyPiece[] DP_pHypercubes = new MyPiece[32];

		private static MyPiece[] DP_pSpecial = new MyPiece[32];

		private static MyPiece[] DP_pGoals = new MyPiece[32];

		private static MyPiece[] DP_pBlocks = new MyPiece[32];

		private static MyPiece[] DP_pArtifacts = new MyPiece[32];

		private static int[,] DrawGameElements_DrawGrassLR_neighbors = new int[2, 2]
		{
			{ 1, 0 },
			{ -1, 0 }
		};

		private static int[] DrawGameElements_DrawGrassLR_images = new int[2] { 1229, 1228 };

		private static int[,] DrawGameElements_neighbors = new int[4, 2]
		{
			{ 1, 0 },
			{ -1, 0 },
			{ 0, 1 },
			{ 0, -1 }
		};

		private static int[] DrawGameElements_images = new int[4] { 1177, 1180, 1183, 1173 };

		private static int[] DrawGameElements_imagesHighlights = new int[4] { 1178, 1181, 1184, 1174 };

		private static int[] DrawGameElements_imagesHighlightShadows = new int[4] { 1179, 1182, -1, 1175 };

		private static bool DrawDigBarAnim_made = false;

		private static CurvedVal DrawDigBarAnim_cvClearAnimX = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvClearAnimScale = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvClearAnimAlpha = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvClearAnimColorCycle = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvAllClearAnimX = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvAllClearAnimScale = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvAllClearAnimAlpha = new CurvedVal();

		private static CurvedVal DrawDigBarAnim_cvAllClearAnimColorCycle = new CurvedVal();

		private static int[,] Update_neighbors = new int[4, 2]
		{
			{ 1, 0 },
			{ -1, 0 },
			{ 0, 1 },
			{ 0, -1 }
		};

		private static Rect TriggerDigPiece_goldCollectRect = new Rect(GlobalMembers.M(10), GlobalMembers.M(10), 100 - GlobalMembers.M(20), 100 - GlobalMembers.M(20));

		public static void CalcRotatedBounds(int theW, int theH, double theRot, ref int theNewW, ref int theNewH)
		{
			double num = Math.Cos(theRot);
			double num2 = Math.Sin(theRot);
			theNewW = (int)(Math.Abs(num * (double)theW) + Math.Abs(num2 * (double)theH));
			theNewH = (int)(Math.Abs(num2 * (double)theW) + Math.Abs(num * (double)theH));
		}

		public static int ImgCXOfs(int theId)
		{
			return (int)(GlobalMembersResourcesWP.ImgXOfs(theId) + (float)(GlobalMembersResourcesWP.GetImageById(theId).GetWidth() / 2));
		}

		public static int ImgCYOfs(int theId)
		{
			return (int)(GlobalMembersResourcesWP.ImgYOfs(theId) + (float)(GlobalMembersResourcesWP.GetImageById(theId).GetHeight() / 2));
		}

		public DigGoal(QuestBoard theQuestBoard)
			: base(theQuestBoard)
		{
			mDefaultNeverAllowCascades = true;
			for (int i = 0; i < 8; i++)
			{
				List<bool> list = new List<bool>
				{
					Capacity = 8
				};
				for (int j = 0; j < 8; j++)
				{
					list.Add(false);
				}
				mVisited.Add(list);
			}
			mSkipMoveCreditIdCheck = false;
			mWantTopFrame = true;
			mGridDepth = 0.0;
			mRandRowIdx = 0;
			mArtifactMinTiles = 0;
			mArtifactMaxTiles = 8;
			mAllowDescent = false;
			mForceScroll = false;
			mQuestBoard.mScrambleUsesLeft = 1;
			mTextFlashTicks = 0;
			mAllClearedAnimAtTick = -1;
			mClearedAnimAtTick = -1;
			mArtifactPossRange.SetConstant(12.0);
			mNextArtifactTileCount = -1;
			mGoldRushTipPieceId = -1;
			mUpdateCnt.value = 0;
			mGoldValue = 1;
			mGemValue = 1;
			mArtifactBaseValue = 1;
			mFirstChestPt = true;
			mQuestBoard.mShowLevelPoints = !mQuestBoard.mIsPerpetual;
			mBoardOffsetXExtra = 0;
			mBoardOffsetYExtra = 0;
			mNextBottomHypermixerWait = 0;
			mInMegaDig = false;
			mDigBarFlashCount = 0;
			mClearedAnimPlayed = false;
			mAllClearAnimPlayed = false;
			GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.ResetAnim();
			GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.ResetAnim();
			mBlackrockTipPieceId = -1;
			for (int k = 0; k < 3; k++)
			{
				mTreasureEarnings[k] = 0;
			}
			mDigBarFlash.SetConstant(0.0);
			mDigBarFlash.SetMode(0);
			mQuestBoard.mHypermixerCheckRow = GlobalMembers.M(3);
			mInitPushAnimPixels = 0;
			mUsingRandomizers = false;
			mGoldPointsFXManager = new EffectsManager(mQuestBoard);
			mDustFXManager = new EffectsManager(mQuestBoard);
			mMessageFXManager = new EffectsManager(mQuestBoard);
			mGoldPointsFXManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mDustFXManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mMessageFXManager.Resize(0, 0, GlobalMembers.gApp.mWidth, GlobalMembers.gApp.mHeight);
			mCvScrollY.SetConstant(0.0);
			mCvScrollY.mAppUpdateCountSrc = mUpdateCnt;
			mCvShakey.mAppUpdateCountSrc = mUpdateCnt;
			for (int l = 0; l < 2; l++)
			{
				for (int m = 0; m < 8; m++)
				{
					mOldPieceData[l, m] = new OldPieceData();
					mOldPieceData[l, m].mFlags = -1;
					mOldPieceData[l, m].mColor = -1;
				}
			}
			mLoadingGame = false;
			mDigInProgress = false;
		}

		public override void Dispose()
		{
			for (int i = 0; i < mGoldFx.Count; i++)
			{
				if (mGoldFx[i] != null)
				{
					mGoldFx[i].Dispose();
				}
			}
			mGoldFx.Clear();
			foreach (TileData value in mIdToTileData.Values)
			{
				if (value.mDiamondFx != null)
				{
					value.mDiamondFx.Dispose();
					value.mDiamondFx = null;
				}
				if (value.mGoldInnerFx != null)
				{
					value.mGoldInnerFx.Dispose();
					value.mGoldInnerFx = null;
				}
				if (value.mBlingPIFx != null && value.mBlingPIFx.mRefCount > 0)
				{
					value.mBlingPIFx.mRefCount--;
				}
			}
			for (int j = 0; j < mMovingPieces.Count; j++)
			{
				if (mMovingPieces[j] != null)
				{
					mMovingPieces[j].Dispose();
				}
			}
			mMovingPieces.Clear();
			mGoldPointsFXManager.Dispose();
			mDustFXManager.Dispose();
			mMessageFXManager.Dispose();
		}

		public override int GetBoardX()
		{
			return mQuestBoard.CallBoardGetBoardX() + mBoardOffsetXExtra;
		}

		public override int GetBoardY()
		{
			return mQuestBoard.CallBoardGetBoardY() + GetBoardScrollOffsetY() + mBoardOffsetYExtra;
		}

		public void SyncGrid(int theStartRow)
		{
			bool[] array = new bool[8];
			bool flag = ((theStartRow != 0) ? true : false);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = flag;
			}
			int num = 8;
			for (int j = theStartRow; j < 8; j++)
			{
				int num2 = j + GetGridDepth();
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				mArtifactPoss.Clear();
				for (int k = 0; k < mArtifacts.Count; k++)
				{
					if (GetGridDepth() >= mArtifacts[k].mMinDepth && mPlacedArtifacts.IndexOf(k) < 0 && mCollectedArtifacts.IndexOf(k) < 0)
					{
						int mMinDepth = mArtifacts[k].mMinDepth;
						if (!dictionary.ContainsKey(mMinDepth))
						{
							dictionary.Add(mMinDepth, new List<int>());
						}
						dictionary[mArtifacts[k].mMinDepth].Add(k);
					}
				}
				float num3 = 1f;
				float num4 = (float)mRandRowIdx * num3;
				int num5 = (int)Utils.Round((float)mArtifactPossRange.GetOutVal(num4));
				List<int> list = new List<int>();
				list.AddRange(dictionary.Keys);
				int[] array2 = list.ToArray();
				for (int l = 0; l < array2.Length - 1; l++)
				{
					for (int m = l + 1; m < array2.Length; m++)
					{
						if (array2[l] < array2[m])
						{
							int num6 = array2[l];
							array2[l] = array2[m];
							array2[m] = num6;
						}
					}
				}
				int[] array3 = array2;
				foreach (int key in array3)
				{
					List<int> list2 = dictionary[key];
					for (int num7 = 0; num7 < list2.Count; num7++)
					{
						int index = BejeweledLivePlus.Misc.Common.Rand() % list2.Count;
						int value = list2[num7];
						list2[num7] = list2[index];
						list2[index] = value;
					}
					for (int num8 = 0; num8 < list2.Count; num8++)
					{
						if (mArtifactPoss.Count >= num5)
						{
							break;
						}
						mArtifactPoss.Add(list2[num8]);
					}
				}
				mArtifactPoss.Reverse();
				List<Piece> list3 = new List<Piece>();
				for (int num9 = 0; num9 < 8; num9++)
				{
					bool flag2 = false;
					Piece pieceAtRowCol = mQuestBoard.GetPieceAtRowCol(j, num9);
					if (pieceAtRowCol == null)
					{
						continue;
					}
					bool o_isImmovable = false;
					bool flag3 = mQuestBoard.mIsPerpetual;
					TileData tileData = null;
					if (mUsingRandomizers && num2 >= mGridData.GetRowCount())
					{
						tileData = GenRandomTile(num4, pieceAtRowCol, ref o_isImmovable);
					}
					else
					{
						while (num2 >= mGridData.GetRowCount())
						{
							num2 -= 8;
						}
						GridData.TileData tileData2 = mGridData.At(num2, num9);
						char c = char.ToUpper(tileData2.mAttr);
						Piece pieceAtRowCol2 = mQuestBoard.GetPieceAtRowCol(j, num9);
						switch (c)
						{
						case 'H':
							if (tileData2.mBack != 0)
							{
								mHypercubes.Add(pieceAtRowCol2.mId);
							}
							else
							{
								mQuestBoard.Hypercubeify(pieceAtRowCol2);
							}
							flag3 = false;
							break;
						case 'S':
							pieceAtRowCol2.SetFlag(4096u);
							pieceAtRowCol2.mCanScramble = false;
							flag3 = false;
							break;
						case 'R':
							if (mUsingRandomizers)
							{
								tileData = GenRandomTile(num4, pieceAtRowCol2, ref o_isImmovable);
								if (pieceAtRowCol2.IsFlagSet(65536u) && pieceAtRowCol2.mId == mBlackrockTipPieceId && mQuestBoard.WantsTutorial(16))
								{
									flag3 = false;
								}
							}
							break;
						}
						if (tileData2.mBack != 0)
						{
							if (mIdToTileData.ContainsKey(pieceAtRowCol2.mId))
							{
								tileData = mIdToTileData[pieceAtRowCol2.mId];
							}
							else
							{
								tileData = new TileData();
								mIdToTileData[pieceAtRowCol2.mId] = tileData;
							}
							pieceAtRowCol2.SetFlag(65536u);
							switch (c)
							{
							case 'M':
								flag3 = false;
								tileData.SetAs(EDigPieceType.eDigPiece_Goal, (int)tileData2.mBack, pieceAtRowCol2, this);
								break;
							case 'A':
								tileData.SetAs(EDigPieceType.eDigPiece_Artifact, (int)tileData2.mBack, pieceAtRowCol2, this);
								flag3 = false;
								break;
							default:
								tileData.SetAs(EDigPieceType.eDigPiece_Block, (int)tileData2.mBack, pieceAtRowCol2, this);
								flag3 = false;
								break;
							}
							o_isImmovable = true;
						}
					}
					if (tileData != null)
					{
						if (mNextArtifactTileCount-- <= 0)
						{
							flag2 = true;
						}
						tileData.mIsTopTile = !array[num9];
						if (tileData.mIsTopTile)
						{
							array[num9] = true;
						}
					}
					if (flag2 && flag3)
					{
						list3.Add(pieceAtRowCol);
					}
					if (o_isImmovable)
					{
						pieceAtRowCol.mCanScramble = false;
						pieceAtRowCol.mCanSwap = false;
						pieceAtRowCol.mCanDestroy = true;
						pieceAtRowCol.mCanMatch = false;
						pieceAtRowCol.mColor = -1;
						pieceAtRowCol.mX = mQuestBoard.GetColX(pieceAtRowCol.mCol);
						pieceAtRowCol.mY = mQuestBoard.GetRowY(pieceAtRowCol.mRow);
						num = Math.Min(pieceAtRowCol.mRow, num);
					}
					if (flag2 && list3.Count > 0 && mArtifactPoss.Count > 0)
					{
						Piece piece = list3[BejeweledLivePlus.Misc.Common.Rand() % list3.Count];
						float outVal = mArtifactSpread.GetOutVal(GlobalMembersUtils.GetRandFloatU());
						int index2 = (int)Utils.Round((float)(mArtifactPoss.Count - 1) * outVal);
						mIdToTileData[piece.mId].SetAs(EDigPieceType.eDigPiece_Artifact, mArtifactPoss[index2], piece, this);
						mPlacedArtifacts.Add(mArtifactPoss[index2]);
						mArtifactPoss.RemoveAt(index2);
						AdvanceArtifact(false);
					}
				}
				if (mUsingRandomizers)
				{
					mRandRowIdx++;
				}
			}
			mInitPushAnimPixels = (8 - num + 1) * 100 + GlobalMembers.M(500);
			ResyncInitPushAnim();
		}

		public int GetDarkenedRGBForRow(int theRow, bool theIsShadow)
		{
			float num = (float)GetExtraLuminosity(GetGridDepth() + theRow);
			int num2 = GlobalMembers.M(255) - (int)((double)num * GlobalMembers.M(200.0));
			int num3 = GlobalMembers.M(255) - (int)((double)num * GlobalMembers.M(170.0));
			int num4 = (num3 << 16) | (num2 << 8) | num2;
			if (theIsShadow)
			{
				return Utils.ColorLerp(new Color(num4), new Color(GlobalMembers.M(4210752)), 0.5f).ToInt();
			}
			return num4;
		}

		public void AdvanceArtifact(bool theFirstOne)
		{
			if (mQuestBoard.mIsPerpetual)
			{
				mNextArtifactTileCount = mArtifactMinTiles + BejeweledLivePlus.Misc.Common.Rand(mArtifactMaxTiles - mArtifactMinTiles);
				if (theFirstOne)
				{
					mNextArtifactTileCount += mArtifactSkipTileCount;
				}
			}
		}

		public void UpdateShift()
		{
			float num = GetDigCountPerScroll();
			float num2 = (float)(mCvScrollY.GetOutVal() * (double)num);
			mCvScrollY.IncInVal();
			float num3 = (float)(mCvScrollY.GetOutVal() * (double)num);
			if (((int)num2 != (int)num3 || (num2 != num3 && (double)num2 == 0.0)) && (int)num3 < GetDigCountPerScroll())
			{
				ShiftRowsUp();
			}
			if (mCvScrollY.HasBeenTriggered())
			{
				((DigBoard)mQuestBoard).mRotatingCounter.IncByNum((int)(double)mCvScrollY * GetDigCountPerScroll() * 10);
				mGridDepth += (int)(double)mCvScrollY * GetDigCountPerScroll();
				mCvScrollY.SetConstant(0.0);
			}
		}

		public void ShiftRowsUp()
		{
			for (int num = mMovingPieces.Count - 1; num >= 0; num--)
			{
				Piece piece = mMovingPieces[num];
				if (piece != null)
				{
					mQuestBoard.mPreFXManager.FreePieceEffect(piece);
					mQuestBoard.mPostFXManager.FreePieceEffect(piece);
					piece.release();
				}
			}
			mMovingPieces.Clear();
			for (int i = 0; i < 8; i++)
			{
				mMovingPieces.Add(null);
			}
			List<int> newGemColors = mQuestBoard.GetNewGemColors();
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					Piece piece2 = mQuestBoard.mBoard[j, k];
					if (j == 0)
					{
						mOldPieceData[0, k] = mOldPieceData[1, k];
						if (piece2 != null)
						{
							mOldPieceData[1, k].mFlags = piece2.mFlags;
							mOldPieceData[1, k].mColor = piece2.mColor;
						}
						else
						{
							mOldPieceData[1, k].mFlags = -1;
							mOldPieceData[1, k].mColor = -1;
						}
					}
					if (piece2 != null)
					{
						if (j == 0)
						{
							mMovingPieces[k] = piece2;
							piece2.ClearBoundEffects();
							piece2.mX = 100 * k;
							piece2.mY = 100 * (j - 1);
						}
						else
						{
							mQuestBoard.mBoard[j - 1, k] = piece2;
							piece2.mX = 100 * k;
							piece2.mY = 100 * (j - 1);
							piece2.mRow = j - 1;
						}
						mQuestBoard.mBoard[j, k] = null;
					}
					if (j == 7)
					{
						int num2 = 25;
						piece2 = mQuestBoard.CreateNewPiece(j, k);
						while (num2-- > 0 && mQuestBoard.FindMove(null, 0, true, true, false, piece2))
						{
							piece2.mColor = newGemColors[BejeweledLivePlus.Misc.Common.Rand() % newGemColors.Count];
						}
					}
				}
			}
			SyncGrid(7);
			if (mQuestBoard.FindMove(null, 0, true, true))
			{
				return;
			}
			int num3 = 50;
			while (--num3 > 0)
			{
				Piece piece3 = mQuestBoard.mBoard[0, BejeweledLivePlus.Misc.Common.Rand() % 8];
				if (piece3 != null)
				{
					mQuestBoard.Hypercubeify(piece3);
					break;
				}
			}
		}

		public override int GetSidebarTextY()
		{
			return GlobalMembers.MS(265);
		}

		public override void GameOverExit()
		{
			mQuestBoard.mPoints = mQuestBoard.mLevelPointsTotal;
		}

		public override bool CanPlay()
		{
			if (mCvScrollY.IsDoingCurve())
			{
				return false;
			}
			return base.CanPlay();
		}

		public override int GetTimeDrawX()
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				return mQuestBoard.CallBoardGetTimeDrawX();
			}
			Rect countdownBarRect = mQuestBoard.GetCountdownBarRect();
			return countdownBarRect.mX + (int)((float)countdownBarRect.mWidth * mQuestBoard.mCountdownBarPct);
		}

		public int GetDigCountPerScroll()
		{
			return mDigCountPerScroll * ((!mInMegaDig) ? 1 : 2);
		}

		public virtual void SetShadowClipRect(Graphics g, int theXOff, int theYOff)
		{
			g.SetClipRect(new Rect(theXOff, GlobalMembers.S(mQuestBoard.CallBoardGetBoardY() + mBoardOffsetYExtra + 600 + GlobalMembers.M(6)) + theYOff, GlobalMembers.gApp.mWidth * 2, GlobalMembers.S(200 + GlobalMembers.M(10))));
		}

		public void SetAdditiveClipRect(Graphics g, int theXOff, int theYOff)
		{
			g.SetClipRect(new Rect(theXOff, GlobalMembers.S(mQuestBoard.CallBoardGetBoardY() + mBoardOffsetYExtra + GlobalMembers.M(6)) + theYOff, GlobalMembers.gApp.mWidth * 2, GlobalMembers.S(600 + GlobalMembers.M(10))));
		}

		public bool CheckNeedScroll(bool theMegaDig)
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				return false;
			}
			if (mForceScroll)
			{
				return true;
			}
			for (int num = (theMegaDig ? 7 : 5); num >= 0; num--)
			{
				for (int i = 0; i < 8; i++)
				{
					Piece piece = mQuestBoard.mBoard[num, i];
					if (piece != null && piece.IsFlagSet(65536u))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsDoubleHypercubeActive()
		{
			for (int i = 0; i < mQuestBoard.mLightningStorms.Count; i++)
			{
				LightningStorm lightningStorm = mQuestBoard.mLightningStorms[i];
				if (lightningStorm.mStormType == 7 && lightningStorm.mColor == -1)
				{
					return true;
				}
			}
			return false;
		}

		public double GetExtraLuminosity(int theDepth)
		{
			return Math.Max(0.0, Math.Min(1.0, (double)(theDepth - GlobalMembers.M(4)) / GlobalMembers.M(40.0)));
		}

		public void ResyncInitPushAnim()
		{
			if (!mInitPushAnimCv.IsDoingCurve())
			{
				return;
			}
			foreach (int key in mIdToTileData.Keys)
			{
				Piece pieceById = mQuestBoard.GetPieceById(key);
				if (pieceById != null)
				{
					pieceById.mX = mQuestBoard.GetColX(pieceById.mCol);
					pieceById.mY = (float)mQuestBoard.GetRowY(pieceById.mRow) + (float)GlobalMembers.S((double)mInitPushAnimPixels * (double)mInitPushAnimCv);
				}
			}
		}

		public override void SetupBackground(int theDeltaIdx)
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				mQuestBoard.CallBoardSetupBackground(theDeltaIdx);
			}
			else
			{
				GlobalMembers.gApp.ClearUpdateBacklog(false);
			}
		}

		public override Rect GetCountdownBarRect()
		{
			if (mQuestBoard.mIsPerpetual)
			{
				return new Rect((int)(GlobalMembers.IMG_SXOFS(874) + (float)(GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER.mWidth / 2)), (int)GlobalMembers.IMG_SYOFS(874), GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_BACK.mWidth - GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_TIMER.mWidth, GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_PROGRESS_BAR_BACK.mHeight);
			}
			return mQuestBoard.CallBoardGetCountdownBarRect();
		}

		public override bool WantWarningGlow()
		{
			if (mQuestBoard.mIsPerpetual && CheckNeedScroll(false))
			{
				return false;
			}
			return mQuestBoard.CallBoardWantWarningGlow();
		}

		public void DrawScrollingDigLineText(Graphics g, Font theFont, int theY, string theStr, int theTicksElapsed, CurvedVal theAnimX, CurvedVal theAnimScale, CurvedVal theAnimAlpha, CurvedVal theColorCycle, bool theUseHueColorCycle, int theShadowCount, int theShadowTotal, int theLastX)
		{
			DrawScrollingDigLineText(g, theFont, theY, theStr, theTicksElapsed, theAnimX, theAnimScale, theAnimAlpha, theColorCycle, theUseHueColorCycle, theShadowCount, theShadowTotal, theLastX, 0);
		}

		public void DrawScrollingDigLineText(Graphics g, Font theFont, int theY, string theStr, int theTicksElapsed, CurvedVal theAnimX, CurvedVal theAnimScale, CurvedVal theAnimAlpha, CurvedVal theColorCycle, bool theUseHueColorCycle, int theShadowCount, int theShadowTotal)
		{
			DrawScrollingDigLineText(g, theFont, theY, theStr, theTicksElapsed, theAnimX, theAnimScale, theAnimAlpha, theColorCycle, theUseHueColorCycle, theShadowCount, theShadowTotal, 0, 0);
		}

		public void DrawScrollingDigLineText(Graphics g, Font theFont, int theY, string theStr, int theTicksElapsed, CurvedVal theAnimX, CurvedVal theAnimScale, CurvedVal theAnimAlpha, CurvedVal theColorCycle, bool theUseHueColorCycle, int theShadowCount)
		{
			DrawScrollingDigLineText(g, theFont, theY, theStr, theTicksElapsed, theAnimX, theAnimScale, theAnimAlpha, theColorCycle, theUseHueColorCycle, theShadowCount, 0, 0, 0);
		}

		public void DrawScrollingDigLineText(Graphics g, Font theFont, int theY, string theStr, int theTicksElapsed, CurvedVal theAnimX, CurvedVal theAnimScale, CurvedVal theAnimAlpha, CurvedVal theColorCycle, bool theUseHueColorCycle)
		{
			DrawScrollingDigLineText(g, theFont, theY, theStr, theTicksElapsed, theAnimX, theAnimScale, theAnimAlpha, theColorCycle, theUseHueColorCycle, 0, 0, 0, 0);
		}

		public void DrawScrollingDigLineText(Graphics g, Font theFont, int theY, string theStr, int theTicksElapsed, CurvedVal theAnimX, CurvedVal theAnimScale, CurvedVal theAnimAlpha, CurvedVal theColorCycle, bool theUseHueColorCycle, int theShadowCount, int theShadowTotal, int theLastX, int theLastY)
		{
			float num = (float)theTicksElapsed / 100f;
			double num2 = theAnimAlpha.GetOutVal(num);
			double outVal = theAnimScale.GetOutVal(num);
			double outVal2 = theAnimX.GetOutVal(num);
			double num3 = (double)mQuestBoard.GetBoardX() + 800.0 * outVal2;
			double num4 = ConstantsWP.DIG_BOARD_CLEARED_TEXT_Y;
			if (theShadowCount > 0)
			{
				int theShadowTotal2 = ((theShadowTotal <= 0) ? theShadowCount : theShadowTotal);
				int theShadowCount2 = theShadowCount - 1;
				if (theTicksElapsed >= 0)
				{
					DrawScrollingDigLineText(g, theFont, theY, theStr, theTicksElapsed - GlobalMembers.M(4), theAnimX, theAnimScale, theAnimAlpha, theColorCycle, theUseHueColorCycle, theShadowCount2, theShadowTotal2, (int)num3, (int)num4);
				}
			}
			g.PushState();
			g.SetClipRect(GlobalMembers.S(GetBoardX() + GlobalMembers.M(0)), GlobalMembers.S(GetBoardY()), GlobalMembers.S(800 + GlobalMembers.M(0)), GlobalMembers.S(800));
			g.SetFont(theFont);
			Utils.SetFontLayerColor((ImageFont)theFont, 0, Bej3Widget.COLOR_INGAME_ANNOUNCEMENT);
			Utils.SetFontLayerColor((ImageFont)theFont, 1, Color.White);
			g.Translate(0, GlobalMembers.S(theY));
			Color color = default(Color);
			color = new Color((int)GlobalMembers.gApp.HSLToRGB(GlobalMembers.M(0) + (int)theColorCycle.GetOutVal((double)num % theColorCycle.mInMax), GlobalMembers.M(250), GlobalMembers.M(110)), GlobalMembers.M(200));
			bool flag = false;
			if (theShadowTotal > 0)
			{
				double num5 = (double)theShadowCount / (double)theShadowTotal;
				if (GlobalMembers.M(1) != 0)
				{
					num2 *= Math.Pow(num5 * GlobalMembers.M(0.75), GlobalMembers.M(1.0));
				}
				if (GlobalMembers.M(0) != 0)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
				}
				if (GlobalMembers.M(1) != 0)
				{
					color = Utils.ColorLerp(new Color(GlobalMembers.M(0)), new Color(color.ToInt()), (float)(num5 * GlobalMembers.M(0.75)));
				}
				double num6 = (double)theLastX - num3;
				double num7 = (double)theLastY - num4;
				if (GlobalMembers.M(1) != 0 && num6 == 0.0 && num7 == 0.0)
				{
					flag = true;
				}
			}
			num4 -= GlobalMembers.RS((double)g.mFont.GetAscent() / 2.0);
			if (!flag && num2 > (double)GlobalMembers.M(0.01f))
			{
				g.SetColor(Color.FAlpha((float)num2));
				g.SetColorizeImages(num2 < 1.0);
				num4 -= (double)GlobalMembers.RS(g.mFont.GetAscent()) / 2.0 * outVal;
				g.SetScale((float)outVal, (float)outVal, (float)GlobalMembers.S(num3), (float)GlobalMembers.S(num4));
				int num8 = (int)((float)GetBoardX() + GlobalMembers.S(ConstantsWP.DIG_BOARD_CLEARED_TEXT_RECT_X));
				int num9 = (int)GlobalMembers.S(num4);
				int theMaxWidth = 0;
				int theLineCount = 0;
				int wordWrappedHeight = g.GetWordWrappedHeight((int)(ConstantsWP.DEVICE_WIDTH_F - (float)(num8 * 2)), theStr, -1, ref theMaxWidth, ref theLineCount);
				g.WriteWordWrapped(new Rect(num8, num9 - wordWrappedHeight / 2, GlobalMembers.S(800) - (int)(GlobalMembers.S(ConstantsWP.DIG_BOARD_CLEARED_TEXT_RECT_X) * 2f), num9 + wordWrappedHeight), theStr, -1, 0);
			}
			g.PopState();
		}

		public bool IsSpecialPieceUnlocked(Piece thePiece)
		{
			for (int i = 0; i < IsSpecialPieceUnlocked_neighbors.GetLength(0); i++)
			{
				int num = thePiece.mCol + IsSpecialPieceUnlocked_neighbors[i, 0];
				int num2 = thePiece.mRow + IsSpecialPieceUnlocked_neighbors[i, 1];
				if (num2 < 0 || num2 >= 8 || num < 0 || num >= 8)
				{
					continue;
				}
				Piece piece = mQuestBoard.mBoard[num2, num];
				if (piece != null && mIdToTileData.ContainsKey(piece.mId))
				{
					continue;
				}
				bool flag = true;
				for (int num3 = num2; num3 >= 0; num3--)
				{
					Piece piece2 = mQuestBoard.mBoard[num3, num];
					if (piece2 != null && mIdToTileData.ContainsKey(piece2.mId))
					{
						flag = false;
					}
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsHypermixerCancelledBy(Piece thePiece)
		{
			if (thePiece.IsFlagSet(2u))
			{
				return !thePiece.IsFlagSet(65536u);
			}
			return false;
		}

		public override bool ExtraSaveGameInfo()
		{
			return mQuestBoard.mIsPerpetual;
		}

		public override bool DoEndLevelDialog()
		{
			if (mQuestBoard.mIsPerpetual)
			{
				EndLevelDialog endLevelDialog = new DigGoalEndLevelDialog(mQuestBoard);
				endLevelDialog.SetQuestName("Diamond Mine");
				GlobalMembers.gApp.AddDialog(38, endLevelDialog);
				mQuestBoard.BringToFront(endLevelDialog);
				return true;
			}
			return false;
		}

		public override bool SaveGameExtra(Serialiser theBuffer)
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				return false;
			}
			if (mCvScrollY.mInMin != mCvScrollY.mInMax && !mCvScrollY.HasBeenTriggered())
			{
				return false;
			}
			if (mGoldFx.Count != 0)
			{
				return false;
			}
			int chunkBeginLoc = theBuffer.WriteGameChunkHeader(GameChunkId.eChunkDigGoal);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.DigIdToTileData, mIdToTileData.Count);
			foreach (int key in mIdToTileData.Keys)
			{
				TileData tileData = mIdToTileData[key];
				theBuffer.WriteLong(key);
				theBuffer.WriteShort((short)tileData.mStrength.value);
				theBuffer.WriteShort((short)tileData.mStartStrength);
				theBuffer.WriteShort((short)tileData.mPieceType);
				theBuffer.WriteLong(tileData.mRandCfg);
				theBuffer.WriteBoolean(tileData.mIsTopTile);
			}
			theBuffer.WriteArrayPair(Serialiser.PairID.DigTreasureEarnings, 3, mTreasureEarnings);
			theBuffer.WriteValuePair(Serialiser.PairID.DigTextFlashTicks, mTextFlashTicks);
			theBuffer.WriteValuePair(Serialiser.PairID.DigNextBottomHypermixerWait, mNextBottomHypermixerWait);
			theBuffer.WriteValuePair(Serialiser.PairID.DigArtifactMinTiles, mArtifactMinTiles);
			theBuffer.WriteValuePair(Serialiser.PairID.DigArtifactMaxTiles, mArtifactMaxTiles);
			theBuffer.WriteValuePair(Serialiser.PairID.DigNextArtifactTileCount, mNextArtifactTileCount);
			theBuffer.WriteVectorPair(Serialiser.PairID.DigStartArtifactRow, mStartArtifactRow);
			theBuffer.WriteVectorPair(Serialiser.PairID.DigHypercubes, mHypercubes);
			theBuffer.WriteValuePair(Serialiser.PairID.DigGridDepth, mGridDepth);
			theBuffer.WriteValuePair(Serialiser.PairID.DigRandRowIdx, mRandRowIdx);
			theBuffer.WriteValuePair(Serialiser.PairID.DigFirstChestPt, mFirstChestPt);
			theBuffer.WriteVectorPair(Serialiser.PairID.DigArtifactPoss, mArtifactPoss);
			theBuffer.WriteVectorPair(Serialiser.PairID.DigPlacedArtifacts, mPlacedArtifacts);
			theBuffer.WriteVectorPair(Serialiser.PairID.DigCollectedArtifacts, mCollectedArtifacts);
			theBuffer.WriteValuePair(Serialiser.PairID.DigTimeLimit, ((TimeLimitBoard)mQuestBoard).mTimeLimit);
			theBuffer.WriteSpecialBlock(Serialiser.PairID.DigOldPieceData, 2, 8);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					theBuffer.WriteLong(mOldPieceData[i, j].mFlags);
					theBuffer.WriteByte((byte)mOldPieceData[i, j].mColor);
				}
			}
			theBuffer.WriteValuePair(Serialiser.PairID.DigRotatingCounter, ((DigBoard)mQuestBoard).mRotatingCounter.mCurNumber);
			theBuffer.WriteBoolean(mDigInProgress);
			theBuffer.FinalizeGameChunkHeader(chunkBeginLoc);
			return true;
		}

		public override void LoadGameExtra(Serialiser theBuffer)
		{
			foreach (GoldCollectEffect item in mGoldFx)
			{
				item?.Dispose();
			}
			mGoldFx.Clear();
			mGoldPointsFXManager.Clear();
			mDustFXManager.Clear();
			mMessageFXManager.Clear();
			mIdToTileData.Clear();
			mInitPushAnimCv.SetConstant(0.0);
			int chunkBeginPos = 0;
			GameChunkHeader header = new GameChunkHeader();
			if (!theBuffer.CheckReadGameChunkHeader(GameChunkId.eChunkDigGoal, header, out chunkBeginPos))
			{
				return;
			}
			int num;
			theBuffer.ReadSpecialBlock(out num);
			for (int i = 0; i < num; i++)
			{
				int key = (int)theBuffer.ReadLong();
				TileData tileData = new TileData();
				tileData.mStrength.value = theBuffer.ReadShort();
				tileData.mStartStrength = theBuffer.ReadShort();
				tileData.mPieceType = (EDigPieceType)theBuffer.ReadShort();
				tileData.mRandCfg = (uint)theBuffer.ReadLong();
				tileData.mIsTopTile = theBuffer.ReadBoolean();
				mIdToTileData.Add(key, tileData);
			}
			theBuffer.ReadArrayPair(3, mTreasureEarnings);
			theBuffer.ReadValuePair(out mTextFlashTicks);
			theBuffer.ReadValuePair(out mNextBottomHypermixerWait);
			theBuffer.ReadValuePair(out mArtifactMinTiles);
			theBuffer.ReadValuePair(out mArtifactMaxTiles);
			theBuffer.ReadValuePair(out mNextArtifactTileCount);
			theBuffer.ReadVectorPair(out mStartArtifactRow);
			theBuffer.ReadVectorPair(out mHypercubes);
			theBuffer.ReadValuePair(out mGridDepth);
			theBuffer.ReadValuePair(out mRandRowIdx);
			theBuffer.ReadValuePair(out mFirstChestPt);
			theBuffer.ReadVectorPair(out mArtifactPoss);
			theBuffer.ReadVectorPair(out mPlacedArtifacts);
			theBuffer.ReadVectorPair(out mCollectedArtifacts);
			foreach (int key2 in mIdToTileData.Keys)
			{
				TileData tileData2 = mIdToTileData[key2];
				int mStartStrength = tileData2.mStartStrength;
				uint mRandCfg = tileData2.mRandCfg;
				tileData2.SetAs(tileData2.mPieceType, tileData2.mStrength, mQuestBoard.GetPieceById(key2), this);
				tileData2.mStartStrength = mStartStrength;
				tileData2.mRandCfg = mRandCfg;
			}
			theBuffer.ReadValuePair(out ((TimeLimitBoard)mQuestBoard).mTimeLimit);
			mGoldRushTipPieceId = -1;
			int num2;
			int num3;
			theBuffer.ReadSpecialBlock(out num2, out num3);
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num3; k++)
				{
					mOldPieceData[j, k].mFlags = (int)theBuffer.ReadLong();
					mOldPieceData[j, k].mColor = theBuffer.ReadByte();
				}
			}
			int theValue;
			theBuffer.ReadValuePair(out theValue);
			((DigBoard)mQuestBoard).mRotatingCounter.ResetCounter(theValue);
			mLoadingGame = true;
			mDigInProgress = theBuffer.ReadBoolean();
		}

		public override int GetUICenterX()
		{
			if (mQuestBoard.mIsPerpetual)
			{
				return mQuestBoard.CallBoardGetUICenterX() + GlobalMembers.M(-75);
			}
			return mQuestBoard.CallBoardGetUICenterX();
		}

		public int GetPieceValue(ref TileData theData, ref ETreasureType outType)
		{
			int result = 0;
			outType = ETreasureType.eTreasure_Gold;
			if (theData.mPieceType == EDigPieceType.eDigPiece_Artifact)
			{
				result = mArtifacts[theData.mStrength].mValue * mArtifactBaseValue;
				outType = ETreasureType.eTreasure_Artifact;
			}
			else if (theData.mPieceType == EDigPieceType.eDigPiece_Goal)
			{
				int num = theData.mStrength;
				int index = Math.Min(num - 1, mTreasureRangeMax.Count - 1);
				int num2 = mTreasureRangeMax[index] - mTreasureRangeMin[index];
				num = mTreasureRangeMin[index] + num2;
				outType = (((int)theData.mStrength > 3) ? ETreasureType.eTreasure_Gem : ETreasureType.eTreasure_Gold);
				if (outType == ETreasureType.eTreasure_Gold)
				{
					result = num * mGoldValue;
				}
				else if (outType == ETreasureType.eTreasure_Gem)
				{
					result = num * mGemValue;
				}
			}
			return result;
		}

		public override bool AllowGameOver()
		{
			if (mGoldFx.Count == 0 && !CheckNeedScroll(false))
			{
				return !mDigInProgress;
			}
			return false;
		}

		public override bool WantBottomFrame()
		{
			return true;
		}

		public override bool WantTopFrame()
		{
			return mWantTopFrame;
		}

		public int GetGridDepth()
		{
			return (int)GetGridDepthAsDouble();
		}

		public double GetGridDepthAsDouble()
		{
			return mGridDepth + (double)mCvScrollY * (double)GetDigCountPerScroll();
		}

		public override uint GetRandSeedOverride()
		{
			if (GlobalMembers.gApp.mDiamondMineFirstLaunch)
			{
				if (!mQuestBoard.mIsPerpetual)
				{
					return (uint)GlobalMembers.M(40000000);
				}
				return (uint)GlobalMembers.M(40000000);
			}
			return mQuestBoard.CallBoardGetRandSeedOverride();
		}

		public override bool WantTopLevelBar()
		{
			return false;
		}

		public override void DrawOverlay(Graphics g)
		{
			if (g.mPushedColorVector.Count > 0)
			{
				g.PopColorMult();
			}
			Color mColor = g.mColor;
			if (GlobalMembers.M(0) != 0)
			{
				for (int i = 0; i < mMovingPieces.Count; i++)
				{
					g.SetColor(new Color(-1));
					g.DrawRect((int)GlobalMembers.S(mMovingPieces[i].GetScreenX()), (int)GlobalMembers.S(mMovingPieces[i].GetScreenY()), GlobalMembers.S(100), GlobalMembers.S(100));
				}
			}
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha == 0f)
			{
				return;
			}
			if (flag)
			{
				g.SetColor(Color.FAlpha(alpha));
				g.SetColorizeImages(true);
				g.PushColorMult();
			}
			if (mQuestBoard.mIsPerpetual)
			{
				if (GlobalMembers.M(0) != 0)
				{
					g.SetColor(new Color(0, (int)((float)GlobalMembers.M(80) * mQuestBoard.GetBoardAlpha())));
					g.FillRect(GlobalMembers.S(mQuestBoard.GetBoardX()), GlobalMembers.S(mQuestBoard.GetBoardY() + 600 - GetBoardScrollOffsetY()), GlobalMembers.S(800), GlobalMembers.S(200));
				}
				g.PushState();
				g.PopState();
			}
			g.SetFont(GlobalMembersResources.FONT_HEADER);
			if (mQuestBoard.mIsPerpetual)
			{
				for (int j = 0; j < 2; j++)
				{
					int num = ((mGoldFx.Count > 10) ? 10 : mGoldFx.Count);
					for (int k = 0; k < num; k++)
					{
						if (mGoldFx[k].mLayerOnTop)
						{
							mGoldFx[k].Draw(g, j);
						}
					}
				}
			}
			mMessageFXManager.Draw(g);
			if (flag)
			{
				g.PopColorMult();
				g.SetColorizeImages(false);
			}
		}

		public override void DrawCountPopups(Graphics g)
		{
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha != 0f)
			{
				if (flag)
				{
					g.SetColor(Color.FAlpha(alpha));
					g.SetColorizeImages(true);
					g.PushColorMult();
				}
				mGoldPointsFXManager.Draw(g);
				if (flag)
				{
					g.PopColorMult();
					g.SetColorizeImages(false);
				}
			}
		}

		public override bool DrawScoreWidget(Graphics g)
		{
			if (mQuestBoard.mIsPerpetual)
			{
				return true;
			}
			return false;
		}

		public override void DrawPieces(Graphics g, Piece[] pPieces, int numPieces, bool thePostFX)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < numPieces; i++)
			{
				Piece piece;
				if ((piece = pPieces[i]) == null)
				{
					continue;
				}
				base.DrawPiecePre(g, piece);
				if (mIdToTileData.ContainsKey(piece.mId))
				{
					TileData tileData = mIdToTileData[piece.mId];
					if (mHypercubes.Contains(piece.mId))
					{
						DP_pHypercubes[num].piece = piece;
						DP_pHypercubes[num].tile = tileData;
						num++;
					}
					else if (IsSpecialPiece(piece))
					{
						DP_pSpecial[num2].piece = piece;
						DP_pSpecial[num2].tile = tileData;
						num2++;
					}
					else if (tileData.mPieceType == EDigPieceType.eDigPiece_Goal)
					{
						DP_pGoals[num3].piece = piece;
						DP_pGoals[num3].tile = tileData;
						num3++;
					}
					else if (tileData.mPieceType == EDigPieceType.eDigPiece_Block)
					{
						DP_pBlocks[num4].piece = piece;
						DP_pBlocks[num4].tile = tileData;
						num4++;
					}
					else if (tileData.mPieceType == EDigPieceType.eDigPiece_Artifact)
					{
						DP_pArtifacts[num5].piece = piece;
						DP_pArtifacts[num5].tile = tileData;
						num5++;
					}
				}
			}
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha == 0f)
			{
				return;
			}
			if (flag)
			{
				g.SetColor(Color.FAlpha(alpha));
				g.SetColorizeImages(true);
				g.PushColorMult();
			}
			int num6 = (int)GlobalMembersResourcesWP.ImgXOfs(1176);
			int num7 = (int)GlobalMembersResourcesWP.ImgYOfs(1176);
			g.PushState();
			g.Translate(GlobalMembers.S(-num6), GlobalMembers.S(-num7));
			g.ClearClipRect();
			for (int j = 0; j < num; j++)
			{
				Piece piece = DP_pHypercubes[j].piece;
			}
			for (int j = 0; j < num4; j++)
			{
				Piece piece = DP_pBlocks[j].piece;
				TileData tile = DP_pBlocks[j].tile;
				switch (tile.mStrength.value)
				{
				case 1:
				{
					int theId = (int)(1277 + tile.mRandCfg % 3);
					g.DrawImage(GlobalMembersResourcesWP.GetImageById(theId), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId) + piece.GetScreenY()));
					break;
				}
				case 2:
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR1, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1280) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1280) + piece.GetScreenY()));
					break;
				case 3:
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR2, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1281) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1281) + piece.GetScreenY()));
					break;
				case 4:
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR4, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1283) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1283) + piece.GetScreenY()));
					break;
				default:
					g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR3, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1282) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1282) + piece.GetScreenY()));
					break;
				}
			}
			for (int k = 0; k < 2; k++)
			{
				if (k == 1)
				{
					g.SetDrawMode(Graphics.DrawMode.Additive);
					g.SetColorizeImages(true);
				}
				for (int j = 0; j < num5; j++)
				{
					Piece piece = DP_pArtifacts[j].piece;
					TileData tile = DP_pArtifacts[j].tile;
					if (k == 1)
					{
						float num8 = (float)(GetExtraLuminosity(GetGridDepth() + piece.mRow) * GlobalMembers.M(0.3));
						if ((double)num8 <= GlobalMembers.M(0.1))
						{
							continue;
						}
						g.SetColor(Color.FAlpha(num8));
						SetAdditiveClipRect(g, (int)(0f - g.mTransX + (float)GlobalMembers.MS(0)), (int)(0f - g.mTransY));
					}
					ArtifactData artifactData = mArtifacts[tile.mArtifactId];
					g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mUnderlayImgId), (int)GlobalMembers.S(piece.GetScreenX() + (float)num6), (int)GlobalMembers.S(piece.GetScreenY() + (float)num7));
					g.SetScale(ConstantsWP.DIG_BOARD_ARTIFACT_SCALE, ConstantsWP.DIG_BOARD_ARTIFACT_SCALE, GlobalMembers.S(piece.GetScreenX() + 50f + (float)num6), GlobalMembers.S(piece.GetScreenY() + 50f + (float)num7));
					g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mImageId), (int)GlobalMembers.S(piece.GetScreenX() + (float)num6), (int)GlobalMembers.S(piece.GetScreenY() + (float)num7));
					g.SetScale(1f, 1f, GlobalMembers.S(piece.GetScreenX() + 50f + (float)num6), GlobalMembers.S(piece.GetScreenY() + 50f + (float)num7));
					g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mOverlayImgId), (int)GlobalMembers.S(piece.GetScreenX() + (float)num6), (int)GlobalMembers.S(piece.GetScreenY() + (float)num7));
				}
				for (int j = 0; j < num3; j++)
				{
					Piece piece = DP_pGoals[j].piece;
					TileData tile = DP_pGoals[j].tile;
					int num9 = tile.mStrength;
					if (mQuestBoard.mIsPerpetual)
					{
						g.Translate((int)GlobalMembers.S(piece.GetScreenX()), (int)GlobalMembers.S(piece.GetScreenY()));
						if (k == 1)
						{
							float num10 = (float)(GetExtraLuminosity(GetGridDepth() + piece.mRow) * ((tile.mDiamondFx != null) ? GlobalMembers.M(0.6) : GlobalMembers.M(0.3)));
							if ((double)num10 <= GlobalMembers.M(0.1))
							{
								continue;
							}
							g.SetColor(Color.FAlpha(num10));
							SetAdditiveClipRect(g, (int)(0f - g.mTransX + (float)GlobalMembers.MS(0)), (int)(0f - g.mTransY));
						}
						if (tile.mDiamondFx != null)
						{
							int theId2 = (int)(1277 + tile.mRandCfg % 3);
							g.DrawImage(GlobalMembersResourcesWP.GetImageById(theId2), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId2) + 0f), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId2) + 0f));
							int theX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(tile.mDiamondFx.mDirtImg));
							int theY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(tile.mDiamondFx.mDirtImg));
							if (k == 0)
							{
								g.SetColor(new Color(GetDarkenedRGBForRow(piece.mRow, false)));
							}
							g.SetColorizeImages(true);
							tile.mDiamondFx.DrawDirt(g, theX, theY);
							if (k == 0)
							{
								g.SetColor(new Color(-1));
							}
							theX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(tile.mDiamondFx.mBaseImg));
							theY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(tile.mDiamondFx.mBaseImg));
							tile.mDiamondFx.Draw(g, theX, theY, k == 0);
						}
						else
						{
							int theId3 = 0;
							switch (num9)
							{
							case 1:
								theId3 = 1224;
								break;
							case 2:
								theId3 = 1225;
								break;
							case 3:
								theId3 = 1226;
								break;
							}
							Graphics3D graphics3D = g.Get3D();
							if (graphics3D != null && tile.mGoldInnerFx != null)
							{
								graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKANDCOLOR);
							}
							Image imageById = GlobalMembersResourcesWP.GetImageById(theId3);
							int num11 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId3));
							int num12 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId3));
							g.DrawImage(imageById, num11, num12);
							if (graphics3D != null && tile.mGoldInnerFx != null)
							{
								g.Translate(num11 + imageById.GetWidth() / 2, num12 + imageById.GetHeight() / 2);
								graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_INSIDE);
								tile.mGoldInnerFx.Draw(g);
								graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
								graphics3D.ClearMask();
								g.Translate(-(num11 + imageById.GetWidth() / 2), -(num12 + imageById.GetHeight() / 2));
							}
						}
						g.Translate((int)(0f - GlobalMembers.S(piece.GetScreenX())), (int)(0f - GlobalMembers.S(piece.GetScreenY())));
					}
					else
					{
						int num13 = Math.Min(5, num9);
						int[,] array = new int[5, 3]
						{
							{ 1157, 1158, 1159 },
							{ 1160, 1161, 1162 },
							{ 1163, 1164, 1165 },
							{ 1166, 1167, 1168 },
							{ 1169, 1170, 1171 }
						};
						int theId4 = array[(int)(long)checked((IntPtr)unchecked(num13 - 1)), (int)(long)checked((IntPtr)unchecked((long)(tile.mRandCfg * 10232 % 3)))];
						g.DrawImage(GlobalMembersResourcesWP.GetImageById(theId4), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId4) + piece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId4) + piece.GetScreenY()));
					}
				}
			}
			g.PopState();
			if (flag)
			{
				g.PopColorMult();
				g.SetColorizeImages(false);
			}
		}

		public override void DrawPiecePre(Graphics g, Piece thePiece, float theScale)
		{
			base.DrawPiecePre(g, thePiece, theScale);
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha == 0f)
			{
				return;
			}
			if (flag)
			{
				g.SetColor(Color.FAlpha(alpha));
				g.SetColorizeImages(true);
				g.PushColorMult();
			}
			if (thePiece.IsFlagSet(65536u) && mIdToTileData.ContainsKey(thePiece.mId))
			{
				TileData tileData = mIdToTileData[thePiece.mId];
				int num = tileData.mStrength;
				int num2 = (int)GlobalMembersResourcesWP.ImgXOfs(1176);
				int num3 = (int)GlobalMembersResourcesWP.ImgYOfs(1176);
				g.PushState();
				g.Translate(GlobalMembers.S(-num2), GlobalMembers.S(-num3));
				for (int i = 0; i < 3; i++)
				{
					if (i > 0)
					{
						if (!mQuestBoard.mIsPerpetual || GlobalMembers.M(0) != 0)
						{
							continue;
						}
						switch (i)
						{
						case 1:
						{
							if (tileData.mPieceType != EDigPieceType.eDigPiece_Goal && tileData.mPieceType != 0)
							{
								continue;
							}
							float num4 = (float)(GetExtraLuminosity(GetGridDepth() + thePiece.mRow) * GlobalMembers.M(0.3));
							if (tileData.mDiamondFx != null)
							{
								num4 *= GlobalMembers.M(0.2f);
							}
							if ((double)num4 <= GlobalMembers.M(0.1))
							{
								continue;
							}
							g.SetDrawMode(Graphics.DrawMode.Additive);
							g.SetColorizeImages(true);
							g.SetColor(Color.FAlpha(num4));
							SetAdditiveClipRect(g, (int)(0f - g.mTransX + (float)GlobalMembers.MS(0)), (int)(0f - g.mTransY));
							break;
						}
						case 2:
							g.SetDrawMode(Graphics.DrawMode.Normal);
							g.SetColorizeImages(true);
							SetShadowClipRect(g, (int)(0f - g.mTransX + (float)GlobalMembers.MS(0)), (int)(0f - g.mTransY));
							g.SetDrawMode(Graphics.DrawMode.Normal);
							g.SetColorizeImages(true);
							if (tileData.mPieceType == EDigPieceType.eDigPiece_Goal || tileData.mPieceType == EDigPieceType.eDigPiece_Artifact)
							{
								g.SetColor(new Color(GlobalMembers.M(0), (tileData.mDiamondFx != null) ? GlobalMembers.M(120) : GlobalMembers.M(70)));
							}
							else
							{
								g.SetColor(new Color(GlobalMembers.M(0), GlobalMembers.M(120)));
							}
							break;
						}
					}
					else
					{
						g.ClearClipRect();
					}
					if (mHypercubes.Contains(thePiece.mId))
					{
						g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_HYPERCUBE, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1230) + (thePiece.GetScreenX() + (float)GlobalMembers.M(0))), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1230) + (thePiece.GetScreenY() + (float)GlobalMembers.M(0))));
					}
					else
					{
						if (IsSpecialPiece(thePiece))
						{
							continue;
						}
						if (tileData.mPieceType == EDigPieceType.eDigPiece_Goal)
						{
							if (mQuestBoard.mIsPerpetual)
							{
								g.Translate((int)GlobalMembers.S(thePiece.GetScreenX()), (int)GlobalMembers.S(thePiece.GetScreenY()));
								if (tileData.mDiamondFx != null)
								{
									int theId = (int)(1277 + tileData.mRandCfg % 3);
									g.DrawImage(GlobalMembersResourcesWP.GetImageById(theId), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId) + 0f), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId) + 0f));
									int theX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(tileData.mDiamondFx.mDirtImg));
									int theY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(tileData.mDiamondFx.mDirtImg));
									if (i == 0)
									{
										g.SetColor(new Color(GetDarkenedRGBForRow(thePiece.mRow, false)));
									}
									g.SetColorizeImages(true);
									tileData.mDiamondFx.DrawDirt(g, theX, theY);
									if (i == 0)
									{
										g.SetColor(new Color(-1));
									}
									theX = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(tileData.mDiamondFx.mBaseImg));
									theY = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(tileData.mDiamondFx.mBaseImg));
									tileData.mDiamondFx.Draw(g, theX, theY, false);
								}
								else
								{
									int theId2 = 0;
									switch (num)
									{
									case 1:
										theId2 = 1224;
										break;
									case 2:
										theId2 = 1225;
										break;
									case 3:
										theId2 = 1226;
										break;
									}
									Graphics3D graphics3D = g.Get3D();
									if (graphics3D != null && tileData.mGoldInnerFx != null)
									{
										graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_WRITE_MASKANDCOLOR);
									}
									Image imageById = GlobalMembersResourcesWP.GetImageById(theId2);
									int num5 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId2));
									int num6 = (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId2));
									g.DrawImage(imageById, num5, num6);
									if (graphics3D != null && tileData.mGoldInnerFx != null)
									{
										g.Translate(num5 + imageById.GetWidth() / 2, num6 + imageById.GetHeight() / 2);
										graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_TEST_INSIDE);
										tileData.mGoldInnerFx.Draw(g);
										graphics3D.SetMasking(Graphics3D.EMaskMode.MASKMODE_NONE);
										graphics3D.ClearMask();
										g.Translate(-(num5 + imageById.GetWidth() / 2), -(num6 + imageById.GetHeight() / 2));
									}
								}
								g.Translate((int)(0f - GlobalMembers.S(thePiece.GetScreenX())), (int)(0f - GlobalMembers.S(thePiece.GetScreenY())));
							}
							else
							{
								int num7 = Math.Min(5, num);
								int idByStringId = (int)GlobalMembersResourcesWP.GetIdByStringId($"IMAGE_QUEST_DIG_BOARD_NUGGET{num7}_{1 + tileData.mRandCfg * 10232 % 3}");
								g.DrawImage(GlobalMembersResourcesWP.GetImageById(idByStringId), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(idByStringId) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(idByStringId) + thePiece.GetScreenY()));
							}
						}
						else if (tileData.mPieceType == EDigPieceType.eDigPiece_Block)
						{
							switch (num)
							{
							case 1:
							{
								int theId3 = (int)(1277 + tileData.mRandCfg % 3);
								g.DrawImage(GlobalMembersResourcesWP.GetImageById(theId3), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(theId3) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(theId3) + thePiece.GetScreenY()));
								break;
							}
							case 2:
								g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR1, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1280) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1280) + thePiece.GetScreenY()));
								break;
							case 3:
								g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR2, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1281) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1281) + thePiece.GetScreenY()));
								break;
							case 4:
								g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR4, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1283) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1283) + thePiece.GetScreenY()));
								break;
							case 5:
								g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR3, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1282) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1282) + thePiece.GetScreenY()));
								break;
							default:
								g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_STR3, (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(1282) + thePiece.GetScreenX()), (int)GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(1282) + thePiece.GetScreenY()));
								break;
							}
						}
						else if (tileData.mPieceType == EDigPieceType.eDigPiece_Artifact)
						{
							int num8 = (int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID);
							int num9 = (int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID);
							ArtifactData artifactData = mArtifacts[tileData.mArtifactId];
							g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mUnderlayImgId), (int)GlobalMembers.S(thePiece.GetScreenX() + (float)num8), (int)GlobalMembers.S(thePiece.GetScreenY() + (float)num9));
							g.SetScale(ConstantsWP.DIG_BOARD_ARTIFACT_SCALE, ConstantsWP.DIG_BOARD_ARTIFACT_SCALE, (int)GlobalMembers.S(thePiece.GetScreenX() + 50f + (float)num8), (int)GlobalMembers.S(thePiece.GetScreenY() + 50f + (float)num9));
							g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mImageId), (int)GlobalMembers.S(thePiece.GetScreenX() + (float)num8), (int)GlobalMembers.S(thePiece.GetScreenY() + (float)num9));
							g.SetScale(1f, 1f, GlobalMembers.S(thePiece.GetScreenX() + 50f + (float)num8), GlobalMembers.S(thePiece.GetScreenY() + 50f + (float)num9));
							g.DrawImage(GlobalMembersResourcesWP.GetImageById(artifactData.mOverlayImgId), (int)GlobalMembers.S(thePiece.GetScreenX() + (float)num8), (int)GlobalMembers.S(thePiece.GetScreenY() + (float)num9));
						}
					}
				}
				g.PopState();
			}
			if (flag)
			{
				g.PopColorMult();
				g.SetColorizeImages(false);
			}
		}

		public override void DrawPiecesPost(Graphics g, bool thePostFX)
		{
			base.DrawPiecesPost(g, thePostFX);
			if (mCvScrollY.IsDoingCurve())
			{
				for (int i = 0; i < mMovingPieces.Count; i++)
				{
					if (mMovingPieces[i] != null)
					{
						mQuestBoard.DrawPiece(g, mMovingPieces[i]);
					}
				}
			}
			g.ClearClipRect();
			if (!mQuestBoard.mIsPerpetual)
			{
				mWantTopFrame = true;
				DrawBottomFrame(g);
				mWantTopFrame = false;
			}
			if (mQuestBoard.mIsPerpetual)
			{
				return;
			}
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < mGoldFx.Count; k++)
				{
					mGoldFx[k].Draw(g, j);
				}
			}
		}

		public override void Draw(Graphics g)
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				return;
			}
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha != 0f)
			{
				if (flag)
				{
					g.SetColor(Color.FAlpha(alpha));
					g.SetColorizeImages(true);
					g.PushColorMult();
				}
				GlobalMembers.M(1);
				g.SetColor(new Color(-1));
				if (flag)
				{
					g.PopColorMult();
					g.SetColorizeImages(false);
				}
			}
		}

		public override bool DrawScore(Graphics g)
		{
			return mQuestBoard.mIsPerpetual;
		}

		public override void DrawGameElements(Graphics g)
		{
			base.DrawGameElements(g);
			float alpha = mQuestBoard.GetAlpha();
			bool flag = (double)alpha < 1.0;
			if (alpha == 0f)
			{
				return;
			}
			if (flag)
			{
				g.SetColor(Color.FAlpha(alpha));
				g.SetColorizeImages(true);
				g.PushColorMult();
			}
			List<Piece> list = new List<Piece>();
			new Color(GlobalMembers.M(4210752), GlobalMembers.M(255));
			new Color(GlobalMembers.M(10526880), GlobalMembers.M(255));
			Color color = default(Color);
			int num = (int)GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID);
			int num2 = (int)GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID);
			g.PushState();
			g.ClipRect(GlobalMembers.S(mQuestBoard.CallBoardGetBoardX() + mBoardOffsetXExtra), GlobalMembers.S(mQuestBoard.CallBoardGetBoardY() + mBoardOffsetYExtra), GlobalMembers.S(800), GlobalMembers.S(800));
			int num3 = 0;
			g.Translate(GlobalMembers.S(-num), GlobalMembers.S(num3 - num2));
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 1; j++)
				{
					bool flag2 = false;
					if (i == 1 || i == 4)
					{
						if (list.Count == 0)
						{
							continue;
						}
						for (int k = 0; k < list.Count; k++)
						{
							Piece piece = list[k];
							TileData tileDatum = mIdToTileData[piece.mId];
							if (i == 1)
							{
								g.DrawImage(GlobalMembersResourcesWP.GetImageById(1227), (int)GlobalMembers.S(piece.GetScreenX() + GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_GRASS_ID)), (int)GlobalMembers.S(piece.GetScreenY() + GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_GRASS_ID)));
								continue;
							}
							for (int l = 0; l < DrawGameElements_DrawGrassLR_neighbors.GetLength(0); l++)
							{
								int num4 = piece.mCol + DrawGameElements_DrawGrassLR_neighbors[l, 0];
								int num5 = piece.mRow + DrawGameElements_DrawGrassLR_neighbors[l, 1];
								bool flag3 = false;
								if (num4 >= 0 && num4 < 8 && num5 >= 0 && num5 < 8)
								{
									Piece piece2 = mQuestBoard.mBoard[num5, num4];
									if (piece2 != null)
									{
										flag3 = !piece2.IsFlagSet(65536u);
									}
								}
								else
								{
									flag3 = true;
								}
								if (flag3)
								{
									g.DrawImage(GlobalMembersResourcesWP.GetImageById(DrawGameElements_DrawGrassLR_images[l]), (int)GlobalMembers.S(piece.GetScreenX() + GlobalMembersResourcesWP.ImgXOfs(DrawGameElements_DrawGrassLR_images[l])), (int)GlobalMembers.S(piece.GetScreenY() + GlobalMembersResourcesWP.ImgYOfs(DrawGameElements_DrawGrassLR_images[l])));
								}
							}
						}
						continue;
					}
					Piece[,] mBoard = mQuestBoard.mBoard;
					foreach (Piece piece3 in mBoard)
					{
						if (piece3 == null)
						{
							continue;
						}
						if (GlobalMembers.M(0) != 0 && piece3.mCol == GlobalMembers.M(6))
						{
							int mRow = piece3.mRow;
							GlobalMembers.M(6);
						}
						if (!piece3.IsFlagSet(65536u))
						{
							continue;
						}
						TileData tileData = mIdToTileData[piece3.mId];
						g.SetColor(new Color(GetDarkenedRGBForRow(piece3.mRow, flag2)));
						if (flag2)
						{
							color = g.mColor;
						}
						g.SetColorizeImages(g.mColor != Color.White);
						switch (i)
						{
						case 0:
							if (tileData.mIsTopTile)
							{
								list.Add(piece3);
							}
							g.DrawImage(GlobalMembersResourcesWP.IMAGE_QUEST_DIG_BOARD_CENTER_FULL, (int)GlobalMembers.S(piece3.GetScreenX() + GlobalMembersResourcesWP.ImgXOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID)), (int)GlobalMembers.S(piece3.GetScreenY() + GlobalMembersResourcesWP.ImgYOfs(ResourceId.IMAGE_QUEST_DIG_BOARD_CENTER_FULL_ID)));
							g.SetColorizeImages(flag);
							g.SetColor(new Color(-1));
							break;
						case 2:
						case 3:
						{
							Color mColor = g.mColor;
							for (int num6 = 0; num6 < DrawGameElements_neighbors.GetLength(0); num6++)
							{
								if (tileData.mIsTopTile && num6 == 3)
								{
									continue;
								}
								int num7 = piece3.mCol + DrawGameElements_neighbors[num6, 0];
								int num8 = piece3.mRow + DrawGameElements_neighbors[num6, 1];
								if (num7 < 0 || num7 >= 8 || num8 < 0 || num8 >= 8)
								{
									continue;
								}
								Piece piece4 = mQuestBoard.mBoard[num8, num7];
								if (piece4 == null || !piece4.IsFlagSet(65536u))
								{
									g.DrawImage(GlobalMembersResourcesWP.GetImageById(DrawGameElements_images[num6]), (int)GlobalMembers.S(piece3.GetScreenX() + (float)(100 * DrawGameElements_neighbors[num6, 0]) + GlobalMembersResourcesWP.ImgXOfs(DrawGameElements_images[num6])), (int)GlobalMembers.S(piece3.GetScreenY() + (float)(100 * DrawGameElements_neighbors[num6, 1]) + GlobalMembersResourcesWP.ImgYOfs(DrawGameElements_images[num6])));
									if (!flag2)
									{
										g.SetColor(new Color(-1));
									}
									else
									{
										g.SetColor(new Color(GlobalMembers.M(16777215)));
									}
									int num9 = -1;
									num9 = ((i != 2) ? DrawGameElements_imagesHighlights[num6] : DrawGameElements_imagesHighlightShadows[num6]);
									if (num9 != -1)
									{
										g.DrawImage(GlobalMembersResourcesWP.GetImageById(num9), (int)GlobalMembers.S(piece3.GetScreenX() + (float)(100 * DrawGameElements_neighbors[num6, 0]) + GlobalMembersResourcesWP.ImgXOfs(num9)), (int)GlobalMembers.S(piece3.GetScreenY() + (float)(100 * DrawGameElements_neighbors[num6, 1]) + GlobalMembersResourcesWP.ImgYOfs(num9)));
									}
									if (!flag2)
									{
										g.SetColor(mColor);
									}
									else
									{
										g.SetColor(color);
									}
								}
							}
							break;
						}
						}
					}
					g.SetColorizeImages(flag);
					g.SetColor(new Color(-1));
				}
			}
			g.PopState();
			g.ClearClipRect();
			mDustFXManager.Draw(g);
			g.SetDrawMode(Graphics.DrawMode.Normal);
			if (flag)
			{
				g.PopColorMult();
				g.SetColorizeImages(false);
			}
		}

		public override void DrawGameElementsPost(Graphics g)
		{
			mQuestBoard.CallBoardDrawUI(g);
		}

		public void DrawDigBarAnim(Graphics g, bool theDrawMega)
		{
			if (!DrawDigBarAnim_made)
			{
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_CLEAR_ANIM_X, DrawDigBarAnim_cvClearAnimX);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_CLEAR_ANIM_SCALE, DrawDigBarAnim_cvClearAnimScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_CLEAR_ANIM_ALPHA, DrawDigBarAnim_cvClearAnimAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_CLEAR_ANIM_COLOR_CYCLE, DrawDigBarAnim_cvClearAnimColorCycle);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_ALL_CLEAR_ANIM_X, DrawDigBarAnim_cvAllClearAnimX);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_ALL_CLEAR_ANIM_SCALE, DrawDigBarAnim_cvAllClearAnimScale);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_ALL_CLEAR_ANIM_ALPHA, DrawDigBarAnim_cvAllClearAnimAlpha);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DRAW_DIG_BAR_ALL_CLEAR_ANIM_COLOR_CYCLE, DrawDigBarAnim_cvAllClearAnimColorCycle);
				DrawDigBarAnim_made = true;
			}
			if (!theDrawMega)
			{
				if (GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.IsActive())
				{
					g.PushState();
					g.Translate(GlobalMembers.S(GetBoardX() + 400 + GlobalMembers.M(0)), GlobalMembers.S(mQuestBoard.CallBoardGetBoardY() + 600 + GlobalMembers.M(0)));
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.mDrawTransform.LoadIdentity();
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.mDrawTransform.Scale(GlobalMembers.MS(0.9f), GlobalMembers.S(1f));
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.Draw(g);
					g.PopState();
				}
				if (mClearedAnimAtTick > 0 && (double)((int)mUpdateCnt - mClearedAnimAtTick) < DrawDigBarAnim_cvClearAnimAlpha.mInMax * 100.0)
				{
					DrawScrollingDigLineText(g, GlobalMembersResources.FONT_HUGE, ConstantsWP.DIG_BOARD_CLEARED_TEXT_EXTRA_Y, GlobalMembers._ID("CLEARED!", 542), (int)mUpdateCnt - mClearedAnimAtTick, DrawDigBarAnim_cvClearAnimX, DrawDigBarAnim_cvClearAnimScale, DrawDigBarAnim_cvClearAnimAlpha, DrawDigBarAnim_cvClearAnimColorCycle, false, GlobalMembers.M(0));
				}
			}
			else
			{
				if (GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.IsActive())
				{
					g.PushState();
					g.Translate(GlobalMembers.S(GetBoardX() + 400 + GlobalMembers.M(0)), GlobalMembers.S(mQuestBoard.CallBoardGetBoardY() + 800 + GlobalMembers.M(0)));
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.mDrawTransform.LoadIdentity();
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.mDrawTransform.Scale(GlobalMembers.MS(1f), GlobalMembers.S(1f));
					GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.Draw(g);
					g.PopState();
				}
				if (mAllClearedAnimAtTick > 0 && (double)((int)mUpdateCnt - mAllClearedAnimAtTick) < DrawDigBarAnim_cvAllClearAnimAlpha.mInMax * 100.0)
				{
					DrawScrollingDigLineText(g, GlobalMembersResources.FONT_HUGE, ConstantsWP.DIG_BOARD_ALL_CLEAR_TEXT_EXTRA_Y, GlobalMembers._ID("ALL CLEAR", 544), (int)mUpdateCnt - mAllClearedAnimAtTick, DrawDigBarAnim_cvAllClearAnimX, DrawDigBarAnim_cvAllClearAnimScale, DrawDigBarAnim_cvAllClearAnimAlpha, DrawDigBarAnim_cvAllClearAnimColorCycle, true, g.Is3D() ? GlobalMembers.M(0) : GlobalMembers.M(0));
				}
			}
		}

		public void GoldAnimDoPoints(ETreasureType theType, int theVal)
		{
			if (mQuestBoard.mIsPerpetual)
			{
				mQuestBoard.mLevelPointsTotal += theVal;
				mTextFlashTicks = GlobalMembers.M(120);
				mTreasureEarnings[(int)theType] += theVal;
			}
			else
			{
				mQuestBoard.mLevelPointsTotal++;
				mQuestBoard.mPoints++;
			}
		}

		public void DrawBottomFrame(Graphics g)
		{
			mQuestBoard.CallBoardDrawBottomFrame(g);
		}

		public bool IsSpecialPiece(Piece thePiece)
		{
			return thePiece.IsFlagSet(4103u);
		}

		public override bool DeletePiece(Piece thePiece)
		{
			if (mQuestBoard.mInLoadSave)
			{
				return true;
			}
			int num = mHypercubes.IndexOf(thePiece.mId);
			if (num >= 0)
			{
				mHypercubes.RemoveAt(num);
				mQuestBoard.Hypercubeify(thePiece);
				int[] array = new int[7];
				for (int i = 0; i < 7; i++)
				{
					array[i] = 0;
				}
				Piece[,] mBoard = mQuestBoard.mBoard;
				foreach (Piece piece in mBoard)
				{
					if (piece != null && piece.mColor >= 0 && piece.mColor < 7)
					{
						array[piece.mColor]++;
					}
				}
				int num2 = 0;
				for (int l = 0; l < 7; l++)
				{
					if (array[l] > array[num2])
					{
						num2 = l;
					}
				}
				thePiece.ClearFlag(65536u);
				thePiece.mIsElectrocuting = false;
				thePiece.mElectrocutePercent = 0f;
				mQuestBoard.DoHypercube(thePiece, num2);
				return false;
			}
			int num3 = mMovingPieces.IndexOf(thePiece);
			if (num3 >= 0)
			{
				mMovingPieces[num3] = null;
				return true;
			}
			if (mIdToTileData.ContainsKey(thePiece.mId))
			{
				TileData tileData = mIdToTileData[thePiece.mId];
				if (thePiece.IsFlagSet(65536u) && ((int)tileData.mStrength > 1 || tileData.mPieceType == EDigPieceType.eDigPiece_Artifact))
				{
					mQuestBoard.TallyPiece(thePiece, true);
					if (!mIdToTileData.ContainsKey(thePiece.mId) || !tileData.mIsDeleting)
					{
						return false;
					}
				}
				if (tileData.mBlingPIFx != null)
				{
					tileData.mBlingPIFx.Stop();
					if (tileData.mBlingPIFx.mRefCount > 0)
					{
						tileData.mBlingPIFx.mRefCount--;
					}
					tileData.mBlingPIFx = null;
				}
				mQuestBoard.CallBoardDeletePiece(thePiece);
				if (tileData.mDiamondFx != null)
				{
					tileData.mDiamondFx.Dispose();
				}
				mIdToTileData.Remove(thePiece.mId);
				return false;
			}
			return true;
		}

		public override void PieceTallied(Piece thePiece)
		{
			mGoldRushTipPieceId = -1;
			new List<Piece>();
			if (!thePiece.IsFlagSet(65536u))
			{
				mVisited[thePiece.mRow][thePiece.mCol] = true;
				if (!thePiece.mIsExploding || thePiece.mColor == -1 || ((ulong)thePiece.mExplodeSourceFlags & 5uL) == 0)
				{
					CheckPiece item = new CheckPiece(thePiece.mCol, thePiece.mRow, thePiece.mColor == -1 || ((ulong)thePiece.mExplodeSourceFlags & 2uL) != 0, thePiece.mMoveCreditId);
					mCheckPieces.Add(item);
				}
			}
			base.PieceTallied(thePiece);
		}

		public override void TallyPiece(Piece thePiece, bool thePieceDestroyed)
		{
			bool flag = false;
			if (thePiece.IsFlagSet(65536u) && !IsSpecialPiece(thePiece) && !mQuestBoard.mInLoadSave)
			{
				bool flag2 = false;
				bool theIsSpecialGem = false;
				if (thePiece.mIsExploding && ((ulong)thePiece.mExplodeSourceFlags & 7uL) != 0)
				{
					int mCol = thePiece.mCol;
					int mRow = thePiece.mRow;
					bool value = mVisited[mRow][mCol];
					theIsSpecialGem = true;
					flag = TriggerDigPiece(null, thePiece, theIsSpecialGem, !thePieceDestroyed);
					mVisited[mRow][mCol] = value;
					flag2 = true;
				}
				if (!flag)
				{
					TriggerDigPiece(null, thePiece, theIsSpecialGem, !thePieceDestroyed, !flag2);
				}
			}
			base.TallyPiece(thePiece, thePieceDestroyed);
		}

		public void TriggerDigCoin(Piece thePiece, int theId)
		{
			GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_TREASUREFIND);
			TileData theData = mIdToTileData[thePiece.mId];
			if (mIdToTileData.ContainsKey(thePiece.mId))
			{
				int num = 1147 + BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(9);
				GoldCollectEffect goldCollectEffect = new GoldCollectEffect(this, theData);
				goldCollectEffect.mTreasureType = ETreasureType.eTreasure_Gold;
				goldCollectEffect.mVal = 1;
				goldCollectEffect.mImageId = num;
				goldCollectEffect.mSrcImageId = num;
				goldCollectEffect.mStartPoint = new Point((int)thePiece.CX(), (int)thePiece.CY());
				goldCollectEffect.mTargetPoint = new Point(ConstantsWP.DIG_BOARD_SCORE_CENTER_X, ConstantsWP.DIG_BOARD_SCORE_BTM_Y);
				goldCollectEffect.mTimeMod = GlobalMembersUtils.GetRandFloatU() * GlobalMembers.M(0.5f);
				goldCollectEffect.mIsNugget = true;
				goldCollectEffect.mStopGlowAtPct = GlobalMembers.M(0.9);
				goldCollectEffect.mLayerOnTop = true;
				goldCollectEffect.mGlowRGB = GlobalMembers.M(16118660);
				goldCollectEffect.Init();
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SCALE_CV_DIG_COIN, goldCollectEffect.mScaleCv);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_SPLINE_INTERP_DIG_COIN, goldCollectEffect.mSplineInterp);
				GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_PARTICLE_EMIT_OVER_TIME_DIG_COIN, goldCollectEffect.mParticleEmitOverTime);
				mGoldFx.Add(goldCollectEffect);
			}
		}

		public override bool IsGameSuspended()
		{
			if (!mCvScrollY.IsDoingCurve())
			{
				return base.IsGameSuspended();
			}
			return true;
		}

		public override void Update()
		{
			if (mDigInProgress && !mClearedAnimPlayed && !mAllClearAnimPlayed && !mCvScrollY.IsDoingCurve())
			{
				mDigInProgress = false;
			}
			if (mQuestBoard.mIsPerpetual)
			{
				mQuestBoard.mBoardColors[0] = new Color(77, 61, 40);
				mQuestBoard.mBoardColors[1] = new Color(88, 72, 51);
			}
			if (mClearedAnimPlayed)
			{
				GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.Update();
			}
			if (mAllClearAnimPlayed)
			{
				GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.Update();
			}
			int num = 0;
			while (num < mGoldFx.Count)
			{
				if (mGoldFx[num].mDeleteMe)
				{
					if (mGoldFx[num] != null)
					{
						mGoldFx[num].Dispose();
					}
					mGoldFx.RemoveAt(num);
				}
				else
				{
					mGoldFx[num++].Update();
				}
			}
			if (GlobalMembers.gApp.GetDialog(19) == null)
			{
				mInitPushAnimCv.IncInVal();
			}
			foreach (int key in mIdToTileData.Keys)
			{
				TileData tileData = mIdToTileData[key];
				if (tileData.mDiamondFx != null)
				{
					tileData.mDiamondFx.Update();
				}
				if (tileData.mGoldInnerFx != null)
				{
					tileData.mGoldInnerFx.Update();
				}
				if (tileData.mBlingPIFx != null)
				{
					Piece pieceById = mQuestBoard.GetPieceById(key);
					if (pieceById != null)
					{
						tileData.mBlingPIFx.SetActive(pieceById.mRow < 6);
					}
				}
			}
			mGoldPointsFXManager.Update();
			mUpdateCnt.value++;
			if (mCvScrollY.IsDoingCurve())
			{
				UpdateShift();
			}
			else
			{
				((DigBoard)mQuestBoard).SetCogsAnim(false);
			}
			foreach (List<bool> item in mVisited)
			{
				for (int i = 0; i < item.Count; i++)
				{
					item[i] = false;
				}
			}
			base.Update();
			if (mTextFlashTicks > 0)
			{
				mTextFlashTicks--;
			}
			foreach (CheckPiece mCheckPiece in mCheckPieces)
			{
				int mCol = mCheckPiece.mCol;
				int mRow = mCheckPiece.mRow;
				int mMoveCreditId = mCheckPiece.mMoveCreditId;
				bool mIsHyperCube = mCheckPiece.mIsHyperCube;
				for (int j = 0; j < 4; j++)
				{
					Piece pieceAtRowCol = mQuestBoard.GetPieceAtRowCol(mRow + Update_neighbors[j, 1], mCol + Update_neighbors[j, 0]);
					if (pieceAtRowCol == null)
					{
						continue;
					}
					mQuestBoard.SetMoveCredit(pieceAtRowCol, mMoveCreditId);
					bool flag = false;
					bool theIsSpecialGem = false;
					bool flag2 = false;
					if (mIsHyperCube)
					{
						theIsSpecialGem = true;
						flag = TriggerDigPiece(mQuestBoard.GetPieceAtRowCol(mRow, mCol), pieceAtRowCol, theIsSpecialGem, true);
						flag2 = true;
					}
					if (!flag)
					{
						pieceAtRowCol = mQuestBoard.GetPieceAtRowCol(mRow + Update_neighbors[j, 1], mCol + Update_neighbors[j, 0]);
						if (pieceAtRowCol != null)
						{
							TriggerDigPiece(mQuestBoard.GetPieceAtRowCol(mRow, mCol), pieceAtRowCol, theIsSpecialGem, true, !flag2, true);
						}
					}
				}
			}
			if (mQuestBoard.GetTimeLimit() - mQuestBoard.GetTicksLeft() / 100 >= GlobalMembers.M(1) && mGoldRushTipPieceId != -1)
			{
				int num2 = (mQuestBoard.mIsPerpetual ? GlobalMembers.M(3) : GlobalMembers.M(5));
				int num3 = (mQuestBoard.mIsPerpetual ? GlobalMembers.M(2) : GlobalMembers.M(4));
				Piece piece = mQuestBoard.mBoard[num3, num2];
				if (piece != null && piece.mId == mGoldRushTipPieceId)
				{
					if (mQuestBoard.mGoAnnouncementDone)
					{
						mQuestBoard.ShowHint(piece, true);
						mQuestBoard.mGoAnnouncementDone = false;
					}
					if ((double)mQuestBoard.mVisPausePct == 0.0 && mQuestBoard.AllowTooltips())
					{
						GlobalMembers.gApp.mTooltipManager.RequestTooltip(mQuestBoard, mTutorialHeader, mTutorialText, new Point((int)GlobalMembers.S(piece.CX()), (int)GlobalMembers.S(piece.CY())), GlobalMembers.MS(320), 1, 0, null, null, 0, -1);
					}
				}
			}
			mCheckPieces.Clear();
			if (!mCvScrollY.IsDoingCurve())
			{
				if (mBlackrockTipPieceId != -1 && mQuestBoard.WantsTutorial(16))
				{
					Piece pieceById2 = mQuestBoard.GetPieceById(mBlackrockTipPieceId);
					if (pieceById2 != null)
					{
						mQuestBoard.DeferTutorialDialog(16, pieceById2);
					}
					mBlackrockTipPieceId = -1;
				}
				if (mMovingPieces.Count != 0)
				{
					int count = mMovingPieces.Count;
					for (int k = 0; k < count; k++)
					{
						Piece piece2;
						if ((piece2 = mMovingPieces[k]) != null)
						{
							mQuestBoard.mPreFXManager.FreePieceEffect(piece2);
							mQuestBoard.mPostFXManager.FreePieceEffect(piece2);
							piece2.release();
						}
					}
					mMovingPieces.Clear();
				}
			}
			mDustFXManager.Update();
			mMessageFXManager.Update();
			Piece[,] mBoard = mQuestBoard.mBoard;
			foreach (Piece piece3 in mBoard)
			{
				if (piece3 != null && piece3.IsFlagSet(65536u))
				{
					piece3.mShakeScale = 0f;
				}
			}
			bool flag3 = false;
			if (mAllowDescent && !mCvScrollY.IsDoingCurve())
			{
				flag3 = CheckNeedScroll(false);
			}
			if (flag3)
			{
				if (!mDigBarFlash.IsDoingCurve())
				{
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DIG_BAR_FLASH, mDigBarFlash);
					mDigBarFlash.SetMode(1);
				}
				if (mQuestBoard.IsBoardStill() && mQuestBoard.mQueuedMoveVector.Count == 0 && mQuestBoard.mDeferredTutorialVector.Count == 0 && GlobalMembers.gApp.GetDialog(18) == null)
				{
					mQuestBoard.mNeverAllowCascades = mDefaultNeverAllowCascades;
					mForceScroll = false;
					mInMegaDig = CheckNeedScroll(true);
					if (mGridDepth == 2.0 || mInMegaDig)
					{
						mQuestBoard.mPowerGemThreshold = mPowerGemThresholdDepth40;
					}
					else if (mGridDepth == 0.0)
					{
						mQuestBoard.mPowerGemThreshold = mPowerGemThresholdDepth20;
					}
					mQuestBoard.SetColorCount(6);
					((DigBoard)mQuestBoard).DoTimeBonus(mInMegaDig);
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DIG);
					((DigBoard)mQuestBoard).SetCogsAnim(true);
					mGridDepth += (int)(double)mCvScrollY * GetDigCountPerScroll();
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_CV_SCROLL_Y, mCvScrollY);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_CV_SHAKEY, mCvShakey);
					mCvShakey.mLinkedVal = mCvScrollY;
				}
				else
				{
					mQuestBoard.mNeverAllowCascades = true;
					if (mAllClearedAnimAtTick > 0 && (int)mUpdateCnt - mAllClearedAnimAtTick >= GlobalMembers.M(100))
					{
						mAllClearedAnimAtTick++;
					}
				}
				if (!IsDoubleHypercubeActive())
				{
					if (CheckNeedScroll(true))
					{
						if (mAllClearedAnimAtTick <= 0)
						{
							mDigInProgress = true;
							mAllClearedAnimAtTick = mUpdateCnt;
							mAllClearAnimPlayed = true;
							GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT_MEGA.ResetAnim();
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DIG_LINE_HIT_MEGA, 0, GlobalMembers.M(1.0));
						}
					}
					else if (mAllClearedAnimAtTick <= 0 && mClearedAnimAtTick <= 0)
					{
						mDigInProgress = true;
						mClearedAnimAtTick = mUpdateCnt;
						mClearedAnimPlayed = true;
						GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_DIG_LINE_HIT.ResetAnim();
						GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DIG_LINE_HIT, 0, GlobalMembers.M(1.0));
					}
				}
			}
			else
			{
				mQuestBoard.mNeverAllowCascades = mDefaultNeverAllowCascades;
				if (GlobalMembers.M(1) != 0 && mDigBarFlash.IsDoingCurve())
				{
					mDigBarFlash.SetMode(0);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_DIG_BAR_FLASH_OFF, mDigBarFlash);
					mDigBarFlash.Intercept(string.Empty);
				}
				mDigBarFlashCount = 0;
				if (mClearedAnimAtTick > 0 && (int)mUpdateCnt - mClearedAnimAtTick > GlobalMembers.M(300))
				{
					mClearedAnimAtTick = -1;
					mClearedAnimPlayed = false;
				}
				if (mAllClearedAnimAtTick > 0 && (int)mUpdateCnt - mAllClearedAnimAtTick > GlobalMembers.M(600))
				{
					mAllClearedAnimAtTick = -1;
					mAllClearAnimPlayed = false;
				}
			}
			double num4 = mDigBarFlash;
			mDigBarFlash.IncInVal();
			if (num4 < (double)mDigBarFlash && num4 < GlobalMembers.M(0.1) && (double)mDigBarFlash >= GlobalMembers.M(0.1))
			{
				mDigBarFlashCount++;
				if (mDigBarFlashCount != GlobalMembers.M(1) && mDigBarFlashCount >= GlobalMembers.M(3))
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DIG_NOTIFY, 0, GlobalMembers.M(1.0));
				}
			}
			((DigBoard)mQuestBoard).UpdateCogsAnim();
			float num5 = 1f;
			num5 = -1f;
			mQuestBoard.mBoardUIOffsetY = (int)((float)GetBoardScrollOffsetY() * num5);
			if (mCvShakey.IsDoingCurve() && (int)(double)mCvShakey > 0)
			{
				mQuestBoard.mOffsetX = BejeweledLivePlus.Misc.Common.Rand() % (int)(double)mCvShakey - (int)(double)mCvShakey / 2;
				mQuestBoard.mOffsetY = BejeweledLivePlus.Misc.Common.Rand() % (int)(double)mCvShakey - (int)(double)mCvShakey / 2;
			}
			else
			{
				mQuestBoard.mOffsetX = 0;
				mQuestBoard.mOffsetY = 0;
			}
			int num6 = mHypercubes.Count;
			int num7 = 0;
			while (num7 < num6)
			{
				Piece pieceById3 = mQuestBoard.GetPieceById(mHypercubes[num7]);
				if (pieceById3 == null)
				{
					mHypercubes.RemoveAt(num7);
					num6--;
				}
				else if (pieceById3.mTallied)
				{
					mQuestBoard.Hypercubeify(pieceById3);
					mHypercubes.RemoveAt(num7);
					num6--;
				}
				else if (IsSpecialPieceUnlocked(pieceById3))
				{
					pieceById3.mCanSwap = true;
					mQuestBoard.Hypercubeify(pieceById3);
					mHypercubes.RemoveAt(num7);
					num6--;
				}
				else
				{
					num7++;
				}
			}
		}

		public bool TriggerDigPiece(Piece theFromPiece, Piece thePiece, bool theIsSpecialGem, bool theAllowDeletion, bool theAllowSound)
		{
			return TriggerDigPiece(theFromPiece, thePiece, theIsSpecialGem, theAllowDeletion, theAllowSound, false);
		}

		public bool TriggerDigPiece(Piece theFromPiece, Piece thePiece, bool theIsSpecialGem, bool theAllowDeletion)
		{
			return TriggerDigPiece(theFromPiece, thePiece, theIsSpecialGem, theAllowDeletion, true, false);
		}

		public bool TriggerDigPiece(Piece theFromPiece, Piece thePiece, bool theIsSpecialGem, bool theAllowDeletion, bool theAllowSound, bool theForce)
		{
			if (!theForce && mVisited[thePiece.mRow][thePiece.mCol])
			{
				return false;
			}
			bool flag = false;
			int mRow = thePiece.mRow;
			int mCol = thePiece.mCol;
			if (thePiece.IsFlagSet(65536u))
			{
				thePiece.mShakeScale = 0f;
				if (IsSpecialPiece(thePiece))
				{
					if (thePiece.IsFlagSet(2u))
					{
						if (theFromPiece == null)
						{
							thePiece.mLastColor = mQuestBoard.mNewGemColors[BejeweledLivePlus.Misc.Common.Rand() % Common.size(mQuestBoard.mNewGemColors)];
						}
						else if (((ulong)theFromPiece.mExplodeSourceFlags & 2uL) != 0)
						{
							int num = 20;
							while (num-- > 0)
							{
								thePiece.mLastColor = mQuestBoard.mNewGemColors[BejeweledLivePlus.Misc.Common.Rand() % Common.size(mQuestBoard.mNewGemColors)];
								if (thePiece.mLastColor != theFromPiece.mColor)
								{
									break;
								}
							}
						}
					}
					mQuestBoard.TriggerSpecial(thePiece, theFromPiece);
				}
				else if (mIdToTileData.ContainsKey(thePiece.mId))
				{
					TileData tileData = mIdToTileData[thePiece.mId];
					if (tileData.mIsDeleting)
					{
						return false;
					}
					EDigPieceType mPieceType = tileData.mPieceType;
					if (tileData.mPieceType == EDigPieceType.eDigPiece_Goal || tileData.mPieceType == EDigPieceType.eDigPiece_Artifact)
					{
						if (tileData.mPieceType == EDigPieceType.eDigPiece_Artifact)
						{
							GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_QUEST_GET);
							mQuestBoard.AddToStat(36);
							ArtifactData artifactData = mArtifacts[tileData.mArtifactId];
							mCollectedArtifacts.Add(tileData.mArtifactId);
							Point point = new Point(ConstantsWP.DIG_BOARD_SCORE_CENTER_X, ConstantsWP.DIG_BOARD_SCORE_BTM_Y);
							int num2 = 0;
							int num3 = GlobalMembers.M(200);
							for (int i = 0; i < mGoldFx.Count; i++)
							{
								GoldCollectEffect goldCollectEffect = mGoldFx[i];
								if (goldCollectEffect.mCentering)
								{
									num2 = Math.Max(num2, goldCollectEffect.mExtraSplineTime + num3 - (mQuestBoard.mGameTicks - goldCollectEffect.mStartedAtTick));
								}
							}
							GoldCollectEffect goldCollectEffect2 = new GoldCollectEffect(this, tileData);
							goldCollectEffect2.mExtraSplineTime = num2;
							goldCollectEffect2.mTreasureType = ETreasureType.eTreasure_Artifact;
							goldCollectEffect2.mVal = artifactData.mValue * mArtifactBaseValue;
							goldCollectEffect2.mImageId = artifactData.mImageId;
							goldCollectEffect2.mSrcImageId = artifactData.mImageId;
							goldCollectEffect2.mGlowImageId = 1290;
							goldCollectEffect2.mExtraScaling = GlobalMembers.M(0.75);
							goldCollectEffect2.mStartPoint = new Point((int)thePiece.CX(), (int)thePiece.CY());
							goldCollectEffect2.mTargetPoint = new Point(point.mX, point.mY);
							goldCollectEffect2.mCentering = true;
							goldCollectEffect2.mDisplayVal = artifactData.mValue * mArtifactBaseValue;
							goldCollectEffect2.mDisplayName = artifactData.mName;
							GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_PARTICLE_EMIT_OVER_TIME_ARTIFACT, goldCollectEffect2.mParticleEmitOverTime);
							goldCollectEffect2.mGlowRGB = GlobalMembers.M(10047098);
							goldCollectEffect2.mLayerOnTop = true;
							goldCollectEffect2.Init();
							mGoldFx.Insert(0, goldCollectEffect2);
							int theMoveCreditId = theFromPiece?.mMoveCreditId ?? thePiece.mMoveCreditId;
							mQuestBoard.AddToStat(1, goldCollectEffect2.mVal, theMoveCreditId);
							mQuestBoard.MaxStat(25, mQuestBoard.GetMoveStat(theMoveCreditId, 1));
						}
						else
						{
							if ((int)tileData.mStrength > 3)
							{
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_TREASUREFIND_DIAMONDS, mQuestBoard.GetPanPosition(thePiece));
							}
							else
							{
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_TREASUREFIND, mQuestBoard.GetPanPosition(thePiece));
							}
							int num4 = tileData.mStrength;
							int num5 = num4;
							if (num4 == 9 || !mQuestBoard.mIsPerpetual)
							{
								num5 = 1;
							}
							PointsEffect pointsEffect = null;
							foreach (Effect item in mGoldPointsFXManager.mEffectList[1])
							{
								pointsEffect = (PointsEffect)item;
								if (pointsEffect.mPieceId != thePiece.mId)
								{
									pointsEffect = null;
									continue;
								}
								break;
							}
							ETreasureType eTreasureType = ((num4 > 3) ? ETreasureType.eTreasure_Gem : ETreasureType.eTreasure_Gold);
							int num6 = num5;
							if (mQuestBoard.mIsPerpetual)
							{
								int index = Math.Min(num5 - 1, mTreasureRangeMax.Count - 1);
								int num7 = mTreasureRangeMax[index] - mTreasureRangeMin[index];
								num5 = mTreasureRangeMin[index] + BejeweledLivePlus.Misc.Common.Rand() % (num7 + 1);
								switch (eTreasureType)
								{
								case ETreasureType.eTreasure_Gold:
									num6 = num5 * mGoldValue;
									break;
								case ETreasureType.eTreasure_Gem:
									num6 = num5 * mGemValue;
									break;
								}
							}
							if (pointsEffect != null)
							{
								pointsEffect.mCount += num6;
							}
							else
							{
								pointsEffect = PointsEffect.alloc(num6, thePiece.mId, !mQuestBoard.mIsPerpetual);
								mGoldPointsFXManager.AddEffect(pointsEffect);
							}
							pointsEffect.mX = thePiece.CX();
							pointsEffect.mY = thePiece.CY();
							foreach (Effect item2 in mGoldPointsFXManager.mEffectList[1])
							{
								PointsEffect pointsEffect2 = (PointsEffect)item2;
								if (pointsEffect2 != pointsEffect && Math.Abs(pointsEffect.mX - pointsEffect2.mX) <= (float)GlobalMembers.M(200) && Math.Abs(pointsEffect.mY - pointsEffect2.mY) <= (float)GlobalMembers.M(40))
								{
									pointsEffect.mY += GlobalMembers.M(50);
									break;
								}
							}
							for (int j = 0; j < num5; j++)
							{
								if (mQuestBoard.mIsPerpetual)
								{
									GoldCollectEffect goldCollectEffect2 = new GoldCollectEffect(this, tileData);
									int num8;
									if (tileData.mDiamondFx != null)
									{
										num8 = tileData.mDiamondFx.GetExplodeSubId(BejeweledLivePlus.Misc.Common.Rand());
										switch (num4)
										{
										case 4:
											goldCollectEffect2.mGlowRGB = GlobalMembers.M(4052442);
											goldCollectEffect2.mGlowRGB2 = GlobalMembers.M(4717055);
											break;
										case 5:
											goldCollectEffect2.mGlowRGB = GlobalMembers.M(6580987);
											goldCollectEffect2.mGlowRGB2 = GlobalMembers.M(3386875);
											break;
										case 6:
											goldCollectEffect2.mGlowRGB = GlobalMembers.M(16347389);
											goldCollectEffect2.mGlowRGB2 = GlobalMembers.M(16467965);
											break;
										case 7:
											goldCollectEffect2.mGlowRGB = GlobalMembers.M(16428975);
											goldCollectEffect2.mGlowRGB2 = GlobalMembers.M(16416091);
											break;
										}
										goldCollectEffect2.mUseBaseSparkles = false;
									}
									else
									{
										num8 = 1147 + BejeweledLivePlus.Misc.Common.Rand() % GlobalMembers.M(9);
										goldCollectEffect2.mGlowRGB = GlobalMembers.M(16118660);
									}
									Point point2 = new Point(GlobalMembers.S(ConstantsWP.DIG_BOARD_SCORE_CENTER_X), GlobalMembers.S(ConstantsWP.DIG_BOARD_SCORE_BTM_Y) - GlobalMembersResourcesWP.IMAGE_INGAMEUI_DIAMOND_MINE_SCORE_BAR_BACK.mHeight / 2);
									goldCollectEffect2.mTreasureType = eTreasureType;
									GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_PARTICLE_EMIT_OVER_TIME, goldCollectEffect2.mParticleEmitOverTime);
									goldCollectEffect2.mVal = ((eTreasureType == ETreasureType.eTreasure_Gem) ? mGemValue : mGoldValue);
									goldCollectEffect2.mImageId = num8;
									goldCollectEffect2.mSrcImageId = num8;
									goldCollectEffect2.mStartPoint = new Point((int)((float)mQuestBoard.GetBoardX() + thePiece.mX + (float)TriggerDigPiece_goldCollectRect.mX + (float)(BejeweledLivePlus.Misc.Common.Rand() % TriggerDigPiece_goldCollectRect.mWidth)), (int)((float)mQuestBoard.GetBoardY() + thePiece.mY + (float)TriggerDigPiece_goldCollectRect.mY + (float)(BejeweledLivePlus.Misc.Common.Rand() % TriggerDigPiece_goldCollectRect.mHeight)));
									goldCollectEffect2.mTargetPoint = new Point(point2.mX, point2.mY);
									goldCollectEffect2.mTimeMod = GlobalMembersUtils.GetRandFloatU() * GlobalMembers.M(0.5f);
									goldCollectEffect2.mLayerOnTop = false;
									goldCollectEffect2.Init();
									mGoldFx.Add(goldCollectEffect2);
									int theMoveCreditId2 = theFromPiece?.mMoveCreditId ?? thePiece.mMoveCreditId;
									mQuestBoard.AddToStat(1, goldCollectEffect2.mVal, theMoveCreditId2);
									mQuestBoard.MaxStat(25, mQuestBoard.GetMoveStat(theMoveCreditId2, 1));
								}
								else
								{
									TriggerDigCoin(thePiece, num4 - 1);
								}
							}
						}
						if (((int)tileData.mStrength == 1 || mQuestBoard.mIsPerpetual) && ((int)tileData.mStrength < 9 || tileData.mPieceType == EDigPieceType.eDigPiece_Artifact))
						{
							tileData.SetAs(EDigPieceType.eDigPiece_Block, 1, thePiece, this);
						}
					}
					if (theIsSpecialGem && mPieceType == EDigPieceType.eDigPiece_Block && (int)tileData.mStrength == 4)
					{
						tileData.mStrength.value = 1;
					}
					if ((int)tileData.mStrength >= 1 && ((int)tileData.mStrength != 4 || mPieceType != EDigPieceType.eDigPiece_Block))
					{
						if (theAllowSound && mPieceType == EDigPieceType.eDigPiece_Block)
						{
							switch (tileData.mStrength)
							{
							case 1:
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_DIRT_CRACKED, mQuestBoard.GetPanPosition(thePiece));
								break;
							case 4:
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_BIGSTONE_CRACKED, mQuestBoard.GetPanPosition(thePiece));
								break;
							default:
								GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_DIAMOND_MINE_STONE_CRACKED, mQuestBoard.GetPanPosition(thePiece));
								break;
							}
						}
						if ((int)tileData.mStrength > 4)
						{
							tileData.mStrength.value--;
						}
						tileData.mStrength.value--;
						if ((int)tileData.mStrength == 0)
						{
							CreateRockFragments(thePiece, true);
							tileData.mIsDeleting = true;
							if (theAllowDeletion)
							{
								mQuestBoard.DeletePiece(thePiece);
								flag = true;
							}
						}
						else
						{
							CreateRockFragments(thePiece, false);
						}
					}
				}
				if (!flag)
				{
					thePiece.mElectrocutePercent = 0f;
				}
			}
			mVisited[mRow][mCol] = true;
			return flag;
		}

		public void CreateRockFragments(Piece i_piece, bool i_isDirt)
		{
			AddDustEffect(new FPoint(i_piece.CX(), i_piece.CY()));
			int num = (int)GlobalMembers.S(100.0 * GlobalMembers.M(0.75));
			if (i_isDirt)
			{
				int num2 = ConstantsWP.DIG_BOARD_BROWN_ROCK_BASE_COUNT + BejeweledLivePlus.Misc.Common.Rand(ConstantsWP.DIG_BOARD_BROWN_ROCK_RAND_COUNT);
				for (int i = 0; i < num2; i++)
				{
					CreateRockFragment((int)(i_piece.CX() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), (int)(i_piece.CY() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), GlobalMembersResourcesWP.IMAGE_WALLROCKS_SMALL_BROWN, GlobalMembers.MS(10));
				}
				return;
			}
			CreateRockFragment((int)i_piece.CX(), (int)i_piece.CY(), GlobalMembersResourcesWP.IMAGE_WALLROCKS_LARGE, GlobalMembers.MS(2));
			int num3 = ConstantsWP.DIG_BOARD_MEDIUM_ROCK_BASE_COUNT + BejeweledLivePlus.Misc.Common.Rand(ConstantsWP.DIG_BOARD_MEDIUM_ROCK_RAND_COUNT);
			for (int j = 0; j < num3; j++)
			{
				CreateRockFragment((int)(i_piece.CX() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), (int)(i_piece.CY() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), GlobalMembersResourcesWP.IMAGE_WALLROCKS_MEDIUM, GlobalMembers.MS(6));
			}
			num3 = ConstantsWP.DIG_BOARD_SMALL_ROCK_BASE_COUNT + BejeweledLivePlus.Misc.Common.Rand(ConstantsWP.DIG_BOARD_SMALL_ROCK_RAND_COUNT);
			for (int k = 0; k < num3; k++)
			{
				CreateRockFragment((int)(i_piece.CX() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), (int)(i_piece.CY() - (float)(num / 2) + (float)BejeweledLivePlus.Misc.Common.Rand(num)), GlobalMembersResourcesWP.IMAGE_WALLROCKS_SMALL, GlobalMembers.MS(8));
			}
		}

		public void CreateRockFragment(int theX, int theY, Image theImage, float theSpeed)
		{
			Effect effect = mQuestBoard.mPostFXManager.AllocEffect(Effect.Type.TYPE_WALL_ROCK);
			float num = GlobalMembersUtils.GetRandFloat() * (float)Math.PI;
			effect.mDX = theSpeed * (float)Math.Cos(num);
			effect.mDY = theSpeed * (float)Math.Sin(num);
			effect.mDecel = GlobalMembers.M(0.95f);
			effect.mX = theX;
			effect.mY = theY;
			effect.mGravity = GlobalMembers.M(0.33f);
			effect.mImage = theImage;
			effect.mFrame = BejeweledLivePlus.Misc.Common.Rand(4);
			effect.mDAlpha = 0f;
			mQuestBoard.mPostFXManager.AddEffect(effect);
		}

		public TileData GenRandomTile(float cvLevel, Piece piece, ref bool o_isImmovable)
		{
			double outVal = mCvMinBrickStr.GetOutVal(cvLevel);
			double outVal2 = mCvMaxBrickStr.GetOutVal(cvLevel);
			double outVal3 = mCvMinMineStr.GetOutVal(cvLevel);
			double outVal4 = mCvMaxMineStr.GetOutVal(cvLevel);
			double outVal5 = mCvMineProb.GetOutVal(cvLevel);
			double outVal6 = mCvDarkRockFreq.GetOutVal(cvLevel);
			if ((double)GlobalMembersUtils.GetRandFloatU() < outVal5)
			{
				double num = outVal3 + (outVal4 - outVal3) * (double)mMineStrSpread.GetOutVal(GlobalMembersUtils.GetRandFloatU());
				int theStr = (int)Math.Max(1f, Math.Min(7f, Utils.Round((float)num)));
				if (!mIdToTileData.ContainsKey(piece.mId))
				{
					mIdToTileData.Add(piece.mId, new TileData());
				}
				mIdToTileData[piece.mId].SetAs(EDigPieceType.eDigPiece_Goal, theStr, piece, this);
				piece.SetFlag(65536u);
				o_isImmovable = true;
				return mIdToTileData[piece.mId];
			}
			double num2 = outVal + (outVal2 - outVal) * (double)mBrickStrSpread.GetOutVal(GlobalMembersUtils.GetRandFloatU());
			if (piece.mCol == 0 || piece.mCol == 7)
			{
				num2 *= mCvEdgeBrickStrPerLevel.GetOutVal(cvLevel);
			}
			int num3 = Math.Max(1, Math.Min(5, (int)Utils.Round((float)num2)));
			if (num3 == 4)
			{
				num3 = 5;
			}
			if (num3 == 5 && (double)GlobalMembersUtils.GetRandFloatU() <= outVal6)
			{
				num3 = 4;
			}
			if (!mIdToTileData.ContainsKey(piece.mId))
			{
				mIdToTileData.Add(piece.mId, new TileData());
			}
			mIdToTileData[piece.mId].SetAs(EDigPieceType.eDigPiece_Block, num3, piece, this);
			piece.SetFlag(65536u);
			o_isImmovable = true;
			return mIdToTileData[piece.mId];
		}

		public int GetBoardScrollOffsetY()
		{
			if (!mQuestBoard.mIsPerpetual)
			{
				return 0;
			}
			if ((double)mCvScrollY > 0.0 && (double)mCvScrollY < 1.0 - (double)SexyFramework.GlobalMembers.SEXYMATH_EPSILON)
			{
				return (int)(100.0 - 100.0 * (GetGridDepthAsDouble() - (double)GetGridDepth()));
			}
			return 0;
		}

		public void DeleteAllPieces()
		{
			mIdToTileData.Clear();
			Piece[,] mBoard = mQuestBoard.mBoard;
			foreach (Piece piece in mBoard)
			{
				if (piece != null && piece.IsFlagSet(65536u))
				{
					DeletePiece(piece);
				}
			}
		}

		public override void LevelUp()
		{
			if (mGoldFx.Count == 0)
			{
				base.LevelUp();
			}
		}

		public override void NewGame()
		{
			Dictionary<string, string> mParams = mQuestBoard.mParams;
			mArtifacts.Clear();
			if (!mParams.ContainsKey("TargetCount") || !int.TryParse(mParams["TargetCount"], out mTargetCount))
			{
				mTargetCount = 0;
			}
			List<GridData> list = new List<GridData>();
			Board.ParseGridLayout(mParams["Grids"], list, false);
			if (!mParams.ContainsKey("DigCountPerScroll") || !int.TryParse(mParams["DigCountPerScroll"], out mDigCountPerScroll))
			{
				mDigCountPerScroll = 0;
			}
			if (!mParams.ContainsKey("GoldValue") || !int.TryParse(mParams["GoldValue"], out mGoldValue))
			{
				mGoldValue = 0;
			}
			if (!mParams.ContainsKey("DiamondValue") || !int.TryParse(mParams["DiamondValue"], out mGemValue))
			{
				mGemValue = 0;
			}
			if (!mParams.ContainsKey("ArtifactBaseValue") || !int.TryParse(mParams["ArtifactBaseValue"], out mArtifactBaseValue))
			{
				mArtifactBaseValue = 0;
			}
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_ARTIFACT_POSS_RANGE, mArtifactPossRange);
			if (!mParams.ContainsKey("ArtifactSkipTileCount") || !int.TryParse(mParams["ArtifactSkipTileCount"], out mArtifactSkipTileCount))
			{
				mArtifactSkipTileCount = 0;
			}
			if (!mParams.ContainsKey("PowerGemThresholdDepth0") || !int.TryParse(mParams["PowerGemThresholdDepth0"], out mPowerGemThresholdDepth0))
			{
				mPowerGemThresholdDepth0 = 0;
			}
			if (!mParams.ContainsKey("PowerGemThresholdDepth20") || !int.TryParse(mParams["PowerGemThresholdDepth20"], out mPowerGemThresholdDepth20))
			{
				mPowerGemThresholdDepth20 = 0;
			}
			if (!mParams.ContainsKey("PowerGemThresholdDepth40") || !int.TryParse(mParams["PowerGemThresholdDepth40"], out mPowerGemThresholdDepth40))
			{
				mPowerGemThresholdDepth40 = 0;
			}
			if (mParams.ContainsKey("TreasureRange"))
			{
				List<int> list2 = new List<int>();
				Utils.SplitAndConvertStr(mParams["TreasureRange"], list2);
				for (int i = 0; i < list2.Count; i++)
				{
					if (i % 2 == 0)
					{
						mTreasureRangeMin.Add(list2[i]);
					}
					else if (i % 2 == 1)
					{
						mTreasureRangeMax.Add(list2[i]);
					}
				}
			}
			if (mParams.ContainsKey("ArtifactMinRows"))
			{
				Utils.SplitAndConvertStr(mParams["ArtifactMinRows"], mStartArtifactRow);
			}
			if (!mParams.ContainsKey("ArtifactMinTiles") || !int.TryParse(mParams["ArtifactMinTiles"], out mArtifactMinTiles))
			{
				mArtifactMinTiles = 0;
			}
			if (!mParams.ContainsKey("ArtifactMaxTiles") || !int.TryParse(mParams["ArtifactMaxTiles"], out mArtifactMaxTiles))
			{
				mArtifactMaxTiles = 0;
			}
			for (int j = 1; mParams.ContainsKey($"Artifact{j}"); j++)
			{
				List<string> list3 = new List<string>();
				Utils.SplitAndConvertStr(mParams[$"Artifact{j}"], list3, ',', true, -1);
				ArtifactData artifactData = new ArtifactData();
				artifactData.mId = list3[0];
				artifactData.mName = list3[3];
				artifactData.mMinDepth = SexyFramework.GlobalMembers.sexyatoi(list3[1]);
				artifactData.mValue = SexyFramework.GlobalMembers.sexyatoi(list3[2]);
				string theStringId = $"IMAGE_QUEST_DIG_BOARD_ITEM_{artifactData.mId}_BIG".ToUpper();
				artifactData.mImageId = (int)GlobalMembersResourcesWP.GetIdByStringId(theStringId);
				artifactData.mUnderlayImgId = ARTIFACT_UNDERLAY_IDS[BejeweledLivePlus.Misc.Common.Rand() % ARTIFACT_UNDERLAY_IDS.Length];
				artifactData.mOverlayImgId = ARTIFACT_OVERLAY_IDS[BejeweledLivePlus.Misc.Common.Rand() % ARTIFACT_OVERLAY_IDS.Length];
				if (artifactData.mImageId != -1)
				{
					mArtifacts.Add(artifactData);
				}
			}
			mTutorialHeader = mParams["TutorialHeader"];
			mTutorialText = mParams["TutorialText"];
			if (mParams.ContainsKey("MaxBrickStrPerLevel"))
			{
				mUsingRandomizers = true;
				CurvedVal curvedVal = new CurvedVal();
				CurvedVal curvedVal2 = new CurvedVal();
				CurvedVal curvedVal3 = new CurvedVal();
				mCvMinBrickStr.SetCurve(mParams["MinBrickStrPerLevel"]);
				mCvMaxBrickStr.SetCurve(mParams["MaxBrickStrPerLevel"]);
				mCvEdgeBrickStrPerLevel.SetCurve(mParams["EdgeBrickStrPerLevel"]);
				mCvMinMineStr.SetCurve(mParams["MinMineStrPerLevel"]);
				mCvMaxMineStr.SetCurve(mParams["MaxMineStrPerLevel"]);
				mCvMineProb.SetCurve(mParams["MineProbPerLevel"]);
				mCvDarkRockFreq.SetCurve(mParams["DarkRockFrequency"]);
				curvedVal.SetCurve(mParams["ArtifactSpread"]);
				curvedVal2.SetCurve(mParams["BrickStrSpread"]);
				curvedVal3.SetCurve(mParams["MineStrSpread"]);
				mBrickStrSpread.SetToCurve(curvedVal2);
				mArtifactSpread.SetToCurve(curvedVal);
				mMineStrSpread.SetToCurve(curvedVal3);
				mQuestBoard.mIsPerpetual = true;
				mQuestBoard.mAllowLevelUp = false;
			}
			new List<int>();
			new List<int>();
			new List<int>();
			base.NewGame();
			AdvanceArtifact(true);
			mInitPushAnimCv.mAppUpdateCountSrc = mUpdateCnt;
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eDIG_GOAL_INIT_PUSH_ANIM_CV, mInitPushAnimCv);
			if (Common.size(list) > 0)
			{
				mGridData = Common.front(list);
				mAllowDescent = mUsingRandomizers || mGridData.GetRowCount() > 8;
				if (!mQuestBoard.mContinuedFromLoad)
				{
					SyncGrid(0);
					for (int k = 0; k < 1000; k++)
					{
						if (mQuestBoard.FindMove(null, 0, true, true))
						{
							break;
						}
						Piece piece = mQuestBoard.mBoard[BejeweledLivePlus.Misc.Common.Rand() % 8, BejeweledLivePlus.Misc.Common.Rand() % 8];
						if (piece.mFlags == 0 && piece.mColor != -1)
						{
							piece.mColor = BejeweledLivePlus.Misc.Common.Rand() % 7;
						}
					}
				}
			}
			mQuestBoard.mWantShowPoints = false;
			if (GlobalMembers.gApp.mDiamondMineFirstLaunch)
			{
				int num = (mQuestBoard.mIsPerpetual ? GlobalMembers.M(3) : GlobalMembers.M(5));
				int num2 = (mQuestBoard.mIsPerpetual ? GlobalMembers.M(2) : GlobalMembers.M(4));
				Piece piece2 = mQuestBoard.mBoard[num2, num];
				if (piece2 != null)
				{
					mGoldRushTipPieceId = piece2.mId;
				}
				GlobalMembers.gApp.mDiamondMineFirstLaunch = false;
			}
			mDefaultNeverAllowCascades = mQuestBoard.mNeverAllowCascades;
			if (!mLoadingGame)
			{
				((DigBoard)mQuestBoard).mRotatingCounter.ResetCounter(0);
			}
			mLoadingGame = false;
			mDigInProgress = false;
		}

		public override int GetLevelPoints()
		{
			return mTargetCount;
		}

		public new bool GetTooltipText(Piece thePiece, ref string theHeader, ref string theBody)
		{
			bool result = false;
			if (thePiece.IsFlagSet(65536u))
			{
				TileData tileData = mIdToTileData[thePiece.mId];
				switch (tileData.mPieceType)
				{
				case EDigPieceType.eDigPiece_Artifact:
					theHeader = GlobalMembers._ID("ARTIFACT", 184);
					theBody = GlobalMembers._ID("Uncover this object for bonus points.", 185);
					result = true;
					break;
				case EDigPieceType.eDigPiece_Goal:
					if ((int)tileData.mStrength <= 3)
					{
						theHeader = GlobalMembers._ID("GOLD", 186);
						theBody = GlobalMembers._ID("Dig up gold to score points.", 187);
						result = true;
					}
					else
					{
						theHeader = GlobalMembers._ID("DIAMONDS", 188);
						theBody = GlobalMembers._ID("Dig up diamonds to score points.", 189);
						result = true;
					}
					break;
				case EDigPieceType.eDigPiece_Block:
					if (mHypercubes.Contains(thePiece.mId))
					{
						theHeader = GlobalMembers._ID("HYPERCUBE", 190);
						theBody = GlobalMembers._ID("Uncover this buried Hypercube in order to use it.", 191);
						result = true;
						break;
					}
					switch (tileData.mStrength)
					{
					case 1:
						theHeader = GlobalMembers._ID("DIRT", 192);
						theBody = GlobalMembers._ID("Match a Gem adjacent to this block to break it up.", 193);
						result = true;
						break;
					case 2:
						theHeader = GlobalMembers._ID("ROCKS", 194);
						theBody = GlobalMembers._ID("Match 2 Gems adjacent to this block to break it up.", 195);
						result = true;
						break;
					case 3:
						theHeader = GlobalMembers._ID("STONES", 196);
						theBody = GlobalMembers._ID("Match 3 Gems adjacent to this block to break it up.", 197);
						result = true;
						break;
					case 4:
						theHeader = GlobalMembers._ID("DARK ROCK", 198);
						theBody = GlobalMembers._ID("This block can be destroyed only by blasting it with Special Gems.", 199);
						result = true;
						break;
					case 5:
						theHeader = GlobalMembers._ID("BOULDER", 200);
						theBody = GlobalMembers._ID("Match 4 Gems adjacent to this block to break it up.", 201);
						result = true;
						break;
					default:
						theHeader = GlobalMembers._ID("BOULDER", 202);
						theBody = GlobalMembers._ID("Match Multiple Gems adjacent to this block to break it up.", 203);
						result = true;
						break;
					}
					break;
				}
			}
			return result;
		}

		public void AddDustEffect(FPoint pt)
		{
			ParticleEffect particleEffect = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_SANDSTORM_DIG);
			particleEffect.mX = pt.mX;
			particleEffect.mY = pt.mY;
			particleEffect.mDoDrawTransform = true;
			mDustFXManager.AddEffect(particleEffect);
		}

		public new virtual bool IsGameIdle()
		{
			if (base.IsGameIdle())
			{
				return !mCvScrollY.IsDoingCurve();
			}
			return false;
		}
	}
}
