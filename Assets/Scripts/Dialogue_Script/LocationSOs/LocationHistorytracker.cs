using System.Collections.Generic;
using UnityEngine;

public class LocationHistorytracker : MonoBehaviour
{
    public static LocationHistorytracker Instance;
    private readonly HashSet<LocationSO> locationsVisited = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RecordLocation(LocationSO locationSO)
    {
        if (locationSO != null && locationsVisited.Add(locationSO))
            Debug.Log("Location Recorded: " + locationSO.displayName);
    }

    public bool HasVisited(LocationSO locationSO)
    {
        return locationsVisited.Contains(locationSO);
    }
}
