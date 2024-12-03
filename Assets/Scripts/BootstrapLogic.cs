using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//Bootstrap script to validate that all the persistant components are initilized smoothly and correctly
public class BootstrapLogic : MonoBehaviour
{
    [SerializeField] DiscordManager discordManager;
    public bool loadMainMenu = true;

    private void Start()
    {
        StartCoroutine(Validate());
    }

    private IEnumerator Validate()
    {
        yield return null;

        yield return StartCoroutine(GameManager.Instance.Init());
        yield return StartCoroutine(SteamManager.Instance.InitSteam());
        yield return StartCoroutine(discordManager.Init());

        if (loadMainMenu)
        {
            Debug.Log("Starting Main Menu Scene Load!");
            var operation = SceneManager.LoadSceneAsync(GameManager.Instance.mainMenuScene);
            // Tell unity to activate the scene soon as its ready
            operation.allowSceneActivation = true;

            // While the main menu scene is loading update the progress 
            while (!operation.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
