using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainMenuButton : MonoBehaviour {

    private EventTrigger et;
    private Button bt;
    private SFXManager sfxManagerInstance;

    void Start () {
        et = GetComponent<EventTrigger>();
        bt = GetComponent<Button>();
        if (sfxManagerInstance != null || SFXManager.TryGetInstance(out sfxManagerInstance))
        {
            EventTrigger.Entry deselect = new EventTrigger.Entry();
            deselect.eventID = EventTriggerType.Deselect;
            deselect.callback.AddListener(x => sfxManagerInstance.PlayMenuTraversalSound());
            et.triggers.Add(deselect);

            bt.onClick.AddListener(sfxManagerInstance.PlayMenuClickSound);
        }
    }
	
}
