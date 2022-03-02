using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to fly vertically with their rig
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [AddComponentMenu("XR/Locomotion/Thruster Provider (Action-based)", 11)]
    public class ActionBasedThrusterProvider : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Input System Action that will be used to read Thruster data from the controller. Must be a Value Touch Control.")]
        List<InputActionProperty> m_ThrusterActions;

        [SerializeField]
        public float m_ThrusterMultipier = 10.0f;

        private ConstantForce m_Thruster;

        protected void OnEnable()
        {
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                thrusterAction.EnableDirectAction();

                thrusterAction.action.started += ThrusterStart;
                thrusterAction.action.performed += ThrusterUpdate;
                thrusterAction.action.canceled += ThrusterEnd;
            }
            if (m_Thruster == null) m_Thruster = gameObject.AddComponent<ConstantForce>();
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
        }

        private void ThrusterUpdate(InputAction.CallbackContext obj)
        {
            var value = m_ThrusterMultipier * Mathf.Sqrt(ReadValue());
            m_Thruster.force = new Vector3(0.0f, value, 0.0f);
        }

        private void ThrusterEnd(InputAction.CallbackContext obj)
        {
            m_Thruster.force = Vector3.zero;
        }

        protected float ReadValue()
        {
            float r = 0.0f;
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                float v = thrusterAction.action?.ReadValue<float>() ?? r;
                if (v != 0.0f)
                {
                    r = v;
                }
            }
            return r;
        }
    }
}
