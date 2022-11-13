using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;

namespace HughUtility
{
    //Singleton class도 MonoBehaviour 상속받고 T 도 상속받을 것이다.
    //왜냐? Awake()에서 DontDestroyOnLoad 사용하려고
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj;
                    obj = GameObject.Find(typeof(T).Name);

                    if (obj == null)
                    {
                        obj = new GameObject(typeof(T).Name);
                        instance = obj.AddComponent<T>();
                    }
                    else
                    {
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this);
            }
        }
    }
}