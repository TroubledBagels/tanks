using System.Collections;
using System.IO;
using UnityEngine;
using WiimoteApi;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI numConnected;
    public TextMeshProUGUI playerNum;
    private bool Fading;
    private bool Loaded;
    private bool Loading;
    public CanvasGroup FadeOutCG;
    private float f;
    private float fadeTime = 1.5f;
    private AudioSource so;
    public void Start()
    {
        f = 1;
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        StartCoroutine(FadeIn());
        playerNum.text = "";
    }
    IEnumerator FadeIn()
    {
        Loading = true;
        f = 1;
        yield return new WaitForSeconds(fadeTime);
        Loading = false;
        Loaded = true;
        FadeOutCG.gameObject.SetActive(false);
    }
    public void DisconnectWiimotes()
    {
        foreach (Wiimote wm in WiimoteManager.Wiimotes)
        {
            WiimoteManager.Cleanup(wm);
        }
    }
    public void Update()
    {
        foreach (Wiimote wm in WiimoteManager.Wiimotes)
        {
            wm.SendDataReportMode(InputDataType.REPORT_EXT21);
        }
        if (Loading)
        {
            f -= Time.deltaTime * fadeTime;

            FadeOutCG.alpha = f;
        }
        if (Loaded)
        {
            numConnected.text = WiimoteManager.Wiimotes.Count.ToString();

            if (Fading)
            {
                f += Time.deltaTime * fadeTime;

                FadeOutCG.alpha = f;
            }
        }
    }
    public void PlayGame()
    {
        Fading = true;
        string PlayerNumText = string.Format("{0} Players", WiimoteManager.Wiimotes.Count);
        playerNum.text = PlayerNumText;
        DataScript.PlayerNum = WiimoteManager.Wiimotes.Count;
        FadeOutCG.gameObject.SetActive(true);
        StartCoroutine(LoadScene("GameScene"));
    }
    public void Options()
    {
        Fading = true;
        playerNum.text = "";
        FadeOutCG.gameObject.SetActive(true);
        StartCoroutine(LoadScene("OptionsMenu"));
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void WiimoteScan()
    {
        foreach (Wiimote remote in WiimoteManager.Wiimotes)
        {
            WiimoteManager.Cleanup(remote);
        }
        WiimoteManager.FindWiimotes();
        foreach (Wiimote wm in WiimoteManager.Wiimotes)
        {
            wm.SetupIRCamera(IRDataType.BASIC);
            wm.SendDataReportMode(InputDataType.REPORT_BUTTONS_IR10_EXT9);
            Debug.Log(wm.current_ext);
        }
    }
    IEnumerator LoadScene(string target)
    {
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(target);
    }
}
