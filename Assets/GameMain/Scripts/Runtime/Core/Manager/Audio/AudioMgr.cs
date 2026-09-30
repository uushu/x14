using System.Collections.Generic;
using cfg;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace GameMain
{
    public class AudioMgr : AMonoManager<AudioMgr>
    {
        public AudioMixer audioMixer;

        public AudioSource bgmSource;

        public List<AudioSource> audioOneShot;

        public List<AudioSource> audioLoop;
        
        const string MusicVolumeTag = "MusicVolume";
        const string SoundVolumeTag = "SoundVolume";

        public bool MusicOn
        {
            get => LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(false).musicIsOn;
            set
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(true).musicIsOn = value;
                
                SetMusicMute(!value);
            }
        }

        private int _musicVolume ;
        public int MusicVolume
        {
            get=> _musicVolume;
            set
            {
                _musicVolume = value;
                
                SetVolume(MusicVolumeTag, MusicOn ? value : 0);
            }
        }

        public bool SoundOn
        {
            get => LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(false).soundIsOn;
            set
            {
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(true).soundIsOn = value;
                
                SetSoundMute(!value);
            }
        }

        private int _soundVolume;
        public int SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = value;
                SetVolume(SoundVolumeTag, SoundOn ? value : 0);
            }
        }

        public bool ShakeIsOn
        {
            get => LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(false).shakeIsOn;
            set => LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(true).shakeIsOn = value;
        }

        private readonly EventGroup _eventGroup = new EventGroup();

        protected override void OnEnter()
        {
            base.OnEnter();

            _musicVolume = _soundVolume = 100;

            SoundOn = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(false).soundIsOn;
            MusicOn = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Setting>(false).musicIsOn;
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            _eventGroup.RemoveAllListener();
        }

        public void SetMusicVolume(int volume)
        {
            SetVolume(MusicVolumeTag, volume);
        }

        public void SetSoundEffectVolume(int volume)
        {
            SetVolume(SoundVolumeTag, volume);
        }

        private void SetMusicMute(bool mute)
        {
            SetVolume(MusicVolumeTag, mute ? 0 : MusicVolume);
        }

        private void SetSoundMute(bool mute)
        {
            SetVolume(SoundVolumeTag, mute ? 0 : SoundVolume);
        }

        private void SetVolume(string volumeName, int value)
        {
            float volume = value * 0.5f - 50f;
            audioMixer.SetFloat(volumeName, volume);
        }

        private AudioSource GetAudioSourceFromOneShot()
        {
            AudioSource source = audioOneShot[0];
            audioOneShot.RemoveAt(0);

            audioOneShot.Add(source);
            source.transform.SetAsLastSibling();

            return source;
        }
        
        private AudioSource GetOneShotOnPlaying(string audioClipName)
        {
            foreach (var audioSource in audioOneShot)
            {
                if (audioSource.isPlaying && audioSource.clip != null && audioSource.clip.name == audioClipName)
                {
                    return audioSource;
                }
            }

            return null;
        }
        
        private AudioSource GetAudioSourceFromLoop(string audioClipName)
        {
            foreach (var audioSource in audioLoop)
            {
                if (audioSource.isPlaying && audioSource.clip != null && audioSource.clip.name == audioClipName)
                {
                    return audioSource;
                }
            }

            return null;
        }

        private AudioSource GetAudioSourceFromLoop()
        {
            AudioSource source = audioLoop[0];
            audioLoop.RemoveAt(0);

            audioLoop.Add(source);
            source.transform.SetAsLastSibling();

            return source;
        }

        public void PlayAudioOneShot(AudioClipConfig config)
        {
            if (config == null)
            {
                Debugger.LogError("播放音效失败 config=null");
                return;
            }

            if (config.Status == 0)
            {
                PlayAudioOneShot(config.AudioName, config.AudioVolume);
            }
        }

        public async void PlayAudioOneShot(string soundName, float audioVolume = 0.5f)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                Debugger.LogError("音效名=NULL");
                return;
            }

            var audioClip = await ResourceMgr.Instance.LoadAudioClip(soundName);
            var audioSource = GetAudioSourceFromOneShot();

            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.volume = audioVolume;
            audioSource.PlayOneShot(audioClip);
        }

        public void PlayAudioLoop(AudioClipConfig config)
        {
            PlayAudioLoop(config.AudioName, config.AudioVolume);
        }

        public async void PlayAudioLoop(string soundName, float audioVolume)
        {
            if (string.IsNullOrEmpty(soundName)) return;

            var audioClip = await ResourceMgr.Instance.LoadAudioClip(soundName);
            var audioSource = GetAudioSourceFromLoop(soundName);

            if (audioSource != null) return;

            audioSource = GetAudioSourceFromLoop();

            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }

        public bool AudioIsPlaying(string soundName)
        {
            foreach (var audioSource in audioOneShot)
            {
                if (audioSource.isPlaying && audioSource.clip.name.Equals(soundName))
                {
                    return true;
                }
            }
            
            foreach (var audioSource in audioLoop)
            {
                if (audioSource.isPlaying && audioSource.clip.name.Equals(soundName))
                {
                    return true;
                }
            }

            return false;
        }

        public void StopLoopAudio(string soundName)
        {
            if (string.IsNullOrEmpty(soundName)) return;

            var audioSource = GetAudioSourceFromLoop(soundName);

            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        public void StopOneShotAudio(string soundName)
        {
            if (string.IsNullOrEmpty(soundName)) return;

            var audioSource = GetOneShotOnPlaying(soundName);

            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        
        public void StopBgm()
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }

        public void ResumeBgm()
        {
            if (bgmSource.isPlaying == false)
            {
                var volume = bgmSource.volume;
                bgmSource.volume = 0;

                DOTween.To(() => bgmSource.volume, (value) =>
                {
                    bgmSource.volume = value;
                }, volume, 1f);

                bgmSource.Play();
            }
        }

        public void SetBgmVolume(float volume)
        {
            bgmSource.volume = volume;
        }

        public void PlayBgm(AudioClipConfig config)
        {
            if (config.Status == 0)
            {
                PlayBgm(config.AudioName, config.AudioVolume);
            }
        }

        public async void PlayBgm(string musicAudio, float volume)
        {
            if (string.IsNullOrEmpty(musicAudio))
            {
                bgmSource.Stop();
                return;
            }

            volume = Mathf.Clamp01(volume);

            if (bgmSource.isPlaying && bgmSource.clip != null && bgmSource.clip.name.Equals(musicAudio)) return;
            
            var audioClip = await ResourceMgr.Instance.LoadAudioClip(musicAudio);
            
            bgmSource.clip = audioClip;
            bgmSource.volume = volume;
            bgmSource.Play();
        }

        public void StopSoundAudio()
        {
            foreach (var audioItem in audioOneShot)
            {
                audioItem.Stop();
            }
            
            foreach (var audioItem in audioLoop)
            {
                audioItem.Stop();
            }
        }

        public void StopAllAudio()
        {
            StopSoundAudio();

            bgmSource.Stop();
        }
    }
}