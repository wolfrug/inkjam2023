using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClipDataType { None, Music, SFX, Ambience }

public enum MusicType
{
    NONE = 0000,
    MUSIC_MAP = 2000,
}
public enum AmbienceType
{
    NONE = 0000,
    AMBIENT_MAP = 1000,
}
public enum SFXType
{
    NONE = 0000,
    // Basic UI stuff like clicks and hover on/off
    UI_CLICK = 1000,
    UI_HOVERON = 1001,
    UI_HOVEROFF = 1002,

    UI_POINTER_DOWN = 1003,
    UI_POINTER_UP = 1004,

    // Maybe for like, sliders
    UI_DRAGSTART = 2000,
    UI_DRAGCANCEL = 2001,
    UI_DRAGCOMPLETE = 2002,
    UI_DRAGGING = 2003,
    // Probably for the actual new UI stuff
    UI_INV_TAKE = 3000,
    UI_INV_USE = 3001,
    UI_INV_OPEN = 3002,
    UI_INV_CLOSE = 3003,

    SFX_LIGHTNING = 4000,
    SFX_FOOTSTEP = 4001,
    SFX_DROPBAIT = 4002,
    SFX_LIGHTON = 4003,
    SFX_DEATHGROAN = 4004,
    SFX_TAKE_NOTE = 4005,
    SFX_ZOMBIE_MOAN = 5000,
    SFX_ZOMBIE_GROWL = 5001,
    SFX_ZOMBIE_HIT = 5002,
    SFX_ZOMBIE_FOOTSTEP = 5003,

}

[CreateAssetMenu(menuName = "MiTale/Audio/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public AudioClipDataType m_type = AudioClipDataType.SFX;
    [NaughtyAttributes.ShowIf("m_type", AudioClipDataType.Music)]
    [SerializeField] protected List<MusicClip> m_musicClips;
    [NaughtyAttributes.ShowIf("m_type", AudioClipDataType.Ambience)]
    [SerializeField] protected List<AmbienceClip> m_ambienceClips;
    [NaughtyAttributes.ShowIf("m_type", AudioClipDataType.SFX)]
    [SerializeField] protected List<SFXClip> m_sfxClips;

    public List<MusicClip> GetMusicClips()
    {
        return m_musicClips;
    }
    public List<SFXClip> GetSFXClips()
    {
        return m_sfxClips;
    }
    public List<AmbienceClip> GetAmbienceClips()
    {
        return m_ambienceClips;
    }

    public MusicClip GetRandomMusicClip(MusicType type)
    {
        List<MusicClip> randomList = new List<MusicClip> { };
        foreach (MusicClip clip in GetMusicClips())
        {
            if (clip.m_type == type)
            {
                randomList.Add(clip);
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
    public AmbienceClip GetRandomAmbienceClip(AmbienceType type)
    {
        List<AmbienceClip> randomList = new List<AmbienceClip> { };
        foreach (AmbienceClip clip in GetAmbienceClips())
        {
            if (clip.m_type == type)
            {
                randomList.Add(clip);
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

    public SFXClip GetRandomSFXClip(SFXType type)
    {
        List<SFXClip> randomList = new List<SFXClip> { };
        foreach (SFXClip clip in GetSFXClips())
        {
            if (clip.m_type == type)
            {
                randomList.Add(clip);
            }
        }
        if (randomList.Count > 0)
        {
            return randomList[Random.Range(0, randomList.Count)];
        }
        else
        {
            Debug.LogWarning("No SFX of type " + type.ToString() + "found!");
            return null;
        }
    }

}