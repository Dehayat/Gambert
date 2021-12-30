using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject main;
    public GameObject credits;
    public UnityEngine.EventSystems.EventSystem eventSystem;

    private bool isLoading = false;
    public void Play()
    {
        if (isLoading)
        {
            return;
        }
        isLoading = true;
        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(LoadGame());
    }
    IEnumerator LoadGame()
    {
        BlackScreen.instance.done += ScreenIsBlack;
        BlackScreen.instance.FadeToBlack();
        yield return new WaitUntil(() => isScreenBlack);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CharacterTest");
    }
    private bool isScreenBlack = false;
    private void ScreenIsBlack()
    {
        isScreenBlack = true;
        BlackScreen.instance.done -= ScreenIsBlack;
    }

    private GameObject lastSelected;
    public void Credits()
    {
        lastSelected = eventSystem.currentSelectedGameObject;
        main.SetActive(false);
        credits.SetActive(true);
        credits.GetComponent<Animator>().Play("Credits");
        eventSystem.SetSelectedGameObject(credits);
    }
    public void OpenMenu()
    {
        credits.SetActive(false);
        main.SetActive(true);
        eventSystem.SetSelectedGameObject(lastSelected);
    }
}
