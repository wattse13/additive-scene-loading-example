using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Steps throgh scene loading process 
/// Uses functions in AdditiveSceneController to carry out the actual loading/unloading functions
/// At each step of loading process checks to see if corresponding interface has been implemented
/// by passed in LoadTransition instance
/// If LoadTransition instance has implemented corresponding interface
/// list of actions held in LoadTransition instance are called before the next step of scene loading is started
/// </summary>

public class LoadFlowController : MonoBehaviour
{
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    public IEnumerator LoadRoutine(AdditiveSceneController sceneController, LoadTransition loadTransition)
    {
        if (loadTransition is IBeforeUnloadScene beforeUnloadActions)
        {
            if (!IsListNullOrEmpty(beforeUnloadActions.BeforeUnloadScene))
            {
                for (int i = 0; i < beforeUnloadActions.BeforeUnloadScene.Count; i++)
                {
                    beforeUnloadActions.BeforeUnloadScene[i]();
                }
                yield return endOfFrame;
            }
        }

        // handle scene unloading
        StartCoroutine(sceneController.UnloadScenes(loadTransition.ScenesToKeep));

        // wait until unloading is finished
        while (sceneController.IsLoading) { yield return null; }

        if (loadTransition is IBeforeLoadScene beforeLoadActions)
        {
            if (!IsListNullOrEmpty(beforeLoadActions.BeforeLoadScene))
            {
                for (int i = 0; i < beforeLoadActions.BeforeLoadScene.Count; i++)
                {
                    beforeLoadActions.BeforeLoadScene[i]();
                }
                yield return endOfFrame;
            }
        }

        // handle scene loading
        StartCoroutine(sceneController.LoadScenes(loadTransition.ScenesToKeep, loadTransition.ActiveScene));

        // wait until loading is finished
        while (sceneController.IsLoading) { yield return null; }

        if (loadTransition is IBeforeAssetsUnload beforeAssetUnload)
        {
            if (!IsListNullOrEmpty(beforeAssetUnload.BeforeAssetUnload))
            {
                for (int i = 0; i < beforeAssetUnload.BeforeAssetUnload.Count; i++)
                {
                    beforeAssetUnload.BeforeAssetUnload[i]();
                }
                yield return endOfFrame;
            }
        }

        // unload assets
        yield return StartCoroutine(sceneController.UnloadAssets());

        // wait until unloading assets is finished
        while (sceneController.IsLoading) { yield return null; }

        if (loadTransition is IAfterAssetsUnload afterAssetsUnload)
        {
            if (!IsListNullOrEmpty(afterAssetsUnload.AfterAssetUnload))
            {
                for (int i = 0; i < afterAssetsUnload.AfterAssetUnload.Count; i++)
                {
                    afterAssetsUnload.AfterAssetUnload[i]();
                }
            }
        }
    }

    private bool IsListNullOrEmpty(List<Action> actions)
    {
        if (actions == null) { return true; }
        if (actions.Count == 0) { return true; }
        else { return false; }
    }
}
