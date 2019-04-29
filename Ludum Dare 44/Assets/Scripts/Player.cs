using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 250f;
    float jumpHeight = 500f;
    float xaxis;
    float yaxis;
    public bool facingRight = true;

    public bool jump = false;
    public bool grounded = false;
    public Transform GroundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public int Health = 5;
    public bool invincible = false;

    // Knock back variables
    private float knockBackMagnitude = 300;
    private float knockBackTimer = 0f;
    private float knockBackTime = 30f;
    private bool knockBack = false;

    // Yoyo
    public GameObject Yoyo;
    public bool throwYoyo = true;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        xaxis = Input.GetAxisRaw("Horizontal");
        yaxis = Input.GetAxisRaw("Vertical");

        SetAnimationVariables();

        if (yaxis > 0) jump = true;

        if (knockBackTimer > 0) {
            CheckKnockBackCounter();
        } 
    }

    void FixedUpdate() {

        // if (throwYoyo) rb.velocity = new Vector2(0, rb.velocity.y);

        // Set RB Velocity
        if (!knockBack) {
            Vector2 velX = rb.velocity;
            velX.x = xaxis * speed * Time.deltaTime;
            rb.velocity = velX;
        }
        // Clamp move speed
        if (rb.velocity.magnitude > speed) {
            rb.velocity *= speed / rb.velocity.magnitude;
        }

        // Apply jump
        if (jump && grounded && !knockBack) {
            Jump();
        } else {
            jump = false;
        }

        // Flip player
        if (xaxis < 0 && facingRight) {
            Flip();
        } else if (xaxis > 0 && !facingRight) {
            Flip();
        }

        
        // Throw Yo-yo, here goes nothing
        if (Input.GetMouseButtonDown(0) && throwYoyo) {
            throwYoyo = false;
            animator.SetBool("IsThrowing", true);
            Yoyo yoyo = Yoyo.GetComponent<Yoyo>();
            bool currentDirection = facingRight;
            yoyo.Throw(currentDirection);
        }

        grounded = Physics2D.OverlapCircle(GroundCheck.position, 0.15f, groundLayer);
    }

    private void Jump() {
        Vector2 velY = rb.velocity;
        velY.y = jumpHeight * yaxis * Time.deltaTime;
        rb.velocity = velY;
        jump = false;
        grounded = false;
    }

    private void Flip() {
        facingRight = !facingRight;

        Vector3 _SCALE = gameObject.transform.GetChild(0).transform.localScale;
        _SCALE.x *= -1;
        gameObject.transform.GetChild(0).transform.localScale = _SCALE; 
    }

    private void CheckKnockBackCounter() {
        knockBackTimer--;
        if (knockBackTimer <= 0) {
            knockBack = false;
            animator.SetBool("IsKnockBack", false);
        }
    }

    private void StartKnockBack(Vector2 vel) {
        knockBack = true;
        knockBackTimer = knockBackTime;
        animator.SetBool("IsKnockBack", true);
        KnockBack(vel);
    }

    private void KnockBack(Vector2 vel) {

        if (vel.x <= 0 && rb.velocity.x < 0 && xaxis < 0) {
            rb.velocity = new Vector2(knockBackMagnitude * Time.deltaTime, knockBackMagnitude * Time.deltaTime);
        } else if (vel.x >= 0 && rb.velocity.x > 0 && xaxis > 0) {
            rb.velocity = new Vector2(-knockBackMagnitude * Time.deltaTime, knockBackMagnitude * Time.deltaTime);
        } else if (vel.x <= 0) {
            rb.velocity = new Vector2(-knockBackMagnitude * Time.deltaTime, knockBackMagnitude * Time.deltaTime);
        } else if (vel.y >= 0) {
            rb.velocity = new Vector2(knockBackMagnitude * Time.deltaTime, knockBackMagnitude * Time.deltaTime);
        }
    }

    private void Damage(GameObject go) {
        Health -= 0;
        if (CheckDamage()) {
            return;
        }
        invincible = true;
        StartKnockBack(go.GetComponent<Rigidbody2D>().velocity);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        Invoke("resetInvincibility", 1.5f);
    }

    private bool CheckDamage() {
        if (Health <= 0) {
            Die();
            return true;
        } else {
            return false;
        }
    }

    private void Die() {
        Destroy(gameObject);
    }

    private void resetInvincibility() {
        invincible = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }

    private void SetAnimationVariables() {
        // Logic for jumping / falling
        if (rb.velocity.y > 0) {
			animator.SetBool("IsRaising", true);
			animator.SetBool("IsFalling", false);
		} else if (rb.velocity.y < 0) {
			animator.SetBool("IsRaising", false);
			animator.SetBool("IsFalling", true);
		} else {
			animator.SetBool("IsFalling", false);
			animator.SetBool("IsRaising", false);
		}

        if (yaxis < 0) {
            animator.SetBool("IsSheilding", true);
        } else if (yaxis >= 0) {
            animator.SetBool("IsSheilding", false);
        }

        if (rb.velocity.y == 0) {
            animator.SetFloat("MoveSpeed", Mathf.Abs(xaxis));
        } else {
            animator.SetFloat("MoveSpeed", 0);
        }
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag == "Enemy") {
            if (!invincible) Damage(col.gameObject);
        }
    }
}
