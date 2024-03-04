using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class PlayerController : MonoBehaviour
{
    public int PlayerID;
    public bool Alive;
    public Vector3 InputDirection;
    public int shots;
    public int deaths;
    public int speed;
    public float IRScaler;
    public int proj1ID;
    public int proj2ID;
    public int lobID;
    public int mineID;
    private JSONRead.Projectile proj1;
    private JSONRead.Projectile proj2;
    private JSONRead.LobProjectile LobShotObject;
    private JSONRead.LandMine mineObject;
    private bool canShoot1;
    private bool canShoot2;
    private bool canShootLob;
    private bool canLayMine;
    public bool Active;
    [HideInInspector]
    public int ExtraHits;
    public float cooldownMultiplier;
    public float LaserChargeTime;
    [HideInInspector]
    public int currentPUID;

    private Vector3 latestRotation;
    public Vector2 lookDirection;

    public GameObject bullet;
    public GameObject rocket;
    public GameObject mine;
    public GameObject lobShot;
    public GameObject Missile;
    public GameObject EMPObject;
    public Transform ap;
    public Transform turret;
    public Transform targetLoc;
    private Camera MainCam;
    private MeshRenderer[] Meshes; //finds all meshes
    private AudioSource so;
    private bool AudioPlaying;

    public Material blueMat;
    public Material redMat;
    public Material yellowMat;
    public Material greenMat;
    public Material purpleMat;
    public Material deadMat;

    public AudioClip moveSounds;
    public AudioClip tankDeath;
    public List<AudioClip> ShootAudio = new List<AudioClip>();

    public ParticleSystem smoke;
    public ParticleSystem deathParticles;
    public ParticleSystem LaserBeamCharge;

    public Wiimote controller;

    /*void Awake()
    {
        switch (PlayerID)
        { //reads PlayerID
            case 1: //if PlayerID = 1
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = blueMat; //set all meshes to blue
                }
                break;
            case 2:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = redMat;
                }
                break;
            case 3:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = greenMat;
                }
                break;
            case 4:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = yellowMat;
                }
                break;
            default:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = purpleMat;
                }
                break;
        }
    }*/
    // Start is called before the first frame update
    void Start()
    {
        Alive = true;
        cooldownMultiplier = 1f;

        proj1 = JSONRead.ProjLookUp(proj1ID);
        proj2 = JSONRead.ProjLookUp(proj2ID);
        LobShotObject = JSONRead.LobLookUp(lobID);
        mineObject = JSONRead.MineLookUp(mineID);

        canShoot1 = true;
        canShoot2 = true;
        canShootLob = true;
        canLayMine = true;
        Active = true;

    	AudioPlaying = false;
    	so = GetComponent<AudioSource>();
    	so.clip = moveSounds;
        so.volume = DataScript.Volume;
        Meshes = GetComponentsInChildren<MeshRenderer>();
        MainCam = Camera.main;
        Debug.Log(WiimoteManager.Wiimotes.Count);
        switch (PlayerID)
        { //reads PlayerID
            case 1: //if PlayerID = 1
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = blueMat; //set all meshes to blue
                }
                controller = WiimoteManager.Wiimotes[0];
                break;
            case 2:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = redMat;
                }
                controller = WiimoteManager.Wiimotes[1];
                break;
            case 3:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = greenMat;
                }
                controller = WiimoteManager.Wiimotes[2];
                break;
            case 4:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = yellowMat;
                }
                controller = WiimoteManager.Wiimotes[3];
                break;
            default:
                foreach (MeshRenderer block in Meshes)
                {
                    block.material = purpleMat;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate() {
        //Debug.Log(ExtraHits);
        int ret;
        do
        {
            ret = controller.ReadWiimoteData();
        } while (ret > 0);
        //Debug.Log(controller.Button.a);
        float[,] ir = controller.Ir.GetProbableSensorBarIR();
        for (int i = 0; i < 2; i++)
        {
            float x = ir[i, 0] * -0.07f; // 1023f;
            float y = ir[i, 1] * -0.06f; // 767f;
                                         //Debug.Log(new Vector2(x, y));
            targetLoc.position = new Vector3(x + 32, 7, y + 18.5f);
        }

        NunchuckData ncData = controller.Nunchuck; //gets the data
        InputDirection = new Vector3((float)(ncData.stick[0] - 124) / 95, 0, (float)(ncData.stick[1] - 127) / 92); //sets it into InputDirection
        if (InputDirection.x <= 0.1f && InputDirection.x >= -0.1f) InputDirection.x = 0f;
        if (InputDirection.y <= 0.1f && InputDirection.y >= -0.1f) InputDirection.y = 0f;
        Debug.Log(InputDirection);
        if (Alive) {
    		lookDirection = new Vector2(targetLoc.position.x, targetLoc.position.z) - new Vector2(transform.position.x, transform.position.z); //sets the location needed
    		float angle = Vector3.SignedAngle(Vector3.forward, new Vector3(lookDirection.x, 0, lookDirection.y), Vector3.up); //finds angle
    		turret.rotation = Quaternion.Euler(-90f, angle, 180f); //sets rotation
    	}
        if (Alive && Active)
        {
            //InputDirection.Normalize(); //makes the length of the vector a max of 1
            //Debug.Log(InputDirection);

            //InputDirection.x = Input.GetAxis("Horizontal"); //Get the x input axis value
            //InputDirection.z = Input.GetAxis("Vertical"); //Get the y input axis value
            Movement(InputDirection); //send the direction to a new function

            if (controller.Button.a && canShoot1 || Input.GetKey(KeyCode.E) && canShoot1)
            { //checks for the shoot button
                StartCoroutine(Shoot(bullet, 1)); //calls the function
                canShoot1 = false;
            }
            if (controller.Button.b && canShoot2 || Input.GetKey(KeyCode.F) && canShoot2)
            {
                StartCoroutine(Shoot(rocket, 2));
                canShoot2 = false;
            }
            if (ncData.z && canShootLob || Input.GetKey(KeyCode.R) && canShootLob)
            {
                StartCoroutine(Shoot(lobShot, 3));
                canShootLob = false;
            }
            if (ncData.c && canLayMine || Input.GetKey(KeyCode.Space) && canLayMine)
            {
                StartCoroutine(LayMine());
                canLayMine = false;
            }
            if (controller.Button.plus || Input.GetKeyDown(KeyCode.V))
            {
                StartCoroutine(PowerUpLaunch(currentPUID));
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(Die());
            }
        }
        /*if (controller.Button.home || Input.GetKeyDown(KeyCode.G))
        {
            MainCam.GetComponent<Generation>().Generate();
        }*/
        if (controller.Button.one || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ButtonPressed");
            StartCoroutine(MainCam.GetComponent<GameManager>().Pause(PlayerID));
        }
    }
    void Movement(Vector3 direction)
    { //set the position to the same position + the vector of the direction times speed time the time since the last frame
        transform.position = transform.position + new Vector3(direction.x * speed * Time.fixedDeltaTime, 0, direction.z * speed * Time.fixedDeltaTime);

        //if the axes are not 0, it sets the latest rotation to the direction
        if (direction.z != 0 || direction.x != 0) { latestRotation = direction; StartCoroutine(MovementSound()); }

        //if the axes are 0 it stops the sound
        //if (direction.z == 0 && direction.x == 0) { so.Stop(); }

        if (latestRotation.x > 0) {
        	transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(new Vector3(0, 0, 1), latestRotation, Vector3.forward), 0); //sets the rotation on the y axis to the angle between vectors
        } else {
        	transform.rotation = Quaternion.Euler(0, -Vector3.SignedAngle(new Vector3(0, 0, 1), latestRotation, Vector3.forward), 0); //sets the rotation on the y axis to the angle between vectors
        }
    }
    IEnumerator Shoot(GameObject projectile, int WeaponNum) {
        int rdm = Random.Range(0, ShootAudio.Count);
        so.PlayOneShot(ShootAudio[rdm]);
        GameObject ShotProj = Instantiate(projectile, ap.position, Quaternion.Euler(0f, turret.transform.rotation.eulerAngles.y - 180, 0f)); //spawns the projectile
        ShotProj.GetComponent<ProjectileScript>().Shooter = PlayerID;
        yield return new WaitForSeconds(ShootAudio[rdm].length);
        switch(WeaponNum)
        {
            case 1:
                yield return new WaitForSeconds(proj1.cooldown * cooldownMultiplier);
                canShoot1 = true;
                break;
            case 2:
                yield return new WaitForSeconds(proj2.cooldown * cooldownMultiplier);
                canShoot2 = true;
                break;
            case 3:
                yield return new WaitForSeconds(LobShotObject.cooldown * cooldownMultiplier);
                canShootLob = true;
                break;
            default:
                break;
        }
        switch (PlayerID)
        {
            case 1:
                MainCam.GetComponent<GameManager>().P1Shots++;
                break;
            case 2:
                MainCam.GetComponent<GameManager>().P2Shots++;
                break;
            case 3:
                MainCam.GetComponent<GameManager>().P3Shots++;
                break;
            case 4:
                MainCam.GetComponent<GameManager>().P4Shots++;
                break;
            default:
                break;
        }
        yield return null;
    }
    IEnumerator LayMine() {
    	GameObject tempmineobj = Instantiate(mine, transform.position, transform.rotation); //spawns the mine
        tempmineobj.GetComponent<LandMine>().LayerID = PlayerID;
        yield return new WaitForSeconds(mineObject.cooldown);
        canLayMine = true;
        yield return null;
    }
    public IEnumerator Die(int ShooterID = 0) {
        if (ExtraHits > 0)
        {
            ExtraHits--;
        }
        else
        {
            Alive = false;

            //play animation
            smoke.Play();
            deathParticles.Play();
            if (ShooterID != PlayerID)
            {
                switch (ShooterID)
                {
                    case 1:
                        MainCam.GetComponent<GameManager>().P1Kills++;
                        break;
                    case 2:
                        MainCam.GetComponent<GameManager>().P2Kills++;
                        break;
                    case 3:
                        MainCam.GetComponent<GameManager>().P3Kills++;
                        break;
                    case 4:
                        MainCam.GetComponent<GameManager>().P4Kills++;
                        break;
                    default:
                        break;
                }
            }
            switch (PlayerID)
            {
                case 1:
                    MainCam.GetComponent<GameManager>().P1Deaths++;
                    break;
                case 2:
                    MainCam.GetComponent<GameManager>().P2Deaths++;
                    break;
                case 3:
                    MainCam.GetComponent<GameManager>().P3Deaths++;
                    break;
                case 4:
                    MainCam.GetComponent<GameManager>().P4Deaths++;
                    break;
                default:
                    break;
            }
            foreach (MeshRenderer mesh in Meshes)
            {
                mesh.material = deadMat;
            }
            so.PlayOneShot(tankDeath);
            yield return new WaitForSeconds(3.5f);
            deathParticles.gameObject.SetActive(false);
            
        }
    	yield return null;    	
    }
    IEnumerator MovementSound() {
    	if (AudioPlaying) {
    		yield return null;
    	}
    	else {
    		so.Play();
    		AudioPlaying = true;

    		yield return new WaitForSeconds(so.clip.length);
    		AudioPlaying = false;
    		yield return null;
    	}    	
    }
    public IEnumerator CooldownReset()
    {
        yield return new WaitForSeconds(15);
        cooldownMultiplier = 1f;
        yield return null;
    }
    IEnumerator PowerUpLaunch(int id)
    {
        switch (id)
        {
            case 3:
                GameObject airstrike = Instantiate(Missile, new Vector3(targetLoc.position.x, 50, targetLoc.position.z), Quaternion.identity);
                airstrike.GetComponent<Missile>().Shooter = PlayerID;
                break;
            case 4:
                Instantiate(EMPObject, new Vector3(targetLoc.position.x, 30, targetLoc.position.z), Quaternion.identity);
                break;
        }
        currentPUID = -1;
        yield return null;
    }
    public IEnumerator EMPHit()
    {
        Active = false;
        yield return new WaitForSeconds(5);
        Active = true;
        yield return null;
    }
}
