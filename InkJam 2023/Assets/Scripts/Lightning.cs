using System.Collections;
using System.Collections.Generic;
using MiTale;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lightning : MonoBehaviour
{
    public List<Light2D> m_lights;
    public float m_maxIntensity = 7.5f;
    public Vector2 m_minMaxTimeBetweenStrikes = new Vector2(5f, 15f);
    public Vector2 m_minMaxZRotation = new Vector2(-10f, 10f);

    private float m_timeSinceLastStrike = 0f;
    // Start is called before the first frame update
    void Start()
    {
        SetLightIntensities(0f);
    }

    void SetLightIntensities(float intensity)
    {
        foreach (Light2D light in m_lights)
        {
            light.intensity = intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceLastStrike += Time.deltaTime;
        if (m_timeSinceLastStrike > Random.Range(m_minMaxTimeBetweenStrikes[0], m_minMaxTimeBetweenStrikes[1]))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(m_minMaxZRotation[0], m_minMaxZRotation[1]));
            SetLightIntensities(m_maxIntensity);
            AudioManager.Instance.PlaySFX(SFXType.SFX_LIGHTNING);
            m_timeSinceLastStrike = -0.5f;
        }
        else if (m_timeSinceLastStrike > 0f)
        {
            SetLightIntensities(0f);
        }
    }
}
