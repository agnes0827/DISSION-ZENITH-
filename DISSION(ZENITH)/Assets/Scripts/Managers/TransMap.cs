using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransMap : MonoBehaviour
{
    public string sceneToLoad; // 로드할 씬 번호
    private FadeManager theFade;
    private PlayerController1 thePlayer;

    void Start()
    {
        theFade = FindObjectOfType<FadeManager>();
        thePlayer = FindObjectOfType<PlayerController1>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.name == "player")
        {
            thePlayer.currentMapName = sceneToLoad;
            StartCoroutine(TransferCoroutine());
        }
        
    }

    IEnumerator TransferCoroutine()
    {
        
        theFade.FadeOut();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneToLoad); // 지정된 번호의 씬으로 이동

        theFade.FadeIn();
        
    }

}
