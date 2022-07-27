using System.Collections.Generic;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEngine
{
    public class Reformable : MonoBehaviour
    {
        protected ProBuilderMesh m_Mesh;
        protected Collider m_Collider;

        public delegate bool ProcessSelectionVector3(Vector3 v);
        public delegate Vector3 ProcessVector3(Vector3 v);

        private List<Reformable> m_Children;

        [System.NonSerialized]
        public bool degenerate = false;

        private void OnEnable()
        {
            if (m_Mesh == null) m_Mesh = GetComponent<ProBuilderMesh>();
            if (m_Mesh == null)
            {
                m_Mesh = gameObject.AddComponent<ProBuilderMesh>();
                m_Mesh.Clear();
            }
            if (m_Collider == null) m_Collider = GetComponent<Collider>();
            m_Children = new List<Reformable>();
        }

        public void Draw()
        {
            if (m_Collider) m_Collider.enabled = false;

            Vector3 position = gameObject.transform.localPosition;
            gameObject.transform.localPosition = Vector3.zero;
            Quaternion rotation = gameObject.transform.localRotation;
            gameObject.transform.localRotation = Quaternion.identity;
            Vector3 scale = gameObject.transform.localScale;
            gameObject.transform.localScale = Vector3.one;

            m_Mesh.Clear();

            OnDraw();

            DeleteChildren();

            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;
            gameObject.transform.localScale = scale;

            if (m_Collider) m_Collider.enabled = true;
        }

        public virtual void OnDraw()
        {

        }

        private Reformable CreateChild()
        {
            var child = gameObject.AddComponent<Reformable>();
            m_Children.Add(child);
            return child;
        }

        private void DeleteChildren(bool deleteSelf=false)
        {
            foreach (Reformable reformable in m_Children) reformable.DeleteChildren(true);
            if (deleteSelf) Destroy(this);
            m_Children.Clear();
        }

        // initialize with a cube, y is up
        public Reformable Cube(Vector3 size)
        {
            if (size.x == 0 || size.y == 0 || size.z == 0) return Empty();
            Reformable cube = CreateChild();
            cube.m_Mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            return cube;
        }

        public Reformable Copy()
        {
            Reformable copy = CreateChild();
            copy.m_Mesh = Instantiate(m_Mesh);
            return copy;
        }

        public Reformable Empty()
        {
            var empty = CreateChild();
            empty.m_Mesh = Instantiate(m_Mesh);
            empty.m_Mesh.Clear();
            return empty;
        }

        public void Join(params Reformable[] meshes)
        {
            ProBuilderMesh[] meshlist = new ProBuilderMesh[meshes.Length + 1];
            meshlist[0] = m_Mesh;
            for (int i = 0; i < meshes.Length; i++)
            {
                meshlist[i + 1] = meshes[i].m_Mesh;
            }
            CombineMeshes.Combine(meshlist, m_Mesh);

            for (int i = 0; i < meshes.Length; i++) Destroy(meshes[i].m_Mesh.gameObject);
        }

        public void SetMaterial(Material material)
        {
            m_Mesh.SetMaterial(m_Mesh.faces, material);
        }

        public void Transform(Vector3? position = null, Vector3? scale = null)
        {
            Vector3 _position = position ?? Vector3.zero;
            Vector3 _scale = scale ?? Vector3.one;

            Vertex[] vertices = m_Mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < m_Mesh.vertexCount; i++)
            {
                newPosition = vertices[i].position;
                newPosition.x = (newPosition.x + _position.x) * _scale.x;
                newPosition.y = (newPosition.y + _position.y) * _scale.y;
                newPosition.z = (newPosition.z + _position.z) * _scale.z;
                vertices[i].position = newPosition;
            }
            m_Mesh.SetVertices(vertices);
        }

        public void Transform(ProcessVector3 position)
        {
            Vertex[] vertices = m_Mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < m_Mesh.vertexCount; i++)
            {
                newPosition = position(vertices[i].position);
                vertices[i].position = newPosition;
            }
            m_Mesh.SetVertices(vertices);
        }

        public void SetPosition(ProcessSelectionVector3 selection, ProcessVector3 position)
        {
            Vertex[] vertices = m_Mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < m_Mesh.vertexCount; i++)
            {
                if (selection(vertices[i].position))
                {
                    newPosition = position(vertices[i].position);
                    vertices[i].position = newPosition;
                }
            }
            m_Mesh.SetVertices(vertices);
        }

        public void FlipFaces()
        {
            for (int i = 0; i < m_Mesh.faces.Count; i++)
            {
                m_Mesh.faces[i].Reverse();
            }
        }

        public void Array(int count, Vector3 stride)
        {
            Reformable[] meshes = new Reformable[count - 1];
            for (int i = 0; i < count - 1; i++)
            {
                meshes[i] = Copy();
                meshes[i].Transform((i + 1) * stride);
            }
            Join(meshes);
        }

        public void Bevel(float bevel)
        {
            m_Mesh.SetSelectedFaces(m_Mesh.faces);
            List<Face> faces = ProBuilder.MeshOperations.Bevel.BevelEdges(m_Mesh, m_Mesh.selectedEdges, bevel);
            m_Mesh.SetSelectedFaces(faces);
            m_Mesh.ClearSelection();
        }
    }
}
