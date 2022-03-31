using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Written by Abdul Galeel Ali

public class Building : MonoBehaviour
{
    private UnitSelectable Selectable;
    public GameObject Mesh;

    private BuilderCommands Builder;

    private float StartHeight;

    private float BuildTime;
    private float TotalTime;

    public Transform ColoursParent;

    private void Awake()
    {
        Selectable = GetComponent<UnitSelectable>();
        StartHeight = Mathf.Abs(Mesh.transform.localPosition.y);
    }

    public void SetBuilding(int time, BuilderCommands builderCommands, Color color)
    {
        Builder = builderCommands;
        TotalTime = 0;
        BuildTime = time;

        if (ColoursParent)
        {
            for (int i = 0; i < ColoursParent.childCount; i++)
            {
                ColoursParent.GetChild(i).GetComponent<Renderer>().material.color = color;
            }
        }
    }

    public bool Build()
    {
        Selectable.CurrentHealth += Selectable.MaxHealth * (1 / BuildTime);
        TotalTime += 1;
        Mesh.transform.localPosition = new Vector3(Mesh.transform.localPosition.x, Mesh.transform.localPosition.y + (StartHeight * (1 / BuildTime)), Mesh.transform.localPosition.z);

        if (TotalTime >= BuildTime)
        {
            Builder.IsBuilding = false;

            MonoBehaviour[] comps = GetComponents<MonoBehaviour>();

            foreach (var comp in comps)
            {
                if (comp.enabled == false)
                {
                    comp.enabled = true;
                }
            }

            return true;
        }

        return false;
    }

    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            Selectable.CurrentHealth -= amount;

            if (Selectable.CurrentHealth <= 0)
            {
                Builder.IsBuilding = false;
                Destroy(gameObject);
            }
        }
    }
}