using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SonarRadar : MonoBehaviour
{
    [Header("Calibration values")]
    [SerializeField] float tickTime = 8f;
    [SerializeField] int fishLayer = 8;
    int upgradeLevel;

    [Header("Assignees")]
    [SerializeField] float[] maxDistanceRadius;
    [SerializeField] GameObject submarine;
    [SerializeField] ImageAnimation imageAnim;

    

    [SerializeField] GameObject scanArea;
    LayerMask fishLayerMask;

    float timeLeft;
    int numberOfSprites;
    int framesPerSprite;
    int frameIndex;
    bool SonarAnimation = false;

    Collider[] fishes;
    Dictionary<int, Collider> timeFrames = new Dictionary<int, Collider>();


    public Action<float, Vector3> sonarBeep;

    private void Start()
    {
        fishLayerMask = (1 << fishLayer);
        if (maxDistanceRadius.Length>0) 
            ChangeMaxDistance(maxDistanceRadius[0]);

        if(imageAnim!=null)
        {
            numberOfSprites = imageAnim.sprites.Length;
            framesPerSprite = imageAnim.framesPerSprite;
        }
    }

    private void FixedUpdate()
    {
        if (!SonarAnimation)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                timeLeft = tickTime;
                fishes = fishInArea();
                frameIndex = imageAnim.index*framesPerSprite;
                SonarAnimation = true;
                foreach (Collider fish in fishes)
                {
                    Vector3 angle;
                    int sIndex;
                    fishAngle(fish, out angle, out sIndex);
                    if (!timeFrames.ContainsKey(sIndex * framesPerSprite))
                    {
                        timeFrames.Add(sIndex * framesPerSprite, fish);
                    }

                }

            }
        } else
        {
            SonarSignal();
        }

    }

    private void SonarSignal()
    {
        if (timeFrames.Count == 0)
        {
            SonarAnimation = false;
        }

        if (frameIndex > framesPerSprite * numberOfSprites) frameIndex = 0;

        //Debug.Log(timeFrames.Count);
        if (timeFrames.ContainsKey(frameIndex))
        {
            Vector3 angle;
            int sIndex;
            fishAngle(timeFrames[frameIndex], out angle, out sIndex);
            sonarBeep.Invoke(DistPercentage(timeFrames[frameIndex]), angle);
            timeFrames.Remove(frameIndex);
        }
        frameIndex++;
    }
    
    

    private Collider[] fishInArea()
    {
        return Physics.OverlapBox(scanArea.transform.position, scanArea.transform.localScale / 2,
            Quaternion.identity, fishLayerMask);
    }

    private void ChangeMaxDistance(float maxDistRad)
    {
        scanArea.transform.localScale = new Vector3(maxDistRad*2, maxDistRad*2, maxDistRad*2);
    }


    public float DistPercentage(Collider fish)
    {
        float dist = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(fish.transform.position.x, 0, fish.transform.position.z)); 

        if (maxDistanceRadius.Length == 0) return 1;
        return dist / maxDistanceRadius[maxDistanceRadius.Length - 1];

    }

    public Vector3 fishDirection(Collider fish)
    {
        return (fish.transform.position - submarine.transform.position).normalized;
    }

    public void fishAngle(Collider fish, out Vector3 angle, out int spriteIndex)
    {
        Vector3 directionToFish = fish.transform.position - submarine.transform.position;

        Vector3 submarineForward = submarine.transform.forward;
        Vector2 flatDirectionToFish = new Vector2(directionToFish.x, directionToFish.z);
        Vector2 flatSubmarineForward = new Vector2(submarineForward.x, submarineForward.z);
        float angleBetween = Vector2.SignedAngle(flatSubmarineForward, flatDirectionToFish);

        float radians = angleBetween * Mathf.Deg2Rad;
        Vector2 sonarPosition = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));

        angle = new Vector3(-sonarPosition.x, 0, sonarPosition.y);
        spriteIndex = (int) ((-angleBetween>0? -angleBetween : (360 - angleBetween)) / 
            (360 / numberOfSprites));
        //Debug.Log(-angleBetween + " : " + spriteIndex);
    }
    
}
