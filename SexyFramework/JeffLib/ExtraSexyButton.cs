using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Sound;
using SexyFramework.Widget;

namespace JeffLib
{
	public class ExtraSexyButton : ButtonWidget
	{
		protected uint[] mMask;

		protected MemoryImage mGIFMask;

		protected int mGIFMaskIgnoreColor;

		protected int mMaskWidth;

		protected bool mDraw;

		protected bool mStopExcludedSounds;

		protected bool mBlink;

		protected List<SoundInstance> mMouseOverExclusionList = new List<SoundInstance>();

		public Animator mOverAnimation = new Animator();

		public Animator mDownAnimation = new Animator();

		public Animator mButtonAnimation = new Animator();

		public Color mOverColor = default(Color);

		public Color mDownColor = default(Color);

		public SoundInstance mMouseOverSnd;

		public int mMouseOverSndID;

		public int mPitchShift;

		public bool mAdditiveOver;

		public bool mAdditiveDown;

		public bool mSyncFrames;

		public bool mUsesAnimators;

		public ExtraSexyButton(int theId, ButtonListener theButtonListener)
			: base(theId, theButtonListener)
		{
			mUsesAnimators = true;
			mGIFMaskIgnoreColor = 0;
			mGIFMask = null;
			mDraw = true;
			mBlink = false;
			mSyncFrames = false;
			mStopExcludedSounds = true;
			mButtonAnimation.PauseAnim(false);
			mButtonAnimation.SetPingPong(false);
			mButtonAnimation.SetDelay(10);
			mButtonAnimation.SetMaxFrames(1);
			mButtonAnimation.StopWhenDone(false);
			mOverAnimation.PauseAnim(true);
			mOverAnimation.SetPingPong(false);
			mOverAnimation.SetDelay(10);
			mOverAnimation.SetMaxFrames(1);
			mOverAnimation.StopWhenDone(false);
			mDownAnimation.PauseAnim(true);
			mDownAnimation.SetPingPong(false);
			mDownAnimation.SetDelay(10);
			mDownAnimation.SetMaxFrames(1);
			mDownAnimation.StopWhenDone(false);
			mMouseOverSnd = null;
			mMaskWidth = 0;
			mMouseOverSndID = -1;
			mDownImage = null;
			mOverImage = null;
			mButtonImage = null;
			mDoFinger = true;
			mPitchShift = int.MaxValue;
			mAdditiveDown = (mAdditiveOver = false);
			mOverColor = (mDownColor = new Color(0, 0, 0, 0));
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override bool IsPointVisible(int pX, int pY)
		{
			if (pX >= mWidth || pY >= mHeight || pX < 0 || pY < 0)
			{
				return false;
			}
			if (mMask == null && mGIFMask == null)
			{
				return true;
			}
			if (mMask != null)
			{
				int num = pY * mMaskWidth + pX;
				uint num2 = mMask[num];
				if (num2 == uint.MaxValue)
				{
					return true;
				}
			}
			else if (mGIFMask != null)
			{
				int num3 = pY * mGIFMask.mWidth + pX;
				uint num4;
				if (mGIFMask.mColorIndices != null)
				{
					byte b = mGIFMask.mColorIndices[num3];
					num4 = mGIFMask.mColorTable[b];
				}
				else
				{
					num4 = mGIFMask.GetBits()[num3];
				}
				if (num4 != mGIFMaskIgnoreColor)
				{
					return true;
				}
			}
			return false;
		}

		public override void MouseEnter()
		{
			base.MouseEnter();
			if (mButtonListener != null)
			{
				mButtonListener.ButtonMouseEnter(mId);
			}
			if (mMouseOverSnd != null && !mMouseOverSnd.IsPlaying())
			{
				bool flag = true;
				for (int i = 0; i < mMouseOverExclusionList.Count; i++)
				{
					if (mMouseOverExclusionList[i].IsPlaying())
					{
						if (!mStopExcludedSounds)
						{
							flag = false;
							break;
						}
						mMouseOverExclusionList[i].Stop();
					}
				}
				if (flag || mStopExcludedSounds)
				{
					if (mPitchShift != int.MaxValue)
					{
						mMouseOverSnd.AdjustPitch(mPitchShift);
					}
					mMouseOverSnd.Play(false, false);
				}
			}
			else if (mMouseOverSndID != -1)
			{
				if (mPitchShift != int.MaxValue)
				{
					SoundInstance soundInstance = GlobalMembers.gSexyAppBase.mSoundManager.GetSoundInstance(mMouseOverSndID);
					if (soundInstance != null)
					{
						soundInstance.AdjustPitch(mPitchShift);
						soundInstance.Play(false, true);
					}
				}
				else
				{
					GlobalMembers.gSexyApp.PlaySample(mMouseOverSndID);
				}
			}
			if (mOverAnimation.IsPaused() && !mIsDown)
			{
				mOverAnimation.ResetAnim();
				mOverAnimation.PauseAnim(false);
				if (mSyncFrames && mOverAnimation.GetMaxFrames() > mButtonAnimation.GetFrame())
				{
					mOverAnimation.mUpdateCnt = mButtonAnimation.mUpdateCnt;
					mOverAnimation.SetFrame(mButtonAnimation.GetFrame());
				}
				mButtonAnimation.PauseAnim(true);
			}
			else
			{
				if (!mIsDown || !mDownAnimation.IsPaused())
				{
					return;
				}
				if (mDownImage != null)
				{
					mOverAnimation.PauseAnim(true);
				}
				else
				{
					mOverAnimation.ResetAnim();
					mOverAnimation.PauseAnim(false);
					if (mSyncFrames && mOverAnimation.GetMaxFrames() > mButtonAnimation.GetFrame())
					{
						mOverAnimation.mUpdateCnt = mButtonAnimation.mUpdateCnt;
						mOverAnimation.SetFrame(mButtonAnimation.GetFrame());
					}
				}
				mButtonAnimation.PauseAnim(true);
				mDownAnimation.ResetAnim();
				mDownAnimation.PauseAnim(false);
				if (mSyncFrames && mDownAnimation.GetMaxFrames() > mOverAnimation.GetFrame())
				{
					mDownAnimation.mUpdateCnt = mButtonAnimation.mUpdateCnt;
					mDownAnimation.SetFrame(mButtonAnimation.GetFrame());
				}
			}
		}

		public override void MouseLeave()
		{
			base.MouseLeave();
			if (mButtonListener != null)
			{
				mButtonListener.ButtonMouseLeave(mId);
			}
			Animator animator = null;
			if (!mOverAnimation.IsPaused())
			{
				animator = mOverAnimation;
			}
			else if (!mDownAnimation.IsPaused())
			{
				animator = mDownAnimation;
			}
			mOverAnimation.PauseAnim(true);
			mDownAnimation.PauseAnim(true);
			mButtonAnimation.ResetAnim();
			mButtonAnimation.PauseAnim(false);
			if (mSyncFrames && mButtonAnimation.GetMaxFrames() > animator.GetFrame())
			{
				mButtonAnimation.mUpdateCnt = animator.mUpdateCnt;
				mButtonAnimation.SetFrame(animator.GetFrame());
			}
		}

		public override void MouseDown(int pX, int pY, int pClickCount)
		{
			base.MouseDown(pX, pY, pClickCount);
			if (mDownImage != null)
			{
				mOverAnimation.PauseAnim(true);
			}
			else
			{
				mOverAnimation.ResetAnim();
				mOverAnimation.PauseAnim(false);
				if (mSyncFrames && mOverAnimation.GetMaxFrames() > mButtonAnimation.GetFrame())
				{
					mOverAnimation.mUpdateCnt = mButtonAnimation.mUpdateCnt;
					mOverAnimation.SetFrame(mButtonAnimation.GetFrame());
				}
			}
			mButtonAnimation.PauseAnim(true);
			mDownAnimation.ResetAnim();
			mDownAnimation.PauseAnim(false);
			if (mSyncFrames && mDownAnimation.GetMaxFrames() > mOverAnimation.GetFrame())
			{
				mDownAnimation.mUpdateCnt = mOverAnimation.mUpdateCnt;
				mDownAnimation.SetFrame(mOverAnimation.GetFrame());
			}
		}

		public override void MouseUp(int pX, int pY)
		{
			base.MouseUp(pX, pY);
			if (mIsOver)
			{
				Animator animator = null;
				if (!mButtonAnimation.IsPaused())
				{
					animator = mButtonAnimation;
				}
				else if (!mDownAnimation.IsPaused())
				{
					animator = mDownAnimation;
				}
				mDownAnimation.PauseAnim(true);
				mButtonAnimation.PauseAnim(true);
				mOverAnimation.ResetAnim();
				mOverAnimation.PauseAnim(false);
				if (mSyncFrames && animator != null && mOverAnimation.GetMaxFrames() > animator.GetFrame())
				{
					mOverAnimation.mUpdateCnt = animator.mUpdateCnt;
					mOverAnimation.SetFrame(animator.GetFrame());
				}
			}
		}

		public override void Update()
		{
			base.Update();
			if (!mDisabled && GlobalMembers.gSexyApp.mHasFocus)
			{
				mButtonAnimation.UpdateAnim();
				mOverAnimation.UpdateAnim();
				mDownAnimation.UpdateAnim();
				if (mButtonAnimation.FrameChanged() || mOverAnimation.FrameChanged() || mDownAnimation.FrameChanged() || mBlink)
				{
					MarkDirty();
				}
			}
		}

		public override void Draw(Graphics pGfx)
		{
			if (!mDraw)
			{
				return;
			}
			if (!mUsesAnimators)
			{
				base.Draw(pGfx);
				return;
			}
			int theX = 0;
			int theY = 0;
			if (mFont != null)
			{
				theX = (mWidth - mFont.StringWidth(mLabel)) / 2;
				theY = (mHeight + mFont.GetAscent() - mFont.GetAscent() / 6 - 1) / 2;
			}
			Image image = ((mOverImage == null) ? mButtonImage : mOverImage);
			if (mDisabled)
			{
				base.Draw(pGfx);
			}
			else if (!mButtonAnimation.IsPaused() && mButtonImage != null)
			{
				pGfx.DrawImageCel(mButtonImage, 0, 0, mButtonAnimation.GetFrame());
			}
			else if (!mDownAnimation.IsPaused() && mDownImage != null)
			{
				if (!mBlink && mAdditiveDown)
				{
					pGfx.SetDrawMode(0);
					pGfx.SetColorizeImages(true);
					pGfx.SetColor(mDownColor);
				}
				pGfx.DrawImageCel(mDownImage, 0, 0, mDownAnimation.GetFrame());
				if (!mBlink && mAdditiveDown)
				{
					pGfx.SetDrawMode(0);
					pGfx.SetColorizeImages(false);
				}
			}
			else if (!mDownAnimation.IsPaused() && mDownImage == null)
			{
				if (image != null)
				{
					if (!mBlink && mAdditiveDown)
					{
						pGfx.SetDrawMode(1);
						pGfx.SetColorizeImages(true);
						pGfx.SetColor(mDownColor);
					}
					pGfx.DrawImageCel(image, 1, 1, mOverAnimation.GetFrame());
					if (!mBlink && mAdditiveDown)
					{
						pGfx.SetDrawMode(0);
						pGfx.SetColorizeImages(false);
					}
				}
			}
			else if (!mOverAnimation.IsPaused() && image != null)
			{
				if (!mBlink && mAdditiveOver)
				{
					pGfx.SetDrawMode(1);
					pGfx.SetColorizeImages(true);
					pGfx.SetColor(mOverColor);
				}
				pGfx.DrawImageCel(image, 0, 0, mOverAnimation.GetFrame());
				if (!mBlink && mAdditiveOver)
				{
					pGfx.SetDrawMode(0);
					pGfx.SetColorizeImages(false);
				}
			}
			if (mBlink && !mIsOver)
			{
				mIsOver = true;
				int num = mUpdateCnt % 100;
				if (num > 50)
				{
					num = 100 - num;
				}
				pGfx.SetColor(255, 255, 255, 255 * num / 50);
				pGfx.SetColorizeImages(true);
				pGfx.SetDrawMode(1);
				if (mDisabled)
				{
					base.Draw(pGfx);
				}
				else if (!mButtonAnimation.IsPaused() && mButtonImage != null)
				{
					pGfx.DrawImageCel(mButtonImage, 0, 0, mButtonAnimation.GetFrame());
				}
				else if (!mButtonAnimation.IsPaused() && mButtonImage == null && mOverImage != null)
				{
					num = mUpdateCnt % 254;
					if (num > 127)
					{
						num = 254 - num;
					}
					pGfx.SetColor(255, 255, 255, num);
					pGfx.DrawImageCel(mOverImage, 0, 0, 0);
				}
				else if (!mDownAnimation.IsPaused() && mDownImage != null)
				{
					pGfx.DrawImageCel(mDownImage, 0, 0, mDownAnimation.GetFrame());
				}
				else if (!mDownAnimation.IsPaused() && mDownImage == null && image != null)
				{
					pGfx.DrawImageCel(image, 1, 1, mOverAnimation.GetFrame());
				}
				else if (!mOverAnimation.IsPaused() && image != null)
				{
					pGfx.DrawImageCel(image, 0, 0, mOverAnimation.GetFrame());
				}
				pGfx.SetDrawMode(0);
				pGfx.SetColorizeImages(false);
				mIsOver = false;
			}
			if (mFont != null)
			{
				if (mIsOver)
				{
					pGfx.SetColor(mColors[1]);
				}
				else
				{
					pGfx.SetColor(mColors[0]);
				}
				pGfx.SetFont(mFont);
				pGfx.DrawString(mLabel, theX, theY);
			}
		}

		public void DrawBoundingBox(Graphics pGfx)
		{
			pGfx.SetColor(255, 255, 255, 128);
			pGfx.DrawRect(0, 0, mWidth, mHeight);
		}

		public void SetMask(uint[] pMask, int pWidth)
		{
			mMask = pMask;
			mMaskWidth = pWidth;
		}

		public void SetMask(MemoryImage gif_mask, int ignore_color)
		{
			mGIFMask = gif_mask;
			mGIFMaskIgnoreColor = ignore_color;
		}

		public void SetDraw(bool pDraw)
		{
			mDraw = pDraw;
		}

		public void SetStopExcludedSounds(bool pS)
		{
			mStopExcludedSounds = pS;
		}

		public void SetBlink(bool pBlink)
		{
			mBlink = pBlink;
		}
	}
}
