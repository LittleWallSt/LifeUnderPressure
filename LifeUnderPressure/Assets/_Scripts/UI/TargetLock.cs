using UnityEngine;
using UnityEngine.UI;

public class TargetLock : MonoBehaviour
{
    private Vector3 initPos;

    [SerializeField] Canvas canvas;
    [SerializeField] Scanner scanner;

    Image lockImage;


    Color basicColor = Color.white;
    Color activeColor = Color.cyan;



    private RectTransform LockPosition;
    private void Start()
    {
        scanner.lockActive += ActivateLock;
        scanner.targetLock += LockOnTarget;
        LockPosition = GetComponent<RectTransform>();
        lockImage = GetComponent<Image>();
        initPos = Camera.main.ViewportToScreenPoint(new Vector2(0f, 0f));
        LockPosition.anchoredPosition = initPos;
    }


    private void LockOnTarget(Vector3 pos)
    {
        MoveToTarget(pos);
    }

    private void ActivateLock(bool active)
    {
        lockImage.color = active? activeColor : basicColor;
    }

   private void MoveToTarget(Vector3 target)
    {

        Vector2 adjustedPosition = Camera.main.WorldToScreenPoint(target);
        RectTransform mCanvas = canvas.GetComponent<RectTransform>();

        adjustedPosition.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
        adjustedPosition.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;

        Vector2 pos = adjustedPosition - mCanvas.sizeDelta / 2f;

        LockPosition.anchoredPosition = pos;

    }
}
