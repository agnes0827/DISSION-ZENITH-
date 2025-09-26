using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transmap : MonoBehaviour
{
    public string sceneToLoad; // �ε��� �� ��ȣ
    private FadeManager theFade; //���̵�޴��� ȣ��
    private PlayerController thePlayer; //�÷��̾� ȣ��

    void Start()
    {
        theFade = FindObjectOfType<FadeManager>();
        thePlayer = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�浹 �� ������Ʈ �̸��� �÷��̾�� StartCoroutine(TransferCoroutine()) ȣ��
        if (collision.gameObject.name == "Player")
        {
            thePlayer.currentMapName = sceneToLoad;
            StartCoroutine(TransferCoroutine());
        }

    }

    IEnumerator TransferCoroutine()
    {

        theFade.FadeOut();
        yield return new WaitForSeconds(1f);
        Debug.Log(sceneToLoad + "�� �̵�");
        SceneManager.LoadScene(sceneToLoad); // ������ ��ȣ�� ������ �̵�

        theFade.FadeIn();

    }
}
