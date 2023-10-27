using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomAudioSource : MonoBehaviour
{ // This is mainly for SFX-type audio
    public AudioClipData m_audioClips;
    private AudioSource source;

    [NaughtyAttributes.ShowIf("Type", AudioClipDataType.SFX)]
    public SFXType m_randomDefaultSFXType = SFXType.NONE;
    [NaughtyAttributes.ShowIf("Type", AudioClipDataType.Music)]
    public MusicType m_randomDefaultMusicType = MusicType.NONE;
    [NaughtyAttributes.ShowIf("Type", AudioClipDataType.Ambience)]
    public AmbienceType m_randomDefaultAmbienceType = AmbienceType.NONE;

    [Tooltip("Select this when we want to use the audio manager for SFX, meaning no sound files need to be added to the custom audio source")]
    public bool m_useAudioManagerForSFX = false;
    [Tooltip("Attempts to play the randomdefaultambiencetype on enable")]
    public bool m_playOnEnable = false;

    [Tooltip("Only set to false if you know you only need one (e.g. music player, ambience player)")]
    public bool m_useAudioSourcePool = true;

    [Tooltip("When we 'expect' a lot of false positives, e.g. items that sometimes do or sometimes don't have certain types, set to false")]
    public bool m_debugWarnings = false;

    private Dictionary<SFXType, List<SFXClip>> m_sfxDict = new Dictionary<SFXType, List<SFXClip>> { };
    private List<AudioSource> m_audioSourcePool = new List<AudioSource> { };

    void Awake()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.AddSource(this);
        }
        if (m_audioClips != null)
        {
            UpdateSoundDictionary(m_audioClips.GetSFXClips());
        }
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        };
        source.playOnAwake = false; // we never want this, instead we use m_playOnEnable
    }

    void OnEnable()
    {
        if (m_randomDefaultSFXType != SFXType.NONE && Type == AudioClipDataType.SFX)
        { // set it, does not play it
            if (m_playOnEnable)
            {
                PlayOneShot(m_randomDefaultSFXType);
            }
            Clip = GetRandomClip(m_randomDefaultSFXType)?.m_clip;
        }
        if (m_randomDefaultMusicType != MusicType.NONE && Type == AudioClipDataType.Music && m_playOnEnable)
        { // this is more a convenience
            AudioManager.Instance.PlayMusic(m_randomDefaultMusicType, true);
        }
        if (m_randomDefaultAmbienceType != AmbienceType.NONE && Type == AudioClipDataType.Ambience && m_playOnEnable)
        { // more a convenience
            AudioManager.Instance.PlayAmbience(m_randomDefaultAmbienceType, true);
        }
    }
    public AudioSource AudioSource
    {
        get
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }
            return GetAudioSource();
        }
    }
    public AudioClipDataType Type
    {
        get
        {
            if (m_audioClips == null)
            {
                return AudioClipDataType.None;
            }
            else
            {
                return m_audioClips.m_type;
            }
        }
    }

    public List<AudioSource> AudioSources
    {
        get
        {
            return m_audioSourcePool;
        }
    }

    SFXClip GetRandomClip(SFXType type)
    {
        List<SFXClip> clipList = new List<SFXClip> { };
        m_sfxDict.TryGetValue(type, out clipList);
        if (clipList != null)
        {
            return clipList[Random.Range(0, clipList.Count)];
        }
        else
        {
            if (m_debugWarnings)
            {
                Debug.LogWarning("No SFXtype of type " + type + " found!", gameObject);
            };
            return null;
        }
    }

    AudioSource GetAudioSource()
    {
        if (m_useAudioSourcePool)
        {
            if (m_audioSourcePool.Count < 1)
            {
                return CreateAudioSource();
            }
            else
            {
                AudioSource tryFind = m_audioSourcePool.Find((x) => !x.isPlaying);
                if (tryFind == null)
                {
                    return CreateAudioSource();
                }
                else
                {
                    return tryFind;
                }
            }
        }
        else
        {
            return source;
        }
    }

    AudioSource CreateAudioSource()
    {
        var type = source.GetType();
        var newSource = gameObject.AddComponent(type);
        var fields = type.GetFields();
        foreach (var field in fields) field.SetValue(newSource, field.GetValue(source));
        m_audioSourcePool.Add(newSource as AudioSource);
        AudioSource newAudioSource = newSource as AudioSource;
        newAudioSource.playOnAwake = false;
        newAudioSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
        newAudioSource.rolloffMode = source.rolloffMode;
        newAudioSource.spatialBlend = source.spatialBlend;
        newAudioSource.maxDistance = source.maxDistance;
        newAudioSource.spread = source.spread;
        newAudioSource.dopplerLevel = source.dopplerLevel;
        newAudioSource.reverbZoneMix = source.reverbZoneMix;
        newAudioSource.volume = source.volume;

        return newSource as AudioSource;
    }

    public void PlayRandomType(SFXType type)
    {
        if (m_useAudioManagerForSFX)
        {
            AudioManager.Instance.PlaySFX(type);
        }
        else
        {
            SFXClip randomClip = GetRandomClip(type);
            if (randomClip != null)
            {
                AudioSource sfxSource = GetAudioSource();
                sfxSource.clip = randomClip.m_clip;
                sfxSource.pitch = Random.Range(randomClip.m_pitchVariance.x, randomClip.m_pitchVariance.y);
                sfxSource.volume = randomClip.m_volume;
                sfxSource.loop = source.loop;
                Play(sfxSource);
            }
        }
    }
    public void Play(AudioSource target)
    {
        if (target.clip == null)
        {
            // Debug.LogWarning ("No clip found!", source);
        }
        else
        {
            target.Play();
        };
    }
    public void PlayDefaultRandomSFXType()
    { // Mainly for on-click events, by setting the random SFX type first
        if (m_randomDefaultSFXType != SFXType.NONE)
        {
            PlayRandomType(m_randomDefaultSFXType);
        }
    }
    public void Stop()
    {
        AudioSource.Stop();
    }
    public AudioClip Clip
    {
        get
        {
            return AudioSource.clip;
        }
        set
        {
            AudioSource.clip = value;
        }
    }
    public void PlayAndLoop(SFXClip clip)
    {
        if (clip != null)
        {
            AudioSource sfxSource = GetAudioSource();
            sfxSource.clip = clip.m_clip;
            sfxSource.pitch = Random.Range(clip.m_pitchVariance.x, clip.m_pitchVariance.y);
            sfxSource.volume = clip.m_volume;
            sfxSource.loop = true;
            Play(sfxSource);
        }
    }
    public void PlayAndLoop(SFXType type)
    {
        SFXClip randomClip = GetRandomClip(type);
        if (randomClip != null)
        {
            AudioSource sfxSource = GetAudioSource();
            sfxSource.clip = randomClip.m_clip;
            sfxSource.pitch = Random.Range(randomClip.m_pitchVariance.x, randomClip.m_pitchVariance.y);
            sfxSource.volume = randomClip.m_volume;
            sfxSource.loop = true;
            Play(sfxSource);
        }
    }
    public void StopLoop(bool stopSound = true)
    {
        if (m_useAudioSourcePool)
        {
            foreach (AudioSource audioSource in m_audioSourcePool.FindAll((x) => x.loop))
            {
                audioSource.loop = false;
                if (stopSound)
                {
                    audioSource.Stop();
                }
            }
        }
        else
        {
            AudioSource.loop = false;
            if (stopSound)
            {
                AudioSource.Stop();
            }
        }
    }
    public void PlayOneShot(SFXClip clip)
    {
        if (clip != null)
        {
            if (m_useAudioManagerForSFX)
            {
                AudioManager.Instance.PlaySFX(clip);
            }
            else
            {
                AudioSource sfxSource = GetAudioSource();
                sfxSource.pitch = Random.Range(clip.m_pitchVariance.x, clip.m_pitchVariance.y);
                sfxSource.volume = clip.m_volume;
                sfxSource.loop = false;
                sfxSource.PlayOneShot(clip.m_clip);
            }
        }
    }
    public void PlayOneShot(SFXType type)
    {
        if (m_useAudioManagerForSFX)
        {
            AudioManager.Instance.PlaySFX(type);
        }
        else
        {
            SFXClip randomClip = GetRandomClip(type);
            if (randomClip != null)
            {
                AudioSource sfxSource = GetAudioSource();
                sfxSource.pitch = Random.Range(randomClip.m_pitchVariance.x, randomClip.m_pitchVariance.y);
                sfxSource.volume = randomClip.m_volume;
                sfxSource.loop = false;
                sfxSource.PlayOneShot(randomClip.m_clip);
            }
        }
    }

    public void UpdateSoundDictionary(List<SFXClip> newSounds, bool clearOld = false)
    {
        if (clearOld)
        {
            m_sfxDict.Clear();
        }
        if (newSounds != null)
        {
            foreach (SFXClip clip in newSounds)
            { // make a list of all them soundssss babeh
                if (m_sfxDict.ContainsKey(clip.m_type))
                {
                    m_sfxDict[clip.m_type].Add(clip);
                }
                else
                {
                    List<SFXClip> newList = new List<SFXClip> { clip };
                    m_sfxDict.Add(clip.m_type, newList);
                    // Debug.Log ("Added new sound list of type " + clip.m_type + " to dictionary. List is now: " + m_soundsDict[clip.m_type].Count + " elements deep.");
                }
            }
            // Debug.Log ("Added " + m_soundsDict.Count + " sounds to dictionary!");
        };
    }
    void OnDestroy()
    {
        //AudioManager.Instance.RemoveSource(this);
    }
}