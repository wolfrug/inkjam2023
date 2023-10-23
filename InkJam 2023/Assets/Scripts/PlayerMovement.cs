using System.Collections;
using System.Collections.Generic;
using MiTale;
using Unity.VisualScripting;
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
            return m_enabled && !GameManager.narrativeOn;
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
            m_playerRB.simulated = true;
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
            m_playerRB.simulated = false;
        }
    }

    public void PlayFootStepSound()
    {
        m_audioSource.PlayOneShot(SFXType.SFX_FOOTSTEP);
    }
    public void TurnTowardsObjectAndInteract(GameObject target)
    {
        StartCoroutine(Turner(target));
    }
    IEnumerator Turner(GameObject target)
    {
        Vector3 targetPos = target.transform.position;
        targetPos.x = targetPos.x - transform.position.x;
        targetPos.y = targetPos.y - transform.position.y;
        float rotationAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        while (true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotationAngle, Vector3.forward), 5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if (Quaternion.Angle(transform.rotation, Quaternion.AngleAxis(rotationAngle, Vector3.forward)) <= 0.01f)
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.1f);
        Interact();
    }
    public void Interact()
    {
        m_playerAnimator.SetTrigger("interact");
    }
}
