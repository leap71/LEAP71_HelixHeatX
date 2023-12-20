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
            protected enum EFluid   { COOL, HOT };
            protected LocalFrame    m_oFirstInletFrame;
            protected LocalFrame    m_oSecondInletFrame;
            protected LocalFrame    m_oFirstOutletFrame;
            protected LocalFrame    m_oSecondOutletFrame;
            protected LocalFrame    m_oCentreBottomFrame;
            protected float         m_fIORadius;
            protected Voxels        m_voxBounding;
            protected float         m_fPlateThickness;
            protected float         m_fWallThickness;


            /// <summary>
            /// Task function that instantiates a new helix heat exchanger object
            /// and calls its construction logic.
            /// Please use this function as the entry point for the PicoGK library.
            /// </summary>
            public static void Task()
            {
                Library.Log("Starting Task.");

                HelixHeatX oCoolCube = new HelixHeatX();
                oCoolCube.voxConstruct();

                Library.Log("Finished Task successfully.");
            }

            /// <summary>
            /// Instantiates a new object of the type HelixHeatX.
            /// It uses preset defaults for the relevant parameters.
            /// </summary>
            protected HelixHeatX()
            {
                BaseBox oInnerBox           = new BaseBox(new LocalFrame(), 100, 100, 100);
                float fHalfIOLengthSpacing  = 75f;
                float fHalfIOWidthSpacing   = 26.5f;
                m_oFirstInletFrame          = new LocalFrame(new Vector3(-fHalfIOLengthSpacing,     -fHalfIOWidthSpacing,   50), -Vector3.UnitX);
                m_oSecondInletFrame         = new LocalFrame(new Vector3(-fHalfIOLengthSpacing,     fHalfIOWidthSpacing,    50), -Vector3.UnitX);
                m_oFirstOutletFrame         = new LocalFrame(new Vector3(fHalfIOLengthSpacing,      -fHalfIOWidthSpacing,   50), Vector3.UnitX);
                m_oSecondOutletFrame        = new LocalFrame(new Vector3(fHalfIOLengthSpacing,      fHalfIOWidthSpacing,    50), Vector3.UnitX);
                BaseCylinder oFirstInlet    = new BaseCylinder(m_oFirstInletFrame,      12, 7);
                BaseCylinder oSecondInlet   = new BaseCylinder(m_oSecondInletFrame,     12, 7);
                BaseCylinder oFirstOutlet   = new BaseCylinder(m_oFirstOutletFrame,     12, 7);
                BaseCylinder oSecondOutlet  = new BaseCylinder(m_oSecondOutletFrame,    12, 7);
                Sh.PreviewBoxWireframe(         oInnerBox,      Cp.clrBlack);
                Sh.PreviewCylinderWireframe(    oFirstInlet,    Cp.clrRed);
                Sh.PreviewCylinderWireframe(    oSecondInlet,   Cp.clrRed);
                Sh.PreviewCylinderWireframe(    oFirstOutlet,   Cp.clrRed);
                Sh.PreviewCylinderWireframe(    oSecondOutlet,  Cp.clrRed);


                //inputs
                m_oCentreBottomFrame        = new LocalFrame(new Vector3(-50, 0, 50), Vector3.UnitX, Vector3.UnitZ);
                BaseBox oOuterBox           = new BaseBox(new LocalFrame(new Vector3(0, 0, -4)), 107, 2f * fHalfIOLengthSpacing + 24f, 104);
                m_voxBounding               = oOuterBox.voxConstruct();
                Sh.PreviewBoxWireframe(oOuterBox, Cp.clrBlack);
                m_fPlateThickness           = 3.5f;
                m_fWallThickness            = 0.8f;
                m_fIORadius                 = 7f;
            }

            /// <summary>
            /// This function holds the construction logic of the geometry,
            /// combining all sub-components into one part and exporting the final print file at the end.
            /// Generates screenshots during the generation sequence that will be saved to the specified output folder.
            /// </summary>
            protected Voxels voxConstruct()
            {
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_00"));

                Voxels voxHotCornerFins     = voxGetTurningFins(EFluid.HOT);
                Voxels voxCoolCornerFins    = voxGetTurningFins(EFluid.COOL);
                Voxels voxAllCornerFins     = Sh.voxUnion(voxHotCornerFins, voxCoolCornerFins);
                Sh.PreviewVoxels(voxAllCornerFins, Cp.clrWarning, 0.5f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_01"));


                Voxels voxHotStraightFins   = voxGetStraightFins(EFluid.HOT);
                Voxels voxCoolStraightFins  = voxGetStraightFins(EFluid.COOL);
                Voxels voxAllStraightFins   = Sh.voxUnion(voxHotStraightFins, voxCoolStraightFins);
                Sh.PreviewVoxels(voxAllStraightFins, Cp.clrToothpaste, 0.5f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_02"));


                Voxels voxFins              = Sh.voxUnion(voxAllCornerFins, voxAllStraightFins);
                Sh.PreviewVoxels(voxFins, Cp.clrRock);


                Voxels voxStructure         = voxGetOuterStructure();
                Sh.PreviewVoxels(voxStructure, Cp.clrRock, 0.3f);


                GetHelicalVoid(
                    out Voxels voxHotFluidVoid,
                    out Voxels voxHotFluidSplitters,
                    EFluid.HOT);


                GetHelicalVoid(
                    out Voxels voxCoolFluidVoid,
                    out Voxels voxCoolFluidSplitters,
                    EFluid.COOL);


                voxHotFluidVoid         = Sh.voxSubtract(voxHotFluidVoid, Sh.voxOffset(voxCoolFluidVoid, m_fWallThickness));
                voxCoolFluidVoid        = Sh.voxSubtract(voxCoolFluidVoid, Sh.voxOffset(voxHotFluidVoid, m_fWallThickness));
                Sh.PreviewVoxels(voxHotFluidVoid, Cp.clrPitaya, 0.5f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_03"));
                Sh.PreviewVoxels(voxCoolFluidVoid, Cp.clrFrozen, 0.5f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_04"));


                Voxels voxInnerVolume   = Sh.voxUnion(voxHotFluidVoid, voxCoolFluidVoid);
                Voxels voxSplitters     = Sh.voxUnion(voxHotFluidSplitters, voxCoolFluidSplitters);
                Voxels voxOuterVolume   = Sh.voxOffset(voxInnerVolume, 0.9f);


                GetFlange(
                    out Voxels voxFlange,
                    out Voxels voxScrewHoles,
                    out Voxels voxFlangeScrewCutters);
                Sh.PreviewVoxels(voxScrewHoles, Cp.clrRed);
                Sh.PreviewVoxels(voxFlangeScrewCutters, Cp.clrRed, 0.4f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_05"));


                voxFlange               = Sh.voxOverOffset(voxFlange, 5f, 0f);
                voxFlange               = Sh.voxSmoothen(voxFlange, 0.5f);
                Sh.PreviewVoxels(voxFlange, Cp.clrGray, 0.6f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_06"));


                voxOuterVolume          = Sh.voxUnion(voxOuterVolume, voxFlange);
                voxOuterVolume          = Sh.voxUnion(voxOuterVolume, voxGetIOSupports());
                voxOuterVolume          = Sh.voxOverOffset(voxOuterVolume, 5f, 0f);
                voxOuterVolume          = Sh.voxSmoothen(voxOuterVolume, 0.5f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_07"));


                AddCentrePiece(
                    ref voxOuterVolume);


                voxOuterVolume          = Sh.voxUnion(voxOuterVolume, voxStructure);
                voxOuterVolume          = Sh.voxSubtract(voxOuterVolume, voxScrewHoles);
                voxOuterVolume          = Sh.voxExtrudeZSlice(voxOuterVolume, 4f, -4f);
                voxOuterVolume          = Sh.voxSubtract(voxOuterVolume, voxGetPrintWeb());

                Voxels voxResult        = Sh.voxSubtract(voxOuterVolume, voxInnerVolume);
                voxResult               = Sh.voxUnion(voxResult, voxFins);
                voxResult               = Sh.voxUnion(voxResult, voxSplitters);
                voxResult               = Sh.voxIntersect(voxResult, m_voxBounding);

                Voxels voxThreads       = voxGetIOThreads();
                voxResult               = Sh.voxUnion(voxResult, voxThreads);
                voxResult               = Sh.voxSubtract(voxResult, voxGetIOCuts());


                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_08"));
                Uf.Wait(0.1f);
                Library.oViewer().RemoveAllObjects();
                Sh.PreviewVoxels(voxResult, Cp.clrRock);
                Uf.Wait(0.1f);
                Library.oViewer().RequestScreenShot(Sh.strGetExportPath(Sh.EExport.TGA, $"Screenshot_09"));


                Sh.ExportVoxelsToSTLFile(voxResult, Sh.strGetExportPath(Sh.EExport.STL, "HelixHeatX"));
                return voxResult;
            }
        }
    }
}