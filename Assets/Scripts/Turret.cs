public class Turret : Enemy {
	public override void PeriodicUpdate() {
		base.PeriodicUpdate();

		if (AttackTarget) {
			//rotate _turretPivot towards AttackTarget
			//
		}
	}

	public override void ClearTarget() {
		base.ClearTarget();
		
		Anim.Play("Idle");
	}
}
