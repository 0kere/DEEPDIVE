using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sRandomPallete : MonoBehaviour
{
    [SerializeField] private float timeBetweenPalleteSwitch;
    [SerializeField] private TMPro.TextMeshProUGUI bitsText;
    [SerializeField] private int palleteCost;

    [SerializeField] private sColorControl primaryImage;
    [SerializeField] private sColorControl secondaryImage;
    [SerializeField] private sColorControl tertiaryImage;
    [SerializeField] private sColorControl outerLeft;
    [SerializeField] private sColorControl outerRight;
    [SerializeField] private sColorControl bitText;
    [SerializeField] private sColorControl bitIcon;

    [SerializeField] private sPalleteSelector palleteSelector;

    private IEnumerator randomizeRoutine;

    [SerializeField] GameObject handObject;
    private void OnEnable()
    {
        bitsText.text = GameManager.instance.totalBits + "/" + palleteCost;
        randomizeRoutine = Randomsize();
        StartCoroutine(randomizeRoutine);
        if (GameManager.instance.totalBits >= palleteCost)
        {
            handObject.SetActive(true);
        }
        else
        { 
            handObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(randomizeRoutine);
        randomizeRoutine = null;
    }

    public void OnPressed()
    {
        if (GameManager.instance.totalBits >= palleteCost)
        {
            List<ColorPallete> unownedPallete = new List<ColorPallete>();
            List<ColorPallete> temp = sColourSwitchManager.instance.palletes;
            for (int i = 0; i < temp.Count; i++)
            {
                if (!sColourSwitchManager.instance.ownedPalletes[i])
                {
                    unownedPallete.Add(temp[i]);
                }
            }
            if (unownedPallete.Count == 0) { return; }//all paletes have been unlocked
            if (unownedPallete.Count == 1) { GPSHandler.AllPalletesUnlocked(); } //the last pallete will be achived here
            int rand = Random.Range(0, unownedPallete.Count);
            rand = sColourSwitchManager.instance.palletes.IndexOf(unownedPallete[rand]);
            sColourSwitchManager.instance.ownedPalletes[rand] = true;
            palleteSelector.UpdatePallateState(rand);
            sColourSwitchManager.instance.OnUpdatePallete(rand);
            GameManager.instance.totalBits -= palleteCost;
            GameManager.instance.uiM.UpdateBits();
            bitsText.text = GameManager.instance.totalBits + "/" + palleteCost;
            GameManager.instance.SaveData();
        }
    }

    private IEnumerator Randomsize()
    {
        WaitForSeconds delay = new WaitForSeconds(timeBetweenPalleteSwitch);
        int index = sColourSwitchManager.instance.curPalleteIndex;
        int palleteCount = sColourSwitchManager.instance.palletes.Count;
        UpdatePalleteColor(index);
        while (true)
        {
            yield return delay;
            int rand = Random.Range(0, palleteCount);
            Debug.Log(rand.ToString());
            UpdatePalleteColor(rand);
            yield return null;
        }
    }

    private void UpdatePalleteColor(int index)
    {
        ColorPallete thisPallete = sColourSwitchManager.instance.palletes[index];

        primaryImage.LerpColor(primaryImage.ReturnCurrentColor(), thisPallete.primary, false);
        secondaryImage.LerpColor(secondaryImage.ReturnCurrentColor(), thisPallete.secondary, false);
        tertiaryImage.LerpColor(tertiaryImage.ReturnCurrentColor(), thisPallete.tertiary, false);
        outerLeft.LerpColor(outerLeft.ReturnCurrentColor(), thisPallete.secondary, false);
        outerRight.LerpColor(outerRight.ReturnCurrentColor(), thisPallete.primary, false);
        bitText.LerpColor(bitText.ReturnCurrentColor(), thisPallete.primary, false);
        bitIcon.LerpColor(bitIcon.ReturnCurrentColor(), thisPallete.tertiary, false);
    }
}
