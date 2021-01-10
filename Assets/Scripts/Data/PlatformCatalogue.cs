using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformDifficulty
{ 
    Easy = 0,
    Medium,
    Hard,
    DarkSouls,
}

[System.Serializable]
public class PlatformAmountPerLayer
{
    public PlatformDifficulty Difficulty;
    public int Amount;
}

[System.Serializable]
public class PlatformConfig
{
    public string Name;
    public PlatformDifficulty Difficulty;
    public GameObject Asset;
}

[CreateAssetMenu(menuName = "My Assets/Platform Catalogue Config")]
public class PlatformCatalogue : ScriptableObject
{
    public List<PlatformConfig> Platforms;
    public List<PlatformAmountPerLayer> LayerData;
}
