using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI : MonoBehaviour
{
    public float timerLength;
    public float timeLeft;
    public float scaleSpeed;
    public GameObject timer;
    public AudioClip Beep;
    public TextMeshProUGUI timerText;
    private AudioSource so;
    public GameObject P1Parent;
    public GameObject P2Parent;
    public GameObject P3Parent;
    public GameObject P4Parent;
    private TextMeshProUGUI[] P1Texts;
    private TextMeshProUGUI[] P2Texts;
    private TextMeshProUGUI[] P3Texts;
    private TextMeshProUGUI[] P4Texts;
    public GameObject MidRoundScoreboard;
    private CanvasGroup MRScoreCG;
    public bool FadingIn;
    public bool FadingOut;
    public bool counting;
    public float f;
    public GameObject CountdownTimer;
    private TextMeshProUGUI CountdownTMP;
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        MRScoreCG = MidRoundScoreboard.GetComponent<CanvasGroup>();
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        timerText = timer.GetComponent<TextMeshProUGUI>();
        Screen.SetResolution(1920, 1080, true);
        P1Texts = P1Parent.GetComponentsInChildren<TextMeshProUGUI>();
        P2Texts = P2Parent.GetComponentsInChildren<TextMeshProUGUI>();
        P3Texts = P3Parent.GetComponentsInChildren<TextMeshProUGUI>();
        P4Texts = P4Parent.GetComponentsInChildren<TextMeshProUGUI>();
        CountdownTMP = CountdownTimer.GetComponent<TextMeshProUGUI>();
        gm = GetComponent<GameManager>();

        if (GetComponent<Generation>().PlayerNum == 2)
        {
            timerLength = 25f;
        }
        else if (GetComponent<Generation>().PlayerNum == 3)
        {
            timerLength = 30f;
        }
        else if (GetComponent<Generation>().PlayerNum == 4)
        {
            timerLength = 40f;
        }
        timeLeft = timerLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) UpdateScores();
    }
    private void FixedUpdate()
    {
        if (counting)
        {
            timeLeft -= 0.02f;
            timerText.text = timeLeft.ToString("n2");
            timeLeft = (float)Math.Truncate(100f * timeLeft) / 100f;
        }

        if ((timeLeft % 10 == 0 || timeLeft == 5 || timeLeft == 3 || timeLeft == 2 || timeLeft == 1) && counting)
        {
            so.PlayOneShot(Beep);
        }
        if (timeLeft < 1)
        {
            timerText.color = new Color(1, 0, 0, 1);
        }
        else if (timeLeft < 2)
        {
            timerText.color = new Color(1, 0.5f, 0, 1);
        }
        else if (timeLeft < 3)
        {
            timerText.color = new Color(1, 0.75f, 0, 1);
        }
        else if (timeLeft < 5)
        {
            timerText.color = new Color(1, 1, 0, 1);
        }
        else
        {
            timerText.color = new Color(1, 1, 1, 1);
        }
        if (timeLeft <= 0 && counting)
        {
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject tank in tanks)
            {
                StartCoroutine(tank.GetComponent<PlayerController>().Die());
            }
            counting = false;
            UpdateScores();
        }
        if (FadingIn && f <= 1f)
        {
            f += 2f * 0.02f;
            timerText.alpha = 1 - f;
            MRScoreCG.alpha = f; 
        }
        else if (FadingIn && f > 1f)
        {
            FadingIn = false;
        }
        if (FadingOut && f >= 0f)
        {
            f -= 2f * 0.02f;
            timerText.alpha = 1 - f;
            MRScoreCG.alpha = f;
        }
        else if (FadingOut && f < 0f)
        {
            FadingOut = false;
        }
    }
    public void SetTimer(float PlayerNum)
    {
        if (PlayerNum == 2)
        {
            timerLength = 90f;
        }
        else if (PlayerNum == 3)
        {
            timerLength = 120f;
        }
        else if (PlayerNum == 4)
        {
            timerLength = 150f;
        }
        timeLeft = timerLength;
    }
    public void StartTimer()
    {
        counting = true;
    }
    public void UpdateScores()
    {
        P1Texts[0].text = gm.P1Kills.ToString();
        P1Texts[1].text = gm.P1Deaths.ToString();
        P1Texts[2].text = gm.P1KD.ToString("n1");
        P1Texts[3].text = gm.P1Score.ToString();
        P2Texts[0].text = gm.P2Kills.ToString();
        P2Texts[1].text = gm.P2Deaths.ToString();
        P2Texts[2].text = gm.P2KD.ToString("n1");
        P2Texts[3].text = gm.P2Score.ToString();
        P3Texts[0].text = gm.P3Kills.ToString();
        P3Texts[1].text = gm.P3Deaths.ToString();
        P3Texts[2].text = gm.P3KD.ToString("n1");
        P3Texts[3].text = gm.P3Score.ToString();
        P4Texts[0].text = gm.P4Kills.ToString();
        P4Texts[1].text = gm.P4Deaths.ToString();
        P4Texts[2].text = gm.P4KD.ToString("n1");
        P4Texts[3].text = gm.P4Score.ToString();
    }
    public IEnumerator Countdown()
    {
        CountdownTMP.text = "3";
        //so.PlayOneShot(Beep);
        yield return new WaitForSeconds(0.95f);
        CountdownTMP.text = "";
        yield return new WaitForSeconds(0.05f);
        CountdownTMP.text = "2";
        //so.PlayOneShot(Beep);
        yield return new WaitForSeconds(0.95f);
        CountdownTMP.text = "";
        yield return new WaitForSeconds(0.05f);
        CountdownTMP.text = "1";
        //so.PlayOneShot(Beep);
        yield return new WaitForSeconds(0.95f);
        CountdownTMP.text = "";
        yield return new WaitForSeconds(0.05f);
        CountdownTMP.text = "Start";
        //so.PlayOneShot(Beep);
        yield return new WaitForSeconds(0.95f);
        CountdownTMP.text = "";
        yield return null;
    }
}
