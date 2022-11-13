using System;
using System.Threading;

namespace HughUtility
{
    //T type�� ���� LazySingleton�� �� �̴ϴ�.
    //T type���� class�� ��� �޽��ϴ�.
    //�̷� ������ ���� �����ؼ� �ڵ带 �ۼ�����!!
    public class LazySingleton<T> where T : class
    {
        //CreateInstance ȣ���ؼ� Lazy<T> type���� ���� �б��������� �����Ұ̴ϴ�
        private static readonly Lazy<T> instance = new Lazy<T>(CreateInstance, LazyThreadSafetyMode.ExecutionAndPublication);

        //t type���� ���� ��ȯ�Ұ̴ϴ�.
        private static T CreateInstance()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        // T type�� ���� instance�� �� �ҷ��� ���̴ϴ�.
        public static T GetInstance
        {
            get
            {
                return instance.Value;
            }
        }

        // ������, �Ҹ��� ���� ������
        protected LazySingleton() { }
        ~LazySingleton() { }
    }
}