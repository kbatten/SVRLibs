namespace UnityEngine
{
    public class ChairReformable : MonoBehaviour
    {
        public float m_Width = ChairReformableEntity.DefaultWidth;
        public float m_Depth = ChairReformableEntity.DefaultDepth;
        public float m_LegWidth = ChairReformableEntity.DefaultLegWidth;
        public float m_LegHeight = ChairReformableEntity.DefaultLegHeight;
        public float m_LegTopMultiplier = ChairReformableEntity.DefaultLegTopMultiplier;
        public float m_LegAngleCoef = ChairReformableEntity.DefaultLegAngleCoef;
        public float m_LegInset = ChairReformableEntity.DefaultLegInset;
        public float m_BaseHeight = ChairReformableEntity.DefaultBaseHeight;
        public float m_SideHeight = ChairReformableEntity.DefaultSideHeight;
        public float m_SideWidth = ChairReformableEntity.DefaultSideWidth;
        public float m_BackHeight = ChairReformableEntity.DefaultBackHeight;
        public float m_BackDepth = ChairReformableEntity.DefaultBackDepth;
        public float m_CushionHeight = ChairReformableEntity.DefaultCushionHeight;
        public float m_CushionBevel = ChairReformableEntity.DefaultCushionBevel;
        public int m_CushionCount = ChairReformableEntity.DefaultCushionCount;
        public float m_BackCushionDepth = ChairReformableEntity.DefaultBackCushionDepth;

        public Material m_LegMaterial;
        public Material m_BaseMaterial;
        public Material m_SideMaterial;
        public Material m_BackMaterial;
        public Material m_CushionMaterial;
        public Material m_BackCushionMaterial;

        private ChairReformableEntity m_Entity;

        public bool m_Update = false;
        public bool m_UpdateRandom = false;

        private void OnEnable()
        {
            if (m_Entity == null) m_Entity = GetComponent<ChairReformableEntity>();
            if (m_Entity == null) m_Entity = gameObject.AddComponent<ChairReformableEntity>();
            m_Update = true;
            m_UpdateRandom = false;
        }

        private void Update()
        {
            if (m_Update || m_UpdateRandom)
            {
                if (m_UpdateRandom)
                {
                    m_Width = ChairReformableEntity.DefaultWidth * Random.Range(0.01f, 2);
                    m_Depth = ChairReformableEntity.DefaultDepth * Random.Range(0.01f, 2);
                    m_LegWidth = ChairReformableEntity.DefaultLegWidth * Random.Range(0.01f, 2);
                    m_LegHeight = ChairReformableEntity.DefaultLegHeight * Random.Range(0.01f, 2);
                    m_LegTopMultiplier = ChairReformableEntity.DefaultLegTopMultiplier * Random.Range(0.01f, 2);
                    m_LegAngleCoef = ChairReformableEntity.DefaultLegAngleCoef * Random.Range(0.01f, 2);
                    m_LegInset = ChairReformableEntity.DefaultLegInset * Random.Range(0.01f, 2);
                    m_BaseHeight = ChairReformableEntity.DefaultBaseHeight * Random.Range(0.01f, 2);
                    m_SideHeight = ChairReformableEntity.DefaultSideHeight * Random.Range(0.01f, 2);
                    m_SideWidth = ChairReformableEntity.DefaultSideWidth * Random.Range(0.01f, 2);
                    m_BackHeight = ChairReformableEntity.DefaultBackHeight * Random.Range(0.01f, 2);
                    m_BackDepth = ChairReformableEntity.DefaultBackDepth * Random.Range(0.01f, 2);
                    m_CushionHeight = ChairReformableEntity.DefaultCushionHeight * Random.Range(0.01f, 2);
                    m_CushionBevel = ChairReformableEntity.DefaultCushionBevel * Random.Range(0.01f, 2);
                    m_CushionCount = (int)(Random.Range(1, 10));
                    m_BackCushionDepth = ChairReformableEntity.DefaultBackCushionDepth * Random.Range(0.01f, 2);
                    m_UpdateRandom = false;
                }

                m_Entity.m_Width = m_Width;
                m_Entity.m_Depth = m_Depth;
                m_Entity.m_LegWidth = m_LegWidth;
                m_Entity.m_LegHeight = m_LegHeight;
                m_Entity.m_LegTopMultiplier = m_LegTopMultiplier;
                m_Entity.m_LegAngleCoef = m_LegAngleCoef;
                m_Entity.m_LegInset = m_LegInset;
                m_Entity.m_BaseHeight = m_BaseHeight;
                m_Entity.m_SideHeight = m_SideHeight;
                m_Entity.m_SideWidth = m_SideWidth;
                m_Entity.m_BackHeight = m_BackHeight;
                m_Entity.m_BackDepth = m_BackDepth;
                m_Entity.m_CushionHeight = m_CushionHeight;
                m_Entity.m_CushionBevel = m_CushionBevel;
                m_Entity.m_CushionCount = m_CushionCount;
                m_Entity.m_BackCushionDepth = m_BackCushionDepth;

                m_Entity.Draw();
                m_Update = false;
            }
        }
    }
}