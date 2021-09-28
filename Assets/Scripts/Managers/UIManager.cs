using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour//todo add an end game routine, somewhere the colors for all bgame object should lerp to the background color (secondary) with the menu reappering and score text returing the correct location during this
{
    [SerializeField] private TextMeshProUGUI scoreText; //used to display highscore and will then lerp to position and size for the score counter, lerping down to 0 at the same time
    private RectTransform scoreRect;
    [SerializeField] private Vector2 scoreMenuPos;
    [SerializeField] private Vector2 scoreGamePos;
    [SerializeField] private float scoreMenuSize;
    [SerializeField] private float scoreGameSize;

    [SerializeField] private GameObject menuCanvas;
    private RectTransform menuRect;

    [SerializeField] private float canvasLerpSpeed;
    [SerializeField] private Vector2 lerpToPos;

    private GameObject curActiveCanvas;

    [SerializeField] private RectTransform logoRect;
    [SerializeField] private TMPro.TextMeshProUGUI newHighScoreText;
    private Color32 highScoreColor;

    private Vector2 logoMenuPos;
    [SerializeField] private Vector2 logoOffScreenPos;
    [SerializeField] private Vector2 logoCenterScreenPos;

    [SerializeField] private sColorControl backgroundColorController; //need this one to go from tertianry to secondary

    [SerializeField] private Image crownIcon;

    [SerializeField] private Vector2 leftScreenPos;
    [SerializeField] private Vector2 rightScreenPos;

    [SerializeField] private GameObject palleteSelector;
    private RectTransform palleteSelectorRect;
    [SerializeField] private Vector2 palleteSelectorOnScreenPos;
    [HideInInspector] public bool palleteSelectorActive;

    [SerializeField] private TextMeshProUGUI totalBitsText;
    [SerializeField] private TextMeshProUGUI addBitsText;
    [SerializeField] private float bitsLerpMaxDuration;
    [SerializeField] private float bitsLerpSpeed;
    [SerializeField] private float addBitsFadeOutSpeed;

    [SerializeField] private GameObject gift;
    private RectTransform giftRect;
    private Vector2 initGiftPos;

    [SerializeField] private Image okereImage;
    [SerializeField] private Image deepImage;
    [SerializeField] private Image diveImage;
    [SerializeField] private GameObject deepDiveObject;

    private bool settingsActive;
    [SerializeField] private RectTransform settingsRect;
    [SerializeField] private TextMeshProUGUI signOutText;

    [SerializeField] private TextMeshProUGUI inputTypeText;
    [SerializeField] private GameObject unsupportedText;
    bool isGyro;
    private void Start()
    {
        curActiveCanvas = menuCanvas;

        scoreRect = scoreText.gameObject.GetComponent<RectTransform>();
        menuRect = menuCanvas.GetComponent<RectTransform>();
        palleteSelectorRect = palleteSelector.GetComponent<RectTransform>();
        giftRect = gift.GetComponent<RectTransform>();

        initGiftPos = giftRect.anchoredPosition;
        highScoreColor= crownIcon.color;
        logoMenuPos = logoRect.anchoredPosition;
        addBitsText.gameObject.SetActive(false);
        gift.SetActive(false);
        palleteSelector.SetActive(palleteSelectorActive);
        UpdateBits();
        UpdateHighScore();

        //Moves everything to inital positions
        menuRect.anchoredPosition = lerpToPos;
        scoreRect.position = menuRect.position; //score is anchored to top of screen so this should just ensure it isnt on screen
        logoRect.anchoredPosition = logoCenterScreenPos;
        //was going to set background color here but this runs before the sColorControl start and so throws an error. Default background color has been changed instead
    }

    private void test()
    {
        Debug.Log("WORKWED");
    }

    public void OnPlayButton()
    {
        if (GameManager.instance.isGameOver && !palleteSelectorActive)
        {
            GameManager.instance.StartGame();
            StartCoroutine(PlayGameRoutine());
            sSoundManager.PlayButtonPressClip();
        }     
    }

    public void OnSettingsButton()
    {
        if (settingsActive)
        {
            settingsActive = false;
            Vector2 menuLeft = leftScreenPos;
            menuLeft.y = menuRect.anchoredPosition.y;
            StartCoroutine(LerpPos(menuRect, menuLeft, Vector3.zero));
            menuLeft.y = scoreRect.anchoredPosition.y;
            StartCoroutine(LerpPos(scoreRect, menuLeft, scoreMenuPos));
            menuLeft.y = settingsRect.anchoredPosition.y;
            StartCoroutine(LerpPos(settingsRect, Vector2.zero, menuLeft));
        }
        else
        {
            settingsActive = true;
            Vector2 menuRight = rightScreenPos;
            menuRight.y = menuRect.anchoredPosition.y;
            StartCoroutine(LerpPos(menuRect, Vector3.zero, menuRight));
            menuRight.y = scoreRect.anchoredPosition.y;
            StartCoroutine(LerpPos(scoreRect, scoreMenuPos, menuRight));
            menuRight.y = settingsRect.anchoredPosition.y;
            StartCoroutine(LerpPos(settingsRect, menuRight, Vector2.zero));
        }
        sSoundManager.PlayButtonPressClip();

    }

    public void OnPalleteButton()
    {
        Debug.Log("button pressed");
        if (palleteSelectorActive)
        {
            palleteSelectorActive = false;
            Vector2 menuLeft = leftScreenPos;
            menuLeft.y = menuRect.anchorMin.y;
            StartCoroutine(LerpPos(menuRect, menuLeft, Vector3.zero));
            menuLeft.y = scoreRect.anchoredPosition.y;
            StartCoroutine(LerpPos(scoreRect, menuLeft, scoreMenuPos));
            StartCoroutine(MovePalleteSelectorOffScreen());
        }
        else
        {
            palleteSelectorActive = true;
            palleteSelector.SetActive(palleteSelectorActive);
            Vector2 menuRight = rightScreenPos;
            menuRight.y = menuRect.anchorMin.y;
            StartCoroutine(LerpPos(menuRect, Vector2.zero, menuRight));
            menuRight.y = scoreRect.anchoredPosition.y;
            StartCoroutine(LerpPos(scoreRect, scoreMenuPos, menuRight));
            StartCoroutine(LerpPos(palleteSelectorRect, leftScreenPos, palleteSelectorOnScreenPos));
        }
        sSoundManager.PlayButtonPressClip();

    }

    private IEnumerator MovePalleteSelectorOffScreen()
    {
        yield return StartCoroutine(LerpPos(palleteSelectorRect, palleteSelectorOnScreenPos, rightScreenPos));
        palleteSelector.SetActive(palleteSelectorActive);
    }

    public void OnLeaderBoardButton()
    {
        Debug.Log("Leaderboard clicked");
        if (GPSHandler.IsAuthenticated())
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            System.Action leaderboard = OnLeaderBoardButton;
            System.Action temp = OnLeaderBoardButton;
            GPSHandler.InteractiveSignIn(temp);
        }
        sSoundManager.PlayButtonPressClip();

    }

    public void OnAchievementButton()
    {
        Debug.Log("Achievements clicked");
        if (GPSHandler.IsAuthenticated())
        {
            Social.ShowAchievementsUI();
        }
        else
        {
            System.Action temp = OnAchievementButton;
            GPSHandler.InteractiveSignIn(temp);
        }
        sSoundManager.PlayButtonPressClip();

    }

    public void OnLogOut()
    {
        if (GPSHandler.IsAuthenticated())
        {
            signOutText.text = "Sign In";
            GPSHandler.LogOut();
        }
        else
        {
            GPSHandler.InteractiveSignIn(null);
        }
        sSoundManager.PlayButtonPressClip();

    }

    public void OnInputType()
    {
        isGyro = !isGyro;
        unsupportedText.SetActive(false);
        if (isGyro && !SystemInfo.supportsGyroscope)
        {
            inputTypeText.text = "< Gyroscope >";
            unsupportedText.SetActive(true);
        }
        else if (isGyro)
        {
            inputTypeText.text = "< Gyroscope >";
            GameManager.instance.tryGyro = true;
        }
        else
        { 
            inputTypeText.text = "< Accelerometer >";
        }
        sSoundManager.PlayButtonPressClip();

    }

    public void LogInProccesed() //Called after log in button process is complete
    {
        if (GPSHandler.IsAuthenticated())
        {
            signOutText.text = "Sign Out";
        }
        else
        {
            signOutText.text = "Sign In";
        }
    }
    /// <summary>
    /// This is to be called after GPGS has had the chance to complete authentication
    /// </summary>
    public void InitGameMenu()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(.3f);
        deepDiveObject.SetActive(true);//Should avoid the objects colour being set in start of sColorControl

        float t = 0f;
        backgroundColorController.startColorType = sColorControl.PalleteComponents.secondnary;
        backgroundColorController.curColorType = sColorControl.PalleteComponents.secondnary;
        backgroundColorController.UpdateColor(null);
        Vector2 scoreInitPos = scoreRect.anchoredPosition;
        StartCoroutine(LerpPos(scoreRect, scoreInitPos, scoreMenuPos));
        StartCoroutine(LerpPos(menuRect, lerpToPos, Vector3.zero));
        StartCoroutine(LerpPos(logoRect, logoCenterScreenPos, logoMenuPos));

        while (t <= 1f)
        {
            t += Time.deltaTime * canvasLerpSpeed;
            Color32 newCol = okereImage.color;
            newCol.a = (byte)Mathf.Lerp(255f, 0f, t);
            okereImage.color = newCol;

            newCol = deepImage.color;
            newCol.a = (byte)Mathf.Lerp(0, 255, t);
            deepImage.color = newCol;

            newCol = diveImage.color;
            newCol.a = (byte)Mathf.Lerp(0, 255, t);
            diveImage.color = newCol;
            yield return null;
        }
        okereImage.gameObject.SetActive(false);
    }

    public void OnGiftPressed(int giftValue)
    {
        StartCoroutine(Gift(giftValue));
        Vector2 giftRightScreen = rightScreenPos;
        giftRightScreen.y = initGiftPos.y;
        StartCoroutine(LerpPos(giftRect, giftRect.anchoredPosition, giftRightScreen));
        sSoundManager.PlayButtonPressClip();

    }

    private IEnumerator Gift(int value)
    {
        //fade in add bits text
        addBitsText.text = "+ " + value;
        if (!addBitsText.gameObject.activeSelf)
        {
            addBitsText.gameObject.SetActive(true);
            Color32 temp = addBitsText.color;
            temp.a = 0;
            addBitsText.color = temp;
        }
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * addBitsFadeOutSpeed;
            Color32 addBitsCol = sColourSwitchManager.instance.ReturnCurrentPrimaryColor();
            
            addBitsCol.a = (byte)Mathf.Lerp(0, 255, t);
            addBitsText.color = addBitsCol;

            yield return null;
        }

        //lerp add bits and total bits texts
        float initTotal = GameManager.instance.totalBits;
        float timeStep = bitsLerpMaxDuration / value;
        yield return new WaitForSeconds(bitsLerpMaxDuration); //initial wait time to ensure players can see the bits being added. Uses the max time step to give a consistent delay here

        timeStep = Mathf.Min(timeStep, bitsLerpSpeed);
        WaitForSeconds step = new WaitForSeconds(timeStep);

        GameManager.instance.totalBits += value;
        GameManager.instance.SaveData();
        while (value > 0)
        {
            yield return step;
            addBitsText.text = "+ " + --value;
            totalBitsText.text = "x " + ++initTotal;
        }
        //fade out add bits
        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * addBitsFadeOutSpeed;
            Color32 addBitsCol = sColourSwitchManager.instance.ReturnCurrentPrimaryColor();
            addBitsCol.a = (byte)Mathf.Lerp(255, 0, t);
            addBitsText.color = addBitsCol;

            yield return null;
        }
    }

    public void ChangeCanvas(GameObject newCanvas)
    {
        curActiveCanvas.SetActive(false);
        curActiveCanvas = newCanvas;
        if (curActiveCanvas is object)
        { 
            curActiveCanvas.SetActive(true);
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * canvasLerpSpeed;
            menuRect.anchoredPosition = Vector3.Lerp(Vector3.zero, lerpToPos, t);
            scoreRect.anchoredPosition = Vector3.Lerp(scoreMenuPos, scoreGamePos, t);
            scoreText.fontSize = Mathf.Lerp(scoreMenuSize, scoreGameSize, t);
            logoRect.anchoredPosition = Vector3.Lerp(logoMenuPos, logoOffScreenPos, t);

            Color32 highScoreCol = newHighScoreText.color;
            highScoreCol.a = (byte)Mathf.Lerp(highScoreColor.a, 0, t);
            newHighScoreText.color = highScoreCol;

            highScoreCol = crownIcon.color;
            highScoreCol.a = (byte)Mathf.Lerp(highScoreColor.a, 0, t);
            crownIcon.color = highScoreCol;
            yield return null;
        }

        menuRect.gameObject.SetActive(false);
    }

    public IEnumerator EndGameEarlyRoutine(bool isNewHighScore)//moves the score to the center of screen while tertiary background still shown
    {
        float t = 0f;
        while (t <= 1f) //
        {
            t += Time.deltaTime * canvasLerpSpeed;
            scoreRect.anchoredPosition = Vector3.Lerp(scoreGamePos, scoreMenuPos, t);
            scoreText.fontSize = Mathf.Lerp(scoreGameSize, scoreMenuSize, t);
            if (isNewHighScore) //enable crown and new highscore text if new highscore only
            {
                newHighScoreText.enabled = true;
                Color32 highScoreCol = newHighScoreText.color;
                highScoreCol.a = (byte)Mathf.Lerp(0, highScoreColor.a, t);
                newHighScoreText.color = highScoreCol;

                crownIcon.enabled = true;
                highScoreCol = crownIcon.color;
                highScoreCol.a = (byte)Mathf.Lerp(0, highScoreColor.a, t);
                crownIcon.color = highScoreCol;
            }
            yield return null;
        }

    }

    public IEnumerator EndGameRoutine(bool isNewHighScore) //moves back to the menu
    {
        menuRect.gameObject.SetActive(true);

        //enable addbits texct before menu is moved
        int toAdd = GameManager.instance.bitsEarnt;
        int total = GameManager.instance.totalBits;
        int initTotal = total - toAdd;
        if (toAdd > 0)
        {
            Debug.Log("Add text set up and enabled");
            addBitsText.gameObject.SetActive(true);
            //Color temp = addBitsText.color;
            //temp.a = bitsAddInitAlpha;
            //addBitsText.color = temp;
        }

        if (GameManager.instance.ReturnGiftAvailable())
        {
            gift.SetActive(true);
            giftRect.anchoredPosition = initGiftPos;
        }

        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * canvasLerpSpeed;
            menuRect.anchoredPosition = Vector3.Lerp(leftScreenPos, Vector3.zero, t);

            logoRect.anchoredPosition = Vector3.Lerp(logoOffScreenPos, logoMenuPos, t);

            Color32 highScoreCol = newHighScoreText.color;
            highScoreCol.a = (byte)Mathf.Lerp(0, highScoreColor.a, t);
            newHighScoreText.color = highScoreCol;
            if (!crownIcon.IsActive())
            { 
                crownIcon.enabled = true;
                highScoreCol = crownIcon.color;
                highScoreCol.a = (byte)Mathf.Lerp(0, highScoreColor.a, t);
                crownIcon.color = highScoreCol;
            }

            scoreText.text = GameManager.instance.highScore.ToString();
            if (isNewHighScore)
            {
                newHighScoreText.enabled = true;
            }
            else
            {
                newHighScoreText.enabled = false;
            }
            //backgroundColorController.LerpColor(sColourSwitchManager.instance.ReturnCurrentTertiaryColor(), sColourSwitchManager.instance.ReturnCurrentSecondaryColor());
            yield return null;
        }

        //Handle bits earnt once menu is visible again
        if (toAdd > 0)
        {
            totalBitsText.text = "x " + initTotal;
            addBitsText.text = "+ " + toAdd;
            float timeStep = bitsLerpMaxDuration / toAdd;
            yield return new WaitForSeconds(bitsLerpMaxDuration); //initial wait time to ensure players can see the bits being added. Uses the max time step to give a consistent delay here

            timeStep = Mathf.Min(timeStep, bitsLerpSpeed);
            WaitForSeconds step = new WaitForSeconds(timeStep);

            while (toAdd > 0)
            {
                yield return step;
                addBitsText.text = "+ " + --toAdd;
                totalBitsText.text = "x " + ++initTotal;
            }
        }
        totalBitsText.text = "x " + total;
        addBitsText.text = "+ 0";

        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * addBitsFadeOutSpeed;
            Color32 addBitsCol = newHighScoreText.color;
            addBitsCol.a = (byte)Mathf.Lerp(highScoreColor.a, 0, t);
            addBitsText.color = addBitsCol;

            yield return null;
        }
    }

    public void UpdateBits()
    {
        totalBitsText.text = "x " + GameManager.instance.totalBits;
    }

    public void UpdateHighScore()
    {
        scoreText.text = GameManager.instance.highScore.ToString();
    }

    private IEnumerator LerpPos(RectTransform obj, Vector2 startPos, Vector2 newPos)
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * canvasLerpSpeed;
            obj.anchoredPosition = Vector2.Lerp(startPos, newPos, t);

            yield return null;
        }
    }
}
