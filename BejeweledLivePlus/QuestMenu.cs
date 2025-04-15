using System;
using System.Collections.Generic;
using System.Linq;
using BejeweledLivePlus;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using BejeweledLivePlus.UI;
using BejeweledLivePlus.Widget;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Resource;
using SexyFramework.Widget;
using Common = SexyFramework.Common;

public class QuestMenu : Widget, Bej3ButtonListener, ButtonListener
{
	public struct BtnData
	{
		public int mBaseId;

		public int mIconId;

		public BtnData(int baseId, int iconId)
		{
			mBaseId = baseId;
			mIconId = iconId;
		}
	}

	public enum EProgress
	{
		eProgress_NotShown,
		eProgress_Unrevealed,
		eProgress_Revealed,
		eProgress_Complete
	}

	public struct DeferredSound
	{
		public int mId;

		public int mOnTick;

		public double mVolume;
	}

	public static readonly int QUEST_COUNT = 5;

	public Graphics3D.PerspectiveCamera mCamera;

	public ButtonWidget mRightButton;

	public ButtonWidget mLeftButton;

	public ButtonWidget mBackButton;

	public ButtonWidget mQuitButton;

	public bool mWantBlink;

	public bool mIsOverOrb;

	public Point mOverOrbPos;

	public int mQuestButtonSoundCount;

	public double mQuestSetNumDisp;

	public double mDQuestSetNumDisp;

	public int mLastQuestSetNum;

	public int mQuestSetNum;

	public int mGemOver;

	public Rect mGemRect;

	public Rect mGemRectUnscaled;

	public int mOverCount;

	public Announcement mAnnouncement;

	public CurvedVal mAnnouncementCenterPct;

	public CurvedVal mProgressPaneSlidePct;

	public CurvedVal mButtonsSlidePct;

	public CurvedVal mPercentageAppearPct;

	public CurvedVal mPercentageTextAlpha;

	public CurvedVal mQuestTutorialTextPct;

	public CurvedVal mQuestObjTransitionBtnHidePct;

	public Point mQuestPortalOrigin;

	public int mBreakCountdown;

	public int mDeferButtonId;

	public int mDeferButtonTick;

	public Dictionary<string, BtnData> mQuestToButtonImageIdMap = new Dictionary<string, BtnData>();

	public Background mBackground;

	public PlayMenu mPlayMenu;

	public QuestMenuBtn[] mQuestButtons = new QuestMenuBtn[8];

	public QuestMenuBtn[] mQuestCompleteButtons = new QuestMenuBtn[8];

	public EffectsManager mPreFXManager;

	public EffectsManager mPostFXManager;

	public CurvedVal mCameraHeight;

	public CurvedVal mBkgYOffset;

	public ResourceRef mFanfareBkg;

	public ResourceRef mFanfare;

	public int mFanfareTicks;

	public bool mPlayingFanfare;

	public QuestObject[] mQuestObjs = new QuestObject[QUEST_COUNT];

	public EProgress[] mQuestObjFromProgress = new EProgress[QUEST_COUNT];

	public Rect[] mObjClickRects = new Rect[QUEST_COUNT];

	public CurvedVal mFadeInProgress;

	public bool mDrawGemsAsOverlay;

	public List<DeferredSound> mDeferredSounds = new List<DeferredSound>();

	protected void SetupCamera(Graphics g)
	{
		float num = (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mWidth / (float)SexyFramework.GlobalMembers.gSexyApp.mScreenBounds.mHeight;
		if (g != null)
		{
			num = (float)g.mDestImage.mWidth / (float)g.mDestImage.mHeight;
		}
		float inFovDegrees = BejeweledLivePlus.GlobalMembers.M(38.5f) * num;
		mCamera.Init(inFovDegrees, num, 0.1f, 1000f);
		SexyCoords3 sexyCoords = new SexyCoords3();
		sexyCoords.Translate(0f, BejeweledLivePlus.GlobalMembers.M(-0.75f), 0f);
		mCamera.SetCoords(mCamera.GetCoords().Leave(sexyCoords));
		SexyCoords3 sexyCoords2 = new SexyCoords3();
		sexyCoords2.RotateRadZ((0f - (float)mQuestSetNumDisp) * (BejeweledLivePlus.GlobalMembers.M_PI / 4f));
		mCamera.SetCoords(mCamera.GetCoords().Leave(sexyCoords2));
	}

	public override void Dispose()
	{
		mPreFXManager.Dispose();
		mPostFXManager.Dispose();
		for (int i = 0; i < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; i++)
		{
			mQuestObjs[i] = null;
		}
		BejeweledLivePlus.GlobalMembers.KILL_WIDGET(mBackground);
		mBackground = null;
		RemoveAllWidgets(true, false);
		BejeweledLivePlusApp.UnloadContent("QuestMenu_Shared");
		BejeweledLivePlusApp.LoadContentQuestMenu(-1);
		base.Dispose();
	}

	protected void LoadFanfareResources()
	{
	}

	public QuestMenu(PlayMenu thePlayMenu)
	{
	}

	public void LinkUpButtonImages()
	{
	}

	public FloatyOrb AddFloatyOrb(int theIdx)
	{
		return null;
	}

	public bool IsLastQuestCompleted()
	{
		return BejeweledLivePlus.GlobalMembers.gApp.mProfile.mQuestsCompleted[mQuestSetNum, mGemOver];
	}

	public override void Resize(Rect theRect)
	{
		Resize(theRect);
	}

	public void SetupBackground()
	{
		if (BejeweledLivePlus.GlobalMembers.gApp.Is3DAccelerated())
		{
			mBackground = new Background("images\\1200\\backgrounds\\quest\\quest.pam");
			mBackground.Resize(BejeweledLivePlus.GlobalMembers.S(-160), 0, BejeweledLivePlus.GlobalMembers.S(1920), BejeweledLivePlus.GlobalMembers.S(1200));
		}
		else
		{
			mBackground = new Background("images\\NonResize\\2donly\\questmenu\\background");
			mBackground.mClip = false;
			mBackground.mAllowRealign = false;
			mBackground.Resize(BejeweledLivePlus.GlobalMembers.S(-160), BejeweledLivePlus.GlobalMembers.MS(0), BejeweledLivePlus.GlobalMembers.S(1920), BejeweledLivePlus.GlobalMembers.S(1200));
			mBackground.mImageOverlayAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0.01,1,0.02,1,####         ~~###"));
		}
		PopAnim popAnim = mBackground.GetPopAnim();
		if (popAnim != null)
		{
			popAnim.mDrawScale *= 4f;
			mBackground.mClip = false;
			popAnim.mClip = false;
		}
		BejeweledLivePlus.GlobalMembers.gApp.mWidgetManager.AddWidget(mBackground);
	}

	public void RefreshProgress(EProgress[] theProgressArr)
	{
		bool flag = false;
		for (int i = 0; i < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; i++)
		{
			if (flag)
			{
				theProgressArr[i] = EProgress.eProgress_NotShown;
			}
			else if (mQuestObjs[i].mQuestsCompleted < BejeweledLivePlusAppConstants.QUESTS_REQUIRED_PER_SET)
			{
				flag = true;
				theProgressArr[i] = EProgress.eProgress_Unrevealed;
			}
			else if (mQuestObjs[i].mQuestsCompleted < BejeweledLivePlusAppConstants.QUESTS_PER_SET)
			{
				theProgressArr[i] = EProgress.eProgress_Revealed;
			}
			else
			{
				theProgressArr[i] = EProgress.eProgress_Complete;
			}
		}
	}

	public override void Update()
	{
	}

	public void DrawObject(Graphics g, int theNum, int theX, int theY, bool theIsOverlay)
	{
		float num = (float)((double)theNum - mQuestSetNumDisp);
		if (!(Math.Abs(num) > BejeweledLivePlus.GlobalMembers.M(2f)))
		{
			Graphics3D graphics3D = g.Get3D();
			float num2 = Math.Max(0f, 1f - Math.Abs(num) * BejeweledLivePlus.GlobalMembers.M(0.6f));
			g.SetColor(new Color(255 * (int)num2, 255 * (int)num2, 255 * (int)num2));
			SexyTransform2D theTransform = new SexyTransform2D(true);
			theTransform.Translate(num * (float)BejeweledLivePlus.GlobalMembers.MS(1300), (float)Math.Pow(Math.Abs(num), (double)BejeweledLivePlus.GlobalMembers.M(1.3f) * (double)BejeweledLivePlus.GlobalMembers.MS(100) + (double)mCameraHeight * (double)BejeweledLivePlus.GlobalMembers.MS(1400)));
			g.PushState();
			if (graphics3D != null)
			{
				graphics3D.PushTransform(theTransform);
			}
			else
			{
				g.Translate((int)theTransform.m02, (int)theTransform.m12);
			}
			if (theIsOverlay)
			{
				mQuestObjs[theNum].DrawOverlay(g, theX, theY);
			}
			else
			{
				mQuestObjs[theNum].Draw(g, theX, theY);
			}
			graphics3D?.PopTransform();
			g.PopState();
		}
	}

	public override void Draw(Graphics g)
	{
	}

	public override void DrawOverlay(Graphics g, int thePriority)
	{
	}

	public void DrawTemple(Graphics g)
	{
		DrawTemple2D(g);
	}

	public void DrawTemple2D(Graphics g)
	{
	}

	public int GetCompleteCount()
	{
		int num = 0;
		for (int i = 0; i < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; i++)
		{
			num += mQuestObjs[i].mQuestsCompleted;
		}
		return num;
	}

	public bool WantNextBtnBlink()
	{
		if (mWantBlink && !IsBusy() && (double)mButtonsSlidePct < BejeweledLivePlus.GlobalMembers.M(0.5) && BejeweledLivePlus.GlobalMembers.gApp.GetDialog(39) == null && BejeweledLivePlus.GlobalMembers.gApp.mBoard == null && GetCompleteCount() != BejeweledLivePlusAppConstants.NUM_QUEST_SETS * BejeweledLivePlusAppConstants.QUESTS_PER_SET)
		{
			return !mRightButton.mDisabled;
		}
		return false;
	}

	public bool IsBusy()
	{
		if (!mQuestObjs[mQuestSetNum].IsBusy())
		{
			return mBreakCountdown > 0;
		}
		return true;
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		for (int i = 0; i < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; i++)
		{
			if (mQuestObjFromProgress[i] != 0 && mObjClickRects[i].Contains(BejeweledLivePlus.GlobalMembers.RS(x), BejeweledLivePlus.GlobalMembers.RS(y)))
			{
				if (IsBusy())
				{
					break;
				}
				BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS);
				mQuestSetNum = i;
				BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestPage = mQuestSetNum;
				mWantBlink = false;
				BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestBlink = mWantBlink;
			}
		}
	}

	public void DoCompleteQuest(int theBtnIdx)
	{
		if (!BejeweledLivePlus.GlobalMembers.gApp.mProfile.mQuestsCompleted[mQuestSetNum, theBtnIdx])
		{
			BejeweledLivePlus.GlobalMembers.gApp.mProfile.mQuestsCompleted[mQuestSetNum, theBtnIdx] = true;
			BejeweledLivePlus.GlobalMembers.gApp.mProfile.WriteProfile();
			FloatyOrb floatyOrb = new FloatyOrb();
			floatyOrb.mIdx = mQuestObjs[mQuestSetNum].mQuestsCompleted;
			floatyOrb.mAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,1,0.01,1.5,#### T~###      P~### z####"));
			floatyOrb.mNormalOffset.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;-250,250,0.01,1,Pmri 59o)w  ]####   G51se ^V### .P###"));
			floatyOrb.mLerp.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.01,1,#### X####        H~####~###"));
			floatyOrb.mScale.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;2,5,0.01,1,^###     P3n[*  Bd-_]  1#<$G"));
			if (mAnnouncement != null)
			{
				floatyOrb.mPosFrom.mX = mAnnouncement.mPos.mX;
				floatyOrb.mPosFrom.mY = mAnnouncement.mPos.mY;
			}
			else
			{
				floatyOrb.mPosFrom.mX = mWidgetManager.mLastMouseX;
				floatyOrb.mPosFrom.mY = mWidgetManager.mLastMouseY;
			}
			mQuestObjs[mQuestSetNum].mFloatyOrbs.Add(floatyOrb);
			mDrawGemsAsOverlay = true;
			mQuestObjs[mQuestSetNum].mQuestsCompleted++;
			int num = 0;
			for (int i = 0; i < BejeweledLivePlusAppConstants.NUM_QUEST_SETS; i++)
			{
				num += mQuestObjs[i].mQuestsCompleted;
			}
		}
	}

	public void ButtonPress(int theId)
	{
		if (theId >= 1000)
		{
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_QUEST_MENU_BUTTON1, 0, BejeweledLivePlus.GlobalMembers.M(0.7));
		}
		else if (theId == 0 || theId == 1)
		{
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
		}
	}

	public void ButtonDepress(int theId)
	{
		switch (theId)
		{
		case 0:
		{
			if (IsBusy())
			{
				return;
			}
			mWantBlink = false;
			BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestBlink = mWantBlink;
			int num = (mQuestSetNum + BejeweledLivePlusAppConstants.NUM_QUEST_SETS - 1) % BejeweledLivePlusAppConstants.NUM_QUEST_SETS;
			if (num != BejeweledLivePlusAppConstants.NUM_QUEST_SETS - 1)
			{
				mQuestSetNum = num;
				mDQuestSetNumDisp = mQuestSetNum;
				BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestPage = mQuestSetNum;
			}
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_RELEASE, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
			break;
		}
		case 1:
		{
			if (IsBusy())
			{
				return;
			}
			mWantBlink = false;
			BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestBlink = mWantBlink;
			int num2 = (mQuestSetNum + 1) % BejeweledLivePlusAppConstants.NUM_QUEST_SETS;
			if (num2 != 0)
			{
				mQuestSetNum = num2;
				mDQuestSetNumDisp = mQuestSetNum;
				BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestPage = mQuestSetNum;
			}
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_RELEASE, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
			break;
		}
		case 2:
		{
			if (IsBusy())
			{
				return;
			}
			DeviceImage deviceImage = BejeweledLivePlus.GlobalMembers.gApp.mRestartRT.Lock(BejeweledLivePlus.GlobalMembers.gApp.mScreenBounds.mWidth, BejeweledLivePlus.GlobalMembers.gApp.mScreenBounds.mHeight);
			if (deviceImage != null)
			{
				Graphics graphics = new Graphics(deviceImage);
				graphics.Translate(-BejeweledLivePlus.GlobalMembers.gApp.mScreenBounds.mX, 0);
				bool flag = BejeweledLivePlus.GlobalMembers.gApp.mTooltipManager.mVisible;
				BejeweledLivePlus.GlobalMembers.gApp.mTooltipManager.mVisible = false;
				mWidgetManager.DrawWidgetsTo(graphics);
				BejeweledLivePlus.GlobalMembers.gApp.mTooltipManager.mVisible = flag;
			}
			BejeweledLivePlus.GlobalMembers.gApp.mRestartRT.Unlock();
			BejeweledLivePlus.GlobalMembers.gApp.DoPlayMenu();
			BejeweledLivePlus.GlobalMembers.KILL_WIDGET(mBackground);
			mBackground = null;
			BejeweledLivePlus.GlobalMembers.gApp.ClearUpdateBacklog(false);
			if (mFanfareTicks == 0)
			{
				BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_PRESS, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
			}
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BACKTOMAIN, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
			break;
		}
		case 3:
			BejeweledLivePlus.GlobalMembers.gApp.AskQuit();
			break;
		}
		if (theId < 1000)
		{
			return;
		}
		int num3 = theId - 1000;
		if (num3 >= 1000)
		{
			num3 -= 1000;
		}
		if (IsBusy())
		{
			mDeferButtonId = theId;
			mDeferButtonTick = mUpdateCnt;
			return;
		}
		mWantBlink = false;
		BejeweledLivePlus.GlobalMembers.gApp.mProfile.mLastQuestBlink = mWantBlink;
		mDrawGemsAsOverlay = false;
		mGemOver = num3;
		string text = BejeweledLivePlus.GlobalMembers.gApp.mQuestDataParser.mQuestDataVector[mQuestSetNum * BejeweledLivePlusAppConstants.QUESTS_PER_SET + num3].mParams["Challenge"];
		if (!SexyFramework.GlobalMembers.gIs3D)
		{
			BejeweledLivePlus.GlobalMembers.gApp.mBoard.mBackground.SetVisible(false);
		}
		else
		{
			mBackground.SetVisible(false);
			SetVisible(false);
		}
		BejeweledLivePlus.GlobalMembers.gApp.mBoard.mQuestPortalPct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0.1,6,0.0125,1,#)EK         ~~aWC"));
		BejeweledLivePlus.GlobalMembers.gApp.mBoard.mQuestPortalCenterPct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0,1,#### V*0wJ     W~###  q~###"), BejeweledLivePlus.GlobalMembers.gApp.mBoard.mQuestPortalPct);
		float num4 = 1f / BejeweledLivePlus.GlobalMembers.S(1f);
		mQuestPortalOrigin = (BejeweledLivePlus.GlobalMembers.gApp.mBoard.mQuestPortalOrigin = new Point(mQuestButtons[num3].mX + mQuestButtons[num3].mWidth / 2 - BejeweledLivePlus.GlobalMembers.S(160), mQuestButtons[num3].mY + mQuestButtons[num3].mHeight / 2) * num4);
		BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_CLICKFLYIN, 0, BejeweledLivePlus.GlobalMembers.M(1.0), BejeweledLivePlus.GlobalMembers.M(8.0));
	}

	public override void KeyChar(char theChar)
	{
	}

	public void UpdateDeferredSounds()
	{
		if (mDeferredSounds.Count() == 0)
		{
			return;
		}
		int num = 0;
		while (num < Common.size(mDeferredSounds))
		{
			if (mUpdateCnt >= mDeferredSounds.ToArray()[num].mOnTick)
			{
				BejeweledLivePlus.GlobalMembers.gApp.PlaySample(mDeferredSounds.ToArray()[num].mId, 0, mDeferredSounds.ToArray()[num].mVolume);
				mDeferredSounds.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public void AddDeferredSound(int theSoundId, int theDelayTicks, double theVol)
	{
		DeferredSound item = default(DeferredSound);
		item.mId = theSoundId;
		item.mOnTick = mUpdateCnt + theDelayTicks;
		item.mVolume = theVol;
		mDeferredSounds.Add(item);
	}

	public virtual void ButtonMouseEnter(int theId)
	{
		if (theId >= 1000)
		{
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_QUEST_MENU_BUTTON_MOUSEOVER1, 0, BejeweledLivePlus.GlobalMembers.M(0.5));
		}
		else
		{
			BejeweledLivePlus.GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_BUTTON_MOUSEOVER, 0, BejeweledLivePlus.GlobalMembers.M(1.0));
		}
	}

	public void SetPortalAnnouncement(Announcement theAnnouncement, bool theWon)
	{
		mAnnouncement = theAnnouncement;
		if (!IsLastQuestCompleted() && theWon)
		{
			mBreakCountdown = BejeweledLivePlus.GlobalMembers.M(280);
		}
		mAnnouncementCenterPct.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b+0,1,0.003571,1,~###    $~###    eWs)y ;#=]M"));
	}

	public override void SetVisible(bool isVisible)
	{
		if (isVisible)
		{
			BejeweledLivePlusApp.LoadContent("QuestMenu_Shared");
			BejeweledLivePlusApp.LoadContentQuestMenu(1);
			LinkUpButtonImages();
		}
		else
		{
			BejeweledLivePlusApp.UnloadContent("QuestMenu_Shared");
			BejeweledLivePlusApp.LoadContentQuestMenu(-1);
		}
		base.SetVisible(isVisible);
	}

	public bool ButtonsEnabled()
	{
		throw new NotImplementedException();
	}

	public void ButtonPress(int theId, int theClickCount)
	{
		throw new NotImplementedException();
	}

	public void ButtonDownTick(int theId)
	{
		throw new NotImplementedException();
	}

	public void ButtonMouseLeave(int theId)
	{
		throw new NotImplementedException();
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
		throw new NotImplementedException();
	}
}
