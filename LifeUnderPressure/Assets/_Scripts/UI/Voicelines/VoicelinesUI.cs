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
        if (v.delay <= 0)
        {
            ResetBox();
            voicelineBox.SetActive(true);
            StartCoroutine(TypeTextUncapped(v));
        }
        else
        {
            Debug.Log("supposedly run");
            StartCoroutine(RunCoroutinesInSequence(v)); 
        }
    }

    private void ResetBox()
    {
        StopAllCoroutines();
        voicelineText.text = "";
        voicelineBox.SetActive(false);
    }

    IEnumerator RunCoroutinesInSequence(Voiceline v)
    {
        yield return new WaitForSecondsRealtime(v.delay);
        ResetBox();
        voicelineBox.SetActive(true);
        yield return StartCoroutine(TypeTextUncapped(v));
    }


    IEnumerator TypeTextUncapped(Voiceline v)
    {
        
        int j = 0;
        //if (v.onVoicelineStart == null) yield return null;
        foreach (string line in v.voicelines)
        {
            if (v.onVoicelineStart != null && v.onVoicelineStart.Length > j && !v.onVoicelineStart[j].IsNull) //AudioManager.instance?.PlayOneShot(v.onVoicelineStart[j], 
                //Submarine.Instance.transform.position); //?? play voiceline idk 
                AudioManager.instance?.PlayVoiceline(v.onVoicelineStart[j]); 
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
            yield return new WaitForSecondsRealtime(0.6f);

        }

        ResetBox();
    }
}
