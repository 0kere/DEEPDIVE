using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class ColorSwitchEventArgs : 
[System.Serializable]
public class ColorPallete
{
    public Color primary;
    public Color secondary;
    public Color tertiary;
}

public class sColourSwitchManager : MonoBehaviour
{
    private static sColourSwitchManager csm;
    public static sColourSwitchManager instance
    {
        get
        {
            if (!csm)
            {
                csm = FindObjectOfType<sColourSwitchManager>();
                if (!csm)
                {
                }
                else
                {
                    csm.Init();
                }
            }
            return csm;
        }
    }

    private void Init() { }

    public delegate void Switch(ColorPallete pallete, float speed, float cooldown);
    public static event Switch switchEvent;

    public delegate void GameEnd(ColorPallete pallete, float speed);
    public static event GameEnd gameEndEvent;

    public delegate void UpdatePallete();
    public static event UpdatePallete updateEvent;

    public int curPalleteIndex = 0;
    [SerializeField] private ColorPallete curColorPallete;
    public List<ColorPallete> palletes = new List<ColorPallete>();
    public List<bool> ownedPalletes = new List<bool>();
    [SerializeField] private float switchSpeed;
    [SerializeField] private float switchCooldown;
    private float initSwitchSpeed;
    private float initCooldownSpeed;
    [SerializeField] private float switchGameOverSpeed;

    private void Awake()
    {
        initSwitchSpeed = switchSpeed;
        initCooldownSpeed = switchCooldown;
    }

    void Start()
    {
        GameManager.gameOverEvent += GameOver;
        curColorPallete = palletes[curPalleteIndex];
        updateEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSwitch(curColorPallete, ReturnSwitchSpeed(), ReturnCooldownSpeed());
        }
    }

    public void SwitchColors()
    {
        OnSwitch(curColorPallete, ReturnSwitchSpeed(), ReturnCooldownSpeed());
    }

    private void OnSwitch(ColorPallete pallete, float speed, float cooldown)
    {
        if (GameManager.instance.allowColorSwitching)
        { 
            switchEvent?.Invoke(pallete, speed, cooldown); //only raises event if event is not null
            //sSoundManager.PlayColorSwitchClip();
        }
    }

    private void GameOver()
    {
        OnGameEnd(curColorPallete, switchGameOverSpeed);
    }

    public void OnGameEnd(ColorPallete pallete, float speed)
    {
        gameEndEvent?.Invoke(pallete, speed);
    }

    public void OnUpdatePallete(int newPalleteIndex)
    {
        if (GameManager.instance.uiM.palleteSelectorActive) //this will hopefully fix the seemingly random calling of this function
        { 
            Debug.Log(newPalleteIndex);
            curPalleteIndex = newPalleteIndex;
            curColorPallete = palletes[newPalleteIndex];
            updateEvent?.Invoke();
        }
    }

    public ColorPallete ReturnCurrentColorPair()
    {
        return curColorPallete;
    }
    public Color ReturnCurrentPrimaryColor()
    {
        return curColorPallete.primary;
    }
    public Color ReturnCurrentSecondaryColor()
    {
        return curColorPallete.secondary;
    }
    public Color ReturnCurrentTertiaryColor()
    {
        return curColorPallete.tertiary;
    }
    public float ReturnSwitchSpeed()
    {
        float speedIncrease = GameManager.instance.gameSpeed / GameManager.instance.initGameSpeed;
        return switchSpeed / speedIncrease;
        return switchSpeed;
    }

    public float ReturnCooldownSpeed()
    {
        //float speedIncrease = GameManager.instance.gameSpeed / GameManager.instance.initGameSpeed;
        //return switchCooldown * (2- speedIncrease);
        return switchCooldown;
    }
}
