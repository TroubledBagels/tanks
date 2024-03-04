using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public JSONRead.Projectile proj;
    public int ProjectileID;
    private Rigidbody rb;
    private int bounces;    
    private int maxDistance = 1000;
    private ParticleSystem ps;
    private Vector3 MoveDirection;
    public AudioClip reflection;
    public AudioClip explosion;
    private AudioSource so;
    private TrailRenderer tr;
    public Material trailMaterial;
    public AudioClip bgNoise;
    private bool projAlive;
    public int Shooter;
    private Camera MainCam;
    // Start is called before the first frame update
    void Start()
    {
        MainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        tr = GetComponent<TrailRenderer>();
        proj = JSONRead.ProjLookUp(ProjectileID); //find the projectile
        if (proj.passiveNoise)
        {
            projAlive = true;
            so.clip = bgNoise;
            StartCoroutine(PlayBGNoise());
        }
        if (proj.explosive)
        {
            ps = GetComponentInChildren<ParticleSystem>();
            Vector3 ParticleScale = new Vector3(3.3f, 3.3f, 3.3f);
            ps.gameObject.transform.localScale = ParticleScale;
        }

        tr.enabled = proj.td.trailEnabled;
        tr.time = proj.td.time;
        tr.material = trailMaterial;
        
        bounces = proj.bounces;
        MoveDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = MoveDirection * proj.velocity * Time.timeScale;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Reflection(bounces);
        }
        else if (collision.gameObject.tag == "projectile")
        {
            if (proj.explosive)
            {
                projAlive = false;
                so.Stop();
                StartCoroutine(Explode());
            }
            else
            {
                projAlive = false;
                so.Stop();
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.tag == "Player")
        {  
            if (proj.explosive)
            {
                projAlive = false;
                so.Stop();
                StartCoroutine(Explode());
            }
            else
            {
                StartCoroutine(collision.gameObject.GetComponent<PlayerController>().Die());
                switch (Shooter)
                {
                    case 1:
                        MainCam.GetComponent<GameManager>().P1Hits++;
                        break;
                    case 2:
                        MainCam.GetComponent<GameManager>().P2Hits++;
                        break;
                    case 3:
                        MainCam.GetComponent<GameManager>().P3Hits++;
                        break;
                    case 4:
                        MainCam.GetComponent<GameManager>().P4Hits++;
                        break;
                    default:
                        break;
                }
                projAlive = false;
                so.Stop();
                Destroy(gameObject);
            }
        }
    }
    void Reflection(int bouncesLeft)
    {
        if (bouncesLeft > 0)
        {
            so.PlayOneShot(reflection);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit wallHit;
            if (Physics.Raycast(ray, out wallHit, maxDistance))
            {
                MoveDirection = Vector3.Reflect(MoveDirection, wallHit.normal);
            }
            bounces--;
        }
        else
        {
            if (proj.explosive)
            {
                projAlive = false;
                so.Stop();
                StartCoroutine(Explode());
            }
            else
            {
                projAlive = false;
                so.Stop();
                Destroy(gameObject);
            }
        }
    }   
    IEnumerator Explode()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        tr.enabled = false;
        MoveDirection = new Vector3(0, 0, 0);

        so.PlayOneShot(explosion);

        Collider[] cols = Physics.OverlapSphere(transform.position, proj.explRadius);
        
        foreach (Collider thing in cols) {
            Debug.Log(thing.tag);
            if (thing.tag == "Player")
            {
                switch (Shooter)
                {
                    case 1:
                        MainCam.GetComponent<GameManager>().P1Hits++;
                        break;
                    case 2:
                        MainCam.GetComponent<GameManager>().P2Hits++;
                        break;
                    case 3:
                        MainCam.GetComponent<GameManager>().P3Hits++;
                        break;
                    case 4:
                        MainCam.GetComponent<GameManager>().P4Hits++;
                        break;
                    default:
                        break;
                }
                StartCoroutine(thing.GetComponent<PlayerController>().Die());
            }
            else if (thing.tag == "Wall")
            {
                if (thing.gameObject.GetComponent<Block>().ThisBlock.destructible)
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
    IEnumerator PlayBGNoise()
    {
        while (projAlive)
        {
            so.Play();
            yield return new WaitForSeconds(so.clip.length);
        }
        yield return null;
    }
}
