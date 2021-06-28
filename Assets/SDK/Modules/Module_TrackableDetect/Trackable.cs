using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trackable
{
    public int trackableId;

    protected Trackable(int id)
    {
        this.trackableId = id;
    }

    public int GetTrackableId()
    {
        return trackableId;
    }
}
