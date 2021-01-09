using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenarioDifficultyData
{
    public float Height = 0;
    public List<BasePlatformLogic> PlatformsAvailable;    
}

[CreateAssetMenu(menuName = "My Assets/Scenario Config")]
public class ScenarioConfig : ScriptableObject
{
    public List<ScenarioDifficultyData> ScenarioData;
}
