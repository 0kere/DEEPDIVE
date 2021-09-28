using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandle //Interface for an InputHandler to add custom functionality to mobile inputs
{
    void Tap(Vector2 touchPos);
    //void DoubleTap(Vector2 touchPosOne, Vector2 touchPosTwo); //Not used currently
    //void Hold(Vector2 touchPos, float duration); //Not used currently
    //void Drag(Vector2 startPos, Vector2 endPos, float dragTime); //Not used currently
    void Tilt(float rotValue);
    //void Move(Vector2 startPos, Vector2 curPos, Vector2 deltaPos, float duration); //Not used currently
}
[RequireComponent(typeof(InputHandler))]
public class InputManager : MonoBehaviour
{
    private float curTouchTime;//current time touch has been held down
    private Vector2 startTouch;
    private InputHandler inputhandler; //
    [Header("Tap Settings")]
    [SerializeField] private float maxDragPercent;
    [SerializeField] private float maxTapTime;
    private float maxDragDistance;
    private bool isMovedToFarToTap;

    [Header("Tilt Settings")]
    [SerializeField] private float tiltDeadZoneValue = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        maxDragDistance = Screen.width * maxDragPercent;
        inputhandler = GetComponent<InputHandler>();
        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("Supports gyro");
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.Log("Doesnt support gyro");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    InitTouch(touch);
                    break;
                case TouchPhase.Moved:
                    if (Vector2.Distance(startTouch, touch.position) > maxDragDistance)
                    {
                        isMovedToFarToTap = true;
                    }
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    EndTouch(touch);
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    break;
            }

            curTouchTime += Time.deltaTime;
        }
        if (SystemInfo.supportsGyroscope && GameManager.instance.tryGyro) 
        {
            HandleTiltGyro();
        }
        else
        {
            HandleTiltAccelerometer();
        }
    }

    private void HandleTiltGyro()
    {
        if (Input.gyro.gravity.x > tiltDeadZoneValue || Input.gyro.gravity.x < -tiltDeadZoneValue)
        {
            Debug.Log("Tilting gyro with value: " + Input.gyro.gravity.x);
            inputhandler.Tilt(Input.gyro.gravity.x);
        }
    }
    private void HandleTiltAccelerometer()
    {
        if (Input.acceleration.x > tiltDeadZoneValue || Input.acceleration.x < -tiltDeadZoneValue)
        {
            //Debug.Log("Tilting accel with value: " + Input.gyro.gravity.x);
            inputhandler.Tilt(Input.acceleration.x);
        }
    }

    private void InitTouch(Touch touch)
    {
        curTouchTime = 0f;
        startTouch = touch.position;
        isMovedToFarToTap = false;
    }

    private void EndTouch(Touch touch)
    {
        if (Vector2.Distance(startTouch, touch.position) > maxDragDistance)
        {
            isMovedToFarToTap = true;
        }
        if (!isMovedToFarToTap && curTouchTime <= maxTapTime)
        {
            inputhandler.Tap(touch.position);
        }
    }
}
