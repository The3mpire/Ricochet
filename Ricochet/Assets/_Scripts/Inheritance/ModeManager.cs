﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enumerables;

public abstract class ModeManager : MonoBehaviour
{
    public abstract bool UpdateScore(Enumerables.ETeam team, int value);

    public abstract bool AltUpdateScore(ETeam team, int value);

    public abstract ETeam GetMaxScore();
}
