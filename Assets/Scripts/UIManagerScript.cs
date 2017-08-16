using UnityEngine;
using System.Collections;

public class UIManagerScript : MonoBehaviour {

    /* variaveis usadas para passar a informação dos settings para o mundo de jogo*/

    //indicates wich sensor is being used
    static public int sensor = 0;

    //indicates which camera type visualiser is being used
    static public int cam = 0;

    //indicates which position is selected
    static public int view = 0;

    //indicates if the running isntance is a client or server
    static public bool is_server = true;

    //if the running insbtance is a client indicates the ip address of the server
    static public string ipAddress = "127.0.0.1";

    // extreme mode flag
    static public bool extr;
    
    private void Start()
    {
        DontDestroyOnLoad(GameObject.FindWithTag("UIManager"));
    }



    private void StartGame()
    {
        Application.LoadLevel("MainScene");
    }

    private void GoSettings()
    {
        Application.LoadLevel("SettingsScene");
    }


}
