using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBitsSpawner : MonoBehaviour
{
    [SerializeField] private int maxBitsToSpawn;
    private List<GameObject> bits = new List<GameObject>();
    private List<GameObject> active = new List<GameObject>();
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bits.Add(transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        SpawnBits();
    }

    private void OnDisable()
    {
        DespawnBits();
    }

    private void SpawnBits()
    {
        int bitsToSpawn = Random.Range(0, maxBitsToSpawn + 1);
        for (int i = 0; i <= bitsToSpawn; i++)
        {
            int rand = Random.Range(0, bits.Count);
            bits[rand].SetActive(true);
            active.Add(bits[rand]);
            bits.RemoveAt(rand);
        }
    }

    private void DespawnBits()
    {
        for (int i = 0; i < active.Count; i++)
        {
            active[i].SetActive(false);
            bits.Add(active[i]);
        }
        active.Clear();
    }
}
