using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactUiClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ArtifactMenu artifactMenu; // ����θ� �ڵ� Ž��
    [SerializeField] private bool hideAfterPick = true; // ���� �� ���� ������

    public void OnPointerClick(PointerEventData e)
    {
        var img = GetComponent<Image>();
        if (img == null || img.sprite == null) return;

        if (artifactMenu == null)
            artifactMenu = FindObjectOfType<ArtifactMenu>(true);

        if (artifactMenu != null && artifactMenu.TryAddArtifact(img.sprite))
        {
            if (hideAfterPick) gameObject.SetActive(false); // Ŭ���� UI ����(�Ǵ� Destroy)
        }
    }
}