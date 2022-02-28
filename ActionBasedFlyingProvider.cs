using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to fly their rig
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [AddComponentMenu("XR/Locomotion/Flying Provider (Action-based)", 11)]
    public class ActionBasedFlyingProvider : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Input System Action that will be used to read Thruster data from the controller. Must be a Value Vector3 Control.")]
        List<InputActionProperty> m_ThrusterActions;

        [SerializeField]
        public Rigidbody m_Superman;

        [SerializeField]
        public GameObject m_Controller;

        protected void OnEnable()
        {
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                thrusterAction.EnableDirectAction();

                thrusterAction.action.started += ThrusterStart;
                thrusterAction.action.performed += ThrusterUpdate;
                thrusterAction.action.canceled += ThrusterEnd;
            }
        }

        protected void OnDisable()
        {
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                thrusterAction.action.started -= ThrusterStart;
                thrusterAction.action.performed -= ThrusterUpdate;
                thrusterAction.action.canceled -= ThrusterEnd;

                thrusterAction.DisableDirectAction();
            }
        }

        private void ThrusterStart(InputAction.CallbackContext obj)
        {
            Debug.Log("ThrusterStart");
        }

        private void ThrusterUpdate(InputAction.CallbackContext obj)
        {
            var value = ReadValue();
            Debug.Log("ThrusterUpdate: " + value);
            value = new Vector3 { x = 0, y = 10, z = 0 };
            m_Superman.AddForce(value);
            
        }

        private void ThrusterEnd(InputAction.CallbackContext obj)
        {
            Debug.Log("ThrusterEnd");
        }

        protected Vector3 ReadValue()
        {
            Vector3 r = Vector3.zero;
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                // r = thrusterAction.action?.ReadValue<Vector3>() ?? r;
                r = m_Controller.transform.forward;
                Debug.Log(r);
                r = r * 40;
            }
            return r;
        }
    }
}
