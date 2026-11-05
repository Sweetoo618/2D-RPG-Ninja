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
        if (_currentWeapon.weaponType == WeaponType.Magic)
        {
            if (_currentAttackPosition == null || _currentWeapon.projectilePrefab == null) yield break;
            if (_playerMana == null || _playerMana.CurrentMana < _currentWeapon.requiredMana) yield break;
            Projectile projectile = Instantiate(_currentWeapon.projectilePrefab,
                _currentAttackPosition.position, Quaternion.identity);
            projectile.Direction = _lastAttackDirection;
            projectile.Damage = damage;
            _playerMana.UseMana(_currentWeapon.requiredMana);
        }
        else
        {
            if (_enemyTarget == null) yield break;
            EnemyHealth targetHealth = _enemyTarget.GetComponent<EnemyHealth>();
            if (targetHealth == null || targetHealth.CurrentHealth <= 0f) yield break;
            if (Vector2.Distance(transform.position, _enemyTarget.transform.position) > meleeRange) yield break;
            targetHealth.TakeDamage(damage, isCritical);
        }
        AttackPerformed?.Invoke();
        _playerAnimations.SetAttackAnimation(true);
        yield return new WaitForSeconds(0.5f);
        _playerAnimations.SetAttackAnimation(false);
        _attackCoroutine = null;
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
