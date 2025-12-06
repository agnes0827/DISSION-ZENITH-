using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ArtifactEventViewer : MonoBehaviour
{
    public static ArtifactEventViewer Instance { get; private set; }

    [Header("UI 연결")]
    [SerializeField] private GameObject panelRoot;   // 이 스크립트가 붙은 객체
    [SerializeField] private Image eventImageHolder;

    [Header("효과음")]
    [SerializeField] private AudioClip popupSound;   // [추가] 재생할 효과음 클립
    private AudioSource _audioSource;                // [추가] 소리를 낼 스피커 컴포넌트

    private Action _onEventCompleteCallback;
    private ArtifactDefinition _currentDef;
    private bool _isWaitingForKeyToCloseLetter = false; // 편지 닫기 대기 상태

    void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();

        if (panelRoot == null)
        {
            panelRoot = this.gameObject;
        }

        panelRoot.SetActive(false);
    }

    void Update()
    {
        if (!_isWaitingForKeyToCloseLetter) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogue)
        {
            _isWaitingForKeyToCloseLetter = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isWaitingForKeyToCloseLetter = false;

            if (eventImageHolder != null)
            {
                eventImageHolder.gameObject.SetActive(false);
            }

            DialogueManager.Instance.StartDialogue(_currentDef.eventDialogueID, this.gameObject);
            StartCoroutine(WaitForDialogueEnd());
        }
    }

    public void ShowEvent(ArtifactDefinition def, Action onComplete)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager가 씬에 없습니다!");
            onComplete?.Invoke();
            return;
        }

        // 콜백 저장
        _onEventCompleteCallback = onComplete;
        _currentDef = def;

        // 이미지 설정
        if (eventImageHolder != null)
        {
            eventImageHolder.sprite = def.eventImage;
            eventImageHolder.gameObject.SetActive(def.eventImage != null);
        }

        // 패널 활성화
        panelRoot.SetActive(true);
        _isWaitingForKeyToCloseLetter = true;

        // 효과음 재생
        PlayPopupSound();
    }

    private void PlayPopupSound()
    {
        if (_audioSource != null && popupSound != null)
        {
            _audioSource.PlayOneShot(popupSound);
        }
    }

    private IEnumerator WaitForDialogueEnd()
    {
        yield return null;

        // isDialogue가 true인 동안 계속 대기
        while (DialogueManager.Instance.isDialogue)
        {
            yield return null;
        }

        panelRoot.SetActive(false);

        // 콜백(아티팩트 등록)을 실행합니다.
        _onEventCompleteCallback?.Invoke();
        _onEventCompleteCallback = null;
    }
}