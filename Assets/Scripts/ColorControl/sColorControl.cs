using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sColorControl : MonoBehaviour
{
    public enum PalleteComponents
    {
        primary, secondnary, tertiary
    }

    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isText;
    [SerializeField] private bool isExcludeFromEndSwitch;
    public PalleteComponents startColorType;
    [SerializeField] private bool isStartAsPlayerColor;
    [SerializeField] private bool isSwitch;
    [SerializeField] private bool isFadeOut;
    [SerializeField] private bool isVisible; //background starts secondary so anyting primary should be visible

    public PalleteComponents curColorType;

    private IEnumerator colorSwitchRoutine;

    private Color curColor;

    private SpriteRenderer thisRend;
    private TMPro.TextMeshProUGUI textRef;

    [SerializeField] private bool isParticleSystem;
    private ParticleSystem ps;

    UnityEngine.UI.Image imageRef;
    [SerializeField] private bool isImage;

    private void OnDestroy()
    {
        sColourSwitchManager.gameEndEvent -= GameEnd;
        sColourSwitchManager.switchEvent -= SwitchColours;
        sColourSwitchManager.updateEvent -= UpdatePallete;
    }

    // Start is called before the first frame update
    void Start()
    {
        sColourSwitchManager.gameEndEvent += GameEnd;
        sColourSwitchManager.switchEvent += SwitchColours;
        sColourSwitchManager.updateEvent += UpdatePallete;

        if (isText)
        {
            textRef = GetComponent<TMPro.TextMeshProUGUI>();
        }
        else if (isParticleSystem)
        {
            ps = GetComponent<ParticleSystem>();
        }
        else if (isImage)
        {
            imageRef = GetComponent<UnityEngine.UI.Image>();
        }
        else
        {
            thisRend = GetComponent<SpriteRenderer>();
        }

        if (isStartAsPlayerColor)
        {
            startColorType = GameManager.instance.playerColorCont.curColorType;   
        }
        curColorType = startColorType;

        switch (curColorType)
        {
            case PalleteComponents.primary:
                isVisible = true;
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                break;
            case PalleteComponents.secondnary:
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                break;
            case PalleteComponents.tertiary:
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                break;
            default:
                break;
        }
        if (isText) { curColor = textRef.color; }
        else if (isParticleSystem) { var psMain = ps.main; curColor = psMain.startColor.color; }
        else if (isImage) { curColor = imageRef.color; }
        else { curColor = thisRend.color; }
    }

    private void OnEnable()
    {
        if (thisRend is null && textRef is null && imageRef is null && ps is null) { return; }//lets start function run on first enable
        sColourSwitchManager.switchEvent += SwitchColours;
        sColourSwitchManager.gameEndEvent += GameEnd;
        sColourSwitchManager.updateEvent += UpdatePallete;
        colorSwitchRoutine = null;
        if (isStartAsPlayerColor)
        {
            startColorType = GameManager.instance.playerColorCont.curColorType;
        }
        curColorType = startColorType;

        switch (curColorType)
        {
            case PalleteComponents.primary:
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentPrimaryColor(); }
                break;
            case PalleteComponents.secondnary:
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentSecondaryColor(); }
                break;
            case PalleteComponents.tertiary:
                if (isText) { textRef.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else if (isImage) { imageRef.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                else { thisRend.color = sColourSwitchManager.instance.ReturnCurrentTertiaryColor(); }
                break;
            default:
                break;
        }
        if (GameManager.instance.playerColorCont is object)
        { 
            isVisible = GameManager.instance.playerColorCont.curColorType == curColorType;
        }
        if (isText) { curColor = textRef.color; }
        else if (isParticleSystem) { var psMain = ps.main; curColor = psMain.startColor.color; }
        else if (isImage) { curColor = imageRef.color; }
        else { curColor = thisRend.color; }
    }

    private void UpdatePallete()
    {
            switch (curColorType)
            {
                case PalleteComponents.primary:
                    LerpColor(curColor, sColourSwitchManager.instance.ReturnCurrentPrimaryColor(), false);
                    break;
                case PalleteComponents.secondnary:
                    LerpColor(curColor, sColourSwitchManager.instance.ReturnCurrentSecondaryColor(), false);
                    break;
                case PalleteComponents.tertiary:
                    LerpColor(curColor, sColourSwitchManager.instance.ReturnCurrentTertiaryColor(), false);
                    break;
                default:
                    break;
            }
    }

    private void OnDisable()
    {
        sColourSwitchManager.switchEvent -= SwitchColours;
        sColourSwitchManager.gameEndEvent -= GameEnd;
        sColourSwitchManager.updateEvent -= UpdatePallete;

    }

    private void SwitchColours(ColorPallete pallete, float speed, float cooldown)
    {

        if (isPlayer)
        { 
            GameManager.instance.playerIsPrimary = !GameManager.instance.playerIsPrimary;
        }
        if (colorSwitchRoutine is object) { return; } //allow a delay between mechanic activation
        isVisible = !isVisible;
        if (!isSwitch)
        {
            if (!isFadeOut) {return;}
            if (isVisible)
            {//fades in or out objects that dont switch with the background
                switch (curColorType)
                {
                    case PalleteComponents.primary:
                        Color transparentCol = pallete.primary;
                        transparentCol.a = 0f;
                        colorSwitchRoutine = ColourLerp(transparentCol, pallete.primary, false, speed, cooldown);
                        StartCoroutine(colorSwitchRoutine);
                        break;
                    case PalleteComponents.secondnary:
                        Color trasnparentCol = pallete.secondary;
                        trasnparentCol.a = 0f;
                        colorSwitchRoutine = ColourLerp(trasnparentCol, pallete.secondary, false, speed, cooldown);
                        StartCoroutine(colorSwitchRoutine);
                        break;
                    case PalleteComponents.tertiary:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (curColorType)
                {
                    case PalleteComponents.primary:
                        Color transparentCol = pallete.primary;
                        transparentCol.a = 0f;
                        colorSwitchRoutine = ColourLerp(pallete.primary, transparentCol, false, speed, cooldown);
                        StartCoroutine(colorSwitchRoutine);
                        break;
                    case PalleteComponents.secondnary:
                        Color trasnparentCol = pallete.secondary;
                        trasnparentCol.a = 0f;
                        colorSwitchRoutine = ColourLerp(pallete.secondary, trasnparentCol, false, speed, cooldown);
                        StartCoroutine(colorSwitchRoutine);
                        break;
                    case PalleteComponents.tertiary:
                        break;
                    default:
                        break;
                }
            }
            return;
        }

        switch (curColorType)
        {
            case PalleteComponents.primary:
                colorSwitchRoutine = ColourLerp(pallete.primary, pallete.secondary, true, speed, cooldown);
                StartCoroutine(colorSwitchRoutine);
                //curColorType = PalleteComponents.secondnary;
                break;
            case PalleteComponents.secondnary:
                colorSwitchRoutine = ColourLerp(pallete.secondary, pallete.primary, true, speed, cooldown);
                StartCoroutine(colorSwitchRoutine);
                //curColorType = PalleteComponents.primary;
                break;
            case PalleteComponents.tertiary:
                break;
            default:
                break;
        }
    }

    private void GameEnd(ColorPallete pallete, float speed)
    {
        if (!isExcludeFromEndSwitch)
        {
            //curColorType = PalleteComponents.tertiary;
            if (colorSwitchRoutine is object) { StopCoroutine(colorSwitchRoutine); }
            StartCoroutine(ColourLerp(curColor, pallete.tertiary, true, speed, 0f));
        }
        StartCoroutine(Reset());
            
    }

    private IEnumerator Reset()
    {
        yield return GameManager.instance.gameOverDelayWFS;
        if (isStartAsPlayerColor)
        {
            startColorType = GameManager.instance.playerColorCont.curColorType;
        }
        LerpColor(curColor, ReturnColor(startColorType), true);
        //curColorType = startColorType;
    }

    public void LerpColor(Color startCol, Color endCol, bool updateType)
    {
        StartCoroutine(ColourLerp(startCol, endCol, updateType, sColourSwitchManager.instance.ReturnSwitchSpeed()/2, 0f));
    }

    private IEnumerator ColourLerp(Color startCol, Color endCol, bool updateType, float speed, float cooldown) 
    {

        if (updateType)
            curColorType = ReturnType(endCol);
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * speed;
            curColor = Color.Lerp(startCol, endCol, t);
            if (isText && textRef is object) { textRef.color = curColor; }
            else if (isParticleSystem && ps is object) { var psMain = ps.main; psMain.startColor = curColor; }
            else if (isImage && imageRef is object) { imageRef.color = curColor; }
            else if (thisRend is object) { thisRend.color = curColor; }
            yield return null;
        }


        yield return new WaitForSeconds(cooldown);
        colorSwitchRoutine = null;
    }

    private PalleteComponents ReturnType(Color color)
    {
        if (color == sColourSwitchManager.instance.ReturnCurrentPrimaryColor())
        {
            return PalleteComponents.primary;
        }
        else if (color == sColourSwitchManager.instance.ReturnCurrentSecondaryColor())
        {
            return PalleteComponents.secondnary;
        }
        else if (color == sColourSwitchManager.instance.ReturnCurrentTertiaryColor())
        {
            return PalleteComponents.tertiary;
        }
        return curColorType;
    }

    private Color ReturnColor(PalleteComponents color)
    {
        switch (color)
        {
            case PalleteComponents.primary:
                return sColourSwitchManager.instance.ReturnCurrentPrimaryColor();
            case PalleteComponents.secondnary:
                return sColourSwitchManager.instance.ReturnCurrentSecondaryColor();
            case PalleteComponents.tertiary:
                return sColourSwitchManager.instance.ReturnCurrentTertiaryColor();
            default:
                return curColor;
        }
    }

    public bool ReturnIsVisibile(Color curPlayer)
    {
        if (CompareColor(curColor, curPlayer))
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
        return isVisible;
    }

    public Color ReturnCurrentColor()
    {
        return curColor;
    }

    public static bool CompareColor(Color i, Color j)
    {
        if (i == j) { return true; }
        return false;
    }

    public void UpdateColor(ColorPallete pallete)
    {
        if (pallete is null) { pallete = sColourSwitchManager.instance.ReturnCurrentColorPair(); }
        Color newCol = curColor;
        switch (curColorType)
        {
            case PalleteComponents.primary:
                newCol = pallete.primary;
                break;
            case PalleteComponents.secondnary:
                newCol = pallete.secondary;
                break;
            case PalleteComponents.tertiary:
                newCol = pallete.tertiary;
                break;
            default:
                break;
        }
        if (isText) { textRef.color = newCol; }
        else if (isParticleSystem) { var psMain = ps.main; psMain.startColor = newCol; }
        else if (isImage) { imageRef.color = newCol; }
        else { thisRend.color = newCol; }
        curColor = newCol;
    }
}
