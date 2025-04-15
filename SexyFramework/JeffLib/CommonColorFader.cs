using System;

namespace JeffLib
{
	public class CommonColorFader
	{
		public FColor mColor = new FColor();

		public FColor mMaxColor = new FColor();

		public FColor mMinColor = new FColor();

		public bool mForward;

		public bool mEnabled;

		public float mRedChange;

		public float mGreenChange;

		public float mBlueChange;

		public float mAlphaChange;

		public int mDuration;

		public CommonColorFader()
		{
			mForward = true;
			mEnabled = true;
			mDuration = -1;
			mRedChange = (mGreenChange = (mBlueChange = (mAlphaChange = 0f)));
		}

		public bool Update()
		{
			if (!mEnabled)
			{
				return false;
			}
			int num = 0;
			if (mForward)
			{
				mColor.mRed += mRedChange;
				mColor.mGreen += mGreenChange;
				mColor.mBlue += mBlueChange;
				mColor.mAlpha += mAlphaChange;
				if (Common._ATLIMIT(mColor.mRed, mMaxColor.mRed, mRedChange))
				{
					num++;
					mColor.mRed = mMaxColor.mRed;
				}
				if (Common._ATLIMIT(mColor.mGreen, mMaxColor.mGreen, mGreenChange))
				{
					num++;
					mColor.mGreen = mMaxColor.mGreen;
				}
				if (Common._ATLIMIT(mColor.mBlue, mMaxColor.mBlue, mBlueChange))
				{
					num++;
					mColor.mBlue = mMaxColor.mBlue;
				}
				if (Common._ATLIMIT(mColor.mAlpha, mMaxColor.mAlpha, mAlphaChange))
				{
					num++;
					mColor.mAlpha = mMaxColor.mAlpha;
				}
			}
			else
			{
				mColor.mRed -= mRedChange;
				mColor.mGreen -= mGreenChange;
				mColor.mBlue -= mBlueChange;
				mColor.mAlpha -= mAlphaChange;
				if (Common._ATLIMIT(mColor.mRed, mMinColor.mRed, 0f - mRedChange))
				{
					num++;
					mColor.mRed = mMinColor.mRed;
				}
				if (Common._ATLIMIT(mColor.mGreen, mMinColor.mGreen, 0f - mGreenChange))
				{
					num++;
					mColor.mGreen = mMinColor.mGreen;
				}
				if (Common._ATLIMIT(mColor.mBlue, mMinColor.mBlue, 0f - mBlueChange))
				{
					num++;
					mColor.mBlue = mMinColor.mBlue;
				}
				if (Common._ATLIMIT(mColor.mAlpha, mMinColor.mAlpha, 0f - mAlphaChange))
				{
					num++;
					mColor.mAlpha = mMinColor.mAlpha;
				}
			}
			if (num == 4)
			{
				if (mDuration > 0 && --mDuration <= 0)
				{
					mEnabled = false;
					return true;
				}
				mForward = !mForward;
				return true;
			}
			return false;
		}

		public void SetSpeed(int s)
		{
			mRedChange = (mGreenChange = (mBlueChange = (mAlphaChange = s)));
		}

		public void FadeOverTime(int frames)
		{
			mRedChange = (mMaxColor.mRed - mMinColor.mRed) / (float)frames;
			mGreenChange = (mMaxColor.mGreen - mMinColor.mGreen) / (float)frames;
			mBlueChange = (mMaxColor.mBlue - mMinColor.mBlue) / (float)frames;
			mAlphaChange = (mMaxColor.mAlpha - mMinColor.mAlpha) / (float)frames;
		}

		public void AlphaFadeIn(int arate)
		{
			mEnabled = true;
			mForward = true;
			mDuration = 1;
			mAlphaChange = Math.Abs(arate);
			mRedChange = (mGreenChange = (mBlueChange = 0f));
			mColor.mRed = (mColor.mGreen = (mColor.mBlue = 255f));
			mColor.mAlpha = 0f;
			mMinColor = mColor;
			mMaxColor.mRed = (mMaxColor.mGreen = (mMaxColor.mBlue = (mMaxColor.mAlpha = 255f)));
		}

		public void AlphaFadeOut(int arate)
		{
			mEnabled = true;
			mForward = false;
			mDuration = 1;
			mAlphaChange = Math.Abs(arate);
			mRedChange = (mGreenChange = (mBlueChange = 0f));
			mColor.mRed = (mColor.mGreen = (mColor.mBlue = (mColor.mAlpha = 255f)));
			mMaxColor = mColor;
			mMinColor.mRed = (mMinColor.mGreen = (mMinColor.mBlue = 255f));
			mMinColor.mAlpha = 0f;
		}
	}
}
