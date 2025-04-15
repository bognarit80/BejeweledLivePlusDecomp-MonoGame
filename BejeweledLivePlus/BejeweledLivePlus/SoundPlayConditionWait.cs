using SexyFramework.Sound;

namespace BejeweledLivePlus
{
	public class SoundPlayConditionWait : SoundPlayCondition
	{
		public SoundInstance WaitInstance { get; set; }

		public SoundPlayConditionWait()
		{
			WaitInstance = null;
		}

		public SoundPlayConditionWait(int waitId)
		{
			WaitInstance = GlobalMembers.gApp.mSoundManager.GetExistSoundInstance(waitId);
		}

		public override void update()
		{
		}

		public override bool shouldActivate()
		{
			bool result = true;
			SoundInstance waitInstance = WaitInstance;
			if (waitInstance != null && waitInstance.IsPlaying())
			{
				result = false;
			}
			return result;
		}
	}
}
