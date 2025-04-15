namespace SexyFramework
{
	public class NativeDisplay
	{
		public int mRGBBits;

		public ulong mRedMask;

		public ulong mGreenMask;

		public ulong mBlueMask;

		public int mRedBits;

		public int mGreenBits;

		public int mBlueBits;

		public int mRedShift;

		public int mGreenShift;

		public int mBlueShift;

		public int[] mRedAddTable;

		public int[] mGreenAddTable;

		public int[] mBlueAddTable;

		public ulong[] mRedConvTable;

		public ulong[] mGreenConvTable;

		public ulong[] mBlueConvTable;

		public NativeDisplay()
		{
			mRedConvTable = new ulong[256];
			mGreenConvTable = new ulong[256];
			mBlueConvTable = new ulong[256];
			mRGBBits = 0;
			mRedMask = 0uL;
			mGreenMask = 0uL;
			mBlueMask = 0uL;
			mRedBits = 0;
			mGreenBits = 0;
			mBlueBits = 0;
			mRedShift = 0;
			mGreenShift = 0;
			mBlueShift = 0;
			mRedAddTable = null;
			mGreenAddTable = null;
			mBlueAddTable = null;
		}
	}
}
