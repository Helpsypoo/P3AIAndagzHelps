public class Liberated : Unit {
    public bool IsLeader;
    
    public override void Update() {
        LookWhereYoureGoing();
    }

    public void PeriodicUpdate() {
        base.PeriodicUpdate();
        
        if (IsLeader) {
            if (GameManager.Instance.ActiveWaypoints.Count > 0) {
                MoveTo(GameManager.Instance.ActiveWaypoints[0].transform.position);
            }
        } else {
            MoveTo(GameManager.Instance.GetFollowingPosition(this));
        }
    }
}
