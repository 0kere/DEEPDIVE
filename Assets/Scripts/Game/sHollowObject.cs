using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sHollowObject : MonoBehaviour
{
    private enum Direction
    { 
        up, down, left, right
    }
    [SerializeField] private Direction moveDir;
    private Direction initMoveDir;
    private Vector3 moveVector;
    private Vector3 initLocalPos;
    private Vector3 activationPos;
    private bool active;

    private bool onScreen;
    [SerializeField] private sObjectGroup objGroup;
    [SerializeField] private float speedMod;

    [SerializeField] private bool limitDist;
    [SerializeField] private float distance;
    private bool reachedLimit;

    [SerializeField] private bool isReactivatable; //when reactivated object will move in the opposite direction and also must be a limit dist or it wont reactivate


    [SerializeField] private bool hasDelay;
    private bool _hasDelay;
    [SerializeField] private float delay;
    void Awake()
    {
        initMoveDir = moveDir;
        initLocalPos = transform.localPosition;
        _hasDelay = hasDelay;
        moveVector = ReturnMoveDirection();
    }


    private void OnDisable()
    {
        moveDir = initMoveDir;
        moveVector = ReturnMoveDirection();
        reachedLimit = false;
        active = false;
        onScreen = false;
        transform.localPosition = initLocalPos;
        sColourSwitchManager.switchEvent -= Activate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!onScreen)
        {
            if (objGroup.isEnteredScreen)
            {
                onScreen = true;
                sColourSwitchManager.switchEvent += Activate;
            }
        }
        if (active && !reachedLimit)
        {
            transform.position += moveVector * Time.deltaTime * speedMod * GameManager.instance.gameSpeed;
            if (CheckDistance() && limitDist)
            {
                reachedLimit = true;
            }
        }
    }

    private bool CheckDistance()
    {
        bool reachedDistance = false;
        switch (moveDir)
        {
            case Direction.up:
                if (activationPos.y + distance <= transform.localPosition.y)
                { reachedDistance = true; }
                break;
            case Direction.down:
                if (activationPos.y - distance >= transform.localPosition.y)
                { reachedDistance = true; }
                break;
            case Direction.left:
                if (activationPos.x - distance >= transform.localPosition.x)
                { reachedDistance = true; }
                break;
            case Direction.right:
                if (activationPos.x + distance <= transform.localPosition.x)
                { reachedDistance = true; }
                break;
            default:
                break;
        }
        return reachedDistance;
    }
    private Vector3 ReturnMoveDirection()
    {
        Vector3 dir = Vector3.zero;
        switch (moveDir)
        {
            case Direction.up:
                dir = transform.up;
                break;
            case Direction.down:
                dir = -transform.up;
                break;
            case Direction.left:
                dir = -transform.right;
                break;
            case Direction.right:
                dir = transform.right;
                break;
            default:
                break;
        }
        return dir;
    }

    private Direction ReturnOppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.up:
                dir = Direction.down;
                break;
            case Direction.down:
                dir = Direction.up;
                break;
            case Direction.left:
                dir = Direction.right;
                break;
            case Direction.right:
                dir = Direction.left;
                break;
            default:
                break;
        }
        return dir;
    }

    private void Activate(ColorPallete pallete, float speed, float cooldown) //params only needed for the event
    {
        if (_hasDelay) { StartCoroutine(ActivateDelay()); return; }
        if (!active)
        {
            active = true;
            activationPos = transform.localPosition;

        }
        else if (active && reachedLimit && isReactivatable)
        {
            activationPos = transform.localPosition;
            moveDir = ReturnOppositeDirection(moveDir);
            moveVector = ReturnMoveDirection();
            reachedLimit = false;
        }
        _hasDelay = hasDelay;
    }

    private IEnumerator ActivateDelay()
    {
        yield return new WaitForSeconds(delay);
        _hasDelay = false;
        ColorPallete unrequried = new ColorPallete();
        Activate(unrequried, 0f, 0f);
    }
}
