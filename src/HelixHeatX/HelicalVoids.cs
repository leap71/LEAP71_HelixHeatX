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
            /// Generates the helical fluid volume of the specified type (first fluid or second fluid).
            /// Including the inlet and oulet splitter walls.
            /// </summary>
            protected void GetHelicalVoid(out Voxels voxInnerVolume, out Voxels voxSplitters, EFluid eFluid)
            {
                float fPhiStart = MathF.PI;
                if (eFluid == EFluid.COOL)
                {
                    fPhiStart = 0;
                }
                float fBeam         = 0.5f * m_fPlateThickness;
                float fStartZ       = 0;
                float fEndZ         = 100f;
                float fTotalLength  = fEndZ - fStartZ;
                float fInterPlateThickness
                                    = m_fWallThickness;
                uint nTurns         = (uint)(fTotalLength / (2f * m_fPlateThickness + 2f * fInterPlateThickness));
                float fTurns        = nTurns - 0.5f;
                float fSlope        = (fTurns * 2f * MathF.PI) / fTotalLength;

                //helical void
                Lattice latVoid     = new Lattice();
                Vector3 vecFirstPt1 = new Vector3();
                Vector3 vecFirstPt2 = new Vector3();
                Vector3 vecLastPt1  = new Vector3();
                Vector3 vecLastPt2  = new Vector3();

                uint nSamples = (uint)(fTotalLength / 0.005f);
                for (int i = 0; i < nSamples; i++)
                {
                    float fLengthRatio  = (float)i / (float)(nSamples);
                    float fZ            = fStartZ + fLengthRatio * (fEndZ - fStartZ);
                    float fPhi          = fPhiStart + fSlope * (fZ - fStartZ);
                    float fInnerRadius  = fGetInnerRadius(fPhi, fLengthRatio);
                    float fOuterRadius  = fGetOuterRadius(fPhi, fLengthRatio) - fBeam;
                    Vector3 vecPt1      = VecOperations.vecGetCylPoint(fInnerRadius, fPhi, fZ);
                    Vector3 vecPt2      = VecOperations.vecGetCylPoint(fOuterRadius, fPhi, fZ);
                    vecPt1              = vecTrafo(vecPt1);
                    vecPt2              = vecTrafo(vecPt2);
                    Vector3 vecPt3      = vecPt1 + 3f * Vector3.UnitZ;
                    Vector3 vecPt4      = vecPt2 + 3f * Vector3.UnitZ;
                    latVoid.AddBeam(vecPt1, fBeam, vecPt2, fBeam);
                    latVoid.AddBeam(vecPt1, fBeam, vecPt3, 0.2f);
                    latVoid.AddBeam(vecPt2, fBeam, vecPt4, 0.2f);

                    if (i == 0)
                    {
                        vecFirstPt1 = vecPt1;
                        vecFirstPt2 = vecPt2;
                    }
                    if (i == nSamples - 1)
                    {
                        vecLastPt1 = vecPt1;
                        vecLastPt2 = vecPt2;
                    }
                }
                Voxels voxHelicalVoid = new Voxels(latVoid);
                GetInlet(   out Voxels voxInlet,    out Voxels voxInletSplitter, eFluid, vecFirstPt1, vecFirstPt2, fBeam);
                GetOutlet(  out Voxels voxOutlet,   out Voxels voxOutletSplitter, eFluid, vecLastPt1, vecLastPt2, fBeam);

                voxInnerVolume      = Sh.voxUnion(voxInlet, voxOutlet);
                voxInnerVolume      = Sh.voxUnion(voxInnerVolume, voxHelicalVoid);
                voxSplitters        = Sh.voxUnion(voxInletSplitter, voxOutletSplitter);
            }
		}
	}
}