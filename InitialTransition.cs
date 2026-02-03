using System;
using System.Collections.Generic;

/// <summary>
/// Defines what scenes are needed for game state
/// Defines what scene should be set active 
/// Interfaces define when a list of actions should be called during the scene load/unload process
/// </summary>

public class InitialTransition : LoadTransition, IBeforeLoadScene, IBeforeAssetsUnload, IAfterAssetsUnload
{
    private const string activeString = "MainCampus";
    private readonly List<string> scenesToKeep = new List<string> { "Management", "Audio", "MainMenu", "MainCampus", "Player" };
    private readonly List<Action> beforeLoadScene;
    private readonly List<Action> beforeAssetUnload;
    private readonly List<Action> afterAssetUnload;

    public override string ActiveScene { get => activeString; }
    public override List<string> ScenesToKeep { get => scenesToKeep; }
    // required by IBeforeLoadScene interface
    public List<Action> BeforeLoadScene { get => beforeLoadScene; }
    // required by IAfterAssetUnload interface
    public List<Action> AfterAssetUnload { get => afterAssetUnload; }
    // required by IBeforeAssetUnload interface
    public List<Action> BeforeAssetUnload { get => beforeAssetUnload; }

    public InitialTransition(GameFlowManager gameFlowManager)
    {
        beforeLoadScene = new List<Action>();
        beforeLoadScene.Add(gameFlowManager.DisableMovementInputs);
        beforeLoadScene.Add(gameFlowManager.LockCursorRequest);
        beforeLoadScene.Add(gameFlowManager.OpenLoadBar);
        beforeLoadScene.Add(gameFlowManager.RequestCreateDialogueTabs);
        beforeLoadScene.Add(gameFlowManager.CreateLocationData);
        beforeLoadScene.Add(gameFlowManager.CreatePlayerSpawnData);

        beforeAssetUnload = new List<Action>();
        beforeAssetUnload.Add(gameFlowManager.RequestSetCameras);
        beforeAssetUnload.Add(gameFlowManager.RequestSetSun);

        afterAssetUnload = new List<Action>();
        afterAssetUnload.Add(gameFlowManager.RequestPlayerCharList);
        afterAssetUnload.Add(gameFlowManager.RequestSettingsInitialize);
        afterAssetUnload.Add(gameFlowManager.SwitchToStartMenuUIInputs);
        afterAssetUnload.Add(gameFlowManager.UnlockCursorRequest);
        afterAssetUnload.Add(gameFlowManager.CloseLoadBar);
    }
}
