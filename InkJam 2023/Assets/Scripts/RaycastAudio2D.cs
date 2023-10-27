using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAudio2D : MonoBehaviour
{
    public Transform m_listener;
    public CustomAudioSource m_source;
    public LayerMask m_targetMasks;

    public float m_volumeDecreasePerObject = 0.25f;

    void Start()
    {
        if (m_listener == null)
        {
            m_listener = FindObjectOfType<AudioListener>().transform;
        }
        if (m_source == null)
        {
            m_source = GetComponent<CustomAudioSource>();
        }
    }

    public int RayCastTest2D(GameObject target)
    {
        //precompute our ray settings
        Vector3 start = transform.position;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float distance = 100f;

        //draw the ray in the editor
        Debug.DrawRay(start, direction * distance, Color.red);

        //do the ray test
        RaycastHit2D[] sightTestResults = Physics2D.RaycastAll(start, direction, distance, m_targetMasks.value);

        //now iterate over all results to work out what has happened
        int count = 0;
        for (int i = 0; i < sightTestResults.Length; i++)
        {
            Debug.Log("<color=red>Sighttest result: " + sightTestResults[i].collider.gameObject + "</color>");
            if (sightTestResults[i].collider.gameObject == target)
            {
                return count;
            }
            else
            {
                count++;
            }
        }
        return count;
    }

    void FixedUpdate()
    {
        int startMultiplier = 0;
        if (m_listener != null)
        {
            int silenceMultiplier = RayCastTest2D(m_listener.gameObject);
            if (silenceMultiplier != startMultiplier)
            {
                foreach (AudioSource source in m_source.AudioSources)
                {
                    source.volume = Mathf.Lerp(source.volume, 1f - (m_volumeDecreasePerObject * silenceMultiplier), Time.deltaTime * 5f);
                };
                startMultiplier = silenceMultiplier;
            }
        }
    }
}
