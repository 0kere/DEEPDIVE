using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles custom functionaility for mobile inputs
/// </summary>
public class InputHandler : MonoBehaviour, IInputHandle
{
    public void Tap(Vector2 startPos)
    {
        if (!GameManager.instance.isGameOver)
        {
            sColourSwitchManager.instance.SwitchColors();   
        }
    }

    //private void DoubleTap(Vector2 touchPosOne, Vector2 touchPosTwo) { } //not currently used
    //private void Hold(Vector2 touchPos, float duration) { } //Not used currently
    //private void Drag(Vector2 startPos, Vector2 endPos, float dragTime) { } //Not used currently

    public void Tilt(float rotValue)
    {
        if (GameManager.instance.player is object)
        {
            Debug.Log("player is object");
            GameManager.instance.player.OnMove(rotValue);
        }
    }

    //private void Move(Vector2 startPos, Vector2 curPos, Vector2 deltaPos, float duration) { } //Not used currently


}
