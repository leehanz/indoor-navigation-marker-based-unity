//using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

//used to update AR stuff using colliders
public class NavigatorSpawner : MonoBehaviour
{
    string NAVIGATOR_TAG = "nav";
    string CALIBRATION_TAG = "calibration";
    string DESTINATION_TAG = "destination";
    
    [SerializeField] ARReferencePointManager mAnchorManager;
    ARReferencePoint currentAnchor; //spawned anchor when putting somthing AR on screen
    [SerializeField] GameObject navModelPrefab;
    [SerializeField] GameObject navTriggerPrefab;
    GameObject navModel;
    GameObject navTrigger;

    [SerializeField] PathDrawer pathDrawer;
    bool isPathEnough { get { return pathDrawer.line.positionCount>0; } }

    [SerializeField] Transform mDevice; // ar camera
    [SerializeField] Text debugText;

    private void Start()
    {
        ToggleTransform(navModel = Instantiate(navModelPrefab), false);
        ToggleTransform(navTrigger = Instantiate(navTriggerPrefab), false);
        //hasEntered = false;
        //hasExited = false;
    }

    //what to do when entering a collider
    private void OnTriggerEnter(Collider other)
    {
        // if it is a destination spawn a pin and delete current arrow + current path + delete current destination
        if (other.CompareTag(DESTINATION_TAG) && pathDrawer.currDestination.name.Equals(other.name))
        {
            Debug.Log("arrived");
            ShowDestination();
        }
        //if it is a navTrigger then caculate angle and spawn a new AR arrow
        else if (other.CompareTag(NAVIGATOR_TAG) && isPathEnough)
        {
            Debug.Log("enter");
            ShowNextNavigator();
            //debugText.text = "follow added anchor..."+cnt;
        }
        // show calibration text nearby calibration points
        else if (other.tag.Equals(CALIBRATION_TAG))
        {
            debugText.text = "choose the destination.";
        }
    }

    //what to do when exiting a collider
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        //if it is a navTrigger then delete Anchor and arrow and create a new trigger
        if (other.CompareTag(NAVIGATOR_TAG))
        {
            SetTrigger();
        }
        // if destination than delete anchor
        else if (other.CompareTag(DESTINATION_TAG))
        {
            SetTrigger();
        }
    }

    public void ShowNextNavigator()
    {
        // position arrow a bit before the camera and a bit lower
        Vector3 pos = mDevice.position + mDevice.forward * .5f + mDevice.up * -.15f;
        // rotation
        var lookPos = pathDrawer.line.GetPosition(1) - this.transform.position;
        lookPos.y = 0;
        var rot = Quaternion.LookRotation(lookPos);

        UpdateAnchor(pos, rot, () => {
            debugText.text = "follow the arrow...";
        });
    }

    void ShowDestination()
    {
        var lookPos = this.transform.position - pathDrawer.currDestination.transform.position;
        lookPos.y = 0;
        var rot = Quaternion.LookRotation(lookPos);

        UpdateAnchor(pathDrawer.transform.position, rot, () => {
            var dname = pathDrawer.currDestination.name;
            debugText.text = "You have arrived at " + dname +" !";
            pathDrawer.Clear();
            ToggleTransform(navModel, false);
            ToggleTransform(navTrigger, false);
        });
        
    }

    public void UpdateAnchor(Vector3 pos, Quaternion rot, Action callback = null)
    {
        ClearAnchor();

        currentAnchor = mAnchorManager.AddReferencePoint(new Pose(pos, rot));
        if (currentAnchor != null)
        {
            navModel.transform.SetParent(currentAnchor.transform, false);
        }

        SetTransform(navModel, pos, rot);
        ToggleTransform(navModel, true);
        callback?.Invoke();
    }

    public void ClearAnchor()
    {
        navModel.transform.SetParent(null);
        ToggleTransform(navModel, false);

        if (currentAnchor!= null && mAnchorManager.RemoveReferencePoint(currentAnchor))
        {
            Destroy(currentAnchor.gameObject);
        }
    }

    public void SetTrigger()
    {
        ToggleTransform(navTrigger, false);
        SetTransform(navTrigger, this.transform.position, this.transform.rotation);
        ToggleTransform(navTrigger, true);
    }

    public void InitializePos(Vector3 pos)
    {
        this.transform.position = pos;
    }

    private void ToggleTransform(GameObject obj, bool show)
    {
        obj.SetActive(show);
    }

    private void SetTransform(GameObject obj, Vector3 pos, Quaternion rot)
    {
        obj.transform.position = pos;
        obj.transform.rotation = rot;
    }
}
