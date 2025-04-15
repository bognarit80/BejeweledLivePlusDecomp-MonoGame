using Microsoft.Xna.Framework.Content;

namespace SexyFramework
{
	public class ProxyMusicInterface : MusicInterface
	{
		private MusicInterface mTargetInterface;

		private bool mDeleteTarget;

		public ProxyMusicInterface(MusicInterface theTargetInterface, bool deleteTarget)
		{
			mTargetInterface = theTargetInterface;
			mDeleteTarget = deleteTarget;
		}

		public override void Dispose()
		{
			if (mDeleteTarget && mTargetInterface != null)
			{
				mTargetInterface.Dispose();
			}
			base.Dispose();
		}

		public override bool LoadMusic(int theSongId, string theFileName, ContentManager content)
		{
			return mTargetInterface.LoadMusic(theSongId, theFileName, content);
		}

		public override void PlayMusic(int theSongId, int theOffset, bool noLoop)
		{
			PlayMusic(theSongId, theOffset, noLoop, 0L);
		}

		public override void PlayMusic(int theSongId, int theOffset)
		{
			PlayMusic(theSongId, theOffset, false, 0L);
		}

		public override void PlayMusic(int theSongId)
		{
			PlayMusic(theSongId, 0, false, 0L);
		}

		public override void PlayMusic(int theSongId, int theOffset, bool noLoop, long theStartPos)
		{
			mTargetInterface.PlayMusic(theSongId, theOffset, noLoop, theStartPos);
		}

		public override void StopMusic(int theSongId)
		{
			mTargetInterface.StopMusic(theSongId);
		}

		public override void PauseMusic(int theSongId)
		{
			mTargetInterface.PauseMusic(theSongId);
		}

		public override void ResumeMusic(int theSongId)
		{
			mTargetInterface.ResumeMusic(theSongId);
		}

		public override void StopAllMusic()
		{
			mTargetInterface.StopAllMusic();
		}

		public override void UnloadMusic(int theSongId)
		{
			mTargetInterface.UnloadMusic(theSongId);
		}

		public override void UnloadAllMusic()
		{
			mTargetInterface.UnloadAllMusic();
		}

		public override void PauseAllMusic()
		{
			mTargetInterface.PauseAllMusic();
		}

		public override void ResumeAllMusic()
		{
			mTargetInterface.ResumeAllMusic();
		}

		public override void FadeIn(int theSongId, int theOffset, double theSpeed)
		{
			FadeIn(theSongId, theOffset, theSpeed, false);
		}

		public override void FadeIn(int theSongId, int theOffset)
		{
			FadeIn(theSongId, theOffset, 0.002, false);
		}

		public override void FadeIn(int theSongId)
		{
			FadeIn(theSongId, -1, 0.002, false);
		}

		public override void FadeIn(int theSongId, int theOffset, double theSpeed, bool noLoop)
		{
			mTargetInterface.FadeIn(theSongId, theOffset, theSpeed, noLoop);
		}

		public override void FadeOut(int theSongId, bool stopSong)
		{
			FadeOut(theSongId, stopSong, 0.004);
		}

		public override void FadeOut(int theSongId)
		{
			FadeOut(theSongId, true, 0.004);
		}

		public override void FadeOut(int theSongId, bool stopSong, double theSpeed)
		{
			mTargetInterface.FadeOut(theSongId, stopSong, theSpeed);
		}

		public override void FadeOutAll(bool stopSong)
		{
			FadeOutAll(stopSong, 0.004);
		}

		public override void FadeOutAll()
		{
			FadeOutAll(true, 0.004);
		}

		public override void FadeOutAll(bool stopSong, double theSpeed)
		{
			mTargetInterface.FadeOutAll(stopSong, theSpeed);
		}

		public override void SetSongVolume(int theSongId, double theVolume)
		{
			mTargetInterface.SetSongVolume(theSongId, theVolume);
		}

		public override void SetSongMaxVolume(int theSongId, double theMaxVolume)
		{
			mTargetInterface.SetSongMaxVolume(theSongId, theMaxVolume);
		}

		public override bool IsPlaying(int theSongId)
		{
			return mTargetInterface.IsPlaying(theSongId);
		}

		public override void SetVolume(double theVolume)
		{
			mTargetInterface.SetVolume(theVolume);
		}

		public override void SetMusicAmplify(int theSongId, double theAmp)
		{
			mTargetInterface.SetMusicAmplify(theSongId, theAmp);
		}

		public override void Update()
		{
			mTargetInterface.Update();
		}
	}
}
