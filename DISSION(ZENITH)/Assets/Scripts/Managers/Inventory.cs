using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventory;
    private bool activated;

    public void ToggleInventory()
    {
        activated = !activated; //   (<->)

        SoundManager.Instance.PlaySFX(SfxType.UISelect, 0.7f, false);

        if (activated)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        activated = false;
        if (PlayerController.Instance != null)
            PlayerController.Instance.ResumeMovement();

        inventory.SetActive(false);
    }

    public void OpenInventory()
    {
        activated = true;
        if (PlayerController.Instance != null)
            PlayerController.Instance.StopMovement();

        inventory.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (activated) CloseInventory();
            else OpenInventory();

            SoundManager.Instance.PlaySFX(SfxType.UISelect, 0.7f, false);
        }
    }
}
