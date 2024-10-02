using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{

    public Sprite[] sprites;
    public int framesPerSprite = 6;
    public bool loop = true;
    public bool destroyOnEnd = false;

    public int index = 0;
    private Image image;
    private int frame = 0;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if (!loop && index == sprites.Length)
            return;
        frame++;
        if (frame < framesPerSprite)
            return;
        image.sprite = sprites[index];
        frame = 0;
        index++;
        if (index >= sprites.Length)
        {
            if (loop) 
                index = 0;
                // Janko >>
                AudioManager.instance.PlayOneShot(FMODEvents.instance.sonarSFX, Camera.main.transform.position);
                // Janko <<
            }
                
            if (destroyOnEnd)
                Destroy(gameObject);
        }
    }
}

//taken from: https://gist.github.com/almirage/e9e4f447190371ee6ce9
