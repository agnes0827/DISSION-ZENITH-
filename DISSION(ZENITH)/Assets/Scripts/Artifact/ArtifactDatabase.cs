using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Artifact Database", menuName = "Artifacts/Artifact Database")]
public class ArtifactDatabase : ScriptableObject
{
    [Tooltip("게임에 존재하는 모든 아티팩트 Definition 파일을 여기에 등록하세요.")]
    public List<ArtifactDefinition> allArtifacts;

    public ArtifactDefinition GetArtifactByID(string id)
    {
        // 리스트에서 artifactID가 일치하는 첫 번째 항목을 찾음
        return allArtifacts.FirstOrDefault(artifact => artifact.artifactID == id);
    }
}