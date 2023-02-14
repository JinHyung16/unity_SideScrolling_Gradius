using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HughUtility
{
    /*
    public class Observer : MonoBehaviour
    {

    }
    */

    public interface MultiplayObserver
    {
        void UpdateScore(int score);
    }

    public interface MultiplaySubject
    {
        void RegisterObserver(MultiplayObserver observer);
        void RemoveObserver(MultiplayObserver observer);
        void NotifyObserver();
    }
}
