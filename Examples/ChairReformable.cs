using UnityEngine.ProBuilder;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class ChairReformable : XRBaseInteractable
    {
        public float m_Width = 3.5f;
        public float m_Depth = 1.2f;
        public float m_LegWidth = 0.04f;
        public float m_LegHeight = 0.3f;
        public float m_LegTopMultiplier = 2;
        public float m_LegAngleCoef = 0.3f;
        public float m_LegInset = 0.1f;
        public float m_BaseHeight = 0.1f;
        public float m_SideHeight = 0.6f;
        public float m_SideWidth = 0.1f;
        public float m_BackHeight = 0.8f;
        public float m_BackDepth = 0.1f;
        public float m_CushionHeight = 0.2f;
        public float m_CushionBevel = 0.02f;
        public int m_CushionCount = 3;
        public float m_BackCushionDepth = 0.2f;

        public Material m_LegMaterial;
        public Material m_BaseMaterial;
        public Material m_SideMaterial;
        public Material m_BackMaterial;
        public Material m_CushionMaterial;
        public Material m_BackCushionMaterial;

        private bool m_FirstTime = true;
        private bool m_previouslySelected = false;

        private Reformable m_Object;
        private ProBuilderMesh m_TriggerMesh;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_TriggerMesh = GetComponent<ProBuilderMesh>();
            if (m_Object == null) m_Object = gameObject.AddComponent<Reformable>();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            switch (updatePhase)
            {
                // During Fixed update we want to perform any physics-based updates (e.g., Kinematic or VelocityTracking).
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    break;

                // During Dynamic update we want to perform any Transform-based manipulation (e.g., Instantaneous).
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (isSelected && !m_previouslySelected)
                    {
                        if (m_FirstTime)
                        {
                            m_FirstTime = false;

                            // Hide trigger object
                            m_TriggerMesh.Clear();
                            m_TriggerMesh.ToMesh();
                            m_TriggerMesh.Refresh();
                        }
                        else
                        {
                            m_Width = 3.5f * Random.Range(0.01f, 2);
                            m_Depth = 1.2f * Random.Range(0.01f, 2);
                            m_LegWidth = 0.04f * Random.Range(0.01f, 2);
                            m_LegHeight = 0.3f * Random.Range(0.01f, 2);
                            m_LegTopMultiplier = 2 * Random.Range(0.01f, 2);
                            m_LegAngleCoef = 0.3f * Random.Range(0.01f, 2);
                            m_LegInset = 0.1f * Random.Range(0.01f, 2);
                            m_BaseHeight = 0.1f * Random.Range(0.01f, 2);
                            m_SideHeight = 0.6f * Random.Range(0.01f, 2);
                            m_SideWidth = 0.1f * Random.Range(0.01f, 2);
                            m_BackHeight = 0.8f * Random.Range(0.01f, 2);
                            m_BackDepth = 0.1f * Random.Range(0.01f, 2);
                            m_CushionHeight = 0.2f * Random.Range(0.01f, 2);
                            m_CushionBevel = 0.02f * Random.Range(0.01f, 2);
                            m_CushionCount = (int)(Random.Range(1, 10));
                            m_BackCushionDepth = 0.2f * Random.Range(0.01f, 2); ;
                        }

                        m_previouslySelected = true;

                        m_Object.Clear();

                        m_Object.Join(
                            LegMesh(),
                            BaseMesh(),
                            SideMesh(),
                            BackMesh(),
                            CushionMesh(),
                            BackCushionMesh()
                            );
                    }
                    else if (!isSelected && m_previouslySelected)
                    {
                        m_previouslySelected = false;
                    }
                    break;

                // During OnBeforeRender we want to perform any last minute Transform position changes before rendering (e.g., Instantaneous).
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    break;

                // Late update is only used to handle detach as late as possible.
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    break;
            }
        }

        private Reformable LegMesh()
        {
            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = m_LegWidth, y = m_LegHeight, z = m_LegWidth });

            mesh.SetMaterial(m_LegMaterial);

            // Transform (set on ground)
            mesh.Transform(new Vector3 { x = 0, y = m_LegHeight / 2, z = 0 });

            // Set Position (coneify)
            mesh.SetPosition(
                delegate (Vector3 v) { return v.y > 0; },
                delegate (Vector3 v) { return new Vector3 { x = v.x * m_LegTopMultiplier, y = v.y, z = v.z * m_LegTopMultiplier }; });

            // Set Position (angle)
            mesh.SetPosition(
                delegate (Vector3 v) { return v.y > 0; },
                delegate (Vector3 v) { return new Vector3 { x = v.x + m_LegAngleCoef * m_LegHeight, y = v.y, z = v.z + m_LegAngleCoef * m_LegHeight }; });

            // Transform (inset)
            mesh.Transform(new Vector3 { x = m_LegInset, y = 0, z = m_LegInset });

            // Transform (move to corner)
            mesh.Transform(new Vector3 { x = -m_Width / 2, y = 0, z = -m_Depth / 2 });

            // Clone
            Reformable cloneMesh = mesh.Copy();

            // Transform (mirror x)
            cloneMesh.Transform(
                delegate (Vector3 v) { return new Vector3 { x = -v.x, y = v.y, z = v.z }; });

            // Flip normals
            cloneMesh.FlipFaces();

            // Join
            mesh.Join(cloneMesh);

            // Clone
            cloneMesh = mesh.Copy();

            // Transform (mirror z)
            cloneMesh.Transform(
                delegate (Vector3 v) { return new Vector3 { x = v.x, y = v.y, z = -v.z }; });

            // Flip normals
            cloneMesh.FlipFaces();

            // Join
            mesh.Join(cloneMesh);

            return mesh;
        }

        private Reformable BaseMesh()
        {
            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = m_Width, y = m_BaseHeight, z = m_Depth });

            mesh.SetMaterial(m_BaseMaterial);

            // Transform (set on legs)
            mesh.Transform(
                delegate (Vector3 v) { return new Vector3 { x = v.x, y = v.y + m_LegHeight + m_BaseHeight / 2, z = v.z }; });

            return mesh;
        }

        private Reformable SideMesh()
        {
            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = m_SideWidth, y = m_SideHeight, z = m_Depth - m_BackDepth });

            mesh.SetMaterial(m_SideMaterial);

            // Transform (set on base)
            mesh.Transform(new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_SideHeight / 2, z = 0 });

            // Transform (move to side, adjust for BackDepth)
            mesh.Transform(new Vector3 { x = -m_Width / 2 + m_SideWidth / 2, y = 0, z = -m_BackDepth / 2 });

            // Clone
            Reformable cloneMesh = mesh.Copy();

            // Transform (mirror x)
            mesh.Transform(
                delegate (Vector3 v) { return new Vector3 { x = -v.x, y = v.y, z = v.z }; });

            // Flip normals
            mesh.FlipFaces();

            // Join
            mesh.Join(cloneMesh);

            return mesh;
        }

        private Reformable BackMesh()
        {
            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = m_Width, y = m_BackHeight, z = m_BackDepth });

            mesh.SetMaterial(m_BackMaterial);

            // Transform (set on base)
            mesh.Transform(new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_BackHeight / 2, z = 0 });

            // Transform (move to back)
            mesh.Transform(new Vector3 { x = 0, y = 0, z = m_Depth / 2 - m_BackDepth / 2 });

            return mesh;
        }

        private Reformable CushionMesh()
        {
            float cushionWidth = ((m_Width - (m_SideWidth * 2)) / m_CushionCount);

            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = cushionWidth, y = m_CushionHeight, z = m_Depth - m_BackDepth });

            mesh.SetMaterial(m_CushionMaterial);

            // Transform (set on base)
            mesh.Transform(new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_CushionHeight / 2, z = 0 });

            // Transform (move to side)
            mesh.Transform(new Vector3 { x = cushionWidth * (m_CushionCount - 1) / -2, y = 0, z = 0 });

            // Array
            Vector3 stride = new Vector3 { x = cushionWidth, y = 0, z = 0 };
            mesh.Array(m_CushionCount, stride);

            // Transform (move to back)
            mesh.Transform(new Vector3 { x = 0, y = 0, z = -m_BackDepth / 2 });

            // Bevel
            mesh.Bevel(m_CushionBevel);

            return mesh;
        }

        private Reformable BackCushionMesh()
        {
            float cushionWidth = ((m_Width - (m_SideWidth * 2)) / m_CushionCount);

            // New Cube
            Reformable mesh = m_Object.Cube(new Vector3 { x = cushionWidth, y = m_BackHeight - m_CushionHeight, z = m_BackCushionDepth });

            mesh.SetMaterial(m_BackCushionMaterial);

            // Transform (set on base)
            mesh.Transform(new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_CushionHeight + (m_BackHeight - m_CushionHeight) / 2, z = 0 });

            // Transform (move to side)
            mesh.Transform(new Vector3 { x = cushionWidth * (m_CushionCount - 1) / -2, y = 0, z = 0 });

            // Transform (move to back)
            mesh.Transform(new Vector3 { x = 0, y = 0, z = m_Depth / 2 - m_BackDepth - m_BackCushionDepth / 2 });

            // Bevel
            mesh.Bevel(m_CushionBevel);

            // Array
            Vector3 stride = new Vector3 { x = cushionWidth, y = 0, z = 0 };
            mesh.Array(m_CushionCount, stride);

            return mesh;
        }
    }
}
