using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider2;
    public int nextMove;  //-1, 0, 1

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2 = GetComponent<CapsuleCollider2D>();
        ReserveThink();
    }

    private void ReserveThink()
    {
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    //재귀함수
    private void Think()
    {
        nextMove = Random.Range(-1, 2);
        anim.SetInteger("walkSpeed", nextMove);

        if (nextMove != 0)
            spriteRenderer.flipX = (nextMove == 1);
        ReserveThink();
    }

    private void FixedUpdate() {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //지형 체크
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVector, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 1, 
            LayerMask.GetMask("Platform"));
        if (rayHit.collider == null) {
            //print("위험한 낭떨어지!!");
            nextMove *= -1;
            spriteRenderer.flipX = (nextMove == 1);

            CancelInvoke();
            ReserveThink();
        }
    }

    public void OnDamaged()
    {
        //에너미 색깔 
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        
        //에어미 뒤집기
        spriteRenderer.flipY = true;

        //에너미 바닥으로 낙하
        capsuleCollider2.enabled = false;

        //에너미 위로 튕기기
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);

        Invoke("OffDamaged", 5);
    }

    void OffDamaged()
    {
        gameObject.SetActive(false);
    }

}
