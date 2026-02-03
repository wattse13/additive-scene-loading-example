using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Carries out necessary loading/unloading functions based on transitionType given to HandleTransition
/// Every possible scene transition has an associated LoadTransition instance which defines which scenes will be loaded/unloaded
/// Management should never be unloaded
/// </summary>

public class AdditiveSceneController : MonoBehaviour
{
    public LoadingBarController loadBarController;
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    public bool IsLoading { get; private set; }

    public IEnumerator UnloadScenes(List<string> scenesToKeep)
    {
        IsLoading = true;

        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        for (int j = 0; j < loadedScenes.Length; j++)
        {
            // Management should never be unloaded
            if (loadedScenes[j].name == "Management" || loadedScenes[j].name == "Audio") { continue; }

            // Don't unload scenes that are needed for the next scene
            if (scenesToKeep.Contains(loadedScenes[j].name)) { continue; }

            // try and unload scene
            yield return CheckUnloadValidity(loadedScenes[j].name);
        }
        yield return endOfFrame;

        IsLoading = false;
    }

    public IEnumerator LoadScenes(List<string> scenesToLoad, string activeScene)
    {
        IsLoading = true;

        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            yield return LoadScene(scenesToLoad[i], activeScene);
        }

        yield return endOfFrame;
        yield return SetSceneActive(activeScene);

        IsLoading = false;
    }

    private IEnumerator SetSceneActive(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).IsValid()) { Debug.LogError($"{sceneName} is not a valid scene"); }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        yield return endOfFrame;
    }

    private IEnumerator CheckUnloadValidity(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).IsValid())
        {
            yield return UnloadScene(sceneName);
        }
        else { Debug.LogError(sceneName + " is not valid?"); }
    }

    public IEnumerator UnloadAssets()
    {
        IsLoading = true;
        var loadRoutine = Resources.UnloadUnusedAssets();
        loadBarController.StartLoading(loadRoutine);
        while (!loadRoutine.isDone) { yield return null; }
        IsLoading = false;
    }

    private IEnumerator LoadScene(string sceneName, string activeScene)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded) { yield break; }

        var loadRoutine = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        loadBarController.StartLoading(loadRoutine);

        while (!loadRoutine.isDone) { yield return null; }

        yield return endOfFrame;
    }

    private IEnumerator UnloadScene(string sceneName)
    {
        if (sceneName == "Management" || sceneName == "Audio") { yield break; }
        if (!SceneManager.GetSceneByName(sceneName).isLoaded) { yield break; }

        var loadRoutine = SceneManager.UnloadSceneAsync(sceneName);
        loadBarController.StartLoading(loadRoutine);
        while (!loadRoutine.isDone) { yield return null; }
    }
}

