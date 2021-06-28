// Copyright 2020 The Tilt Brush Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System;

namespace TiltBrush
{
    public class QuadStripBrushStretchUV : QuadStripBrush
    {
        struct UpdateUVRequest
        {
            public int back, front;

            public bool IsValid { get { return back != -1; } }
            public void Clear() { back = -1; front = -1; }
            public void Set(int back_, int front_)
            {
                back = back_; front = front_;
            }
        }

        // Store width in Z component of uv0. Currently used only by hypercolor./在uv0的Z分量中存储宽度。目前仅由hypercolor使用。
        [SerializeField] bool m_StoreWidthInTexcoord0Z;
        private float[] m_QuadLengths;
        // private float m_TotalStrokeLength;
        private UpdateUVRequest m_UpdateUVRequest = new UpdateUVRequest();

        protected override void InitBrush(BrushDescriptor desc, TrTransform localPointerXf)
        {
            base.InitBrush(desc, localPointerXf);
            m_QuadLengths = new float[m_NumQuads];
            // m_TotalStrokeLength = 0.0f;
            m_UpdateUVRequest.Clear();
        }

        public override GeometryPool.VertexLayout GetVertexLayout(BrushDescriptor desc)
        {
            return new GeometryPool.VertexLayout
            {
                bUseColors = true,
                bUseNormals = true,
                bUseTangents = true,
                uv0Size = m_StoreWidthInTexcoord0Z ? 3 : 2,
                uv0Semantic = m_StoreWidthInTexcoord0Z ? GeometryPool.Semantic.XyIsUvZIsDistance : GeometryPool.Semantic.XyIsUv,
                uv1Size = 0
            };
        }

        // This should only be called for the first quad of each solid//这应该只为每个实体的第一个四元组调用
        override protected void UpdateUVsForQuad(int iQuadIndex)
        {
            var rMasterBrush = m_Geometry;
            //compute length of this quad and add it to our total//计算这个四边形的长度并把它加到我们的总数中
            float fQuadLength = QuadLength(rMasterBrush.m_Vertices, iQuadIndex);
            // m_TotalStrokeLength -= m_QuadLengths[iQuadIndex];
            m_QuadLengths[iQuadIndex] = fQuadLength;
            // m_TotalStrokeLength += fQuadLength;

            //store our backface length, but don't bother adding it to our total/存储我们的背面长度，但不要费心添加到我们的总长度
            if (m_EnableBackfaces)
            {
                Debug.Assert(iQuadIndex % 2 == 0);
                m_QuadLengths[iQuadIndex + 1] = fQuadLength;
            }
        }

        override protected void UpdateUVsForSegment(int iSegmentBack,
                                                    int iSegmentFront, float size)
        {
            if (m_UpdateUVRequest.IsValid && m_UpdateUVRequest.back != iSegmentBack)
            {
                FlushUpdateUVRequest();
            }
            if (m_UpdateUVRequest.IsValid)
            {
                // Take the union of the two requests//以两个请求的联合为例
                iSegmentFront = Mathf.Max(iSegmentFront, m_UpdateUVRequest.front);
            }
            m_UpdateUVRequest.Set(iSegmentBack, iSegmentFront);
        }

        override public void ApplyChangesToVisuals()
        {
            UnityEngine.Profiling.Profiler.BeginSample("QuadStripBrushStretchUV.ApplyChangesToVisuals");
            FlushUpdateUVRequest();

            MeshFilter mf = GetComponent<MeshFilter>();
            MasterBrush geometry = m_Geometry;
            mf.mesh.vertices = geometry.m_Vertices;
            mf.mesh.normals = geometry.m_Normals;
            mf.mesh.colors32 = geometry.m_Colors;
            mf.mesh.tangents = geometry.m_Tangents;


            if (m_StoreWidthInTexcoord0Z)
            {
                mf.mesh.SetUVs(0, geometry.m_UVWs);
            }
            else//被执行
            {
                mf.mesh.uv = geometry.m_UVs;
            }
            mf.mesh.RecalculateBounds();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        // iSegmentBack       quad index of segment, trailing edge
        // iSegmentFront      quad index of segment, leading edge
        // "solid" is my term for "the thing comprised of a frontface and backface quad"
        //iSegmentBack段后缘四元索引
        //ISegment前段四元索引，前缘 
        //“实心”是我对“由正面和背面四边形组成的东西”的称呼
        protected void FlushUpdateUVRequest()
        {
            MasterBrush rMasterBrush = m_Geometry;
            if (!m_UpdateUVRequest.IsValid)
            {
                return;
            }

            else//被执行
            {
                //Debug.Log("m_UpdateUVRequest.IsValid==true");
            }

            int iSegmentBack = m_UpdateUVRequest.back;
            int iSegmentFront = m_UpdateUVRequest.front;

            m_UpdateUVRequest.Clear();

            int quadsPerSolid = m_EnableBackfaces ? 2 : 1;
            //Debug.Log("FlushUpdateUVRequest中m_EnableBackfaces的值为：" + m_EnableBackfaces);//值为false

            int numSolids = (iSegmentFront - iSegmentBack) / quadsPerSolid;//quadsPerSolid==1

            float fYStart, fYEnd;
            {
                float random01 = m_rng.In01(iSegmentBack * 6);
                int numV = m_Desc.m_TextureAtlasV;  //m_Desc.m_TextureAtlasV==1
                int iAtlas = (int)(random01 * numV);
                fYStart = (iAtlas) / (float)numV;
                fYEnd = (iAtlas + 1) / (float)numV;

            }

            //get length of current segment//获取当前段的长度
            float fSegmentLength = 0.0f;
            for (int iSolid = 0; iSolid < numSolids; ++iSolid)
            {
                fSegmentLength += m_QuadLengths[iSegmentBack + (iSolid * quadsPerSolid)];
            }
            // Just enough to get rid of NaNs. If length is 0, doesn't really matter what UVs are
            //足以摆脱南斯。如果长度为0，那么UV是什么并不重要
            if (fSegmentLength == 0) { fSegmentLength = 1; }

            //then, run back through the last segment and update our UVs
            //然后，返回最后一段并更新UV
            float fRunningLength = 0.0f;
            for (int iSolid = 0; iSolid < numSolids; ++iSolid)
            {
                int iQuadIndex = iSegmentBack + (iSolid * quadsPerSolid);
                int iVertIndex = iQuadIndex * 6;
                float thisSolidLength = m_QuadLengths[iQuadIndex];  // assumes frontface == backface length假设正面==背面长度
                float fXStart = fRunningLength / fSegmentLength;
                float fXEnd = (fRunningLength + thisSolidLength) / fSegmentLength;
                fRunningLength += thisSolidLength;

                rMasterBrush.m_UVs[iVertIndex].Set(fXStart, fYStart);
                rMasterBrush.m_UVs[iVertIndex + 1].Set(fXEnd, fYStart);
                rMasterBrush.m_UVs[iVertIndex + 2].Set(fXStart, fYEnd);
                rMasterBrush.m_UVs[iVertIndex + 3].Set(fXStart, fYEnd);
                rMasterBrush.m_UVs[iVertIndex + 4].Set(fXEnd, fYStart);
                rMasterBrush.m_UVs[iVertIndex + 5].Set(fXEnd, fYEnd);


                if (m_StoreWidthInTexcoord0Z)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        rMasterBrush.m_UVWs[iVertIndex + i] = new Vector3(
                          rMasterBrush.m_UVs[iVertIndex + i].x,
                          rMasterBrush.m_UVs[iVertIndex + i].y
                        );
                    }
                }
                else//被执行
                {
                    //Debug.Log("m_StoreWidthInTexcoord0Z==null");
                }
            }

            // Update tangent space更新切线空间
            ComputeTangentSpaceForQuads(
                rMasterBrush.m_Vertices,
                rMasterBrush.m_UVs,
                rMasterBrush.m_Normals,
                rMasterBrush.m_Tangents,
                quadsPerSolid * 6,
                iSegmentBack * 6,
                iSegmentFront * 6);

            if (m_StoreWidthInTexcoord0Z)
            {
                Vector3 uvw;
                for (int iSolid = 0; iSolid < numSolids; ++iSolid)
                {
                    int iQuadIndex = iSegmentBack + (iSolid * quadsPerSolid);
                    int iVertIndex = iQuadIndex * 6;
                    float width = (rMasterBrush.m_Vertices[iVertIndex + 0]
                                  - rMasterBrush.m_Vertices[iVertIndex + 2]).magnitude;
                    for (int i = 0; i < 6; i++)
                    {
                        uvw = rMasterBrush.m_UVWs[iVertIndex + i];
                        uvw.z = width;
                        rMasterBrush.m_UVWs[iVertIndex + i] = uvw;
                    }
                }
            }
            else//被执行
            {
                //Debug.Log("m_StoreWidthInTexcoord0Z为空");
            }

            if (m_EnableBackfaces)
            {
                for (int iSolid = 0; iSolid < numSolids; ++iSolid)
                {
                    int iQuadIndex = iSegmentBack + (iSolid * quadsPerSolid);
                    int iVertIndex = iQuadIndex * 6;
                    if (m_StoreWidthInTexcoord0Z)
                    {
                        MirrorQuadFace(rMasterBrush.m_UVWs, iVertIndex);
                    }
                    else
                    {
                        MirrorQuadFace(rMasterBrush.m_UVs, iVertIndex);
                    }
                    MirrorTangents(rMasterBrush.m_Tangents, iVertIndex);
                }

                //Debug.Log("m_EnableBackfaces==true");
            }

            else//被执行
            {
                //Debug.Log("m_EnableBackfaces==false");
            }
        }

        override protected void UpdateUVs(int iQuad0, int iQuad1, float size)
        {
            // Store the length of the quads we just created. This doesn't update UVs.存储我们刚刚创建的四边形的长度。这不会更新UV。
            int iNumQuadsPer = m_EnableBackfaces ? 2 : 1;
            for (int i = iQuad0; i < iQuad1; i += iNumQuadsPer)
            {
                UpdateUVsForQuad(i);
            }
            // This actually modifies the UVs//这实际上会修改UV
            UpdateUVsForSegment(m_InitialQuadIndex, iQuad1, size);
        }

        public override BatchSubset FinalizeBatchedBrush()
        {
            FlushUpdateUVRequest();
            return base.FinalizeBatchedBrush();
        }

        public override void FinalizeSolitaryBrush()
        {
            FlushUpdateUVRequest();
            base.FinalizeSolitaryBrush();
        }
    }
}  // namespace TiltBrush
