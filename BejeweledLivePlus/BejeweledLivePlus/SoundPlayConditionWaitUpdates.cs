namespace BejeweledLivePlus
{
	public class SoundPlayConditionWaitUpdates : SoundPlayCondition
	{
		private int updateElapsed;

		public int NumUpdates { get; set; }

		public SoundPlayConditionWaitUpdates()
		{
			NumUpdates = 0;
			updateElapsed = 0;
		}

		public SoundPlayConditionWaitUpdates(int numUpdates)
		{
			NumUpdates = numUpdates;
			updateElapsed = 0;
		}

		public override void update()
		{
			updateElapsed++;
		}

		public override bool shouldActivate()
		{
			return updateElapsed >= NumUpdates;
		}
	}
}
