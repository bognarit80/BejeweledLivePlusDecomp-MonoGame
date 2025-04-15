using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SexyFramework
{
	public class SoundEffectMusicInterface : MusicInterface
	{
		protected Dictionary<int, SoundEffectWrapper> m_SoundDict = new Dictionary<int, SoundEffectWrapper>();

		private SoundEffectWrapper m_CurrSong;

		private int m_CurrSongID = -1;

		private bool m_PauseByFunction;

		protected bool m_onDeactive;

		protected bool m_onServiceDeactive;

		protected bool m_onServiceActive;

		public int mUpdateCount;

		public int mActiveCount;

		public event SongChangedEventHandle mHandle;

		public void SongChanged(object sender, EventArgs e)
		{
		}

		public SoundEffectMusicInterface()
		{
			m_isUserMusicOn = !MediaPlayer.GameHasControl;
			MediaPlayer.MediaStateChanged += OnMediaPlayerStateChanged;
		}

		public override bool isPlayingUserMusic()
		{
			return !MediaPlayer.GameHasControl;
		}

		public override void stopUserMusic()
		{
			m_PauseByFunction = true;
			MediaPlayer.Pause();
			if (m_CurrSong != null)
			{
				m_CurrSong.play();
				MediaPlayer.IsRepeating = !m_CurrSong.mStopOnFade;
				MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
			}
		}

		public override bool LoadMusic(int theSongId, string theFileName, ContentManager content)
		{
			Song song = null;
			try
			{
				song = content.Load<Song>(theFileName);
			}
			catch (Exception)
			{
				song = null;
				return false;
			}
			if (m_SoundDict.ContainsKey(theSongId))
			{
				m_SoundDict[theSongId].load(song);
			}
			else
			{
				m_SoundDict.Add(theSongId, new SoundEffectWrapper(song));
			}
			return true;
		}

		public override void UnloadAllMusic()
		{
			m_SoundDict.Clear();
		}

		public override void PlayMusic(int theSongId, int theOffset, bool noLoop, long theStartPos)
		{
			StopAllMusic();
			if (m_SoundDict.ContainsKey(theSongId))
			{
				m_CurrSongID = theSongId;
				m_CurrSong = m_SoundDict[theSongId];
				m_SoundDict[theSongId].mStopOnFade = noLoop;
				m_SoundDict[theSongId].mVolume = (m_SoundDict[theSongId].mVolumeCap = m_MusicVolume);
				m_SoundDict[theSongId].mVolumeAdd = 0.0;
				if (MediaPlayer.GameHasControl)
				{
					m_SoundDict[theSongId].play();
					MediaPlayer.IsRepeating = !noLoop;
					MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
				}
			}
		}

		public override void SetVolume(double theVolume)
		{
			m_MusicVolume = (float)theVolume;
			if (!m_isUserMusicOn)
			{
				MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
			}
		}

		public override void StopAllMusic()
		{
			if (m_isUserMusicOn)
			{
				return;
			}
			MediaPlayer.Stop();
			foreach (SoundEffectWrapper value in m_SoundDict.Values)
			{
				value.mVolumeAdd = 0.0;
				value.mVolume = 0.0;
				value.m_isPlaying = false;
			}
			m_CurrSong = null;
			m_CurrSongID = -1;
		}

		public override void PauseAllMusic()
		{
			if (!m_isUserMusicOn && MediaPlayer.State != MediaState.Paused && m_CurrSong != null)
			{
				m_PauseByFunction = true;
				MediaPlayer.Pause();
			}
		}

		public override void ResumeAllMusic()
		{
			if (m_isUserMusicOn || !m_PauseByFunction || m_CurrSong == null)
			{
				return;
			}
			m_PauseByFunction = false;
			MediaQueue queue = MediaPlayer.Queue;
			Song activeSong = queue.ActiveSong;
			if (m_CurrSong.m_Song.Name != activeSong.Name)
			{
				m_CurrSong.play();
				MediaPlayer.IsRepeating = !m_CurrSong.mStopOnFade;
				MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
				SongChangedEventArgs songChangedEventArgs = new SongChangedEventArgs();
				songChangedEventArgs.songID = m_CurrSongID;
				songChangedEventArgs.loop = !m_CurrSong.mStopOnFade;
				if (this.mHandle != null)
				{
					this.mHandle(this, songChangedEventArgs);
				}
			}
			else
			{
				MediaPlayer.Resume();
			}
		}

		public override void FadeIn(int theSongId, int theOffset, double theSpeed, bool noLoop)
		{
			PlayMusic(theSongId, theOffset, noLoop);
		}

		public override void FadeOut(int theSongId, bool stopSong, double theSpeed)
		{
			StopAllMusic();
		}

		public override void FadeOutAll(bool stopSong, double theSpeed)
		{
			if (!m_isUserMusicOn)
			{
				StopAllMusic();
			}
		}

		public override bool IsPlaying(int theSongId)
		{
			if (m_isUserMusicOn)
			{
				return false;
			}
			if (m_SoundDict.ContainsKey(theSongId))
			{
				return m_SoundDict[theSongId].isPlaying();
			}
			return false;
		}

		public override void Update()
		{
			if (mGameEvent == EGameEvent.State_OnDeactive || mServiceEvent == EServiceEvent.State_OnServiceDeactive)
			{
				return;
			}
			if ((mCurState == EMusicInterfaceState.State_GameMusicStopedInGame || mCurState == EMusicInterfaceState.State_UserMusicStoppedInGame) && m_onDeactive && !m_onServiceDeactive && mUpdateCount > 1)
			{
				mCurState = EMusicInterfaceState.State_None;
				m_onDeactive = false;
				MediaPlayer.Resume();
				m_isUserMusicOn = !MediaPlayer.GameHasControl;
			}
			else if (mCurState == EMusicInterfaceState.State_UserMusicStoppedInGame && !m_onDeactive && !m_onServiceDeactive && mUpdateCount > 100)
			{
				mCurState = EMusicInterfaceState.State_None;
				mUpdateCount = 0;
				if (m_CurrSong == null || m_CurrSong.mStopOnFade)
				{
					return;
				}
				MediaQueue queue = MediaPlayer.Queue;
				Song activeSong = queue.ActiveSong;
				if (m_CurrSong.m_Song.Name != activeSong.Name)
				{
					m_CurrSong.play();
					MediaPlayer.IsRepeating = !m_CurrSong.mStopOnFade;
					MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
					SongChangedEventArgs songChangedEventArgs = new SongChangedEventArgs();
					songChangedEventArgs.songID = m_CurrSongID;
					songChangedEventArgs.loop = !m_CurrSong.mStopOnFade;
					if (this.mHandle != null)
					{
						this.mHandle(this, songChangedEventArgs);
					}
				}
				else
				{
					MediaPlayer.Resume();
				}
				m_isUserMusicOn = !MediaPlayer.GameHasControl;
			}
			else if (mCurState == EMusicInterfaceState.State_GameMusicStopedInGame && !m_onDeactive && !m_onServiceDeactive && mUpdateCount > 1)
			{
				mCurState = EMusicInterfaceState.State_None;
				mUpdateCount = 0;
				if (m_CurrSong == null || m_CurrSong.mStopOnFade)
				{
					return;
				}
				MediaQueue queue2 = MediaPlayer.Queue;
				Song activeSong2 = queue2.ActiveSong;
				if (m_CurrSong.m_Song.Name != activeSong2.Name)
				{
					m_CurrSong.play();
					MediaPlayer.IsRepeating = !m_CurrSong.mStopOnFade;
					MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
					SongChangedEventArgs songChangedEventArgs2 = new SongChangedEventArgs();
					songChangedEventArgs2.songID = m_CurrSongID;
					songChangedEventArgs2.loop = !m_CurrSong.mStopOnFade;
					if (this.mHandle != null)
					{
						this.mHandle(this, songChangedEventArgs2);
					}
				}
				else
				{
					MediaPlayer.Resume();
				}
				m_isUserMusicOn = !MediaPlayer.GameHasControl;
			}
			else if (mCurState == EMusicInterfaceState.State_UserMusicStopedOutGame && m_isUserMusicOn && MediaPlayer.State != MediaState.Playing && m_onDeactive && m_onServiceDeactive && mUpdateCount > 1)
			{
				m_onDeactive = false;
				m_onServiceDeactive = false;
				mCurState = EMusicInterfaceState.State_None;
				if (m_CurrSong == null || m_CurrSong.mStopOnFade)
				{
					return;
				}
				MediaQueue queue3 = MediaPlayer.Queue;
				Song activeSong3 = queue3.ActiveSong;
				if (m_CurrSong.m_Song.Name != activeSong3.Name)
				{
					m_CurrSong.play();
					MediaPlayer.IsRepeating = !m_CurrSong.mStopOnFade;
					MediaPlayer.Volume = Common.CaculatePowValume(m_MusicVolume);
					SongChangedEventArgs songChangedEventArgs3 = new SongChangedEventArgs();
					songChangedEventArgs3.songID = m_CurrSongID;
					songChangedEventArgs3.loop = !m_CurrSong.mStopOnFade;
					if (this.mHandle != null)
					{
						this.mHandle(this, songChangedEventArgs3);
					}
				}
				else
				{
					MediaPlayer.Resume();
				}
				m_isUserMusicOn = !MediaPlayer.GameHasControl;
			}
			else if (mCurState == EMusicInterfaceState.State_None)
			{
				m_onServiceDeactive = false;
				m_onDeactive = false;
				mUpdateCount = 0;
			}
			else
			{
				mUpdateCount++;
			}
			bool isUserMusicOn = m_isUserMusicOn;
		}

		public override void OnDeactived()
		{
			m_onDeactive = true;
			mGameEvent = EGameEvent.State_OnDeactive;
		}

		public override void OnActived()
		{
			if (!MediaPlayer.GameHasControl)
			{
				m_isUserMusicOn = true;
				mCurState = EMusicInterfaceState.State_None;
				m_onDeactive = false;
			}
			mActiveCount = 1;
			mGameEvent = EGameEvent.State_OnActived;
		}

		public override void OnServiceDeactived()
		{
			m_onServiceDeactive = true;
			m_onServiceActive = false;
			mServiceEvent = EServiceEvent.State_OnServiceDeactive;
		}

		public override void OnServiceActived()
		{
			m_onServiceActive = true;
			mServiceEvent = EServiceEvent.State_OnServiceActived;
			if (mActiveCount == 1 && m_isUserMusicOn && MediaPlayer.State != MediaState.Playing)
			{
				mCurState = EMusicInterfaceState.State_UserMusicStopedOutGame;
			}
			mActiveCount = 0;
		}

		public override void RegisterCallback(SongChangedEventHandle handle)
		{
			mHandle += handle;
		}

		public override void OnMediaPlayerStateChanged(object sender, EventArgs e)
		{
			MediaState state = MediaPlayer.State;
			if (m_isUserMusicOn && state == MediaState.Paused)
			{
				mCurState = EMusicInterfaceState.State_UserMusicStoppedInGame;
				mUpdateCount = 0;
			}
			else if (state == MediaState.Paused)
			{
				mCurState = EMusicInterfaceState.State_GameMusicStopedInGame;
				mUpdateCount = 0;
			}
			else
			{
				mCurState = EMusicInterfaceState.State_None;
			}
			m_isUserMusicOn = !MediaPlayer.GameHasControl;
		}
	}
}
