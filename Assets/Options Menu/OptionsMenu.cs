using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Image BlackScreen;
    public Slider VolSlider;
    public float SliderVal;
    private bool Fading;
    private bool Loaded;
    private bool Loading;
    private float FadeLength = 1.5f;
    private float f;
    private AudioSource so;
    public AudioClip SoundTest;
    // Start is called before the first frame update
    void Start()
    {
        SliderVal = Mathf.Log10(DataScript.Volume);
        VolSlider.value = SliderVal;
        so = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {
        Loading = true;
        f = 1;
        yield return new WaitForSeconds(FadeLength);
        Loading = false;
        Loaded = true;
        BlackScreen.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Loading)
        {
            f -= Time.deltaTime * FadeLength;
            BlackScreen.color = new Color(0, 0, 0, f);
        }
        if (Loaded)
        {
            SliderVal = Mathf.Pow(10, VolSlider.value);
            DataScript.Volume = SliderVal;
            if (Fading)
            {
                f += Time.deltaTime * FadeLength;
                BlackScreen.color = new Color(0, 0, 0, f);
            }
            Debug.Log(SliderVal);
        }
    }
    public void Back()
    {
        StartCoroutine(BackButton());
    }
    public IEnumerator BackButton()
    {
        BlackScreen.gameObject.SetActive(true);
        Fading = true;
        yield return new WaitForSeconds(FadeLength);
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }
    public void SoundButton()
    {
        so.volume = DataScript.Volume;
        so.PlayOneShot(SoundTest);
    }
}
