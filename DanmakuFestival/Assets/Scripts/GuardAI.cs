using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : PathFinder {

    public float moveSpeed;
    private Vector2Int curIndexLocation;
    private Vector2 targetMovePos;

    private Rigidbody2D rb;
    //
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        curIndexLocation = ToRoomIndexLocation(this.transform.position);
        targetMovePos = GetTargetMovePosition(curIndexLocation);
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(this.transform.position, targetMovePos);
        rb.MovePosition(Vector3.MoveTowards(this.transform.position, targetMovePos, moveSpeed * Time.deltaTime * GameManager.timeScale));


        Vector2 _dir = new Vector2(targetMovePos.x - this.transform.position.x, targetMovePos.y - this.transform.position.y);
        float _angle = Mathf.Rad2Deg * Mathf.Atan(_dir.y / _dir.x) + 90;
        if(_dir.x == 0)
        {
            if (_dir.y > 0) _angle = 0;
            else _angle = 180;
        }
        if (_dir.x > 0) _angle += 180;
        this.transform.localRotation = Quaternion.Euler(Vector3.forward * (_angle));

        
    }
}
