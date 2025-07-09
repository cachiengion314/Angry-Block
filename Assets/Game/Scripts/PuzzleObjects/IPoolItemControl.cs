using UnityEngine;
using UnityEngine.Pool;

public interface IPoolItemControl
{
    public void InjectPool(ObjectPool<GameObject> grassPool, ObjectPool<GameObject> other = null);
    public void RemoveFromTable(); // execute destroy along side with game logic
    public void FullyRemoveFromTable(); // execute destroy along side with game logic
    public void Release(); // just make item disappear
}
