using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int BlockID;
    public JSONRead.Block ThisBlock;
    // Start is called before the first frame update
    void Start()
    {
        ThisBlock = JSONRead.BlockLookUp(BlockID);
        transform.localScale = new Vector3(1, ThisBlock.height, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
