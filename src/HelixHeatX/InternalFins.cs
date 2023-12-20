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
            /// Generates fins along the corners of the specified fluid volume.
            /// In order to make the fins printable, they feature a rooftop-like
            /// height distribution across their width.
            /// </summary>
            protected Voxels voxGetTurningFins(EFluid eFluid)
            {
                float fPhiStart     = MathF.PI;
                if (eFluid == EFluid.COOL)
                {
                    fPhiStart       = 0;
                }
                float fWallThickness
                                    = 0.4f;
                float fBeam         = 0.5f * fWallThickness;
                float fStartZ       = 0;
                float fEndZ         = 100f;
                float fTotalLength  = fEndZ - fStartZ;
                float fInterPlateThickness
                                    = 0.8f;
                uint nTurns         = (uint)(fTotalLength / (2f * m_fPlateThickness + 2f * fInterPlateThickness));
                float fTurns        = nTurns - 0.5f;
                float fSlope        = (fTurns * 2f * MathF.PI) / fTotalLength;

                Lattice latFins     = new Lattice();
                uint nSamples       = (uint)(fTotalLength / 0.005f);
                for (int i = 0; i < nSamples; i++)
                {
                    float fLengthRatio  = (float)i / (float)(nSamples);
                    float fZ            = fStartZ + fLengthRatio * (fEndZ - fStartZ);
                    float fPhi          = fPhiStart + fSlope * (fZ - fStartZ);
                    float fPhiDeg       = fPhi / MathF.PI * 180f;
                    fPhiDeg %= 360;

                    float dAngle = 20;
                    if (fPhiDeg > 45f - dAngle && fPhiDeg < 45f + dAngle ||
                        fPhiDeg > 135f - dAngle && fPhiDeg < 135f + dAngle ||
                        fPhiDeg > 225f - dAngle && fPhiDeg < 225f + dAngle ||
                        fPhiDeg > 315f - dAngle && fPhiDeg < 315f + dAngle)
                    {
                        uint nFins = 20;
                        for (int j = 0; j < nFins; j++)
                        {
                            float fPhiFin       = fPhi - 15f / 180f * MathF.PI * MathF.Cos(3f * ((float)j / (float)(nFins) - 0.5f));

                            float fInnerRadius  = fGetInnerRadius(fPhiFin, fLengthRatio);
                            float fOuterRadius  = fGetOuterRadius(fPhiFin, fLengthRatio) - fBeam;
                            float fRadius       = fInnerRadius + 5f + (float)j / (float)(nFins - 1) * (fOuterRadius - 10f - fInnerRadius);
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ - 0.5f * m_fPlateThickness);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ + 0.5f * m_fPlateThickness);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);

                            vecPt1.Z -= 1.5f;
                            vecPt2.Z -= 1.5f;

                            Vector3 vecPt3      = 0.5f * (vecPt1 + vecPt2);
                            Vector3 vecPt4      = vecPt3 + 3f * Vector3.UnitZ;
                            latFins.AddBeam(vecPt1, fBeam, vecPt4, fBeam);
                            latFins.AddBeam(vecPt2, fBeam, vecPt4, fBeam);
                        }
                    }
                }
                return new Voxels(latFins);
            }

            /// <summary>
            /// Generates fins along the straight sections of each turn of the specified fluid volume.
            /// In order to make them printable, the horizontal fins feature a rooftop-like
            /// height distribution across their width.
            /// The vertical fins feature a twist for mixing.
            /// </summary>
            protected Voxels voxGetStraightFins(EFluid eFluid)
            {
                float fPhiStart     = MathF.PI;
                if (eFluid == EFluid.COOL)
                {
                    fPhiStart       = 0;
                }
                float fWallThickness
                                    = 0.4f;
                float fBeam         = 0.5f * fWallThickness;
                float fStartZ       = 0;
                float fEndZ         = 100f;
                float fTotalLength  = fEndZ - fStartZ;
                float fInterPlateThickness
                                    = 0.8f;
                uint nTurns         = (uint)(fTotalLength / (2f * m_fPlateThickness + 2f * fInterPlateThickness));
                float fTurns        = nTurns - 0.5f;
                float fSlope        = (fTurns * 2f * MathF.PI) / fTotalLength;

                Lattice latFins     = new Lattice();
                uint nSamples       = (uint)(fTotalLength / 0.005f);
                for (int i = 0; i < nSamples; i++)
                {
                    float fLengthRatio  = (float)i / (float)(nSamples);
                    float fZ            = fStartZ + fLengthRatio * (fEndZ - fStartZ);
                    float fPhi          = fPhiStart + fSlope * (fZ - fStartZ);
                    float fPhiDeg       = fPhi / MathF.PI * 180f;
                    fPhiDeg %= 360;

                    float dAngle = 15;
                    if (fPhiDeg > 0f - dAngle && fPhiDeg < 0 + dAngle ||
                        fPhiDeg > 360f - dAngle && fPhiDeg < 360 + dAngle ||
                        fPhiDeg > 180f - dAngle && fPhiDeg < 180f + dAngle)
                    {
                        uint nFins = 8;
                        for (int j = 0; j < nFins; j++)
                        {
                            float fPhiFin       = fPhi - 15f / 180f * MathF.PI * MathF.Cos(3f * ((float)j / (float)(nFins) - 0.5f));

                            float fInnerRadius  = fGetInnerRadius(fPhiFin, fLengthRatio);
                            float fOuterRadius  = fGetOuterRadius(fPhiFin, fLengthRatio) - fBeam;
                            float fRadius       = fInnerRadius + 5f + (float)j / (float)(nFins - 1) * (fOuterRadius - 10f - fInnerRadius);
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ - 0.5f * m_fPlateThickness);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ + 0.5f * m_fPlateThickness);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);

                            vecPt1.Z -= 1.5f;
                            vecPt2.Z -= 1.5f;

                            Vector3 vecPt3      = 0.5f * (vecPt1 + vecPt2);
                            Vector3 vecPt4      = vecPt3 + 3f * Vector3.UnitZ;
                            latFins.AddBeam(vecPt1, fBeam, vecPt4, fBeam);
                            latFins.AddBeam(vecPt2, fBeam, vecPt4, fBeam);
                        }
                    }
                    else if (fPhiDeg > 90f - dAngle && fPhiDeg < 90f + dAngle)
                    {
                        uint nFins = 8;
                        for (int j = 0; j < nFins; j++)
                        {
                            float fPhiFin       = fPhi - 15f / 180f * MathF.PI * MathF.Cos(3f * ((float)j / (float)(nFins) - 0.5f));

                            float fInnerRadius  = fGetInnerRadius(fPhiFin, fLengthRatio);
                            float fOuterRadius  = fGetOuterRadius(fPhiFin, fLengthRatio) - fBeam;
                            float fRadius       = fInnerRadius + 5f + (float)j / (float)(nFins - 1) * (fOuterRadius - 10f - fInnerRadius);
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ - 0.5f * m_fPlateThickness);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ + 0.5f * m_fPlateThickness);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);

                            vecPt1.Z -= 1.5f;
                            vecPt2.Z -= 1.5f;

                            Vector3 vecPt3      = 0.5f * (vecPt1 + vecPt2);

                            float dTurnPhi      = (fPhiDeg - 270 + dAngle) / (2f * dAngle) * 2f * MathF.PI;
                            vecPt1              = VecOperations.vecRotateAroundZ(vecPt1, dTurnPhi, vecPt3);
                            vecPt2              = VecOperations.vecRotateAroundZ(vecPt2, dTurnPhi, vecPt3);

                            Vector3 vecPt4      = vecPt3 + 3f * Vector3.UnitZ;
                            latFins.AddBeam(vecPt1, fBeam, vecPt4, fBeam);
                            latFins.AddBeam(vecPt2, fBeam, vecPt4, fBeam);
                        }
                    }
                    else if (fPhiDeg > 270f - dAngle && fPhiDeg < 270f + dAngle)
                    {
                        uint nFins = 8;
                        for (int j = 0; j < nFins; j++)
                        {
                            float fPhiFin       = fPhi - 15f / 180f * MathF.PI * MathF.Cos(3f * ((float)j / (float)(nFins) - 0.5f));

                            float fInnerRadius  = fGetInnerRadius(fPhiFin, fLengthRatio);
                            float fOuterRadius  = fGetOuterRadius(fPhiFin, fLengthRatio) - fBeam;
                            float fRadius       = fInnerRadius + 5f + (float)j / (float)(nFins - 1) * (fOuterRadius - 10f - fInnerRadius);
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ - 0.5f * m_fPlateThickness);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fRadius, fPhiFin, fZ + 0.5f * m_fPlateThickness);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);

                            vecPt1.Z -= 1.5f;
                            vecPt2.Z -= 1.5f;

                            Vector3 vecPt3      = 0.5f * (vecPt1 + vecPt2);

                            float dTurnPhi      = (fPhiDeg - 270 + dAngle) / (2f * dAngle) * 2f * MathF.PI;
                            vecPt1              = VecOperations.vecRotateAroundZ(vecPt1, dTurnPhi, vecPt3);
                            vecPt2              = VecOperations.vecRotateAroundZ(vecPt2, dTurnPhi, vecPt3);

                            Vector3 vecPt4      = vecPt3 + 3f * Vector3.UnitZ;
                            latFins.AddBeam(vecPt1, fBeam, vecPt4, fBeam);
                            latFins.AddBeam(vecPt2, fBeam, vecPt4, fBeam);
                        }
                    }
                }
                return new Voxels(latFins);
            }
		}
	}
}