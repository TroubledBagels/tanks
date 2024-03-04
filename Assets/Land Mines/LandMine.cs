using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    public int MineID;
    public JSONRead.LandMine mine;
    private AudioSource so;
    public AudioClip explosion;
    private ParticleSystem ps;
    private MeshRenderer mr;
    private int timer;
    private bool ticking;
    public Material regMat;
    public Material detMat;
    private bool primed;
    private bool exploded;
    public int LayerID;
    // Start is called before the first frame update
    void Start()
    {
        exploded = false;
        primed = false;
        ticking = true;
        mine = JSONRead.MineLookUp(MineID);
        ps = GetComponentInChildren<ParticleSystem>();
        so = GetComponent<AudioSource>();
        so.volume = DataScript.Volume;
        mr = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer == mine.primeTime * 50)
        {
            primed = true;
            mr.material = regMat;
        }
        if (timer % 20 == 0 && ticking && primed)
        {
            ProximityCheck();
        }
        if (timer >= ((mine.totalTime + mine.primeTime) * 50) && ticking)
        {
            StartCoroutine(Fuse());
        }
    }
    private void FixedUpdate()
    {
        if (ticking)
        {
            timer++;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "projectile" && ticking && primed)
        {
            Destroy(other.gameObject);
            StartCoroutine(Explode());
        }
    }
    public IEnumerator Explode()
    {
        if (!exploded)
        {
            exploded = true;
            ticking = false;
            MeshRenderer[] allMeshes = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in allMeshes)
            {
                mesh.enabled = false;
            }

            so.PlayOneShot(explosion);

            Collider[] cols = Physics.OverlapSphere(transform.position, mine.explRadius);

            foreach (Collider thing in cols)
            {
                if (thing.tag == "Player")
                {
                    StartCoroutine(thing.GetComponent<PlayerController>().Die(LayerID));
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
    void ProximityCheck()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, mine.detRange);

        foreach (Collider collider in cols)
        {
            if (collider.tag == "Player")
            {
                StartCoroutine(Fuse());
            }
        }
    }
    IEnumerator Fuse()
    {
        ticking = false;
        yield return new WaitForSeconds(mine.fuseTime / 5);
        mr.material = detMat;
        yield return new WaitForSeconds(mine.fuseTime / 5);
        mr.material = regMat;
        yield return new WaitForSeconds(mine.fuseTime / 5);
        mr.material = detMat;
        yield return new WaitForSeconds(mine.fuseTime / 5);
        mr.material = regMat;
        yield return new WaitForSeconds(mine.fuseTime / 5);
        mr.material = detMat;
        StartCoroutine(Explode());
        yield return null;
    }
}
