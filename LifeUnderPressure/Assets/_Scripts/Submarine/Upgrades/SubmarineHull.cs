using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SubmarineHull : SubmarineUpgrade
{
    [Header("Hull")]
    [SerializeField] private float[] upgrades = null;

    [Header("Pop Up")]
    [SerializeField] private CanvasGroup _upgradeText;

    private Submarine submarine;

    private float text_cooldown = 3f;
    private float text_fadeDuration = 3f;

    [ContextMenu("SU_EditorSetup")]
    protected override void SU_EditorSetup()
    {
        base.SU_EditorSetup();

        if (upgrades.Length != maxLevel + 1)
        {
            upgrades = new float[maxLevel + 1];
        }
    }
    public override void Init(params object[] setList)
    {
        base.Init(setList);

        foreach (object setObj in setList)
        {
            if (setObj as Submarine)
            {
                this.submarine = (Submarine)setObj;
            }
        }

        submarine.SetThicknessOfHull(upgrades[level]);
        _upgradeText.enabled = false;
        _upgradeText.gameObject.SetActive(false);
    }
    private void Update()
    {
        // Debug
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            submarine.SetThicknessOfHull(1f);
        }
#endif
    }
    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        submarine.SetThicknessOfHull(upgrades[level]);

        UpgradeText();
    }

    public void UpgradeText()
    {
        _upgradeText.gameObject.SetActive(true);

        _upgradeText.alpha = 1.0f;

        StartCoroutine(FadeOutAfterCooldown());
    }

    IEnumerator FadeOutAfterCooldown()
    {
        // Wait for the cooldown period before starting the fade
        yield return new WaitForSeconds(text_cooldown);


        float elapsedTime = 0f;

        while (elapsedTime < text_fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the new alpha based on how much time has passed
            _upgradeText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / text_fadeDuration);

            yield return null;
        }

        _upgradeText.alpha = 0f;
        _upgradeText.gameObject.SetActive(false);

    }
}
