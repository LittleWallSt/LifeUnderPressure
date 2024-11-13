using System.Collections;
using TMPro;
using UnityEngine;

public class VoicelinesUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI voicelineText;
    [SerializeField] GameObject voicelineBox;
    public static VoicelinesUI Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }


    private void Start()
    {
        ResetBox();
    }

    public void CallVoiceline(Voiceline v)
    {
        ResetBox();
        voicelineBox.SetActive(true);
        StartCoroutine(TypeTextUncapped(v)); 
    }

    private void ResetBox()
    {
        StopAllCoroutines();
        voicelineText.text = "";
        voicelineBox.SetActive(false);
    }

    

    //float charactersPerSecond = 10;

    IEnumerator TypeTextUncapped(Voiceline v)
    {
        int j = 0;
        //if (v.onVoicelineStart == null) yield return null;
        foreach (string line in v.voicelines)
        {
            if (v.onVoicelineStart!=null && v.onVoicelineStart.Length>0 &&!v.onVoicelineStart[j].IsNull) AudioManager.instance?.PlayOneShot(v.onVoicelineStart[j], 
                Submarine.Instance.transform.position); //?? play voiceline idk
            float timer = 0;
            float interval = 1 / v.charPerSecond; 
            string textBuffer = null;
            char[] chars = line.ToCharArray();
            int i = 0;

            while (i < chars.Length)
            {
                if (timer < Time.deltaTime)
                {
                    textBuffer += chars[i];
                    voicelineText.text = textBuffer;
                    timer += interval;
                    i++;
                }
                else
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }

            j++;
            Debug.Log(j);

        }

        ResetBox();
    }
}
