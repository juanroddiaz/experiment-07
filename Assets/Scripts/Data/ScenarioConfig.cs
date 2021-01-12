using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayersChunkData
{
    public int LayersAmount;
    public PlatformDifficulty LowerDifficulty;
    public PlatformDifficulty UpperDifficulty;
}

[CreateAssetMenu(menuName = "My Assets/Scenario Config")]
public class ScenarioConfig : ScriptableObject
{
    public List<LayersChunkData> LayersData;
}
