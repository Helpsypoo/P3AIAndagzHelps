using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percival : Unit {

    [SerializeField] private GameObject _minePrefab;
    [SerializeField] private int _mines;

    private bool _placingMine;
    private Landmine _mineToDisarm;

    public override void Awake() {
        base.Awake();
        _abilityCharges = _mines;
    }

    public override void PeriodicUpdate() {
        base.PeriodicUpdate();

        //if (_placingMine && AtDestination) {
        //    _placingMine = false;
        //    GameObject newMine = Instantiate(_minePrefab, transform.position, Quaternion.identity);
        //}

        if (_mineToDisarm != null && AtDestination) {
            Destroy(_mineToDisarm.gameObject);
            _mineToDisarm = null;
            
        }

    }

    public override void PerformAction(Vector3 position, Transform target = null) {

        if (target != null && target.CompareTag(Globals.MINES_TAG)) {
            Landmine mine = target.GetComponent<Landmine>();
            if (mine == null) {
                throw new System.Exception($"Attempted to get Landmine component from object, {target.name}, which was tagged as a Mine, but no Landmine component was found.");
            }
            _mineToDisarm = mine;
            MoveTo(position);
            return;
        }

        if (_abilityCharges < 1) return;

        StartCoroutine(ThrowMine(position));
        GameManager.Instance.MoveArrow.Play(GameManager.Instance.SelectionMarker.Position, GameManager.Instance.SelectionMarker.transform.up);
        //_placingMine = true;
        //MoveTo(position);
        _abilityCharges--;

    }

    private IEnumerator ThrowMine(Vector3 position) {

        GameObject newMine = Instantiate(_minePrefab, transform.position, Quaternion.identity);

        float timer = 0;
        while (timer < 1f) {
            newMine.transform.position = Vector3.Lerp(newMine.transform.position, position, timer);
            timer += Time.deltaTime * 0.5f;
            yield return null;
        }

    }

}
