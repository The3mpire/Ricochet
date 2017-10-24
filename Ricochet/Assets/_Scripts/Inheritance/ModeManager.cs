﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enumerables;

public abstract class ModeManager : MonoBehaviour
{
    public abstract void UpdateScore(Enumerables.ETeam team, int value);
}