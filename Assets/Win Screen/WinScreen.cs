using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SortAlgorithm;
using WiimoteApi;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public List<string> P1Data = new List<string>();
    public List<string> P2Data = new List<string>();
    public List<string> P3Data = new List<string>();
    public List<string> P4Data = new List<string>();
    public List<int> PlayerScores = new List<int>();
    public List<int> UnsortedPlayerOrder = new List<int>();
    public List<int> PlayerOrder = new List<int>();

    public GameObject FirstPlaceContainer;
    public GameObject SecondPlaceContainer;
    public GameObject ThirdPlaceContainer;
    public GameObject FourthPlaceContainer;
    public Image FirstPlaceBG;
    public Image SecondPlaceBG;
    public Image ThirdPlaceBG;
    public Image FourthPlaceBG;
    public TextMeshProUGUI WinText;
    private TextMeshProUGUI[] FirstPlaceTexts;
    private TextMeshProUGUI[] SecondPlaceTexts;
    private TextMeshProUGUI[] ThirdPlaceTexts;
    private TextMeshProUGUI[] FourthPlaceTexts;

    public int PlayerNum;

    public GameObject tank;
    private MeshRenderer[] tankParts;
    public Material blueMat;
    public Material redMat;
    public Material greenMat;
    public Material yellowMat;

    private AudioSource so;
    public AudioClip Music;
    public AudioClip ButtonSound;
    private bool MusicPlaying;

    private Wiimote Player1Wiimote;

    private bool FadingOut;
    public Image BlackScreen;
    private float f;
    private float startingVolume;
    private int fadeI;
    private bool FadingIn;
    private float FadeSpeed;
    public string target;
    // Start is called before the first frame update
    void Start()
    {
        //FirstPlaceTexts = FirstPlaceContainer.GetComponentsInChildren<TextMeshProUGUI>();
        //SecondPlaceTexts = SecondPlaceContainer.GetComponentsInChildren<TextMeshProUGUI>();
        //ThirdPlaceTexts = ThirdPlaceContainer.GetComponentsInChildren<TextMeshProUGUI>();
        //FourthPlaceTexts = FourthPlaceContainer.GetComponentsInChildren<TextMeshProUGUI>();
        //tankParts = tank.GetComponentsInChildren<MeshRenderer>();
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        startingVolume = so.volume;
        //FetchData();
        //SortOrder();
        //ShowScores();
        BlackScreen.color = new Color(0, 0, 0, 1);
        FadingIn = true;
        f = 1;
        fadeI = 100;
        FadeSpeed = 3;
        //Player1Wiimote = WiimoteManager.Wiimotes[0];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        int ret;
        do
        {
            ret = Player1Wiimote.ReadWiimoteData();
        } while (ret > 0);*/
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            P1Data.Clear();
            P2Data.Clear();
            P3Data.Clear();
            P4Data.Clear();
            PlayerScores.Clear();
            UnsortedPlayerOrder.Clear();
            GenerateData();
            SortOrder();
            ShowScores();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FetchData();
            SortOrder();
            ShowScores();
        }
        if (/*Player1Wiimote.Button.a || */Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(EndScene());
        }
        if (FadingOut)
        {
            f += FadeSpeed * Time.deltaTime;
            fadeI++;
            so.volume = startingVolume * ((100 - fadeI) / 100);
            BlackScreen.color = new Color(0, 0, 0, f);
            Debug.Log(BlackScreen.color);
            if (f >= 100) FadingOut = false;
        }
        if (FadingIn)
        {
            f -= FadeSpeed * Time.deltaTime;
            fadeI--;
            so.volume = startingVolume * ((100 - fadeI) / 100);
            BlackScreen.color = new Color(0, 0, 0, f);
            Debug.Log(BlackScreen.color);
            if (f <= 0) FadingIn = false;
        }
        StartCoroutine(PlayMusic());
    }
    IEnumerator EndScene()
    {
        //Play sound
        so.PlayOneShot(ButtonSound);

        //Fade
        FadingOut = true;

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(target);

        yield return null;
    }
    IEnumerator PlayMusic()
    {
        if (!MusicPlaying)
        {
            MusicPlaying = true;
            so.PlayOneShot(Music);
            yield return new WaitForSeconds(Music.length);
            MusicPlaying = false;
        }
        yield return null;
    }
    void FetchData()
    {
        P1Data.Clear();
        P2Data.Clear();
        P3Data.Clear();
        P4Data.Clear();
        PlayerScores.Clear();
        UnsortedPlayerOrder.Clear();

        PlayerNum = DataScript.PlayerNum;

        P1Data = DataScript.P1Data;
        P2Data = DataScript.P2Data;
        P3Data = DataScript.P3Data;
        P4Data = DataScript.P4Data;

        PlayerScores.Add(Int32.Parse(P1Data[7]));
        PlayerScores.Add(Int32.Parse(P2Data[7]));
        PlayerScores.Add(Int32.Parse(P3Data[7]));
        PlayerScores.Add(Int32.Parse(P4Data[7]));
    }
    void GenerateData()
    {
        PlayerNum = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < 7; i++)
        {
            P1Data.Add(UnityEngine.Random.Range(0, 15).ToString()); //Generate random data for main stats
            P2Data.Add(UnityEngine.Random.Range(0, 15).ToString());
            P3Data.Add(UnityEngine.Random.Range(0, 15).ToString());
            P4Data.Add(UnityEngine.Random.Range(0, 15).ToString());
        }
        for (int i = 0; i < 4; i++)
        {
            int temp = UnityEngine.Random.Range(0, 15); //Generate score
            PlayerScores.Add(temp); //Add score to score list
            if (i == 0) P1Data.Add(temp.ToString()); //Add score to the Player Data
            else if (i == 1) P2Data.Add(temp.ToString());
            else if (i == 2) P3Data.Add(temp.ToString());
            else P4Data.Add(temp.ToString());
        }
    }
    void SortOrder()
    {
        for (int i = 0; i < 4; i++)
        {
            UnsortedPlayerOrder.Add(i + 1);
        }
        Tuple<List<int>, List<int>> SortedLists = ListSort.SortBtoS(PlayerScores, UnsortedPlayerOrder);
        PlayerOrder = SortedLists.Item2;
    }
    void ShowScores()
    {
        for (int i = 0; i < PlayerNum; i++)
        {
            if (PlayerOrder[i] == 1)
            {
                if (i == 0)
                {
                    foreach (MeshRenderer mr in tankParts)
                    {
                        mr.material = blueMat;
                    }
                    FirstPlaceBG.color = new Color(0, 1, 1, 0.176f);
                    WinText.text = string.Format("Player {0} Wins!", PlayerOrder[i]);
                    for (int j = 0; j < P1Data.Count; j++)
                    {
                        if (j != 3) FirstPlaceTexts[j].text = P1Data[j];
                        else FirstPlaceTexts[j].text = P1Data[j];
                    }
                }
                else if (i == 1)
                {
                    SecondPlaceBG.color = new Color(0, 1, 1, 0.176f);
                    for (int j = 0; j < P1Data.Count; j++)
                    {
                        if (j != 3) SecondPlaceTexts[j].text = P1Data[j];
                        else SecondPlaceTexts[j].text = P1Data[j];
                    }
                }
                else if (i == 2)
                {
                    ThirdPlaceBG.gameObject.SetActive(true);
                    ThirdPlaceBG.color = new Color(0, 1, 1, 0.176f);
                    for (int j = 0; j < P1Data.Count; j++)
                    {
                        if (j != 3) ThirdPlaceTexts[j].text = P1Data[j];
                        else ThirdPlaceTexts[j].text = P1Data[j];
                    }
                }
                else if (i == 3)
                {
                    FourthPlaceBG.gameObject.SetActive(true);
                    FourthPlaceBG.color = new Color(0, 1, 1, 0.176f);
                    for (int j = 0; j < P1Data.Count; j++)
                    {
                        if (j != 3) FourthPlaceTexts[j].text = P1Data[j];
                        else FourthPlaceTexts[j].text = P1Data[j];
                    }
                }
            }
            else if (PlayerOrder[i] == 2)
            {
                if (i == 0)
                {
                    foreach (MeshRenderer mr in tankParts)
                    {
                        mr.material = redMat;
                    }
                    FirstPlaceBG.color = new Color(1, 0, 0, 0.176f);
                    WinText.text = string.Format("Player {0} Wins!", PlayerOrder[i]);
                    for (int j = 0; j < P2Data.Count; j++)
                    {
                        if (j != 3) FirstPlaceTexts[j].text = P2Data[j];
                        else FirstPlaceTexts[j].text = P2Data[j];
                    }
                }
                else if (i == 1)
                {
                    SecondPlaceBG.color = new Color(1, 0, 0, 0.176f);
                    for (int j = 0; j < P2Data.Count; j++)
                    {
                        if (j != 3) SecondPlaceTexts[j].text = P2Data[j];
                        else SecondPlaceTexts[j].text = P2Data[j];
                    }
                }
                else if (i == 2)
                {
                    ThirdPlaceBG.gameObject.SetActive(true);
                    ThirdPlaceBG.color = new Color(1, 0, 0, 0.176f);
                    for (int j = 0; j < P2Data.Count; j++)
                    {
                        if (j != 3) ThirdPlaceTexts[j].text = P2Data[j];
                        else ThirdPlaceTexts[j].text = P2Data[j];
                    }
                }
                else if (i == 3)
                {
                    FourthPlaceBG.gameObject.SetActive(true);
                    FourthPlaceBG.color = new Color(1, 0, 0, 0.176f);
                    for (int j = 0; j < P2Data.Count; j++)
                    {
                        if (j != 3) FourthPlaceTexts[j].text = P2Data[j];
                        else FourthPlaceTexts[j].text = P2Data[j];
                    }
                }
            }
            else if (PlayerOrder[i] == 3)
            {
                if (i == 0)
                {
                    foreach (MeshRenderer mr in tankParts)
                    {
                        mr.material = greenMat;
                    }
                    FirstPlaceBG.color = new Color(0, 1, 0, 0.176f);
                    WinText.text = string.Format("Player {0} Wins!", PlayerOrder[i]);
                    for (int j = 0; j < P3Data.Count; j++)
                    {
                        if (j != 3) FirstPlaceTexts[j].text = P3Data[j];
                        else FirstPlaceTexts[j].text = P3Data[j];
                    }
                }
                else if (i == 1)
                {
                    SecondPlaceBG.color = new Color(0, 1, 0, 0.176f);
                    for (int j = 0; j < P3Data.Count; j++)
                    {
                        if (j != 3) SecondPlaceTexts[j].text = P3Data[j];
                        else SecondPlaceTexts[j].text = P3Data[j];
                    }
                }
                else if (i == 2)
                {
                    ThirdPlaceBG.gameObject.SetActive(true);
                    ThirdPlaceBG.color = new Color(0, 1, 0, 0.176f);
                    for (int j = 0; j < P3Data.Count; j++)
                    {
                        if (j != 3) ThirdPlaceTexts[j].text = P3Data[j];
                        else ThirdPlaceTexts[j].text = P3Data[j];
                    }
                }
                else if (i == 3)
                {
                    FourthPlaceBG.gameObject.SetActive(true);
                    FourthPlaceBG.color = new Color(0, 1, 0, 0.176f);
                    for (int j = 0; j < P3Data.Count; j++)
                    {
                        if (j != 3) FourthPlaceTexts[j].text = P3Data[j];
                        else FourthPlaceTexts[j].text = P3Data[j];
                    }
                }
            }
            else if (PlayerOrder[i] == 4)
            {
                if (i == 0)
                {
                    foreach (MeshRenderer mr in tankParts)
                    {
                        mr.material = yellowMat;
                    }
                    FirstPlaceBG.color = new Color(1, 1, 0, 0.176f);
                    WinText.text = string.Format("Player {0} Wins!", PlayerOrder[i]);
                    for (int j = 0; j < P4Data.Count; j++)
                    {
                        if (j != 3) FirstPlaceTexts[j].text = P4Data[j];
                        else FirstPlaceTexts[j].text = P4Data[j];
                    }
                }
                else if (i == 1)
                {
                    SecondPlaceBG.color = new Color(1, 1, 0, 0.176f);
                    for (int j = 0; j < P4Data.Count; j++)
                    {
                        if (j != 3) SecondPlaceTexts[j].text = P4Data[j];
                        else SecondPlaceTexts[j].text = P4Data[j];
                    }
                }
                else if (i == 2)
                {
                    ThirdPlaceBG.gameObject.SetActive(true);
                    ThirdPlaceBG.color = new Color(1, 1, 0, 0.176f);
                    for (int j = 0; j < P4Data.Count; j++)
                    {
                        if (j != 3) ThirdPlaceTexts[j].text = P4Data[j];
                        else ThirdPlaceTexts[j].text = P4Data[j];
                    }
                }
                else if (i == 3)
                {
                    FourthPlaceBG.gameObject.SetActive(true);
                    FourthPlaceBG.color = new Color(1, 1, 0, 0.176f);
                    for (int j = 0; j < P4Data.Count; j++)
                    {
                        if (j != 3) FourthPlaceTexts[j].text = P4Data[j];
                        else FourthPlaceTexts[j].text = P4Data[j];
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

}