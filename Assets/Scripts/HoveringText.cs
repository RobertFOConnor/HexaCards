using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringText : MonoBehaviour {

    public Transform target;
    public Vector3 offset = Vector3.up;
    public bool clamptoScreen = false;
    public float clampBorderSize = .05f;
    public bool useMainCamera = true;
    Camera cameraToUse;
    private Camera cam;
    private Transform thisTransform;
    private Transform camTransform;

	// Use this for initialization
	void Start () {
        thisTransform = transform;
        if (useMainCamera) {
            cam = Camera.main;
        } else
        {
            cam = cameraToUse;
        }
        camTransform = cam.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (clamptoScreen) {
            Vector3 relativePosition = camTransform.InverseTransformPoint(target.position);
            relativePosition.z = Mathf.Max(relativePosition.z, 1.0f);
            thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
            thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
                                             Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
                                             thisTransform.position.z);
        }
        else
        {
            thisTransform.position = cam.WorldToViewportPoint(target.position + offset);
        }
    }
	
}
