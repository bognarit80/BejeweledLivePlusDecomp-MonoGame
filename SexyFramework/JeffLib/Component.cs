using System.Collections.Generic;
using SexyFramework.Graphics;

namespace JeffLib
{
	public class Component
	{
		public float mValue;

		public float mOriginalValue;

		public float mTargetValue;

		public int mStartFrame;

		public int mEndFrame;

		public float mValueDelta;

		public Component()
		{
			mValue = 0f;
			mOriginalValue = 0f;
			mTargetValue = 0f;
			mStartFrame = 0;
			mEndFrame = 0;
			mValueDelta = 0f;
		}

		public Component(float val)
		{
			mValue = (mOriginalValue = (mTargetValue = val));
			mStartFrame = 0;
			mEndFrame = 0;
		}

		public Component(float val, float target)
		{
			mValue = (mOriginalValue = val);
			mTargetValue = target;
			mStartFrame = 0;
			mEndFrame = 0;
			mValueDelta = ((mEndFrame - mStartFrame == 0) ? 0f : ((target - val) / (float)(mEndFrame - mStartFrame)));
		}

		public Component(float val, float target, int start)
		{
			mValue = (mOriginalValue = val);
			mTargetValue = target;
			mStartFrame = start;
			mEndFrame = 0;
			mValueDelta = ((mEndFrame - mStartFrame == 0) ? 0f : ((target - val) / (float)(mEndFrame - mStartFrame)));
		}

		public Component(float val, float target, int start, int end)
		{
			mValue = (mOriginalValue = val);
			mTargetValue = target;
			mStartFrame = start;
			mEndFrame = end;
			mValueDelta = ((mEndFrame - mStartFrame == 0) ? 0f : ((target - val) / (float)(mEndFrame - mStartFrame)));
		}

		public bool Active(int count)
		{
			if (count >= mStartFrame)
			{
				return count <= mEndFrame;
			}
			return false;
		}

		public void SyncState(DataSyncBase sync)
		{
		}

		public void Update()
		{
			mValue += mValueDelta;
			if ((mValueDelta > 0f && mValue > mTargetValue) || (mValueDelta < 0f && mValue < mTargetValue))
			{
				mValue = mTargetValue;
			}
		}

		public static bool UpdateComponentVec(List<Component> vec, int update_count)
		{
			bool result = true;
			for (int i = 0; i < vec.Count; i++)
			{
				Component component = vec[i];
				if (component.Active(update_count))
				{
					component.Update();
					return false;
				}
				if (update_count < component.mStartFrame)
				{
					result = false;
				}
			}
			return result;
		}

		public static bool UpdateComponentVec(List<KeyValuePair<Component, Image>> vec, int update_count)
		{
			bool result = true;
			for (int i = 0; i < vec.Count; i++)
			{
				Component key = vec[i].Key;
				if (key.Active(update_count))
				{
					key.Update();
					return false;
				}
				if (update_count < key.mStartFrame)
				{
					result = false;
				}
			}
			return result;
		}

		public static float GetComponentValue(List<Component> v, float def_value, int update_count)
		{
			for (int i = 0; i < v.Count; i++)
			{
				Component component = v[i];
				if (update_count < component.mStartFrame)
				{
					return component.mValue;
				}
				if (component.Active(update_count))
				{
					return component.mValue;
				}
				if (i == v.Count - 1)
				{
					return component.mValue;
				}
			}
			return def_value;
		}
	}
}
