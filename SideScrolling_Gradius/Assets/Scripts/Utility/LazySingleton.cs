using System;
using System.Threading;

namespace HughUtility
{
    //T type에 대해 LazySingleton을 줄 겁니다.
    //T type들은 class를 상속 받습니다.
    //이런 식으로 말로 이해해서 코드를 작성하자!!
    public class LazySingleton<T> where T : class
    {
        //CreateInstance 호출해서 Lazy<T> type으로 오직 읽기전용으로 생성할겁니다
        private static readonly Lazy<T> instance = new Lazy<T>(CreateInstance, LazyThreadSafetyMode.ExecutionAndPublication);

        //t type으로 만들어서 반환할겁니다.
        private static T CreateInstance()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        // T type에 대해 instance한 걸 불러와 쓸겁니다.
        public static T GetInstance
        {
            get
            {
                return instance.Value;
            }
        }

        // 생성자, 소멸자 선언 해주자
        protected LazySingleton() { }
        ~LazySingleton() { }
    }
}