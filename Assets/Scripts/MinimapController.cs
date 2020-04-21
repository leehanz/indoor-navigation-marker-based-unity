using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class MinimapController : MonoBehaviour
{
    public Transform targetToFollow;    // The transform of the gameobject that gets followed
    public Quaternion targetRot;        // The rotation of the device camera from Frame.Pose.rotation    
    [SerializeField] RawImage minimap;            // The rawimage the view of the camera gets rendered to
    [SerializeField] Camera mapCamera;     // The camera that captures the map
    [SerializeField] Camera arCamera;     // The camera that captures the map
    [SerializeField] GameObject arrow;            // The direction indicator on the player indicator
    [SerializeField] GameObject switchButton;     // Button that switches the views
    float rotationSmoothingSpeed = 2f; // rotation speed, change to personal preference

    bool map = false;           // boolean to tell if map is showing (phone position)
    bool pressed = false;       // boolean to tell if map is showing (button press)
    RenderTexture texture;      // field to save texture to set again after view switch

    // Use lateUpdate to assure that the camera is updated after the target has been updated.
    void LateUpdate()
    {
        if (!targetToFollow)
            return;
        //receive rotation from camera
        Vector3 targetEulerAngles = targetRot.eulerAngles;

        //switch view when phone is turned horizontal
        if(targetRot.x > 0.5)
        {
            if(!map)
            {
                //show mapview 
                map = true;
                arCamera.enabled = false;
                minimap.gameObject.SetActive(false);
                texture = mapCamera.targetTexture;
                mapCamera.targetTexture = null;
                mapCamera.orthographicSize = 15;
                switchButton.SetActive(false);
            }
        } else
        {
            if(map)
            {
                //show cameraview
                map = false;
                arCamera.enabled = true;
                minimap.gameObject.SetActive(true);
                mapCamera.targetTexture = texture;
                mapCamera.orthographicSize = 7;
                switchButton.SetActive(true);
            }
        }
        // Calculate the current rotation angle around the Y axis we want to apply to the camera.
        // We add 180 degrees as the device camera points to the negative Z direction
        float rotationToApplyAroundY = targetEulerAngles.y; //+ 180;
        //Debug.Log(fullscreenCamera.gameObject.transform.localRotation.eulerAngles);
        //Debug.Log("old:" + rotationToApplyAroundY);
        // Smooth interpolation between current camera rotation angle and the rotation angle we want to apply.
        // Use LerpAngle to handle correctly when angles > 360
        float newCamRotAngleY = Mathf.LerpAngle(arrow.transform.eulerAngles.y, rotationToApplyAroundY, rotationSmoothingSpeed * Time.deltaTime);
        Quaternion newCamRotYQuat = Quaternion.Euler(0, newCamRotAngleY, 0);
        //extra check to make sure that the rotation of the arrow does not change when accessing mapview from placing phone horizontal
        if(targetEulerAngles.x < 65)
        {
            arrow.transform.rotation = newCamRotYQuat;
        }
    }

    //logic when switch button is pressed
    public void Switch()
    {
        if (!pressed)
        {
            //show mapview
            pressed = true;
            arCamera.enabled = false;
            minimap.gameObject.SetActive(false);
            texture = mapCamera.targetTexture;
            mapCamera.targetTexture = null;
            mapCamera.orthographicSize = 15;
        }
        else
        {
            //show cameraview
            pressed = false;
            arCamera.enabled = true;
            minimap.gameObject.SetActive(true);
            mapCamera.targetTexture = texture;
            mapCamera.orthographicSize = 7;
        }
    }
}
