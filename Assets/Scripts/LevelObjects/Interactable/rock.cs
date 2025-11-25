using System.Collections;
using Save;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelObjects.Interactable
{
    public class rock : Interactable
    {
        private Collider2D _collider;
        private SpriteRenderer _sprite;
        
        private Animator _poofAnimator;
        private List<SpriteRenderer> _poofRenderers = new List<SpriteRenderer>();
        private const string poofClipName = "DustPoof";
        private const string poofTriggerName = "Poof";

        private bool _used;
        private bool _poofPlayed;
        private bool _poofAnimatorOriginallyEnabled = false;

        void Start()
        {
            Debug.Log($"[{name}] rock.Start() called");

            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _poofAnimator = GetComponentInChildren<Animator>();

            if (_poofAnimator != null)
            {
                _poofAnimatorOriginallyEnabled = _poofAnimator.enabled;
                _poofAnimator.enabled = false;
                Debug.Log($"[{name}] Found Animator (controller assigned={_poofAnimator.runtimeAnimatorController != null}). Disabled Animator to prevent auto-first-frame draw.");
                
                var allPoofRenderers = _poofAnimator.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var r in allPoofRenderers)
                {
                    if (r == _sprite) continue;
                    _poofRenderers.Add(r);
                }
                
                foreach (var r in _poofRenderers)
                {
                    r.enabled = false;
                }
                
                var controller = _poofAnimator.runtimeAnimatorController;
                if (controller != null && controller.animationClips != null && controller.animationClips.Length > 0)
                {
                    bool found = false;
                    foreach (var c in controller.animationClips)
                    {
                        Debug.Log($"[{name}] Animator clip: {c.name} (length={c.length})");
                        if (string.Equals(c.name, poofClipName, StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Log($"[{name}] Found poof clip '{poofClipName}' with length {c.length}");
                            found = true;
                        }
                    }
                    if (!found)
                        Debug.Log($"[{name}] Poof clip '{poofClipName}' not found in Animator controller.");
                }
                else
                {
                    Debug.Log($"[{name}] Animator has no controller or no clips.");
                }
            }
            else
            {
                Debug.Log($"[{name}] No Animator found on rock or its children.");
            }
            
            bool active = _sprite != null && _sprite.enabled && _collider != null && _collider.enabled;
            _used = !active;
            Debug.Log($"[{name}] Initialized. active={active}, used={_used}");
        }

        public override void Interact()
        {
            Debug.Log($"[{name}] Interact() called. used={_used}");

            if (_used) return;
            _used = true;

            if (_collider != null) _collider.enabled = false;
            
            Debug.Log($"[{name}] Interact stack trace:\n" + Environment.StackTrace);

            if (_poofAnimator != null && !_poofPlayed)
            {
                _poofPlayed = true;
                
                foreach (var r in _poofRenderers)
                    r.enabled = true;
                
                _poofAnimator.enabled = true;
                
                if (HasAnimatorTrigger(_poofAnimator, poofTriggerName))
                {
                    Debug.Log($"[{name}] Triggering animator trigger '{poofTriggerName}'");
                    _poofAnimator.SetTrigger(poofTriggerName);
                }
                else
                {
                    Debug.Log($"[{name}] Playing animator state '{poofClipName}'");
                    _poofAnimator.Play(poofClipName, -1, 0f);
                }

                StartCoroutine(DisableAfterPoofFromAnimator());
                return;
            }
            
            if (_sprite != null)
            {
                _sprite.enabled = false;
                Debug.Log($"[{name}] No animator found or already played; sprite disabled immediately.");
            }
        }

        private bool HasAnimatorTrigger(Animator animator, string trigger)
        {
            if (animator == null) return false;
            foreach (var p in animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger && p.name == trigger)
                    return true;
            }
            return false;
        }

        private IEnumerator DisableAfterPoofFromAnimator()
        {
            float duration = 0f;
            var controller = _poofAnimator != null ? _poofAnimator.runtimeAnimatorController : null;
            if (controller != null && controller.animationClips != null && controller.animationClips.Length > 0)
            {
                foreach (var c in controller.animationClips)
                {
                    if (string.Equals(c.name, poofClipName, StringComparison.OrdinalIgnoreCase))
                    {
                        duration = c.length;
                        break;
                    }
                }
                
                if (duration <= 0f)
                    duration = controller.animationClips[0].length;
            }

            Debug.Log($"[{name}] Poof duration determined: {duration} seconds. Waiting...");

            if (duration > 0f)
                yield return new WaitForSeconds(duration);
            else
                yield return null;

            if (_sprite != null) _sprite.enabled = false;
            Debug.Log($"[{name}] Poof finished; sprite disabled.");
            
            foreach (var r in _poofRenderers)
                r.enabled = false;

            if (_poofAnimator != null)
            {
                _poofAnimator.enabled = false;
                Debug.Log($"[{name}] Animator disabled after poof.");
            }
        }

        public override InteractableObjectState SaveState()
        {
            return new InteractableObjectState
            {
                uniqueId = gameObject.name,
                isActive = _sprite != null && _sprite.enabled && _collider != null && _collider.enabled,
                position = transform.position,
                rotation = transform.rotation
            };
        }

        public override void LoadState(InteractableObjectState state)
        {
            if (state == null) return;

            transform.position = state.position;
            transform.rotation = state.rotation;

            bool active = state.isActive;
            if (_sprite != null) _sprite.enabled = active;
            if (_collider != null) _collider.enabled = active;
            _used = !active;

            Debug.Log($"[{name}] LoadState completed. active={active}, used={_used}");
        }
    }
}
