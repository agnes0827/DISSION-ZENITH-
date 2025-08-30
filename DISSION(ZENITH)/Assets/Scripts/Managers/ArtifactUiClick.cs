using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArtifactUiClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ArtifactMenu artifactMenu; // 비워두면 자동 탐색
    [SerializeField] private bool hideAfterPick = true; // 넣은 뒤 원본 숨길지

    public void OnPointerClick(PointerEventData e)
    {
        var img = GetComponent<Image>();
        if (img == null || img.sprite == null) return;

        if (artifactMenu == null)
            artifactMenu = FindObjectOfType<ArtifactMenu>(true);

        if (artifactMenu != null && artifactMenu.TryAddArtifact(img.sprite))
        {
            if (hideAfterPick) gameObject.SetActive(false); // 클릭된 UI 숨김(또는 Destroy)
        }
    }
}