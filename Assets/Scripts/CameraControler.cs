using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    GameObject player;

    [SerializeField] private GameManager _gameManager;

    [SerializeField]
    float timeOffset;

    [SerializeField]
    Vector2 posOffset;
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  player = GameObject.Find("diver");
if(player){
        Vector3 startPos = transform.position;
        Vector3 endPos = player.transform.position;
            endPos.x += posOffset.x;
            endPos.y -= posOffset.y;
            endPos.z = -1;

        transform.position = Vector3.Lerp(startPos, endPos, timeOffset*Time.deltaTime);
    }
}
}
