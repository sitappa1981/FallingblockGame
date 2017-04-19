using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Text txtTitle;           //タイトル文字
    public Text txtScore;           //スコアの文字
    public Text txtTime;            //残り時間の文字(小数点第二まで表示する)
    public Text txtRanking;         //ランキングの文字
    public Text[] txtRank;          //ランキングボタンを押した時に表示されるランキングの文字
    public GameObject Reset;        //リセットボタン
    public GameObject Ranking;      //ランキングボタン
    public GameObject bar;          //下のバー

    public float remain = 60;       //残り時間を格納する変数

    public bool startwait;          //ピースの落下の終了判定
    public int[] Rank = new int[6]; //ランキングの点数を格納する場所
    public int newRank = 0;         //ランキングの順位を管理する為の変数
    public int index;               //ランキングボタンの進捗管理

    static public int score;        //点数を格納する変数
    static public bool isWait;      //開始待ちの真偽値


    // Use this for initialization
    void Start () {
        //リセットボタンのON,OFFを扱えるようにする
        Reset = GameObject.Find("Canvas/Reset");
        //リセットボタンをOFFにする
        Reset.SetActive(false);
        //ランキングボタンのON,OFFを扱えるようにする
        Ranking = GameObject.Find("Canvas/Ranking");
        //ランキングボタンをOFFにする
        Ranking.SetActive(false);
        //以前のスコアを登録する
        Rank[1] = GameDate.Point1;
        Rank[2] = GameDate.Point2;
        Rank[3] = GameDate.Point3;
        Rank[4] = GameDate.Point4;
        Rank[5] = GameDate.Point5;
        //前回の点数を格納
        Rank[1] = PlayerPrefs.GetInt("Rank1", Rank[1]);
        Rank[2] = PlayerPrefs.GetInt("Rank2", Rank[1]);
        Rank[3] = PlayerPrefs.GetInt("Rank3", Rank[1]);
        Rank[4] = PlayerPrefs.GetInt("Rank4", Rank[1]);
        Rank[5] = PlayerPrefs.GetInt("Rank5", Rank[1]);


        //タイトルに文字を出力する
        txtTitle.text = "Wait...";
        //スコアに文字を出力する
        txtScore.text = "000000";
        //時間に文字を出力する
        txtTime.text = "00.00s";
        //開始待ちを真にする
        isWait = true;
        //現在の点数を0点にする(staticなので定義時に初期化すると不具合を起こすから)
        score = 0;
        index = 0;
        startwait = false;
        //下のバーを表示する
        bar.SetActive(true);

        //コルーチン処理を起動
        StartCoroutine("MoveCheck");
    }
	
	// Update is called once per frame
	void Update () {
        //ピースの落下完了判定が真の場合
        if (startwait) {
            //時間のカウントダウンを行う
            CountDown();
        }
        // DisplayScoreの文字列に白紙とスコアの数字を代入する
        string DisplayScore = "" + score;
        //スコアテキストの文字を6桁表示する
        txtScore.text = DisplayScore.PadLeft(6,'0');
    }

    //時間のカウントダウンを行い、時間を出力する
    void CountDown() {
        //残り時間が0以上の場合
        if (remain > 0) {
            //残り時間を減らし続ける
            remain -= Time.deltaTime;
            //残り時間を小数点第二桁までテキストで表示する
            txtTime.text = remain.ToString("f2");
        //残り時間が0以上でなければ
        } else {
            if (!isWait) {
                //タイトルテキストに文字を出力する
                txtTitle.text = "Time Up!";
                //コルーチン処理「ReStart」を実行
                StartCoroutine("ReStart");
            }
            //残り時間を0にする
            remain = 0.0f;
            //残り時間を0で表示する
            txtTime.text = remain.ToString("f2");
            //開始待ち判定を真にする
            isWait = true;
        }
    }

    //コルーチン処理でピースの落下判定
    IEnumerator MoveCheck() {
        //ランキングの文字を白紙にする
        for (int i = 0; i < 5; i++) {
            txtRank[i].text = " ";
        }
        //1秒間のウェイトを入れる
        yield return new WaitForSeconds(1);
        //while文を回す(基本的にbreakで出て行くまでひたすら回る)
        while (true) {
            //停止判断を真とする
            bool isStop = true;
            //ピースタグの付いている全てのオブジェクトを格納
            GameObject[] AllPiece = GameObject.FindGameObjectsWithTag("Piece");
            //存在するピースを全て検査
            foreach (GameObject stored in AllPiece) {
                //加速度が0.3以上の場合(ピースの落下中とか)
                if (stored.GetComponent<Rigidbody2D>().velocity.magnitude > 0.3f ){
                    //停止判断を偽とする
                    isStop = false;
                }
            }
            //停止判断が真の場合(ピースが動いていない場合)
            if (isStop) {
                //開始待ちを偽にする
                isWait = false;
                //タイトル文字を変更する
                txtTitle.text = "GO!";
                //1秒間のウェイトを入れる
                yield return new WaitForSeconds(1);
                //タイトル文字を白紙にする
                txtTitle.text = "";
                //タイトル文字を白紙化した判定を真とする
                startwait = true;
                //この処理から出て行く
                break;
            }
            //処理から出て行く
            yield return null;
        }
    }

    //コルーチン処理でシーンのリロードを行う
    IEnumerator ReStart() {
        setRank();
        //3秒待機する
        yield return new WaitForSeconds(3);
        //タイトルテキストを変更する
        txtTitle.text = "Game Over";
        //リセットボタンをONにする
        Reset.SetActive(true);
        //ランキングボタンをONにする
        Ranking.SetActive(true);

        /*        //ループ処理
                while (true) {
                    //マウスの左ボタンを押した時
                    if (Input.GetMouseButtonDown(0)) {
                        //自シーンMainをリロード
                        SceneManager.LoadScene("_Main");
                    }
                    //処理から出て行く
                    yield return null;
        
    }*/
    }



    //ランキングを管理する処理
    void setRank() {
        //0点なら処理しない
        if (score == 0) return;
        //スコアがいるべき順位をゼロ位とする
        newRank = 0;
        //スコアが何位かを特定する。
        for (int i = 5; i > 0; i--) {
            //スコアがランクの点数より高ければ
            if (score > Rank[i]) {
                //ニューランクの所に番号を入れる
                newRank = i;
            }
        }
        //ゼロ位のままだったら何もしない
        if (newRank != 0) {
            //下位から順に繰り下げ処理を行う
            for (int i = 4; i >= newRank; i--) {
                //ランクの順位の繰り下げ処理を行う
                Rank[i + 1] = Rank[i];
            }
            //空いた席にスコアを入れる
            Rank[newRank] = score;
            Debug.Log(newRank);
        }
        //ゲームデータにランキングの数値を格納する
        GameDate.Point1 = Rank[1];
        GameDate.Point2 = Rank[2];
        GameDate.Point3 = Rank[3];
        GameDate.Point4 = Rank[4];
        GameDate.Point5 = Rank[5];


        PlayerPrefs.SetInt("Rank1", Rank[1]);
        PlayerPrefs.SetInt("Rank2", Rank[2]);
        PlayerPrefs.SetInt("Rank3", Rank[3]);
        PlayerPrefs.SetInt("Rank4", Rank[4]);
        PlayerPrefs.SetInt("Rank5", Rank[5]);

    }

    //リスタートボタンの管理
    public void ResetButton() {
        SceneManager.LoadScene("_Main");
    }

    //ランキングボタンの管理
    public void RankingButton() {
        switch (index) {
            case 0:
                txtRanking.text = "戻る";
                for (int i = 0; i < 5; i++) {
                    txtRank[i].text = "Rank " + (i + 1) + ": " + Rank[i + 1];
                }
                //タイトルテキストを変更する
                txtTitle.text = "";
                //下のバーを消してしまう
                bar.SetActive(false);
                index = 1;
                break;

            case 1:
                txtRanking.text = "ランキング";
                for (int i = 0; i < 5; i++) {
                    txtRank[i].text = " ";
                }
                //タイトルテキストを変更する
                txtTitle.text = "Game Over";
                index = 0;
                break;
        }
    }
}
