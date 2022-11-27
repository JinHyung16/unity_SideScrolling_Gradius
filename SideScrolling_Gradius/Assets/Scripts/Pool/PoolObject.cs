using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    /// <summary>
    /// Pooling ���� object���� �ݵ�� ���� �־�� �� script
    /// NewPoolManager���� �ش� Object�� PoolObject Component�� ���ٸ�
    /// ������ �Ҵ����ְ� �ִ�.
    /// </summary>
    
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public void RemovePrefab()
    {
        this.gameObject.SetActive(false);
    }
}
