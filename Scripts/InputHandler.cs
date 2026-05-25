using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public TurnManager turn;
    public WindowManager window;
    public PlayerUnit player;

    public bool leftshiftpressed;

    public Image altUI;
    public TMP_Text altBtnTxt;
    public bool altMode;

    private void Awake()
    {
            Debug.Log("IMECompositionMode Off");
            Input.imeCompositionMode = IMECompositionMode.Off;
    }

    private void OnDisable()
    {
        //화면 전환시 프레스 전부 해제
        if (leftshiftpressed)
        {
            Debug.Log("LeftShift released");
            leftshiftpressed = false;
            AltMode();
        }
    }

    private bool CanUsePlayerInput()
    {
        return turn != null && turn.CanUsePlayerInput;
    }

    public void AltMode()
    {
        //키와 버튼 동시 입력을 방지
        if (leftshiftpressed)
            return;

        altMode = !altMode;
        if (altUI != null && altBtnTxt != null)
            if (altMode)
            {
                altUI.color = Color.black;
                altBtnTxt.text = "<color=#D4B000>●</color>이동(shift)";
            }
            else
            {
                altUI.color = new Color32(212, 176, 0, 255);
                altBtnTxt.text = "<color=#000000>●</color>경공(shift)";
            }
    }

    public void MoveU()
    {
        if (!CanUsePlayerInput()) return;
        if (altMode)
            player.DoFlash(Vector2Int.up);
        else
            MapManager.Instance.Player.DoMove(Vector2Int.up);
    }

    public void MoveD()
    {
        if (!CanUsePlayerInput()) return;
        if (altMode)
            player.DoFlash(Vector2Int.down);
        else
            MapManager.Instance.Player.DoMove(Vector2Int.down);
    }

    public void MoveL()
    {
        if (!CanUsePlayerInput()) return;
        if (altMode)
            player.DoFlash(Vector2Int.left);
        else
            MapManager.Instance.Player.DoMove(Vector2Int.left);
    }

    public void MoveR()
    {
        if (!CanUsePlayerInput()) return;
        if (altMode)
            player.DoFlash(Vector2Int.right);
        else
            MapManager.Instance.Player.DoMove(Vector2Int.right);
    }

    public void DoZ()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.PrimaryAction();
    }

    public void DoX()
    {
        if (!CanUsePlayerInput()) return;
        MapManager.Instance.Player.SecondaryAction();
    }

    public void DoSpace()
    {
        if (!CanUsePlayerInput()) return;
        player.TurnEndReq();
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

        /*if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Tab released");

            if (window != null)
            {
                window.CloseInfo();
            }
        }*/

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("LeftShift pressed");
            AltMode();
            leftshiftpressed = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && leftshiftpressed)
        {
            Debug.Log("LeftShift released");
            leftshiftpressed = false;
            AltMode();
        }
    }
}
