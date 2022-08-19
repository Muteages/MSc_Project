using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerMovement : MonoBehaviour
{

    public float defaultSpeed = 0;
    [SerializeField, Range(5f, 100f)]
    public float maxSpeed = 50f;
    public float alphaFactor = 0.001f;
    public bool HMDBased = false;

    public bool activate = true;
    public Foveate cameraL;
    public Foveate cameraR;
    public Transform cameraTransform;

    [Header("Hack")]
    public bool isAutoMove = false;

    private float currentSpeed = 0f;
    private float acceleration = 0f;
    
    private bool isCollided = false;
    private InputDevice leftDevice;
    private InputDevice helmet;

    private CharacterController cc;


    void GetController()
    {
        List<InputDevice> devices = new List<InputDevice>();

        //get helmet and controller
        InputDeviceCharacteristics inputDevices = (InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller) & InputDeviceCharacteristics.HeadMounted;
        InputDevices.GetDevicesWithCharacteristics(inputDevices, devices);

        if (devices.Count > 1)
        {
            //Debug.Log("count: " + devices.Count);
            helmet = devices[0];
            leftDevice = devices[1];
        }
    } 

    // Start is called before the first frame update
    void Start()
    {
        GetController();
        cc = GetComponent<CharacterController>();
        currentSpeed = defaultSpeed;
        UpdateFoveationAlpha();
    }

    private void UpdateFoveationAlpha()
    {
        if (cameraL.foveationAlpha < 0.08f)
        {
            cameraL.foveationAlpha = currentSpeed * alphaFactor;
            cameraR.foveationAlpha = currentSpeed * alphaFactor;

            if (cameraL.noFoveation)
            {
                cameraL.foveationAlpha = 0f;
                cameraR.foveationAlpha = 0f;
            }
        }

        //Debug.Log("current alpha:" + cameraL.foveationAlpha);
    }

    private void UpdateDarkScale()
    {
        if (currentSpeed > defaultSpeed)
        {
            cameraL.darkScale = (1 -  currentSpeed / maxSpeed) * maxSpeed + (currentSpeed / maxSpeed) * 20;
            cameraR.darkScale = (1 -  currentSpeed / maxSpeed) * maxSpeed + (currentSpeed / maxSpeed) * 20;
        }

        //if (currentSpeed < defaultSpeed)
        //{
        //    cameraL.darkScale = maxSpeed;
        //    cameraR.darkScale = maxSpeed;
        //}
    }

    private void UpdateSpeed()
    {
        // speed state
       currentSpeed += acceleration * Time.deltaTime;
       //Debug.Log("current speed: " + currentSpeed);

        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        if (currentSpeed < defaultSpeed && !isCollided)
        {
            currentSpeed = defaultSpeed;
        }

        if (isCollided)
        {
            currentSpeed = 0f;
            isCollided = false;
        }
    }

    private void ResetValues()
    {

        cameraL.darkScale = maxSpeed;
        cameraR.darkScale = maxSpeed;

        currentSpeed = defaultSpeed;

        cameraL.foveationAlpha = 0.0f;
        cameraR.foveationAlpha = 0.0f;


    }
 
    // Update is called once per frame
    void FixedUpdate()
    {
        if (activate)
        {
            if (!leftDevice.isValid || !helmet.isValid)
            {
                GetController();
            }

            leftDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerVal);
            leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isClicked);

            if (isClicked)
            {
                isAutoMove = !isAutoMove;
            }

            UpdateFoveationAlpha();

            if (isAutoMove)
            {
                // Avoid flying to the sky
                Vector3 transformXZ = cameraTransform.forward;
                transformXZ.y = 0f;

                // Switch orientation control between HMD and controller
                cc.Move(currentSpeed * Time.deltaTime * (HMDBased ? transformXZ : -transform.forward));

                if (triggerVal > 0.1f)
                {
                    acceleration = 2f;
                }
                else
                {
                    acceleration = -3f;
                }

                UpdateSpeed();        
                UpdateDarkScale();
            }

            else
            {
                ResetValues();
            }

        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isAutoMove)
        {
            if(hit.collider.tag == "Wall")
            {
                //Debug.Log("hit from controller");
                isCollided = true;
            }
        }
    }
}
