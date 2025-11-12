using UnityEngine;
            
            public class PresenceTrigger : MonoBehaviour
            {
                // Optional serialized field can be removed to avoid stale inspector links
                [SerializeField] private GameManager _gameManager;
            
                private void Awake()
                {
                    if (_gameManager == null)
                        _gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
                }
            
                private void OnEnable()
                {
                    if (_gameManager == null)
                        _gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
                }
            
                private void EnsureGameManager()
                {
                    if (_gameManager == null)
                        _gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
                }
            
                private void OnTriggerEnter(Collider other)
                {
                    EnsureGameManager();
                    if (_gameManager == null)
                    {
                        Debug.LogWarning("GameManager not found when presence triggered.");
                        return;
                    }
                    
                    _gameManager.GoToMainMenu();
                }
            
                private void OnTriggerExit(Collider other)
                {
                    EnsureGameManager();
                    if (_gameManager == null)
                        return;
            
                    // Example exit action
                    // _gameManager.SomeOtherMethod(...);
                }
            }