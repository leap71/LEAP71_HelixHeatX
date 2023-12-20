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
		public class ThreadCutter
		{
			protected LocalFrame	m_oFrame;
			protected float			m_fLength;
			protected float			m_fSlope;
			protected float			m_fCoreRadius;
			protected float			m_fMaxRadius;

			/// <summary>
			/// Screw cutter for simulating a post-production thread cut.
			/// https://gewindeaufschneider.de/blog/die-13-wichtigsten-gewindearten-die-sie-kennen-sollten/
			/// </summary>
			public ThreadCutter(
				LocalFrame	oFrame,
				float		fLength,
				float		fMaxRadius,
				float		fCoreRadius,
				float		fSlope)
			{
				m_oFrame		= oFrame;
				m_fLength		= fLength;
				m_fSlope		= fSlope;
				m_fCoreRadius	= fCoreRadius;
				m_fMaxRadius	= fMaxRadius;
			}

			public Voxels voxConstruct()
			{
				BaseCylinder oCore		= new BaseCylinder(m_oFrame, m_fLength, m_fCoreRadius);
                Voxels voxCore			= oCore.voxConstruct();

				BaseCylinder oBounding	= new BaseCylinder(m_oFrame, m_fLength, m_fMaxRadius);
                Voxels voxBounding		= oBounding.voxConstruct();
				float fTurns			= m_fLength / m_fSlope;
				float fBeam1			= 0.5f * m_fSlope;
				float fBeam2			= 0.1f;

				Lattice oLattice		= new Lattice();
				for (float fPhi = 0; fPhi <= fTurns * 2f * MathF.PI; fPhi += 0.005f)
				{
					float dS			= fPhi / (2f * MathF.PI) * m_fSlope;
					Vector3 vecRel1		= VecOperations.vecGetCylPoint(m_fCoreRadius, fPhi, dS);
					Vector3 vecRel2		= VecOperations.vecGetCylPoint(m_fMaxRadius, fPhi, dS);
					Vector3 vecPt1		= VecOperations.vecTranslatePointOntoFrame(m_oFrame, vecRel1);
					Vector3 vecPt2		= VecOperations.vecTranslatePointOntoFrame(m_oFrame, vecRel2);
					oLattice.AddBeam(vecPt1, fBeam1, vecPt2, fBeam2, false);
				}
                Voxels voxThread		= Sh.voxUnion(voxCore, new Voxels(oLattice));
				voxThread				= Sh.voxIntersect(voxThread, voxBounding);
				return voxThread;
			}
		}
	}
}