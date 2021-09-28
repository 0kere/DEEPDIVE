using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class Bounds
    {
        public float min;
        public float max;
    }
    private sColorControl colorControlRef;
    bool isMoving;
    bool isLeftDir;
    float rotLerp;
    private Quaternion originalRotation;
    [SerializeField] private float rotSpeed;
    [SerializeField] private float rotResetMod;
    [SerializeField] private float limitRot;
    Bounds rotationBounds = new Bounds();
    [SerializeField] private float minDistToRotate;

    [SerializeField] private float speed;
    private float hVelocity;
    [SerializeField] private float hBounds;

    [SerializeField] private bool collisionOnEnter; //Im not sure if it plays better with the players collision with obstacles being check in stay or enter.
    //stay means if they switch colour while still within an obstacle the game ends
    //enter means they have to hit an active obstacle and can be inside something when switching

    bool died;

    void Start()
    {
        hBounds = Camera.main.orthographicSize * Screen.width / Screen.height - transform.localScale.x;
        originalRotation = transform.rotation;

        colorControlRef = gameObject.GetComponent<sColorControl>();

        rotationBounds.min = transform.rotation.eulerAngles.z - limitRot;
        rotationBounds.max = transform.rotation.eulerAngles.z + limitRot;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            OnMove(-0.3f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnMove(0.3f);
        }
        if (!isMoving && !died)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotSpeed * rotResetMod);
        }
        isMoving = false;
    }

    public void OnMove(float moveValue)
    {
        if (died) { return; }
        isMoving = true;

        float movementDir = 0; //can only move left or right so this should be enough

        if (moveValue > 0)
        {
            movementDir = 1;
            if (isLeftDir)
            {
                rotLerp = 0f;
            }
            isLeftDir = false;
        }
        else if (moveValue < 0f)
        {
            movementDir = -1;
            if (!isLeftDir) //dir changed this frame
            {
                rotLerp = 0f;
            }
            isLeftDir = true;
        }
        //Movement
        hVelocity += moveValue * Time.deltaTime * speed;
        hVelocity = Mathf.Clamp(hVelocity, -hBounds, hBounds);
        Vector3 newPos = transform.position;
        newPos.x = hVelocity;


        if (Vector3.Distance(transform.position, newPos) > minDistToRotate)//Rotation
        {
            transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime * movementDir);
            Quaternion rot = transform.rotation;
            //rot.z = Mathf.Clamp(rot.z, -limitRot, limitRot);
            rot = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, rotationBounds.min, rotationBounds.max)));
            transform.rotation = rot;
        }
        else { isMoving = false; }
        
        transform.position = newPos; //apply position at the end as its needed for rotation calculations
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bit"))
        {
            collision.gameObject.SetActive(false);
            GameManager.instance.bitsEarnt++;
            sSoundManager.PlayBitPickUpClip();
        }

        if (died || GameManager.instance.isInvincible || !collisionOnEnter) { return; }
        sColorControl cont = collision.gameObject.GetComponent<sColorControl>();
        if (cont is object && cont.curColorType == colorControlRef.curColorType)
        {
            died = true;
            GameManager.instance.OnGameOver();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (died || GameManager.instance.isInvincible || collisionOnEnter) { return; }
        sColorControl cont = collision.gameObject.GetComponent<sColorControl>();
        if (cont is object && cont.curColorType == colorControlRef.curColorType)
        {
            died = true;
            GameManager.instance.OnGameOver();
        }
    }
}
