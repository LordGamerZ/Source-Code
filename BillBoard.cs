using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class BillBoard : MonoBehaviour
{
    public GameObject MeleeIcon;
    public GameObject RangedIcon;
    public GameObject BuilderIcon;
    public GameObject BuildingIcon;

    private void Awake()
    {
        MeleeIcon.SetActive(false);
        RangedIcon.SetActive(false);
        BuilderIcon.SetActive(false);
        BuildingIcon.SetActive(false);
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (PlayerInteraction.Instance)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - PlayerInteraction.Instance.transform.position);
        }
    }
}
