using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class NewPoolManager : MonoBehaviour
{
    #region Singleton
    private static NewPoolManager instance;

    public static NewPoolManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Pooling();
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private Dictionary<PoolableType, Dictionary<string, List<PoolObject>>> PoolDictionary = new Dictionary<PoolableType, Dictionary<string, List<PoolObject>>>();

    private void Pooling()
    {
        foreach (PoolableType type in Enum.GetValues(typeof(PoolableType)))
        {
            PoolDictionary.Add(type, new Dictionary<string, List<PoolObject>>());
        }

    }

    public GameObject GetPrefab(PoolableType _type, string _name)
    {
        if (!PoolDictionary[_type].ContainsKey(_name))
        {
            PoolDictionary[_type].Add(_name, new List<PoolObject>());
        }

        if (PoolDictionary[_type][_name].Count < 1)
        {
            PoolDictionary[_type][_name].Add(CreatePoolObject(_type, _name));
        }

        PoolObject obj = PoolDictionary[_type][_name][0];
        PoolDictionary[_type][_name].Remove(obj);
        obj.RemovePrefab();

        return (obj.gameObject);
    }

    public void DespawnObject(PoolableType _type, GameObject obj)
    {
        if (obj.TryGetComponent<PoolObject>(out PoolObject poolObj))
        {
            if (PoolDictionary[_type].ContainsKey(poolObj.Name))
            {
                poolObj.RemovePrefab();
                PoolDictionary[_type][poolObj.Name].Add(poolObj);
            }
        }
    }

    private PoolObject CreatePoolObject(PoolableType _type, string _name)
    {
        GameObject obj = Resources.Load(PrefabPath(_type) + _name, typeof(GameObject)) as GameObject;

        if (obj == null)
        {
            return null;
        }

        obj = Instantiate(obj);

        if (obj.TryGetComponent<PoolObject>(out PoolObject poolObj))
        {
            poolObj.Name = _name;
            return poolObj;
        }
        else
        {
            poolObj = obj.AddComponent<PoolObject>();
            poolObj.Name = _name;
            return poolObj;
        }
    }

    private string PrefabPath(PoolableType _type)
    {
        switch (_type)
        {
            case PoolableType.Item:
                return "Prefab/Item/";
            case PoolableType.EChaser:
                return "Prefab/Enemy/";
            case PoolableType.EBoomber:
                return "Prefab/Enemy/";
            case PoolableType.EUFO:
                return "Prefab/Enemy/";
            case PoolableType.EGround:
                return "Prefab/Enemy/";
            case PoolableType.EBoss:
                return "Prefab/Enemy/";
            case PoolableType.EBullet:
                return "Prefab/EnemyBullet/";
            case PoolableType.PBullet:
                return "Prefab/PlayerBullet/";
            case PoolableType.PShell:
                return "Prefab/PlayerBullet/";
            case PoolableType.MultiChaser:
                return "NakamaPrefab/";
        }
        return "Prefab/";
    }
    public enum PoolableType
    {
        //I로 시작하는건 item
        Item, //power up item , increase shell item

        //E로 시작하는건 Enemy 관련
        EChaser,
        EBoomber,
        EUFO,
        EGround,
        EBoss,

        EBullet, //enemy bullet 관련

        //P로 시작하는건 Player 관련
        PBullet, //player bullet
        PShell, //player shell

        MultiChaser, //multiplay에 사용될 Chaser Enemy
    }
}
