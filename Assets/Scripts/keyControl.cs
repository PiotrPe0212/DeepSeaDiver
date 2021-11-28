using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyControl : MonoBehaviour
{

    private Rigidbody2D _key;
    private float _staticForceValue = 98;
    // Start is called before the first frame update
    void Start()
    {
        _key = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      //  _key.AddForce(Vector2.up * _staticForceValue, ForceMode2D.Force);
      //  StartCoroutine(keyFloating());
    }
    IEnumerator keyFloating()
    {
        yield return new WaitForSeconds(1);
        if (_staticForceValue == 1.88f)
        {
            _staticForceValue = 1.71f;

        }
        else if (_staticForceValue == 1.71f)
        {
            _staticForceValue = 1.88f;

        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
       // Debug.Log("iGetIT!!!1");
        if (other.gameObject.name == "diver")
        {
            Destroy(gameObject);
            GameManager.Instance.KeyGetting = 1;
            keyHook.Instance.KeyGetting = 1;
           
        }
    }

}
