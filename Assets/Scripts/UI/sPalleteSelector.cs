using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPalleteSelector : MonoBehaviour
{
    [SerializeField] private GameObject palletePrefab;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform content;
    List<GameObject> palleteObject = new List<GameObject>();

    private void Start()
    {
        SetUpButtons();
    }

    private void SetUpButtons()
    {
        for (int i = 0; i < sColourSwitchManager.instance.palletes.Count; i++)
        {
            GameObject temp = Instantiate(palletePrefab) as GameObject;
            temp.transform.SetParent(canvas.transform, false);
            temp.transform.SetParent(content);
            temp.GetComponent<sPalleteIcon>().SetUpIcon(i, sColourSwitchManager.instance.ownedPalletes[i]);
            palleteObject.Add(temp);
        }
    }

    public void UpdatePallateState(int index)
    {
        palleteObject[index].GetComponent<sPalleteIcon>().Unlocked(index);
    }
}
