using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdsDialog : MonoBehaviour
{

    /// <summary>Вызывается по кнопке startAds</summary>
    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            var showOptions = new ShowOptions
            {
                resultCallback = HandleShowResult
            };
            Advertisement.Show(null, showOptions);
        }
        else
        {
            LoadNextLevel();
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                LoadNextLevel();
                break;

            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                LoadNextLevel();
                break;

            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    public void LoadNextLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
}
