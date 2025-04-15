namespace JeffLib
{
	public class FadeData
	{
		public enum FadeType
		{
			Fade_None,
			Fade_Out,
			Fade_In
		}

		public int mFadeState;

		public int mFadeOutRate;

		public int mFadeInRate;

		public int mFadeOutTarget;

		public int mFadeInTarget;

		public int mVal;

		public int mFadeCount;

		public bool mStopWhenDone;

		public FadeData()
		{
			mFadeState = 0;
			mFadeOutTarget = (mFadeInTarget = 0);
			mFadeOutRate = (mFadeInRate = 0);
			mVal = 0;
			mFadeCount = 0;
			mStopWhenDone = true;
		}

		public FadeData(FadeData fd)
		{
			mFadeState = fd.mFadeState;
			mFadeOutTarget = fd.mFadeOutTarget;
			mFadeOutRate = fd.mFadeOutRate;
			mVal = fd.mVal;
			mFadeCount = fd.mFadeCount;
			mStopWhenDone = fd.mStopWhenDone;
		}
	}
}
