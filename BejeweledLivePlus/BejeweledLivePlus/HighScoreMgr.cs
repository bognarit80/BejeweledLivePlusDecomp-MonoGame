using System.Collections.Generic;

namespace BejeweledLivePlus
{
	public class HighScoreMgr
	{
		public Dictionary<string, HighScoreTable> mHighScoreMap = new Dictionary<string, HighScoreTable>();

		public bool mNeedSave;

		public HighScoreMgr()
		{
			mNeedSave = false;
		}

		public bool Submit(string theTable, string theName, int theValue, int thePicture)
		{
			HighScoreTable orCreateTable = GetOrCreateTable(theTable);
			if (orCreateTable.Submit(theName, theValue, thePicture))
			{
				GlobalMembers.gApp.SaveHighscores();
				return true;
			}
			return false;
		}

		public HighScoreTable GetOrCreateTable(string theTable)
		{
			string highScoreTableId = GlobalMembers.gApp.GetHighScoreTableId(theTable);
			for (int i = 0; i < 7 && !(GlobalMembers.gApp.GetModeHeading((GameMode)i) == theTable); i++)
			{
			}
			HighScoreTable value = null;
			if (mHighScoreMap.TryGetValue(highScoreTableId, out value))
			{
				return value;
			}
			mHighScoreMap.Add(highScoreTableId, new HighScoreTable(highScoreTableId));
			mHighScoreMap[highScoreTableId].mManager = this;
			mNeedSave = true;
			return mHighScoreMap[highScoreTableId];
		}
	}
}
