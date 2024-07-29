using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : Unit {
	[SerializeField] private GameObject _destructionEffect;
	public override void Die() {
		if (_destructionEffect) {
			Instantiate(_destructionEffect, transform.position, Quaternion.identity);
		}
		
		gameObject.SetActive(false);
	}
}
