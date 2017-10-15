using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    protected abstract Enumerables.EPowerUp PowerUpType { get; }

    protected abstract Color ShieldColor { get; }
}
