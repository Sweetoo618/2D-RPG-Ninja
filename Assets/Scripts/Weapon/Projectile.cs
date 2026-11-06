using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime = 3f;
    // 贴图自身朝向：0 为朝右，90 为朝上
    [SerializeField] private float spriteAngle;
    
    private Vector3 _direction;

    public Vector3 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            if (_direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - spriteAngle;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    public float Damage { get; set; }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (Direction.sqrMagnitude <= 0.0001f) return;
        transform.position += Direction.normalized * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponentInParent<IDamageable>()?.TakeDamage(Damage);
        Destroy(gameObject);
    }
}
