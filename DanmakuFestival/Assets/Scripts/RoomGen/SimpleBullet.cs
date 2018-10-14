using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBullet : MonoBehaviour {

    public Vector2 velocity;
    public Rigidbody2D rb;

    private void Update()
    {
        rb.MovePosition(this.transform.position + (Vector3)velocity * Time.deltaTime * GameManager.timeScale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
