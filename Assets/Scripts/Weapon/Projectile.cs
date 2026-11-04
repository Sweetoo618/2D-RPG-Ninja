using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime = 3f;
    
    public Vector3 Direction { get; set; }
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
