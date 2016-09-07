using UnityEngine;
using System.Collections;

public class FinalController : MonoBehaviour
{
    public string storeURL = "https://play.google.com/store/apps/details?id=com.StarAge.TwoJump";

    void Start()
    {
        StartCoroutine(WaitAndRestart(0.5f));
    }

    IEnumerator WaitAndRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                Application.OpenURL(storeURL);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

}
