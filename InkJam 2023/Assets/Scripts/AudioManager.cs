using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Audio;

namespace MiTale
{
    public class CustomAudioClip
    {
        public AudioClip m_clip;
    }

    [System.Serializable]
    public class MusicClip : CustomAudioClip
    {
        public MusicType m_type = MusicType.NONE;
    }

    [System.Serializable]
    public class SFXClip : CustomAudioClip
    {
        public SFXType m_type = SFXType.NONE;

        [Tooltip("Pitch varies between these two extremes every time an SFX is played")]
        [NaughtyAttributes.MinMaxSlider(-3f, 3f)]
        public Vector2 m_pitchVariance = new(1f, 1f);

        [Tooltip("Volume of the SFX clip")]
        [Range(0.0f, 1.0f)]
        public float m_volume = 1.0f;
    }

    [System.Serializable]
    public class AmbienceClip : CustomAudioClip
    {
        public AmbienceType m_type = AmbienceType.NONE;
    }

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager m_instance;
        private List<CustomAudioSource> allSources = new() { };
        public CustomAudioSource musicSource;
        public CustomAudioSource ambientSource;
        public CustomAudioSource sfxSource;
        public AudioClipData[] m_music;
        public AudioClipData[] m_ambience;
        public AudioClipData[] m_sfx;

        public AudioMixer m_audioMixer;
        public float audioVolume = 1f;
        public float musicVolume = 1f;

        public GameObject muted;
        private Coroutine m_musicFader;
        private Coroutine m_ambienceFader;
        private Coroutine m_musicQueuer;
        private AmbienceType m_currentAmbientClipType = AmbienceType.NONE;

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static AudioManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<AudioManager>();
                }
                return m_instance;
            }
            set
            {
                m_instance = value;
            }
        }

        public void ResetSources()
        {
            for (int i = 0; i < allSources.Count; i++)
            {
                if (allSources[i] == null)
                {
                    allSources.RemoveAt(i);
                }
            }
        }

        public void AddSource(CustomAudioSource source)
        {
            // Automagically happens on initialization of audio source
            if (!allSources.Contains(source))
            {
                allSources.Add(source);
                //source.volume = audioVolume;
            }
        }

        public void RemoveSource(CustomAudioSource source)
        {
            if (allSources.Contains(source))
            {
                allSources.Remove(source);
            }
        }

        public void SetAudioVolume(float volume)
        {
            m_audioMixer.SetFloat("MasterVolume", volume);
            //PlayerPrefs.SetFloat("WillowGuardMasterVolume", volume);
        }

        public void SetAudioVolumeReverse(float volume)
        {
            SetAudioVolume(1f - volume);
        }

        public void SetMusicVolumeReverse(float volume)
        {
            SetMusicVolume(1f - volume);
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.AudioSource.volume = volume;
            }

            musicVolume = volume;
        }

        public void PlaySFX(SFXType type)
        {
            SFXClip clip = GetRandomSFXClip(type);
            if (clip != null)
            {
                PlaySFX(clip);
            }
        }

        public void PlaySFX(SFXClip clip)
        {
            if (clip != null)
            {
                if (clip.m_clip != null)
                {
                    sfxSource.PlayOneShot(clip);
                    //sfxSource.AudioSource.PlayOneShot (clip.m_clip); // Audiomanager does not do volume/pitch variance
                }
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            // simple simple
            PlayMusic(clip, true);
        }

        public void PlayMusic(AudioClip clip, bool fade = true)
        {
            if (m_musicFader != null)
            {
                StopCoroutine(m_musicFader);
            }

            if (clip == null)
            {
                if (fade)
                {
                    m_musicFader =
                        StartCoroutine(FaderSwitcher(null, musicSource.AudioSource, musicVolume, m_musicFader));
                    return;
                }
                else
                {
                    musicSource.Stop();
                    return;
                }
            }
            else
            {
                if (clip != musicSource.Clip)
                {
                    if (fade)
                    {
                        m_musicFader =
                            StartCoroutine(FaderSwitcher(clip, musicSource.AudioSource, musicVolume, m_musicFader));
                    }
                    else
                    {
                        musicSource.Clip = clip;
                        musicSource.Play(musicSource.AudioSource);
                    }

                    ;
                }

                ;
            }
        }

        public void PlayMusic(MusicType type, bool fade = true)
        {
            MusicClip randomClip = GetRandomMusicClip(type);
            if (randomClip != null)
            {
                PlayMusic(randomClip.m_clip, fade);
            }
            else
            {
                Debug.LogWarning("Could not find any music of type " + type.ToString() + ", stopping music instead.");
                musicSource.Stop();
            }
        }

        public void PlayAmbience(AmbienceType type, bool fade = true)
        {
            if (m_ambienceFader != null)
            {
                StopCoroutine(m_ambienceFader);
            }

            AmbienceClip randomClip = GetRandomAmbienceClip(type); // ...not so random
            if (randomClip == null)
            {
                m_currentAmbientClipType = AmbienceType.NONE;
                if (fade)
                {
                    m_ambienceFader =
                        StartCoroutine(FaderSwitcher(null, ambientSource.AudioSource, audioVolume, m_ambienceFader));
                    return;
                }
                else
                {
                    ambientSource.AudioSource.Stop();
                    return;
                }
            }
            else
            {
                if (randomClip.m_clip != ambientSource.Clip)
                {
                    // we don't want to restart a clip that is already playing...
                    m_currentAmbientClipType = randomClip.m_type;
                    if (fade)
                    {
                        m_ambienceFader = StartCoroutine(FaderSwitcher(randomClip.m_clip, ambientSource.AudioSource,
                            audioVolume, m_ambienceFader));
                    }
                    else
                    {
                        ambientSource.Clip = randomClip.m_clip;
                        ambientSource.Play(ambientSource.AudioSource);
                    }

                    ;
                }
            }

            ;
        }

        public void QueueMusic(MusicType musicToQueue, MusicType musicToPlayAfter)
        {
            // a simple 'queue music' type thing to play e.g. victory tunes
            // we don't enqueue things in more complicated ways...
            if (m_musicQueuer != null)
            {
                StopCoroutine(m_musicQueuer);
            }

            MusicClip firstClip = GetRandomMusicClip(musicToQueue);
            MusicClip secondClip = GetRandomMusicClip(musicToPlayAfter);
            if (firstClip != null && secondClip != null)
            {
                m_musicQueuer = StartCoroutine(MusicQueue(firstClip.m_clip, secondClip.m_clip));
            }
        }

        private IEnumerator MusicQueue(AudioClip clipToPlayFirst, AudioClip clipToPlaySecond)
        {
            // First we fade into the start clip
            if (m_musicFader != null)
            {
                StopCoroutine(m_musicFader);
            }

            yield return m_musicFader =
                StartCoroutine(FaderSwitcher(clipToPlayFirst, musicSource.AudioSource, musicVolume, m_musicFader));
            // then we wait until it is done (-1 second)
            yield return new WaitForSeconds(clipToPlayFirst.length - 1f);
            // then we fade into the next one!
            yield return m_musicFader =
                StartCoroutine(FaderSwitcher(clipToPlaySecond, musicSource.AudioSource, musicVolume, m_musicFader));
            // and we're done!
            m_musicQueuer = null;
        }

        private IEnumerator FaderSwitcher(AudioClip targetClip, AudioSource targetSource, float targetVolume,
            Coroutine thisRoutine)
        {
            // Volume down
            while (targetSource.volume > 0f)
            {
                targetSource.volume -= 0.1f;
                yield return null;
            }

            targetSource.volume = 0f;
            if (targetClip != null)
            {
                targetSource.clip = targetClip;
                targetSource.Play();
            }
            else
            {
                targetSource.Stop();
            }

            // Volume up
            while (targetSource.volume < targetVolume)
            {
                targetSource.volume += 0.1f;
                yield return null;
            }

            targetSource.volume = musicVolume;
            thisRoutine = null;
        }

        private MusicClip GetRandomMusicClip(MusicType type)
        {
            List<MusicClip> randomList = new() { };
            foreach (AudioClipData data in m_music)
            {
                foreach (MusicClip clip in data.GetMusicClips())
                {
                    if (clip.m_type == type)
                    {
                        randomList.Add(clip);
                    }
                }
            }

            if (randomList.Count > 0)
            {
                return randomList[Random.Range(0, randomList.Count)];
            }
            else
            {
                Debug.LogWarning("No music of type " + type.ToString() + "found!");
                return null;
            }
        }

        private AmbienceClip GetRandomAmbienceClip(AmbienceType type)
        {
            List<AmbienceClip> randomList = new() { };
            foreach (AudioClipData data in m_ambience)
            {
                foreach (AmbienceClip clip in data.GetAmbienceClips())
                {
                    if (clip.m_type == type)
                    {
                        randomList.Add(clip);
                    }
                }
            }

            if (randomList.Count > 0)
            {
                return randomList[Random.Range(0, randomList.Count)];
            }
            else
            {
                Debug.LogWarning("No ambience of type " + type.ToString() + "found!");
                return null;
            }
        }

        private SFXClip GetRandomSFXClip(SFXType type)
        {
            List<SFXClip> randomList = new() { };
            foreach (AudioClipData data in m_sfx)
            {
                foreach (SFXClip clip in data.GetSFXClips())
                {
                    if (clip.m_type == type)
                    {
                        randomList.Add(clip);
                    }
                }
            }

            if (randomList.Count > 0)
            {
                return randomList[Random.Range(0, randomList.Count)];
            }
            else
            {
                // Debug.LogWarning ("No SFX of type " + type.ToString () + "found!");
                return null;
            }
        }

        #region convenienceMethods

        public void PlayMapMusic()
        {
            PlayMusic(MusicType.MUSIC_MAP);
            PlayAmbience(AmbienceType.NONE, true);
        }

        public void MusicFadeOut()
        {
            PlayMusic(null, true);
        }

        #endregion
    }
}