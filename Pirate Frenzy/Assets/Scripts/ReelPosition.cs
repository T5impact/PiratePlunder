using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReelPositionType
{
    Seven,
    Bar,
    Lemon
}

public class ReelPosition : MonoBehaviour
{
    [SerializeField] private ReelPositionType reelPositionType;
    public ReelPositionType ReelPositionType { get => reelPositionType; }
}
