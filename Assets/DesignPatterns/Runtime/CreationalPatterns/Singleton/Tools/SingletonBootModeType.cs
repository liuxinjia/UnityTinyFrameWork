
using UnityEngine;

public enum SingletonBootModeType
{
    /// <summary>
    /// The singleton will be created before the first scene of the application is loaded.
    /// </summary>
    [InspectorName("Default")]
    [Tooltip("The singleton will be created before the first scene of the application is loaded.")]
    GAMESTART,
    /// <summary>
    /// The singleton will be created when a specified scene is loaded and destroyed when
    /// the this scene is unloaded.
    /// </summary>
    [InspectorName("Scene")]
    [Tooltip("The singleton will be created when a specified scene is loaded and destroyed when the this scene is unloaded.")]
    SCENESCOPE,

}