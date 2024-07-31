using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLiberated : MonoBehaviour {

    public float Speed;

    [SerializeField] private Vector3 _returnPos;
    private Vector3 _startPos;
    private float timer = 0f;

    private Animator _anim;
    private void Awake() {
        _anim = GetComponent<Animator>();
        _startPos = transform.position;
    }

    private void Update() {

        if (timer > 0f) {
            timer -= Time.deltaTime;
            return;
        }

        _anim.SetFloat("Speed", Speed);
        transform.position += (transform.forward * Time.deltaTime * Speed);

        if (Vector3.Distance(transform.position, _returnPos) < 0.1f) {
            transform.position = _startPos;
            timer = Random.Range(5f, 15f);
        }
        
    }

    public void LeftFootStep() { }
    public void RightFootStep() { }

}
