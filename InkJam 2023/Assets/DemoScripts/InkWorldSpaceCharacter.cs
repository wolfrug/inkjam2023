using System.Collections;
using System.Collections.Generic;
using InkEngine;
using UnityEngine;

public class InkWorldSpaceCharacter : MonoBehaviour
{
    public GameObject m_target;
    public string m_characterTag;
    public InkWorldTextWriter m_targetWriter;
    public bool m_initOnEnable = true;
    [HideInInspector] public GenericWorldSpaceToCanvasIcon m_icon;

    void OnEnable()
    {
        if (m_initOnEnable)
        {
            Init();
        }
    }

    public void Init()
    {
        if (m_target == null)
        {
            m_target = gameObject;
        }
        m_icon = m_target.GetComponent<GenericWorldSpaceToCanvasIcon>();
        if (m_icon == null)
        {
            m_icon = m_target.AddComponent<GenericWorldSpaceToCanvasIcon>();
        }
        if (m_characterTag == "")
        {
            m_characterTag = gameObject.name;
        }
        if (m_targetWriter == null)
        {
            m_targetWriter = FindObjectOfType<InkWorldTextWriter>();
            m_targetWriter?.AddInkWorldSpaceCharacter(this);
        }
    }
    public GenericWorldSpaceToCanvasIcon Icon
    {
        get
        {
            return m_target.AddComponent<GenericWorldSpaceToCanvasIcon>();
        }
    }
    public void PlayKnot(string knot)
    {
        if (m_targetWriter != null)
        {
            m_targetWriter.PlayKnot(knot);
        }
    }
    public void SetCharacterAndPlayKnot(string knot)
    {
        if (m_targetWriter != null)
        {
            m_targetWriter.SetCurrentSpeakingCharacter(this);
            PlayKnot(knot);
        }
    }

}
