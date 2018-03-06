using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainMenuButton : MonoBehaviour {

    private EventTrigger et;
    private Button bt;
    private MusicManager musicManagerInstance;

    void Start () {
        et = GetComponent<EventTrigger>();
        bt = GetComponent<Button>();
        if (musicManagerInstance != null || MusicManager.TryGetInstance(out musicManagerInstance))
        {
            EventTrigger.Entry deselect = new EventTrigger.Entry();
            deselect.eventID = EventTriggerType.Deselect;
            deselect.callback.AddListener(x => musicManagerInstance.PlayMenuTraversalSound());
            et.triggers.Add(deselect);

            bt.onClick.AddListener(musicManagerInstance.PlayMenuClickSound);
        }
    }
	
}
