using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public GameObject inventory; // inventory ��ü ���⿡ �ֱ�
    private bool activated; // inventory ȭ�� Ȱ��ȭ/��Ȱ��ȭ

    public void Continue()
    {
        activated = false; // activated ���� ����
        inventory.SetActive(false); // inventory ȭ�� ��Ȱ��ȭ
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Tab Ű Ŭ�� ��
        {
            activated = !activated; // activated ���� ����

            if (activated)
            {
                inventory.SetActive(true); // inventory ȭ�� Ȱ��ȭ
            }
            else
            {
                inventory.SetActive(false); // inventory ȭ�� ��Ȱ��ȭ
            }
        }
    }
}
