//
// SPDX-License-Ivoxtifier: Apache-2.0
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

	namespace ConstructionModules
	{
		public class ScrewHole
		{
			protected LocalFrame	m_oFrame;
			protected float			m_fLength;
            protected float         m_fCoreRadius;
            protected float			m_fHeadLength;
			protected float			m_fHeadRadius;


            /// <summary>
            /// Generates a dummy "screw shape".
            /// The geometry is used to cut-out material where screws will be placed after printing.
            /// The thread's end is pointy.
            /// The screw thread is not modelled in detail.
            /// </summary>
            public ScrewHole(
				LocalFrame	oFrame,
				float		fLength,
				float		fCoreRadius,
                float       fHeadLength,
                float       fHeadRadius)
			{
				m_oFrame		= oFrame;
				m_fLength		= fLength;
				m_fCoreRadius	= fCoreRadius;
				m_fHeadLength	= fHeadLength;
                m_fHeadRadius   = fHeadRadius;
			}

            public Voxels voxConstruct()
            {
                Lattice latScrew    = new Lattice();
                Vector3 vecDir      = m_oFrame.vecGetLocalZ();

                //head
                Vector3 vecPt1      = m_oFrame.vecGetPosition();
                Vector3 vecPt0      = vecPt1 + vecDir * m_fHeadLength;

                //thread
                Vector3 vecPt2      = m_oFrame.vecGetPosition() - vecDir * m_fLength;
                Vector3 vecPt3      = vecPt2 - vecDir * (2f * m_fCoreRadius);

                latScrew.AddBeam(vecPt0, m_fHeadRadius, vecPt1, m_fHeadRadius, false);
                latScrew.AddBeam(vecPt2, m_fCoreRadius, vecPt1, m_fCoreRadius, false);
                latScrew.AddBeam(vecPt2, m_fCoreRadius, vecPt3, 0.1f, true);
                return new Voxels(latScrew);
            }
		}
	}
}