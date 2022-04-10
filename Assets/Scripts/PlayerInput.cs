﻿using UnityEngine;

namespace Scripts
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;

        private void Update()
        {
            _player.rotation = Input.GetAxis("Horizontal");
            _player.burst = Input.GetAxis("Vertical");
            _player.shoot = Input.GetButton("Fire1");
        }
    }
}
