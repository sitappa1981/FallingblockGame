using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAction : MonoBehaviour {

    public GameObject[] piece;      //ピースのプレハブを配列で扱う
    GameObject pieceObject;
    public int initNum = 45;        //初期セットされるピース数
    public float interval = 0.1f;   //生成する時間間隔
    public bool isTap;              //マウスがクリックされているかどうかの判定
    public AudioClip Popsound;      //消去時の効果音
    public AudioClip Playmusic;     //ゲーム中のバックサウンド


    List<GameObject> ChainPiece;    //連鎖ピースを格納する可変長配列
    public string ColorNum;         //格納処理の実行と共にピースの名前から色番号を退避させる場所

    // Use this for initialization
    void Start () {
        //クリックされている判定を偽にする
        isTap = false;
        //音を鳴らす 
        AudioSource.PlayClipAtPoint(Playmusic, transform.position);
        //コルーチン処理を指示する
        StartCoroutine("SetPiece",initNum);
	}
	
	// Update is called once per frame
	void Update () {
        //ゲームマネージャーのisWaitが真の場合は機能しない
        if (GameManager.isWait) return;
        //マウスがクリックされた時
        if (Input.GetMouseButtonDown(0) && !isTap) {
            //isTapを真にする
            isTap = true;
            //OnTapを実行する
            OnTap();
            //マウスの左クリックを離した時
        } else if(Input.GetMouseButtonUp(0)){
            //isTapを偽にする
            isTap = false;
            if (ChainPiece.Count > 0) {
                //OffTapのコルーチンを実行
                StartCoroutine("OffTap",gameObject);
            }

        } else if (isTap) {
            isDrag();
        }
    }

    //ピースを生成するコルーチン処理
    IEnumerator SetPiece(int Num) {
        //Num回繰り返す
        for (int idx = 0; idx < Num; idx++) {
            //ランダムで0～7の数字を代入する(ボールが8種類ある為)
            int idz = Random.Range(0,8);
            //x座標に対して-1.7f～1.7fの範囲を代入する(横幅に幅を持たせる為、これを行わないとそのまま縦に落ちる)
            float x = Random.Range(-1.7f,1.7f);
            //横幅x縦幅を画面外の上の所に座標を指定する
            Vector2 test = new Vector2(x,6.5f);
            //ピースをインスタンス生成する
            pieceObject = Instantiate(piece[idz], test, Quaternion.identity);
            //コルーチンで0.1秒待機する
            yield return new WaitForSeconds(interval);
        }
    }

    //マウスをクリックされた時
    void OnTap() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
        if (hit.collider) {
            //ピースタグでなければ処理しない
            if (hit.collider.gameObject.tag != "Piece") return;
            //コンソールに名前を表示する
            Debug.Log(hit.collider.gameObject.name);
            //色番号を文字として取り出して格納する(6文字目から1桁の色番号を確保する)
            ColorNum = hit.collider.gameObject.name.Substring(6, 1);
            //Raycastで確認したgameObjectに対してSendMessage"HighLight"を送る
            hit.collider.gameObject.gameObject.SendMessage("HighLight", SendMessageOptions.DontRequireReceiver);
            //押し始めでリスト配列を初期化
            ChainPiece = new List<GameObject>();
            //1個目を格納
            ChainPiece.Add(hit.collider.gameObject);
        }
    }

    //クリックされたピースが一秒後に消去するコルーチン処理
    IEnumerator OffTap(GameObject TapPiece) {
        //削除した個数をカウントする変数を宣言
        int AddPiece = 0;
        //３個以上なら
        if (ChainPiece.Count > 2) {
            //クリックされたオブジェクトを全て 
            foreach (GameObject stored in ChainPiece) {
                //消去処理を行う
                Destroy(stored);
                //音を鳴らす 
                AudioSource.PlayClipAtPoint(Popsound, transform.position);
                //削除した個数を増やす
                AddPiece++;
                //消した個数に応じて点数を加算(1個かける100)
                GameManager.score += AddPiece * 100;
            }
            //チップのリストを空にする
            ChainPiece = new List<GameObject>();
            //削除した分のピースを生成する
            StartCoroutine("SetPiece",AddPiece);
        } else {
            //２個以下の場合は、クリアーを実行する
            ClearPiece(); 
        }
        yield return null;
    }

    //クリックされたままドラックされた時
    void isDrag() {
        //同じピースかどうかの真偽値
        bool isSame = false; 
        //レイキャストでオブジェクトを認識する
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //レイキャスト先にコライダーがあった場合
        if (hit.collider) {
            //ピースでなければ処理対象外
            if (hit.collider.gameObject.tag != "Piece") return;
            //格納中の全ピースを検査する 
            foreach (GameObject stored in ChainPiece) { 
                if (stored == hit.collider.gameObject) {
                    //同じピースを選択したを真とする
                    isSame = true; 
                }
            }
            //同じピースで無ければ格納する
            if (!isSame) {
                if (hit.collider.gameObject.name.Substring(6,1) == ColorNum) {
                    //Raycastで確認したgameObjectに対してSendMessage"HighLight"を送る
                    hit.collider.gameObject.SendMessage("HighLight");
                    //ドラッグ中にヒットしたピースを格納
                    ChainPiece.Add(hit.collider.gameObject);
                } else {
                    //色が異なる場合の処理を実行
                    ClearPiece();
                }
            }
        }
    }

    //選択したピースの色が異なる場合
    void ClearPiece() {
        //これまでクリックしたピース全てに対して
        foreach (GameObject stored in ChainPiece) {
            //元画像に戻す伝達
            stored.SendMessage("InitImage");
        }
        //格納していた変動配列を初期化
        ChainPiece = new List<GameObject>();
    }
}
