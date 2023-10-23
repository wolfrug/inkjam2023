using System.Collections;
using System.Collections.Generic;
using MiTale;
using Pathfinding;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public Rigidbody2D m_rb;
    public Animator m_animator;

    public AIPath m_aiScript;

    public CustomAudioSource m_audioSource;

    public GenericTrigger m_trigger;
    public float m_speed;
    public float m_turnspeed;

    public float m_runDistance = 5f;
    public float m_eatDistance = 1f;

    public Transform m_moveTarget;

    public LayerMask m_targetMasks;

    public Transform[] m_defaultTargets;

    public Vector2 m_randomMoanTime = new Vector2(10f, 15f);
    private float m_timeSinceLastMoan = 0f;

    private bool m_dead = false;

    // Start is called before the first frame update
    void Start()
    {
        m_aiScript.maxSpeed = m_speed;

    }

    public void TriggerEnter(GameObject target)
    {
        if (!m_dead)
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
            for (int i = 0; i < sightTestResults.Length; i++)
            {
                Debug.Log("<color=red>Sighttest result: " + sightTestResults[i].collider.gameObject + "</color>");
                if (sightTestResults[i].collider.gameObject == target && m_moveTarget.tag != "Bait")
                {
                    if (m_moveTarget != target.transform && target.tag == "Player")
                    {
                        PlayEatSound();
                    }
                    m_moveTarget = target.transform;
                    break;
                }
                if (sightTestResults[i].collider.tag == "LOSBlocker")
                {
                    break;
                }
            }
        }
    }
    public void TriggerExit(GameObject target)
    {
        if (!m_dead)
        {
            if (m_moveTarget == target.transform)
            {
                if (m_trigger.contents.Count > 0)
                {
                    m_moveTarget = m_trigger.contents[Random.Range(0, m_trigger.contents.Count)].transform;
                }
                else
                {
                    m_moveTarget = m_defaultTargets[Random.Range(0, m_defaultTargets.Length)];
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_dead)
        {
            if (!GameManager.narrativeOn)
            {
                if (m_moveTarget != null)
                {
                    m_aiScript.destination = m_moveTarget.transform.position;
                    Vector3 targetPos = m_moveTarget.position;
                    targetPos.x = targetPos.x - transform.position.x;
                    targetPos.y = targetPos.y - transform.position.y;
                    float rotationAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

                    //transform.rotation = Quaternion.Euler(transform.right);

                    if (Vector3.Distance(m_moveTarget.position, transform.position) < m_runDistance)
                    {
                        if (m_moveTarget.tag == "DefaultTarget")
                        {
                            TriggerExit(m_moveTarget.gameObject);
                        }
                        m_aiScript.maxSpeed = m_speed * 2f;
                        m_animator.SetBool("isRunning", true);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(rotationAngle, Vector3.forward), m_turnspeed * Time.deltaTime);
                    }
                    else
                    {
                        m_aiScript.maxSpeed = m_speed;
                        m_animator.SetBool("isRunning", false);
                    }
                    m_animator.SetFloat("speed", m_aiScript.desiredVelocity.magnitude);
                    if (Vector3.Distance(m_moveTarget.position, transform.position) < m_eatDistance && (m_moveTarget.tag == "Bait" || m_moveTarget.tag == "Player"))
                    {
                        m_animator.SetBool("isEating", true);
                        //m_rb.velocity = Vector3.zero;
                        if (m_moveTarget.tag == "Bait")
                        {
                            m_moveTarget.parent.GetComponent<Bait>().EatBait(this);
                        }
                        else
                        {
                            GameManager.instance.CaughtByZombie();
                        }
                    }
                    else
                    {
                        m_animator.SetBool("isEating", false);
                    }
                }
            }
        }

    }
    void Update()
    {
        if (!m_dead)
        {
            if (!GameManager.narrativeOn)
            {
                m_timeSinceLastMoan += Time.deltaTime;
                if (m_timeSinceLastMoan > Random.Range(m_randomMoanTime.x, m_randomMoanTime.y))
                {
                    m_audioSource.PlayOneShot(SFXType.SFX_ZOMBIE_MOAN);
                    m_timeSinceLastMoan = 0f;
                    if (m_moveTarget.tag == "DefaultTarget")
                    {
                        TriggerExit(m_moveTarget.gameObject);
                    }
                }
            }
        }
    }
    public void Kill()
    {
        m_animator.SetBool("isDead", true);
        m_dead = true;
        m_aiScript.canMove = false;
        m_audioSource.PlayOneShot(SFXType.SFX_ZOMBIE_MOAN);
    }
    public void PlayFootStepSound()
    {
        m_audioSource.PlayOneShot(SFXType.SFX_ZOMBIE_FOOTSTEP);
    }
    public void PlayEatSound()
    {
        m_audioSource.PlayOneShot(SFXType.SFX_ZOMBIE_GROWL);
    }

}
