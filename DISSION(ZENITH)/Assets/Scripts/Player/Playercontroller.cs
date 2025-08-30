using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playercontroller : MonoBehaviour
{
    private Rigidbody2D rb;

    //�÷��̾� �̵� �ӵ�
    public float moveSpeed = 3f;

    //��������Ʈ �̹���
    private SpriteRenderer spriteRenderer;

    //Ű �Է� ���� ���� x,y��
    private Vector2 movement;

    //���� �ٶ󺸴� ���� Front, Back, Left, Right
    private string currentDirection = "Front";

    //�ȱ� ��� ������ ��ȣ 0~3
    private int walkFrame = 0;
    //�ִϸ��̼� �� �ð� ������ ���� Ÿ�̸�
    private float animationTimer = 0f;
    //�������� �ٲ�� �ð�(0.1�ʸ��� ��ȯ)
    private float animationInterval = 0.1f; // �ִϸ��̼� ������ ���� (��)

    // ��ȭ ��� �� �÷��̾� �̵� ����
    private DialogueManager dialogueManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //��/�� ����Ű
        movement.x = Input.GetAxisRaw("Horizontal");
        //��/�� ����Ű
        movement.y = Input.GetAxisRaw("Vertical");

        //����Ű�� ������ ��
        if (movement != Vector2.zero && !dialogueManager.isDialogue &&
            !(MiniGameManager.Instance != null && MiniGameManager.Instance.IsMiniGameActive))
        {
            MovePlayer(); //���� �̵� ����
            AnimateWalk();//�ȴ� �ִϸ��̼� ���
        }
        else
        {
            SetIdleSprite();//�ƹ��͵� �ȴ����� �� ������ �� �ִ� �̹���
        }
    }

    //���� �̵� �����ϴ� �Լ�
    void MovePlayer()
    {
        transform.Translate(movement.normalized * moveSpeed * Time.deltaTime);
        UpdateDirection(); //���� �ٶ󺸴� ����(currentDirection) ����
    }

    //���� �ٶ󺸴� ���� �Ǻ�
    void UpdateDirection()
    {
        //x�� ������ > y�� ������ -> ��/�� �������� �Ǵ�
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            currentDirection = movement.x > 0 ? "Right" : "Left";
        }
        else
        {
            currentDirection = movement.y > 0 ? "Back" : "Front";
        }
    }

    //���� �ð�(animationInterval)���� ��������Ʈ�� ������ �Ȱ� ���̰� ��
    void AnimateWalk()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationInterval)
        {
            animationTimer = 0f;

            switch (walkFrame)
            {
                case 0:
                    SetSprite(currentDirection + "_Left");
                    break;
                case 1:
                    SetSprite(currentDirection + "_Idle");
                    break;
                case 2:
                    SetSprite(currentDirection + "_Right");
                    break;
                case 3:
                    SetSprite(currentDirection + "_Idle");
                    break;
            }

            walkFrame = (walkFrame + 1) % 4;
        }
    }

    //���� ������ �� �ִϸ��̼� �ʱ�ȭ
    void SetIdleSprite()
    {
        walkFrame = 0;
        animationTimer = 0f;
        SetSprite(currentDirection + "_Idle");
    }

    //Resources/PlayerSprites/ �������� ������ �̸��� ��������Ʈ�� �ε�
    void SetSprite(string spriteName)
    {
        Sprite sprite = Resources.Load<Sprite>("PlayerSprites/" + spriteName);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("��������Ʈ�� ã�� �� �����ϴ�: " + spriteName);
            //�ش� ��������Ʈ�� ���� �� ��� �޼���
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
