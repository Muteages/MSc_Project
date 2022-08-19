using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class KeyboardMovement : MonoBehaviour
{
    public float mouseSpeed = 2f;
    public float defaultSpeed = 5f;
    [SerializeField, Range(5f, 100f)]
    public float maxSpeed = 30f;
    public float alphaFactor = 0.002f;

    public bool activate = false;

    public Foveate cameraL;
    public Foveate cameraR;

    //private Rigidbody rb;
    private float currentSpeed = 0f;
    private float acceleration = 0f;
    private bool isAutoMove = false;


    private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentSpeed = defaultSpeed;
        updateFoveationAlpha();
    }

    private void updateFoveationAlpha()
    {
        cameraL.foveationAlpha = currentSpeed * alphaFactor;
        cameraR.foveationAlpha = currentSpeed * alphaFactor;
        //Debug.Log("current alpha:" + cameraL.foveationAlpha);
    }

    private void updateSpeed()
    {
        // speed state
        currentSpeed += acceleration * Time.deltaTime;
        //Debug.Log("current speed: " + currentSpeed);

        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        if (currentSpeed < defaultSpeed)
        {
            currentSpeed = defaultSpeed;
        }
    }

    //update acceleration by pressing shift/control
    private void updateAcceleration()
    {
        // change acceleration
        if (Input.GetKey(KeyCode.LeftShift))
        {
            acceleration = 1f;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            acceleration = -1f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            acceleration = 0f;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (activate)
        {
            //mouse movement
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSpeed, 0);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isAutoMove = !isAutoMove;
            }

            updateSpeed();

            //auto-move

            if (isAutoMove)
            {
                cc.Move(currentSpeed * Time.deltaTime * -transform.forward);
            }
            // move by WASD
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    cc.Move(-transform.forward * currentSpeed * Time.deltaTime);
                }

                else if (Input.GetKey(KeyCode.S))
                {
                    cc.Move(transform.forward * currentSpeed * Time.deltaTime);
                }

                else if (Input.GetKey(KeyCode.A))
                {
                    cc.Move(transform.right * currentSpeed * Time.deltaTime);
                }

                else if (Input.GetKey(KeyCode.D))
                {
                    cc.Move(-transform.right * currentSpeed * Time.deltaTime);
                }

                else
                {
                    acceleration = 0f;
                    cc.Move(Vector3.zero);
                }              
            }

            updateAcceleration();
            updateFoveationAlpha();
        }

    }
}
