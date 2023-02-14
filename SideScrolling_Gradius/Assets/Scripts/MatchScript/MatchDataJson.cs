using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

public class MatchDataJson
{
    public static string Position(Vector2 velocity, Vector2 position)
    {
        var values = new Dictionary<string, string>
        {
            {"velocity_x", velocity.x.ToString() },
            {"velocity_y", velocity.y.ToString() },
            {"position_x", position.x.ToString() },
            {"position_y", position.y.ToString() }
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

    //매치 진입 수 리스폰 해주는 OpCodes 던지기 위해선 필요
    public static string SpawnPlayer(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() },
        };

        return values.ToJson();
    }

    public static string MultiScoreUpdate(int score)
    {
        var values = new Dictionary<string, string>
        {
            { "multiScore", score.ToString() },
        };

        return values.ToJson();
    }
    public static string EnemyPosition(Vector2 velocity, Vector2 position)
    {
        var values = new Dictionary<string, string>
        {
            {"enemy_velocity_x", velocity.x.ToString() },
            {"enemy_velocity_y", velocity.y.ToString() },
            {"enemy_position_x", position.x.ToString() },
            {"enemy_position_y", position.y.ToString() }
        };

        return values.ToJson();
    }

    public static string EnemyDiePos(Vector2 position)
    {
        var values = new Dictionary<string, string>
        {
            { "enemy_die_pos_x", position.x.ToString() },
            {"enemy_die_pos_y", position.y.ToString() }
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
