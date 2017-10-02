﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour {

    private enum DogState
    {
        IDLE,
        FOLLOW,
        CHASE,
        ATTACK
    }

    private float _upp;
    [SerializeField]
    private float _dogSpeed = 120;
    [SerializeField]
    private float _idleDistToPlayer = 60;
    [SerializeField]
    private float _chaseDistToTarget = 50;
    private DogState _state;

    [SerializeField]
    private float _hitDisplacement = 5;
    private bool _hit = false;

    private SpriteRenderer _renderer;

    [SerializeField]
    private Transform _player;
    private GameObject _targetEnemy;

	void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _state = DogState.IDLE;
    }

	void Start () {
        PixelsToUnits();
	}

    private void PixelsToUnits()
    {
        _upp = GameManager.Instance.Config.UPP;
        _dogSpeed = _dogSpeed * _upp;
        _idleDistToPlayer = _idleDistToPlayer * _upp;
        _chaseDistToTarget = _chaseDistToTarget * _upp;
    }

    void Update()
    {
        switch(_state)
        {
            case DogState.IDLE:
                if (StartChasingEnemy())
                    _state = DogState.CHASE;
                else 
                {
                    if (!IsNearPlayer())
                        _state = DogState.FOLLOW;
                    else
                        Idle();
                }
                break;
            case DogState.FOLLOW:
                if (StartChasingEnemy())
                    _state = DogState.CHASE;
                else 
                {
                    if (IsNearPlayer())
                        _state = DogState.IDLE;
                    else
                        Follow();
                }
                break;
            case DogState.CHASE:
                if (IsNearTarget())
                {
                    Chase();
                }
                else
                {
                    _state = DogState.IDLE;
                }
                    
                break;
            case DogState.ATTACK:
                Attack();
                break;
        }
        _renderer.sortingOrder = -(int)transform.position.y;
    }

    private void Idle()
    {
    }

    private void Follow()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        Vector3 movement = direction * _dogSpeed * Time.deltaTime;
        transform.position += movement;

        if (direction.x > 0)
            _renderer.flipX = false;
        else
            _renderer.flipX = true;
    }

    private void Chase()
    {
        Vector2 direction = (_targetEnemy.transform.position - transform.position).normalized;
        Vector3 movement = direction * _dogSpeed * Time.deltaTime;
        transform.position += movement;

        if (direction.x > 0)
            _renderer.flipX = false;
        else
            _renderer.flipX = true;
        
    }

    private void Attack()
    {
        _state = DogState.IDLE;
    }

    private bool IsNearPlayer()
    {
        Vector3 dist = _player.position - transform.position;
        
        return dist.sqrMagnitude < _idleDistToPlayer * _idleDistToPlayer;
    }

    private bool IsNearTarget()
    {
        if (!_targetEnemy.gameObject.activeInHierarchy)
            return false;

        return Vector3.Distance(_targetEnemy.transform.position, transform.position) < _chaseDistToTarget + 1;
    }

    private bool StartChasingEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, _chaseDistToTarget, 1 << LayerMask.NameToLayer("Enemy"));
        if (cols.Length > 0)
        {
            int idx = Random.Range(0, cols.Length);
            _targetEnemy = cols[idx].gameObject;

            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (_state == DogState.CHASE && col.gameObject.tag == "Enemy")
        {
            if (_state == DogState.CHASE && _targetEnemy == col.gameObject)
            {
                Debug.Log("ATTACK");
                _state = DogState.ATTACK;
            }
            else
            {
                StartCoroutine(Hit(col.contacts[0].normal));
            }
        }
    }

    IEnumerator Hit(Vector3 direction)
    {
        float displacement = 0.0f;

        _hit = true;
        while (displacement < _hitDisplacement)
        {
            Vector3 movement = direction * (_dogSpeed * 5) * Time.deltaTime;
            transform.position += movement;
            displacement += movement.magnitude;
            yield return null;
        }
        _hit = false;

    }
}