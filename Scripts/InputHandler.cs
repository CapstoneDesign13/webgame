using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public PlayerUnit player;
    public TurnManager turn;
    public WindowManager window;

    public void MoveU()
    {
        player.DoMove(Vector2Int.up);
    }

    public void MoveD()
    {
        player.DoMove(Vector2Int.down);
    }

    public void MoveL()
    {
        player.DoMove(Vector2Int.left);
    }

    public void MoveR()
    {
        player.DoMove(Vector2Int.right);
    }

    public void DoZ()
    {
        player.DoAttack();
    }

    public void DoX()
    {
        player.DoDefense();
    }

    public void DoSpace()
    {
        turn.EndPlayerTurn();
    }

    private void Start()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

    private void Update()
    {
        // 방향키 + WASD (이동)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            MoveU();

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            MoveD();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveL();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveR();

        // Z X C 액션 키
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z pressed");
            player.DoAttack();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X pressed");
            player.DoDefense();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C pressed");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE pressed");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab pressed");
            window.OpenInfo();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Tab released");
            window.OpenField();
        }
    }
}