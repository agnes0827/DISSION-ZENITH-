using UnityEngine;
using Cinemachine;
using System.Collections;

public class BattleIntroSequence : MonoBehaviour
{
    [Header("몬스터 정보")]
    [SerializeField] private string monsterId = "LibraryBoss";

    [Header("카메라 설정")]
    [SerializeField] private CinemachineVirtualCamera playerFollowCam;
    [SerializeField] private CinemachineVirtualCamera monsterFocusCam;
    [SerializeField] private GameObject monsterObject; // 보스 몬스터 오브젝트

    [Header("연출 설정")]
    [SerializeField] private float focusDelay = 0.5f;        // 씬 로드 후 포커스 시작까지 대기 시간
    [SerializeField] private float focusDuration = 3.5f;     // 몬스터 포커스 유지 시간

    private bool sequenceDone = false;

    IEnumerator Start()
    {
        if (playerFollowCam == null || monsterFocusCam == null || monsterObject == null)
        {
            Debug.LogError("BattleIntroSequence: 필요한 카메라 또는 몬스터 오브젝트가 연결되지 않았습니다!");
            yield break;
        }

        // 보스 처치 여부 확인
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsMonsterDefeated(monsterId))
        {
            yield break;
        }

        // --- 연출 시작 ---
        if (sequenceDone) yield break; // 중복 실행 방지
        sequenceDone = true;

        // 0. 초기 카메라 우선순위 설정 확인 (플레이어 카메라가 더 높아야 함)
        int originalPlayerPriority = playerFollowCam.Priority;
        int originalMonsterPriority = monsterFocusCam.Priority;
        if (originalPlayerPriority <= originalMonsterPriority)
        {
            // 플레이어 카메라 우선순위를 높여서 시작은 플레이어를 비추도록 함
            playerFollowCam.Priority = originalMonsterPriority + 1;
            originalPlayerPriority = playerFollowCam.Priority; // 변경된 값 저장
        }

        PlayerController.Instance?.StopMovement();

        // 1. 짧게 대기 후 몬스터에게 카메라 포커스 이동
        yield return new WaitForSeconds(focusDelay);
        monsterFocusCam.Priority = originalPlayerPriority + 1; // 몬스터 카메라 우선순위를 더 높여 활성화

        // 2. 몬스터를 잠시 보여줌
        yield return new WaitForSeconds(focusDuration);

        // 3. 다시 플레이어 카메라로 복구
        monsterFocusCam.Priority = originalMonsterPriority; // 원래 우선순위로 되돌림 (플레이어 카메라가 다시 활성화됨)

        // 플레이어 이동 다시 가능하게
        PlayerController.Instance?.ResumeMovement();
    }
}
