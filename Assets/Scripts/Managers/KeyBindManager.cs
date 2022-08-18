using UnityEngine;

[System.Serializable]
public struct KeyBind
{
    public string keyAction;
    public ItemType type;
    public KeyCode key;
    public float duration;
}

public class KeyBindManager : MonoBehaviour
{
    private EquipmentManager handler;
    public KeyBind[] keyBinds;

    private void Awake()
    {
        handler = GetComponent<EquipmentManager>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        CheckForKey();
    }

    public void CheckForKey()  //MAYBE MAKE MORE EFFICIENT LATER
    {
        if (handler.equipedItem == null || Cursor.lockState == CursorLockMode.None)
            return;

        foreach (KeyBind k in keyBinds)
        {
            if (k.type == handler.equipedItem.itemType)
            {
                if (Input.GetKeyDown(k.key))
                {
                    PlayAnimation(k);
                }
            }
        }
    }

    public void PlayAnimation(KeyBind bind)
    {
        if (handler.activeAnimator == null)
        {
            Debug.LogWarning("Animator not found in equiped item <Unable to play animation>");
            return;
        }
        handler.activeAnimator.CrossFade(bind.keyAction, bind.duration);
    }

}
