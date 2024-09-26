using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using System.Collections.Generic;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to fly vertically with their rig
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [AddComponentMenu("XR/Locomotion/Thruster Provider", 11)]
    public class ActionBasedThrusterProvider : MonoBehaviour
    {
        public enum thrusterStyle
        {
            normal,
            superman,
            ironman
        }

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read Thruster data from the controller. Must be a Value Touch Control.")]
        InputActionReference m_ThrusterAction;

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read rotation data from the controller. Must be a Value Quaternion.")]
        InputActionReference m_ThrusterDirectionAction;

        private GameObject m_ThrusterDirection;

        [SerializeField]
        [Tooltip("The Input System Action that will be used to read button data from the controller. Must be a Button Control.")]
        InputActionReference m_ThrusterStyleAction;

        [SerializeField]
        public thrusterStyle m_ThrusterStyle = thrusterStyle.normal;

        [SerializeField]
        public float m_ThrusterMultiplier = 1.0f;

        [SerializeField]
        public XROrigin m_XROrigin;

        private float m_ThrusterInternalMultiplier = 20.0f;
        private ConstantForce m_Thruster;


        protected void OnEnable()
        {
            m_ThrusterAction.action.performed += ThrusterUpdate;
            m_ThrusterAction.action.canceled += ThrusterEnd;

            m_ThrusterDirectionAction.action.performed += ThrusterDirectionUpdate;
            m_ThrusterDirectionAction.action.performed += ThrusterUpdate;
            if (m_ThrusterDirection == null) m_ThrusterDirection = new GameObject();

            if (m_ThrusterStyleAction) m_ThrusterStyleAction.action.started += ThrusterStyleStart;
 
            if (m_Thruster == null) m_Thruster = gameObject.AddComponent<ConstantForce>();
        }

        protected void OnDisable()
        {
            m_ThrusterAction.action.performed -= ThrusterUpdate;
            m_ThrusterAction.action.canceled -= ThrusterEnd;

            m_ThrusterDirectionAction.action.performed -= ThrusterUpdate;
            m_ThrusterDirectionAction.action.performed -= ThrusterDirectionUpdate;

            if (m_ThrusterStyleAction) m_ThrusterStyleAction.action.started -= ThrusterStyleStart;
        }

        protected void Awake()
        {
            if (m_XROrigin == null)
                m_XROrigin = FindFirstObjectByType<XROrigin>();
        }

        private void ThrusterUpdate(InputAction.CallbackContext obj)
        {
            Vector3 direction = m_ThrusterDirection.transform.up;
            switch (m_ThrusterStyle)
            {
                case thrusterStyle.normal:
                    direction = m_XROrigin.transform.rotation * m_ThrusterDirection.transform.up;
                    break;
                case thrusterStyle.superman:
                    direction = m_XROrigin.transform.rotation * m_ThrusterDirection.transform.forward;
                    break;
                case thrusterStyle.ironman:
                    direction = m_XROrigin.transform.rotation * m_ThrusterDirection.transform.forward * -1;
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
            return m_ThrusterDirectionAction.action?.ReadValue<Quaternion>() ?? Quaternion.identity;
        }

        protected float ReadValue()
        {
            return m_ThrusterAction.action?.ReadValue<float>() ?? 0.0f;
        }
    }
}
