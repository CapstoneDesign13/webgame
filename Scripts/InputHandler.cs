using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TurnManager turn;
    public WindowManager window;

    private void Start()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

    private bool CanUsePlayerInput()
    {
        return turn != null && turn.CanUsePlayerInput;
    }

    public void MoveU()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoMove(Vector2Int.up);
    }

    public void MoveD()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoMove(Vector2Int.down);
    }

    public void MoveL()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoMove(Vector2Int.left);
    }

    public void MoveR()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoMove(Vector2Int.right);
    }

    public void DoZ()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoAttack();
    }

    public void DoX()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.DoDefense();
    }

    public void DoSpace()
    {
        if (turn == null) return;
        turn.EndPlayerTurn();
    }

    private void Update()
    {
        if (!CanUsePlayerInput())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveU();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveD();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveL();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveR();
        }

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE pressed");
            DoSpace();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab pressed");

            if (window != null)
            {
                window.OpenInfo();
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Tab released");

            if (window != null)
            {
                window.CloseInfo();
            }
        }
    }
}
