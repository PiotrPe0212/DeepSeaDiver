using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTime : MonoBehaviour
{
    ParticleSystem _particleSys;
    // Start is called before the first frame update
    void Start()
    {
        _particleSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale < 0.01f)
        {
            _particleSys.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
