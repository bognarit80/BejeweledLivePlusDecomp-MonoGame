using Microsoft.Xna.Framework.Media;

namespace SexyFramework
{
	public class SoundEffectWrapper
	{
		public Song m_Song;

		public double mVolume;

		public double mVolumeAdd;

		public double mVolumeCap = 1.0;

		public bool mStopOnFade;

		public bool m_isPlaying;

		public SoundEffectWrapper(Song s)
		{
			load(s);
		}

		public void load(Song s)
		{
			m_Song = s;
		}

		public bool isPlaying()
		{
			return m_isPlaying;
		}

		public void play()
		{
			MediaPlayer.Play(m_Song);
			m_isPlaying = true;
		}

		public void stop()
		{
			MediaPlayer.Stop();
			m_isPlaying = false;
		}

		public void setLoop(bool isLooped)
		{
			MediaPlayer.IsRepeating = isLooped;
		}

		public void setVolume(float volume)
		{
			MediaPlayer.Volume = Common.CaculatePowValume(volume);
		}
	}
}
