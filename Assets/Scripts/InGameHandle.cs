using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHandle : MonoBehaviour {
    public static InGameHandle Instance;
    [SerializeField] private Whose _playerNext;
    public Whose PlayerNext {
        get {
            return _playerNext;
        }
        private set {
            _playerNext = value;
        }
    }
    public Status status;
    [Header("Node")]
    public Node node;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private int nodeNumber;
    private int[,] array = new int[19,19];
    [Header("UI")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button playAgain;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Button backButton;
    

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if(Instance != this) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        ListenActions();
        CreateNodes();
        status = Status.InGame;
        node = null;
    }

    private void OnDestroy() {
        RemoveActions();
    }

    private void ListenActions() {
        playAgain.onClick.AddListener(PlayAgainButtonOnClick);
        backButton.onClick.AddListener(BackButtonOnClick);
    }

    private void RemoveActions() {
        playAgain.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }

    private void PlayAgainButtonOnClick() {
        status = Status.InGame;
        resultPanel.SetActive(false);
        controlPanel.SetActive(true);
        PlayerNext = Whose.My;
        foreach (Transform child in nodeParent) {
            Destroy(child.gameObject);
        }
        CreateNodes();
    }

    private void BackButtonOnClick() {
        if (node != null) {
            node.ResetNode();
        }
    }

    private void CreateNodes() {
        int id = 0;
        float y = 2.295f;
        for (int i = 0; i < nodeNumber; i++) {
            float x = -2.295f;
            for (int j = 0; j < nodeNumber; j++) {
                GameObject nodeGO = Instantiate(nodePrefab, new Vector3(x, y, 0), quaternion.identity, nodeParent);
                nodeGO.name = id.ToString();
                x += 0.254f;
                array[i, j] = (int)Whose.None;
                id++;
            }
            y -= 0.254f;
        }
    }

    public void UpdateArray(int i, int j, Whose whose) {
        array[i, j] = (int)whose;
        switch (CheckResult()) {
            case Result.Win:
                status = Status.GameOver;
                resultPanel.SetActive(true);
                controlPanel.SetActive(false);
                resultText.text = "Player 1 win";
                break;
            case Result.Loose:
                status = Status.GameOver;
                resultPanel.SetActive(true);
                controlPanel.SetActive(false);
                resultText.text = "Player 2 win";
                break;
            case Result.Balance:
                status = Status.GameOver;
                resultPanel.SetActive(true);
                controlPanel.SetActive(false);
                resultText.text = "Draw";
                break;
        }
    }

    #region Calculator
    private Result CheckResult() {
        // Check player 1 has win
        if (CheckPlayer(Whose.My) == true)
            return Result.Win;
        // Check player 2 has win
        if (CheckPlayer(Whose.Opponent) == true)
            return Result.Loose;
        // Whether or not player can continue match
        bool balance = true;
        for (int i = 0; i < nodeNumber; i++) {
            for (int j = 0; j < nodeNumber; j++) {
                if (array[i, j] == (int)Whose.None) {
                    balance = false;
                    break;
                }
            }
        }
        if (balance == true)
            return Result.Balance;
        return Result.None;
    }

    private bool CheckPlayer(Whose whose) {
        int player = (int)whose;
        int opponent = 0;
        switch (whose) {
            case Whose.My:
                opponent = (int)Whose.Opponent;
                break;
            case Whose.Opponent:
                opponent = (int)Whose.My;
                break;
        }
        int count;
        int startPoint;
        int endPoint;
        bool preventFront;
        bool preventBack;
        for (int i = 0; i < nodeNumber; i++) {
            for (int j = 0; j < nodeNumber; j++) {
                if (array[i, j] == player) {
                    // Theo chieu ngang
                    count = 1;
                    startPoint = j;
                    endPoint = j;
                    preventFront = false;
                    preventBack = false;
                    if (j < nodeNumber - 4) {
                        for (int k = j + 1; k < nodeNumber; k++) {
                            if (array[i, k] == player) {
                                count++;
                                endPoint = k;
                            }
                            else {
                                break;
                            }
                        }
                        if (count == 5) {
                            if (startPoint == 0) {
                                preventFront = true;
                            }
                            else {
                                for (int k = startPoint-1; k >= 0; k--) {
                                    if (array[i, k] == opponent) {
                                        preventFront = true;
                                        break;
                                    }
                                }
                            }
                            if (endPoint == nodeNumber - 1) {
                                preventBack = true;
                            }
                            else {
                                for (int k = endPoint + 1; k < nodeNumber; k++) {
                                    if (array[i, k] == opponent) {
                                        preventBack = true;
                                        break;
                                    }
                                }
                            }
                            if (!preventFront || !preventBack)
                                return true;
                        }
                    }
                    // Theo chieu doc
                    count = 1;
                    startPoint = i;
                    endPoint = i;
                    preventFront = false;
                    preventBack = false;
                    if (i < nodeNumber - 4) {
                        for (int k = i + 1; k < nodeNumber; k++) {
                            if (array[k,j] == player) {
                                count++;
                                endPoint = k;
                            }
                            else {
                                break;
                            }
                        }
                        if (count == 5) {
                            if (startPoint == 0) {
                                preventFront = true;
                            }
                            else {
                                for (int k = startPoint-1; k >= 0; k--) {
                                    if (array[k,j] == opponent) {
                                        preventFront = true;
                                        break;
                                    }
                                }
                            }
                            if (endPoint == nodeNumber - 1) {
                                preventBack = true;
                            }
                            else {
                                for (int k = endPoint + 1; k < nodeNumber; k++) {
                                    if (array[k,j] == opponent) {
                                        preventBack = true;
                                        break;
                                    }
                                }
                            }
                            if (!preventFront || !preventBack)
                                return true;
                        }
                    }
                    // Theo chieu cheo phai
                    count = 1;
                    preventFront = false;
                    preventBack = false;
                    if (i < nodeNumber - 4 && j < nodeNumber - 4) {
                        int startPoint1 = i;
                        int startPoint2 = j;
                        int endPoint1 = i;
                        int endPoint2 = j;
                        int n = 1;
                        while (i+n < nodeNumber && j+n < nodeNumber) {
                            if (array[i + n, j + n] == player) {
                                count++;
                                endPoint1 = i + n;
                                endPoint2 = j + n;
                                n++;
                            }
                            else {
                                break;
                            }
                        }
                        if (count == 5) {
                            if (startPoint1 == 0 || startPoint2 == 0) {
                                preventFront = true;
                            }
                            else {
                                int k = 1;
                                while (startPoint1 - k >= 0 && startPoint2 - k >= 0) {
                                    if (array[startPoint1 - k, startPoint2 - k] == opponent) {
                                        preventFront = true;
                                        break;
                                    }
                                    k++;
                                }
                            }
                            if (endPoint1 == nodeNumber - 1 || endPoint2 == nodeNumber - 1) {
                                preventBack = true;
                            }
                            else {
                                int k = 1;
                                while (endPoint1 + k < nodeNumber && endPoint2 + k < nodeNumber) {
                                    if (array[endPoint1 + k, endPoint2 + k] == opponent) {
                                        preventBack = true;
                                        break;
                                    }
                                    k++;
                                }
                            }
                            if (!preventFront || !preventBack)
                                return true;
                        }
                    }
                    // Theo chieu cheo trai
                    count = 1;
                    preventFront = false;
                    preventBack = false;
                    if (i < nodeNumber - 4 && j > 3) {
                        int startPoint1 = i;
                        int startPoint2 = j;
                        int endPoint1 = i;
                        int endPoint2 = j;
                        int n = 1;
                        while (i+n < nodeNumber && j-n >= 0) {
                            if (array[i + n, j - n] == player) {
                                count++;
                                endPoint1 = i + n;
                                endPoint2 = j - n;
                                n++;
                            }
                            else {
                                break;
                            }
                        }
                        if (count == 5) {
                            if (startPoint1 == 0 || startPoint2 == nodeNumber - 1) {
                                preventFront = true;
                            }
                            else {
                                int k = 1;
                                while (startPoint1 - k >= 0 && startPoint2 + k < nodeNumber) {
                                    if (array[startPoint1 - k, startPoint2 + k] == opponent) {
                                        preventFront = true;
                                        break;
                                    }
                                    k++;
                                }
                            }
                            if (endPoint1 == nodeNumber - 1 || endPoint2 == 0) {
                                preventBack = true;
                            }
                            else {
                                int k = 1;
                                while (endPoint1 + k < nodeNumber && endPoint2 - k >= 0) {
                                    if (array[endPoint1 + k, endPoint2 - k] == opponent) {
                                        preventBack = true;
                                        break;
                                    }
                                    k++;
                                }
                            }
                            if (!preventFront || !preventBack)
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    #endregion

    public void SetPlayerNext() {
        if (PlayerNext == Whose.My) {
            PlayerNext = Whose.Opponent;
        }
        else if(PlayerNext == Whose.Opponent) {
            PlayerNext = Whose.My;
        }
    }
}
