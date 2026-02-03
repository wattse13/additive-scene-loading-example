# additive-scene-loading-example

I developed these scripts to handle scene transitions for a Unity WebGL project that utilizes an additive scene architecture.

Please note, that some of these scripts, `GameFlowManager.cs` in particular, have been shortened or may reference scripts that are not present in this repo. This was done on purpose to make the example code a bit more concise.


## `GameFlowManager`

This script is the entry point for the game. Its primary responsibility is to respond to scene transition requests.

To do this, it first creates a dictionary of `SceneTransition` enum keys and LoadTransition values. When `GameFlowManager` receives a scene transition request event, that event is invoked with a `SceneTransition` enum argument. That enum argument is used to look up the corresponding `LoadTransition` instance value. The scene loading coroutine in the `LoadFlowController` class can then be started with the `LoadTransition` instance value as an argument.


## `LoadTransition`

Instances of this abstract class are used to define:
- What scenes need to be loaded together
- Which scene needs to be set as the active scene
- What events need to be invoked at specified points in the loading/unloading process

Instances of `LoadTransition` are created by `GameFlowManager`. When passed into the `LoadFlowController` as an argument, the `LoadFlowController` checks what interfaces the `LoadTransition` instance implements. The implemented interfaces inform `LoadFlowController` if the `LoadTransition` instance has any actions that need to be carried out at a specified point in the load/unload process.

`NewGameTransition` and `InitialTransition` are both examples of `LoadTransition` instances.


## `LoadFlowController`

This script's only function is to use a coroutine to step through the loading/unloading process. It takes an instance of `LoadTranstion` as an argument. There are four points in the load/unload process when events can be invoked. They are:
- Before a scene is unloaded
- After a scene is unloaded
- Before assets are unloaded
- After assets are unloaded

At each point of the load/unload process `LoadFlowController` checks which interfaces the `LoadTransistion` instance implements. If the instance implements an interface that corresponds to one of the four points, it will pause the load/unload process, iterate across all elements of the action list held by the `LoadTransition` instance, and then move on to the next portion of the load/unload process. 


## `AdditiveSceneController`

This script is used by `LoadFlowController` to carry out the actual loading/unloading of scenes using Unity's `SceneManagement` namespace. Before loading/unloading scenes it will carry out checks that include:
- Making sure the given scene is valid
- Making sure global scenes like Management or Audio are never unloaded
