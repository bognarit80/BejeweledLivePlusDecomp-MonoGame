using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;

namespace JeffLib
{
	public class FwooshImage
	{
		public const uint SEXY_RAND_MAX = 2147483647u;

		public MemoryImage mImage;

		public float mAlpha;

		public int mX;

		public int mY;

		public int mDelay;

		public float mSize;

		public bool mIncText;

		public bool mIsDelaying;

		public float mSizeInc;

		public float mAlphaDec;

		public float mMaxSize;

		public bool mForward;

		public Transform mGlobalTransform = new Transform();

		public FwooshImage()
		{
			mImage = null;
			mIsDelaying = false;
			mAlpha = 255f;
			mX = 0;
			mY = 0;
			mDelay = 0;
			mSize = 0f;
			mIncText = true;
			mAlphaDec = 2f;
			mSizeInc = 0.07f;
			mMaxSize = 1.2f;
			mForward = true;
		}

		public void Update()
		{
			if (mAlpha < 255f && mAlpha > 0f)
			{
				mAlpha -= mAlphaDec;
				if (mAlpha < 0f)
				{
					mAlpha = 0f;
				}
			}
			else if (mIncText && mSize < mMaxSize)
			{
				mSize += mSizeInc;
				if (mSize >= mMaxSize)
				{
					mIncText = false;
				}
			}
			else
			{
				if (mIncText || (!(mSize > 1f) && mForward && mDelay <= 0))
				{
					return;
				}
				if ((mSize > 1f || mDelay == 0 || !mForward) && !mIsDelaying)
				{
					if (!mForward && mDelay > 0)
					{
						mDelay--;
					}
					if (mForward || mDelay <= 0)
					{
						mSize -= mSizeInc;
					}
					if (!mForward && mSize < 0f)
					{
						mSize = 0f;
					}
					else if (mDelay > 0 && mSize <= 1f)
					{
						mSize = 1f;
						mIsDelaying = true;
					}
				}
				else if (mDelay > 0)
				{
					mDelay--;
					if (mDelay == 0)
					{
						mIsDelaying = false;
					}
				}
				if (mSize <= 1f && mDelay == 0 && mForward)
				{
					mSize = 1f;
					mAlpha -= mAlphaDec;
				}
			}
		}

		public void Draw(Graphics g)
		{
			if (mImage != null && !(mAlpha <= 0f) && !(mSize <= 0f))
			{
				mGlobalTransform.Reset();
				if (!SexyFramework.Common._eq(mSize, 1f))
				{
					mGlobalTransform.Scale(mSize, mSize);
				}
				g.PushState();
				if (mAlpha != 255f)
				{
					g.SetColor(255, 255, 255, (int)mAlpha);
					g.SetColorizeImages(true);
				}
				if (!g.Is3D())
				{
					g.DrawImageTransform(mImage, mGlobalTransform, mX, mY);
				}
				else
				{
					g.DrawImageTransformF(mImage, mGlobalTransform, mX, mY);
				}
				g.SetColorizeImages(false);
				g.PopState();
			}
		}

		public void Reverse()
		{
			mForward = false;
			mIncText = true;
		}

		public static void inlineUpper(ref string theData)
		{
			theData = theData.ToUpper();
		}

		public static void inlineLower(ref string theData)
		{
			theData = theData.ToLower();
		}

		public static void inlineLTrim(ref string theData)
		{
			inlineLTrim(ref theData, " \t\r\n");
		}

		public static void inlineLTrim(ref string theData, string theChars)
		{
			for (int i = 0; i < theData.Length; i++)
			{
				if (theChars.IndexOf(theData[i]) < 0)
				{
					theData = theData.Remove(0, i);
					break;
				}
			}
		}

		public static void inlineRTrim(ref string theData)
		{
			inlineRTrim(ref theData, " \t\r\n");
		}

		public static void inlineRTrim(ref string theData, string theChars)
		{
			for (int num = theData.Length - 1; num >= 0; num--)
			{
				if (theChars.IndexOf(theData[num]) < 0)
				{
					theData = theData.Remove(num + 1);
					break;
				}
			}
		}

		public static void inlineTrim(ref string theData)
		{
			inlineTrim(ref theData, " \t\r\n");
		}

		public static void inlineTrim(ref string theData, string theChars)
		{
			inlineRTrim(ref theData, theChars);
			inlineLTrim(ref theData, theChars);
		}
	}
}
