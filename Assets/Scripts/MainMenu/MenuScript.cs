using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour 
{
    public void setStartPositionX(float x)
    {
        SpawnSettings.spawnPosition.x = x;
    }
    public void setStartPositionY(float y)
    {
        SpawnSettings.spawnPosition.y = y;
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("DetskiySad");
    }
}