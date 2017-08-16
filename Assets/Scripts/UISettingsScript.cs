using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISettingsScript : MonoBehaviour {

    public Text sensor_sel;
    public Text camera_sel;
    public Text view_sel;

    public void StartGame()
    {
        UIManagerScript.is_server = true;
        Application.LoadLevel("MainScene");
    }

    public void ConnectButton()
    {
        UIManagerScript.is_server = false;
        UIManagerScript.ipAddress = GameObject.FindWithTag("ipAddress").GetComponent<Text>().text;
        Application.LoadLevel("MainScene");

    }

    //Sensors

    public void ObserverButton()
    {
        sensor_sel.text = "Observer";
        UIManagerScript.sensor = 0;
    }

    

    public void LeapButton()
    {
        sensor_sel.text = "Leap Motion";
        UIManagerScript.sensor = 1;
    }

    public void WiimoteButton()
    {
        sensor_sel.text = "Wiimote";
        UIManagerScript.sensor = 2;
    }

    public void OmniButton()
    {
        sensor_sel.text = "Omni Phantom";
        UIManagerScript.sensor = 3;
    }

    //Cameras

    public void PCButton()
    {
        camera_sel.text = "PC Display";
        UIManagerScript.cam = 0;
    }

    public void OculusButton()
    {
        camera_sel.text = "Oculus Rift";
        UIManagerScript.cam = 1;
    }

    public void VRButton()
    {
        camera_sel.text = "VR2000";
        UIManagerScript.cam = 2;
    }

    //Views


    public void RedButton()
    {
        view_sel.text = "Red Side";
        UIManagerScript.view = 2;
    }


    public void PurpleButton()
    {
        view_sel.text = "Purple Side";
        UIManagerScript.view = 3;
    }

    //Others


    public void GodViewButton()
    {
        sensor_sel.text = "GOD MODE";
        camera_sel.text = "GOD MODE";
        UIManagerScript.sensor = 4;
    }

    public void AndroidViewButton()
    {
        sensor_sel.text = "Android MODE";
        camera_sel.text = "Android MODE";
        UIManagerScript.sensor = 5;
    }

    public void ExtremeButton(){
        GameObject.Find("btn_observer").SetActive(false);
        GameObject.Find("btn_omni").SetActive(false);
        GameObject.Find("btn_godmode").SetActive(false);
        GameObject.Find("btn_android").SetActive(false);
        LeapButton();
        UIManagerScript.extr = true;
    }
}
