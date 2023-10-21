using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiTale {

    [RequireComponent (typeof (CustomAudioSource))]
    public class ButtonAudio : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

        [NaughtyAttributes.InfoBox ("Remember to assign the SFX Mixer Group to the spawned Audio Source!", NaughtyAttributes.EInfoBoxType.Normal)]
        [NaughtyAttributes.Required]
        public AudioClipData m_audioClipData;
        private bool isFocused;

        private CustomAudioSource m_audioSource;

        void Awake () {
            m_audioSource = GetComponent<CustomAudioSource> ();
            m_audioSource.m_audioClips = m_audioClipData;
        }
        public void OnPointerClick (PointerEventData eventData) { // REQUIRES EVENT SYSTEM AND PHYSICS RAYCASTER ON CAMERA!
            if (m_audioSource.m_randomDefaultSFXType != SFXType.NONE) {
                m_audioSource.PlayDefaultRandomSFXType ();
            } else {
                m_audioSource.PlayOneShot (SFXType.UI_CLICK);
            }
        }

        public void OnPointerEnter (PointerEventData coll) {
            m_audioSource.PlayOneShot (SFXType.UI_HOVERON);
        }
        public void OnPointerExit (PointerEventData coll) {
            m_audioSource.PlayOneShot (SFXType.UI_HOVEROFF);
        }
        public void OnPointerDown (PointerEventData coll) {
            m_audioSource.PlayOneShot (SFXType.UI_POINTER_DOWN);
        }
        public void OnPointerUp (PointerEventData coll) {
            m_audioSource.PlayOneShot (SFXType.UI_POINTER_UP);
        }
        public void OnBeginDrag (PointerEventData eventData) {
            m_audioSource.PlayOneShot (SFXType.UI_DRAGSTART);
        }

        // Activate this if we need it for some reason
        public void OnDrag (PointerEventData data) {
            /*  if (m_audioSource.AudioSource.loop == false) {
                  m_audioSource.PlayAndLoop (SFXType.UI_DRAGGING);
              }
              */
        }

        public void OnEndDrag (PointerEventData eventData) {
            /* if (m_audioSource.AudioSource.loop) {
                 m_audioSource.StopLoop ();
             } */
            m_audioSource.PlayOneShot (SFXType.UI_DRAGCOMPLETE);
        }

        public void OnFocusOn () {
            m_audioSource.PlayOneShot (SFXType.UI_HOVERON);
        }
        public void OnFocusOff () {
            m_audioSource.PlayOneShot (SFXType.UI_HOVEROFF);
        }

        void Update () {
            if (enabled) {
                if (EventSystem.current.currentSelectedGameObject == gameObject && !isFocused) {
                    isFocused = true;
                    OnFocusOn ();
                }
                if (isFocused && EventSystem.current.currentSelectedGameObject != gameObject) {
                    isFocused = false;
                    OnFocusOff ();
                }
            }
        }
    }
}