using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sResizeCamera : MonoBehaviour
{
    private float designOrthoSize;
    private float designAspect;
    private float designWidth;
    private float designAspectHeight = 16;
    private float designAspectWidth = 10;

    [SerializeField] private Transform background; //background sized to fit the playfied

    private void Awake()
    {
        designOrthoSize = Camera.main.orthographicSize;
        designAspect = designAspectHeight / designAspectWidth;
        designWidth = designOrthoSize / designAspect;

        Resize();
    }
    //adapted from (SylafrsOne, 2014)
    private void Resize()
    {
        float size = designWidth / Camera.main.aspect;
        Camera.main.orthographicSize = Mathf.Max(size, designOrthoSize);

        float height = Camera.main.orthographicSize * 2f;
        float width = height * Screen.width / Screen.height;
        Vector2 scale = new Vector2(width, height);
        background.localScale = scale;
    }
    //end of adapted code
}
