using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AnimationFoveation : MonoBehaviour
{

    public Foveate cameraL;
    public Foveate cameraR;

    private float acceleration = 0f;
    private float maxSpeed = 1.8f;

    private InputDevice leftDevice;
    private InputDevice helmet;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
       GetController();
       InitFoveationAlpha();
       InitDarkScale();
       animator = gameObject.GetComponent<Animator>();
       //animator.recorderMode = AnimatorRecorderMode;
       animator.speed = 0f;
    }

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

    void InitFoveationAlpha()
    {
        cameraL.foveationAlpha = 0f;
        cameraR.foveationAlpha = 0f;
    }

    void UpdateFoveationAlpha()
    {

        cameraL.foveationAlpha = animator.speed / maxSpeed * 0.08f;
        cameraR.foveationAlpha = animator.speed / maxSpeed * 0.08f;
        //Debug.Log("foveationAlpha: " + cameraL.foveationAlpha);
        if (cameraL.noFoveation)
        {
            cameraL.foveationAlpha = 0f;
            cameraR.foveationAlpha = 0f;
        }
    }


    void InitDarkScale()
    {
        cameraL.darkScale = 10.0f;
        cameraR.darkScale = 10.0f;

        cameraL.transitionSize = 5.0f;
        cameraR.transitionSize = 5.0f;
    }

    void UpdateAniSpeed()
    {
        animator.speed += acceleration * Time.deltaTime;
        animator.speed = (animator.speed >= 0f ? animator.speed : 0f);
        animator.speed = (animator.speed < maxSpeed ? animator.speed : maxSpeed);
        //Debug.Log("current speed: " + animator.speed);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //UpdateFoveationAlpha();
        if (!leftDevice.isValid)
        {
            GetController();
        }

        leftDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerVal);

        if (triggerVal > 0.1)
        {
            acceleration = .2f;
        }

        else
        {
            acceleration = -.3f;
        }

        UpdateAniSpeed();
        UpdateFoveationAlpha();
    }
}
