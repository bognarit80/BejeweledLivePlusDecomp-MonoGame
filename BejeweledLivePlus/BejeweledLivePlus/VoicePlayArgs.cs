namespace BejeweledLivePlus
{
	public class VoicePlayArgs
	{
		public int SoundID { get; set; }

		public int Pan { get; set; }

		public double Volume { get; set; }

		public int InterruptID { get; set; }

		public SoundPlayCondition Condition { get; set; }

		public VoicePlayArgs()
		{
			SoundID = -1;
			Pan = 0;
			Volume = 1.0;
			InterruptID = -1;
			Condition = null;
		}

		public VoicePlayArgs(int id, int pan, double volume, int interruptId, SoundPlayCondition condition)
		{
			SoundID = id;
			Pan = pan;
			Volume = volume;
			InterruptID = interruptId;
			Condition = condition;
		}
	}
}
