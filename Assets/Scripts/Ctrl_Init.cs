using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctrl_Init : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Server is Available? " + (Client.Instance != null));
        UnityEngine.SceneManagement.SceneManager.LoadScene("01_Main");
    }
}
