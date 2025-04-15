using System;
using System.Collections.Generic;
using System.Linq;
using BejeweledLivePlus;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using Common = SexyFramework.Common;

public class QuestObject
{
	public enum EAnimState
	{
		eAnim_Unrevealed,
		eAnim_Revealing,
		eAnim_Revealed,
		eAnim_Completing,
		eAnim_Complete
	}

	public struct ImageConfig
	{
		public MemoryImage mDestImage;

		public float mCompletePct;

		public float mIntensity;

		public float mContrast;

		public ImageConfig(MemoryImage img, float cmp, float iten, float com)
		{
			mDestImage = img;
			mCompletePct = cmp;
			mIntensity = iten;
			mContrast = com;
		}
	}

	public EAnimState mAnimState;

	public CurvedVal mTransitionStreamerCount;

	public CurvedVal mTransitionRumble;

	public CurvedVal mGrayscaleAlpha;

	public CurvedVal mGrayscaleAlphaExtra;

	public CurvedVal mBaseObjAlpha;

	public CurvedVal mBaseObjAddAlpha;

	public CurvedVal mTransitionStreamerMag;

	public CurvedVal mStreamerMag;

	public CurvedVal mBackgroundDarken;

	public CurvedVal mTransitionStreamerColorShift;

	public CurvedVal mStreamerColorShift;

	public CurvedVal mGlintAlpha;

	public CurvedVal mStreamerExtraRot;

	public CurvedVal mMaskAlpha;

	public QuestMenu mQuestMenu;

	public double mGrayscaleAlphaMult;

	public int mTransitionTicks;

	public int mNewStreamerDelay;

	public int mStreamerTgt;

	public int mClearStreamersAt;

	public int mClearTransStreamersAt;

	public int mUpdateCnt;

	public Point mRumbleOff;

	public Color mGlintColor;

	public double mAlpha;

	public double mStreamerStartMag;

	public QuestObjectData mData;

	public bool mDrawTris;

	public bool mDrawStreamerTris;

	public MemoryImage mGrayscaleBase;

	public MemoryImage mGrayscaleAdd;

	public float mGrayscaleBaseComplete;

	public float mGrayscaleAddComplete;

	public List<FloatyOrb> mFloatyOrbs = new List<FloatyOrb>();

	public int mQuestsCompleted;

	public List<BorderEffect> mBorderFx = new List<BorderEffect>();

	public List<BorderEffect> mTransitionBorderFx = new List<BorderEffect>();

	public List<StreamerEffect> mTransitionStreamers = new List<StreamerEffect>();

	public List<StreamerEffect> mStreamers = new List<StreamerEffect>();

	private int mQuestObjIdx;

	private double mTransitionGlowAlpha;

	public QuestObject(QuestMenu theQuestMenu, int theId, int theCountCompleted)
	{
		mQuestMenu = theQuestMenu;
		mQuestObjIdx = theId;
		mQuestsCompleted = theCountCompleted;
		mGrayscaleBase = null;
		mGrayscaleAdd = null;
		mDrawTris = false;
		mDrawStreamerTris = false;
		Reset();
	}

	public void Reset()
	{
		DeleteGrayscale();
		ClearOrbs();
		ClearFx(true, true, true, true);
		mData = default(QuestObjectData);
		mGrayscaleAlpha.SetConstant(0.0);
		mTransitionStreamerMag.SetConstant(1.0);
		mTransitionStreamerColorShift.SetConstant(0.0);
		mGrayscaleAlphaExtra.SetConstant(0.0);
		mTransitionRumble.SetConstant(0.0);
		mBaseObjAlpha.SetConstant(0.0);
		mBaseObjAddAlpha.SetConstant(0.0);
		mStreamerMag.SetConstant(1.0);
		mStreamerColorShift.SetConstant(0.0);
		mBackgroundDarken.SetConstant(0.0);
		mMaskAlpha.SetConstant(0.0);
		mTransitionTicks = 0;
		mNewStreamerDelay = 0;
		mClearStreamersAt = 0;
		mClearTransStreamersAt = 0;
		mAlpha = 0.0;
		mUpdateCnt = 0;
		mStreamerTgt = 0;
		mStreamerStartMag = 0.0;
		mGrayscaleAlphaMult = 0.0;
		if (mQuestsCompleted >= BejeweledLivePlusAppConstants.QUESTS_PER_SET)
		{
			SetAnim(EAnimState.eAnim_Complete);
		}
		else if (mQuestsCompleted >= BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET)
		{
			SetAnim(EAnimState.eAnim_Revealed);
		}
		else
		{
			SetAnim(EAnimState.eAnim_Unrevealed);
		}
	}

	public void SetAnim(EAnimState theAnimIdx)
	{
		mAnimState = theAnimIdx;
		mStreamerStartMag = mData.mStreamerStartMag;
		switch (mAnimState)
		{
		case EAnimState.eAnim_Unrevealed:
		{
			BorderEffect borderEffect4 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect4.mPhase.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b30,1,0.005,1,#         ~~"));
			borderEffect4.mPhase.SetMode(1);
			borderEffect4.mAlpha.SetConstant(BejeweledLivePlus.GlobalMembers.M(1f));
			borderEffect4.mMultMagnitudeOuter.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.7f));
			borderEffect4.mMultMagnitudeInner.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.8f));
			borderEffect4.mId = 0;
			mBorderFx.Add(borderEffect4);
			mTransitionStreamerMag.SetConstant(1.0);
			mGlintAlpha.SetConstant(0.0);
			mMaskAlpha.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.3));
			if (BejeweledLivePlus.GlobalMembers.M(1) != 0)
			{
				borderEffect4 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
				borderEffect4.mPhase.SetConstant(1.0);
				borderEffect4.mFxOffsetX = BejeweledLivePlus.GlobalMembers.M(0);
				borderEffect4.mFxOffsetY = BejeweledLivePlus.GlobalMembers.M(0);
				borderEffect4.mAlpha.SetConstant(BejeweledLivePlus.GlobalMembers.M(1.0));
				borderEffect4.mMultMagnitudeOuter.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.7));
				borderEffect4.mMultMagnitudeInner.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.8));
				borderEffect4.mId = 1;
				mBorderFx.Add(borderEffect4);
			}
			mGrayscaleAlphaMult = 0.0;
			mBaseObjAlpha.SetConstant(0.0);
			mBaseObjAddAlpha.SetConstant(0.0);
			mStreamerTgt = BejeweledLivePlus.GlobalMembers.M(150);
			mGlintColor = new Color(0, 0);
			mClearStreamersAt = 0;
			mClearTransStreamersAt = 0;
			break;
		}
		case EAnimState.eAnim_Revealing:
		{
			mQuestMenu.mQuestButtonSoundCount = 0;
			ClearFx(true, false, true, true);
			mQuestMenu.AddDeferredSound(GlobalMembersResourcesWP.SOUND_QUESTMENU_RELICREVEALED_RUMBLE, BejeweledLivePlus.GlobalMembers.M(15), BejeweledLivePlus.GlobalMembers.M(1.0));
			mQuestMenu.AddDeferredSound(GlobalMembersResourcesWP.SOUND_QUESTMENU_RELICREVEALED_OBJECT, BejeweledLivePlus.GlobalMembers.M(70), BejeweledLivePlus.GlobalMembers.M(1.0));
			for (uint num2 = 0u; num2 < Common.size(mBorderFx); num2++)
			{
				mBorderFx.ToArray()[num2].mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,2.5,~###     )~###   Z#### @#M%M"));
			}
			mGlintAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,2,####         ~~###"));
			mTransitionRumble.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,14,0.01,1.5,####o####s~###    >~###  r#### .#KZ[#####"));
			mGrayscaleAlpha.SetConstant(BejeweledLivePlus.GlobalMembers.M(1.0));
			mGrayscaleAlphaExtra.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,4,~###       C~###  ^####"));
			mBaseObjAlpha.SetConstant(0.0);
			mBaseObjAddAlpha.SetConstant(0.0);
			mTransitionStreamerCount.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,40,0.01,2,#&KR ,+0l:    N~Sz(    I~###"));
			mTransitionStreamerMag.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,5,0.01,2.5,6###    3~####~### v####  X#### @####"));
			mBackgroundDarken.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,2.5,####    T#### v~### z~### X####"));
			mTransitionStreamerColorShift.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,2,####        O#### B~###2~###"));
			mStreamerColorShift.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,2.5,####         ~~###"));
			mMaskAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,0.3,0.01,2,~###       0~### )#### k####"));
			mGrayscaleAlphaMult = 0.0;
			mStreamerMag.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,5,0.01,2,6###   T~###  $~### ?#### G####u6###o6###"));
			mGlintColor = new Color(BejeweledLivePlus.GlobalMembers.M(16711935), BejeweledLivePlus.GlobalMembers.M(0));
			for (uint num3 = 0u; num3 < Common.size(mStreamers); num3++)
			{
				mStreamers.ToArray()[num3].mColor2 = BejeweledLivePlus.GlobalMembers.M(10505777);
				mStreamers.ToArray()[num3].mRotExtra.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,2.5,#########         ~~###"));
			}
			for (int i = 0; i <= BejeweledLivePlus.GlobalMembers.M(2); i++)
			{
				BorderEffect borderEffect3 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
				borderEffect3.mId = i;
				borderEffect3.mPhase.SetConstant(0.0);
				borderEffect3.mDelayCnt = BejeweledLivePlus.GlobalMembers.M(80) + BejeweledLivePlus.GlobalMembers.M(15) * i;
				if (i == 2)
				{
					borderEffect3.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,1.5,#,=^   M~SAr     -~P## I#Pij"));
					borderEffect3.mMultMagnitudeOuter.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0.75,10,0.01,0.5,~n5y         ~####"));
					borderEffect3.mMultMagnitudeInner.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+1.0,12.0,0.01,0.5,~kNn         ~####"));
				}
				else
				{
					borderEffect3.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,1.5,#,=^   M~SAr     -~P## I#Pij"));
					borderEffect3.mMultMagnitudeOuter.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0.75,10,0.01,0.5,~n5y         ~####"));
					borderEffect3.mMultMagnitudeInner.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+1.0,12.0,0.01,0.5,~kNn         ~####"));
				}
				borderEffect3.mBorderGlow = true;
				mTransitionBorderFx.Add(borderEffect3);
			}
			mTransitionTicks = BejeweledLivePlus.GlobalMembers.M(230);
			mNewStreamerDelay = BejeweledLivePlus.GlobalMembers.M(300);
			mStreamerTgt = BejeweledLivePlus.GlobalMembers.M(150);
			mClearStreamersAt = BejeweledLivePlus.GlobalMembers.M(150);
			mClearTransStreamersAt = BejeweledLivePlus.GlobalMembers.M(150);
			break;
		}
		case EAnimState.eAnim_Revealed:
			ClearFx(false, true, false, true);
			mNewStreamerDelay = 0;
			mGrayscaleAlphaMult = 1.0;
			mGrayscaleAlpha.SetConstant(1.0);
			mGrayscaleAlphaExtra.SetConstant(0.0);
			mBaseObjAlpha.SetConstant(0.0);
			mBaseObjAddAlpha.SetConstant(0.0);
			mMaskAlpha.SetConstant(0.0);
			mGlintAlpha.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.5));
			mStreamerTgt = BejeweledLivePlus.GlobalMembers.M(150);
			mGlintColor = new Color(BejeweledLivePlus.GlobalMembers.M(16711935), BejeweledLivePlus.GlobalMembers.M(0));
			mClearStreamersAt = 0;
			mClearTransStreamersAt = 0;
			break;
		case EAnimState.eAnim_Completing:
		{
			mQuestMenu.mQuestButtonSoundCount = 0;
			ClearFx(true, false, true, true);
			mQuestMenu.AddDeferredSound(GlobalMembersResourcesWP.SOUND_QUESTMENU_RELICCOMPLETE_RUMBLE, BejeweledLivePlus.GlobalMembers.M(15), BejeweledLivePlus.GlobalMembers.M(1.0));
			mQuestMenu.AddDeferredSound(GlobalMembersResourcesWP.SOUND_QUESTMENU_RELICCOMPLETE_OBJECT, BejeweledLivePlus.GlobalMembers.M(280), BejeweledLivePlus.GlobalMembers.M(1.0));
			BorderEffect borderEffect2 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect2.mX = BejeweledLivePlus.GlobalMembers.M(0);
			borderEffect2.mY = BejeweledLivePlus.GlobalMembers.M(0);
			borderEffect2.mId = 0;
			borderEffect2.mDelayCnt = BejeweledLivePlus.GlobalMembers.M(300);
			mTransitionBorderFx.Add(borderEffect2);
			for (uint num = 0u; num < Common.size(mStreamers); num++)
			{
				mStreamers.ToArray()[num].mColor2 = BejeweledLivePlus.GlobalMembers.M(6644752);
			}
			mNewStreamerDelay = BejeweledLivePlus.GlobalMembers.M(300);
			borderEffect2 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect2.mPhase.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.25));
			borderEffect2.mSortOrder = BejeweledLivePlus.GlobalMembers.M(1);
			borderEffect2.mMultMagnitudeOuter.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,2,0.01,4,#### B#### .F###     UF###h#### 7#O:N"));
			borderEffect2.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,4,####u#### b~###     8~###z#### 7#O:N"));
			mTransitionBorderFx.Add(borderEffect2);
			borderEffect2 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect2.mPhase.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.25));
			borderEffect2.mSortOrder = BejeweledLivePlus.GlobalMembers.M(2);
			borderEffect2.mMultMagnitudeOuter.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1.25,0.01,4,####    k####7q###   Kq### T#O:N"));
			borderEffect2.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,4,####    :#### #~###   1~### X#O:N"));
			mTransitionBorderFx.Add(borderEffect2);
			borderEffect2 = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect2.mPhase.SetConstant(BejeweledLivePlus.GlobalMembers.M(0.25));
			borderEffect2.mSortOrder = BejeweledLivePlus.GlobalMembers.M(3);
			borderEffect2.mMultMagnitudeOuter.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1.25,0.01,4,####    *####   Y####X}###ll###xlO:N"));
			borderEffect2.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,4,####    *####   Y####X}###ll###xlO:N"));
			mBorderFx.Add(borderEffect2);
			mTransitionRumble.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,10,0.01,4,#### q####    l~###_~tu: D#### @######P#######"));
			mStreamerMag.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,4,~### (####      6#### q~###s~###"));
			mStreamerColorShift.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,2,####        O#### B~###2~###"));
			mBackgroundDarken.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,4,#########   E~###     &~### X####"));
			mMaskAlpha.SetConstant(0.0);
			mGlintColor = new Color(BejeweledLivePlus.GlobalMembers.M(16777215), BejeweledLivePlus.GlobalMembers.M(0));
			mBaseObjAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,3,####   I####    k~### k~####~###"));
			mBaseObjAddAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,5,####   &####   [~###j~###  V####"));
			mGrayscaleAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,3,~###       =~###  d####"));
			mGrayscaleAlphaExtra.SetConstant(0.0);
			mGrayscaleAlphaMult = 1.0;
			mTransitionStreamerCount.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,120,0.01,4,#&KQ  (52W)     I~####~Sz(  R~###"));
			mTransitionStreamerMag.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,5,0.01,3.5,6###  v~###   ,~###  <~### d####"));
			mTransitionStreamerColorShift.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,2.5,#########      r####  {~###2~###"));
			mTransitionTicks = BejeweledLivePlus.GlobalMembers.M(430);
			mNewStreamerDelay = BejeweledLivePlus.GlobalMembers.M(300);
			mStreamerTgt = BejeweledLivePlus.GlobalMembers.M(150);
			mClearStreamersAt = BejeweledLivePlus.GlobalMembers.M(0);
			mClearTransStreamersAt = BejeweledLivePlus.GlobalMembers.M(300);
			break;
		}
		case EAnimState.eAnim_Complete:
		{
			ClearFx(false, true, false, true);
			BorderEffect borderEffect = new BorderEffect(ref mData.mMarkerInner, ref mData.mMarkerOuter, mData.mMarkerLength);
			borderEffect.mPhase.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b30,1,0.01,1,#         ~~"));
			borderEffect.mId = 0;
			mBorderFx.Add(borderEffect);
			mNewStreamerDelay = 0;
			mStreamerTgt = BejeweledLivePlus.GlobalMembers.M(70);
			mGlintColor = new Color(BejeweledLivePlus.GlobalMembers.M(16777215), BejeweledLivePlus.GlobalMembers.M(0));
			mMaskAlpha.SetConstant(0.0);
			mBaseObjAlpha.SetConstant(1.0);
			mGrayscaleAlphaMult = 0.0;
			mClearStreamersAt = 0;
			mClearTransStreamersAt = 0;
			break;
		}
		}
		if (mAnimState == EAnimState.eAnim_Revealing)
		{
			mQuestMenu.mQuestObjTransitionBtnHidePct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.009375,3,#8E- q~###      2~hcW |#ArI"));
		}
		else if (mAnimState == EAnimState.eAnim_Completing)
		{
			if (mQuestMenu.GetCompleteCount() == BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET)
			{
				mQuestMenu.mQuestObjTransitionBtnHidePct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.009375,3,#8E- q~###        0~ArI"));
			}
			else
			{
				mQuestMenu.mQuestObjTransitionBtnHidePct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,5,#8E- 0~###      s~hcW |#ArI"));
			}
		}
	}

	public void DoAwardTransition()
	{
		if (mAnimState == EAnimState.eAnim_Unrevealed)
		{
			SetAnim(EAnimState.eAnim_Revealing);
		}
		if (mAnimState == EAnimState.eAnim_Revealed)
		{
			SetAnim(EAnimState.eAnim_Completing);
		}
	}

	public void ClearFx(bool theTransition, bool theNormal, bool theStreamers, bool theBorderFx)
	{
		if (theTransition)
		{
			if (theStreamers)
			{
				for (uint num = 0u; num < mTransitionStreamers.Count(); num++)
				{
					mTransitionStreamers.ToArray()[num] = null;
				}
				mTransitionStreamers.Clear();
			}
			if (theBorderFx)
			{
				for (uint num2 = 0u; num2 < mTransitionBorderFx.Count(); num2++)
				{
					mTransitionBorderFx.ToArray()[num2] = null;
				}
				mTransitionBorderFx.Clear();
			}
		}
		if (!theNormal)
		{
			return;
		}
		if (theStreamers)
		{
			for (uint num3 = 0u; num3 < mStreamers.Count(); num3++)
			{
				mStreamers.ToArray()[num3] = null;
			}
			mStreamers.Clear();
		}
		if (theBorderFx)
		{
			for (uint num4 = 0u; num4 < mBorderFx.Count(); num4++)
			{
				mBorderFx.ToArray()[num4] = null;
			}
			mBorderFx.Clear();
		}
	}

	public bool IsBusy()
	{
		if (mFloatyOrbs.Count() == 0 && mAnimState != EAnimState.eAnim_Revealing)
		{
			return mAnimState == EAnimState.eAnim_Completing;
		}
		return true;
	}

	public void Draw(Graphics g, int theX, int theY)
	{
	}

	public void DrawOverlay(Graphics g, int theX, int theY)
	{
	}

	public void Update()
	{
	}

	public void AddStreamer(bool theIsTransition)
	{
	}

	public void RefreshEmitters()
	{
		for (uint num = 0u; num < Common.size(mBorderFx); num++)
		{
			mBorderFx.ToArray()[num].RefreshEmitters();
		}
	}

	public void ClearOrbs()
	{
		for (uint num = 0u; num < mFloatyOrbs.Count(); num++)
		{
			mFloatyOrbs.ToArray()[num] = null;
		}
		mFloatyOrbs.Clear();
	}

	public void DeleteGrayscale()
	{
		mGrayscaleBase = null;
		mGrayscaleBaseComplete = 0f;
		mGrayscaleAdd = null;
		mGrayscaleAddComplete = 0f;
	}

	public bool CheckForTransition()
	{
		if (mFloatyOrbs.Count() == 0)
		{
			if (mAnimState == EAnimState.eAnim_Unrevealed)
			{
				if (mQuestsCompleted >= BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET)
				{
					SetAnim(EAnimState.eAnim_Revealing);
					return true;
				}
			}
			else if (mAnimState == EAnimState.eAnim_Revealing)
			{
				mTransitionTicks--;
				if (mTransitionTicks <= 0)
				{
					SetAnim(EAnimState.eAnim_Revealed);
					return true;
				}
			}
			else if (mAnimState == EAnimState.eAnim_Revealed)
			{
				if (mQuestsCompleted >= BejeweledLivePlusAppConstants.QUESTS_OPTIONAL_PER_SET + BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET)
				{
					SetAnim(EAnimState.eAnim_Completing);
					return true;
				}
			}
			else if (mAnimState == EAnimState.eAnim_Completing)
			{
				mTransitionTicks--;
				if (mTransitionTicks <= 0)
				{
					SetAnim(EAnimState.eAnim_Complete);
					return true;
				}
			}
		}
		return false;
	}

	public bool IsComplete()
	{
		return mAnimState >= EAnimState.eAnim_Completing;
	}

	public float UpdateGrayscales(float theUpdatePct)
	{
		ImageConfig[] array = new ImageConfig[2]
		{
			new ImageConfig(mGrayscaleBase, mGrayscaleBaseComplete, BejeweledLivePlus.GlobalMembers.M(2f), BejeweledLivePlus.GlobalMembers.M(0.5f)),
			new ImageConfig(mGrayscaleAdd, mGrayscaleAddComplete, BejeweledLivePlus.GlobalMembers.M(1.1f), BejeweledLivePlus.GlobalMembers.M(4f))
		};
		for (int i = 0; i < 2; i++)
		{
			ImageConfig imageConfig = array[i];
			if (theUpdatePct > 0f && imageConfig.mCompletePct < 1f)
			{
				MemoryImage memoryImage = (MemoryImage)GlobalMembersResourcesWP.GetImageById(mData.mObjImageId);
				if (imageConfig.mDestImage != null)
				{
					imageConfig.mDestImage = new MemoryImage();
					imageConfig.mDestImage.mPurgeBits = true;
					imageConfig.mDestImage.SetImageMode(true, true);
					imageConfig.mDestImage.Create(memoryImage.GetWidth(), memoryImage.GetHeight());
				}
				float num = Math.Min(1f - imageConfig.mCompletePct, theUpdatePct);
				GenGrayscaleImage(memoryImage, imageConfig.mDestImage, imageConfig.mIntensity, imageConfig.mContrast, imageConfig.mCompletePct, imageConfig.mCompletePct + num);
				imageConfig.mCompletePct += num;
				theUpdatePct -= num;
				float mCompletePct = imageConfig.mCompletePct;
				float num2 = 1f;
			}
		}
		return theUpdatePct;
	}

	public void GenGrayscaleImage(MemoryImage theImage, MemoryImage theOutImage)
	{
		GenGrayscaleImage(theImage, theOutImage, 1f, 1f, 1f, 1f);
	}

	public void GenGrayscaleImage(MemoryImage theImage, MemoryImage theOutImage, float theIntensity)
	{
		GenGrayscaleImage(theImage, theOutImage, theIntensity, 1f, 1f, 1f);
	}

	public void GenGrayscaleImage(MemoryImage theImage, MemoryImage theOutImage, float theIntensity, float theContrast)
	{
		GenGrayscaleImage(theImage, theOutImage, theIntensity, theContrast, 1f, 1f);
	}

	public void GenGrayscaleImage(MemoryImage theImage, MemoryImage theOutImage, float theIntensity, float theContrast, float theStartPct)
	{
		GenGrayscaleImage(theImage, theOutImage, theIntensity, theContrast, theStartPct, 1f);
	}

	public void GenGrayscaleImage(MemoryImage theImage, MemoryImage theOutImage, float theIntensity, float theContrast, float theStartPct, float theTgtPct)
	{
		uint[] bits = theImage.GetBits();
		uint[] bits2 = theOutImage.GetBits();
		int num = theImage.GetWidth() * theImage.GetHeight();
		int num2 = (int)(theStartPct * (float)num);
		int num3 = (int)(theTgtPct * (float)num);
		for (int i = num2; i < num3; i++)
		{
			int num4 = (int)((double)((bits[i] >> 16) & 0xFF) * 0.3 * (double)theIntensity) + (int)((double)((bits[i] >> 8) & 0xFF) * 0.59 * (double)theIntensity) + (int)((double)(bits[i] & 0xFF) * 0.11 * (double)theIntensity);
			if (theContrast != 1f)
			{
				num4 = (int)(Math.Pow((float)num4 / 255f, theContrast) * 255.0);
			}
			num4 = Math.Min(255, num4);
			bits2[i] = (uint)((uint)((int)((bits[i] & 0xFF000000u) | (uint)(num4 << 16)) | (num4 << 8)) | num4);
		}
	}

	private bool FindImgOrComplain(string theImgName, ref int outId)
	{
		outId = (int)GlobalMembersResourcesWP.GetIdByStringId(theImgName);
		if (outId == -1)
		{
			return false;
		}
		return true;
	}

	public void DrawGrayscaleImage(Graphics g, double theBaseAlpha, double theAddAlpha, double theGemAlpha)
	{
		UpdateGrayscales(10f);
		if (mGrayscaleAddComplete != 1f && mGrayscaleBaseComplete != 1f)
		{
			return;
		}
		g.SetColorizeImages(true);
		if (theBaseAlpha > 0.0)
		{
			g.SetColor(new Color(BejeweledLivePlus.GlobalMembers.M(11167487), (int)(theBaseAlpha * 255.0)));
			Utils.DrawImageCentered(g, mGrayscaleBase, BejeweledLivePlus.GlobalMembers.S(mData.mArtOffsetX), BejeweledLivePlus.GlobalMembers.S(mData.mArtOffsetY));
		}
		if (theAddAlpha > 0.0)
		{
			g.SetDrawMode(Graphics.DrawMode.Additive);
			g.SetColor(new Color(BejeweledLivePlus.GlobalMembers.M(14527231), (int)(theAddAlpha * 255.0)));
			Utils.DrawImageCentered(g, mGrayscaleAdd, BejeweledLivePlus.GlobalMembers.S(mData.mArtOffsetX), BejeweledLivePlus.GlobalMembers.S(mData.mArtOffsetY));
			g.SetDrawMode(Graphics.DrawMode.Normal);
		}
		g.SetColor(new Color(-1));
		g.SetColorizeImages(false);
		if (!(theGemAlpha > 0.0))
		{
			return;
		}
		TranslateToPsd(g);
		g.SetColor(new Color(BejeweledLivePlus.GlobalMembers.M(16777215), (int)(theGemAlpha * 255.0)));
		for (int i = 0; i < BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET; i++)
		{
			string theImgName = $"IMAGE_QUESTOBJECT_{BejeweledLivePlus.GlobalMembers.gObjectImgs[mQuestObjIdx]}_GREEN_GLASS_{i + 1}";
			int outId = 0;
			if (FindImgOrComplain(theImgName, ref outId))
			{
				g.DrawImage(GlobalMembersResourcesWP.GetImageById(outId), (int)BejeweledLivePlus.GlobalMembers.S(GlobalMembersResourcesWP.ImgXOfs(outId)), (int)BejeweledLivePlus.GlobalMembers.S(GlobalMembersResourcesWP.ImgYOfs(outId)));
			}
		}
	}

	public bool WantGrayscale()
	{
		if (mAnimState != EAnimState.eAnim_Revealed && mAnimState != EAnimState.eAnim_Revealing)
		{
			if (mAnimState == EAnimState.eAnim_Unrevealed)
			{
				return mQuestsCompleted == BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET - 1;
			}
			return false;
		}
		return true;
	}

	public void TranslateToPsd(Graphics g)
	{
		mData = default(QuestObjectData);
		Image imageById = GlobalMembersResourcesWP.GetImageById(mData.mObjImageId);
		g.Translate((int)BejeweledLivePlus.GlobalMembers.S((float)mData.mArtOffsetX - GlobalMembersResourcesWP.ImgXOfs(mData.mObjImageId)) - imageById.GetWidth() / 2, (int)BejeweledLivePlus.GlobalMembers.S((float)mData.mArtOffsetY - GlobalMembersResourcesWP.ImgYOfs(mData.mObjImageId)) - imageById.GetHeight() / 2);
	}
}
