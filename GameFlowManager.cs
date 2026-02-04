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
    [SerializeField] LoadFlowController loadFlowController;
    [SerializeField] WeekManager weekManager;

    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private SceneState sceneState;
    [SerializeField] private InputReader inputReader;

    [SerializeField] private CursorLockChannelSO cursorLockRequest;
    [SerializeField] private TeleportChannelSO teleportChannel;
    [SerializeField] private SaveEventChannelSO saveChannel;


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

    public void SaveGame() => saveChannel.RequestSave();
    
    public void RequestCreateDialogueTabs() => OnCreateDialogueTabs?.Invoke();
    
    public void CreateLocationData() => OnCreateLocationData?.Invoke();
    
    public void CreatePlayerSpawnData()
    {
        playerSpawnData = new PlayerSpawnData();
        playerSpawnController = new PlayerSpawnController(sceneState, playerSpawnData);
    }
    
    public void UpdateSceneState() => sceneState.UpdateSceneState();
    
    public void OpenLoadBar() => loadBarController.OpenLoadingBar();
    
    public void CloseLoadBar() => loadBarController.CloseLoadingBar();
    
    public void UnlockCursorRequest() => cursorLockRequest.RaiseCursorLockRequest(true); 
    
    public void LockCursorRequest() => cursorLockRequest.RaiseCursorLockRequest(false);
    
    public void RequestCampusLoopStart() => OnCampusLoopStart?.Invoke();
    public void RequestCampusLoopEnd() => OnCampusLoopEnd?.Invoke();
    
    public void RequestResetPlayerRotation() => OnRequestResetPlayerRotation?.Invoke();
    
    public void RequestStatBarInitialize() => OnStatBarInitalize?.Invoke();
    
    public void RequestInitStatValueManager() => OnRequestInitializeStatValueManager?.Invoke();
    
    public void RequestSavePlayerPosition() => OnPositionSaveRequest?.Invoke();
     
    public void ReturnPlayerStartLocation() 
    {
        PlayerSpawns spawnData = playerSpawnController.ReturnSpawnPoint(PlayerSpawnLocations.MainCampusInitial);
        Vector3 targetLocation = spawnData.SpawnTransform;
        float targetRotation = spawnData.TargetRotation;

        teleportChannel.RaiseTeleport(targetLocation, targetRotation);
    } 

    
    public void RequestSetCameras() => OnSetCamerasRequest?.Invoke();
    
    public void RequestSetSun() => OnSetSun?.Invoke();
    
    public void RequestSettingsInitialize() => OnSettingsInitialize?.Invoke();
    
    public void RequestCreateFirstWeek() => OnRequestCreateFirstWeek?.Invoke();
    public void RequestSetWeek() => OnRequestSetWeek?.Invoke(weekManager.CurrentWeek);
    
    public void RequestExteriorLocations() => OnLocationsLoaded?.Invoke(Buildings.Exterior);
    
    public void RequestSetWorldMapRefPoints() => OnRequestSetWorldMapRefPoints?.Invoke();
    
    public void RequestPlayerCharList() => OnRequestPlayerCharsList?.Invoke();
    
    public void DisableMovementInputs() => inputReader.DisableMovement();
    
    public void SwitchToPlayerInputs() => inputReader.SwitchInputMap(inputReader.InputActionAsset.Player);
    
    public void SwitchToUIInputs() => inputReader.SwitchInputMap(inputReader.InputActionAsset.UI);
    
    public void RequestCreateNPCSpriteMap() => OnCreateNPCSpriteMap?.Invoke();
    
    public void InitializeSkybox() => OnSkyboxInit?.Invoke();
}