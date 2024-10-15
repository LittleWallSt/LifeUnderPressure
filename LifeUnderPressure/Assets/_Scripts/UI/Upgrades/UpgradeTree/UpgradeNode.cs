using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public SkillNode skillNode;
    [TextArea(5, 5)]
    [SerializeField] string description;

    [SerializeField] TextMeshProUGUI level;
    

    private void Start()
    {
        AssignUpgrade();
        //UpdateNode();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UpgradeTreeCanvas.Instance!=null && !UpgradeTreeCanvas.Instance.hoverActive)
        {
            
            UpgradeTreeCanvas.Instance.SetHoverMenu(true, description, 
                getRequirementText(skillNode), gameObject.GetComponent<RectTransform>());
            UpgradeTreeCanvas.Instance.hoverActive = true;


        }
    }

    private string getRequirementText(SkillNode skillNode)
    {
        SubmarineUpgrade temp = skillNode.GetUpgrade();
        if (temp==null)
        {
            Debug.Log("Cant get upgrade");
            return "";
        }
        string reqText = "Next level requirement: ";
        if (temp.Level >= temp.MaxLevel)
        { 
            reqText = "Max level reached.";
            return reqText;
        }
        else if (!skillNode.isUnlocked && skillNode.prerequisites!=null)
        {
            if (skillNode.LockedBehindQuest && !skillNode.isUnlocked)
                reqText += "Upgrade locked. " + '\n';
            foreach (var req in skillNode.prerequisites)
            {
                reqText += req.skillNode.upgradeType.ToString() + " Lv:" + req.skillNode.requiredLevelForUnlock + ", " + '\n';
            }
        }
        
        reqText += temp.GetLevelUpgradeCost().ToString() + " XP";
        return reqText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UpgradeTreeCanvas.Instance != null && UpgradeTreeCanvas.Instance.hoverActive)
        {
            UpgradeTreeCanvas.Instance.SetHoverMenu(false);
            UpgradeTreeCanvas.Instance.hoverActive = false;
        } 
    }

    

    public void UpdateNode()
    {
        level.text = "Lv: " + skillNode.GetUpgrade().Level.ToString();
        //skillNode.GetUpgrade();
    }


    public void AssignUpgrade()
    {
        SubmarineUpgrade[] upgrades = FindObjectsOfType<SubmarineUpgrade>();
        if (upgrades.Length == 0)
        {
            Debug.Log("no upgrades");
            return;
        }
        foreach(var _upgrade in upgrades)
        { 
            if (skillNode.upgradeType == _upgrade.type) skillNode.SetUpgrade(_upgrade);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        Submarine submarine = Submarine.Instance;
        if (!skillNode.LockedBehindQuest) UnlockUpgrade(true);
        if (!skillNode.isUnlocked) return;
        bool upgraded = skillNode.GetUpgrade().TryUpgradeLevel(submarine.Money);
        if (!upgraded) return;

        UpdateNode();
        UnlockUpgrade(false);

        UpgradeTreeCanvas.Instance.SetHoverMenu(true, description,
                getRequirementText(skillNode), gameObject.GetComponent<RectTransform>());

        // Janko >> 
        AudioManager.instance.PlayOneShot(FMODEvents.instance.upgradeFX, Camera.main.transform.position);
        // Janko <<


    }


    public bool CanUnlock(SkillNode node)
    {
        if (node.prerequisites != null || node.prerequisites.Length == 0) return true;
        foreach (var prerequisite in node.prerequisites)
        {
            if (!prerequisite.skillNode.isUnlocked || 
                prerequisite.skillNode.GetUpgrade().Level < prerequisite.skillNode.requiredLevelForUnlock)
            {
                return false; 
            }
        }
        return true;
    }


    private void UnlockUpgrade(bool state)
    {
        if (CanUnlock(skillNode))
        {
            skillNode.isUnlocked = state;
        }
    }

    public void UnlockQuestNode()
    {
        Debug.Log("unlockee");
        UnlockUpgrade(true);
        
    }

   

    
}

[Serializable]
public class SkillNode
{
    public UpgradeType upgradeType;
    SubmarineUpgrade upgrade;
    public bool isUnlocked = false;  
    public bool LockedBehindQuest = false;
    public int requiredLevelForUnlock;  // Level required to unlock the next node
    public UpgradeNode[] prerequisites;  

    public void SetUpgrade(SubmarineUpgrade up)
    {
        this.upgrade = up;
    }

    public SubmarineUpgrade GetUpgrade() { return upgrade; }
}

public enum UpgradeType
{
    Scanner,
    Hull, 
    Lights,
    Boost,
    Motor,
    None
}
