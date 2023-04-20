using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CS.Scripts.Utils;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Timer = CS.Scripts.Utils.Timer;

namespace Game
{
    public class Player : MonoBehaviour
    {
        private const float FallTime = 0.1f;

        [Header("General")]
        [SerializeField] private int initialCubes;

        [SerializeField] private float speed;
        [SerializeField] private float width;
        [SerializeField] private float cubeHeight;
        [SerializeField] private float pathOffset;
        [SerializeField] private float sensitivity;

        [Header("Ground checking")] 
        [SerializeField] private Vector3 boxOffset;
        [SerializeField] private Vector3 boxSize;
        
        [Space]

        // Для внедрения зависимостей в класс в рамках тестового задания я выбрал обычную сериализацию полей. 
        // Для серьёзного проекта или даже прототипа я бы больше потрудился в выборе инструмента. Например, Zenject или кастомная утилита
        [Header("References")]
        [SerializeField] private PathCreator path;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerCube cubePrefab;

        private readonly List<PlayerCube> _cubes = new List<PlayerCube>();
        private readonly Timer _fallTimer = new Timer(FallTime);

        private float _currentDistance = 0f;
        private float _currentOffset = 0f;

        private bool _isMoving = true;

        public Vector3 GetPosition(PlayerCube cube)
        {
            var index = _cubes.Contains(cube) ? _cubes.IndexOf(cube) : _cubes.Count;
            
            var result = transform.position + Vector3.up * (pathOffset + index * cubeHeight);
            return result;
        }

        private void AddCube() => AddCube(true);
        private void AddCube(bool shootSound)
        {
            var cube = Instantiate(cubePrefab, transform);
            cube.SetTarget(this);
            
            cube.transform.position = transform.position + Vector3.up * (pathOffset + _cubes.Count * cubeHeight);
            _cubes.Add(cube);

            if (shootSound)
            {
                SoundService.Instance.Shoot(ESound.AddCube);
            }

            cube.OnCubeDestroy += Subscribe;

            void Subscribe()
            {
                cube.OnCubeDestroy -= Subscribe;
                
                RemoveCube(cube);
            }
        }

        private void RemoveCube(PlayerCube cube)
        {
            if (cube == _cubes.First())
            {
                if (_cubes.Count > 1)
                {
                    _cubes[1].OnCollideCube += AddCube;
                }
            }

            _cubes.Remove(cube);
            
            if (!_cubes.Any())
            {
                SceneManager.LoadSceneAsync("CS/Scenes/Game", LoadSceneMode.Single);
            }
        }

        private void Fall()
        {
            _isMoving = false;
            rb.isKinematic = false;
            rb.useGravity = true;

            StartCoroutine(RestartCoroutine());

            IEnumerator RestartCoroutine()
            {
                const float delay = 1.5f;

                yield return new WaitForSeconds(delay);
                SceneManager.LoadSceneAsync("CS/Scenes/Game", LoadSceneMode.Single);
            }
        }
        private void Awake()
        {
            for (var i = 0; i < initialCubes; i++)
            {
                AddCube(false);
            }

            var cube = _cubes.First();
            cube.OnCollideCube += AddCube;

            _fallTimer.OnTimerOver += Fall;
        }

        private bool IsGrounded()
        {
            var colliders = Physics.OverlapBox(transform.position + boxOffset, boxSize / 2, transform.rotation);

            var result = colliders.Any(value => value.CompareTag("Ground"));
            return result;
        }

        private void Update()
        {
            if (!_isMoving) return;

            if (!IsGrounded())
            {
                _fallTimer.Update(Time.deltaTime);
            }
            else
            {
                _fallTimer.Reset();
            }

            if (Input.GetMouseButton(0))
            {
                _currentOffset += Input.GetAxis("Mouse X") * sensitivity;
                _currentOffset = Mathf.Clamp(_currentOffset, -width / 2, width / 2);
            }

            _currentDistance += speed * Time.deltaTime;
            if (_currentDistance >= path.path.length)
            {
                SceneManager.LoadSceneAsync("CS/Scenes/Game", LoadSceneMode.Single);
            }

            var realDistance = path.path.length - _currentDistance;

            var direction = path.path.GetDirectionAtDistance(realDistance);
            direction.y = 0;

            transform.forward = -direction;
            transform.position = path.path.GetPointAtDistance(realDistance) + Vector3.up * pathOffset + transform.right * _currentOffset;
        }
    }
}