using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sObjectGroup : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    private bool isEntirelyOnScreen;
    [HideInInspector] public bool isEnteredScreen;
    
    sObstacleSpawner spawnerRef;

    [SerializeField] private Transform startObj;
    [SerializeField] private Transform endObj;

    Vector3 spawnPos = new Vector3(Screen.width / 2, 0, 0);
    [SerializeField] private float screenBottomYValue;
    [SerializeField] private float screenTopYValue;

    sObstacleController cont;


    // Start is called before the first frame update
    void Awake()
    {
        cont = gameObject.GetComponent<sObstacleController>();
        spawnerRef = transform.parent.gameObject.GetComponent<sObstacleSpawner>();
        spawnPos = transform.position;
        spawnPos.y = 0f;
        spawnPos = Camera.main.ScreenToWorldPoint(spawnPos);
        screenBottomYValue = spawnPos.y;
        spawnPos.x = 0f;
        spawnPos.z = 0f;
        spawnPos.y -= (startObj.position.y - transform.position.y); //lower spawn position by distance from center to start

        screenTopYValue = (Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y);

    }

    private void OnEnable()
    {
        GameManager.gameOverEvent += GameOver;

        SpawnInGroup();
    }

    private void OnDisable()
    {
        GameManager.gameOverEvent -= GameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if (endObj.position.y >= screenBottomYValue && isActive && !isEntirelyOnScreen) //if end is on screen spawn new obj
        {
            Debug.Log("On screen");
            isEntirelyOnScreen = true;
            spawnerRef.ObstacleEntirelyOnScreen();
        }
        else if (endObj.position.y >= screenTopYValue && isActive && isEntirelyOnScreen)
        {
            isEnteredScreen = false;
            isEntirelyOnScreen = false;
            isActive = false;
            transform.GetChild(0).gameObject.SetActive(false);
            cont.movingUp = false;
            transform.position = spawnPos;
        }
        if (startObj.position.y >= screenBottomYValue && isActive && !isEnteredScreen)
        {
            isEnteredScreen = true;
        }
    }

    public void GameOver()
    {
        isActive = false; //caused a soft lock for the longest time cause an invisible obstacle would trigger an obstacle spawn
    }
    public void SpawnInGroup()

    {
        isEntirelyOnScreen = false;
        isEnteredScreen = false;
        transform.position = spawnPos;
        cont.movingUp = true;
        //spawnPos.x = 0f; //has to be set to 0 again as it was always been set to 173 at somepoint
        transform.GetChild(0).gameObject.SetActive(true);
        isActive = true;
        Debug.Log("Spawn in group " + gameObject.name);
    }
}
