using System;
using System.Collections.Generic;

/// <summary>
/// Defines what scenes are needed for game state
/// Defines what scene should be set active 
/// Interfaces define when a list of actions should be called during the scene load/unload process
/// </summary>

public class NewGameTransition : LoadTransition, IBeforeLoadScene, IBeforeAssetsUnload, IAfterAssetsUnload
{
    private const string activeScene = "MainCampus";
    private readonly List<string> scenesToKeep = new List<string> { "Management", "Audio", "CampusManagement", "MainCampus", "Player", "UI" };
    private readonly List<Action> beforeSceneLoad;
    private readonly List<Action> beforeAssetUnload;
    private readonly List<Action> afterAssetUnload;

    public override string ActiveScene { get => activeScene; }
    public override List<string> ScenesToKeep { get => scenesToKeep; }
    // required by IBeforeLoadScene interface
    public List<Action> BeforeLoadScene { get => beforeSceneLoad; }
    // required by IBeforeAssetUnload interface
    public List<Action> BeforeAssetUnload { get => beforeAssetUnload; }
    // required by IAfterAssetUnload interface
    public List<Action> AfterAssetUnload { get => afterAssetUnload; }

    public NewGameTransition(GameFlowManager gameFlowManager)
    {
        beforeSceneLoad = new List<Action>();
        beforeSceneLoad.Add(gameFlowManager.DisableMovementInputs);
        beforeSceneLoad.Add(gameFlowManager.LockCursorRequest);
        beforeSceneLoad.Add(gameFlowManager.OpenLoadBar);

        beforeAssetUnload = new List<Action>();
        beforeAssetUnload.Add(gameFlowManager.RequestInitStatValueManager);
        beforeAssetUnload.Add(gameFlowManager.RequestCreateFirstWeek);
        beforeAssetUnload.Add(gameFlowManager.RequestSetCameras);
        beforeAssetUnload.Add(gameFlowManager.RequestSetSun);
        beforeAssetUnload.Add(gameFlowManager.RequestCreateNPCSpriteMap);

        afterAssetUnload = new List<Action>();
        afterAssetUnload.Add(gameFlowManager.SwitchToPlayerInputs);
        afterAssetUnload.Add(gameFlowManager.RequestSettingsInitialize);
        afterAssetUnload.Add(gameFlowManager.RequestSavePlayerPosition);
        afterAssetUnload.Add(gameFlowManager.ReturnPlayerStartLocation);
        afterAssetUnload.Add(gameFlowManager.SaveGame);
        afterAssetUnload.Add(gameFlowManager.RequestStatBarInitialize);
        afterAssetUnload.Add(gameFlowManager.RequestCampusLoopStart);
        afterAssetUnload.Add(gameFlowManager.RequestExteriorLocations);
        afterAssetUnload.Add(gameFlowManager.RequestSetWorldMapRefPoints);
        afterAssetUnload.Add(gameFlowManager.RequestExteriorMap);
        afterAssetUnload.Add(gameFlowManager.UpdateSceneState);
        afterAssetUnload.Add(gameFlowManager.InitializeSkybox);
        afterAssetUnload.Add(gameFlowManager.RequestInitialEngagement);
        afterAssetUnload.Add(gameFlowManager.RequestResetPlayerRotation);
        afterAssetUnload.Add(gameFlowManager.CloseLoadBar);
    }
}
