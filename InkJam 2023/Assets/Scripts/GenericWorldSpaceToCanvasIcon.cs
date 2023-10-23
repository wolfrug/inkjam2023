using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericWorldSpaceToCanvasIcon : MonoBehaviour {
    public GameObject Target {
        get {
            return m_target;
        }
        set {
            m_target = value;
            useRawPosition = false;
        }
    }
    public RectTransform canvasObject;
    public bool useRawPosition = false;

    [Tooltip ("Hide the object if the target is disabled")]
    public bool hideIfTargetIsDisabled = true;

    [Tooltip ("Don't allow the canvas icon to leave the viewspace.")]
    public bool restrictToViewScreen = false;

    [Tooltip ("Only show the canvas icon when it would otherwise be outside the viewspace")]
    public bool onlyShowOutsideViewScreen = false;
    public Vector3 TargetPosition {
        get {
            if (useRawPosition) {
                return m_targetPosition;
            }

            try {
                return Target.transform.position;
            } catch (NullReferenceException e) {
                return Vector3.zero;
            }
        }
        set {
            m_targetPosition = value;
            useRawPosition = true;
        }
    }

    public bool scaleByDistanceFromCamera = true;
    [SerializeField]
    private float distanceForScaleOne = 200f;

    [SerializeField]
    private Camera targetCamera;
    private Vector3 pos;
    private Vector3 m_targetPosition;
    private GameObject m_target;

    void Awake () {
        if (!useRawPosition && Target == null) {
            Target = gameObject;
        }
        if (targetCamera == null) {
            targetCamera = Camera.main;
        };
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (canvasObject == null) {
            return;
        }

        if (!useRawPosition && Target == null) {
            canvasObject.gameObject.SetActive (false);
        }

        if (!Target.activeInHierarchy && hideIfTargetIsDisabled) {
            canvasObject.gameObject.SetActive (false);
        } else {
            canvasObject.gameObject.SetActive (true);
        }

        pos = RectTransformUtility.WorldToScreenPoint (targetCamera, TargetPosition);

        if (restrictToViewScreen) {
            bool isOutside = false;
            float rectWidth = canvasObject.rect.width / 2f;
            float rectHeight = canvasObject.rect.height / 2f;
            if (pos.x + rectWidth > Screen.width) {
                pos.x = Screen.width - rectWidth;
                isOutside = true;
            }
            if (pos.x - rectWidth < 0f) {
                pos.x = rectWidth;
                isOutside = true;
            }
            if (pos.y + rectHeight > Screen.height) {
                pos.y = Screen.height - rectHeight;
                isOutside = true;
            }
            if (pos.y - rectHeight < 0f) {
                pos.y = rectHeight;
                isOutside = true;
            }
            if (onlyShowOutsideViewScreen && canvasObject.gameObject.activeInHierarchy) { // show it if it's outside, hide it otherwise (unless already hidden)
                canvasObject.gameObject.SetActive (isOutside);
            }
        };
        canvasObject.position = pos;
        if (scaleByDistanceFromCamera) {
            float distance = Vector3.Distance (targetCamera.transform.position, TargetPosition);
            float newscale = Mathf.Clamp (distanceForScaleOne / distance, 0.2f, 1f);
            // Debug.Log ("Distance between camera and " + gameObject.name + " :" + distance.ToString ());
            canvasObject.localScale = new Vector3 (newscale, newscale, newscale);
        }

    }

    void OnDisable () {
        if (hideIfTargetIsDisabled && Target == gameObject) {
            canvasObject?.gameObject.SetActive (false);
        }
    }
}