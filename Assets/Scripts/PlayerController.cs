using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    public float jumpForce = 20f;
    private float defaultJumpForce = 0;
    public float maxJumpForce = 30f;
    public float wallJumpForce;
    private float jumpCharge;
    public float dashForce = 15f;
    public float maxDashForce = 15f;
    public float stealthForce = 5f;
    public float maxStealthForce = 5f;
    public float groundGravityScale = 0;//重力
    private bool isGrounded;
    private bool isWalled;
    public float maxDownVelocityY = -30f;
    public float braekPower = 0.2f;//摩擦
    private bool isRight;//移動方向判定
    private Vector3 startPosition;//初期位置
    private Vector3 nowPosition;//現在位置
    private Vector3 wallPosition;//ぶつかった壁の位置
    

    void Start()
    {

        Application.targetFrameRate = 60;
        //アタッチされたオブジェクトの
        //Rigidbody2Dコンポーネントを呼び出す
        this.rigid2D = GetComponent<Rigidbody2D>();
        //defaultJumpForceに初期値を設定
        defaultJumpForce = jumpForce;
        //playerの初期位置を取得
        this.startPosition = GetComponent<Transform>().position;
    }


    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    //押下時にrigid2D内のRigidbody2DのAddForceメソッドを
        //    //呼び出す
        //    this.rigid2D.AddForce(transform.up * this.jumpForce);
        //}
        if (Input.GetKey(KeyCode.UpArrow))
        {
            
            if(jumpCharge < 1)//長押し判定
            {
                jumpCharge += 0.1f;
            }
        }

        int jumpKey=0;
        if (Input.GetKeyUp(KeyCode.UpArrow) && isGrounded==true)
        {
            if (isWalled == true && Input.GetKey(KeyCode.LeftShift))//壁ジャンプ
            {
                if (this.wallPosition.x > nowPosition.x)
                {
                    jumpKey = -1;
                }
                if (this.wallPosition.x < nowPosition.x)
                {
                    jumpKey = 1;
                }
                if (jumpCharge > 1)
                {

                }
                    Vector2 jump = new Vector2(jumpKey * maxDashForce, maxJumpForce);
                this.rigid2D.velocity = jump;
                jumpCharge = 0;
                return;
            }
            if (jumpCharge > 1)//大ジャンプ
            {
                Vector2 jump = new Vector2(rigid2D.velocity.x, maxJumpForce);
                this.rigid2D.velocity = jump;
                //jumpForce = defaultJumpForce;
                jumpCharge = 0;
            }
            else//小ジャンプ
            {
                Vector2 jump = new Vector2(rigid2D.velocity.x, jumpForce);
                this.rigid2D.velocity = jump;
                jumpCharge = 0;
            }

            //this.rigid2D.AddForce(jump);

            //AddForceからVelocityに変更
            //直接速度の数値を書き換える

            //動いてる向きに応じて真理値判定
            
            
        }
        if (rigid2D.velocity.x > 0)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow)) key = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) key = -1;

        //条件文用の変数(Absは絶対値の意)
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        if (!Input.GetKey(KeyCode.RightShift))
        {
            if (speedx < this.maxDashForce)
            {
                this.rigid2D.velocity = new Vector2(key * this.dashForce, rigid2D.velocity.y);
                    //( * transform.right);
            
                Debug.Log("go");
            }
        }
        else
        {
            if (speedx < this.maxStealthForce)
            {
                //this.rigid2D.AddForce
                //    (key * this.stealthForce * transform.right);
                this.rigid2D.velocity = new Vector2(key * this.stealthForce, rigid2D.velocity.y);

                Debug.Log("slow");
            }
        }

        if(isGrounded == false && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
            //空中にいて、かつ左右どちらかに動いていれば、実行
        {
           if(isRight==true && Input.GetAxis("Horizontal") > 0)
            {
                //右に動こうとしてて、右向きの慣性がついていれば実行しない(return)
                Debug.Log("right");
                return;
            }
            if (isRight == false && Input.GetAxis("Horizontal") < 0)
            {
                Debug.Log("left");
                return;
            }
            
            //減衰力を移動の速度のXに掛け算する
            this.rigid2D.velocity = new Vector2(rigid2D.velocity.x * braekPower,rigid2D.velocity.y);
            
        }

        //プレイヤーが崖から落ちたら初期位置に戻す
        this.nowPosition = GetComponent<Transform>().position;
        if (this.nowPosition.y < 0)
        {
            transform.position = this.startPosition;
        }
        //落下速度上限
        if(rigid2D.velocity.y < maxDownVelocityY)
        {
            this.rigid2D.velocity = new Vector2(rigid2D.velocity.x, maxDownVelocityY);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            groundGravityScale = 5f;
            this.rigid2D.gravityScale = groundGravityScale;
            isGrounded = true;
            isWalled = true;
            Debug.Log("壁");
            //衝突したオブジェクトのTransformを取得
            Transform wallTransform = collision.transform;
            // Transformからposition値を取得
            this.wallPosition = wallTransform.position;
        }
    }
    //接地判定
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundGravityScale = 5f;
            this.rigid2D.gravityScale = groundGravityScale;
            isGrounded = true;
            Debug.Log("着地");
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundGravityScale = 10f;
            this.rigid2D.gravityScale = groundGravityScale;
            isGrounded = false;
            Debug.Log("空中");
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            groundGravityScale = 10f;
            this.rigid2D.gravityScale = groundGravityScale;
            isWalled = false;
            Debug.Log("空中");
        }
    }
}    
