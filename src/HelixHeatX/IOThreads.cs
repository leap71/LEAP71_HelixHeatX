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
    using ConstructionModules;

    namespace CoolCube
    {
        public partial class HelixHeatX
        {
            /// <summary>
            /// Generates thread reinforcements on all inlet and outlet pipe ends.
            /// Adds material for post-processing thread cutting and connectors.
            /// </summary>
            protected Voxels voxGetIOThreads()
            {
                List<Voxels> aVoxelList       = new List<Voxels>();
                float fOuterRadius              = 14f;
                float fLength                   = 12f;
                Vector3 vecShift                = Vector3.UnitZ;
                LocalFrame oFrame1              = LocalFrame.oGetTranslatedFrame(m_oFirstInletFrame, vecShift);
                LocalFrame oFrame2              = LocalFrame.oGetTranslatedFrame(m_oSecondInletFrame, vecShift);
                LocalFrame oFrame3              = LocalFrame.oGetTranslatedFrame(m_oFirstOutletFrame, vecShift);
                LocalFrame oFrame4              = LocalFrame.oGetTranslatedFrame(m_oSecondOutletFrame, vecShift);
                oFrame1                         = LocalFrame.oGetInvertFrame(oFrame1, true, false);
                oFrame2                         = LocalFrame.oGetInvertFrame(oFrame2, true, false);
                oFrame3                         = LocalFrame.oGetInvertFrame(oFrame3, true, false);
                oFrame4                         = LocalFrame.oGetInvertFrame(oFrame4, true, false);
                oFrame1                         = LocalFrame.oGetTranslatedFrame(oFrame1, -fLength * oFrame1.vecGetLocalZ());
                oFrame2                         = LocalFrame.oGetTranslatedFrame(oFrame2, -fLength * oFrame2.vecGetLocalZ());
                oFrame3                         = LocalFrame.oGetTranslatedFrame(oFrame3, -fLength * oFrame3.vecGetLocalZ());
                oFrame4                         = LocalFrame.oGetTranslatedFrame(oFrame4, -fLength * oFrame4.vecGetLocalZ());
                ThreadReinforcement oThread1    = new ThreadReinforcement(oFrame1, fLength, m_fIORadius, fOuterRadius);
                ThreadReinforcement oThread2    = new ThreadReinforcement(oFrame2, fLength, m_fIORadius, fOuterRadius);
                ThreadReinforcement oThread3    = new ThreadReinforcement(oFrame3, fLength, m_fIORadius, fOuterRadius);
                ThreadReinforcement oThread4    = new ThreadReinforcement(oFrame4, fLength, m_fIORadius, fOuterRadius);
                aVoxelList.Add(oThread1.voxConstruct());
                aVoxelList.Add(oThread2.voxConstruct());
                aVoxelList.Add(oThread3.voxConstruct());
                aVoxelList.Add(oThread4.voxConstruct());

                Voxels voxIOFlanges             = Sh.voxUnion(aVoxelList);
                Sh.PreviewVoxels(voxIOFlanges, Cp.clrRacingGreen, 0.6f);
                return voxIOFlanges;
            }
		}
	}
}