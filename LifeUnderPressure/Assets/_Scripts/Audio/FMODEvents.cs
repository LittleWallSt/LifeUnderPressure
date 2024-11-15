using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    /*[field: Header("VO Tutorial Lines")]
    [field: SerializeField] 
    public EventReference voiceLine_Tutorial_01 { get; private set; }*/

    [field: Header("Atmosphere SFX")]
    [field: SerializeField]
    public EventReference ambienceToPlay { get; private set; }

    [field: SerializeField]
    public EventReference AMB_Random_Ambient_SFX { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Whale { get; set; }

    [field: Header("Upgrades")]
    [field: SerializeField]
    public EventReference upgradeFX { get; private set; }

    [field: Header("Docking Station")]
    [field: SerializeField]
    public EventReference dockingSFX { get; private set; }

    [field: Header("Scanning")]
    [field: SerializeField]
    public EventReference scannedNotificationSFX { get; private set; }

    [field: SerializeField]
    public EventReference scanningSFX { get; private set; }

    [field: Header("Submarine SFX")]
    [field: SerializeField]
    public EventReference propellerSFX { get; private set; }

    [field: SerializeField]
    public EventReference sonarSFX { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Warning { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Collision {  get; private set; }

    [field: SerializeField]
    public EventReference SFX_Implosion { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Death {  get; private set; }

    [field: SerializeField]
    public EventReference SFX_Cracking { get; private set; }

    [field: SerializeField]
    public EventReference SFX_Lightswitch { get; private set; }

    [field: Header("Music Tracks")]
    [field: SerializeField]
    public EventReference musicToPlay { get; set; }

    [field: SerializeField]
    public EventReference menuMusic { get; set; }

    [field: Header("UI")]
    [field: SerializeField]
    public EventReference SFX_UI_Click { get; private set; }

    [field: SerializeField]
    public EventReference SFX_UI_Click_Fish { get; private set; }

    [field: SerializeField]
    public EventReference SFX_UI_Hover { get; private set; }

    [field: Header("Misc")]
    [field: SerializeField]
    public EventReference SFX_Current {  get; private set; }

    [field: Header("VO")]

    [field: Header("Tutorial")]
    [field: SerializeField]
    public EventReference VO_TUT_AddLineHERE { get; private set; }

    [field: Header("MISC")]
    [field: SerializeField]
    public EventReference VO_MISC_GetToWork { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterLastFishSZ { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterLastFishTZ { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterLastFishMZ { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_CaveCollapsingNoIssue { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_CaveCollapsedOut { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_TooDeep_01 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_TooDeep_02 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_TooDeep_03 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterCollision_01 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterCollision_02 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_Start {  get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterDeath_01 { get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterDeath_02 {  get; private set; }

    [field: SerializeField]
    public EventReference VO_MISC_AfterGoodFind { get; private set; }

    [field: Header("Sunlight Zone")]
    [field: SerializeField]
    public EventReference VO_SZ_BlueTang { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_BlueTangDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_ChinookSalmon { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_ChinookSalmonDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_Clownfish { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_ClownfishDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_GreenTurtle { get; private set; }

    [field: SerializeField]
    public EventReference VO_SZ_GreenTurtleDesc { get; private set; }

    [field: Header("Twilight Zone")]
    [field: SerializeField]
    public EventReference VO_TZ_BlueWhale { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_BlueWhaleDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_FireflySquid { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_FireflySquidDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_GiantOctopus { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_GiantOctopusDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_SpiderCrab { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_SpiderCrabDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_Sunfish { get; private set; }

    [field: SerializeField]
    public EventReference VO_TZ_SunfishDesc { get; private set; }

    [field: Header("Midnight Zone")]
    [field: SerializeField]
    public EventReference VO_MZ_Anglerfish { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_AnglerfishDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_CookieCutter { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_CookieCutterDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_GulperEel { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_GulperEelDesc { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_DumboOctopus { get; private set; }

    [field: SerializeField]
    public EventReference VO_MZ_DumboOctopusDesc { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }
}