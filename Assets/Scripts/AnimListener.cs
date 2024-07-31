using UnityEngine;

public class AnimListener : MonoBehaviour {

    private Unit unit;

    private void Awake() {
        unit = GetComponentInParent<Unit>();
        if (unit == null) {
            unit = transform.root.GetComponent<Unit>();
        }

    }

    public void Fire() {
        //Debug.Log("Fire!!!!!!");
        if (unit && (unit.State != UnitState.Dead || unit.State != UnitState.Locked)) {
            unit.Fire();
        }
    }
    
    public void LeftFootstep() {
        Debug.Log("Left Step");
        if (!unit || !unit.UnitStats || unit.UnitStats.Footsteps.Length == 0 || !unit.LeftFoot) {
            return;
        }

        float _volume = .3f;
        int _priority = 150;
        
        if (unit.UnitStats.Name == "Liberated") {
            _volume = .1f;
            _priority = 200;
        }
        
        AudioManager.Instance.Play(unit.UnitStats.Footsteps, MixerGroups.SFX, new Vector2(.9f, 1.1f), _volume, unit.LeftFoot.position, _priority);
    }
    
    public void RightFootstep() {
        if (!unit || !unit.UnitStats || unit.UnitStats.Footsteps.Length == 0 || !unit.RightFoot) {
            return;
        }
        AudioManager.Instance.Play(unit.UnitStats.Footsteps, MixerGroups.SFX, new Vector2(.9f, 1.1f), .1f, unit.RightFoot.position, 200);
    }

}
