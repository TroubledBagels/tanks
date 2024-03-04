using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int PowerUpID;
    public float rotationSpeed = 20;
    private AudioSource so;
    public AudioClip pickUpSound;
    public bool MultiPartModel = false;
    // Start is called before the first frame update
    void Start()
    {
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
    }
    private void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotationSpeed * Time.deltaTime, transform.rotation.eulerAngles.z);   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (PowerUpID)
            {
                case 1:
                    other.gameObject.GetComponent<PlayerController>().ExtraHits = 3;
                    break;
                case 2:
                    other.gameObject.GetComponent<PlayerController>().cooldownMultiplier = 0.3f;
                    StartCoroutine(other.gameObject.GetComponent<PlayerController>().CooldownReset());
                    break;
                case 3:
                    other.gameObject.GetComponent<PlayerController>().currentPUID = 3;
                    MultiPartModel = true;
                    break;
                case 4:
                    other.gameObject.GetComponent<PlayerController>().currentPUID = 4;
                    MultiPartModel = true;
                    break;
                default:
                    StartCoroutine(other.gameObject.GetComponent<PlayerController>().Die());
                    break;
            }
            StartCoroutine(PlaySound());
        }
    }
    IEnumerator PlaySound()
    {
        if (MultiPartModel)
        {
            MeshRenderer[] allMeshes = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in allMeshes)
            {
                mesh.enabled = false;
            }
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        
        GetComponent<BoxCollider>().enabled = false;
        so.PlayOneShot(pickUpSound);
        yield return new WaitForSeconds(pickUpSound.length);
        Destroy(gameObject);
        yield return null;
    }
}
