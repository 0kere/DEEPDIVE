using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sObstacleController : MonoBehaviour
{
    [SerializeField] private bool isMovingUp;
    public bool movingUp;
    [SerializeField] private float speedMod;

    private void OnEnable()
    {
        GameManager.gameOverEvent += OnGameOver;
        movingUp = isMovingUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingUp)
        {
            transform.position += transform.up * Time.deltaTime * speedMod * GameManager.instance.gameSpeed;

        }
    }

    private void OnGameOver()
    {
        //movingUp = false;//i dont think this really does anything but cause problems
    }
}
