using UnityEngine;
using System;

public class ChairReformableEntity : Reformable
{
    public const float DefaultWidth = 3.5f;
    public const float DefaultDepth = 1.2f;
    public const float DefaultLegWidth = 0.1f;
    public const float DefaultLegHeight = 0.3f;
    public const float DefaultLegTopMultiplier = 2;
    public const float DefaultLegAngleCoef = 0.3f;
    public const float DefaultLegInset = 0.1f;
    public const float DefaultBaseHeight = 0.1f;
    public const float DefaultSideHeight = 0.6f;
    public const float DefaultSideWidth = 0.1f;
    public const float DefaultBackHeight = 0.8f;
    public const float DefaultBackDepth = 0.1f;
    public const float DefaultCushionHeight = 0.2f;
    public const float DefaultCushionBevel = 0.02f;
    public const int DefaultCushionCount = 3;
    public const float DefaultBackCushionDepth = 0.2f;

    [NonSerialized] public float m_Width = DefaultWidth;
    [NonSerialized] public float m_Depth = DefaultDepth;
    [NonSerialized] public float m_LegWidth = DefaultLegWidth;
    [NonSerialized] public float m_LegHeight = DefaultLegHeight;
    [NonSerialized] public float m_LegTopMultiplier = DefaultLegTopMultiplier;
    [NonSerialized] public float m_LegAngleCoef = DefaultLegAngleCoef;
    [NonSerialized] public float m_LegInset = DefaultLegInset;
    [NonSerialized] public float m_BaseHeight = DefaultBaseHeight;
    [NonSerialized] public float m_SideHeight = DefaultSideHeight;
    [NonSerialized] public float m_SideWidth = DefaultSideWidth;
    [NonSerialized] public float m_BackHeight = DefaultBackHeight;
    [NonSerialized] public float m_BackDepth = DefaultBackDepth;
    [NonSerialized] public float m_CushionHeight = DefaultCushionHeight;
    [NonSerialized] public float m_CushionBevel = DefaultCushionBevel;
    [NonSerialized] public int m_CushionCount = DefaultCushionCount;
    [NonSerialized] public float m_BackCushionDepth = DefaultBackCushionDepth;

    [NonSerialized] public Material m_LegMaterial;
    [NonSerialized] public Material m_BaseMaterial;
    [NonSerialized] public Material m_SideMaterial;
    [NonSerialized] public Material m_BackMaterial;
    [NonSerialized] public Material m_CushionMaterial;
    [NonSerialized] public Material m_BackCushionMaterial;

    public override void OnDraw()
    {
        Join(
            LegMesh(),
            BaseMesh(),
            SideMesh(),
            BackMesh(),
            CushionMesh(),
            BackCushionMesh()
            );
    }

    private Reformable LegMesh()
    {
        // New Cube
        Reformable mesh = Cube(new Vector3 { x = m_LegWidth, y = m_LegHeight, z = m_LegWidth });

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
        Reformable mesh = Cube(new Vector3 { x = m_Width, y = m_BaseHeight, z = m_Depth });

        mesh.SetMaterial(m_BaseMaterial);

        // Transform (set on legs)
        mesh.Transform(
            delegate (Vector3 v) { return new Vector3 { x = v.x, y = v.y + m_LegHeight + m_BaseHeight / 2, z = v.z }; });

        return mesh;
    }

    private Reformable SideMesh()
    {
        // New Cube
        Reformable mesh = Cube(new Vector3 { x = m_SideWidth, y = m_SideHeight, z = m_Depth - m_BackDepth });

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
        Reformable mesh = Cube(new Vector3 { x = m_Width, y = m_BackHeight, z = m_BackDepth });

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
        Reformable mesh = Cube(new Vector3 { x = cushionWidth, y = m_CushionHeight, z = m_Depth - m_BackDepth });

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
        Reformable mesh = Cube(new Vector3 { x = cushionWidth, y = m_BackHeight - m_CushionHeight, z = m_BackCushionDepth });

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