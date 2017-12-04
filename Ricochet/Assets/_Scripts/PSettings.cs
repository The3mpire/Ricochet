using Enumerables;

internal class PSettings
{
    public ECharacter Character { get; set; }

    public ETeam Team { get; set; }


    public PSettings()
    {
        Character = ECharacter.None;
        Team = ETeam.None;
    }
    public PSettings(ECharacter character, ETeam team)
    {
        Character = character;
        Team = team;
    }
}