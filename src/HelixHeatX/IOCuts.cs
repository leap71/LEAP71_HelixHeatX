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
            /// Generates shapes to cut open all inlet and outlet pipe ends.
            /// </summary>
            protected Voxels voxGetIOCuts()
            {
                List<Voxels> aVoxelList     = new List<Voxels>();
                float fCutRadius            = 2.5f;
                float fCutLength            = 12;
                LatticeManifold oCut1       = new LatticeManifold(m_oFirstInletFrame, fCutLength, fCutRadius);
                LatticeManifold oCut2       = new LatticeManifold(m_oSecondInletFrame, fCutLength, fCutRadius);
                LatticeManifold oCut3       = new LatticeManifold(m_oFirstOutletFrame, fCutLength, fCutRadius);
                LatticeManifold oCut4       = new LatticeManifold(m_oSecondOutletFrame, fCutLength, fCutRadius);
                aVoxelList.Add(oCut1.voxConstruct());
                aVoxelList.Add(oCut2.voxConstruct());
                aVoxelList.Add(oCut3.voxConstruct());
                aVoxelList.Add(oCut4.voxConstruct());

                LocalFrame oFrame1          = LocalFrame.oGetTranslatedFrame(m_oFirstInletFrame, (fCutLength + 2) * m_oFirstInletFrame.vecGetLocalZ());
                LocalFrame oFrame2          = LocalFrame.oGetTranslatedFrame(m_oSecondInletFrame, (fCutLength + 2) * m_oSecondInletFrame.vecGetLocalZ());
                LocalFrame oFrame3          = LocalFrame.oGetTranslatedFrame(m_oFirstOutletFrame, (fCutLength + 2) * m_oFirstOutletFrame.vecGetLocalZ());
                LocalFrame oFrame4          = LocalFrame.oGetTranslatedFrame(m_oSecondOutletFrame, (fCutLength + 2) * m_oSecondOutletFrame.vecGetLocalZ());
                aVoxelList.Add(new Voxels(Sh.latFromBeam(
                    oFrame1.vecGetPosition(),
                    oFrame1.vecGetPosition() - 4f * oFrame1.vecGetLocalZ(), 7f, 2f,
                    false)));
                aVoxelList.Add(new Voxels(Sh.latFromBeam(
                    oFrame2.vecGetPosition(),
                    oFrame2.vecGetPosition() - 4f * oFrame2.vecGetLocalZ(), 7f, 2f,
                    false)));
                aVoxelList.Add(new Voxels(Sh.latFromBeam(
                    oFrame3.vecGetPosition(),
                    oFrame3.vecGetPosition() - 4f * oFrame3.vecGetLocalZ(), 7f, 2f,
                    false)));
                aVoxelList.Add(new Voxels(Sh.latFromBeam(
                    oFrame4.vecGetPosition(),
                    oFrame4.vecGetPosition() - 4f * oFrame4.vecGetLocalZ(), 7f, 2f,
                    false)));

                Voxels voxIOCuts            = Sh.voxUnion(aVoxelList);
                Sh.PreviewVoxels(voxIOCuts, Cp.clrRed, 0.3f);
                return voxIOCuts;
            }

            /// <summary>
            /// Generates screw threads positioned on all inlets and outlets.
            /// They can be used to simulate the post-processed part.
            /// </summary>
            protected Voxels voxGetIOScrewCuts()
            {
                List<Voxels> aVoxelList   = new List<Voxels>();
                float fCoreRadius           = 5f;
                float fMaxRadius            = 6f;
                float fCutLength            = 14;
                LocalFrame oFrame1          = LocalFrame.oGetTranslatedFrame(m_oFirstInletFrame, (-0.2f) * m_oFirstInletFrame.vecGetLocalZ());
                LocalFrame oFrame2          = LocalFrame.oGetTranslatedFrame(m_oSecondInletFrame, (-0.2f) * m_oSecondInletFrame.vecGetLocalZ());
                LocalFrame oFrame3          = LocalFrame.oGetTranslatedFrame(m_oFirstOutletFrame, (-0.2f) * m_oFirstOutletFrame.vecGetLocalZ());
                LocalFrame oFrame4          = LocalFrame.oGetTranslatedFrame(m_oSecondOutletFrame, (-0.2f) * m_oSecondOutletFrame.vecGetLocalZ());
                ThreadCutter oCut1          = new ThreadCutter(oFrame1, fCutLength, fMaxRadius, fCoreRadius, 1.3f);
                ThreadCutter oCut2          = new ThreadCutter(oFrame2, fCutLength, fMaxRadius, fCoreRadius, 1.3f);
                ThreadCutter oCut3          = new ThreadCutter(oFrame3, fCutLength, fMaxRadius, fCoreRadius, 1.3f);
                ThreadCutter oCut4          = new ThreadCutter(oFrame4, fCutLength, fMaxRadius, fCoreRadius, 1.3f);
                aVoxelList.Add(oCut1.voxConstruct());
                aVoxelList.Add(oCut2.voxConstruct());
                aVoxelList.Add(oCut3.voxConstruct());
                aVoxelList.Add(oCut4.voxConstruct());

                Voxels voxScrews            = Sh.voxUnion(aVoxelList);
                Sh.PreviewVoxels(voxScrews, Cp.clrRed, 0.4f);
                return voxScrews;
            }
		}
	}
}