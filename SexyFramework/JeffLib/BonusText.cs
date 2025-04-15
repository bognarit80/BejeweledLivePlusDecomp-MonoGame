using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace JeffLib
{
	public class BonusText
	{
		public const int CHANGE_AMOUNT = 8;

		protected float mBulgePct;

		protected float mBulgeAmt;

		protected float mBulgeDec;

		protected float mSize;

		protected int mBulgeDir;

		protected float mX;

		protected float mY;

		protected int mUpdateCnt;

		protected int mTextWidth;

		protected int mHue;

		protected int mLife;

		protected int mAlphaFadeDelay;

		protected int mImageDrawMode;

		protected bool mLeftJustifyImg;

		protected bool mDone;

		protected bool mUseSolidColor;

		protected bool mImageUsesSolidColor;

		protected string mText;

		protected Font mFont;

		protected Image mImage;

		protected float mAlpha;

		protected float mAlphaDecRate;

		protected float mSpeed;

		public int mSpace;

		public int mImageCel;

		public Color mImageColor;

		public Color mSolidColor;

		public BonusText(string pText, Font pFont, float pX, float pY, float pSpeed, int pAlphaDelay)
		{
			mBulgePct = 1f;
			mBulgeAmt = (mBulgeDec = 0f);
			mBulgeDir = 0;
			mAlphaFadeDelay = pAlphaDelay;
			mUseSolidColor = false;
			mAlpha = 255f;
			mHue = 0;
			mLife = 250;
			mSpeed = pSpeed;
			mAlphaDecRate = 1f;
			mSolidColor = new Color(Color.White);
			mX = ((pX <= -1f) ? ((float)((GlobalMembers.gSexyAppBase.mWidth - pFont.StringWidth(pText)) / 2)) : pX);
			mY = ((pY <= -1f) ? ((float)((GlobalMembers.gSexyAppBase.mHeight - pFont.GetHeight()) / 2)) : pY);
			mText = pText;
			mFont = pFont;
			mImage = null;
			mLeftJustifyImg = true;
			mImageUsesSolidColor = true;
			mImageDrawMode = 0;
			mSpace = 5;
			mImageColor = new Color(Color.White);
			mImageCel = 0;
			mDone = false;
			mUpdateCnt = 0;
			mSize = 1f;
			int num = mFont.StringWidth(pText);
			mTextWidth = num;
		}

		public BonusText()
		{
			mSize = 1f;
			mBulgePct = 1f;
			mBulgeAmt = (mBulgeDec = 0f);
			mBulgeDir = 0;
			mX = (mY = (mUpdateCnt = (mTextWidth = (mHue = (mLife = (mAlphaFadeDelay = (mImageDrawMode = 0)))))));
			mLeftJustifyImg = (mDone = (mUseSolidColor = (mImageUsesSolidColor = false)));
			mFont = null;
			mImage = null;
			mAlpha = (mAlphaDecRate = (mSpeed = 0f));
			mSpace = (mImageCel = 0);
		}

		public void Bulge(float pct, float rate, int count)
		{
			mSize = 1f;
			mBulgePct = pct;
			mBulgeAmt = rate;
			mBulgeDir = 1;
			mBulgeDec = (pct - 1f) / (float)count;
		}

		public void Update()
		{
			mUpdateCnt++;
			if (mDone)
			{
				return;
			}
			if (mBulgeDir != 0)
			{
				mSize += (float)mBulgeDir * mBulgeAmt;
				if (mBulgeDir > 0 && mSize >= mBulgePct)
				{
					mSize = mBulgePct;
					mBulgeDir = -1;
					mBulgePct -= mBulgeDec;
				}
				else if (mBulgeDir < 0 && mSize <= 1f)
				{
					mSize = 1f;
					mBulgeDir = 1;
				}
				if (SexyFramework.Common._eq(mSize, 1f) && SexyFramework.Common._leq(mBulgePct, 1f))
				{
					mSize = 1f;
					mBulgeDir = 0;
				}
			}
			mY -= mSpeed;
			if (mY < (float)(-mFont.mHeight))
			{
				mDone = true;
			}
			if (!mUseSolidColor || !mImageUsesSolidColor)
			{
				mHue = (mHue + 7) % 255;
				if (--mLife <= 0)
				{
					mLife = 0;
					if (mAlpha <= 0f)
					{
						mDone = true;
					}
				}
			}
			if (--mAlphaFadeDelay > 0)
			{
				return;
			}
			mAlpha -= mAlphaDecRate;
			if (mAlpha < 0f)
			{
				mAlpha = 0f;
				if (mUseSolidColor || mLife <= 0)
				{
					mDone = true;
				}
			}
		}

		public void Draw(Graphics g)
		{
			if (mDone)
			{
				return;
			}
			float num = (float)mLife / 18f;
			if (num > 1f)
			{
				num = 1f;
			}
			int num2 = 0;
			int theY = 0;
			int num3 = 0;
			int theX = 0;
			if (mImage != null)
			{
				if (mImage.mHeight > mFont.GetHeight())
				{
					num2 = (int)(mY + (float)((mImage.GetCelHeight() - mFont.GetHeight()) / 2) + (float)mFont.GetAscent());
					theY = (int)mY;
				}
				else
				{
					num2 = (int)(mY + (float)mFont.GetAscent());
					theY = (int)(mY + (float)((mFont.GetHeight() - mImage.mHeight) / 2));
				}
				num3 = (int)(mLeftJustifyImg ? (mX + (float)mImage.GetCelWidth() + (float)mSpace) : mX);
				theX = (int)(mLeftJustifyImg ? mX : (mX + (float)mTextWidth + (float)mSpace));
			}
			else
			{
				num2 = (int)(mY + (float)mFont.GetAscent());
				num3 = (int)mX;
			}
			Graphics3D graphics3D = g.Get3D();
			if (!SexyFramework.Common._eq(mSize, 1f) && graphics3D != null)
			{
				int theMaxWidth = 0;
				int num4 = g.GetFont().StringWidth(mText);
				int wordWrappedHeight = g.GetWordWrappedHeight(1000000, mText, -1, ref theMaxWidth, ref theMaxWidth);
				SexyTransform2D theTransform = new SexyTransform2D(false);
				theTransform.Translate(-num3 - num4 / 2 + GlobalMembers.gSexyAppBase.mScreenBounds.mX, -num2 - wordWrappedHeight / 2);
				theTransform.Scale(mSize, mSize);
				theTransform.Translate(num3 + num4 / 2 - GlobalMembers.gSexyAppBase.mScreenBounds.mX, num2 + wordWrappedHeight / 2);
				graphics3D.PushTransform(theTransform);
			}
			g.SetFont(mFont);
			Color color = ((!mUseSolidColor) ? new Color((int)((GlobalMembers.gSexyAppBase.HSLToRGB(mHue, 255, 128) & 0xFFFFFF) | ((uint)(num * 255f) << 24))) : new Color(mSolidColor));
			color.mAlpha = (int)mAlpha;
			g.SetColor(color);
			g.WriteWordWrapped(new Rect(num3, num2, 1000000, 100000), mText);
			if (mImage != null)
			{
				g.PushState();
				g.SetDrawMode(mImageDrawMode);
				if (!mImageUsesSolidColor)
				{
					g.SetColorizeImages(true);
					g.SetColor(color);
				}
				else if (mImageColor != Color.White)
				{
					g.SetColorizeImages(true);
					Color color2 = new Color(mImageColor);
					color2.mAlpha = (int)num;
					g.SetColor(color2);
				}
				g.DrawImageCel(mImage, theX, theY, mImageCel);
				g.PopState();
			}
			if (!SexyFramework.Common._eq(mSize, 1f))
			{
				graphics3D?.PopTransform();
			}
		}

		public void AddImage(Image img, bool solid_color, bool left_justify, int img_draw_mode)
		{
			mImage = img;
			mImageUsesSolidColor = solid_color;
			mLeftJustifyImg = left_justify;
			mImageDrawMode = img_draw_mode;
		}

		public void AddImage(Image img, bool solid_color, bool left_justify)
		{
			AddImage(img, solid_color, left_justify, 0);
		}

		public bool IsDone()
		{
			return mDone;
		}

		public void SetAlpha(int a)
		{
			mAlpha = a;
		}

		public void SetAlphaDelay(int d)
		{
			mAlphaFadeDelay = d;
		}

		public void SetAlphaDecRate(float d)
		{
			mAlphaDecRate = d;
		}

		public void SetMaxLife(int l)
		{
			mLife = l;
		}

		public void NoHSL()
		{
			mUseSolidColor = true;
		}

		public void SetX(float x)
		{
			mX = x;
		}

		public void SetY(float y)
		{
			mY = y;
		}

		public int GetWidth()
		{
			if (mImage == null)
			{
				return mTextWidth;
			}
			return mTextWidth + mImage.GetCelWidth() + mSpace;
		}

		public float GetX()
		{
			return mX;
		}

		public float GetY()
		{
			return mY;
		}

		public Font GetFont()
		{
			return mFont;
		}

		public string GetString()
		{
			return mText;
		}
	}
}
