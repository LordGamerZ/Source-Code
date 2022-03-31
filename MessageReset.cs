using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Written by Abdul Galeel Ali

public class MessageReset : MonoBehaviour
{    
    private float Timer;
    public float ResetTime;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if(Timer > ResetTime)
        {
            Timer = 0;
            GetComponent<TextMeshProUGUI>().text = "";

            enabled = false;
        }
    }
}
