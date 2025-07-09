using UnityEngine;

public class NotifyCountdown : MonoBehaviour
{
    float timer = 0;
    [SerializeField] float _timeDestroy = 3;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > _timeDestroy)
        {
            Destroy(this.gameObject);
            timer = 0;

        }
    }
}
