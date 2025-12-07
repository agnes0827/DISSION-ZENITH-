using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D 제어용

public class TorchPickup : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    [Tooltip("플레이어 자식으로 달려있는 Light 2D 오브젝트를 여기에 넣으세요")]
    public GameObject playerLightObject;

    private bool isPlayerNearby = false;

    private void Start()
    {
        if (playerLightObject == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // 플레이어 자식 중에 "TorchLight"라는 이름의 오브젝트를 찾음
                Transform t = player.transform.Find("TorchLight");
                if (t != null) playerLightObject = t.gameObject;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        // 플레이어가 근처에 있고 + F키를 눌렀을 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            GetTorch();
        }
    }

    void GetTorch()
    {
        SoundManager.Instance.PlaySFX(SfxType.Torch, 0.5f);
        if (playerLightObject != null)
        {
            playerLightObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}