using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartAction : MonoBehaviour {

    public GameObject[] piece;          //ピースのPrefabを配列で扱う
    GameObject pieceObject;             //インスタンス生成に使用するGameObjectを格納する場所
    public GameObject manual;           //マニュアルの表示を担当する
    public int initNum = 45;            //ピース数
    public int setbutton;               //セットボタンの状況管理
    public float interval = 0.1f;       //生成する時間間隔
    public bool roop;                   //ループ処理用の真偽値
    public Text setText;                //セットボタンの文字
    public Text txttitle;               //タイトル文字

	// Use this for initialization
	void Start () {
        //コルーチン処理を指示する
        StartCoroutine("SetPiece",initNum);
        setbutton = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //roopが真の時は
        if (roop) {
            //コルーチン処理を実行する
            StartCoroutine("SetPiece", initNum);
        }
    }

    //コルーチン処理
    IEnumerator SetPiece(int Num) {
        //ループを偽にする
        roop = false;
        //ピースを生成する
        for (int idx = 0; idx < Num; idx++) {
            //ランダムで0～7の数字を代入する(ボールが8種類ある為)
            int idz = Random.Range(0, 8);
            //x座標に対して-1.7f～1.7fの範囲を代入する(横幅に幅を持たせる為、これを行わないとそのまま縦に落ちる)
            float x = Random.Range(-1.7f, 1.7f);
            //横幅x縦幅を画面外の上の所に座標を指定する
            Vector2 test = new Vector2(x, 6.5f);
            //ピースをインスタンス生成する
            pieceObject = Instantiate(piece[idz], test, Quaternion.identity);
            //コルーチンで0.1秒待機する
            yield return new WaitForSeconds(interval);
        }
        //ループを真にする
        roop = true;
    }

    //スタートボタンを押した時
    public void startButton() {
        //メインシーンに飛ぶ
        SceneManager.LoadScene("_Main");
    }

    //セットボタンを押した時
    public void setButton() {
        switch (setbutton) {
            case 0:
                //簡単なマニュアルを表示する
                manual.SetActive(true);
                //セットボタンの文字を戻るに変更する
                setText.text = "戻る";
                txttitle.text = "";
                //setbuttonを1にする
                setbutton = 1;
                break;

            case 1:
                //セットボタンの文字をマニュアルに変更する
                setText.text = "マニュアル";
                txttitle.text = "Falling block Game";
                //簡単なマニュアルを非表示する
                manual.SetActive(false);
                //setbuttonを0にする
                setbutton = 0;
                break;
        }
    }
}
