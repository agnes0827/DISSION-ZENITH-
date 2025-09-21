using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour
{
    // 1. 싱글톤 정의 (다른 곳에서 쉽게 접근 가능)
    public static CutsceneManager Instance { get; private set; }
    public static bool IsCutscenePlaying { get; private set; }

    // 2. 컷씬 상태 변경 이벤트 (PlayerController와 DialogueInteraction에서 구독)
    public static event Action<bool> OnCutsceneStateChanged;

    // 3. 수집할 아티팩트 데이터 저장 필드
    private Sprite artifactSpriteToCollect;

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

    [Header("전체 컷신 시퀀스")]
    public List<CutsceneAction> actions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 4. Play 메서드 수정: 아티팩트 스프라이트 데이터를 받습니다.
    public void Play(Sprite artifactSprite)
    {
        if (IsCutscenePlaying) return;

        artifactSpriteToCollect = artifactSprite;
        IsCutscenePlaying = true;

        // 5. 이벤트 발행: 플레이어 조작 잠금 요청
        OnCutsceneStateChanged?.Invoke(true);

        StartCoroutine(ExecuteCutscene());
    }

    private IEnumerator ExecuteCutscene()
    {
        foreach (CutsceneAction action in actions)
        {
            yield return StartCoroutine(ProcessAction(action));
        }
        EndCutscene();
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

    private void EndCutscene()
    {
        // 1. 컷신 종료 후 아티팩트 UI에 등록
        ArtifactMenu artifactMenu = FindObjectOfType<ArtifactMenu>();
        if (artifactMenu != null && artifactSpriteToCollect != null)
        {
            artifactMenu.TryAddArtifact(artifactSpriteToCollect);
            Debug.Log($"[Cutscene] 아티팩트 '{artifactSpriteToCollect.name}' UI에 등록 완료.");
        }

        // 2. 상태 해제 및 이벤트 발행
        IsCutscenePlaying = false;
        OnCutsceneStateChanged?.Invoke(false);

        Debug.Log("컷신 종료!");
    }
}