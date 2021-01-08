using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpikePosition
{
    Left,
    Right,
    Top,
    Bottom
}

[System.Serializable]
public class SpikeData
{
    public Vector2 CellCoords;
    public SpikePosition Position;
}

public class SpikeObjectLogic : MonoBehaviour
{
    [SerializeField]
    private Transform _container;

    public void Initialize(SpikePosition position)
    {
        float orientation = 0.0f;
        switch (position)
        {
            case SpikePosition.Bottom:
                break;
            case SpikePosition.Left:
                orientation = -90.0f;
                break;
            case SpikePosition.Right:
                orientation = 90.0f;
                break;
            case SpikePosition.Top:
                orientation = 180.0f;
                break;
        }
        _container.localEulerAngles += new Vector3(0.0f, 0.0f, orientation);
    }
}
