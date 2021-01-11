using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLayerLogic : MonoBehaviour
{
    [SerializeField]
    private PlatformDifficulty _minimumDifficulty;
    [SerializeField]
    private PlatformDifficulty _maximumDifficulty;

    private ScenarioController _controller;

    public void Initialize(ScenarioController controller, int layerIndex)
    {
        _controller = controller;
        // get difficulty platforms amount
        // if different, get a random amount between the two
        // instantiate platforms between the position constrains
    }
}
