using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact Definition", menuName = "Artifacts/Artifact Definition")]
public class ArtifactDefinition : ScriptableObject
{
    [Header("아티팩트 정보")]
    [Tooltip("절대 중복되지 않는 고유 ID (예: Library_KeyArtifact)")]
    public string artifactID;         // 아티팩트 고유 ID (GameStateManager에 저장될 값)

    [Tooltip("아티팩트 메뉴 UI 등에 표시될 이름")]
    public string artifactName;       // 아티팩트 이름 (필요 없다면 생략 가능)

    [Tooltip("아티팩트 메뉴 UI에 표시될 스프라이트")]
    public Sprite artifactSprite;     // 아티팩트 아이콘/이미지

    [TextArea]
    [Tooltip("아티팩트에 대한 설명 (필요 없다면 생략 가능)")]
    public string description;        // 아티팩트 설명

    [Header("컷신 정보")]
    [Tooltip("이 아티팩트 획득 시 재생할 컷신 씬의 이름 (없으면 비워둠)")]
    public string flashbackSceneName; // 연결된 컷신 씬 이름

    [Space(10)]
    [Tooltip("true이면, 씬 이동 대신 인플레이스 이벤트를 재생")]
    public bool hasInPlaceEvent = false;

    [Tooltip("이벤트에서 보여줄 큰 이미지 (예: 편지)")]
    public Sprite eventImage;

    [Tooltip("이벤트에서 재생할 Dialogue ID")]
    public string eventDialogueID;
}