using GameAnalyticsSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds references and actions needed to complete scene transitions
/// Instances of the LoadTransition class then reference the actions they need in order to carry out a successful scene transition
/// </summary>

public class GameFlowManager : MonoBehaviour
{
    private Dictionary<SceneTransition, LoadTransition> transitionsDict;

    private AdditiveSceneController sceneController;
    private LoadingBarController loadBarController;
    private PlayerSpawnController playerSpawnController;
    private PlayerSpawnData playerSpawnData;
    [SerializeField] LoadFlowController loadFlowController; // drag/drop reference
    [SerializeField] WeekManager weekManager; // drag/drop reference

    [SerializeField] private SaveSystem saveSystem; // drag/drop reference
    [SerializeField] private SceneState sceneState; // drag/drop reference
    [SerializeField] private InputReader inputReader; // drag/drop reference

    [SerializeField] private CursorLockChannelSO cursorLockRequest; // drag/drop reference
    [SerializeField] private TeleportChannelSO teleportChannel; // drag/drop reference
    [SerializeField] private SaveEventChannelSO saveChannel; // drag/drop reference


    public static Action OnCreateDialogueTabs;
    public static Action OnCreateLocationData;
    public static Action OnStatBarInitalize;
    public static Action<Buildings> OnLocationsLoaded;
    public static Action OnCampusLoopStart;
    public static Action OnCampusLoopEnd;
    public static Action OnPositionSaveRequest;
    public static Action OnSetCamerasRequest;
    public static Action OnSetSun;
    public static Action OnSettingsInitialize;
    public static Action OnRequestCreateFirstWeek;
    public static Action<RunTimeWeek> OnRequestSetWeek;
    public static Action OnRequestSetWorldMapRefPoints;
    public static Action OnCreateNPCSpriteMap;
    public static Action OnSkyboxInit;
    public static Action OnRequestInitializeStatValueManager;

    public static Action OnRequestPlayerCharsList;
    public static Action OnRequestResetPlayerRotation;

    private void Awake()
    {
        CreateTransitionDict();

        if (sceneController == null) { sceneController = gameObject.GetComponent<AdditiveSceneController>(); }
        if (loadBarController == null) { loadBarController = FindObjectOfType<LoadingBarController>(); }

        if (saveSystem == null) { Debug.LogError("Didn't find save system"); }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void OnEnable()
    {
        LoadEventChannelSO.OnLoadingRequested += LoadScene;
    }

    private void OnDisable()
    {
        LoadEventChannelSO.OnLoadingRequested -= LoadScene;
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    private void CreateTransitionDict()
    {
        transitionsDict = new Dictionary<SceneTransition, LoadTransition>();
        transitionsDict.Clear();
        transitionsDict.Add(SceneTransition.Initial, new InitialTransition(this));
        transitionsDict.Add(SceneTransition.MainMenuToNewCampus, new NewGameTransition(this));
        transitionsDict.Add(SceneTransition.BuildingInteriorToCampus, new BuildingToCampusTransition(this));
        transitionsDict.Add(SceneTransition.TypingMinigame, new TypingMinigameTransition(this));
        transitionsDict.Add(SceneTransition.ItemSearchMiniGame, new ItemSearchMinigameTransition(this));
        transitionsDict.Add(SceneTransition.FoodMiniGame, new FoodMiniGameTransition(this));
        transitionsDict.Add(SceneTransition.DialogueMiniGame, new DialogueSceneTransition(this));
        transitionsDict.Add(SceneTransition.GameOver, new GameOverTransition(this));
    }

    private void InitializeGame()
    {
        StartCoroutine(loadFlowController.LoadRoutine(sceneController, transitionsDict[SceneTransition.Initial]));
    }

    private void LoadScene(SceneTransition transitionType, bool showLoadingScrene)
    {
        if (!transitionsDict.ContainsKey(transitionType)) { Debug.LogError($"GameFlowManager's Transition Dictionary does not contain an entry for {transitionType}"); }

        StartCoroutine(loadFlowController.LoadRoutine(sceneController, transitionsDict[transitionType]));
    }

    private void InitializeAnalytics() => GameAnalytics.Initialize();

    public void ResetData()
    {
        saveSystem.ResetData();
    }
    // Save
    public void SaveGame() => saveChannel.RequestSave();
    // Save
    public void RequestCreateDialogueTabs() => OnCreateDialogueTabs?.Invoke();
    // Save
    public void CreateLocationData() => OnCreateLocationData?.Invoke();
    // Save
    public void CreatePlayerSpawnData()
    {
        playerSpawnData = new PlayerSpawnData();
        playerSpawnController = new PlayerSpawnController(sceneState, playerSpawnData);
    }
    // Save
    public void UpdateSceneState() => sceneState.UpdateSceneState();
    // Save
    public void OpenLoadBar() => loadBarController.OpenLoadingBar();
    // Save
    public void CloseLoadBar() => loadBarController.CloseLoadingBar();
    // Save
    public void UnlockCursorRequest() => cursorLockRequest.RaiseCursorLockRequest(true); 
    // Save
    public void LockCursorRequest() => cursorLockRequest.RaiseCursorLockRequest(false);
    // Save
    public void RequestCampusLoopStart() => OnCampusLoopStart?.Invoke();
    public void RequestCampusLoopEnd() => OnCampusLoopEnd?.Invoke();
    // Save
    public void RequestResetPlayerRotation() => OnRequestResetPlayerRotation?.Invoke();
    // Save
    public void RequestStatBarInitialize() => OnStatBarInitalize?.Invoke();
    // Save
    public void RequestInitStatValueManager() => OnRequestInitializeStatValueManager?.Invoke();
    // Save
    public void RequestSavePlayerPosition() => OnPositionSaveRequest?.Invoke();
    // Save 
    public void ReturnPlayerStartLocation() 
    {
        PlayerSpawns spawnData = playerSpawnController.ReturnSpawnPoint(PlayerSpawnLocations.MainCampusInitial);
        Vector3 targetLocation = spawnData.SpawnTransform;
        float targetRotation = spawnData.TargetRotation;

        teleportChannel.RaiseTeleport(targetLocation, targetRotation);
    } 

    // Save
    public void RequestSetCameras() => OnSetCamerasRequest?.Invoke();
    // Save
    public void RequestSetSun() => OnSetSun?.Invoke();
    // Save
    public void RequestSettingsInitialize() => OnSettingsInitialize?.Invoke();
    // Save
    public void RequestCreateFirstWeek() => OnRequestCreateFirstWeek?.Invoke();
    public void RequestSetWeek() => OnRequestSetWeek?.Invoke(weekManager.CurrentWeek);
    // Save
    public void RequestExteriorLocations() => OnLocationsLoaded?.Invoke(Buildings.Exterior);
    // Save
    public void RequestSetWorldMapRefPoints() => OnRequestSetWorldMapRefPoints?.Invoke();
    // Save
    public void RequestPlayerCharList() => OnRequestPlayerCharsList?.Invoke();
    // Save
    public void DisableMovementInputs() => inputReader.DisableMovement();
    // Save
    public void SwitchToPlayerInputs() => inputReader.SwitchInputMap(inputReader.InputActionAsset.Player);
    // Save
    public void SwitchToUIInputs() => inputReader.SwitchInputMap(inputReader.InputActionAsset.UI);
    // Save
    public void RequestCreateNPCSpriteMap() => OnCreateNPCSpriteMap?.Invoke();
    // Save
    public void InitializeSkybox() => OnSkyboxInit?.Invoke();
}