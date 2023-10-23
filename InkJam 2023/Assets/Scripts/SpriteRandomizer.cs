using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomizer : MonoBehaviour
{
    public List<Sprite> m_randomSprites;
    public SpriteRenderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        if (m_renderer == null)
        {
            m_renderer = GetComponent<SpriteRenderer>();
        }
        RandomizeSprite();
    }

    [NaughtyAttributes.Button]
    public void RandomizeSprite()
    {
        if (m_randomSprites.Count > 0)
        {
            m_renderer.sprite = m_randomSprites[Random.Range(0, m_randomSprites.Count)];
        }
        if (m_renderer.sprite == null)
        {
            m_renderer.enabled = false;
        }
    }
}
