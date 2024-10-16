using UnityEngine;
using UnityEngine.UI;

public class TargetLock : MonoBehaviour
{
    [SerializeField] private float lockLerpSpeed = 10f;

    private Vector3 initPos;

    [SerializeField] Canvas canvas;
    [SerializeField] Scanner scanner;

    Image lockImage;

    [SerializeField]
    Image[] lockImages;
    [SerializeField] Vector2[] lockImagesInitialPos;
    [SerializeField] Vector2[] desiredPos;

    Color inRange = Color.red;
    Color scanning = Color.green;


    Color currentColor;



    private RectTransform LockPosition;

    [SerializeField] RectTransform[] LockPositionsRect;
    private void Start()
    {
        scanner.lockActive += ActivateLock;
        scanner.targetLock += LockOnTarget;
        scanner.updateScanner += DecreaseZoom;
        scanner.resetScannerLock += ResetLockPos;
        LockPosition = GetComponent<RectTransform>();
        lockImage = GetComponent<Image>();
        initPos = Camera.main.ViewportToScreenPoint(new Vector2(0f, 0f));
        LockPosition.anchoredPosition = initPos;

        if (lockImages.Length > 0 && lockImagesInitialPos.Length > 0)
        {
            for (int i = 0; i < lockImages.Length; i++)
            {
                LockPositionsRect[i] = lockImages[i].gameObject.GetComponent<RectTransform>();
                LockPositionsRect[i].anchoredPosition = lockImagesInitialPos[i];
            }
        }
    }


    private void LockOnTarget(Vector3 pos)
    {
        MoveToTarget(pos);
    }

    private void ActivateLock(ScannerState state)
    {

        switch (state)
        {
            case ScannerState.Inactive:
                currentColor = Color.white;
                break;
            case ScannerState.InRange:
                currentColor = inRange; break;
            case ScannerState.Scanning:
                currentColor = scanning; break;
        }

        foreach(Image image in lockImages)
        {
            image.color= currentColor;  
        }
    }

    private void DecreaseZoom(float time)
    {
        for (int i = 0; i < LockPositionsRect.Length; i++)
        {
            float x = (lockImagesInitialPos[i].x + desiredPos[i].x)*(1-time);
            float y = (lockImagesInitialPos[i].y + desiredPos[i].y) * (1 - time);
            LockPositionsRect[i].anchoredPosition = new Vector2(x, y);

        }
    }

    private void ResetLockPos()
    {
        LockPosition.anchoredPosition = new Vector2(0, 0);
        for (int i = 0; i < LockPositionsRect.Length; i++)
        {
            LockPositionsRect[i].anchoredPosition = lockImagesInitialPos[i];
        }
    }

    private void MoveToTarget(Vector3 target)
    {

        Vector2 adjustedPosition = Camera.main.WorldToScreenPoint(target);
        RectTransform mCanvas = canvas.GetComponent<RectTransform>();

        adjustedPosition.x *= mCanvas.rect.width / (float)Camera.main.pixelWidth;
        adjustedPosition.y *= mCanvas.rect.height / (float)Camera.main.pixelHeight;

        Vector2 pos = adjustedPosition - mCanvas.sizeDelta / 2f;

        LockPosition.anchoredPosition = Vector2.Lerp(LockPosition.anchoredPosition, pos, lockLerpSpeed * Time.deltaTime); 
    }
}
