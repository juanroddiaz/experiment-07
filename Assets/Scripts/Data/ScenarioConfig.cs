using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Scenario Config")]
public class ScenarioConfig : ScriptableObject
{
    public List<PlatformLayerLogic> ScenarioData;
}
