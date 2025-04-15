namespace JeffLib
{
	public class Oscillator
	{
		public float mVal;

		public float mMinVal;

		public float mMaxVal;

		public float mInc;

		public float mAccel;

		public bool mForward;

		public void Init(float min_val, float max_val, bool start_at_max, float accel)
		{
			mAccel = accel;
			mMinVal = min_val;
			mMaxVal = max_val;
			mInc = 0f;
			if (start_at_max)
			{
				mVal = mMaxVal;
				mForward = false;
			}
			else
			{
				mVal = mMinVal;
				mForward = true;
			}
		}

		public void Update()
		{
			if (mForward)
			{
				mInc += mAccel;
				mVal += mInc;
				if (mVal >= mMaxVal)
				{
					mVal = mMaxVal;
					mForward = false;
				}
			}
			else
			{
				mInc -= mAccel;
				mVal += mInc;
				if (mVal <= mMinVal)
				{
					mVal = mMinVal;
					mForward = true;
				}
			}
		}

		public float GetVal()
		{
			return mVal;
		}
	}
}
