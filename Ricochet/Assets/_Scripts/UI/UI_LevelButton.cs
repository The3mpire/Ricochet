using Enumerables;
using UnityEngine;

public class UI_LevelButton : UI_MenuButton
{
    [SerializeField] private BuildIndex buildIndex;

    private LevelSelectManager manager;
    
    public BuildIndex GetBuildIndex()
    {
        return buildIndex;
    }

    public void MyClick()
    {
        manager.SetLoadLevel(buildIndex);
    }

    public void SetManager(LevelSelectManager manager)
    {
        this.manager = manager;
    }
}
