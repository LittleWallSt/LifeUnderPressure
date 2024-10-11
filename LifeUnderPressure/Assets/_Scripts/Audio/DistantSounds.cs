using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantSounds : MonoBehaviour
{
    private float shouldPlaySoundTimer = 0f;

    [Header("Timer")]
    [SerializeField]
    private float timeBetweenSounds = 75f;
    [SerializeField]
    private float startTimeBetweenSounds = 20f;

    [Header("X Coordinates")]
    [SerializeField]
    private int minPositiveX = 35;
    [SerializeField]
    private int maxPositiveX = 40;
    [SerializeField]
    private int minNegativeX = -35;
    [SerializeField]
    private int maxNegativeX = -40;

    [Header("Z Coordinates")]
    [SerializeField]
    private int minPositiveZ = 35;
    [SerializeField]
    private int maxPositiveZ = 40;
    [SerializeField]
    private int minNegativeZ = -35;
    [SerializeField]
    private int maxNegativeZ = -40;

    private void Start()
    {
        shouldPlaySoundTimer = startTimeBetweenSounds;
    }

    private void Update()
    {
        PlayDistantSound(FMODEvents.instance.SFX_Whale, transform.position + ChooseRandomPosition());
    }

    private void PlayDistantSound(EventReference sound, Vector3 worldPos)
    {
        if (shouldPlaySoundTimer <= 0f)
        {
            AudioManager.instance.PlayOneShot(sound, worldPos);
            shouldPlaySoundTimer = timeBetweenSounds;
        }
        else
            shouldPlaySoundTimer -= Time.deltaTime;
    }
    
    private Vector3 ChooseRandomPosition()
    {
        if (shouldPlaySoundTimer > 0f)
            return Vector3.zero;

        int randomPositiveX = Random.Range(minPositiveX, maxPositiveX + 1);
        int randomPositiveZ = Random.Range(minPositiveZ, maxPositiveZ + 1);
        int randomNegativeX = Random.Range(minNegativeX, maxNegativeX + 1);
        int randomNegativeZ = Random.Range(minNegativeZ, maxNegativeZ + 1);

        int[] possibleChoices;

        possibleChoices = new int[4];
        possibleChoices[0] = randomPositiveX;
        possibleChoices[1] = randomPositiveZ;
        possibleChoices[2] = randomNegativeX;
        possibleChoices[3] = randomNegativeZ;

        Vector3 chosenPosition = new Vector3(possibleChoices[Random.Range(0, possibleChoices.Length)], 0f, 
                                            possibleChoices[Random.Range(0, possibleChoices.Length)]);
        
        //Debug.Log(chosenPosition);
        return chosenPosition;

        /*// new code >>
        // Decide if in front or in the back
        bool shouldBePositive = true;

        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
            shouldBePositive = true;
        else if (randomChoice == 1)
            shouldBePositive = false;

        Vector3 chosenPositionNew;

        if (shouldBePositive)
            chosenPositionNew = new Vector3(possibleChoices[Random.Range(0, 2)], 0f, possibleChoices[Random.Range(0, 2)]);
        else
            chosenPositionNew = new Vector3(possibleChoices[Random.Range(2, 4)], 0f, possibleChoices[Random.Range(2, 4)]);
        // new code << */
    }
}