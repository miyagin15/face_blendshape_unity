using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Linq;

public class BlendShapeCSV : MonoBehaviour {

    private string CSVblendshape = "";
    private string blendShapeList = "";
    // Use this for initialization
    void Start()
    {
        // // ファイル書き出し
        // // 現在のフォルダにsaveData.csvを出力する(決まった場所に出力したい場合は絶対パスを指定してください)
        // // 引数説明：第1引数→ファイル出力先, 第2引数→ファイルに追記(true)or上書き(false), 第3引数→エンコード
        // StreamWriter sw = new StreamWriter(@"saveData.csv", false, Encoding.GetEncoding("Shift_JIS"));
        // // ヘッダー出力
        // string[] s1 = { "プレイヤー名", "記録" };
        // string s2 = string.Join(",", s1);
        // sw.WriteLine(s2);
        // // データ出力
        // for (int i = 0; i < 3; i++)
        // {
        //     string[] str = { "tatsu", "" + (i + 1) };
        //     string str2 = string.Join(",", str);
        //     sw.WriteLine(str2);
        // }
        // // StreamWriterを閉じる
        // sw.Close();


        // // ファイル読み込み
        // // 引数説明：第1引数→ファイル読込先, 第2引数→エンコード
        // StreamReader sr = new StreamReader(@"saveData.csv", Encoding.GetEncoding("Shift_JIS"));
        // string line;
        // // 行がnullじゃない間(つまり次の行がある場合は)、処理をする
        // while ((line = sr.ReadLine()) != null)
        // {
        //     // コンソールに出力
        //     Debug.Log(line);
        // }
        // // StreamReaderを閉じる
        // sr.Close();
    }
    public float[] beforeValue = new float[62];
    public float[] nowValue = new float[62];
    int blendshapeint = 0;
	bool kaigyo = false;
    int roop = 0;
    // Update is called once per frame
    void Update () {
        var net = NetworkMeshAnimator.Instance;
        var nameList = net.blendShapeName;

        var dicShapeWeight = net.dic;
        var dict_sorted = dicShapeWeight.OrderBy((x) => x.Key);  //昇順に変更
        int i = 0;
        int countSame = 0;
        roop = roop + 1;
        if(roop %3==0){
            foreach (KeyValuePair<string, float> pair in dict_sorted)
            {
                beforeValue[i] = nowValue[i];
                nowValue[i] = pair.Value;
                i = i + 1;
                if ((nowValue[i] == beforeValue[i]))
                {
                    countSame = countSame + 1;
                }
                if (countSame > 50)
                {
                    Debug.Log("no face ");
                    return;
                }
            }
            foreach (KeyValuePair<string, float> pair in dict_sorted)
            {
                if (blendshapeint == 0)
                {
                    blendShapeList = blendShapeList + pair.Key + ",";
                }

                if (CSVblendshape == "")
                {
                    CSVblendshape = (pair.Value).ToString("f1");
                }
                else if (pair.Key.Contains("tongueOut"))
                {
                    CSVblendshape = CSVblendshape + "," + (pair.Value).ToString("f1") + "\n";
                    blendshapeint = blendshapeint + 1;
                    kaigyo = true;
                    return;
                }
                else if (kaigyo)
                {
                    CSVblendshape = CSVblendshape + (pair.Value).ToString("f1");
                    kaigyo = false;
                }
                else
                {
                    CSVblendshape = CSVblendshape + "," + (pair.Value).ToString("f1");
                }

            }
		}

    }
	public void makeCSV(){
        StreamWriter sw = new StreamWriter(@"BlendShapeData.csv", false, Encoding.GetEncoding("Shift_JIS"));
        // ヘッダー出力
        sw.WriteLine(blendShapeList);
        // データ出力
        sw.WriteLine(CSVblendshape);
        // StreamWriterを閉じる
        sw.Close();
	}
}
