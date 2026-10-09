using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReload : MonoBehaviour
{
    [SerializeField]private string sceneName = "Menu";
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName);
        }        
    }
}
