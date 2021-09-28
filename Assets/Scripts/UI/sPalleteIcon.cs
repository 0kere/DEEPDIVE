using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPalleteIcon : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image primaryImage;
    [SerializeField] private UnityEngine.UI.Image secondaryImage;
    [SerializeField] private UnityEngine.UI.Image tertiaryImage;
    [SerializeField] private UnityEngine.UI.Image outerLeft;
    [SerializeField] private UnityEngine.UI.Image outerRight;
    [SerializeField] private UnityEngine.UI.Image lockedIcon;

    public void SetUpIcon(int palleteIndex, bool owned)
    {
        ColorPallete thisPallete = sColourSwitchManager.instance.palletes[palleteIndex];
        if (owned)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { sColourSwitchManager.instance.OnUpdatePallete(palleteIndex); });
            lockedIcon.gameObject.SetActive(false);
        }
        else
        {
            lockedIcon.gameObject.SetActive(true);
        }
        primaryImage.color = thisPallete.primary;
        secondaryImage.color = thisPallete.secondary;
        tertiaryImage.color = thisPallete.tertiary;
        outerLeft.color = thisPallete.secondary;
        outerRight.color = thisPallete.primary;
    }

    public void Unlocked(int index)
    {
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { sColourSwitchManager.instance.OnUpdatePallete(index); });
        lockedIcon.gameObject.SetActive(false);
    }
}
