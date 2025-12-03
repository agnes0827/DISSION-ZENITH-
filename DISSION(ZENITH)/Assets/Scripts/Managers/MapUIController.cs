using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    [System.Serializable]
    public class SceneMap
    {
        public string sceneName;   // 씬 이름 
        public Sprite mapSprite;   // 해당 씬의 지도 스프라이트
    }

    [Header("지도 이미지를 표시할 UI 이미지")]
    public Image mapImage;

    [Header("씬 이름 - 지도 스프라이트 매핑")]
    public List<SceneMap> sceneMaps = new List<SceneMap>();

    private void OnEnable()
    {
        // 지도 탭이 켜질 때마다 현재 씬 기준으로 지도 이미지 갱신
        UpdateMapForCurrentScene();
    }

    public void UpdateMapForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        foreach (var sceneMap in sceneMaps)
        {
            if (sceneMap.sceneName == currentSceneName)
            {
                mapImage.sprite = sceneMap.mapSprite;
                mapImage.enabled = true;
                return;
            }
        }

        // 해당 씬에 맞는 지도가 없으면 숨기거나 기본 이미지로 변경
        mapImage.enabled = false;
        Debug.LogWarning($"[MapUIController] {currentSceneName} 씬에 대응하는 지도 스프라이트가 없습니다.");
    }
}
