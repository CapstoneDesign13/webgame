using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TurnManager turn;
    public WindowManager window;

    public void MoveU()
    {
        MapManager.Instance.Player.DoMove(Vector2Int.up);
    }

    public void MoveD()
    {
        MapManager.Instance.Player.DoMove(Vector2Int.down);
    }

    public void MoveL()
    {
        MapManager.Instance.Player.DoMove(Vector2Int.left);
    }

    public void MoveR()
    {
        MapManager.Instance.Player.DoMove(Vector2Int.right);
    }

    public void DoZ()
    {
        MapManager.Instance.Player.DoAttack();
    }

    public void DoX()
    {
        MapManager.Instance.Player.DoDefense();
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
            DoZ();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X pressed");
            DoX();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C pressed");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE pressed");
            DoSpace();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab pressed");
            window.OpenInfo();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Tab released");
            window.CloseInfo();
        }
    }
}