using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WiimoteApi;

public class GameManager : MonoBehaviour
{
    public List<GameObject> Tanks = new List<GameObject>();
    public int P1Deaths;
    public int P2Deaths;
    public int P3Deaths;
    public int P4Deaths;
    public int P1Kills;
    public int P2Kills;
    public int P3Kills;
    public int P4Kills;
    public float P1KD;
    public float P2KD;
    public float P3KD;
    public float P4KD;
    public int P1Score;
    public int P2Score;
    public int P3Score;
    public int P4Score;
    public int P1Shots;
    public int P2Shots;
    public int P3Shots;
    public int P4Shots;
    public int P1Hits;
    public int P2Hits;
    public int P3Hits;
    public int P4Hits;
    public float P1Accuracy;
    public float P2Accuracy;
    public float P3Accuracy;
    public float P4Accuracy;
    public CanvasGroup FadeOutCanvas;
    private int WinScore = 10;
    private bool P1Alive;
    private bool P2Alive;
    private bool P3Alive;
    private bool P4Alive;
    private bool Reloading;
    private int NumAlive;
    private bool Ending;
    private float f = 0f;
    public string WinScreen;

    public List<string> P1Data = new List<string>();
    public List<string> P2Data = new List<string>();
    public List<string> P3Data = new List<string>();
    public List<string> P4Data = new List<string>();

    private Wiimote PauseMote;
    public bool Paused;
    public GameObject PauseMenu;

    public AudioSource RecordPlayer;
    // Start is called before the first frame update
    void Start()
    {
        RecordPlayer.volume = DataScript.Volume / 2;
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (!Reloading)
        {
            for (int i = 1; i <= 4; i++)
            {
                NumAlive = GetComponent<Generation>().PlayerNum;
                P1Alive = Tanks[0].GetComponent<PlayerController>().Alive;
                if (!P1Alive) NumAlive--;
                P2Alive = Tanks[1].GetComponent<PlayerController>().Alive;
                if (!P2Alive) NumAlive--;
                if (GetComponent<Generation>().PlayerNum > 2) P3Alive = Tanks[2].GetComponent<PlayerController>().Alive;
                if (GetComponent<Generation>().PlayerNum > 3) P4Alive = Tanks[3].GetComponent<PlayerController>().Alive;
            }
            if (NumAlive == 1)
            {
                if (P1Alive) P1Score++;
                if (P2Alive) P2Score++;
                if (P3Alive) P3Score++;
                if (P4Alive) P4Score++;
                StartCoroutine(NewRound());
            }
            if (NumAlive <= 0)
            {
                StartCoroutine(NewRound());
            }
            #region KD Statements
            if (P1Deaths != 0)
            {
                P1KD = P1Kills / P1Deaths;
            }
            else
            {
                P1KD = P1Kills / (P1Deaths + 1);
            }
            if (P2Deaths != 0)
            {
                P2KD = P2Kills / P2Deaths;
            }
            else
            {
                P2KD = P2Kills / (P2Deaths + 1);
            }
            if (P3Deaths != 0)
            {
                P3KD = P3Kills / P3Deaths;
            }
            else
            {
                P3KD = P3Kills / (P3Deaths + 1);
            }
            if (P4Deaths != 0)
            {
                P4KD = P4Kills / P4Deaths;
            }
            else
            {
                P4KD = P4Kills / (P4Deaths + 1);
            }
            #endregion
            #region Accuracy Statements
            if (P1Shots != 0)
            {
                P1Accuracy = (P1Hits / P1Shots) * 100f;
            }
            else
            {
                P1Accuracy = (P1Hits / 1) * 100f;
            }
            if (P2Shots != 0)
            {
                P2Accuracy = (P2Hits / P2Shots) * 100f;
            }
            else
            {
                P2Accuracy = (P2Hits / 1) * 100f;
            }
            if (P3Shots != 0)
            {
                P3Accuracy = (P3Hits / P3Shots) * 100f;
            }
            else
            {
                P3Accuracy = (P3Hits / 1) * 100f;
            }
            if (P4Shots != 0)
            {
                P4Accuracy = (P4Hits / P4Shots) * 100f;
            }
            else
            {
                P4Accuracy = (P4Hits / 1) * 100f;
            }
            #endregion
            if (P1Score >= WinScore || P2Score >= WinScore || P3Score >= WinScore || P4Score >= WinScore) StartCoroutine(EndGame());
        }
        if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(NewRound());
        if (Ending)
        {
            GetComponent<UI>().timerText.alpha = 0;
            f += 2f * Time.deltaTime;
            FadeOutCanvas.alpha = f;
        }
        if (Paused)
        {
            
            int ret;
            do
            {
                ret = PauseMote.ReadWiimoteData();
            } while (ret > 0);
            
            if (PauseMote.Button.a || Input.GetKeyDown(KeyCode.Escape))
            {
                Unpause();
            }
            if (PauseMote.Button.b || Input.GetKeyDown(KeyCode.F1))
            {
                Quit();
            }
        }
    }
    public void Quit()
    {
        WinScreen = "MainMenu";
        Debug.LogWarning(WinScreen);
        StartCoroutine(EndGame());
    }
    public IEnumerator Pause(int PlayerID)
    {
        Debug.Log("Reached Pause()");
        PauseMenu.SetActive(true);
        PauseMote = WiimoteManager.Wiimotes[PlayerID - 1];
        Time.timeScale = 0;
        Debug.Log("Reached end of Pause()");
        yield return new WaitForSecondsRealtime(1);
        Paused = true;
        yield return null;
    }
    public void Unpause()
    {
        PauseMenu.SetActive(false);
        Paused = false;
        PauseMote = null;
        Time.timeScale = 1;
    }
    IEnumerator EndGame()
    {
        Ending = true;
        foreach (GameObject tank in Tanks)
        {
            tank.GetComponent<PlayerController>().Active = false;
        }

        #region setLists

        P1Data.Add(P1Kills.ToString());
        P1Data.Add(P1Deaths.ToString());
        P1Data.Add(P1KD.ToString("n1"));
        P1Data.Add(P1Shots.ToString());
        P1Data.Add(P1Hits.ToString());
        P1Data.Add((P1Shots - P1Hits).ToString());
        P1Data.Add(P1Accuracy.ToString());
        P1Data.Add(P1Score.ToString());

        P2Data.Add(P2Kills.ToString());
        P2Data.Add(P2Deaths.ToString());
        P2Data.Add(P2KD.ToString("n1"));
        P2Data.Add(P2Shots.ToString());
        P2Data.Add(P2Hits.ToString());
        P2Data.Add((P2Shots - P1Hits).ToString());
        P2Data.Add(P2Accuracy.ToString());
        P2Data.Add(P2Score.ToString());

        P3Data.Add(P3Kills.ToString());
        P3Data.Add(P3Deaths.ToString());
        P3Data.Add(P3KD.ToString("n1"));
        P3Data.Add(P3Shots.ToString());
        P3Data.Add(P3Hits.ToString());
        P3Data.Add((P3Shots - P1Hits).ToString());
        P3Data.Add(P3Accuracy.ToString());
        P3Data.Add(P3Score.ToString());

        P4Data.Add(P4Kills.ToString());
        P4Data.Add(P4Deaths.ToString());
        P4Data.Add(P4KD.ToString("n1"));
        P4Data.Add(P4Shots.ToString());
        P4Data.Add(P4Hits.ToString());
        P4Data.Add((P4Shots - P1Hits).ToString());
        P4Data.Add(P4Accuracy.ToString());
        P4Data.Add(P4Score.ToString());

        DataScript.P1Data = P1Data;
        DataScript.P2Data = P2Data;
        DataScript.P3Data = P3Data;
        DataScript.P4Data = P4Data;

        DataScript.PlayerNum = GetComponent<Generation>().PlayerNum;

        #endregion

        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene(WinScreen);
        yield return null;
    }
    IEnumerator NewRound()
    {
        Reloading = true;
        GetComponent<UI>().counting = false;
        Tanks.Clear();
        GetComponent<UI>().UpdateScores();
        GetComponent<UI>().FadingIn = true;
        yield return new WaitForSeconds(1f);
        GetComponent<UI>().SetTimer(GetComponent<Generation>().PlayerNum);
        yield return StartCoroutine(GetComponent<Generation>().Generate());
        Debug.Log(Tanks.Count);
        foreach (GameObject tank in Tanks)
        {
            tank.GetComponent<PlayerController>().Active = false;
        }

        //Play tank animation
        GetComponent<UI>().FadingOut = true;
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(GetComponent<UI>().Countdown());

        Reloading = false;

        foreach (GameObject tank in Tanks)
        {
            tank.GetComponent<PlayerController>().Active = true;
        }

        GetComponent<UI>().StartTimer();
        
        yield return null;
    }
    IEnumerator StartGame()
    {
        Reloading = true;
        GetComponent<UI>().SetTimer(GetComponent<Generation>().PlayerNum);
        yield return StartCoroutine(GetComponent<Generation>().Generate());

        Debug.Log(Tanks.Count);
        foreach (GameObject tank in Tanks)
        {
            tank.GetComponent<PlayerController>().Active = false;
        }

        GetComponent<UI>().f = 1f;
        yield return new WaitForSeconds(1f);
        GetComponent<UI>().FadingOut = true;
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(GetComponent<UI>().Countdown());

        Reloading = false;

        foreach (GameObject tank in Tanks)
        {
            tank.GetComponent<PlayerController>().Active = true;
        }

        GetComponent<UI>().StartTimer();

        yield return null;
    }
}
