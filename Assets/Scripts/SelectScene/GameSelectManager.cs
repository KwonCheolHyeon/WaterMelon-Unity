using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectManager : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void StartGame(string _sceneName)
    {
        UtisScript.LoadScene(_sceneName);
    }
}
