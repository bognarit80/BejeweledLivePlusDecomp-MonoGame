using System.Collections.Generic;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace JeffLib
{
	public class AfterEffectsTimeline
	{
		protected int mUpdateCount;

		protected List<Component> mPosX = new List<Component>();

		protected List<Component> mPosY = new List<Component>();

		protected List<Component> mScaleX = new List<Component>();

		protected List<Component> mScaleY = new List<Component>();

		protected List<Component> mAngle = new List<Component>();

		protected List<Component> mOpacity = new List<Component>();

		public Image mImage;

		public float mOverallAlphaPct = 1f;

		public float mOverallXScale = 1f;

		public float mOverallYScale = 1f;

		public int mCel;

		public bool mMirror;

		public bool mHoldLastFrame;

		public int mStartFrame;

		public int mEndFrame;

		public void AddPosX(Component x)
		{
			x.mStartFrame += mStartFrame;
			x.mEndFrame += mStartFrame;
			mPosX.Add(x);
		}

		public void AddPosY(Component y)
		{
			y.mStartFrame += mStartFrame;
			y.mEndFrame += mStartFrame;
			mPosY.Add(y);
		}

		public void AddScaleX(Component c)
		{
			c.mStartFrame += mStartFrame;
			c.mEndFrame += mStartFrame;
			mScaleX.Add(c);
		}

		public void AddScaleY(Component c)
		{
			c.mStartFrame += mStartFrame;
			c.mEndFrame += mStartFrame;
			mScaleY.Add(c);
		}

		public void AddAngle(Component c)
		{
			c.mStartFrame += mStartFrame;
			c.mEndFrame += mStartFrame;
			mAngle.Add(c);
		}

		public void AddOpacity(Component c)
		{
			c.mStartFrame += mStartFrame;
			c.mEndFrame += mStartFrame;
			mOpacity.Add(c);
		}

		public void Update()
		{
			if (mUpdateCount <= mEndFrame)
			{
				mUpdateCount++;
			}
			if (mUpdateCount >= mStartFrame && mUpdateCount <= mEndFrame)
			{
				Component.UpdateComponentVec(mPosX, mUpdateCount);
				Component.UpdateComponentVec(mPosY, mUpdateCount);
				Component.UpdateComponentVec(mScaleX, mUpdateCount);
				Component.UpdateComponentVec(mScaleY, mUpdateCount);
				Component.UpdateComponentVec(mAngle, mUpdateCount);
				Component.UpdateComponentVec(mOpacity, mUpdateCount);
			}
		}

		public void Draw(Graphics g, int force_alpha)
		{
			if (mImage == null || mUpdateCount < mStartFrame || (mUpdateCount > mEndFrame && !mHoldLastFrame))
			{
				return;
			}
			int num = (int)(Component.GetComponentValue(mOpacity, 1f, mUpdateCount) * mOverallAlphaPct * 255f);
			if (num > 0)
			{
				if (num > 255)
				{
					num = 255;
				}
				if (force_alpha >= 0)
				{
					num = force_alpha;
				}
				if (num != 255)
				{
					g.SetColorizeImages(true);
					g.SetColor(255, 255, 255, num);
				}
				Transform transform = new Transform();
				float componentValue = Component.GetComponentValue(mAngle, 0f, mUpdateCount);
				if (componentValue != 0f)
				{
					transform.RotateRad(componentValue);
				}
				float num2;
				float num3 = (num2 = Component.GetComponentValue(mScaleX, 1f, mUpdateCount) * mOverallXScale);
				if (SexyFramework.Common.size(mScaleY) > 0)
				{
					num2 = Component.GetComponentValue(mScaleY, 1f, mUpdateCount) * mOverallYScale;
				}
				if (mMirror)
				{
					num3 *= -1f;
				}
				if (!SexyFramework.Common._eq(num3, 1f) || !SexyFramework.Common._eq(num2, 1f))
				{
					transform.Scale(num3, num2);
				}
				float componentValue2 = Component.GetComponentValue(mPosX, 0f, mUpdateCount);
				float componentValue3 = Component.GetComponentValue(mPosY, 0f, mUpdateCount);
				Rect celRect = mImage.GetCelRect(mCel);
				if (g.Is3D())
				{
					g.DrawImageTransformF(mImage, transform, celRect, componentValue2, componentValue3);
				}
				else
				{
					g.DrawImageTransform(mImage, transform, celRect, componentValue2, componentValue3);
				}
				g.SetColorizeImages(false);
			}
		}

		public void Draw(Graphics g)
		{
			Draw(g, -1);
		}

		public bool Done()
		{
			return mUpdateCount > mEndFrame;
		}

		public void Reset()
		{
			mUpdateCount = 0;
			for (int i = 0; i < SexyFramework.Common.size(mPosX); i++)
			{
				mPosX[i].mValue = mPosX[i].mOriginalValue;
			}
			for (int j = 0; j < SexyFramework.Common.size(mPosY); j++)
			{
				mPosY[j].mValue = mPosY[j].mOriginalValue;
			}
			for (int k = 0; k < SexyFramework.Common.size(mScaleX); k++)
			{
				mScaleX[k].mValue = mScaleX[k].mOriginalValue;
			}
			for (int l = 0; l < SexyFramework.Common.size(mScaleY); l++)
			{
				mScaleY[l].mValue = mScaleY[l].mOriginalValue;
			}
			for (int m = 0; m < SexyFramework.Common.size(mAngle); m++)
			{
				mAngle[m].mValue = mAngle[m].mOriginalValue;
			}
			for (int n = 0; n < SexyFramework.Common.size(mOpacity); n++)
			{
				mOpacity[n].mValue = mOpacity[n].mOriginalValue;
			}
		}

		public int GetUpdateCount()
		{
			return mUpdateCount;
		}
	}
}
