using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : PathFinder {

    public GameObject bulletGO;
    public float sprayAngle;
    public float bulletSpeed;
    public float fireInterval;
    public float timeSinceLastFire = 0;

    public float moveSpeed;
    private Vector2Int curIndexLocation;
    private Vector2 targetMovePos;
    private float curAngle;

    private Rigidbody2D rb;
    public List<Vector2Int> patrolPath;
    public int patrolIndex =0;

    public float noticeRange;
    public float fieldOfViewAngle;


    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!room.IsActive) return;
        curIndexLocation = ToRoomIndexLocation(this.transform.position);
        if(room.IsAlert) targetMovePos = GetAlertedMovePosition(curIndexLocation);
        else
        {
            targetMovePos = new Vector2(room.transform.position.x,room.transform.position.y) + (Vector2)patrolPath[patrolIndex] * GameManager.roomGenerationFields.roomScale;
            if (((Vector2)this.transform.position - targetMovePos).sqrMagnitude  <= GameManager.roomGenerationFields.roomScale * GameManager.roomGenerationFields.roomScale *0.25f) patrolIndex++;
            if (patrolIndex >= patrolPath.Count) patrolIndex = 0;
        }
    }


    private void FixedUpdate()
    {
        if (!room.IsActive) return;
        curAngle = GetLookRotAngle(this.transform.position, targetMovePos);
        this.transform.localRotation = Quaternion.Euler(Vector3.forward * (curAngle));
        if (room.IsAlert)
        {
            if (targetMovePos == (Vector2)PlayerManager.player.position && PlayerIsWithinRange(noticeRange))
            {
                TryFire();
                GLDraw.DrawLine(this.transform.position, targetMovePos, GameManager.roomGenerationFields.dangerColor);
            }
            else
            {
                rb.MovePosition(Vector3.MoveTowards(this.transform.position, targetMovePos, moveSpeed * Time.deltaTime * GameManager.timeScale));
                GLDraw.DrawLine(this.transform.position, targetMovePos, GameManager.roomGenerationFields.warningColor);
            }
        }
        else 
        {
            rb.MovePosition(Vector3.MoveTowards(this.transform.position, targetMovePos, moveSpeed * Time.deltaTime * GameManager.timeScale));
            GLDraw.DrawLine(this.transform.position, targetMovePos, GameManager.roomGenerationFields.moveColor);
            if (PlayerIsWithinRange(noticeRange) && PlayerIsWithinFOV())
            {
                GLDraw.DrawLine(this.transform.position, PlayerManager.player.position,GameManager.roomGenerationFields.warningColor);
                if (HasDirectLineOFSight(this.transform.position, PlayerManager.player.position, 0))
                {
                    AlertRoom(true);
                }
            }
        }
    }

    private void TryFire()
    {
        timeSinceLastFire += Time.deltaTime * GameManager.timeScale;
        if (timeSinceLastFire < fireInterval) return;
        timeSinceLastFire = 0;
        float _dir = curAngle + Random.Range(-sprayAngle, sprayAngle);
        SimpleBullet _bullet = Instantiate(bulletGO, this.transform.position + this.transform.up *3.5f, Quaternion.Euler(Vector3.forward * _dir), null).GetComponent<SimpleBullet>();
        _bullet.velocity = AlterVector(Vector3.up * bulletSpeed, _dir);
    }

    private bool PlayerIsWithinRange(float _range)
    {
        if (((this.transform.position - PlayerManager.player.position).magnitude<= _range * GameManager.roomGenerationFields.roomScale)) return true;
        else return false;
    }

    private bool PlayerIsWithinFOV()
    {
        float _lookAngleToPlayer = GetLookRotAngle(this.transform.position, PlayerManager.player.position);
        if (Mathf.Pow(_lookAngleToPlayer - curAngle, 2) <= Mathf.Pow(fieldOfViewAngle, 2)) return true;
        else return false;
    }

    private void AlertRoom(bool _status)
    {
        if (room.IsAlert && _status) return;
        room.IsAlert = _status;
        if (_status)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), false);
            for (int i = 0; i < room.enemyTransforms.Count; i++)
            {
                GLDraw.DrawLine(this.transform.position, room.enemyTransforms[i].position, GameManager.roomGenerationFields.infoColor, 0.15f);
            }
        }
        else
        {

            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);

        }
    }
}
