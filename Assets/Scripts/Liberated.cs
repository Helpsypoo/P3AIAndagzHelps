public class Liberated : Unit {
    public bool IsLeader;
    
    public void PeriodicUpdate() {
        base.PeriodicUpdate();

        if (Health <= 0) {
            return;
        }
        
        if (IsLeader) {
            if (GameManager.Instance.IsProcessing) {
                MoveTo(GameManager.Instance.KillZone.position);
                return;
            }
            if (GameManager.Instance.ActiveWaypoints.Count > 0) {
                MoveTo(GameManager.Instance.ActiveWaypoints[0].transform.position);
            }
        } else {
            MoveTo(GameManager.Instance.GetFollowingPosition(this));
        }
    }

    public override void Die() {
        base.Die();
        
        GameManager.Instance.ProcessLiberatedDeath(this);
    }
}
