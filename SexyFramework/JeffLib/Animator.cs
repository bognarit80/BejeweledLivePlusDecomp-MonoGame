using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace JeffLib
{
	public class Animator
	{
		protected int mCurrentFrame;

		protected int mMaxFrames;

		protected int mFrameDelay;

		protected int mNumIterations;

		protected int mLoopCount;

		protected int mLoopStart;

		protected int mLoopEnd;

		protected int mLoopDir;

		protected int mStepAmt;

		protected int mId;

		protected int mPriority;

		protected int mTimeLimit;

		protected int mCurrentTime;

		protected float mXOff;

		protected float mYOff;

		protected bool mPaused;

		protected bool mPingPong;

		protected bool mAnimForward;

		protected bool mStopWhenDone;

		protected bool mDone;

		protected bool mFrameChanged;

		protected bool mLoopSubsection;

		protected bool mDrawAdditive;

		protected bool mDrawColorized;

		protected bool mDrawRandomly;

		protected Color mAdditiveColor;

		protected Color mColorizeVal;

		protected Image mImage;

		protected FadeData mFadeData = new FadeData();

		protected List<int> mFrameDelays = new List<int>();

		protected List<int> mRandomFrames = new List<int>();

		public int mUpdateCnt;

		public bool mCanRotate;

		public bool mResetOnStart;

		protected void UpdateFadeData()
		{
			if (mFadeData.mFadeState == 2)
			{
				if ((mFadeData.mVal += mFadeData.mFadeInRate) >= mFadeData.mFadeInTarget)
				{
					mFadeData.mVal = mFadeData.mFadeInTarget;
					if (mFadeData.mFadeCount > 0)
					{
						mFadeData.mFadeCount--;
					}
					mFadeData.mFadeState = 1;
				}
			}
			else if ((mFadeData.mVal -= mFadeData.mFadeOutRate) <= mFadeData.mFadeOutTarget)
			{
				mFadeData.mVal = mFadeData.mFadeOutTarget;
				if (mFadeData.mFadeCount > 0)
				{
					mFadeData.mFadeCount--;
				}
				mFadeData.mFadeState = 2;
			}
			if (mFadeData.mFadeCount == 0)
			{
				mFadeData.mFadeState = 0;
				if (mFadeData.mStopWhenDone)
				{
					mDone = true;
				}
			}
		}

		protected void _Init()
		{
		}

		public Animator()
		{
			_Init();
		}

		public Animator(Animator a)
		{
			a.CopyTo(this);
		}

		public virtual bool UpdateAnim(bool change_loop_count)
		{
			mUpdateCnt++;
			mFrameChanged = false;
			if (mTimeLimit > 0 && !mPaused && !mDone && ++mCurrentTime >= mTimeLimit)
			{
				mPaused = true;
				mDone = true;
			}
			if (!mPaused && !mDone)
			{
				int num = ((!mPingPong) ? ((mLoopDir <= 0) ? (-1) : 0) : (mAnimForward ? (-1) : 0));
				int num2 = mMaxFrames;
				int num3 = ((mFrameDelays.Count > 0 && mMaxFrames > 1) ? mFrameDelays[mCurrentFrame] : mFrameDelay);
				if (mLoopSubsection)
				{
					num = mLoopStart - 1;
					num2 = mLoopEnd + 1;
				}
				if (num3 != 0 && mUpdateCnt % num3 == 0 && (mNumIterations == 0 || mLoopCount <= mNumIterations))
				{
					mFrameChanged = true;
					if (!mPingPong || mDrawRandomly)
					{
						if (!mDrawRandomly)
						{
							if (mLoopDir >= 0)
							{
								if ((mCurrentFrame += mStepAmt) >= num2)
								{
									if (mLoopSubsection)
									{
										num = mLoopStart;
									}
									mCurrentFrame = num;
									if (change_loop_count)
									{
										mLoopCount++;
									}
									if (mStopWhenDone || (mLoopCount >= mNumIterations && mNumIterations > 0))
									{
										mPaused = true;
										mDone = true;
									}
								}
							}
							else if ((mCurrentFrame -= mStepAmt) <= num)
							{
								if (mLoopSubsection)
								{
									num2 = mLoopEnd;
								}
								mCurrentFrame = num2 - 1;
								if (change_loop_count)
								{
									mLoopCount++;
								}
								if (mStopWhenDone || (mLoopCount >= mNumIterations && mNumIterations > 0))
								{
									mPaused = true;
									mDone = true;
								}
							}
						}
						else
						{
							if (mRandomFrames.Count == 0)
							{
								if (change_loop_count)
								{
									mLoopCount++;
								}
								if (mStopWhenDone || (mLoopCount >= mNumIterations && mNumIterations > 0))
								{
									mPaused = true;
									mDone = true;
								}
								for (int i = 0; i < mMaxFrames; i++)
								{
									mRandomFrames.Add(i);
								}
							}
							mCurrentFrame = SexyFramework.Common.Rand() % mRandomFrames.Count;
							mRandomFrames.RemoveAt(mCurrentFrame);
						}
					}
					else if (!mAnimForward)
					{
						if ((mCurrentFrame += mStepAmt) >= num2)
						{
							if (change_loop_count)
							{
								mLoopCount++;
							}
							mCurrentFrame = num2 - 2;
							if (mMaxFrames == 1)
							{
								mCurrentFrame = 0;
							}
							mAnimForward = true;
							if (mLoopCount >= mNumIterations && mNumIterations > 0)
							{
								mPaused = (mDone = true);
							}
						}
					}
					else if (mAnimForward && (mCurrentFrame -= mStepAmt) <= num)
					{
						if (change_loop_count)
						{
							mLoopCount++;
						}
						if (mLoopSubsection)
						{
							mCurrentFrame = num + 2;
						}
						else
						{
							mCurrentFrame = ((num < 0) ? (num + 2) : (num + 1));
						}
						if (mMaxFrames == 1 && mCurrentFrame == 1)
						{
							mCurrentFrame--;
						}
						mAnimForward = false;
						if (mStopWhenDone || (mLoopCount >= mNumIterations && mNumIterations > 0))
						{
							mCurrentFrame = num;
							mPaused = true;
							mDone = true;
						}
					}
				}
				if (mFadeData.mFadeState != 0)
				{
					UpdateFadeData();
				}
			}
			if (mMaxFrames == 1)
			{
				mFrameChanged = false;
			}
			return mFrameChanged;
		}

		public virtual bool UpdateAnim()
		{
			return UpdateAnim(true);
		}

		public virtual void PauseAnim(bool pPause)
		{
			mPaused = pPause;
		}

		public virtual void ResetAnim()
		{
			mPaused = false;
			mCurrentFrame = ((mLoopDir < 0) ? (mMaxFrames - 1) : 0);
			mDone = false;
			mLoopCount = 0;
			mLoopStart = 0;
			mLoopEnd = mMaxFrames;
			mLoopSubsection = false;
			mCurrentTime = 0;
			mUpdateCnt = 0;
		}

		public virtual void Clear()
		{
			mPaused = true;
			mUpdateCnt = (mCurrentFrame = 0);
			mFrameDelay = 1;
			mMaxFrames = 1;
			mLoopDir = 1;
			mPingPong = false;
			mAnimForward = true;
			mLoopSubsection = false;
			mStopWhenDone = false;
			mDone = false;
			mFrameChanged = true;
			mLoopCount = (mNumIterations = 0);
			mStepAmt = 1;
			mImage = null;
			mXOff = (mYOff = 0f);
			mLoopStart = (mLoopEnd = 0);
			mId = -1;
			mPriority = 0;
			mResetOnStart = false;
			mCanRotate = false;
			mDrawAdditive = false;
			mDrawColorized = false;
			mDrawRandomly = false;
			mTimeLimit = (mCurrentTime = -1);
			mFadeData = new FadeData();
		}

		public virtual void LoopSubsection(int pStartFrame, int pEndFrame)
		{
			mLoopSubsection = true;
			mLoopStart = pStartFrame;
			mLoopEnd = pEndFrame;
		}

		public virtual void SetLoopDir(int pDir)
		{
			mLoopDir = pDir;
			if (pDir < 0)
			{
				mCurrentFrame = mMaxFrames - 1;
			}
		}

		public virtual void SetTimeLimit(int t)
		{
			mTimeLimit = t;
			mCurrentTime = 0;
		}

		private void START_ADDITIVE(Graphics g, Color val)
		{
			g.SetDrawMode(1);
			if (mFadeData.mFadeState == 0)
			{
				g.SetColor(val);
			}
			else
			{
				g.SetColor(val.mRed, val.mGreen, val.mBlue, mFadeData.mVal);
			}
			g.SetColorizeImages(true);
		}

		private void START_ADDITIVE(Graphics g)
		{
			g.SetDrawMode(0);
			g.SetColorizeImages(false);
		}

		private void START_COLORIZED(Graphics g, Color val)
		{
			g.SetColorizeImages(true);
			if (mFadeData.mFadeState == 0)
			{
				g.SetColor(val);
			}
			else
			{
				g.SetColor(val.mRed, val.mGreen, val.mBlue, mFadeData.mVal);
			}
		}

		private void STOP_COLORIZED(Graphics g)
		{
			g.SetColorizeImages(false);
		}

		private void START_FADE(Graphics g)
		{
			if (!mDrawAdditive && !mDrawColorized && mFadeData.mFadeState != 0)
			{
				g.SetColor(255, 255, 255, mFadeData.mVal);
				g.SetColorizeImages(true);
			}
		}

		private void STOP_FADE(Graphics g)
		{
			if (!mDrawAdditive && !mDrawColorized && mFadeData.mFadeState != 0)
			{
				g.SetColorizeImages(false);
			}
		}

		private void SETUP_DRAWING(Graphics g)
		{
			START_FADE(g);
			if (mDrawAdditive)
			{
				START_ADDITIVE(g, mAdditiveColor);
			}
			else if (mDrawColorized)
			{
				START_COLORIZED(g, mColorizeVal);
			}
		}

		private void END_DRAWING(Graphics g)
		{
			STOP_FADE(g);
			if (mDrawAdditive)
			{
				START_ADDITIVE(g, mAdditiveColor);
			}
			else if (mDrawColorized)
			{
				START_COLORIZED(g, mColorizeVal);
			}
		}

		public virtual void DrawAdditively(Color pColor)
		{
			StopDrawingColorized();
			mDrawAdditive = true;
			mAdditiveColor = pColor;
		}

		public virtual void StopDrawingAdditively()
		{
			mDrawAdditive = false;
		}

		public virtual void DrawColorized(Color pColor)
		{
			StopDrawingAdditively();
			mDrawColorized = true;
			mColorizeVal = pColor;
		}

		public virtual void StopDrawingColorized()
		{
			mDrawColorized = false;
		}

		public virtual void SetMaxFrames(int pMax)
		{
			mMaxFrames = pMax;
			mCurrentFrame = 0;
			if (mLoopDir < 0)
			{
				mCurrentFrame = mMaxFrames - 1;
			}
			mFrameDelays.Clear();
			for (int i = 0; i < mFrameDelays.Count; i++)
			{
				mFrameDelays.Add(mFrameDelay);
			}
		}

		public virtual void SetImage(Image pImage)
		{
			mImage = null;
			mCurrentFrame = 0;
			if (pImage != null)
			{
				SetMaxFrames((pImage.mNumCols > pImage.mNumRows) ? pImage.mNumCols : pImage.mNumRows);
			}
			mImage = pImage;
		}

		public virtual void Draw(Graphics g, int pX, int pY)
		{
			if (mImage != null && !IsPaused() && !IsDone())
			{
				SETUP_DRAWING(g);
				g.DrawImageCel(mImage, pX + (int)mXOff, pY + (int)mYOff, GetFrame());
				END_DRAWING(g);
			}
		}

		public virtual void Draw(Graphics g, float pX, float pY, bool pSmooth)
		{
			if (mImage == null || IsPaused() || IsDone())
			{
				return;
			}
			SETUP_DRAWING(g);
			if (pSmooth)
			{
				int theX = 0;
				int theY = 0;
				if (mImage.mNumCols > mImage.mNumRows)
				{
					theX = GetFrame() * mImage.GetCelWidth();
					theY = 0;
				}
				else if (mImage.mNumRows > mImage.mNumCols)
				{
					theX = 0;
					theY = GetFrame() * mImage.GetCelHeight();
				}
				g.DrawImageF(mImage, pX + mXOff, pY + mYOff, new Rect(theX, theY, mImage.GetCelWidth(), mImage.GetCelHeight()));
			}
			else
			{
				g.DrawImageCel(mImage, (int)(pX + mXOff), (int)(pY + mYOff), GetFrame());
			}
			END_DRAWING(g);
		}

		public virtual void DrawStretched(Graphics g, float pX, float pY, float pPct)
		{
			if (mImage != null && !IsPaused() && !IsDone())
			{
				SETUP_DRAWING(g);
				float num = (float)mImage.GetCelWidth() * pPct;
				float num2 = (float)mImage.GetCelHeight() * pPct;
				g.DrawImageCel(theDestRect: new Rect((int)(pX + mXOff), (int)(pY + mYOff), (int)num, (int)num2), theImageStrip: mImage, theCel: GetFrame());
				END_DRAWING(g);
			}
		}

		public virtual void DrawStretched(Graphics g, float pX, float pY, int pWidth, int pHeight)
		{
			if (mImage != null && !IsPaused() && !IsDone())
			{
				SETUP_DRAWING(g);
				g.DrawImageCel(theDestRect: new Rect((int)(pX + mXOff), (int)(pY + mYOff), pWidth, pHeight), theImageStrip: mImage, theCel: GetFrame());
				END_DRAWING(g);
			}
		}

		public virtual void DrawRotated(Graphics g, float pX, float pY, float pAngle, bool pSmooth, float pCenterX, float pCenterY)
		{
			if (mImage == null || IsPaused() || IsDone())
			{
				return;
			}
			SETUP_DRAWING(g);
			if (pSmooth)
			{
				int theX = 0;
				int theY = 0;
				if (mImage.mNumCols > mImage.mNumRows)
				{
					theX = GetFrame() * mImage.GetCelWidth();
					theY = 0;
				}
				else if (mImage.mNumRows > mImage.mNumCols)
				{
					theX = 0;
					theY = GetFrame() * mImage.GetCelHeight();
				}
				g.DrawImageRotatedF(theSrcRect: new Rect(theX, theY, mImage.GetCelWidth(), mImage.GetCelHeight()), theImage: mImage, theX: pX + mXOff, theY: pY + mYOff, theRot: pAngle, theRotCenterX: pCenterX, theRotCenterY: pCenterY);
			}
			else
			{
				Rect celRect = mImage.GetCelRect(GetFrame());
				g.DrawImageRotated(mImage, (int)(pX + mXOff), (int)(pY + mYOff), pAngle, (int)pCenterX, (int)pCenterY, celRect);
			}
			END_DRAWING(g);
		}

		public virtual void DrawRandomly(bool pRandom)
		{
			mDrawRandomly = pRandom;
			if (mDrawRandomly)
			{
				mRandomFrames.Clear();
				for (int i = 0; i < mMaxFrames; i++)
				{
					mRandomFrames.Add(i);
				}
			}
		}

		public virtual void SetNumIterations(int aIt)
		{
			mNumIterations = aIt;
			mLoopCount = 0;
		}

		public virtual void SetFrame(int pFrame)
		{
			mCurrentFrame = pFrame;
		}

		public virtual void SetDelay(int pDelay)
		{
			mFrameDelay = pDelay;
			if (mFrameDelays.Count > 0)
			{
				for (int i = 0; i < mFrameDelays.Count; i++)
				{
					mFrameDelays[i] = pDelay;
				}
			}
		}

		public virtual void SetDelay(int pDelay, int pFrame)
		{
			mFrameDelays[pFrame] = pDelay;
			mFrameDelay = pDelay;
		}

		public virtual bool FrameChanged()
		{
			if (mFrameChanged)
			{
				mFrameChanged = false;
				return true;
			}
			return false;
		}

		public virtual void CopyTo(Animator rhs)
		{
			if (this != rhs)
			{
				mImage = null;
				if (rhs.mFadeData == null)
				{
					mFadeData = null;
				}
				else
				{
					mFadeData = new FadeData(rhs.mFadeData);
				}
				mTimeLimit = rhs.mTimeLimit;
				mCurrentTime = rhs.mCurrentTime;
				mUpdateCnt = rhs.mUpdateCnt;
				mAnimForward = rhs.mAnimForward;
				mDone = rhs.mDone;
				mFrameChanged = rhs.mFrameChanged;
				mFrameDelay = rhs.mFrameDelay;
				SetMaxFrames(rhs.mMaxFrames);
				mPaused = rhs.mPaused;
				mPingPong = rhs.mPingPong;
				mStopWhenDone = rhs.mStopWhenDone;
				if (rhs.mImage != null)
				{
					SetImage(rhs.mImage);
				}
				mNumIterations = rhs.mNumIterations;
				mLoopCount = rhs.mLoopCount;
				mLoopStart = rhs.mLoopStart;
				mLoopEnd = rhs.mLoopEnd;
				mLoopDir = rhs.mLoopDir;
				mStepAmt = rhs.mStepAmt;
				mLoopSubsection = rhs.mLoopSubsection;
				mXOff = rhs.mXOff;
				mYOff = rhs.mYOff;
				mPriority = rhs.mPriority;
				mId = rhs.mId;
				mDrawAdditive = rhs.mDrawAdditive;
				mAdditiveColor = rhs.mAdditiveColor;
			}
		}

		public bool IsDone()
		{
			if (!mDone)
			{
				if (mNumIterations > 0)
				{
					return mLoopCount >= mNumIterations;
				}
				return false;
			}
			return true;
		}

		public bool IsPaused()
		{
			return mPaused;
		}

		public bool PingPongs()
		{
			return mPingPong;
		}

		public bool IsPlaying()
		{
			if (!IsDone())
			{
				return !IsPaused();
			}
			return false;
		}

		public bool StopWhenDone()
		{
			return mStopWhenDone;
		}

		public int GetFrame()
		{
			if (mMaxFrames != 1)
			{
				return mCurrentFrame;
			}
			return 0;
		}

		public int GetMaxFrames()
		{
			return mMaxFrames;
		}

		public int GetDelay()
		{
			return mFrameDelay;
		}

		public int GetStepAmt()
		{
			return mStepAmt;
		}

		public int GetId()
		{
			return mId;
		}

		public int GetPriority()
		{
			return mPriority;
		}

		public int GetTimeLimit()
		{
			return mTimeLimit;
		}

		public int GetLoopStart()
		{
			return mLoopStart;
		}

		public int GetLoopEnd()
		{
			return mLoopEnd;
		}

		public float GetXOff()
		{
			return mXOff;
		}

		public float GetYOff()
		{
			return mYOff;
		}

		public Image GetImage()
		{
			return mImage;
		}

		public void SetPingPong(bool pPong)
		{
			mPingPong = pPong;
		}

		public void StopWhenDone(bool pStop)
		{
			mStopWhenDone = pStop;
		}

		public void SetStepAmount(int pStep)
		{
			mStepAmt = pStep;
		}

		public void SetXOffset(float x)
		{
			mXOff = x;
		}

		public void SetYOffset(float y)
		{
			mYOff = y;
		}

		public void SetXYOffset(float x, float y)
		{
			SetXOffset(x);
			SetYOffset(y);
		}

		public void SetId(int id)
		{
			mId = id;
		}

		public void SetPriority(int p)
		{
			mPriority = p;
		}

		public void SetDone()
		{
			mDone = true;
		}

		public void ResetTime()
		{
			mCurrentTime = 0;
		}

		public FadeData GetFadeData()
		{
			return mFadeData;
		}

		public int GetFadeOutRate()
		{
			return mFadeData.mFadeOutRate;
		}

		public int GetFadeOutTarget()
		{
			return mFadeData.mFadeOutTarget;
		}

		public int GetFadeInRate()
		{
			return mFadeData.mFadeInRate;
		}

		public int GetFadeInTarget()
		{
			return mFadeData.mFadeInTarget;
		}

		public int GetFadeVal()
		{
			return mFadeData.mVal;
		}

		public int GetFadeCount()
		{
			return mFadeData.mFadeCount;
		}

		public bool GetFadeStopWhenDone()
		{
			return mFadeData.mStopWhenDone;
		}

		public void SetFadeData(FadeData fd)
		{
			mFadeData = fd;
		}

		public void SetFadeOutRate(int r)
		{
			mFadeData.mFadeOutRate = r;
		}

		public void SetFadeOutTarget(int t)
		{
			mFadeData.mFadeOutTarget = t;
		}

		public void SetFadeInRate(int r)
		{
			mFadeData.mFadeInRate = r;
		}

		public void SetFadeInTarget(int t)
		{
			mFadeData.mFadeInTarget = t;
		}

		public void SetFadeVal(int v)
		{
			mFadeData.mVal = v;
		}

		public void SetFadeCount(int c)
		{
			mFadeData.mFadeCount = c;
		}

		public void SetFadeStopWhenDone(bool d)
		{
			mFadeData.mStopWhenDone = d;
		}

		public void FadeIn()
		{
			mFadeData.mFadeState = 2;
		}

		public void FadeIn(int rate, int target)
		{
			FadeIn();
			SetFadeInRate(rate);
			SetFadeInTarget(target);
		}

		public void FadeOut()
		{
			mFadeData.mFadeState = 1;
		}

		public void FadeOut(int rate, int target)
		{
			FadeOut();
			SetFadeOutRate(rate);
			SetFadeOutTarget(target);
		}

		public void StopFading()
		{
			mFadeData.mFadeState = 0;
		}
	}
}
