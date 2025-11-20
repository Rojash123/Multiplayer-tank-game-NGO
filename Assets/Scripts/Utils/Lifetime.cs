using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifeTime = 2f;
    private void OnEnable()
    {
        Destroy(gameObject,lifeTime);
    }
}
