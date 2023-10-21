using System.Collections;
using System.Collections.Generic;
using MiTale;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool m_enabled = true;
    public Animator m_playerAnimator;
    public Rigidbody2D m_playerRB;
    public CustomAudioSource m_audioSource;

    public float m_speed;

    public bool EnableMovement
    {
        get
        {
            return m_enabled;
        }
        set
        {
            m_enabled = value;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (EnableMovement)
        {
            Vector2 translate = Vector2.zero;
            if (Input.GetAxis("Vertical") != 0f)
            {
                m_playerAnimator.SetFloat("speed", 1f);
            }
            else
            {
                m_playerAnimator.SetFloat("speed", 0f);
            }
            if (Input.GetAxis("Horizontal") != 0f)
            {
                m_playerAnimator.SetFloat("speed", 1f);
            }
            else
            {
                m_playerAnimator.SetFloat("speed", 0f);
            }
            translate.y = Input.GetAxis("Vertical");
            translate.x = Input.GetAxis("Horizontal");
            if (translate.x != 0 && translate.y != 0) // Check for diagonal movement
            {
                // limit movement speed diagonally, so you move at 70% speed
                translate.x *= 0.7f;
                translate.y *= 0.7f;
            }

            m_playerRB.velocity = translate * m_speed;
            Vector3 mousePos = Input.mousePosition;
            Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
            float playerRotationAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, playerRotationAngle));
        }
        else
        {
            m_playerAnimator.SetFloat("speed", 0f);
            m_playerRB.velocity = Vector3.zero;
        }
    }

    public void PlayFootStepSound()
    {
        m_audioSource.PlayOneShot(SFXType.SFX_FOOTSTEP);
    }
}
