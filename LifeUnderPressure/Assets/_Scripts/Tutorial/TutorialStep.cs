using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    public bool isCompleted { get; protected set; }
    public abstract void StartStep();

    public abstract void CheckForCompleting();

    protected void CompleteStep()
    {
        isCompleted = true;
        Debug.Log($"{this.GetType().Name} completed!");
    }
}
