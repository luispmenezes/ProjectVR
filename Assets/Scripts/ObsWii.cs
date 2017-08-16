using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ObsWii : MonoBehaviour
{

    [DllImport("UniWii")]
    private static extern void wiimote_start();

    [DllImport("UniWii")]
    private static extern void wiimote_stop();

    [DllImport("UniWii")]
    private static extern int wiimote_count();

    /*Exporta as funcoes de leitura do acceleremetro */

    [DllImport("UniWii")]
    private static extern byte wiimote_getAccX(int which);
    [DllImport("UniWii")]
    private static extern byte wiimote_getAccY(int which);
    [DllImport("UniWii")]
    private static extern byte wiimote_getAccZ(int which);

    /*Exporta as funcoes de leitura */

    [DllImport("UniWii")]
    private static extern float wiimote_getRoll(int which);
    [DllImport("UniWii")]
    private static extern float wiimote_getPitch(int which);
    [DllImport("UniWii")]
    private static extern float wiimote_getYaw(int which);

    private Vector3 oldVec;
    private float posX = 3.29f;
    private float posY = 2.03f;
    static public float posZ;

    public GameObject paddle;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        wiimote_start();


        if (!GetComponent<NetworkView>().isMine)
        {
            this.GetComponent<Camera>().enabled = false;
            this.transform.GetChild(0).gameObject.SetActive(false);
            //this.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            MovUpdate();
        }
        else
        {
            SyncedMovement();
        }
    }

    void MovUpdate()
    {

        float accelX = Mathf.Round(wiimote_getAccX(0)) - 128;
        float accelY = Mathf.Round(wiimote_getAccY(0)) - 125;
        float accelZ = Mathf.Round(wiimote_getAccZ(0)) - 154;

        Vector3 vec = new Vector3(accelY, posY, posZ);
        vec = Vector3.Lerp(oldVec, vec, Time.deltaTime * 5);
        oldVec = vec;

        transform.position = vec;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = GetComponent<Rigidbody>().position;
            stream.Serialize(ref syncPosition);

            syncVelocity = GetComponent<Rigidbody>().velocity;
            stream.Serialize(ref syncVelocity);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = GetComponent<Rigidbody>().position;
        }
    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }

}
