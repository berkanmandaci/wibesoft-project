using UnityEngine;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private AudioSource _musicSource;
        private AudioSource _sfxSource;
        private LogManager _logger=> LogManager.Instance;

        private void Awake()
        {

            // Create audio sources
            _musicSource = gameObject.AddComponent<AudioSource>();
            _sfxSource = gameObject.AddComponent<AudioSource>();

            // Configure music source
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;

            // Configure SFX source
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;

            _logger.LogInfo("Audio Manager initialized", "AudioManager");
        }

        public void PlayMusic(AudioClip clip)
        {
            _musicSource.clip = clip;
            _musicSource.Play();
            _logger.LogInfo($"Playing music: {clip.name}", "AudioManager");
        }

        public void PlaySFX(AudioClip clip)
        {
            _sfxSource.PlayOneShot(clip);
            _logger.LogInfo($"Playing SFX: {clip.name}", "AudioManager");
        }

        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = volume;
            _logger.LogInfo($"Music volume set to: {volume}", "AudioManager");
        }

        public void SetSFXVolume(float volume)
        {
            _sfxSource.volume = volume;
            _logger.LogInfo($"SFX volume set to: {volume}", "AudioManager");
        }
    }
} 