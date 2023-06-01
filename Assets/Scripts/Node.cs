using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    private Whose whose;
    private Color myColor;
    private Color opponentColor;

    private void Start() {
        whose = Whose.None;
        myColor = Color.black;
        opponentColor = Color.white;
    }

    private void OnMouseDown() {
        if (InGameHandle.Instance.status == Status.GameOver)
            return;
        int id = int.Parse(this.name);
        int i = id / 19;
        int j = id % 19;
        if (whose == Whose.None) {
            whose = InGameHandle.Instance.PlayerNext;
            InGameHandle.Instance.UpdateArray(i,j,whose);
            InGameHandle.Instance.SetPlayerNext();
            InGameHandle.Instance.node = this;
            if (whose == Whose.My) {
                GetComponent<SpriteRenderer>().color = myColor;
            }
            else if(whose == Whose.Opponent) {
                GetComponent<SpriteRenderer>().color = opponentColor;
            }
        }
    }

    public void ResetNode() {
        whose = Whose.None;
        Color color = new Color(255, 255, 255, 0);
        GetComponent<SpriteRenderer>().color = color;
        int id = int.Parse(this.name);
        int i = id / 19;
        int j = id % 19;
        InGameHandle.Instance.UpdateArray(i,j,Whose.None);
        InGameHandle.Instance.SetPlayerNext();
        InGameHandle.Instance.node = null;
    }
}
