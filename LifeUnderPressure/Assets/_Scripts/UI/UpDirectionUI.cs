using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpDirectionUI : MonoBehaviour
{
    [SerializeField] private Image image = null;
    [SerializeField] private Sprite horizonSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite fullUpSprite;

    [SerializeField] private float upDot = 0.2f;
    [SerializeField] private float fullUpDot = 0.7f;

    private void FixedUpdate()
    {
        float dot = Vector3.Dot(transform.root.forward, Vector3.up);

        if (dot > fullUpDot)
        {
            image.sprite = fullUpSprite;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dot > upDot)
        {
            image.sprite = upSprite;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dot < -fullUpDot)
        {
            image.sprite = fullUpSprite;
            transform.localScale = new Vector3(1, -1, 1);
        }
        else if (dot < -upDot)
        {
            image.sprite = upSprite;
            transform.localScale = new Vector3(1, -1, 1);
        }
        else image.sprite = horizonSprite;
        
    }
}
