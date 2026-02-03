using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At a minimum all LoadTransition instances need
/// - A list of scenes to be combined
/// - A scene from the list of scenes to be made into the active scene
/// </summary>

public abstract class LoadTransition 
{
    public abstract string ActiveScene { get; }
    public abstract List<string> ScenesToKeep { get; }
}
