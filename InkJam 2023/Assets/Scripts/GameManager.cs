using System.Collections;
using System.Collections.Generic;
using InkEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public InkWriter m_mainWriter;

    public static bool narrativeOn = false;
    private bool gameLost = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        foreach (InkWriter writer in FindObjectsOfType<InkWriter>())
        {
            writer.m_writerStartedEvent.AddListener((x) => SetNarrative = true);
            writer.m_writerFinishedEvent.AddListener((x) => SetNarrative = false);
        }
    }

    public void CaughtByZombie()
    {
        if (!gameLost)
        {
            m_mainWriter.PlayKnot("defeat");
            SetNarrative = true;
            gameLost = true;
        }
    }

    public void Victory()
    {
        m_mainWriter.PlayKnot("victory");
    }

    public void ResetAndRestart()
    {
        m_mainWriter.m_storyData.InkStory = null;
        SceneManager.LoadScene("MainMenu");
    }
    public void VictoryScreen()
    {
        SceneManager.LoadScene("WinScene");
    }

    public bool SetNarrative
    {
        get
        {
            return narrativeOn;
        }
        set
        {
            narrativeOn = value;
        }
    }

    public void PlayMusic(InkDialogueLine line, InkTextVariable variable)
    {
        MusicType musicType = MusicType.NONE;
        string targetMusicType = variable.VariableArguments[0];
        System.Enum.TryParse<MusicType>(targetMusicType, out musicType);
        if (musicType != MusicType.NONE)
        {
            AudioManager.Instance.PlayMusic(musicType, true);
        }
        else
        {
            Debug.Log("Could not find music of type " + targetMusicType);
            AudioManager.Instance.PlayMusic(MusicType.NONE, true);
        }
    }
    public void PlayAmbience(InkDialogueLine line, InkTextVariable variable)
    {
        AmbienceType ambienceType = AmbienceType.NONE;
        string targetAmbienceType = variable.VariableArguments[0];
        System.Enum.TryParse<AmbienceType>(targetAmbienceType, out ambienceType);
        if (ambienceType != AmbienceType.NONE)
        {
            AudioManager.Instance.PlayAmbience(ambienceType, true);
        }
        else
        {
            Debug.Log("Could not find ambience of type " + targetAmbienceType);
            AudioManager.Instance.PlayAmbience(AmbienceType.NONE, true);
        }
    }

    public void PlaySFX(InkDialogueLine line, InkTextVariable variable)
    {
        SFXType sfxType = SFXType.NONE;
        string targetSFXType = variable.VariableArguments[0];
        System.Enum.TryParse<SFXType>(targetSFXType, out sfxType);
        if (sfxType != SFXType.NONE)
        {
            AudioManager.Instance.PlaySFX(sfxType);
        }
        else
        {
            Debug.Log("Could not find SFX of type " + targetSFXType);
        }
    }

}
