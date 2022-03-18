using System.Collections.Generic;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class Reformable : XRBaseInteractable
    {
        protected ProBuilderMesh m_Mesh;


        protected override void OnEnable()
        {
            base.OnEnable();
            m_Mesh = GetComponent<ProBuilderMesh>();
        }

        protected delegate bool GNSelectionVector3(Vector3 v);
        protected delegate Vector3 GNVector3(Vector3 v);

        protected ProBuilderMesh GNJoinGeometry(params ProBuilderMesh[] meshes)
        {
            CombineMeshes.Combine(meshes, meshes[0]);

            for (int i = 1; i < meshes.Length; i++) Destroy(meshes[i].gameObject);

            return meshes[0];
        }

        protected ProBuilderMesh GNJoinGeometry(List<ProBuilderMesh> meshes)
        {
            CombineMeshes.Combine(meshes, meshes[0]);

            for (int i = 1; i < meshes.Count; i++) Destroy(meshes[i].gameObject);

            return meshes[0];
        }

        protected ProBuilderMesh GNCloneMesh(ProBuilderMesh mesh)
        {
            mesh.ToMesh();
            mesh.Refresh();
            return Instantiate(mesh);
        }

        // initialize with a cube, y is up
        protected ProBuilderMesh GNCube(Vector3 size)
        {
            return ShapeGenerator.GenerateCube(PivotLocation.Center, size);
        }

        protected void GNTransform(ProBuilderMesh mesh, Vector3? position = null, Vector3? scale = null)
        {
            Vector3 _position = position ?? Vector3.zero;
            Vector3 _scale = scale ?? Vector3.one;

            Vertex[] vertices = mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                newPosition = vertices[i].position;
                newPosition.x = (newPosition.x + _position.x) * _scale.x;
                newPosition.y = (newPosition.y + _position.y) * _scale.y;
                newPosition.z = (newPosition.z + _position.z) * _scale.z;
                vertices[i].position = newPosition;
            }
            mesh.SetVertices(vertices);
        }

        protected void GNTransform(ProBuilderMesh mesh, GNVector3 position)
        {
            Vertex[] vertices = mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                newPosition = position(vertices[i].position);
                vertices[i].position = newPosition;
            }
            mesh.SetVertices(vertices);
        }

        protected void GNSetPosition(ProBuilderMesh mesh, GNSelectionVector3 selection, GNVector3 position)
        {
            Vertex[] vertices = mesh.GetVertices();
            Vector3 newPosition;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                if (selection(vertices[i].position))
                {
                    newPosition = position(vertices[i].position);
                    vertices[i].position = newPosition;
                }
            }
            mesh.SetVertices(vertices);
        }

        protected void GNFlipFaces(ProBuilderMesh mesh)
        {
            for (int i = 0; i < mesh.faces.Count; i++)
            {
                mesh.faces[i].Reverse();
            }
        }

        protected void GNArray(ProBuilderMesh mesh, int count, Vector3 stride)
        {
            ProBuilderMesh[] meshes = new ProBuilderMesh[count];
            meshes[0] = mesh;
            for (int i = 1; i < count; i++)
            {
                ProBuilderMesh cloneMesh = GNCloneMesh(mesh);
                meshes[i] = cloneMesh;
                GNTransform(cloneMesh, i * stride);
            }
            GNJoinGeometry(meshes);
        }

        protected void GNBevel(ProBuilderMesh mesh, float bevel)
        {
            mesh.SetSelectedFaces(mesh.faces);
            List<Face> faces = Bevel.BevelEdges(mesh, mesh.selectedEdges, bevel);
            mesh.SetSelectedFaces(faces);
            mesh.ClearSelection();
        }
    }
}
