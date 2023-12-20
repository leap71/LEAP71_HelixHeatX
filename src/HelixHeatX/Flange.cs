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
            /// Generates the large flange on the bottom of the cube.
            /// Including screw holes.
            /// </summary>
            protected void GetFlange(
                out Voxels voxFlange,
                out Voxels voxScrewHoles,
                out Voxels voxScrewCutter)
            {
                float fCoreRadius           = 5f;
                float fMaxRadius            = 6f;
                float fCutLength            = 24;
                float fScrewThreadRadius    = 3.5f;
                float fScrewThreadLength    = 2f;
                float fScrewHeadRadius      = 7f;
                float fScrewHeadLength      = 10f;
                List<float> aXValues        = new List<float>() { -60f, 60f };
                List<float> aYValues        = new List<float>() { -38f, 0f, 38f };
                List<Voxels> aFlangeList    = new List<Voxels>();
                List<Voxels> aCutterList    = new List<Voxels>();
                List<Voxels> aScrewList     = new List<Voxels>();

                foreach (float fX in aXValues)
                {
                    foreach (float fY in aYValues)
                    {
                        Vector3 vecPt           = new Vector3(fX, fY, 0f);
                        ScrewHole oScrewHole    = new ScrewHole(new LocalFrame(vecPt + 6f * Vector3.UnitZ), fScrewThreadLength, fScrewThreadRadius, fScrewHeadLength, fScrewHeadRadius);
                        aScrewList.Add(oScrewHole.voxConstruct());

                        BaseCylinder oCyl       = new BaseCylinder(new LocalFrame(vecPt), 8f, fScrewHeadRadius + 5f);
                        aFlangeList.Add(oCyl.voxConstruct());

                        ThreadCutter oCutter    = new ThreadCutter(new LocalFrame(vecPt - 10f * Vector3.UnitZ), fCutLength, fMaxRadius, fCoreRadius, 1.3f);
                        aCutterList.Add(oCutter.voxConstruct());
                    }
                }
                voxScrewHoles   = Sh.voxUnion(aScrewList);
                voxScrewCutter  = Sh.voxUnion(aCutterList);
                voxFlange       = Sh.voxUnion(aFlangeList);
            }
		}
	}
}