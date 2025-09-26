using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public static bool IsCutscenePlaying { get; private set; }
    public static event Action<bool> OnCutsceneStateChanged;

    // 다른 스크립트에서 접근할 수 있도록 public으로 변경
    public Sprite ArtifactSpriteToCollect { get; private set; }
    public string ReturnSceneName { get; private set; }
    public bool needsAcquisitionCompletion { get; private set; } = false;

    // [추가] 오류 해결을 위해 아티팩트 ID를 저장할 변수
    public string collectedArtifactID { get; private set; }


    public enum CutsceneActionType { Move, Dialogue, Wait, Activate, Deactivate }

    [System.Serializable]
    public class CutsceneAction
    {
        public CutsceneActionType actionType;
        public string actionName;
        [Header("대상 오브젝트")]
        public GameObject targetObject;
        [Header("Move 액션")]
        public Transform targetPosition;
        public float moveDuration = 1.0f;
        [Header("Dialogue 액션")]
        public string dialogueID;
        [Header("Wait 액션")]
        public float waitDuration = 1.0f;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // [수정] 오류 해결을 위해 string artifactID 매개변수 추가
    public void SetFlashbackData(Sprite sprite, string returnScene, string artifactID)
    {
        ArtifactSpriteToCollect = sprite;
        ReturnSceneName = returnScene;
        collectedArtifactID = artifactID; // 전달받은 ID를 변수에 저장
        needsAcquisitionCompletion = true;
    }

    public void Play(List<CutsceneAction> actions)
    {
        if (IsCutscenePlaying) return;

        IsCutscenePlaying = true;
        OnCutsceneStateChanged?.Invoke(true);
        StartCoroutine(ExecuteCutscene(actions));
    }

    private IEnumerator ExecuteCutscene(List<CutsceneAction> actions)
    {
        foreach (CutsceneAction action in actions)
        {
            yield return StartCoroutine(ProcessAction(action));
        }
        EndCutscene();
    }

    private void EndCutscene()
    {
        IsCutscenePlaying = false;
        OnCutsceneStateChanged?.Invoke(false);

        if (!string.IsNullOrEmpty(ReturnSceneName))
        {
            string sceneToLoad = ReturnSceneName;
            SceneManager.LoadScene(sceneToLoad);
            Debug.Log($"컷신 종료! 원래 씬 ({sceneToLoad})으로 복귀합니다.");
        }
        else
        {
            Debug.LogWarning("컷신이 끝났지만 돌아갈 씬이 지정되지 않았습니다.");
        }
    }

    public void FinalizeAcquisition()
    {
        if (ArtifactSpriteToCollect == null) return;

        ArtifactMenu artifactMenu = FindObjectOfType<ArtifactMenu>(true);
        if (artifactMenu != null)
        {
            artifactMenu.TryAddArtifact(ArtifactSpriteToCollect);
        }
        else
        {
            Debug.LogError("ArtifactMenu를 찾을 수 없어 UI에 아이템을 추가하지 못했습니다!");
        }

        // 모든 임시 데이터를 깨끗하게 초기화합니다.
        ArtifactSpriteToCollect = null;
        ReturnSceneName = null;
        collectedArtifactID = null; // [추가] ID 데이터도 초기화
        needsAcquisitionCompletion = false;
        Debug.Log("아티팩트 획득 최종 처리 완료.");
    }

    private IEnumerator ProcessAction(CutsceneAction action)
    {
        switch (action.actionType)
        {
            case CutsceneActionType.Move:
                if (action.targetObject != null && action.targetPosition != null)
                {
                    yield return action.targetObject.transform.DOMove(action.targetPosition.position, action.moveDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
                }
                break;
            case CutsceneActionType.Dialogue:
                if (DialogueManager.Instance != null && !string.IsNullOrEmpty(action.dialogueID))
                {
                    DialogueManager.Instance.StartDialogue(action.dialogueID);
                    yield return new WaitUntil(() => !DialogueManager.Instance.isDialogue);
                }
                break;
            case CutsceneActionType.Wait:
                yield return new WaitForSeconds(action.waitDuration);
                break;
            case CutsceneActionType.Activate:
                if (action.targetObject != null) action.targetObject.SetActive(true);
                break;
            case CutsceneActionType.Deactivate:
                if (action.targetObject != null) action.targetObject.SetActive(false);
                break;
        }
    }
}

