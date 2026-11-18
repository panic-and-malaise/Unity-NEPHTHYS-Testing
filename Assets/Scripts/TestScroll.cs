using UnityEngine;

public class TestScroll : MonoBehaviour
{
    [SerializeField] private float _speed;

    void Update()
    {
        transform.position = transform.position + new Vector3(0, _speed * Time.deltaTime, 0);
        if (transform.position.y >= 5.5)
            transform.position = new Vector3(0, -5.5f, 0);
    }
}
