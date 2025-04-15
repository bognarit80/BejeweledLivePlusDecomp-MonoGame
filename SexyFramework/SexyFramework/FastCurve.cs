using SexyFramework.Graphics;

namespace SexyFramework
{
	public class FastCurve
	{
		public float mOutMin;

		public float mOutMax;

		public float mInMin;

		public float mInMax;

		public float mIncRate;

		public float mInVal;

		public bool mTriggered;

		public bool mSingleTrigger;

		public bool mOutputSync;

		protected void InitFromCurveData()
		{
			mTriggered = false;
			mOutputSync = CurvedVal.gsFastCurveData.mOutputSync;
			mSingleTrigger = CurvedVal.gsFastCurveData.mSingleTrigger;
			mOutMin = (float)CurvedVal.gsFastCurveData.mOutMin;
			mOutMax = (float)CurvedVal.gsFastCurveData.mOutMax;
			mInVal = (float)CurvedVal.gsFastCurveData.mInVal;
			mInMin = (float)CurvedVal.gsFastCurveData.mInMin;
			mInMax = (float)CurvedVal.gsFastCurveData.mInMax;
			mIncRate = (float)CurvedVal.gsFastCurveData.mIncRate;
		}

		public FastCurve()
		{
			mOutMin = 0f;
			mOutMax = 1f;
			mInMin = 0f;
			mInMax = 1f;
			mTriggered = false;
			mIncRate = 0f;
			mInVal = 0f;
			mSingleTrigger = false;
			mOutputSync = false;
		}

		public FastCurve(string theData)
			: this(theData, null)
		{
		}

		public FastCurve(string theData, CurvedVal theLinkedVal)
		{
			CurvedVal.gsFastCurveData.SetCurve(theData);
			InitFromCurveData();
		}

		public void SetCurve(string theDataP)
		{
			SetCurve(theDataP, null);
		}

		public void SetCurve(string theDataP, CurvedVal theLinkedVal)
		{
			CurvedVal.gsFastCurveData.SetCurve(theDataP);
			InitFromCurveData();
		}

		public void SetConstant(float theValue)
		{
			mInVal = 0f;
			mTriggered = false;
			mInMin = (mInMax = 0f);
			mOutMin = (mOutMax = theValue);
		}

		public float GetOutVal()
		{
			return GetOutVal(mInVal);
		}

		public float GetOutVal(float theInVal)
		{
			return mOutMax;
		}

		public float GetOutFinalVal()
		{
			return GetOutVal(mInMax);
		}

		public void SetOutRange(float theMin, float theMax)
		{
			mOutMin = theMin;
			mOutMax = theMax;
		}

		public void SetInRange(float theMin, float theMax)
		{
			mInMin = theMin;
			mInMax = theMax;
		}

		public float GetInVal()
		{
			return mInVal;
		}

		public bool SetInVal(float theVal)
		{
			return SetInVal(theVal, false);
		}

		public bool SetInVal(float theVal, bool theRealignAutoInc)
		{
			mInVal = theVal;
			return false;
		}

		public bool IncInVal(float theInc)
		{
			mInVal += theInc;
			bool flag = false;
			if (mInVal > mInMax)
			{
				mInVal = mInMax;
				flag = true;
			}
			else if (mInVal < mInMin)
			{
				mInVal = mInMin;
				flag = true;
			}
			if (flag)
			{
				if (!mTriggered)
				{
					mTriggered = true;
					return false;
				}
				return mSingleTrigger;
			}
			return true;
		}

		public bool IncInVal()
		{
			return IncInVal(mIncRate);
		}

		public bool HasBeenTriggered()
		{
			return mTriggered;
		}

		public void ClearTrigger()
		{
			mTriggered = false;
		}

		public static implicit operator float(FastCurve ImpliedObject)
		{
			return ImpliedObject.GetOutVal();
		}

		public static implicit operator Color(FastCurve ImpliedObject)
		{
			return new Color(255, 255, 255, (int)(255f * ImpliedObject.GetOutVal()));
		}
	}
}
