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
            protected Vector3 vecTrafo(Vector3 vecPt)
            {
                return VecOperations.vecTranslatePointOntoFrame(m_oCentreBottomFrame, vecPt);
            }

            /// <summary>
            /// Describes the inner radius distribution of the helix / spiral.
            /// </summary>
            protected float fGetInnerRadius(float fPhi, float fLengthRatio)
            {
                return 10f * Uf.fGetSuperShapeRadius(fPhi, Uf.ESuperShape.ROUND);
            }

            /// <summary>
            /// Describes the outer radius distribution of the helix / spiral.
            /// </summary>
            protected float fGetOuterRadius(float fPhi, float fLengthRatio)
            {
                return 50f * Uf.fGetSuperShapeRadius(fPhi, Uf.ESuperShape.QUAD);
            }

            /// <summary>
            /// Generates a vertical wall along the centre hole of the spiral fluid volumes
            /// in order to support them during the print.
            /// </summary>
            protected void AddCentrePiece(ref Voxels voxOuterVolume)
            {
                BaseBox oFirstBox   = new BaseBox(m_oCentreBottomFrame, 100, 20, 2);
                Voxels voxFirstBox  = oFirstBox.voxConstruct();
                voxOuterVolume      = Sh.voxUnion(voxOuterVolume, voxFirstBox);
            }
		}
	}
}