using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//원하는 씬 이름 쓰기

public class Transmap : MonoBehaviour
{
    public string sceneToLoad; // 로드할 씬 번호
    //private FadeManager theFade; //페이드메니저 호출
    private PlayerController thePlayer; //플레이어 호출

    void Start()
    {
        //theFade = FindObjectOfType<FadeManager>();
        thePlayer = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //충돌 시 오브젝트 이름이 플레이어면 StartCoroutine(TransferCoroutine()) 호출
        if (collision.gameObject.name == "Player")
        {
            thePlayer.currentMapName = sceneToLoad;
            StartCoroutine(TransferCoroutine());
        }

    }

    IEnumerator TransferCoroutine()
    {

        //theFade.FadeOut();
        yield return new WaitForSeconds(1f);
        Debug.Log(sceneToLoad + "로 이동");
        SceneManager.LoadScene(sceneToLoad); // 지정된 번호의 씬으로 이동

        //theFade.FadeIn();

    }
}
