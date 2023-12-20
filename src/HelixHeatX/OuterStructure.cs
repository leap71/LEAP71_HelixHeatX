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
            /// Generates the outer structure of the cube.
            /// Including a shell wall and reinforcement ribs.
            /// </summary>
            protected Voxels voxGetOuterStructure()
            {
                float fTotalLength          = 100;
                float fBeam                 = 1f;
                Lattice oLattice            = new Lattice();
                List<float> aSidePhis       = new List<float>() { 0, 0.5f * MathF.PI, MathF.PI, 1.5f * MathF.PI };

                for (float fZ = 0; fZ < fTotalLength; fZ += 0.3f)
                {
                    foreach (float fSidePhi in aSidePhis)
                    {
                        {
                            float fLengthRatio  = fZ / fTotalLength;
                            Vector3 vecCentre   = new Vector3(0, 0, fZ);
                            float fPhi          = fSidePhi + 0.25f * MathF.PI * MathF.Cos(2f * 2f * MathF.PI / fTotalLength * fZ);
                            float fInnerRadius  = fGetOuterRadius(fPhi, fLengthRatio) - 15f;
                            float fOuterRadius  = fGetOuterRadius(fPhi, fLengthRatio) + 15f;
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fInnerRadius, fPhi, fZ);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fOuterRadius, fPhi, fZ);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);
                            oLattice.AddBeam(vecPt1, fBeam, vecPt2, fBeam);
                        }
                        {
                            float fLengthRatio  = fZ / fTotalLength;
                            Vector3 vecCentre   = new Vector3(0, 0, fZ);
                            float fPhi          = fSidePhi - 0.25f * MathF.PI * MathF.Cos(2f * 2f * MathF.PI / fTotalLength * fZ);
                            float fInnerRadius  = fGetOuterRadius(fPhi, fLengthRatio) - 15f;
                            float fOuterRadius  = fGetOuterRadius(fPhi, fLengthRatio) + 15f;
                            Vector3 vecPt1      = VecOperations.vecGetCylPoint(fInnerRadius, fPhi, fZ);
                            Vector3 vecPt2      = VecOperations.vecGetCylPoint(fOuterRadius, fPhi, fZ);
                            vecPt1              = vecTrafo(vecPt1);
                            vecPt2              = vecTrafo(vecPt2);
                            oLattice.AddBeam(vecPt1, fBeam, vecPt2, fBeam);
                        }
                    }
                }

                Voxels voxStructure = new Voxels(oLattice);
                voxStructure        = Sh.voxOverOffset(voxStructure, 5f, 0.5f);
                voxStructure        = Sh.voxSmoothen(voxStructure, 1f);
                voxStructure        = Sh.voxIntersect(voxStructure, m_voxBounding);
                return voxStructure;
            }
		}
	}
}