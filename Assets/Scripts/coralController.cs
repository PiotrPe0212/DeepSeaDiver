using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coralController : MonoBehaviour
{

    private Collider2D _detectingBox;
    private Vector2 _detectVector;
    public LayerMask _layerToDetect;
    private Vector2 _detectScaleVector;
    private bool _diverFromAbove;
    // Start is called before the first frame update
    void Start()
    {
        _detectVector = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y+0.85f);
        _detectScaleVector = new Vector2(gameObject.transform.localScale.x * 0.85f, gameObject.transform.localScale.y*0.2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _detectingBox = Physics2D.OverlapBox(_detectVector, _detectScaleVector, 0, _layerToDetect, -1, 3);
        if (!_detectingBox)
        {
            return;
        }
        if (_detectingBox.name == "diver")
            _diverFromAbove = true;
        else
            _diverFromAbove = false;
    }
void OnCollisionStay2D(Collision2D collision) {
    if(collision.gameObject.name == "diver" && _diverFromAbove == true){
 OxygenCounter.Instance.Damage = true;
}

}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (true)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(_detectVector, _detectScaleVector);
    }

}
