using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStep : TutorialStep
{
    public float movementThreshold = 1f;
    public float rotationThreshold = 10f;

    Vector3 initialPosition;
    Quaternion initialRotation;

    bool moved;
    bool rotated;

    GameObject player;

    public override void StartStep()
    {
        isCompleted = false;
        player = TutorialUtility.Instance.submarine;
        initialPosition = player.transform.position;
        initialRotation = player.transform.rotation;
        
    }

    public override void CheckForCompleting()
    {
        if (!moved && Vector3.Distance(initialPosition, player.transform.position) > movementThreshold)
        {
            moved = true;
        }

        if (!rotated && Quaternion.Angle(initialRotation, player.transform.rotation) > rotationThreshold)
        {
            rotated = true;
        }

        if (moved && rotated)
        {
            CompleteStep();
        }
    }


}
