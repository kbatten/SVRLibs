using System.Collections.Generic;
using UnityEngine.ProBuilder;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class ChairReformable : Reformable
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

                        m_Mesh.Clear();

                        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { m_Mesh,
                            LegMesh(),
                            BaseMesh(),
                            SideMesh(),
                            BackMesh(),
                            CushionMesh(),
                            BackCushionMesh()
                        };
                        GNJoinGeometry(meshes);
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

        private ProBuilderMesh LegMesh()
        {
            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = m_LegWidth, y = m_LegHeight, z = m_LegWidth });

            mesh.SetMaterial(mesh.faces, m_LegMaterial);

            // Transform (set on ground)
            GNTransform(mesh, new Vector3 { x = 0, y = m_LegHeight / 2, z = 0 });

            // Set Position (coneify)
            GNSetPosition(mesh,
                delegate (Vector3 v) { return v.y > 0; },
                delegate (Vector3 v) { return new Vector3 { x = v.x * m_LegTopMultiplier, y = v.y, z = v.z * m_LegTopMultiplier }; });

            // Set Position (angle)
            GNSetPosition(mesh,
                delegate (Vector3 v) { return v.y > 0; },
                delegate (Vector3 v) { return new Vector3 { x = v.x + m_LegAngleCoef * m_LegHeight, y = v.y, z = v.z + m_LegAngleCoef * m_LegHeight }; });

            // Transform (inset)
            GNTransform(mesh, new Vector3 { x = m_LegInset, y = 0, z = m_LegInset });

            // Transform (move to corner)
            GNTransform(mesh, new Vector3 { x = -m_Width / 2, y = 0, z = -m_Depth / 2 });

            // Clone
            ProBuilderMesh cloneMesh = GNCloneMesh(mesh);

            // Transform (mirror x)
            GNTransform(cloneMesh,
                delegate (Vector3 v) { return new Vector3 { x = -v.x, y = v.y, z = v.z }; });

            // Flip normals
            GNFlipFaces(cloneMesh);

            // Join
            mesh = GNJoinGeometry(mesh, cloneMesh);

            // Clone
            cloneMesh = GNCloneMesh(mesh);

            // Transform (mirror z)
            GNTransform(cloneMesh,
                delegate (Vector3 v) { return new Vector3 { x = v.x, y = v.y, z = -v.z }; });

            // Flip normals
            GNFlipFaces(cloneMesh);

            // Join
            mesh = GNJoinGeometry(mesh, cloneMesh);

            return mesh;
        }

        private ProBuilderMesh BaseMesh()
        {
            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = m_Width, y = m_BaseHeight, z = m_Depth });

            mesh.SetMaterial(mesh.faces, m_BaseMaterial);

            // Transform (set on legs)
            GNTransform(mesh,
                delegate (Vector3 v) { return new Vector3 { x = v.x, y = v.y + m_LegHeight + m_BaseHeight / 2, z = v.z }; });

            return mesh;
        }

        private ProBuilderMesh SideMesh()
        {
            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = m_SideWidth, y = m_SideHeight, z = m_Depth - m_BackDepth });

            mesh.SetMaterial(mesh.faces, m_SideMaterial);

            // Transform (set on base)
            GNTransform(mesh, new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_SideHeight / 2, z = 0 });

            // Transform (move to side, adjust for BackDepth)
            GNTransform(mesh, new Vector3 { x = -m_Width / 2 + m_SideWidth / 2, y = 0, z = -m_BackDepth / 2 });

            // Clone
            ProBuilderMesh cloneMesh = GNCloneMesh(mesh);

            // Transform (mirror x)
            GNTransform(cloneMesh,
                delegate (Vector3 v) { return new Vector3 { x = -v.x, y = v.y, z = v.z }; });

            // Flip normals
            GNFlipFaces(cloneMesh);

            // Join
            mesh = GNJoinGeometry(mesh, cloneMesh);

            return mesh;
        }

        private ProBuilderMesh BackMesh()
        {
            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = m_Width, y = m_BackHeight, z = m_BackDepth });

            mesh.SetMaterial(mesh.faces, m_BackMaterial);

            // Transform (set on base)
            GNTransform(mesh, new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_BackHeight / 2, z = 0 });

            // Transform (move to back)
            GNTransform(mesh, new Vector3 { x = 0, y = 0, z = m_Depth / 2 - m_BackDepth / 2 });

            return mesh;
        }

        private ProBuilderMesh CushionMesh()
        {
            float cushionWidth = ((m_Width - (m_SideWidth * 2)) / m_CushionCount);

            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = cushionWidth, y = m_CushionHeight, z = m_Depth - m_BackDepth });

            mesh.SetMaterial(mesh.faces, m_CushionMaterial);

            // Transform (set on base)
            GNTransform(mesh, new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_CushionHeight / 2, z = 0 });

            // Transform (move to side)
            GNTransform(mesh, new Vector3 { x = cushionWidth * (m_CushionCount - 1) / -2, y = 0, z = 0 });

            // Array
            Vector3 stride = new Vector3 { x = cushionWidth, y = 0, z = 0 };
            GNArray(mesh, m_CushionCount, stride);

            // Transform (move to back)
            GNTransform(mesh, new Vector3 { x = 0, y = 0, z = -m_BackDepth / 2 });

            // Bevel
            GNBevel(mesh, m_CushionBevel);

            return mesh;
        }

        private ProBuilderMesh BackCushionMesh()
        {
            float cushionWidth = ((m_Width - (m_SideWidth * 2)) / m_CushionCount);

            // New Cube
            ProBuilderMesh mesh = GNCube(new Vector3 { x = cushionWidth, y = m_BackHeight - m_CushionHeight, z = m_BackCushionDepth });

            mesh.SetMaterial(mesh.faces, m_BackCushionMaterial);

            // Transform (set on base)
            GNTransform(mesh, new Vector3 { x = 0, y = m_LegHeight + m_BaseHeight + m_CushionHeight + (m_BackHeight - m_CushionHeight) / 2, z = 0 });

            // Transform (move to side)
            GNTransform(mesh, new Vector3 { x = cushionWidth * (m_CushionCount - 1) / -2, y = 0, z = 0 });

            // Transform (move to back)
            GNTransform(mesh, new Vector3 { x = 0, y = 0, z = m_Depth / 2 - m_BackDepth - m_BackCushionDepth / 2 });

            // Bevel
            GNBevel(mesh, m_CushionBevel);

            // Array
            Vector3 stride = new Vector3 { x = cushionWidth, y = 0, z = 0 };
            GNArray(mesh, m_CushionCount, stride);

            return mesh;
        }
    }
}
