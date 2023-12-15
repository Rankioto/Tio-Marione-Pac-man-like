using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action OnPowerUpStart;
    public Action OnPowerUpStop;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _powerUpDuration;

    [SerializeField]
    private int _health;

    [SerializeField]
    private TMP_Text _healthText;

    [SerializeField]
    private Transform _respawnPoint;

    private Rigidbody _rigidBody;
    private Coroutine _powerUpCoroutine;
    private bool _isPowerUpActive = false;

    public void Dead()
    {
        _health -= 1;
        if (_health > 0 ) 
        {
            transform.position = _respawnPoint.position;
        }
        else
        {
            _health = 0;
            Debug.Log("Lose");
        }
        UpdateUI();
    }

    private void HideAndLockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PickPowerUp()
    {
        if (_powerUpCoroutine != null)
        {
            StopCoroutine(_powerUpCoroutine);
        }
        _powerUpCoroutine = StartCoroutine(StartPowerUp());
    }

    private IEnumerator StartPowerUp()
    {
        _isPowerUpActive = true;
        if (OnPowerUpStart != null)
        {
            OnPowerUpStart();
        }

        yield return new WaitForSeconds(_powerUpDuration);
        
        _isPowerUpActive = false;
        if (OnPowerUpStop != null)
        {
            OnPowerUpStop();
        }
    }

    private void Awake()
    {
        UpdateUI();
        _rigidBody = GetComponent<Rigidbody>();
        HideAndLockCursor();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 horizontalDirection = horizontal * _camera.transform.right;
        Vector3 verticalDirection =  vertical * _camera.transform.forward;
        verticalDirection.y = 0;
        horizontalDirection.y = 0;

        Vector3 movementDirection = horizontalDirection + verticalDirection;

        _rigidBody.velocity = movementDirection * _speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isPowerUpActive)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().Dead();
            }
        }
    }

    private void UpdateUI()
    {
        _healthText.text = "Health: " + _health;
    }
}
