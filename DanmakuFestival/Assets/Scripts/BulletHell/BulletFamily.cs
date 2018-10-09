using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFamily : MathfExtras 
{

    //Everything here should be in world space
    //==========Start Values==========

    //Bullets Stats
    public List<Transform> bullets;

    public float bulletMoveAcceleration;

    public float bulletAngularMoveAcceleration;

    //BulletFamily Compound Movement
    public float bfMoveAcceleration;

    public float bulletFamilyCompoundRotationReadjustment;

    //==========Runtime Values==========
    public float index;

    public float bulletMoveSpeed;
    public float bulletAngularMoveSpeed;

    public float bfMoveSpeed;
    public float bfAngularMoveSpeed;

    public float timeSinceCompoundStart;

    public float timeSincebfBulletMoveIntervalStart;
    public float timeSincebfBulletAngularMoveIntervalStart;

    //========================================
    /*
    public IEnumerator HandleBulletFamilyMovement(float _startRot)
    {
        bulletFamilyCompoundShapeRotationReadjustment = _startRot;
        bfCompoundTimeStart = Time.time;
        bfMoveSpeed = bfMoveStartSpeed;
        bfAngularMoveSpeed = bfAngularMoveStartSpeed;

        while (Time.time - bfCompoundTimeStart < bfCompondActionDuration)
        {
            this.transform.rotation *= Quaternion.Euler(Vector3.forward * (bfCompondRotSpeed + bfAngularMoveStartSpeed * Time.deltaTime));
            this.transform.position += AlterVector(Vector3.up * bfMoveSpeed, bulletFamilyCompoundShapeRotationReadjustment + bfAngularMoveSpeed + bulletFamilyCompoundRotationReadjustment);

            //if (this.transform.position.x * this.transform.position.x > GameManager.squaredGameBoundaryDist || (this.transform.position.y * this.transform.position.y > GameManager.squaredGameBoundaryDist)) Desummon();
            bfAngularMoveSpeed += bfAngularMoveStartSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        bulletAngularMoveSpeed = bulletAngularMoveStartSpeed;
        bulletMoveSpeed = bulletMoveStartSpeed;

        float bfBulletMoveIntervalTimeStart = Time.time - 0.5f * bulletMoveAccelerationPingPongInterval;
        float bfBulletAngularMoveIntervalTimeStart = Time.time - 0.5f * bulletAngularMoveAccelerationPingPongInterval;

        while (bullets.Count > 0)
        {
            bulletMoveSpeed += bulletMoveAcceleration * Time.deltaTime;
            bulletAngularMoveSpeed += bulletAngularMoveAcceleration * Time.deltaTime;

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].transform.rotation *= Quaternion.Euler(Vector3.forward * bulletAngularMoveSpeed);
                bullets[i].transform.position += bullets[i].transform.up * bulletMoveSpeed;

                if(bullets[i].transform.position.x * bullets[i].transform.position.x > GameManager.squaredGameBoundaryDist || (bullets[i].transform.position.y * bullets[i].transform.position.y > GameManager.squaredGameBoundaryDist))
                {
                    bullets[i].gameObject.SetActive(false);
                    GameManager.availableBullets.Add(bullets[i]);
                    bullets.RemoveAt(i);
                    i--;
                }
            }

            if(Time.time - bfBulletMoveIntervalTimeStart > bulletMoveAccelerationPingPongInterval)
            {
                bfBulletMoveIntervalTimeStart = Time.time;
                bfMoveAcceleration = -bfMoveAcceleration;
            }
            if (Time.time - bfBulletAngularMoveIntervalTimeStart > bulletAngularMoveAccelerationPingPongInterval)
            {
                bfBulletAngularMoveIntervalTimeStart = Time.time;
                bulletAngularMoveAcceleration = -bulletAngularMoveAcceleration;
            }
            yield return new WaitForEndOfFrame();
        }
        Desummon();
    }
    */
    public void Desummon()
    {
        while(bullets.Count > 0)
        {
            bullets[0].gameObject.SetActive(false);
            GameManager.availableBullets.Add(bullets[0]);
            bullets.RemoveAt(0);
        }

        GameManager.availableBulletFamily.Add(this);
        this.gameObject.SetActive(false);
    }
}
