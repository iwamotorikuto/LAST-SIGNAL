using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody rb;
    private float speed = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Playerの前後左右の移動
        float xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime; //左右の移動
        float zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;   //前後の移動
        transform.Translate(xMovement, 0, zMovement); //オブジェクトの位置を更新

        //マウスカーソルで左右視点移動
        float mx = Input.GetAxis("Mouse X"); //カーソルの横の移動量を取得
        float my = Input.GetAxis("Mouse Y"); //カーソルの縦の移動量を取得
        if(Mathf.Abs(mx) > 0.001f) //X方向に一定量移動していれば横回転
        {
            transform.RotateAround(transform.position, Vector3.up, mx); //回転軸はplayerオブジェクトのワールド座標Y軸
        }

        ////Wキー(前方移動)
        //if (Input.GetKey(KeyCode.W))
        //{
        //    transform.position += speed * transform.forward * Time.deltaTime;
        //}    
        ////Sキー(後方移動)
        //if (Input.GetKey(KeyCode.S))
        //{
        //    transform.position -= speed * transform.forward * Time.deltaTime;
        //}    
        ////Dキー(右移動)
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.position += speed * transform.right * Time.deltaTime;
        //}    
        ////Aキー(左移動)
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.position -= speed * transform.right * Time.deltaTime;
        //}    
    }
}
