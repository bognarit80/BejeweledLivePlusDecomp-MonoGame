using System;
using System.Collections.Generic;
using BejeweledLivePlus;
using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

public class BorderEffect
{
	public List<float> mDists = new List<float>();

	public double mTotalDist;

	public int mX;

	public int mY;

	public int mFxOffsetX;

	public int mFxOffsetY;

	public PIEffect mFx;

	public Image mImg;

	public int mSortOrder;

	public bool mParticlesInForeground;

	public int mMaxParticles;

	public int mId;

	public CurvedVal mPhase;

	public CurvedVal mMultMagnitudeOuterTextureSpan;

	public CurvedVal mMultMagnitudeOuterTextureSpanMag;

	public CurvedVal mMultMagnitudeOuter;

	public CurvedVal mMultMagnitudeInner;

	public CurvedVal mAlpha;

	public List<float> mMarkerPos = new List<float>();

	public List<float> mMarkerPosOrig = new List<float>();

	public List<float> mMarkerPosOuter = new List<float>();

	public List<float> mMarkerPosOuterOrig = new List<float>();

	public int mMarkerLen;

	public int mUpdateCnt;

	public int mDelayCnt;

	public bool mBorderGlow;

	protected double mLastMultOuterMag;

	protected double mLastMultInnerMag;

	public BorderEffect(ref float theMarkerPos, ref float theMarkerPosOuter, int theLength)
	{
		mId = 0;
		mX = 0;
		mY = 0;
		mFxOffsetX = 0;
		mFxOffsetY = 0;
		mFx = null;
		mUpdateCnt = 0;
		mBorderGlow = false;
		if (theLength > 0)
		{
			List<float> list = mMarkerPos;
			mMarkerPos.Clear();
			List<float>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				mMarkerPos.Add(enumerator.Current);
			}
			list = mMarkerPosOuter;
			mMarkerPosOuter.Clear();
			List<float>.Enumerator enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				mMarkerPos.Add(enumerator2.Current);
			}
		}
		mMarkerLen = theLength;
		mTotalDist = 0.0;
		for (int i = 0; i < mMarkerLen * 2; i++)
		{
			mDists.Add(0f);
		}
		mImg = null;
		mPhase.SetConstant(0.0);
		mMultMagnitudeOuter.SetConstant(1.0);
		mMultMagnitudeInner.SetConstant(1.0);
		mMultMagnitudeOuterTextureSpan.SetConstant(1.0);
		mMultMagnitudeOuterTextureSpanMag.SetConstant(1.0);
		mAlpha.SetConstant(1.0);
		mLastMultOuterMag = 1.0;
		mLastMultInnerMag = 1.0;
		mDelayCnt = 0;
		mMaxParticles = 0;
		mSortOrder = 0;
		mParticlesInForeground = false;
		for (int j = 0; j < Common.size(mDists); j++)
		{
			float num = GetMarkerX(j) - GetMarkerX((j + Common.size(mDists) - 1) % Common.size(mDists));
			float num2 = GetMarkerY(j) - GetMarkerY((j + Common.size(mDists) - 1) % Common.size(mDists));
			mDists[j] = (float)Math.Sqrt(num * num + num2 * num2);
			mTotalDist += mDists[j];
		}
	}

	public void Dispose()
	{
	}

	public void Draw(Graphics g)
	{
		if (mLastMultOuterMag != (double)mMultMagnitudeOuter || mMultMagnitudeOuterTextureSpan.IsDoingCurve())
		{
			MultMagnitudeOuter(mMultMagnitudeOuter);
		}
		if (mLastMultInnerMag != (double)mMultMagnitudeInner)
		{
			MultMagnitudeInner(mMultMagnitudeInner);
		}
		if ((double)mAlpha != 1.0)
		{
			if ((double)mAlpha == 0.0)
			{
				return;
			}
			g.SetColorizeImages(true);
			g.SetColor(Color.FAlpha((float)(double)mAlpha));
		}
		g.PushState();
		g.Translate(BejeweledLivePlus.GlobalMembers.S(mX), BejeweledLivePlus.GlobalMembers.S(mY));
		int num = mMarkerLen;
		if (!mParticlesInForeground)
		{
			DrawParticles(g);
		}
		if (mImg != null)
		{
			double theTextureCount = 0.0;
			double theTgtTextureCount = 0.0;
			double theTextureW = 0.0;
			double thePhase = 0.0;
			CalcTextureProps(ref theTextureCount, ref theTgtTextureCount, ref theTextureW, ref thePhase);
			int num2 = 0;
			double num3 = thePhase;
			double num4 = 0.0;
			double num5 = 0.0;
			while ((double)mDists[num2] <= num3)
			{
				num3 -= (double)mDists[num2];
				num2 = (num2 + 1) % Common.size(mDists);
			}
			double num6 = num3;
			double num7 = num3;
			int num8 = -1;
			int num9 = 0;
			while (true)
			{
				double val = theTextureW - num4;
				double val2 = (double)mDists[num2] - num3;
				double num10 = Math.Min(val, val2);
				if (num2 == num8 && num6 - num3 >= 0.0)
				{
					num10 = Math.Min(num10, num6 - num3);
				}
				num5 += num10;
				num3 += num10;
				double inAlpha = num7 / (double)mDists[num2];
				double inAlpha2 = num3 / (double)mDists[num2];
				int num11 = (num2 + 1) % num;
				double theNum = SexyMath.Lerp(GetMarkerX(num2), GetMarkerX(num11), inAlpha);
				double theNum2 = SexyMath.Lerp(GetMarkerY(num2), GetMarkerY(num11), inAlpha);
				double theNum3 = SexyMath.Lerp(GetMarkerX(num2), GetMarkerX(num11), inAlpha2);
				double theNum4 = SexyMath.Lerp(GetMarkerY(num2), GetMarkerY(num11), inAlpha2);
				double theNum5 = SexyMath.Lerp(GetMarkerOuterX(num2), GetMarkerOuterX(num11), inAlpha);
				double theNum6 = SexyMath.Lerp(GetMarkerOuterY(num2), GetMarkerOuterY(num11), inAlpha);
				double theNum7 = SexyMath.Lerp(GetMarkerOuterX(num2), GetMarkerOuterX(num11), inAlpha2);
				double theNum8 = SexyMath.Lerp(GetMarkerOuterY(num2), GetMarkerOuterY(num11), inAlpha2);
				SexyVertex2D[,] array = new SexyVertex2D[2, 3];
				double num12 = num4 / theTextureW;
				double num13 = num5 / theTextureW;
				array[0, 0].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum);
				array[0, 0].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum2);
				array[0, 0].u = (float)num12;
				array[0, 0].v = 1f;
				array[0, 1].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum7);
				array[0, 1].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum8);
				array[0, 1].u = (float)num13;
				array[0, 1].v = 0f;
				array[0, 2].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum5);
				array[0, 2].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum6);
				array[0, 2].u = (float)num12;
				array[0, 2].v = 0f;
				array[1, 0].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum7);
				array[1, 0].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum8);
				array[1, 0].u = (float)num13;
				array[1, 0].v = 0f;
				array[1, 1].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum);
				array[1, 1].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum2);
				array[1, 1].u = (float)num12;
				array[1, 1].v = 1f;
				array[1, 2].x = (float)BejeweledLivePlus.GlobalMembers.S(theNum3);
				array[1, 2].y = (float)BejeweledLivePlus.GlobalMembers.S(theNum4);
				array[1, 2].u = (float)num13;
				array[1, 2].v = 1f;
				g.DrawTrianglesTex(mImg, array, 2);
				num9 += 2;
				if (num2 == num8 && num6 - num3 <= 0.0)
				{
					break;
				}
				if (num3 >= (double)mDists[num2])
				{
					if (num8 < 0)
					{
						num8 = num2;
					}
					num3 = 0.0;
					num2 = (num2 + 1) % Common.size(mDists);
				}
				if (num5 >= theTextureW)
				{
					num5 = 0.0;
				}
				num7 = num3;
				num4 = num5;
			}
		}
		if (mParticlesInForeground)
		{
			DrawParticles(g);
		}
		g.PopState();
		g.SetColor(new Color(-1));
		g.SetColorizeImages(false);
	}

	public void DrawDebug(Graphics g)
	{
		int num = mMarkerLen;
		for (int i = 0; i < num && i != BejeweledLivePlus.GlobalMembers.M(-1); i++)
		{
			double theNum = GetMarkerX(i);
			double theNum2 = GetMarkerX((i + 1) % num);
			double theNum3 = GetMarkerY(i);
			double theNum4 = GetMarkerY((i + 1) % num);
			double theNum5 = GetMarkerOuterX(i);
			double theNum6 = GetMarkerOuterX((i + 1) % num);
			double theNum7 = GetMarkerOuterY(i);
			double theNum8 = GetMarkerOuterY((i + 1) % num);
			g.SetColor(new Color(BejeweledLivePlus.GlobalMembers.M(16777215), BejeweledLivePlus.GlobalMembers.M(100)));
			g.DrawLine((int)BejeweledLivePlus.GlobalMembers.S(theNum), (int)BejeweledLivePlus.GlobalMembers.S(theNum3), (int)BejeweledLivePlus.GlobalMembers.S(theNum2), (int)BejeweledLivePlus.GlobalMembers.S(theNum4));
			g.DrawLine((int)BejeweledLivePlus.GlobalMembers.S(theNum5), (int)BejeweledLivePlus.GlobalMembers.S(theNum7), (int)BejeweledLivePlus.GlobalMembers.S(theNum6), (int)BejeweledLivePlus.GlobalMembers.S(theNum8));
			g.DrawLine((int)BejeweledLivePlus.GlobalMembers.S(theNum2), (int)BejeweledLivePlus.GlobalMembers.S(theNum4), (int)BejeweledLivePlus.GlobalMembers.S(theNum5), (int)BejeweledLivePlus.GlobalMembers.S(theNum7));
			g.DrawLine((int)BejeweledLivePlus.GlobalMembers.S(theNum5), (int)BejeweledLivePlus.GlobalMembers.S(theNum7), (int)BejeweledLivePlus.GlobalMembers.S(theNum), (int)BejeweledLivePlus.GlobalMembers.S(theNum3));
			g.SetColor(new Color(BejeweledLivePlus.GlobalMembers.M(65280), BejeweledLivePlus.GlobalMembers.M(100)));
			g.FillRect((int)BejeweledLivePlus.GlobalMembers.S(theNum) - 1, (int)BejeweledLivePlus.GlobalMembers.S(theNum3) - 1, 2, 2);
			g.FillRect((int)BejeweledLivePlus.GlobalMembers.S(theNum2) - 1, (int)BejeweledLivePlus.GlobalMembers.S(theNum4) - 1, 2, 2);
			g.FillRect((int)BejeweledLivePlus.GlobalMembers.S(theNum5) - 1, (int)BejeweledLivePlus.GlobalMembers.S(theNum7) - 1, 2, 2);
			g.FillRect((int)BejeweledLivePlus.GlobalMembers.S(theNum6) - 1, (int)BejeweledLivePlus.GlobalMembers.S(theNum8) - 1, 2, 2);
		}
		if (mFx == null)
		{
			return;
		}
		for (int j = 0; j < Common.size(mFx.mLayerVector); j++)
		{
			PILayer layer = mFx.GetLayer(j);
			for (int k = 0; k < Common.size(layer.mEmitterInstanceVector); k++)
			{
				PIEmitterInstance emitter = layer.GetEmitter(k);
				int num2 = Common.size(emitter.mEmitterInstanceDef.mPoints);
				for (int l = 0; l < num2; l++)
				{
					g.DrawLine((int)BejeweledLivePlus.GlobalMembers.S(emitter.mEmitterInstanceDef.mPoints[l].mValuePoint2DVector[0].mValue.X), (int)BejeweledLivePlus.GlobalMembers.S(emitter.mEmitterInstanceDef.mPoints[l].mValuePoint2DVector[0].mValue.Y), (int)BejeweledLivePlus.GlobalMembers.S(emitter.mEmitterInstanceDef.mPoints[(l + 1) % num2].mValuePoint2DVector[0].mValue.X), (int)BejeweledLivePlus.GlobalMembers.S(emitter.mEmitterInstanceDef.mPoints[(l + 1) % num2].mValuePoint2DVector[0].mValue.Y));
				}
			}
		}
	}

	public void DrawParticles(Graphics g)
	{
		if (mFx != null)
		{
			if ((double)mAlpha != 1.0)
			{
				g.PushColorMult();
			}
			g.Translate(mFxOffsetX, mFxOffsetY);
			mFx.Draw(g);
			g.Translate(-mFxOffsetX, -mFxOffsetY);
			if ((double)mAlpha != 1.0)
			{
				g.PopColorMult();
			}
		}
	}

	public void Update()
	{
		if (mDelayCnt > 0)
		{
			mDelayCnt--;
			return;
		}
		mUpdateCnt++;
		mPhase.IncInVal();
		mMultMagnitudeOuter.IncInVal();
		mMultMagnitudeInner.IncInVal();
		mAlpha.IncInVal();
		mMultMagnitudeOuterTextureSpanMag.IncInVal();
		mMultMagnitudeOuterTextureSpan.mOutMax = mMultMagnitudeOuterTextureSpanMag;
		if (mFx != null)
		{
			mFx.Update();
		}
	}

	public bool IsDone()
	{
		if (mAlpha.HasBeenTriggered())
		{
			return (double)mAlpha == 0.0;
		}
		return false;
	}

	public float GetMarkerX(int x)
	{
		return mMarkerPos[x * 2];
	}

	public float GetMarkerY(int y)
	{
		return mMarkerPos[y * 2 + 1];
	}

	public float GetMarkerOrigX(int x)
	{
		return mMarkerPosOrig[x * 2];
	}

	public float GetMarkerOrigY(int y)
	{
		return mMarkerPosOrig[y * 2 + 1];
	}

	public float GetMarkerOuterX(int x)
	{
		return mMarkerPosOuter[x * 2];
	}

	public float GetMarkerOuterY(int y)
	{
		return mMarkerPosOuter[y * 2 + 1];
	}

	public float GetMarkerOuterOrigX(int x)
	{
		return mMarkerPosOuterOrig[x * 2];
	}

	public float GetMarkerOuterOrigY(int y)
	{
		return mMarkerPosOuterOrig[y * 2 + 1];
	}

	public void MultMagnitudeInner(double theMult)
	{
	}

	public void MultMagnitudeOuter(double theMult)
	{
	}

	public void SetEmitter(PIEffect theFx)
	{
	}

	public void RefreshEmitters()
	{
	}

	public void Finish()
	{
		mAlpha.SetConstant(0.0);
	}

	public void CalcTextureProps(ref double theTextureCount, ref double theTgtTextureCount, ref double theTextureW, ref double thePhase)
	{
	}
}
