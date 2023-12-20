//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2023 by LEAP 71
// https://leap71.com
//
// Computational Engineering will profoundly change our physical world in the
// years ahead. Thank you for being part of the journey.
//
// We have developed this library to be used widely, for both commercial and
// non-commercial projects alike. Therefore, have released it under a permissive
// open-source license.
// 
// The LEAP 71 ShapeKernel is based on the PicoGK compact computational geometry 
// framework. See https://picogk.org for more information.
//
// LEAP 71 licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, THE SOFTWARE IS
// PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
//
// See the License for the specific language governing permissions and
// limitations under the License.   
//


using System.Numerics;
using PicoGK;


namespace Leap71
{
    using ShapeKernel;

    namespace CoolCube
    {
        public partial class HelixHeatX
        {
            /// <summary>
            /// Generates the inlet pipe transition and internal splitter wall for the specified fluid volume.
            /// </summary>
            protected void GetInlet(
                out Voxels  voxInlet,
                out Voxels  voxSplitter,
                EFluid      eFluid,
                Vector3     vecPt1,
                Vector3     vecPt2,
                float       fBeam)
            {
                Vector3 vecEnd          = m_oSecondInletFrame.vecGetPosition();
                Vector3 vecEndDir       = -Vector3.UnitX;

                Vector3 vecLengthDir    = (vecPt2 - vecPt1).Normalize();
                Vector3 vecNormal       = Vector3.Cross(Vector3.UnitY, vecLengthDir);
                Vector3 vecStartDir     = Vector3.Cross(vecLengthDir, vecNormal);
                if (eFluid == EFluid.HOT)
                {
                    vecEnd      = m_oFirstInletFrame.vecGetPosition();
                    vecNormal   = Vector3.Cross(-Vector3.UnitZ, vecLengthDir);
                    vecStartDir = Vector3.Cross(vecLengthDir, vecNormal);
                }

                float fInletRadius      = m_fIORadius;
                Vector3 vecStart        = 0.5f * (vecPt1 + vecPt2);
                Vector3 vecStartOri     = (vecPt2 - vecPt1).Normalize();
                float fStartLength      = (vecPt2 - vecStart).Length();
                ISpline oSpline         = new TangentialControlSpline(vecStart, vecEnd, vecStartDir, vecEndDir, 20, 10);

                uint nSamples           = 500;
                Lattice latInlet        = new Lattice();
                Lattice latSplitter     = new Lattice();

                List<Vector3> aPoints   = oSpline.aGetPoints(nSamples);
                for (int i = 0; i < aPoints.Count; i++)
                {
                    float fLengthRatio  = (float)(i) / (float)(aPoints.Count);
                    Vector3 vecPt       = aPoints[i];
                    Vector3 vecLocalX   = vecStartOri;
                    float fBeam2        = Uf.fTransFixed(fBeam, fInletRadius, fLengthRatio);
                    float fLength2      = Uf.fTransFixed(fStartLength, 0f, fLengthRatio);
                    float fTipExtension = Uf.fTransFixed(3f, 10f, fLengthRatio);
                    vecPt1              = vecPt - fLength2 * vecLocalX;
                    vecPt2              = vecPt + fLength2 * vecLocalX;
                    if (vecPt1.Z > vecPt2.Z)
                    {
                        Vector3 vecPt3          = vecPt1 + fTipExtension * Vector3.UnitZ;
                        latInlet.AddBeam(vecPt1, fBeam2, vecPt3, 0.2f);

                        Vector3 vecSplitterPt0  = vecPt2 - 10f * Vector3.UnitZ;
                        Vector3 vecSplitterPt1  = vecPt3 + fBeam2 * Vector3.UnitZ;
                        Vector3 vecSplitterPt2  = vecPt3 + (fBeam2 + 5f) * Vector3.UnitZ;
                        Vector3 vecSplitterPt3  = vecPt3 + (fBeam2 + 10f) * Vector3.UnitZ;
                        float fTopSplitterBeam  = Uf.fTransFixed(0.4f, 1f, fLengthRatio);

                        latSplitter.AddBeam(vecSplitterPt0, 0.4f, vecSplitterPt1, 0.4f);
                        latSplitter.AddBeam(vecSplitterPt1, 0.4f, vecSplitterPt2, fTopSplitterBeam);
                        latSplitter.AddBeam(vecSplitterPt2, fTopSplitterBeam, vecSplitterPt3, fTopSplitterBeam);
                    }
                    else
                    {
                        Vector3 vecPt3          = vecPt2 + fTipExtension * Vector3.UnitZ;
                        latInlet.AddBeam(vecPt2, fBeam2, vecPt3, 0.2f);

                        Vector3 vecSplitterPt0  = vecPt1 - 10f * Vector3.UnitZ;
                        Vector3 vecSplitterPt1  = vecPt3 + fBeam2 * Vector3.UnitZ;
                        Vector3 vecSplitterPt2  = vecPt3 + (fBeam2 + 5f) * Vector3.UnitZ;
                        Vector3 vecSplitterPt3  = vecPt3 + (fBeam2 + 10f) * Vector3.UnitZ;
                        float fTopSplitterBeam  = Uf.fTransFixed(0.4f, 1f, fLengthRatio);

                        latSplitter.AddBeam(vecSplitterPt0, 0.4f, vecSplitterPt1, 0.4f);
                        latSplitter.AddBeam(vecSplitterPt1, 0.4f, vecSplitterPt2, fTopSplitterBeam);
                        latSplitter.AddBeam(vecSplitterPt2, fTopSplitterBeam, vecSplitterPt3, fTopSplitterBeam);
                    }
                    latInlet.AddBeam(vecPt1, fBeam2, vecPt2, fBeam2);
                }
                voxInlet    = new Voxels(latInlet);
                voxSplitter = new Voxels(latSplitter);
                voxSplitter = Sh.voxIntersect(voxSplitter, voxInlet);
            }

            /// <summary>
            /// Generates the outlet pipe transition and internal splitter wall for the specified fluid volume.
            /// </summary>
            protected void GetOutlet(
                out Voxels  voxOutlet,
                out Voxels  voxSplitter,
                EFluid      eFluid,
                Vector3     vecPt1,
                Vector3     vecPt2,
                float       fBeam)
            {
                Vector3 vecEnd          = m_oSecondOutletFrame.vecGetPosition();
                Vector3 vecEndDir       = Vector3.UnitX;

                Vector3 vecLengthDir    = (vecPt2 - vecPt1).Normalize();
                Vector3 vecNormal       = Vector3.Cross(Vector3.UnitY, vecLengthDir);
                Vector3 vecStartDir     = Vector3.Cross(vecLengthDir, vecNormal);

                if (eFluid == EFluid.HOT)
                {
                    vecEnd      = m_oFirstOutletFrame.vecGetPosition();
                    vecNormal   = Vector3.Cross(Vector3.UnitZ, vecLengthDir);
                    vecStartDir = Vector3.Cross(vecLengthDir, vecNormal);
                }
                float fInletRadius      = m_fIORadius;
                Vector3 vecStart        = 0.5f * (vecPt1 + vecPt2);
                Vector3 vecStartOri     = (vecPt2 - vecPt1).Normalize();
                float fStartLength      = (vecPt2 - vecStart).Length();
                ISpline oSpline         = new TangentialControlSpline(vecStart, vecEnd, vecStartDir, vecEndDir, 20, 10);

                uint nSamples           = 500;
                Lattice latOutlet       = new Lattice();
                Lattice latSplitter     = new Lattice();

                List<Vector3> aPoints   = oSpline.aGetPoints(nSamples);
                for (int i = 0; i < aPoints.Count; i++)
                {
                    float fLengthRatio  = (float)(i) / (float)(aPoints.Count);
                    Vector3 vecPt       = aPoints[i];
                    Vector3 vecLocalX   = vecStartOri;
                    float fBeam2        = Uf.fTransFixed(fBeam, fInletRadius, fLengthRatio);
                    float fLength2      = Uf.fTransFixed(fStartLength, 0f, fLengthRatio);
                    float fTipExtension = Uf.fTransFixed(3f, 10f, fLengthRatio);
                    vecPt1              = vecPt - fLength2 * vecLocalX;
                    vecPt2              = vecPt + fLength2 * vecLocalX;
                    if (vecPt1.Z > vecPt2.Z)
                    {
                        Vector3 vecPt3  = vecPt1 + fTipExtension * Vector3.UnitZ;
                        latOutlet.AddBeam(vecPt1, fBeam2, vecPt3, 0.2f);

                        Vector3 vecSplitterPt0 = vecPt2 - 10f * Vector3.UnitZ;
                        Vector3 vecSplitterPt1 = vecPt3 + fBeam2 * Vector3.UnitZ;
                        Vector3 vecSplitterPt2 = vecPt3 + (fBeam2 + 5f) * Vector3.UnitZ;
                        Vector3 vecSplitterPt3 = vecPt3 + (fBeam2 + 10f) * Vector3.UnitZ;
                        float fTopSplitterBeam = Uf.fTransFixed(0.4f, 1f, fLengthRatio);

                        latSplitter.AddBeam(vecSplitterPt0, 0.4f, vecSplitterPt1, 0.4f);
                        latSplitter.AddBeam(vecSplitterPt1, 0.4f, vecSplitterPt2, fTopSplitterBeam);
                        latSplitter.AddBeam(vecSplitterPt2, fTopSplitterBeam, vecSplitterPt3, fTopSplitterBeam);
                    }
                    else
                    {
                        Vector3 vecPt3 = vecPt2 + fTipExtension * Vector3.UnitZ;
                        latOutlet.AddBeam(vecPt2, fBeam2, vecPt3, 0.2f);

                        Vector3 vecSplitterPt0 = vecPt1 - 10f * Vector3.UnitZ;
                        Vector3 vecSplitterPt1 = vecPt3 + fBeam2 * Vector3.UnitZ;
                        Vector3 vecSplitterPt2 = vecPt3 + (fBeam2 + 5f) * Vector3.UnitZ;
                        Vector3 vecSplitterPt3 = vecPt3 + (fBeam2 + 10f) * Vector3.UnitZ;
                        float fTopSplitterBeam = Uf.fTransFixed(0.4f, 1f, fLengthRatio);

                        latSplitter.AddBeam(vecSplitterPt0, 0.4f, vecSplitterPt1, 0.4f);
                        latSplitter.AddBeam(vecSplitterPt1, 0.4f, vecSplitterPt2, fTopSplitterBeam);
                        latSplitter.AddBeam(vecSplitterPt2, fTopSplitterBeam, vecSplitterPt3, fTopSplitterBeam);
                    }
                    latOutlet.AddBeam(vecPt1, fBeam2, vecPt2, fBeam2);
                }
                voxOutlet   = new Voxels(latOutlet);
                voxSplitter = new Voxels(latSplitter);
                voxSplitter = Sh.voxIntersect(voxSplitter, voxOutlet);
            }
		}
	}
}