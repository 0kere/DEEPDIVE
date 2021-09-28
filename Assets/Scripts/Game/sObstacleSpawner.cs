using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ObstacleObject //Why did i make this? it isnt used
{
    public GameObject prefab;
    public int introudctionScore; //score that this obstacle will be added to the available obstacle list
    public bool CanBeIntroduced() { if (GameManager.instance.ReturnScore() >= introudctionScore) { return true; } return false; }
}

public class sObstacleSpawner : MonoBehaviour
{
    private bool isSpawning = true; //toggling this to false turns off spawning. toggling back to true will have no effect
    
    [SerializeField] List<ObstacleObject> obstacleList = new List<ObstacleObject>();
    [SerializeField] List<GameObject> availableObstacles = new List<GameObject>();
    List<List<Transform>> obstaclePool = new List<List<Transform>>(); //obstaclePool[0] = pool for obstacleList[0]

    [SerializeField] private bool waitToSpawn; //set true when obstacle spawned, false when a new obstacle is able to spanw again
    private bool obstacleActive;
    private float LastIndex = -1; //lamst obstacle spawned

    [SerializeField] private float spawnDelay;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameOverEvent += GameOver;
        for (int i = 0; i < obstacleList.Count; i++)
        {
            obstaclePool.Add(new List<Transform>());
        }
        //StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnObstacle();
        }
    }

    private void GameOver()
    {
        isSpawning = false;
        for (int i = 0; i < obstaclePool.Count; i++)
        {
            for (int j = 0; j < obstaclePool[i].Count; j++)
            {
                obstaclePool[i][j].GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void StartSpawning()
    {
        availableObstacles.Clear();
        isSpawning = true;
        waitToSpawn = false;
        StartCoroutine(SpawnLoop());
    }
    private IEnumerator SpawnLoop()
    {
        float t = 0f;
        while (isSpawning)
        {
            if (!waitToSpawn)
            {
                t += Time.deltaTime;
                if (t >= spawnDelay)
                {
                    waitToSpawn = true;
                    t = 0f;
                    SpawnObstacle();
                }
            }
            yield return null;
        }
    }

    private void SpawnObstacle()
    {
        Debug.Log("Spawn obstacle");
        UpdateAvailableObstacle();
        int rand = Random.Range(0, availableObstacles.Count);
        if (rand == LastIndex && availableObstacles.Count > 1) //simple wy to avoid repeats
        {
            if (rand == availableObstacles.Count - 1)
            { //rand index is the last object in list
                rand -= 1;
            }
            else
            {
                rand += 1;
            }
            
        }
        LastIndex = rand;
        if (obstaclePool[rand].Count > 0)
        {
            bool found = false;
            foreach (Transform item in obstaclePool[rand]) //take this foreach out
            {
                if (!item.GetChild(0).gameObject.activeSelf)
                {
                    Debug.Log("SpawnObj: from pool");
                    item.GetComponent<sObjectGroup>().SpawnInGroup();
                    found = true;
                    break;
                }

            }
            //spawn a new one if there is no matching inactive obstacle in scene
            if (!found)
            { 
                Debug.Log("SpawnObj: pool empty");
                GameObject temp = Instantiate(availableObstacles[rand], transform.position, Quaternion.identity, transform);
                obstaclePool[rand].Add(temp.transform);
                temp.GetComponent<sObjectGroup>().SpawnInGroup();
            }
        }
        else
        {
            Debug.Log("SpawnObj: first");
            GameObject temp = Instantiate(availableObstacles[rand], transform.position, Quaternion.identity, transform);
            temp.GetComponent<sObjectGroup>().SpawnInGroup();
            obstaclePool[rand].Add(temp.transform);
        }
        obstacleActive = true;
    }

    private void UpdateAvailableObstacle()
    {
        for (int i = 0; i < obstacleList.Count; i++)
        {
            if (obstacleList[i].CanBeIntroduced() && !availableObstacles.Contains(obstacleList[i].prefab))
            {
                availableObstacles.Add(obstacleList[i].prefab);
            }
        }
    }

    //obstacles let the spawner know when their lower bound passes the cmera lower bound so a new piece can be spawned without issue
    public void ObstacleEntirelyOnScreen()
    {
        waitToSpawn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "End")
        {
            waitToSpawn = false;
        }
    }
}
