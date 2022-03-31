using System;
using System.Collections.Generic;
using UnityEngine;

//Written by Abdul Galeel Ali

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> ActionOnMainThread = new List<Action>();
    private static readonly List<Action> ActionCopiedOnMainThread = new List<Action>();
    private static bool ActionToExecuteOnMainThread = false;

    private void Update()
    {
        if (ActionToExecuteOnMainThread)
        {
            ActionCopiedOnMainThread.Clear();
            lock (ActionOnMainThread)
            {
                ActionCopiedOnMainThread.AddRange(ActionOnMainThread);
                ActionOnMainThread.Clear();
                ActionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < ActionCopiedOnMainThread.Count; i++)
            {
                ActionCopiedOnMainThread[i]();
            }
        }
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (ActionOnMainThread)
        {
            ActionOnMainThread.Add(action);
            ActionToExecuteOnMainThread = true;
        }
    }
}