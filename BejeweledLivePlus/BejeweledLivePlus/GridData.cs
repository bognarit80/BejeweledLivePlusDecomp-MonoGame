using System.Collections.Generic;

namespace BejeweledLivePlus
{
	public class GridData
	{
		public class TileData
		{
			public uint mBack;

			public char mAttr;

			public TileData()
			{
				mBack = 0u;
				mAttr = '\0';
			}
		}

		public List<TileData> mTiles = new List<TileData>();

		public int GetRowCount()
		{
			return mTiles.Count / 8;
		}

		public void AddRow()
		{
			for (int i = 0; i < 8; i++)
			{
				mTiles.Add(new TileData());
			}
		}

		public TileData At(int theRow, int theCol)
		{
			int num = 8 * theRow + theCol;
			if (mTiles.Count <= num)
			{
				int num2 = num + 1 - mTiles.Count;
				for (int i = 0; i < num2; i++)
				{
					mTiles.Add(new TileData());
				}
			}
			return mTiles[num];
		}
	}
}
