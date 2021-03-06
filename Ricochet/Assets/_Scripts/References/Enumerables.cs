﻿namespace Enumerables
{
    public enum BuildIndex
    {
        MAIN_MENU,
        CHARACTER_SELECT,
        LEVEL_SELECT,
        CONTROLLER_MAP,
        DIAGONAL_ALLEY,
        ELEVATOR,
        UP_N_OVER_WIDE,
        EMPTY_LEVEL,
        END_GAME,
        RANDOM
    }

    public enum EPowerUp
    {
        None = -1,
        Random = 0,
        Multiball = 1,
        CatchNThrow = 2,
        CircleShield = 3,
	    Freeze = 4,
	    Shrink = 5
    }

    public enum EMode
    {
        None = -1,
        Soccer = 0,
        Deathmatch = 1
    }

    public enum ETeam
    {
        None = -1,
        RedTeam = 0,
        BlueTeam = 1
    }

    public enum ECharacter
    {
        None = 0,
        Cat = 1,
        Computer = 2,
        MallCop = 3,
        Sushi = 4
    }

    public enum ECharacterAction
    {
        None = -1,
        Jetpack = 0,
        Dash = 1,
        Death = 3
    }
}