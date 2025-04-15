using SexyFramework;
using SexyFramework.Graphics;

namespace JeffLib
{
	public class FColor
	{
		public float mRed;

		public float mGreen;

		public float mBlue;

		public float mAlpha;

		public FColor()
		{
			mRed = (mGreen = (mBlue = 0f));
			mAlpha = 255f;
		}

		public FColor(float r, float g, float b)
		{
			mRed = r;
			mGreen = g;
			mBlue = b;
			mAlpha = 255f;
		}

		public FColor(float r, float g, float b, float a)
		{
			mRed = r;
			mGreen = g;
			mBlue = b;
			mAlpha = a;
		}

		public Color ToColor()
		{
			return new Color((int)mRed, (int)mGreen, (int)mBlue, (int)mAlpha);
		}

		public FColor(Color rhs)
		{
			CopyFrom(rhs);
		}

		private void CopyFrom(Color rhs)
		{
			mRed = rhs.mRed;
			mGreen = rhs.mGreen;
			mBlue = rhs.mBlue;
			mAlpha = rhs.mAlpha;
		}

		public static implicit operator Color(FColor ImpliedObject)
		{
			return new Color((int)ImpliedObject.mRed, (int)ImpliedObject.mGreen, (int)ImpliedObject.mBlue, (int)ImpliedObject.mAlpha);
		}

		public static bool operator ==(FColor a, FColor b)
		{
			return a?.Equals(b) ?? ((object)b == null);
		}

		public static bool operator !=(FColor a, FColor b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is FColor)
			{
				FColor fColor = (FColor)obj;
				if (SexyFramework.Common._eq(mAlpha, fColor.mAlpha) && SexyFramework.Common._eq(mGreen, fColor.mGreen) && SexyFramework.Common._eq(mBlue, fColor.mBlue))
				{
					return SexyFramework.Common._eq(mRed, fColor.mRed);
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
