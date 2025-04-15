using SexyFramework;

namespace JeffLib
{
	public class AlphaFader
	{
		public FColor mColor;

		public float mFadeRate;

		public int mFadeCount;

		public int mMin;

		public int mMax = 255;

		public void Update()
		{
			mColor.mAlpha += mFadeRate;
			if (SexyFramework.Common._geq(mColor.mAlpha, mMax) && mFadeRate > 0f)
			{
				mColor.mAlpha = mMax;
				mFadeRate *= -1f;
				mFadeCount++;
			}
			else if (SexyFramework.Common._leq(mColor.mAlpha, mMin) && mFadeRate < 0f)
			{
				mColor.mAlpha = mMin;
				mFadeRate *= -1f;
				mFadeCount++;
			}
		}
	}
}
