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
    Vector3 initialPos = new Vector3(-8f, 5f, -1f);
    private bool correctInitPos;
    void Start()
    {
        
    }

    private void Awake()
    {
        _gameManager.InitGame += InitialCamera;
        _gameManager.LoadLevel += InitialCamera;
        _gameManager.ResetLevel += InitialCamera;
    }

    private void Destroy()
    {
        _gameManager.InitGame -= InitialCamera;
        _gameManager.LoadLevel -= InitialCamera;
        _gameManager.ResetLevel -= InitialCamera;
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
            correctInitPos = false;
    }
        else
        {
            if(!correctInitPos)
            InitialCamera();
        }
}

    private void InitialCamera()
    {
        transform.position = Vector3.Lerp(transform.position, initialPos, timeOffset * Time.deltaTime);
        correctInitPos = true;
    }
}
