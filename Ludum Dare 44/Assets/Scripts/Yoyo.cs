using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yoyo : MonoBehaviour
{
    public GameObject Target;
    private GameObject currentTarget;

    public GameObject StartingPos;

    private float targetx;
    private float range = 3f;

    private float speed = 10f;
    public bool thrown = false;
    public bool goingOut = false;
    public bool comingBack = false;
    void Start()
    {
        // gameObject.SetActive(false);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true); 
    }

    void FixedUpdate()
    {
        YoyoAI();
    }

    public void Throw (bool facingRight) {
        if (facingRight) {
            CalculateTarget(1);
        } else {
            CalculateTarget(-1);
        }
    }

    private void YoyoAI() {
        float dir = speed * (targetx / Mathf.Abs(targetx));
        if (thrown && goingOut) {

            float xvel = gameObject.transform.position.x + dir * Time.deltaTime;
            transform.position = new Vector2(xvel, gameObject.transform.position.y);
            
           //transform.position = Vector2.MoveTowards(gameObject.transform.position, TargetPos, Speed * Time.deltaTime);


            if (Mathf.Abs(targetx - gameObject.transform.position.x) <= 0.25f) {
                goingOut = false;
                comingBack = true;
            }
        } else if (thrown && comingBack) {

            float xvel = gameObject.transform.position.x + -dir * Time.deltaTime;
            transform.position = new Vector2(xvel, gameObject.transform.position.y);
            //transform.position = Vector2.MoveTowards(gameObject.transform.position, StartingPos.transform.position, Speed * Time.deltaTime);

            if (Mathf.Abs(StartingPos.transform.position.x - gameObject.transform.position.x) <= 0.25f) {
                comingBack = false;
                thrown = false;

                Destroy(currentTarget);

                gameObject.transform.parent.GetComponent<Player>().throwYoyo = true;
                gameObject.transform.parent.GetChild(0).GetComponent<Animator>().SetBool("IsThrowing", false);
            }
        }
    }

    private void CalculateTarget(float facing) {
        targetx = this.gameObject.transform.position.x + (range * facing);

        Vector3 targetSpawn = new Vector3(targetx, gameObject.transform.position.y, gameObject.transform.position.z);

        currentTarget = (GameObject)Instantiate(Target, targetSpawn, gameObject.transform.rotation);
        thrown = true;
        goingOut = true;
        Debug.Log("Facing: " + facing + " Target X: " + targetx);
        if (facing < 0 && targetx > 0 || facing > 0 && targetx < 0) {
            Debug.Log("THERE WAS AN ERROR IN THE LOGIC SOMEWHERE");
            targetx *= -1;
        }
    }
}
