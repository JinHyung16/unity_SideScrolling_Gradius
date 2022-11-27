using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    /// <summary>
    /// Pooling 당할 object들이 반드시 갖고 있어야 할 script
    /// NewPoolManager에서 해당 Object에 PoolObject Component가 없다면
    /// 강제로 할당해주고 있다.
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
