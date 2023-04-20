using System;
using CS.Scripts.Utils;
using UnityEngine;
using Utils;

namespace Game
{
    public class PlayerCube : MonoBehaviour
    {
        private const float FallSpeed = 10f;
        
        public event Action OnCubeDestroy = () => { };
        public event Action OnCollideCube = () => { };

        private Player _player;
        
        public void SetTarget(Player player)
        {
            _player = player;
        }

        private void Update()
        {
            var position = _player.GetPosition(this);
            var delta = position - transform.position;

            if (delta.magnitude < 0.1f) return;
            
            var value = delta.normalized * FallSpeed * Time.deltaTime;
            transform.position += value;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Cube"))
            {
                OnCollideCube.Invoke();
                Destroy(collision.gameObject);
            }
            else if (collision.collider.CompareTag("Wall"))
            {
                // Если мы находимся выше этого кубика, значит, он нам не преграда
                if (collision.collider.transform.position.y < transform.position.y - 0.5f) return;
                
                // Если этот кубик находится позади нас, значит мы его уже прошли
                if (transform.InverseTransformPoint(collision.collider.transform.position).z < 0) return;

                transform.SetParent(null);
                
                var rigidbody = GetComponent<Rigidbody>();
                Destroy(rigidbody);
            
                var colliders = GetComponents<Collider>();
                foreach (var collider in colliders)
                {
                    Destroy(collider);
                }
            
                OnCubeDestroy.Invoke();
                
                Destroy(this);
            }
            else if (collision.collider.CompareTag("Ground"))
            {
                // SoundService.Instance.Shoot(ESound.Hit);
            }
        }
    }
}