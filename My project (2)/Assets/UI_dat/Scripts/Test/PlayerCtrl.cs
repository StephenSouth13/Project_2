using System;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private PlayerModel _playerModel;
    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        _playerModel = new PlayerModel(100f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        var movement = _playerModel.CalculateMovement(horizontalInput,
            verticalInput, moveSpeed, Time.deltaTime);
        transform.position += movement;
        
        if(_playerModel.IsDead)
        {
            Die();
        }
    }
    
    public void TakeDamage(float damage)
    {
        _playerModel.TakeDamage(damage);
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
}