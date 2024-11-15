using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "Voiceline", menuName = "ScriptableObjects/Voicelines", order = 2)]
public class Voiceline : ScriptableObject
{
    public string voicelineName;
    [TextArea(5, 5)]
    public string[] voicelines;

    public float charPerSecond = 10;

    [SerializeField] private EventReference[] OnVoicelineStart;

    public EventReference[] onVoicelineStart => OnVoicelineStart;

}
