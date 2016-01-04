using UnityEngine;
using UnityEngine.SceneManagement;

public class HappyEnd : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        GoogleAnalyticsV3.instance.LogScreen("Level " + SceneManager.GetActiveScene().buildIndex);
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) SceneManager.LoadScene(0);
        else if (Input.touchCount > 0) SceneManager.LoadScene(0);
    }
}
