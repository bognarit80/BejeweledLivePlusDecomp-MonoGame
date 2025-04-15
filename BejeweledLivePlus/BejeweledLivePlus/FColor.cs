using SexyFramework.Graphics;

namespace BejeweledLivePlus
{
	public class FColor
	{
		public float mRed;

		public float mGreen;

		public float mBlue;

		public Color GetColor()
		{
			return new Color((int)mRed, (int)mGreen, (int)mBlue);
		}

		public void Lerp(Color theColor)
		{
			mRed += ((float)theColor.mRed - mRed) / 20f;
			mGreen += ((float)theColor.mGreen - mGreen) / 20f;
			mBlue += ((float)theColor.mBlue - mBlue) / 20f;
		}
	}
}
