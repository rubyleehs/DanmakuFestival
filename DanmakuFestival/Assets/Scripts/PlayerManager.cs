using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MathfExtras {

    public static Transform player;

    public float playerSpeed;
    [Range(0f,1f)]
    public float playerUnmovingTimeScale;
    [Range(0f, 1f)]
    public float playerShiftedTimeScale;

    private float inputYAxis = 0;
    private float inputXAxis = 0;
    private Vector3 moveDir = Vector3.zero;
    private bool IsShifted = false;

    private Vector2 mousePos;
    private float playerRot;

    private Rigidbody2D rb;

    private void Awake()
    {
        player = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
    }
    void Update () {
        inputXAxis = Input.GetAxisRaw("Horizontal");
        inputYAxis = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(inputXAxis, inputYAxis, 0).normalized * playerSpeed;
        IsShifted = Input.GetKey(KeyCode.LeftShift);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerRot = GetLookRotAngle(player.position,mousePos);

    }

    private void FixedUpdate()
    {
        if (IsShifted) GameManager.timeScale = playerShiftedTimeScale;
        else if (moveDir.sqrMagnitude == 0) GameManager.timeScale = playerUnmovingTimeScale;
        else GameManager.timeScale = 1f;

        rb.MovePosition(this.transform.position + moveDir * Time.fixedDeltaTime * GameManager.timeScale);
        player.rotation = Quaternion.Euler(Vector3.forward * playerRot);

    }
}
