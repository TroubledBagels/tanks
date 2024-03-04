using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public JSONRead.Projectile missile;
    private float speed;
    private int rotDirection;
    private float rotSpeed;
    private AudioSource so;
    public AudioClip explosion;
    public AudioClip falling;
    public ParticleSystem ps;
    public ParticleSystem trailps;
    private bool StillPlaying;
    private bool exploded;
    public int Shooter;
    // Start is called before the first frame update
    void Start()
    {
        missile = JSONRead.ProjLookUp(3);
        Debug.Log(missile.explosive);
        speed = -missile.velocity;
        Debug.Log(speed);
        rotDirection = Random.Range(0, 1);
        if (rotDirection == 0) rotDirection = -1;
        rotSpeed = Random.Range(90f, 135f);
        so = GetComponent<AudioSource>();
        so.clip = falling;
        so.volume = DataScript.Volume;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + rotDirection * rotSpeed * Time.deltaTime, 0f);
        if (!exploded) StartCoroutine(PlaySound());
    }
    IEnumerator PlaySound()
    {
        if (StillPlaying) yield return null;
        else
        {
            StillPlaying = true;
            so.PlayOneShot(falling);
            yield return new WaitForSeconds(falling.length);
            StillPlaying = false;
        }
        yield return null;
    }
    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explode());
    }
    public IEnumerator Explode()
    {
        exploded = true;
        so.Stop();
        so.PlayOneShot(explosion);
        trailps.Stop();

        MeshRenderer[] allMeshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in allMeshes)
        {
            mesh.enabled = false;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position, missile.explRadius);
        
        foreach (Collider thing in cols)
        {
            if (thing.tag == "Player")
            {
                StartCoroutine(thing.GetComponent<PlayerController>().Die(Shooter));
            }
            else if (thing.tag == "Wall")
            {
                if (thing.GetComponent<Block>().ThisBlock.destructible)
                {
                    Destroy(thing.gameObject);
                }
            }
            else if (thing.tag == "LandMine")
            {
                StartCoroutine(thing.gameObject.GetComponent<LandMine>().Explode());
            }
        }
        ps.Play();
        yield return new WaitForSeconds(ps.main.duration);
        Destroy(gameObject);
        yield return null;
    }
}
