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
        public enum thrusterStyle
        {
            normal,
            superman,
            ironman
        }

        [SerializeField]
        public GameObject m_Player;

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read Thruster data from the controller. Must be a Value Touch Control.")]
        List<InputActionProperty> m_ThrusterActions;

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read rotation data from the controller. Must be a Value Quaternion.")]
        InputActionProperty m_ThrusterDirectionAction;

        private GameObject m_ThrusterDirection;

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read button data from the controller. Must be a Button Control.")]
        List<InputActionProperty> m_ThrusterStyleActions;

        [SerializeField]
        public thrusterStyle m_ThrusterStyle = thrusterStyle.normal;

        [SerializeField]
        public float m_ThrusterMultiplier = 1.0f;

        private float m_ThrusterInternalMultiplier = 20.0f;
        private ConstantForce m_Thruster;


        protected void OnEnable()
        {
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                thrusterAction.EnableDirectAction();

                thrusterAction.action.performed += ThrusterUpdate;
                thrusterAction.action.canceled += ThrusterEnd;
            }

            m_ThrusterDirectionAction.EnableDirectAction();
            m_ThrusterDirectionAction.action.performed += ThrusterDirectionUpdate;
            m_ThrusterDirectionAction.action.performed += ThrusterUpdate;
            if (m_ThrusterDirection == null) m_ThrusterDirection = new GameObject();

            foreach (InputActionProperty thrusterStyleAction in m_ThrusterStyleActions)
            {
                thrusterStyleAction.EnableDirectAction();

                thrusterStyleAction.action.started += ThrusterStyleStart;
            }

            if (m_Thruster == null) m_Thruster = gameObject.AddComponent<ConstantForce>();
        }

        protected void OnDisable()
        {
            foreach (InputActionProperty thrusterAction in m_ThrusterActions)
            {
                thrusterAction.action.performed -= ThrusterUpdate;
                thrusterAction.action.canceled -= ThrusterEnd;

                thrusterAction.DisableDirectAction();
            }

            m_ThrusterDirectionAction.action.performed -= ThrusterUpdate;
            m_ThrusterDirectionAction.action.performed -= ThrusterDirectionUpdate;
            m_ThrusterDirectionAction.DisableDirectAction();

            foreach (InputActionProperty thrusterStyleAction in m_ThrusterStyleActions)
            {
                thrusterStyleAction.EnableDirectAction();

                thrusterStyleAction.action.started -= ThrusterStyleStart;
            }
        }

        private void ThrusterUpdate(InputAction.CallbackContext obj)
        {
            Vector3 direction = m_ThrusterDirection.transform.up;
            switch (m_ThrusterStyle) {
                case thrusterStyle.normal:
                    direction = m_Player.transform.rotation * m_ThrusterDirection.transform.up;
                    break;
                case thrusterStyle.superman:
                    direction = m_Player.transform.rotation * m_ThrusterDirection.transform.forward;
                    break;
                case thrusterStyle.ironman:
                    direction = m_Player.transform.rotation * m_ThrusterDirection.transform.forward * -1;
                    break;
            }
            m_Thruster.force = direction * m_ThrusterInternalMultiplier * m_ThrusterMultiplier * Mathf.Sqrt(ReadValue());
        }

        private void ThrusterEnd(InputAction.CallbackContext obj)
        {
            m_Thruster.force = Vector3.zero;
        }

        private void ThrusterDirectionUpdate(InputAction.CallbackContext obj)
        {
            m_ThrusterDirection.transform.rotation = ReadDirectionValue();
        }

        private void ThrusterStyleStart(InputAction.CallbackContext obj)
        {
            switch (m_ThrusterStyle)
            {
                case thrusterStyle.normal:
                    m_ThrusterStyle = thrusterStyle.superman;
                    break;
                case thrusterStyle.superman:
                    m_ThrusterStyle = thrusterStyle.ironman;
                    break;
                case thrusterStyle.ironman:
                    m_ThrusterStyle = thrusterStyle.normal;
                    break;
            }
        }

        protected Quaternion ReadDirectionValue()
        {
            return  m_ThrusterDirectionAction.action?.ReadValue<Quaternion>() ?? Quaternion.identity;
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
