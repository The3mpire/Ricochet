using Enumerables;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_LevelButton : UI_MenuButton, ISelectHandler
{
    [SerializeField] private BuildIndex buildIndex;
    [SerializeField] private UI_PreviewPanel previewPanel;

    private LevelSelectManager manager;

    public new void OnSelect(BaseEventData eventData)
    {
        text.color = Color.white;
        previewPanel.ChangeSprite(buildIndex);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        text.color = Color.black;
    }

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
