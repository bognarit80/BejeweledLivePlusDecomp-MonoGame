using System;
using System.Collections.Generic;
using System.Linq;
using SexyFramework;

namespace JeffLib
{
	public class QRand
	{
		internal static class RandomNumbers
		{
			private static Random r;

			internal static int NextNumber()
			{
				if (r == null)
				{
					Seed();
				}
				return r.Next();
			}

			internal static int NextNumber(int ceiling)
			{
				if (r == null)
				{
					Seed();
				}
				return r.Next(ceiling);
			}

			internal static int NextNumber(int min, int max)
			{
				if (r == null)
				{
					Seed();
				}
				return r.Next(min, max);
			}

			internal static void Seed()
			{
				r = new Random();
			}

			internal static void Seed(int seed)
			{
				r = new Random(seed);
			}
		}

		public static int RAND_MAX = 32767;

		public static int gDebugFirstRand = 0;

		public static int gSwaySize = 0;

		protected int mUpdateCnt;

		protected int mLastIndex;

		protected List<float> mWeights = new List<float>();

		protected List<float> mCurSway = new List<float>();

		protected List<int> mLastHitUpdate = new List<int>();

		protected List<int> mPrevLastHitUpdate = new List<int>();

		private void Init()
		{
			RandomNumbers.Seed(0);
			mUpdateCnt = 0;
			mLastIndex = -1;
		}

		public QRand()
		{
			Init();
		}

		public QRand(float value)
		{
			Init();
			SetWeights(new List<float> { value });
		}

		public QRand(List<float> initial_weights)
		{
			Init();
			SetWeights(initial_weights);
		}

		public void SetWeights(List<float> v)
		{
			mWeights.Clear();
			if (v.Count == 1)
			{
				mWeights.Add(1f - v[0]);
				mWeights.Add(v[0]);
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < v.Count; i++)
				{
					mWeights.Add(v[i]);
					num += mWeights[i];
				}
				for (int j = 0; j < mWeights.Count; j++)
				{
					mWeights[j] /= num;
				}
			}
			for (int k = mLastHitUpdate.Count; k < mWeights.Count; k++)
			{
				mLastHitUpdate.Add(0);
				mPrevLastHitUpdate.Add(0);
			}
			mCurSway.Clear();
			SexyFramework.Common.Resize(mCurSway, mWeights.Count);
		}

		public int Next()
		{
			mUpdateCnt++;
			float num = 0f;
			for (int i = 0; i < mWeights.Count(); i++)
			{
				float num2 = mWeights[i];
				if (num2 != 0f)
				{
					float num3 = 1f / num2;
					float num4 = 1f + ((float)(mUpdateCnt - mLastHitUpdate[i]) - num3) / num3;
					float num5 = 1f + ((float)(mUpdateCnt - mPrevLastHitUpdate[i]) - num3 * 2f) / (num3 * 2f);
					float num6 = num2 * Math.Max(Math.Min(num4 * 0.75f + num5 * 0.25f, 3f), 0.333f);
					mCurSway[i] = num6;
					num += num6;
				}
				else
				{
					mCurSway[i] = 0f;
				}
			}
			float num7 = (float)RandomNumbers.NextNumber(1, RAND_MAX) / (float)RAND_MAX * num;
			gDebugFirstRand = (int)num7;
			gSwaySize = mCurSway.Count;
			int j;
			for (j = 0; j < mCurSway.Count && num7 > mCurSway[j]; j++)
			{
				num7 -= mCurSway[j];
			}
			if (j >= mCurSway.Count)
			{
				j--;
			}
			mPrevLastHitUpdate[j] = mLastHitUpdate[j];
			mLastHitUpdate[j] = mUpdateCnt;
			mLastIndex = j;
			return j;
		}

		public int NumWeights()
		{
			return mWeights.Count;
		}

		public int NumNonZeroWeights()
		{
			int num = 0;
			for (int i = 0; i < mWeights.Count; i++)
			{
				if (mWeights[i] != 0f)
				{
					num++;
				}
			}
			return num;
		}

		public void Clear()
		{
			mWeights.Clear();
			mCurSway.Clear();
			mLastHitUpdate.Clear();
			mPrevLastHitUpdate.Clear();
		}

		public bool HasWeight(int idx)
		{
			if (idx >= mWeights.Count)
			{
				return false;
			}
			return mWeights[idx] > 0f;
		}

		public void SyncState(DataSyncBase sync)
		{
			sync.SyncLong(ref mUpdateCnt);
			sync.SyncLong(ref mLastIndex);
			sync.SyncListFloat(mWeights);
			sync.SyncListFloat(mCurSway);
			sync.SyncListInt(mLastHitUpdate);
			sync.SyncListInt(mPrevLastHitUpdate);
		}
	}
}
