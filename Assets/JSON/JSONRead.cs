using System.IO;
using UnityEngine;

public class JSONRead : MonoBehaviour
{
    [System.Serializable]
    public class ProjRoot
    {
        public Projectile[] Projectiles;
        public LobProjectile[] LobShots;
    }
    [System.Serializable]
    public class MineRoot
    {
        public LandMine[] LandMines;
    }
    [System.Serializable]
    public class TrailData
    {
        public bool trailEnabled;
        public float time;
    }
    [System.Serializable]
    public class Projectile
    {
        public int ID;
        public string name;
        public float velocity;
        public int bounces;
        public bool explosive;
        public float explRadius;
        public TrailData td;
        public bool passiveNoise;
        public float cooldown;
    }
    [System.Serializable]
    public class LobProjectile
    {
        public int ID;
        public string name;
        public Vector2 velocity;
        public int bounces;
        public bool explosive;
        public float explRadius;
        public TrailData td;
        public bool passiveNoise;
        public float cooldown;
    }
    [System.Serializable]
    public class LandMine
    {
        public int ID;
        public string name;
        public float explRadius;
        public float detRange;
        public float fuseTime;
        public float totalTime;
        public float primeTime;
        public float cooldown;
    }
    [System.Serializable]
    public class Block
    {
        public int ID;
        public string name;
        public float height;
        public bool destructible;
    }
    [System.Serializable]
    public class BlockRoot
    {
        public Block[] blocks;
    }
    public static ProjRoot allProjs;
    public static MineRoot allMines;
    public static BlockRoot allBlocks;
    private void Start()
    {

    }
    public static Projectile ProjLookUp(int ID)
    {
        string ProjectileJSON = File.ReadAllText(Application.streamingAssetsPath + "/Projectiles.json"); //read file
        allProjs = JsonUtility.FromJson<ProjRoot>(ProjectileJSON); //convert to classes
        ID -= 1; //convert it to 0 indexed
        return allProjs.Projectiles[ID]; //send back projectile
    }
    public static LobProjectile LobLookUp(int ID)
    {
        string ProjectileJSON = File.ReadAllText(Application.streamingAssetsPath + "/Projectiles.json");
        allProjs = JsonUtility.FromJson<ProjRoot>(ProjectileJSON);
        ID -= 1;
        return allProjs.LobShots[ID];
    }
    public static LandMine MineLookUp(int ID)
    {
        string JSON = File.ReadAllText(Application.streamingAssetsPath + "/LandMines.json");
        allMines = JsonUtility.FromJson<MineRoot>(JSON);
        ID--;
        return allMines.LandMines[ID];
    }
    public static Block BlockLookUp(int ID)
    {
        string JSON = File.ReadAllText(Application.streamingAssetsPath + "/Blocks.json");
        allBlocks = JsonUtility.FromJson<BlockRoot>(JSON);
        ID--;
        return allBlocks.blocks[ID];
    }
}
