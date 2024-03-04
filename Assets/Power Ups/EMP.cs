using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP : MonoBehaviour
{
    private JSONRead.Projectile empProj;
    private AudioSource so;
    private Vector3 spinDir;
    private float spinSpd;
    public ParticleSystem Lightning;
    public AudioClip EMPSound;
    // Start is called before the first frame update
    void Start()
    {
        empProj = JSONRead.ProjLookUp(4);
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        spinDir = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
        spinSpd = Random.Range(250, 360);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + spinDir * spinSpd * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explode()); 
    }

    IEnumerator Explode()
    {
        Lightning.Play();
        so.PlayOneShot(EMPSound);
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = false;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, empProj.explRadius);

        foreach (Collider col in cols)
        {
            if (col.gameObject.tag == "Player")
            {
                StartCoroutine(col.gameObject.GetComponent<PlayerController>().EMPHit());
            }
        }
        yield return new WaitForSeconds(EMPSound.length);
        Destroy(gameObject);
        yield return null;
    }
}
