using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to teleport their rig
    /// using a specified input action.
    /// </summary>
    /// <seealso cref="LocomotionProvider"/>
    [AddComponentMenu("XR/Locomotion/Teleportation Provider (Action-based)", 11)]
    public class ActionBasedTeleportationProvider : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Input System Action that will be used to read Teleport data from the controller. Must be a Value Vector2 Control.")]
        List<InputActionReference> m_TeleportActions;


        [SerializeField]
        public XRInteractorLineVisual m_RayVisual;

        [SerializeField]
        public XRRayInteractor m_RayInteractor;
        
        private TeleportationProvider m_TeleportationProvider;

        private bool m_ReadyToTeleport = false;
        private float m_OriginalLineWidth;

        private float m_ViewThreshold = 0.0f;
        private float m_ActivateThreshold = 1.0f;

        protected void OnEnable()
        {
            m_OriginalLineWidth = m_RayVisual?.lineWidth ?? 0.0f;

            m_ViewThreshold = 0.0f;
            bool updatedViewThreshold = false;
            m_ActivateThreshold = 1.0f;
            bool updatedActivateThreshold = false;

            foreach (InputActionReference teleportAction in m_TeleportActions)
            {
                // Supports Vector2 Actions with an optional shared pair of SectorInteraction pressPoints
                foreach (InputBinding inputBinding in teleportAction.action.bindings)
                {
                    foreach (SectorInteraction interaction in ParseSectorInteractions(inputBinding.interactions))
                    {
                        if (!updatedViewThreshold)
                        {
                            m_ViewThreshold = 1.0f;
                            updatedViewThreshold = true;
                        }
                        if (!updatedActivateThreshold)
                        {
                            m_ActivateThreshold = 0.0f;
                            updatedActivateThreshold = true;
                        }
                        // lowest value of pressPoint is view threshold
                        if (interaction.pressPoint < m_ViewThreshold) m_ViewThreshold = interaction.pressPoint;
                        // highest value of pressPoint is activate threshold
                        if (interaction.pressPoint > m_ActivateThreshold) m_ActivateThreshold = interaction.pressPoint;
                    }
                }
                Debug.Log($"{m_ViewThreshold} {m_ActivateThreshold}");
                //teleportAction.EnableDirectAction();

                teleportAction.action.started += TeleportStart;
                teleportAction.action.performed += TeleportUpdate;
                teleportAction.action.canceled += TeleportEnd;
            }

            if (m_TeleportationProvider == null) m_TeleportationProvider = gameObject.AddComponent<TeleportationProvider>();
        }

        protected void OnDisable()
        {
            if (m_RayVisual != null) m_RayVisual.lineWidth = m_OriginalLineWidth;

            foreach (InputActionReference teleportAction in m_TeleportActions)
            {
                teleportAction.action.started -= TeleportStart;
                teleportAction.action.performed -= TeleportUpdate;
                teleportAction.action.canceled -= TeleportEnd;

                //teleportAction.DisableDirectAction();
            }
        }

        private void TeleportStart(InputAction.CallbackContext obj)
        {
            Debug.Log("TeleportStart");
            m_ReadyToTeleport = false;
            m_RayInteractor.enabled = true;
        }

        private void TeleportUpdate(InputAction.CallbackContext obj)
        {
            var value = ReadValue(obj);
            Debug.Log($"TeleportUpdate {value} {obj.interaction}");
            if (!m_ReadyToTeleport && value < m_ActivateThreshold)
            {
                if (m_RayVisual != null) m_RayVisual.lineWidth = (1.0f - ((m_ActivateThreshold - value) / (m_ActivateThreshold - m_ViewThreshold))) * m_OriginalLineWidth;
            }
            else if (!m_ReadyToTeleport && value >= m_ActivateThreshold)
            {
                m_ReadyToTeleport = true;
                if (m_RayVisual != null) m_RayVisual.lineWidth = m_OriginalLineWidth;
            }
        }

        private void TeleportEnd(InputAction.CallbackContext obj)
        {
            var value = ReadValue(obj);
            Debug.Log($"TeleportEnd {value} {obj.interaction}");
            if (m_ReadyToTeleport)
            {
                Vector3 position;
                bool isValid;
                if (m_RayInteractor.TryGetHitInfo(out position, out _, out _ ,out isValid))
                {
                    if (isValid) m_TeleportationProvider.QueueTeleportRequest(new TeleportRequest { destinationPosition = position });
                }
            }
            m_RayInteractor.enabled = false;
        }

        protected float ReadValue(InputAction.CallbackContext obj)
        {
            var value = (obj.action?.ReadValue<Vector2>() ?? Vector2.zero);
            Debug.Log($"ReadValue {value}");
            float r = value.x;
            if (value.y != 0.0f)
                r = value.y;
            /*
            float r = 0.0f;
            foreach (InputActionProperty teleportAction in m_TeleportActions)
            {
                var y = (teleportAction.action?.ReadValue<Vector2>() ?? Vector2.zero).y;
                if (y < 0.0f)
                {
                    y = 0.0f;
                }
                r += y;
            }*/
            return r;
        }

        // Look for SectorInteractions in a string
        protected List<SectorInteraction> ParseSectorInteractions(string interactions)
        {
            var ret = new List<SectorInteraction>();

            Regex rSector = new Regex(@"Sector\([^)]*\)", RegexOptions.IgnoreCase);
            Regex rPressPoint = new Regex(@"pressPoint=[0-9-.]*", RegexOptions.IgnoreCase);

            float pressPoint;

            foreach (Match m in rSector.Matches(interactions))
            {
                pressPoint = SectorInteraction.defaultPressPoint;
                Match mPresspoint = rPressPoint.Match(m.Value);
                if (mPresspoint.Success)
                {
                    pressPoint = float.Parse(mPresspoint.Value.Split("=")[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                var s = new SectorInteraction();
                s.pressPoint = pressPoint;
                ret.Add(s);
            }
            return ret;
        }
    }
}
