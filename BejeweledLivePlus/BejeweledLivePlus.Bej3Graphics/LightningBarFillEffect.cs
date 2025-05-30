using System;
using BejeweledLivePlus.Misc;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace BejeweledLivePlus.Bej3Graphics
{
	public class LightningBarFillEffect : Effect
	{
		private const int NUM_BARFILL_LIGTNING_POINTS = 8;

		public FPoint[,] mPoints = new FPoint[8, 2];

		public float mPercentDone;

		private static SimpleObjectPool thePool_;

		public LightningBarFillEffect()
			: base(Type.TYPE_CUSTOMCLASS)
		{
		}

		public void init()
		{
			init(Type.TYPE_CUSTOMCLASS);
			mPercentDone = 0f;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					mPoints[i, j] = default(FPoint);
				}
			}
		}

		public override void Update()
		{
			bool flag = mPercentDone == 0f;
			mPercentDone += GlobalMembers.M(0.012f);
			if (mPercentDone > 1f)
			{
				mDeleteMe = true;
			}
			else
			{
				if (mFXManager.mBoard.mUpdateCnt % GlobalMembers.M(3) != 0 && !flag)
				{
					return;
				}
				float num = ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_START_X;
				float num2 = ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_START_Y;
				float num3 = (float)ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_END_X + mFXManager.mBoard.mCountdownBarPct * (float)ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_END_X_FULL;
				float num4 = ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_END_Y;
				for (int i = 0; i < 8; i++)
				{
					float num5 = (float)i / 7f;
					float num6 = 1f - Math.Abs(1f - num5 * 2f);
					float num7 = num * (1f - num5) + num3 * num5 + num6 * (GlobalMembersUtils.GetRandFloat() * 60f);
					float num8 = num2 * (1f - num5) + num4 * num5 + num6 * (GlobalMembersUtils.GetRandFloat() * 60f);
					FPoint fPoint = mPoints[i, 0];
					FPoint fPoint2 = mPoints[i, 1];
					if (i != 0 && i != 7)
					{
						float num9 = ConstantsWP.SPEEDBOARD_TIME_LIGHTNING_WIDTH;
						GlobalMembersUtils.GetRandFloat();
						GlobalMembersUtils.GetRandFloat();
						GlobalMembersUtils.GetRandFloat();
						GlobalMembersUtils.GetRandFloat();
					}
				}
			}
		}

		public override void Draw(Graphics g)
		{
			Graphics3D graphics3D = g.Get3D();
			g.PushState();
			float num = GlobalMembers.MIN((1f - mPercentDone) * 8f, 1f) * mFXManager.mBoard.GetAlpha();
			int num2 = (int)((double)num * 255.0);
			if (graphics3D != null)
			{
				SexyVertex2D[,] array = new SexyVertex2D[14, 3];
				int num3 = 0;
				for (int i = 0; i < 7; i++)
				{
					FPoint fPoint = mPoints[i, 0];
					FPoint fPoint2 = mPoints[i, 1];
					FPoint fPoint3 = mPoints[i + 1, 0];
					FPoint fPoint4 = mPoints[i + 1, 1];
					float num4 = (float)i / 7f;
					float num5 = (float)(i + 1) / 7f;
					switch (i)
					{
					case 0:
					{
						SexyVertex2D sexyVertex2D = array[num3++, 0];
						GlobalMembers.S(fPoint.mX);
						GlobalMembers.S(fPoint.mY);
						SexyVertex2D sexyVertex2D2 = array[num3 - 1, 1];
						GlobalMembers.S(fPoint4.mX);
						GlobalMembers.S(fPoint4.mY);
						SexyVertex2D sexyVertex2D3 = array[num3 - 1, 2];
						GlobalMembers.S(fPoint3.mX);
						GlobalMembers.S(fPoint3.mY);
						break;
					}
					case 6:
					{
						SexyVertex2D sexyVertex2D4 = array[num3++, 0];
						GlobalMembers.S(fPoint.mX);
						GlobalMembers.S(fPoint.mY);
						SexyVertex2D sexyVertex2D5 = array[num3 - 1, 1];
						GlobalMembers.S(fPoint2.mX);
						GlobalMembers.S(fPoint2.mY);
						SexyVertex2D sexyVertex2D6 = array[num3 - 1, 2];
						GlobalMembers.S(fPoint3.mX);
						GlobalMembers.S(fPoint3.mY);
						break;
					}
					default:
					{
						SexyVertex2D sexyVertex2D7 = array[num3++, 0];
						GlobalMembers.S(fPoint.mX);
						GlobalMembers.S(fPoint.mY);
						SexyVertex2D sexyVertex2D8 = array[num3 - 1, 1];
						GlobalMembers.S(fPoint4.mX);
						GlobalMembers.S(fPoint4.mY);
						SexyVertex2D sexyVertex2D9 = array[num3 - 1, 2];
						GlobalMembers.S(fPoint3.mX);
						GlobalMembers.S(fPoint3.mY);
						SexyVertex2D sexyVertex2D10 = array[num3++, 0];
						GlobalMembers.S(fPoint.mX);
						GlobalMembers.S(fPoint.mY);
						SexyVertex2D sexyVertex2D11 = array[num3 - 1, 1];
						GlobalMembers.S(fPoint2.mX);
						GlobalMembers.S(fPoint2.mY);
						SexyVertex2D sexyVertex2D12 = array[num3 - 1, 2];
						GlobalMembers.S(fPoint4.mX);
						GlobalMembers.S(fPoint4.mY);
						break;
					}
					}
				}
				g.DrawTrianglesTex(theColor: new Color(GlobalMembers.M(255), GlobalMembers.M(200), GlobalMembers.M(100)), theTexture: GlobalMembersResourcesWP.IMAGE_LIGHTNING_TEX, theVertices: array, theNumTriangles: num3, theDrawMode: 1, tx: g.mTransX, ty: g.mTransY, blend: true, theClipRect: default(Rect));
				g.DrawTrianglesTex(GlobalMembersResourcesWP.IMAGE_LIGHTNING_CENTER, array, num3, new Color(num2, num2, num2), 1, g.mTransX, g.mTransY, true, default(Rect));
			}
			else
			{
				g.SetDrawMode(Graphics.DrawMode.Additive);
				Color color = new Color(GlobalMembers.M(255), GlobalMembers.M(200), GlobalMembers.M(100));
				for (int j = 0; j < 7; j++)
				{
					FPoint fPoint5 = mPoints[j, 0];
					FPoint fPoint6 = mPoints[j, 1];
					FPoint fPoint7 = mPoints[j + 1, 0];
					FPoint fPoint8 = mPoints[j + 1, 1];
					float num6 = 0.3f;
					float theNum = fPoint5.mX * num6 + fPoint6.mX * (1f - num6);
					float theNum2 = fPoint5.mY * num6 + fPoint6.mY * (1f - num6);
					float theNum3 = fPoint6.mX * num6 + fPoint5.mX * (1f - num6);
					float theNum4 = fPoint6.mY * num6 + fPoint5.mY * (1f - num6);
					float theNum5 = fPoint7.mX * num6 + fPoint8.mX * (1f - num6);
					float theNum6 = fPoint7.mY * num6 + fPoint8.mY * (1f - num6);
					float theNum7 = fPoint8.mX * num6 + fPoint7.mX * (1f - num6);
					float theNum8 = fPoint8.mY * num6 + fPoint7.mY * (1f - num6);
					Point[] array2 = new Point[3];
					g.SetColor(color);
					array2[0].mX = (int)GlobalMembers.S(fPoint5.mX);
					array2[0].mY = (int)GlobalMembers.S(fPoint5.mY);
					array2[1].mX = (int)GlobalMembers.S(fPoint8.mX);
					array2[1].mY = (int)GlobalMembers.S(fPoint8.mY);
					array2[2].mX = (int)GlobalMembers.S(fPoint7.mX);
					array2[2].mY = (int)GlobalMembers.S(fPoint7.mY);
					g.PolyFill(array2, 3, false);
					array2[0].mX = (int)GlobalMembers.S(fPoint5.mX);
					array2[0].mY = (int)GlobalMembers.S(fPoint5.mY);
					array2[1].mX = (int)GlobalMembers.S(fPoint6.mX);
					array2[1].mY = (int)GlobalMembers.S(fPoint6.mY);
					array2[2].mX = (int)GlobalMembers.S(fPoint8.mX);
					array2[2].mY = (int)GlobalMembers.S(fPoint8.mY);
					g.PolyFill(array2, 3, false);
					g.SetColor(new Color(num2, num2, num2));
					array2[0].mX = (int)GlobalMembers.S(theNum);
					array2[0].mY = (int)GlobalMembers.S(theNum2);
					array2[1].mX = (int)GlobalMembers.S(theNum7);
					array2[1].mY = (int)GlobalMembers.S(theNum8);
					array2[2].mX = (int)GlobalMembers.S(theNum5);
					array2[2].mY = (int)GlobalMembers.S(theNum6);
					g.PolyFill(array2, 3, false);
					array2[0].mX = (int)GlobalMembers.S(theNum);
					array2[0].mY = (int)GlobalMembers.S(theNum2);
					array2[1].mX = (int)GlobalMembers.S(theNum3);
					array2[1].mY = (int)GlobalMembers.S(theNum4);
					array2[2].mX = (int)GlobalMembers.S(theNum7);
					array2[2].mY = (int)GlobalMembers.S(theNum8);
					g.PolyFill(array2, 3, false);
				}
				g.SetDrawMode(Graphics.DrawMode.Normal);
			}
			g.PopState();
		}

		public new static void initPool()
		{
			thePool_ = new SimpleObjectPool(512, typeof(LightningBarFillEffect));
		}

		public new static LightningBarFillEffect alloc()
		{
			LightningBarFillEffect lightningBarFillEffect = (LightningBarFillEffect)thePool_.alloc();
			lightningBarFillEffect.init();
			return lightningBarFillEffect;
		}

		public override void release()
		{
			Dispose();
			thePool_.release(this);
		}
	}
}
