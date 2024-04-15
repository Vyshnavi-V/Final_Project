using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using Unity.VisualScripting;
/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
/// 
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : Singleton<PlaceOnPlane>
{
    public Transform productAnchor = null;
    public bool placed;

    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    private ARRaycastManager m_SessionOrigin;

    private void OnEnable()
    {
        this.GetComponent<ARPointCloudManager>().enabled = true;
        this.GetComponent<ARPlaneManager>().enabled = true;
    }

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARRaycastManager>();
    }

    void Update()
    {

        // Update plane detection state

        if (Input.touchCount == 0 || placed == true )
        {
            return;
        }
        else
        {
            var touch = Input.GetTouch(0);
            if (m_SessionOrigin.Raycast(touch.position, s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;

                productAnchor.position = hitPose.position;
                if (productAnchor.GetComponentInChildren<ARTapToRotate>() != null)
                {
                    productAnchor.GetComponentInChildren<ARTapToRotate>().enabled = true;
                    productAnchor.GetComponentInChildren<ARTapToRotate>().InitTransform();
                }
                productAnchor.gameObject.SetActive(true);
                placed = true;
                this.GetComponent<ARPointCloudManager>().enabled = false;
                this.GetComponent<ARPlaneManager>().enabled = false;
                DisablePlanes();
            }
        }
    }

    

    public void DisablePlanes()
    {
        var planeManager = this.GetComponent<ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        var pointCloud = this.GetComponent<ARPointCloudManager>();
        foreach (var point in pointCloud.trackables)
        {
            point.gameObject.SetActive(false);
        }
    }

    public void EnablePlanes()
    {
        var planeManager = this.GetComponent<ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        var pointCloud = this.GetComponent<ARPointCloudManager>();
        foreach (var point in pointCloud.trackables)
        {
            point.gameObject.SetActive(false);
        }
    }

    public void Manager(bool val)
    {
        this.GetComponent<ARPointCloudManager>().enabled = val;
        this.GetComponent<ARPlaneManager>().enabled = val;
    }
}
