using System.Collections.Generic;
using UnityEngine;

public class ARTouchGrab : MonoBehaviour
{
    [Header("Grab settings")]
    public float holdDistance = 0.6f;            // distance in front of camera while held
    public float positionSmoothTime = 0.04f;     // smoothing while following
    public float throwMultiplier = 1.5f;         // scale applied to computed throw velocity
    public LayerMask grabbableLayer = ~0;        // optional: filter by layer

    private Camera arCamera;
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;

    // smoothing helpers
    private Vector3 currentVelocity;

    // velocity sampling for throw
    private readonly Queue<Vector3> velocitySamples = new Queue<Vector3>();
    private readonly int maxSamples = 5;
    private Vector3 lastHoldWorldPos;

    void Start()
    {
        arCamera = Camera.main;
        if (arCamera == null)
            Debug.LogWarning("ARTouchGrab: No Camera.main found. Make sure AR Camera is tagged MainCamera.");
    }

    void Update()
    {
        if (arCamera == null) return;

        if (Input.touchCount == 0)
        {
            if (grabbedObject != null)
                ReleaseGrab();
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            TryBeginGrab(touch);
        }
        else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && grabbedObject != null)
        {
            UpdateHeldObject();
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (grabbedObject != null)
                ReleaseGrab();
        }
    }

    private void TryBeginGrab(Touch touch)
    {
        Ray ray = arCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, grabbableLayer))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.CompareTag("Grabbable"))
            {
                BeginGrab(hitObj);
            }
        }
    }

    private void BeginGrab(GameObject obj)
    {
        grabbedObject = obj;
        grabbedRb = grabbedObject.GetComponent<Rigidbody>();

        if (grabbedRb != null)
        {
            grabbedRb.isKinematic = true;
            grabbedRb.linearVelocity = Vector3.zero;
            grabbedRb.angularVelocity = Vector3.zero;
        }

        Vector3 targetPos = arCamera.transform.position + arCamera.transform.forward * holdDistance;
        lastHoldWorldPos = targetPos;
        velocitySamples.Clear();
        currentVelocity = Vector3.zero;
    }

    private void UpdateHeldObject()
    {
        Vector3 targetPos = arCamera.transform.position + arCamera.transform.forward * holdDistance;

        // smooth move
        Vector3 newPos = Vector3.SmoothDamp(grabbedObject.transform.position, targetPos, ref currentVelocity, positionSmoothTime);
        grabbedObject.transform.position = newPos;

        // track movement for throw
        Vector3 sampleVel = (newPos - lastHoldWorldPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        velocitySamples.Enqueue(sampleVel);
        if (velocitySamples.Count > maxSamples) velocitySamples.Dequeue();
        lastHoldWorldPos = newPos;
    }

    private void ReleaseGrab()
    {
        Vector3 avgVel = Vector3.zero;
        foreach (var v in velocitySamples) avgVel += v;
        if (velocitySamples.Count > 0) avgVel /= velocitySamples.Count;

        if (grabbedRb != null)
        {
            grabbedRb.isKinematic = false;
            grabbedRb.linearVelocity = avgVel * throwMultiplier;
        }

        grabbedObject = null;
        grabbedRb = null;
        velocitySamples.Clear();
    }
}
