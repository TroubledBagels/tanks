using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class Generation : MonoBehaviour
{
    public Vector2Int ChunkSize;
    public float WallThreshold;
    public float BasicWallThreshold;
    public float TallBasicThreshold;
    public float DestructibleWallThreshold;
    public float TallDestructibleThreshold;
    public short seed;
    public GameObject BasicWall;
    public GameObject BasicTallWall;
    public GameObject DestrWall;
    public GameObject DestrTallWall;
    public GameObject Floor;
    public float noiseScale;
    public int PlayerNum;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    public GameObject PU1;
    public GameObject PU2;
    public GameObject PU3;
    public GameObject PU4;

    private void Start()
    {
        Random.InitState(0); //sets the state of the inbuilt randomiser for Unity to 0
        /*WiimoteManager.FindWiimotes();
        foreach (Wiimote wm in WiimoteManager.Wiimotes)
        {
            wm.SetupIRCamera(IRDataType.BASIC);
        }*/
        PlayerNum = DataScript.PlayerNum;
        Debug.LogWarning(WiimoteManager.Wiimotes.Count);
        //StartCoroutine(Generate());
    }
    // Update is called once per frame
    void Update()
    {
        Randomiser();
        if (Input.GetKeyDown(KeyCode.G)) //detects if the g key is pressed
        {
            Debug.Log("Key Pressed");
            Generate(); //calls the generate function
        }
    }
    public IEnumerator Generate()
    {
        GameObject[] Walls = GameObject.FindGameObjectsWithTag("Wall"); //adds all the walls to an array
        foreach (GameObject go in Walls) //for every wall
        {
            Destroy(go); //destroy the wall
        }
        GameObject[] FloorWalls = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject fw in FloorWalls)
        {
            Destroy(fw);
        }
        GameObject[] Players = GameObject.FindGameObjectsWithTag("PlayerObject"); //adds all players to an array
        foreach (GameObject tank in Players) { //for each player
            Destroy(tank); //destroys the tank
        }
        GameObject[] Mines = GameObject.FindGameObjectsWithTag("LandMine");
        foreach (GameObject mine in Mines)
        {
            Destroy(mine);
        }
        GameObject[] Projectiles = GameObject.FindGameObjectsWithTag("projectile");
        foreach (GameObject projectile in Projectiles)
        {
            Destroy(projectile);
        }
        GameObject[] pus = GameObject.FindGameObjectsWithTag("powerup");
        foreach (GameObject pu in pus)
        {
            Destroy(pu);
        }

        Instantiate(Floor, new Vector3(0, 0, 0), Quaternion.identity);

        for (int y = (-ChunkSize.y / 2) - 4; y <= (ChunkSize.y / 2) + 4; y++) { //for every y value
            Instantiate(BasicTallWall, new Vector3((-ChunkSize.x / 2) - 4, 1f, y), Quaternion.identity); //place block on left side
            Instantiate(BasicTallWall, new Vector3((ChunkSize.x / 2) + 4, 1f, y), Quaternion.identity); //place block on right side
            Instantiate(BasicWall, new Vector3((-ChunkSize.x / 2) - 4, 4f, y), Quaternion.identity); //place block on left side
            Instantiate(BasicWall, new Vector3((ChunkSize.x / 2) + 4, 4f, y), Quaternion.identity); //place block on right side
        }
        for (int x = (-ChunkSize.x / 2) - 4; x <= (ChunkSize.x / 2) + 4; x++) { //for every x value
            Instantiate(BasicTallWall, new Vector3(x, 1f, (-ChunkSize.y / 2) - 4), Quaternion.identity); //place block on bottom
            Instantiate(BasicTallWall, new Vector3(x, 1f, (ChunkSize.y / 2) + 4), Quaternion.identity); //place block on top
            Instantiate(BasicWall, new Vector3(x, 4f, (-ChunkSize.y / 2) - 4), Quaternion.identity); //place block on bottom
            Instantiate(BasicWall, new Vector3(x, 4f, (ChunkSize.y / 2) + 4), Quaternion.identity); //place block on top
        }

        for (int x = -ChunkSize.x / 2; x <= ChunkSize.x / 2; x++) //for every x value within ChunkSize
        {
            for (int y = -ChunkSize.y / 2; y <= ChunkSize.y / 2; y++) //for every y value within ChunkSize
            {   
                if (PlayerNum == 2) //if there are 2 players
                {
                    float BlockID = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale); //generates the noise value for that position
                    if (x <= (ChunkSize.x / -2) + 4 && y <= (ChunkSize.y / -2) + 4) //if it's within the range of the bottom left corner
                    {
                        if (x == (ChunkSize.x / -2) + 2 && y == (ChunkSize.y / -2) + 2) //if it's in the spot
                        {
                            GameObject CurPlayer = Instantiate(Player1, new Vector3(x, 1f, y), Quaternion.identity);
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                            Debug.Log("P1 spawned");//spawn player
                        }
                    }
                    else if (x >= (ChunkSize.x / 2) - 3 && y >= (ChunkSize.y / 2) - 3) //if it's within the range of the top right corner
                    {
                        if (x == (ChunkSize.x / 2) - 1 && y == (ChunkSize.y / 2) - 1) //if it's in the centre
                        {
                            GameObject CurPlayer = Instantiate(Player2, new Vector3(x, 1f, y), Quaternion.identity);
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                            Debug.Log("P2 spawned");//spawn player
                        }
                    }
                    else if (Vector3.Distance(new Vector3(x, 0, y), new Vector3(0, 0, 0)) <= 3) {
                        if (x == 0 && y == 0)
                        {
                            int puID = Random.Range(1, 5);
                            switch (puID)
                            {
                                case 1:
                                    Instantiate(PU1, new Vector3(x, 1, y), PU1.transform.rotation);
                                    break;
                                case 2:
                                    Instantiate(PU2, new Vector3(x, 1, y), PU2.transform.rotation);
                                    break;
                                case 3:
                                    Instantiate(PU3, new Vector3(x, 1, y), PU3.transform.rotation);
                                    break;
                                case 4:
                                    Instantiate(PU4, new Vector3(x, 1, y), PU4.transform.rotation);
                                    break;
                            }
                        }
                    }
                    else if (BlockID >= WallThreshold) //if the ID is higher than the threshold
                    {
                        if (BlockID >= TallBasicThreshold)
                        {
                            Instantiate(BasicTallWall, new Vector3(x, 1f, y), Quaternion.identity); //spawn wall
                        }
                        else if (BlockID >= BasicWallThreshold)
                        {
                            Instantiate(BasicWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= TallDestructibleThreshold)
                        {
                            Instantiate(DestrTallWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= DestructibleWallThreshold)
                        {
                            Instantiate(DestrWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                    }
                }
                if (PlayerNum == 3) //if there are 2 players
                {
                    float BlockID = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale); //generates the noise value for that position
                    if (x <= (ChunkSize.x / -2) + 4 && y <= (ChunkSize.y / -2) + 4) //if it's within the range of the bottom left corner
                    {
                        if (x == (ChunkSize.x / -2) + 2 && y == (ChunkSize.y / -2) + 2) //if it's in the spot
                        {
                            GameObject CurPlayer = Instantiate(Player1, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x >= (ChunkSize.x / 2) - 3 && y >= (ChunkSize.y / 2) - 3) //if it's within the range of the top right corner
                    {
                        if (x == (ChunkSize.x / 2) - 1 && y == (ChunkSize.y / 2) - 1) //if it's in the centre
                        {
                            GameObject CurPlayer = Instantiate(Player2, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x >= -2 && y >= -2 && x <= 2 && y <= 2) //if it's within the range of the top right corner
                    {
                        if (x == 0 && y == 0) //if it's in the centre
                        {
                            GameObject CurPlayer = Instantiate(Player3, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x <= (ChunkSize.x / -2) + 4 && y >= (ChunkSize.y / 2) - 3) 
                    {
                        if (x == (ChunkSize.x / -2) + 2 && y == (ChunkSize.y / 2) - 2)
                        {
                            int puID = Random.Range(1, 5);
                            switch (puID)
                            {
                                case 1:
                                    Instantiate(PU1, new Vector3(x, 1, y), PU1.transform.rotation);
                                    break;
                                case 2:
                                    Instantiate(PU2, new Vector3(x, 1, y), PU2.transform.rotation);
                                    break;
                                case 3:
                                    Instantiate(PU3, new Vector3(x, 1, y), PU3.transform.rotation);
                                    break;
                                case 4:
                                    Instantiate(PU4, new Vector3(x, 1, y), PU4.transform.rotation);
                                    break;
                            }
                        }
                    }
                    else if (BlockID >= WallThreshold) //if the ID is higher than the threshold
                    {
                        if (BlockID >= TallBasicThreshold)
                        {
                            Instantiate(BasicTallWall, new Vector3(x, 1f, y), Quaternion.identity); //spawn wall
                        }
                        else if (BlockID >= BasicWallThreshold)
                        {
                            Instantiate(BasicWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= TallDestructibleThreshold)
                        {
                            Instantiate(DestrTallWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= DestructibleWallThreshold)
                        {
                            Instantiate(DestrWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                    }
                }
                if (PlayerNum == 4) //if there are 2 players
                {
                    float BlockID = Mathf.PerlinNoise((x + seed) * noiseScale, (y + seed) * noiseScale); //generates the noise value for that position
                    if (x <= (ChunkSize.x / -2) + 4 && y <= (ChunkSize.y / -2) + 4) //if it's within the range of the bottom left corner
                    { //bl
                        if (x == (ChunkSize.x / -2) + 2 && y == (ChunkSize.y / -2) + 2) //if it's in the spot
                        {
                            GameObject CurPlayer = Instantiate(Player1, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x >= (ChunkSize.x / 2) - 3 && y >= (ChunkSize.y / 2) - 3) //if it's within the range of the top right corner
                    { //tr
                        if (x == (ChunkSize.x / 2) - 1 && y == (ChunkSize.y / 2) - 1) //if it's in the centre
                        {
                            GameObject CurPlayer = Instantiate(Player2, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x <= (ChunkSize.x / -2) + 4 && y >= (ChunkSize.y / 2) - 3) //if it's within the range of the top right corner
                    { //tl
                        if (x == (ChunkSize.x / -2) + 2 && y == (ChunkSize.y / 2) - 1) //if it's in the centre
                        {
                            GameObject CurPlayer = Instantiate(Player3, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (x >= (ChunkSize.x / 2) - 3 && y <= (ChunkSize.y / -2) + 4) //if it's within the range of the bottom left corner
                    { //br
                        if (x == (ChunkSize.x / 2) - 1 && y == (ChunkSize.y / -2) + 2) //if it's in the spot
                        {
                            GameObject CurPlayer = Instantiate(Player4, new Vector3(x, 1f, y), Quaternion.identity); //spawn player
                            GetComponent<GameManager>().Tanks.Add(CurPlayer.GetComponentInChildren<PlayerController>().gameObject);
                        }
                    }
                    else if (Vector3.Distance(new Vector3(x, 0, y), new Vector3(0, 0, 0)) <= 3)
                    {
                        if (x == 0 && y == 0)
                        {
                            int puID = Random.Range(1, 5);
                            switch (puID)
                            {
                                case 1:
                                    Instantiate(PU1, new Vector3(x, 1, y), PU1.transform.rotation);
                                    break;
                                case 2:
                                    Instantiate(PU2, new Vector3(x, 1, y), PU2.transform.rotation);
                                    break;
                                case 3:
                                    Instantiate(PU3, new Vector3(x, 1, y), PU3.transform.rotation);
                                    break;
                                case 4:
                                    Instantiate(PU4, new Vector3(x, 1, y), PU4.transform.rotation);
                                    break;
                            }
                        }
                    }
                    else if (BlockID >= WallThreshold) //if the ID is higher than the threshold
                    {
                        if (BlockID >= TallBasicThreshold)
                        {
                            Instantiate(BasicTallWall, new Vector3(x, 1f, y), Quaternion.identity); //spawn wall
                        }
                        else if (BlockID >= BasicWallThreshold)
                        {
                            Instantiate(BasicWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= TallDestructibleThreshold)
                        {
                            Instantiate(DestrTallWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                        else if (BlockID >= DestructibleWallThreshold)
                        {
                            Instantiate(DestrWall, new Vector3(x, 1f, y), Quaternion.identity);
                        }
                    }
                }
            }
        }
        yield return null;
    }
    public void Randomiser()
    {
        seed = (short)((seed + Random.Range(0, 1000)) * Random.Range(0,1000)); //Randomises the seed
    }
}
