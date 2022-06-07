using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 3;
    public float jumpPower= 5;
    public GameManager gameManager;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        //Jump
        if (Input.GetButtonDown("Jump") && animator.GetBool("isJumping") == false) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }

        //Stop 속도
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Sprite 방향
        if (Input.GetButton("Horizontal")) {
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") == -1);
        }

        //애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3 ) {
            animator.SetBool("isWalking", false);
        } else {
            animator.SetBool("isWalking", true);
        }
    }

    public void die()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
    }

    void FixedUpdate() {
        //키보드로 움직이기
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * rigid.gravityScale, ForceMode2D.Impulse);

        //최고 속도 제한하기
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < (maxSpeed * -1))
            rigid.velocity = new Vector2(maxSpeed * -1, rigid.velocity.y);

        //Platform 착지 검사
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, 
                LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) {
                animator.SetBool("isJumping", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            bool isEnemy = other.gameObject.name.StartsWith("Enemy");
            bool isFallEnemy = rigid.velocity.y < 0 && 
                transform.position.y > other.transform.position.y;
            if (isEnemy && isFallEnemy)
                OnAttack(other.transform); //enemy
            else 
                OnDamaged(other.transform);  //enemy + spike
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Item")
        {
            gameManager.AddStagePoint(other.gameObject.name);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
        }
    }

    private void OnAttack(Transform enemyTransform)
    {
        gameManager.AddStagePoint("Enemy");

        EnemyMove enemyMove = enemyTransform.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    private void OnDamaged(Transform enemyTransform)
    {
        gameManager.HealthDown();

        //무적 레이어로 변경
        gameObject.layer = 11;

        //색 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //맞으면 튕기는 반응(넉백)
        //float enemyPostion = collision.transform.position.x;
        float enemyPostion = enemyTransform.position.x;
        float playerPostion = transform.position.x;
        int dirc = playerPostion < enemyPostion ? -1 : 1;
        enabled = false;
        rigid.AddForce(new Vector2(dirc, 1) * rigid.gravityScale * 2, ForceMode2D.Impulse);
        Invoke("EnableTrue", 0.3f);
        animator.SetTrigger("doDamaged");

        //3초 후 복귀
        Invoke("OffDamanaged", 3);
    }

    void EnableTrue()
    {
        enabled = true;
    }

    void OffDamanaged()
    {
        //색 변경
        spriteRenderer.color = new Color(1, 1, 1, 1);

        //무적 레이어로 변경
        gameObject.layer = 10;
    }

    public void respown()
    {
        rigid.velocity = Vector2.zero;
        spriteRenderer.flipY = false;
        transform.position = new Vector3(-2, 5, 0);
    }
}
