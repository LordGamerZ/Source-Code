using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Written by Abdul Galeel Ali

public class Chat : MonoBehaviour
{
    public void SetMessage(string message)
    {
        GameObject container = GameObject.Find("Container");
        transform.SetParent(container.transform);
        GetComponent<TextMeshProUGUI>().text = message;
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        Destroy(this);
    }
}