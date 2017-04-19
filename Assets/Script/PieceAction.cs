using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceAction : MonoBehaviour {

    public Sprite EscImage;         //ニュートラルの時の画像
    public Sprite TapImage;         //タップ時の星付き画像
    public AudioClip TapSound;      //音:タップ時
    public AudioClip CancelSound;   //音:キャンセル時
    public GameObject my;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        DelPiece();

    }

    //ハイライト処理(星付き画像になる)
    void HighLight() {
        GetComponent<SpriteRenderer>().sprite = TapImage;
        AudioSource.PlayClipAtPoint(TapSound,transform.position);
    }

    void InitImage() {
        GetComponent<SpriteRenderer>().sprite = EscImage;
        AudioSource.PlayClipAtPoint(CancelSound, transform.position);

    }

    // 落下したピースを削除する
    void DelPiece() {
        if (my.transform.position.y < -20.0f) {
            Destroy(my);
        }
    }


}
