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
            /// Generates lattice-based support beams underneath the inlet and
            /// outlet pipes in order to make them printable.
            /// </summary>
            protected Voxels voxGetIOSupports()
            {
                float fMinBeam                  = 1f;
                Lattice oLattice                = new Lattice();
                List<LocalFrame> aLocalFrames   = new List<LocalFrame>()
                {
                    m_oFirstInletFrame,
                    m_oFirstOutletFrame,
                    m_oSecondInletFrame,
                    m_oSecondOutletFrame
                };

                foreach (LocalFrame oFrame in aLocalFrames)
                {
                    float fBackwardAngle1 = -50 / 180f * MathF.PI;
                    if (oFrame.vecGetPosition().X > 0)
                    {
                        fBackwardAngle1 = -fBackwardAngle1;
                    }

                    float fBackwardAngle2 = -20 / 180f * MathF.PI;
                    if (oFrame.vecGetPosition().X > 0)
                    {
                        fBackwardAngle2 = -fBackwardAngle2;
                    }

                    float fInwardAngle = 15f / 180f * MathF.PI;
                    if (oFrame.vecGetPosition().Y > 0)
                    {
                        fInwardAngle = -fInwardAngle;
                    }

                    Vector3 vecDir1  = VecOperations.vecRotateAroundAxis(-Vector3.UnitZ, fBackwardAngle1, Vector3.UnitY);
                    vecDir1          = VecOperations.vecRotateAroundAxis(vecDir1, fInwardAngle, Vector3.UnitX);

                    Vector3 vecDir2  = VecOperations.vecRotateAroundAxis(-Vector3.UnitZ, fBackwardAngle2, Vector3.UnitY);
                    vecDir2          = VecOperations.vecRotateAroundAxis(vecDir2, fInwardAngle, Vector3.UnitX);

                    for (float dS = 0f; dS < 30f; dS++)
                    {
                        float fLR       = dS / 30f;
                        float fMaxBeam  = Uf.fTransFixed(m_fIORadius + 6f, m_fIORadius + 2f, fLR);
                        float dH        = (fMaxBeam - fMinBeam) / MathF.Tan((30f) / 180f * MathF.PI);

                        Vector3 vecPt1  = oFrame.vecGetPosition() + (10f - dS) * oFrame.vecGetLocalZ();
                        Vector3 vecKink = vecPt1 + dH * vecDir2;
                        Vector3 vecPt2  = vecKink - (vecKink.Z / vecDir1.Z) * vecDir1;

                        oLattice.AddBeam(vecPt1, fMaxBeam, vecKink, fMinBeam);
                        oLattice.AddBeam(vecKink, fMinBeam, vecPt2, fMinBeam);
                    }
                }
                Sh.PreviewLattice(oLattice, Cp.clrYellow, 0.5f);
                return new Voxels(oLattice);
            }
		}
	}
}