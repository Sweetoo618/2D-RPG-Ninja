using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Weapon secondaryWeapon;
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private Transform[] attackPositions;
    [SerializeField] private GameObject slashEffectPrefab;
    
    private PlayerActions _actions;
    private PlayerAnimations _playerAnimations;
    private PlayerMovement _playerMovement;
    private PlayerMana _playerMana;
    private EnemyBrain _enemyTarget;
    private Coroutine _attackCoroutine;

    private Transform _currentAttackPosition;
    private Vector2 _lastAttackDirection = Vector2.down;
    private Weapon _currentWeapon;

    public Weapon CurrentWeapon => _currentWeapon;
    public Weapon InitialWeapon => initialWeapon;
    public Weapon SecondaryWeapon => secondaryWeapon;
    public event Action AttackPerformed;

    private void Awake()
    {
        _actions = new PlayerActions();
        _playerMana = GetComponent<PlayerMana>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAnimations = GetComponent<PlayerAnimations>();
        _currentWeapon = initialWeapon;
        if (attackPositions != null && attackPositions.Length >= 4)
        {
            _currentAttackPosition = attackPositions[2];
        }
    }

    private void Update()
    {
        if (DialogueManager.IsOpen) return;
        GetFirePosition();
    }

    private void Start()
    {
        _actions.Attack.ClickAttack.performed += ctx => Attack();
    }

    private void Attack()
    {
        if (DialogueManager.IsOpen) return;
        if (_currentWeapon == null || _enemyTarget == null) return;
        StopAttack();
        _attackCoroutine = StartCoroutine(IEAttack());
    }

    private void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
        if (_playerAnimations != null) _playerAnimations.SetAttackAnimation(false);
    }

    private IEnumerator IEAttack()
    {
        if (_currentWeapon == null) yield break;

        bool isCritical;
        float damage = _currentWeapon.CalculateDamage(out isCritical);
        Player player = GetComponent<Player>();
        if (player != null && player.Stats != null)
        {
            damage += _currentWeapon.weaponType == WeaponType.Magic
                ? player.Stats.MagicDamageBonus
                : player.Stats.PhysicalDamageBonus;
        }
        if (_currentWeapon.weaponType == WeaponType.Melee)
        {
            SfxManager.Instance?.PlaySiu();
            if (slashEffectPrefab != null)
            {
                Instantiate(slashEffectPrefab, _currentAttackPosition.position, Quaternion.identity);
            }
            if (CanHitTarget())
            {
                IDamageable target = GetTarget();
                if (target != null) target.TakeDamage(damage, isCritical);
            }
        }
        else if (_currentWeapon.weaponType == WeaponType.Ranged)
        {
            if (_currentWeapon.projectilePrefab == null) yield break;
            FireProjectile(damage);
        }
        else
        {
            if (_currentWeapon.projectilePrefab == null) yield break;
            if (_playerMana == null || _playerMana.CurrentMana < _currentWeapon.requiredMana) yield break;
            FireProjectile(damage);
            _playerMana.UseMana(_currentWeapon.requiredMana);
        }
        AttackPerformed?.Invoke();
        _playerAnimations.SetAttackAnimation(true);
        yield return new WaitForSeconds(0.5f);
        _playerAnimations.SetAttackAnimation(false);
        _attackCoroutine = null;
    }

    private IDamageable GetTarget()
    {
        if (_enemyTarget == null) return null;
        IDamageable target = _enemyTarget.GetComponent<IDamageable>();
        if (target == null || target.IsDead) return null;
        return target;
    }

    private bool CanHitTarget()
    {
        if (GetTarget() == null) return false;
        if (Vector2.Distance(transform.position, _enemyTarget.transform.position) > meleeRange) return false;
        Vector2 toTarget = (Vector2)_enemyTarget.transform.position - (Vector2)transform.position;
        if (toTarget.sqrMagnitude <= 0.0001f) return true;
        return Vector2.Dot(GetFacingDirection(), toTarget.normalized) > 0.5f;
    }

    private void FireProjectile(float damage)
    {
        SfxManager.Instance?.PlayBow();
        Projectile projectile = Instantiate(_currentWeapon.projectilePrefab,
            _currentAttackPosition.position, Quaternion.identity);
        projectile.Direction = GetFacingDirection();
        projectile.Damage = damage;
    }

    private Vector2 GetFacingDirection()
    {
        Vector2 facing = Vector2.zero;
        if (_currentAttackPosition != null)
        {
            facing = (Vector2)_currentAttackPosition.position - (Vector2)transform.position;
        }
        if (facing.sqrMagnitude <= 0.0001f) facing = _lastAttackDirection;
        if (facing.sqrMagnitude <= 0.0001f) return Vector2.down;
        return Mathf.Abs(facing.x) >= Mathf.Abs(facing.y)
            ? new Vector2(Mathf.Sign(facing.x), 0f)
            : new Vector2(0f, Mathf.Sign(facing.y));
    }

    private void GetFirePosition()
    {
        if (_playerMovement == null || attackPositions == null || attackPositions.Length < 4) return;
        Vector2 moveDirection = _playerMovement.MoveDirection;
        if (moveDirection == Vector2.zero) return;
        _lastAttackDirection = moveDirection;
        switch (moveDirection.x)
        {
            case > 0f:
                _currentAttackPosition = attackPositions[1];
                break;
            case < 0f:
                _currentAttackPosition = attackPositions[3];
                break;
        }
        switch (moveDirection.y)
        {
            case > 0f:
                _currentAttackPosition = attackPositions[0];
                break;
            case < 0f:
                _currentAttackPosition = attackPositions[2];
                break;
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (weapon != null) _currentWeapon = weapon;
    }

    public void UnequipWeapon()
    {
        _currentWeapon = null;
    }
    
    private void EnemySelectedCallback(EnemyBrain enemySelected)
    {
        _enemyTarget = enemySelected;
    }

    private void NoEnemySelectionCallback()
    {
        _enemyTarget = null;
    }
    private void OnEnable()
    {
        if (_actions == null) _actions = new PlayerActions();
        _actions.Enable();
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoEnemySelectionCallback;
    }

    private void OnDisable()
    {
        StopAttack();
        if (_actions != null) _actions.Disable();
        SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent -= NoEnemySelectionCallback;
    }
}
