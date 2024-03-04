using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobProjectile : MonoBehaviour
{
    public JSONRead.LobProjectile proj;
    public int ProjectileID;
    private Rigidbody rb;
    private int bounces;
    private ParticleSystem ps;
    private Vector3 MoveDirection;
    public AudioClip reflection;
    public AudioClip explosion;
    private AudioSource so;
    private TrailRenderer tr;
    public Material trailMaterial;
    public AudioClip bgNoise;
    private bool projAlive;
    private int maxDistance = 1000;
    public int Shooter;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        tr = GetComponent<TrailRenderer>();
        proj = JSONRead.LobLookUp(ProjectileID); //find the projectile
        if (proj.passiveNoise)
        {
            projAlive = true;
            so.clip = bgNoise;
            StartCoroutine(PlayBGNoise());
        }
        if (proj.explosive)
        {
            ps = GetComponentInChildren<ParticleSystem>();
            Vector3 ParticleScale = new Vector3(proj.explRadius * 0.3f, proj.explRadius * 0.3f, proj.explRadius * 0.3f);
            ps.gameObject.transform.localScale = ParticleScale;
        }

        tr.enabled = proj.td.trailEnabled;
        tr.time = proj.td.time;
        tr.material = trailMaterial;

        bounces = proj.bounces;
        MoveDirection = transform.forward;

        rb.AddForce(new Vector3(transform.forward.x * proj.velocity.x, proj.velocity.y, transform.forward.z * proj.velocity.x), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Bounce(bounces);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Reflection(bounces);
        }
        else if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(collision.gameObject.GetComponent<PlayerController>().Die(Shooter));
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
    void Bounce(int bouncesLeft)
    {
        if (bouncesLeft > 0)
        {
            rb.AddForce(new Vector3(transform.forward.x * proj.velocity.x * (((float)bounces - 1) / proj.bounces), proj.velocity.y * (((float)bounces - 1) / proj.bounces), transform.forward.z * proj.velocity.x * (((float)bounces - 1) / proj.bounces)), ForceMode.Impulse);
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
    void Reflection(int bouncesLeft)
    {
        if (bouncesLeft > 0)
        {
            so.PlayOneShot(reflection);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit wallHit;
            if (Physics.Raycast(ray, out wallHit, maxDistance))
            {
                transform.forward = Vector3.Reflect(transform.forward, wallHit.normal);
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

        foreach (Collider thing in cols)
        {
            if (thing.tag == "Player")
            {
                StartCoroutine(thing.GetComponent<PlayerController>().Die(Shooter));
            }
            else if (thing.tag == "Destructible")
            {
                Destroy(thing.gameObject);
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
