using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rb;
    public float speed;
    public Transform HitRight;
    public Transform HitLeft;
    public LayerMask GroundLayer;
    public int health;
    public bool MovingRight = true;
    bool HITLEFT = false;
    bool HITRIGHT = false;
    
    // Update is called once per frame
    void Update()
    {
        HITLEFT = Physics2D.OverlapCircle(HitLeft.position, 0.15f, GroundLayer);
        HITRIGHT = Physics2D.OverlapCircle(HitRight.position, 0.15f, GroundLayer);

        if (MovingRight && HITRIGHT) {
            Flip();
            HITRIGHT = false;
        }
        if (!MovingRight && HITLEFT) {
            Flip();
            HITLEFT = false;
        }
    }

    void FixedUpdate() {
        if (MovingRight) {
            Vector2 vel = rb.velocity;
            vel.x = speed * Time.deltaTime;
            rb.velocity = vel;
        } else if (!MovingRight) {
            Vector2 vel = rb.velocity;
            vel.x = -speed * Time.deltaTime;
            rb.velocity = vel;
        }
    }

    public void Damage(int damage) {
        health -= damage;
        CheckHealth();
    }

    private void CheckHealth() {
        if (health <= 0) Die();
    }

    private void Flip() {
        MovingRight = !MovingRight;

        Vector3 _SCALE = gameObject.transform.GetChild(0).transform.localScale;
        _SCALE.x *= -1;
        gameObject.transform.GetChild(0).transform.localScale = _SCALE; 
    }

    private void Die() {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Enemy") {
            Flip();
        }
    }
}
