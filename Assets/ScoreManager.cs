using System;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_InputField[] inputPads = new TMP_InputField[3];
    public TextMeshProUGUI matchResultText;
    int[] result = new int[3];
    int[] selection = new int[9];
    int ball = 0;
    int strike = 0;
    int _out = 0;
    int round = 0;
    bool start = false;

    #region Main rule logic

    void UpdateResult()
    {
        ResetSelection();
        int index = 0;
        System.Random random = new System.Random();

        while (index < result.Length)
        {
            // 중복값이 나오지 않을때만 실행.
            int randomValue = random.Next(0, selection.Length);

            if (selection[randomValue] == 0)
            {
                // 선택 여부 지정
                selection[randomValue] = 1;

                result[index] = randomValue;
                index++;
            }
        }
    }

    void ResetSelection()
    {
        for (int i = 0; i < result.Length ; i++)
        {
            result[i] = 0;
        }

        for (int i = 0; i < selection.Length; i++)
        {
            selection[i] = 0;
        }
    }

    void ResetResult()
    {
        ball = 0;
        strike = 0;
        _out = 0;
        round = 0;
        start = false;
        UpdateResultText();
    }

    void CheckIsStrike()
    {
        // 결과 출력
        for (int i = 0; i < inputPads.Length; i++)
        {
            if (Int32.TryParse(inputPads[i].text, out int playerValue))
            {
                if (playerValue - 1 == result[i])
                {
                    strike += 1;
                }
            }
        }
    }

    void CheckIsBall()
    {
        for (int i = 0; i < inputPads.Length; i++)
        {
            if (Int32.TryParse(inputPads[i].text, out int playerValue))
            {
                if (playerValue - 1 != result[i] && result.Contains(playerValue) == true)
                {
                    ball += 1;
                }
            }
        }
    }

    void CheckIsOut()
    {
        for (int i = 0; i < inputPads.Length; i++)
        {
            if (Int32.TryParse(inputPads[i].text, out int playerValue))
            {
                if (playerValue - 1 != result[i] && result.Contains(playerValue) == false)
                {
                    ball += 1;
                }
            }
        }
    }

    bool CheckIsInputsCorrect()
    {
        for (int i = 0; i < inputPads.Length; i++)
        {
            if (Int32.TryParse(inputPads[i].text, out int playerValue))
            {
                if (playerValue >= 0 && playerValue <= 9)
                {
                    return true;
                }
            }

        }

        return false;
    }
    #endregion

    #region Button Events
    public void OnStartButtonClick()
    {
        if (strike >= 3)
        {
            if(start == true)
            {
                InfoManager.Instance.IncreasePoint(round * 1200);
                start = false;
            }
            return;
        }

        if (CheckIsInputsCorrect() == false)
        {
            SetResultText("Write all inputs 1 between 9");
            return;
        }

        // 시작 상태가 아닐때 시작 처리
        if (start == false)
        {
            start = true;
        }

        // 컴퓨터 선택 초기화
        UpdateResult();
        round += 1;

        // 결과 출력
        CheckIsStrike();
        CheckIsBall();
        CheckIsOut();
        UpdateResultText();
    }

    public void OnResetButtonClick()
    {
        if(InfoManager.Instance.playerInfo.gold > 1000 && start == false)
        {
            InfoManager.Instance.DecreaseGold(1000);
            ResetSelection();
            ResetResult();
            start = true;
            SetResultText("Reset done");
        }
        else
        {
            SetResultText("You cannot reset. check gold amount or check is game started.");
        }
    }
    #endregion

    #region Text update methods
    private void UpdateResultText()
    {
        matchResultText.text = "";
        matchResultText.text += $"{round} Round Result\n";
        matchResultText.text += $"====================\n";
        matchResultText.text += $"{strike}S {ball}B {_out}O";
    }

    private void SetResultText(string text)
    {
        matchResultText.text = text;
    }
    #endregion
}
