using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BaitEatenEvent : UnityEvent<Bait> { }
public class Bait : MonoBehaviour
{
    public float m_baitEatTimeLeft = 5f;
    public GameObject m_sackObject;
    public GameObject m_bloodObject;
    public GameObject m_vfxObject;

    public GenericActivate m_pickupScript;
    public GenericTrigger m_pickupTrigger;
    public bool m_isCarried = false;
    public bool m_isBeingEaten = false;

    public BaitEatenEvent m_onBaitEaten;
    private bool m_wasEaten = false;

    private float m_coolDownTime = 1f;
    private GameObject m_attachTargetCurrent;

    void Start()
    {
        m_vfxObject.SetActive(false);
        m_sackObject.SetActive(true);
    }

    public void EatBait(ZombieMovement eatingZombie) // through messaging system like a goddamned savage
    {
        m_vfxObject.SetActive(true);
        m_isBeingEaten = true;
        m_baitEatTimeLeft -= Time.deltaTime;
        m_pickupScript.enabled = false;
        m_pickupScript.MakeInvisible();
        if (m_isCarried)
        {
            DropBait(null, null);
        }
        if (m_baitEatTimeLeft <= 0f)
        {
            if (!m_wasEaten)
            {
                m_sackObject.SetActive(false);
                m_vfxObject.SetActive(false);
                m_onBaitEaten.Invoke(this);
                m_wasEaten = true;
            }
        }
    }
    public void PickupBait(GameObject attachTarget, GenericActivate activateable)
    {
        if (m_baitEatTimeLeft > 0f && !m_isBeingEaten && !m_isCarried && m_coolDownTime > 0f)
        {
            PlayerMovement pm = attachTarget.GetComponent<PlayerMovement>();
            if (pm == null) { return; }
            pm.m_playerAnimator.SetBool("carrying", true);
            pm.m_speedMultiplier = 0.5f;
            m_sackObject.GetComponent<Collider2D>().enabled = false;
            GameObject m_bloodCopy = Instantiate(m_bloodObject);
            m_bloodCopy.transform.SetParent(null, true);
            transform.SetParent(pm.m_baitAttachPoint, true);
            m_bloodCopy.GetComponent<SpriteRenderer>().sprite = m_bloodObject.GetComponent<SpriteRenderer>().sprite;
            Destroy(m_bloodCopy, 120f);
            m_bloodObject.SetActive(false);
            m_coolDownTime = 0.5f;
            m_isCarried = true;
            m_pickupScript.MakeVisible(true, attachTarget);
            m_attachTargetCurrent = attachTarget;
        }
    }
    public void DropBait(GameObject attachTarget, GenericActivate activatable)
    {
        transform.SetParent(null, true);
        transform.rotation = Quaternion.identity;
        m_bloodObject.SetActive(true);
        m_coolDownTime = -0.5f;
        AudioManager.Instance.PlaySFX(SFXType.SFX_DROPBAIT);
        PlayerMovement pm = attachTarget.GetComponent<PlayerMovement>();
        if (pm == null) { return; }
        pm.m_playerAnimator.SetBool("carrying", false);
        pm.m_speedMultiplier = 1f;
        m_sackObject.GetComponent<Collider2D>().enabled = true;
        m_isCarried = false;
    }
    void FixedUpdate()
    {
        if (m_isCarried)
        {
            m_coolDownTime -= Time.deltaTime;
            if (Input.GetKeyDown(m_pickupScript.inputButton) && m_coolDownTime < 0f)
            {
                DropBait(m_attachTargetCurrent, null);
            }
            if (transform.localPosition != Vector3.zero && transform.parent != null)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
            }
        }
        else
        {
            m_coolDownTime += Time.deltaTime;
        }
    }
}
