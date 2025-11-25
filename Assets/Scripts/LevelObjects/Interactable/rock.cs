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

        // runtime-only references (not serialized)
        private Animator _poofAnimator;
        private List<SpriteRenderer> _poofRenderers = new List<SpriteRenderer>();
        private const string poofClipName = "DustPoof"; // DustPoof is an animation clip
        private const string poofTriggerName = "Poof";

        private bool _used;
        private bool _poofPlayed;
        private bool _poofAnimatorOriginallyEnabled = false;

        void Start()
        {
            Debug.Log($"[{name}] rock.Start() called");

            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();

            // Try to find an Animator in children or on the same GameObject
            _poofAnimator = GetComponentInChildren<Animator>();

            if (_poofAnimator != null)
            {
                // Record whether animator was enabled and then disable it so it won't apply the first-frame sprite
                _poofAnimatorOriginallyEnabled = _poofAnimator.enabled;
                _poofAnimator.enabled = false;
                Debug.Log($"[{name}] Found Animator (controller assigned={_poofAnimator.runtimeAnimatorController != null}). Disabled Animator to prevent auto-first-frame draw.");

                // Gather SpriteRenderers that belong to the poof animator (exclude this rock's main sprite)
                var allPoofRenderers = _poofAnimator.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var r in allPoofRenderers)
                {
                    if (r == _sprite) continue; // don't touch the rock's main sprite
                    _poofRenderers.Add(r);
                }

                // Disable those renderers so the poof's initial sprite doesn't show
                foreach (var r in _poofRenderers)
                {
                    r.enabled = false;
                }

                // Log clips for debugging
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

            // Initialize used based on current enabled state of the visual/collider
            bool active = _sprite != null && _sprite.enabled && _collider != null && _collider.enabled;
            _used = !active;
            Debug.Log($"[{name}] Initialized. active={active}, used={_used}");
        }

        public override void Interact()
        {
            Debug.Log($"[{name}] Interact() called. used={_used}");

            if (_used) return; // only one interaction
            _used = true;

            if (_collider != null) _collider.enabled = false;

            // Debug stack so we can trace unexpected callers
            Debug.Log($"[{name}] Interact stack trace:\n" + Environment.StackTrace);

            if (_poofAnimator != null && !_poofPlayed)
            {
                _poofPlayed = true;

                // Enable renderers that make up the poof visual
                foreach (var r in _poofRenderers)
                    r.enabled = true;

                // Enable the Animator so it can play from a fresh start
                _poofAnimator.enabled = true;

                // If the controller has a trigger named "Poof", use it. Otherwise try to Play the state named poofClipName.
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

            // No animator or already played — disable immediately
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
                // Prefer clip that matches the poofClipName if present
                foreach (var c in controller.animationClips)
                {
                    if (string.Equals(c.name, poofClipName, StringComparison.OrdinalIgnoreCase))
                    {
                        duration = c.length;
                        break;
                    }
                }

                // If we didn't find a matching clip, fall back to the first clip
                if (duration <= 0f)
                    duration = controller.animationClips[0].length;
            }

            Debug.Log($"[{name}] Poof duration determined: {duration} seconds. Waiting...");

            if (duration > 0f)
                yield return new WaitForSeconds(duration);
            else
                yield return null; // wait a frame so the animation can start

            if (_sprite != null) _sprite.enabled = false;
            Debug.Log($"[{name}] Poof finished; sprite disabled.");

            // Disable the poof visuals and animator again so the poof's first-frame doesn't appear later
            foreach (var r in _poofRenderers)
                r.enabled = false;

            if (_poofAnimator != null)
            {
                _poofAnimator.enabled = false;
                // restore original enabled state is unnecessary because we want it off after play
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
