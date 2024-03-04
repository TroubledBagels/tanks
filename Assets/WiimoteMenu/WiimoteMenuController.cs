using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class WiimoteMenuController : MonoBehaviour
{
    public GameObject Cube;
    public Wiimote Player1Controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int ret;
        do
        {
            ret = Player1Controller.ReadWiimoteData();
        } while (ret > 0);
        Debug.Log(Player1Controller.Button.a);
        float[,] ir = Player1Controller.Ir.GetProbableSensorBarIR();
        for (int i = 0; i < 2; i++)
        {
            float x = ir[i, 0] / 1023f;
            float y = ir[i, 1] / 767f;
            Debug.Log(x);
            Debug.Log(y);
            Cube.transform.position = new Vector3(x, y + 3, transform.position.z);
        }
    }
    public void Scan()
    {   
        foreach (Wiimote remote in WiimoteManager.Wiimotes)
        {
            WiimoteManager.Cleanup(remote);
        }
        WiimoteManager.FindWiimotes();
        Player1Controller = WiimoteManager.Wiimotes[0];
        Player1Controller.SetupIRCamera(IRDataType.BASIC);
    }
}
