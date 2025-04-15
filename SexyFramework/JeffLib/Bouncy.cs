namespace JeffLib
{
	public class Bouncy
	{
		protected int mCount;

		protected int mMaxBounces;

		protected float mPct;

		protected float mMaxPct;

		protected float mMinPct;

		protected float mFinalPct;

		protected float mRate;

		protected float mRateDivFactor;

		protected bool mInc;

		protected bool mDone;

		protected float mStartingPct;

		protected float mStartingRate;

		protected bool mStartInc;

		public Bouncy()
		{
			mCount = 0;
			mMaxBounces = 0;
			mPct = 0f;
			mRate = 0f;
			mStartingPct = 0f;
			mStartInc = true;
			mInc = true;
			mDone = false;
			mRateDivFactor = 2f;
			mStartingRate = 0f;
		}

		public void Dispose()
		{
		}

		public void Update()
		{
			if (!mDone)
			{
				mPct += (mInc ? mRate : (0f - mRate));
				float num = ((mCount != mMaxBounces) ? (mInc ? mMaxPct : mMinPct) : mFinalPct);
				if (mInc && mPct >= num)
				{
					mPct = num;
					mInc = false;
					mCount++;
					mRate /= mRateDivFactor;
				}
				else if (!mInc && mPct <= num)
				{
					mPct = num;
					mInc = true;
					mCount++;
					mRate /= mRateDivFactor;
				}
				if (mCount > mMaxBounces)
				{
					mDone = true;
				}
			}
		}

		public void Reset()
		{
			mCount = 0;
			mPct = mStartingPct;
			mInc = mStartInc;
			mDone = false;
			mRate = mStartingRate;
		}

		public float GetPct()
		{
			return mPct;
		}

		public int GetCount()
		{
			return mCount;
		}

		public bool IsDone()
		{
			return mDone;
		}

		public void SetNumBounces(int b)
		{
			mMaxBounces = b;
		}

		public void SetPct(float p)
		{
			SetPct(p, true);
		}

		public void SetPct(float p, bool inc)
		{
			mPct = (mStartingPct = p);
			mInc = (mStartInc = inc);
		}

		public void SetTargetPercents(float minp, float maxp, float finalp)
		{
			mMinPct = minp;
			mMaxPct = maxp;
			mFinalPct = finalp;
		}

		public void SetRate(float r)
		{
			mRate = (mStartingRate = r);
		}

		public void SetRateDivFactor(float d)
		{
			mRateDivFactor = d;
		}
	}
}
