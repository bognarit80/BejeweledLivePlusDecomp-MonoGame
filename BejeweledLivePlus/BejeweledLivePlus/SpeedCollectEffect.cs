using System;
using System.Collections.Generic;
using BejeweledLivePlus.Bej3Graphics;
using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class SpeedCollectEffect : Effect
	{
		public BSpline mSpline = new BSpline();

		public CurvedVal mSplineInterp = new CurvedVal();

		public CurvedVal mAlphaOut = new CurvedVal();

		public CurvedVal mScaleCv = new CurvedVal();

		public ParticleEffect mSparkles;

		public TimeBonusEffect mTimeBonusEffect;

		public int mUpdateCnt;

		public float mAccel;

		public SpeedBoard mBoard;

		public new Image mImage;

		public bool mCentering;

		public Point mStartPoint = default(Point);

		public Point mTargetPoint = default(Point);

		public Point mLastPoint = default(Point);

		public double mLastRotation;

		public float mTimeMod;

		public int mTimeCollected;

		private static SimpleObjectPool thePool_;

		public SpeedCollectEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init(SpeedBoard theSpeedBoard, Point theSrc, Point theTgt, Image theImage, int theTimeCollected, float theTimeMod)
		{
			init(Type.TYPE_CUSTOMCLASS);
			mTimeCollected = theTimeCollected;
			mBoard = theSpeedBoard;
			mImage = theImage;
			mX = theSrc.mX;
			mY = theSrc.mY;
			mLastPoint = theSrc;
			mDAlpha = 0f;
			mCurvedAlpha.SetConstant(1.0);
			mUpdateCnt = 0;
			mStartPoint = theSrc;
			mTargetPoint = theTgt;
			mLastRotation = 0.0;
			mCentering = false;
			mTimeMod = theTimeMod;
			mSparkles = null;
			mTimeBonusEffect = null;
		}

		public override void Dispose()
		{
			if (mSparkles != null)
			{
				mSparkles.mDeleteMe = true;
				mSparkles.mRefCount--;
			}
			if (mTimeBonusEffect != null)
			{
				mTimeBonusEffect.mDeleteMe = true;
				mTimeBonusEffect.mRefCount--;
			}
			base.Dispose();
		}

		public void Init(Piece thePiece)
		{
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_SPLINE_INTERP_A, mSplineInterp);
			GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_ALPHA_OUT, mAlphaOut);
			mSplineInterp.SetInRange(0.0, mSplineInterp.mInMax * (double)mTimeMod);
			mAlphaOut.mIncRate = mSplineInterp.mIncRate;
			mAlphaOut.mInMax = mSplineInterp.mInMax + (double)GlobalMembers.M(0f);
			mScaleCv.SetConstant(1.0);
			mSpline = new BSpline();
			mSpline.AddPoint(mX, mY);
			mSpline.AddPoint(ConstantsWP.SPEEDBOARD_COLLECT_EFFECT_INTERMEDIATE_X, ConstantsWP.SPEEDBOARD_COLLECT_EFFECT_INTERMEDIATE_Y);
			mSpline.AddPoint(mTargetPoint.mX, mTargetPoint.mY);
			mSpline.CalculateSpline();
			mSparkles = ParticleEffect.fromPIEffect(GlobalMembersResourcesWP.PIEFFECT_QUEST_DIG_COLLECT_GOLD);
			mSparkles.SetEmitAfterTimeline(true);
			mSparkles.mDoDrawTransform = false;
			mSparkles.mRefCount++;
			mFXManager.AddEffect(mSparkles);
			List<Effect> list = mFXManager.mBoard.mPostFXManager.mEffectList[21];
			foreach (Effect item in list)
			{
				if (item.mType == Type.TYPE_TIME_BONUS && item.mPieceRel == thePiece)
				{
					mTimeBonusEffect = (TimeBonusEffect)item;
					mTimeBonusEffect.mLightIntensity = GlobalMembers.M(6f);
					mTimeBonusEffect.mLightSize = GlobalMembers.M(300f);
					mTimeBonusEffect.mValue[0] = GlobalMembers.M(50f);
					mTimeBonusEffect.mValue[1] = GlobalMembers.M(-0.0005f);
					mTimeBonusEffect.mRefCount++;
					mTimeBonusEffect.mPieceRel = null;
					mTimeBonusEffect.mOverlay = true;
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_CIRCLE_PCT_SPEED_BOARD_TIME_BONUS, mTimeBonusEffect.mCirclePct);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eEFFECTS_RADIUS_SCALE_SPEED_BOARD_TIME_BONUS, mTimeBonusEffect.mRadiusScale);
					break;
				}
			}
		}

		public double CalcRotation()
		{
			if (mCentering)
			{
				return 0.0;
			}
			if (!mSplineInterp.HasBeenTriggered())
			{
				double num = Math.Atan2((float)mLastPoint.mY - mY, mX - (float)mLastPoint.mX);
				double num2 = num - mLastRotation;
				num2 = ((num2 < 0.0) ? (-1.0) : 1.0) * Math.Min(GlobalMembers.M(0.03), Math.Abs(num2));
				mLastRotation += num2;
				mLastPoint.mX = (int)mX;
				mLastPoint.mY = (int)mY;
			}
			return mLastRotation;
		}

		public override void Update()
		{
			base.Update();
			mUpdateCnt++;
			if (mCentering)
			{
				mX = (float)((double)mStartPoint.mX + (double)mSplineInterp * (double)(mFXManager.mBoard.GetBoardCenterX() - mStartPoint.mX));
				mY = (float)((double)mStartPoint.mY + (double)mSplineInterp * (double)(mFXManager.mBoard.GetBoardCenterY() - mStartPoint.mY));
				if (!mSplineInterp.IncInVal())
				{
					GlobalMembers.gApp.PlaySample(GlobalMembersResourcesWP.SOUND_QUEST_GET);
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_SPLINE_INTERP_B, mSplineInterp);
					mSplineInterp.SetInRange(0.0, mSplineInterp.mInMax * (double)mTimeMod);
					mCentering = false;
					mSpline.AddPoint(mX, mY);
					mSpline.AddPoint(GlobalMembers.M(800f), GlobalMembers.M(150f));
					mSpline.AddPoint(GlobalMembers.M(600f), GlobalMembers.M(175f));
					mSpline.AddPoint(GlobalMembers.M(400f), GlobalMembers.M(150f));
					mSpline.AddPoint(GlobalMembers.M(200f), GlobalMembers.M(300f));
					mSpline.AddPoint(mTargetPoint.mX, mTargetPoint.mY);
					mSpline.CalculateSpline();
					GlobalMembers.gApp.mCurveValCache.GetCurvedVal(PreCalculatedCurvedValManager.CURVED_VAL_ID.eSPEED_BOARD_SCALE_CV, mScaleCv);
					mScaleCv.SetInRange(0.0, mScaleCv.mInMax * (double)mTimeMod);
				}
			}
			else
			{
				mX = mSpline.GetXPoint((float)((double)mSplineInterp * (double)mSpline.GetMaxT()));
				mY = mSpline.GetYPoint((float)((double)mSplineInterp * (double)mSpline.GetMaxT()));
			}
			if (mSparkles != null)
			{
				mSparkles.mX = mX + GlobalMembers.M(-30f);
				mSparkles.mY = mY + GlobalMembers.M(-20f);
			}
			if (mTimeBonusEffect != null)
			{
				mTimeBonusEffect.mX = mX;
				mTimeBonusEffect.mY = mY;
			}
			mScaleCv.IncInVal();
			if (mCentering)
			{
				return;
			}
			mSplineInterp.IncInVal();
			if (mAlphaOut.IsDoingCurve())
			{
				if (mAlphaOut.CheckUpdatesFromEndThreshold(GlobalMembers.M(10)))
				{
					mBoard.TimeCollected(mTimeCollected);
				}
				if (!mAlphaOut.IncInVal())
				{
					mDeleteMe = true;
				}
				if (mSparkles != null)
				{
					mSparkles.mAlpha = (float)(double)mAlphaOut;
				}
				if (mTimeBonusEffect != null)
				{
					mTimeBonusEffect.mAlpha = (float)(double)mAlphaOut;
				}
			}
		}

		public override void Draw(Graphics g)
		{
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(SpeedCollectEffect));
		}

		public static SpeedCollectEffect alloc(SpeedBoard theSpeedBoard, Point theSrc, Point theTgt, Image theImage, int theTimeCollected)
		{
			return alloc(theSpeedBoard, theSrc, theTgt, theImage, theTimeCollected, 1f);
		}

		public static SpeedCollectEffect alloc(SpeedBoard theSpeedBoard, Point theSrc, Point theTgt, Image theImage, int theTimeCollected, float theTimeMod)
		{
			SpeedCollectEffect speedCollectEffect = (SpeedCollectEffect)thePool_.alloc();
			speedCollectEffect.init(theSpeedBoard, theSrc, theTgt, theImage, theTimeCollected, theTimeMod);
			return speedCollectEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
