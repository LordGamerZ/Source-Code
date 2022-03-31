using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class ProgressBar : MonoBehaviour
{
    public RectTransform Fill;

    public bool IsVertical;

    public void ChangeFill(float scale)
    {
        if (!IsVertical)
        {
            Fill.localScale = new Vector2(scale, 1);
        }
        else
        {
            Fill.localScale = new Vector2(1, scale);
        }
    }
}
