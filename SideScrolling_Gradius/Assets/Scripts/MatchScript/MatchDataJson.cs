using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

public class MatchDataJson
{
    public static string PositionAndVelocity(Vector2 velocity, Vector2 position)
    {
        var values = new Dictionary<string, string>
        {
            {"velocity.x", velocity.x.ToString() },
            {"velocity.y", velocity.y.ToString() },
            {"position.x", position.x.ToString() },
            {"position.y", position.y.ToString() }
        };

        return values.ToJson();
    }

    public static string Input(float horizontalInput, float verticalInput, bool fireInput)
    {
        var values = new Dictionary<string, string>
        {
            {"horizontalInput", horizontalInput.ToString() },
            {"verticalInput", verticalInput.ToString() },
            {"fireInput", fireInput.ToString() }
        };

        return values.ToJson();
    }

    public static string Point(float point)
    {
        var values = new Dictionary<string, string>
        {
            {"point", point.ToString() }
        };

        return values.ToJson();
    }

    //매치 진입 수 리스폰 해주는 OpCodes 던지기 위해선 필요
    public static string Respawned(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() },
        };

        return values.ToJson();
    }

    public static string Died(Vector2 position)
    {
        var values = new Dictionary<string, string>
        {
            {"position.x", position.x.ToString() },
            {"position.y", position.y.ToString() },
        };

        return values.ToJson();
    }
}
