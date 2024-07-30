using System;

public static class MissionConditionExtensions
{
    public static MissionMarker.State ToMissionMarkerState(this MissionCondition missionCondition)
    {
        switch (missionCondition)
        {
            case MissionCondition.Complete:
                return MissionMarker.State.Completed;
            case MissionCondition.FailMininumLiberated:
            case MissionCondition.FailUnitsLost:
                return MissionMarker.State.Available;
            case MissionCondition.Locked:
                return MissionMarker.State.Locked;
            default:
                throw new ArgumentOutOfRangeException(nameof(missionCondition), missionCondition, null);
        }
    }
}