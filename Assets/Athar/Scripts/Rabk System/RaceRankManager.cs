using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceRankManager : MonoBehaviour
{
    public static RaceRankManager Instance;

    private List<RaceProgressTracker> racers = new List<RaceProgressTracker>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterCar(RaceProgressTracker tracker)
    {
        if (!racers.Contains(tracker))
            racers.Add(tracker);
    }

    public int GetRank(RaceProgressTracker car)
    {
        var sorted = racers
            .OrderByDescending(r => r.currentLap)
            .ThenByDescending(r => r.trackProgress)
            .ToList();

        return sorted.IndexOf(car); // 0 = first
    }

    public int TotalCars => racers.Count;
}
