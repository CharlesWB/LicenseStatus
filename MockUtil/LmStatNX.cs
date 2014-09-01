// <copyright file="LmStatNX.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace MockUtil
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a NX lmstat report.
    /// </summary>
    public class LmStatNX : StatusWriter
    {
        /// <summary>
        /// Initializes a new instance of the LmStatNX class.
        /// </summary>
        public LmStatNX() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 27000;
            this.Vendor = "uglmd";
            this.Version = "v22.0";
        }

        /// <summary>
        /// Writes the lmstat NX report.
        /// </summary>
        /// <remarks>
        /// If ReportDate is not specified then today's date will be used.
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2006 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 10:43", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1},{0}@SERVER002,{0}@SERVER003", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\UGNX\UGNX.dat:", this.ServerName);
            this.WriteLine();
            this.WriteLine("{0}: license server UP (MASTER) v10.8", this.ServerName);
            this.WriteLine("SERVER002: license server UP v10.8");
            this.WriteLine("SERVER003: license server UP v10.8");
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("     {0}: UP v10.8", this.Vendor);
            this.WriteLine();
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("3d_to_2d_flattener", 1);
            this.WriteFeature("NX13100N", 1);

            this.WriteFeature("UG13500", 2, 1);
            this.WriteUser("steenwyk CAD9649D CAD9649D0.0", 3861, "7:13");
            this.WriteLine();

            this.WriteFeature("adv_assemblies", 7);
            this.WriteFeature("adv_sheet_metal_dsgn", 8);
            this.WriteFeature("ansys_meshers", 2);

            this.WriteFeature("assemblies", 16, 3);
            this.WriteUser("czuhai CAD9651D CAD9651D0.0", 2009, "10:21");
            this.WriteUser("deman CAD9210D CAD9210D0.0", 6118, "9:55");
            this.WriteUser("kreys CAD9654D CAD9654D0.0", 181, "10:06");
            this.WriteLine();

            this.WriteFeature("c_p_p_runtime", 20);
            this.WriteFeature("dotnet_runtime", 28);

            this.WriteFeature("drafting", 16, 8);
            this.WriteUser("boulter CAD9656D CAD9656D0.0", 6049, "10:13");
            this.WriteUser("ehlert CAD9647D CAD9647D0.0", 1884, "9:27");
            this.WriteUser("harkema CAD9698D CAD9698D0.0", 2658, "8:04");
            this.WriteUser("houtman CAD9690D CAD9690D0.0", 703, "8:45");
            this.WriteUser("morris CAD9648D CAD9648D0.0", 5533, "8:09");
            this.WriteUser("owczarzak CAD9692D CAD9692D0.0", 6825, "9:14");
            this.WriteUser("stiop CAD9575d CAD9575d0.0", 5980, "10:16");
            this.WriteUser("tasma CAD9696D CAD9696D0.0", 5347, "10:38");
            this.WriteLine();

            this.WriteFeature("dxf_to_ug", 1);
            this.WriteFeature("dxfdwg", 1);
            this.WriteFeature("features_modeling", 9);
            this.WriteFeature("flat_patterns", 1);
            this.WriteFeature("free_form_modeling", 9);

            this.WriteFeature("gateway", 28, 20);
            this.WriteUser("boulter CAD9656D CAD9656D0.0", 7244, "7:43");
            this.WriteUser("bozarth CAD9574D CAD9574D0.0", 2961, "8:44");
            this.WriteUser("czuhai CAD9651D CAD9651D0.0", 1128, "8:04");
            this.WriteUser("deboer CAD9668D CAD9668D0.0", 3318, "6:35");
            this.WriteUser("deman CAD9210D CAD9210D0.0", 7135, "7:03");
            this.WriteUser("ehlert CAD9647D CAD9647D0.0", 980, "6:43");
            this.WriteUser("harkema CAD9698D CAD9698D0.0", 6763, "7:09");
            this.WriteUser("houtman CAD9690D CAD9690D0.0", 3477, "7:30");
            this.WriteUser("karas CAD9701d CAD9701d0.0", 3782, "7:12");
            this.WriteUser("kreys CAD9654D CAD9654D0.0", 1989, "7:50");
            this.WriteUser("morris CAD9648D CAD9648D0.0", 6274, "8:05");
            this.WriteUser("moshauer CAD8272D CAD8272D0.0", 4876, "6:37");
            this.WriteUser("owczarzak CAD9692D CAD9692D0.0", 615, "9:12");
            this.WriteUser("schestag CAD8273D CAD8273D0.0", 5234, "7:48");
            this.WriteUser("sheffer CAD9653d CAD9653d0.0", 4069, "7:12");
            this.WriteUser("sternaman CAD9657D CAD9657D0.0", 6439, "7:32");
            this.WriteUser("stiop CAD9575d CAD9575d0.0", 838, "7:13");
            this.WriteUser("tasma CAD9696D CAD9696D0.0", 5758, "7:24");
            this.WriteUser("wetters CAD9699d CAD9699d0.0", 1409, "7:17");
            this.WriteUser("white CAD9695D CAD9695D0.0", 4257, "6:38");
            this.WriteLine();

            this.WriteFeature("gfem_ansys", 2);
            this.WriteFeature("gfem_nastran", 2);
            this.WriteFeature("grip_development", 99999);
            this.WriteFeature("grip_execute", 17);
            this.WriteFeature("iges", 1);
            this.WriteFeature("iman_1st", 1);
            this.WriteFeature("iman_ideas", 1);
            this.WriteFeature("iman_ug", 8);
            this.WriteFeature("iman_ug_manager", 1);
            this.WriteFeature("mechanisms", 3);
            this.WriteFeature("motion_dyn", 1);
            this.WriteFeature("motion_kin", 3);
            this.WriteFeature("nx_ansys_export", 1);
            this.WriteFeature("nx_freeform_1", 9);
            this.WriteFeature("nx_freeform_2", 9);
            this.WriteFeature("nx_nas_bn_basic_dsk", 4);
            this.WriteFeature("nx_nas_nonlin_dsk", 2);
            this.WriteFeature("nx_nastran_export", 4);
            this.WriteFeature("nx_nastran_import", 2);

            this.WriteFeature("nx_sheet_metal", 8, 1);
            this.WriteUser("steenwyk CAD9649D CAD9649D0.0", 6964, "10:40");
            this.WriteLine();

            this.WriteFeature("pstudio_cons", 28);
            this.WriteFeature("pv_ugdatagenerator", 28);
            this.WriteFeature("sheet_metal", 8);
            this.WriteFeature("sheet_metal_design", 8);

            this.WriteFeature("solid_modeling", 19, 11);
            this.WriteUser("czuhai CAD9651D CAD9651D0.0", 4423, "10:02");
            this.WriteUser("deboer CAD9668D CAD9668D0.0", 6614, "6:37");
            this.WriteUser("deman CAD9210D CAD9210D0.0", 1709, "10:35");
            this.WriteUser("karas CAD9701d CAD9701d0.0", 5144, "10:09");
            this.WriteUser("kreys CAD9654D CAD9654D0.0", 309, "10:25");
            this.WriteUser("moshauer CAD8272D CAD8272D0.0", 468, "8:57");
            this.WriteUser("schestag CAD8273D CAD8273D0.0", 3091, "7:48");
            this.WriteUser("sheffer CAD9653d CAD9653d0.0", 3106, "10:27");
            this.WriteUser("stiop CAD9575d CAD9575d0.0", 7338, "8:43");
            this.WriteUser("wetters CAD9699d CAD9699d0.0", 211, "8:30");
            this.WriteUser("white CAD9695D CAD9695D0.0", 3227, "6:38");
            this.WriteLine();

            this.WriteFeature("ufunc_execute", 20);
            this.WriteFeature("ug_collaborate", 28);
            this.WriteFeature("ug_nas_bn", 2);
            this.WriteFeature("ug_nas_des", 1);
            this.WriteFeature("ug_scenario", 2);
            this.WriteFeature("ug_smart_models", 28);
            this.WriteFeature("ug_strength_wizard", 1);
            this.WriteFeature("ug_struct_pe_solver", 2);
            this.WriteFeature("ug_to_dxf", 1);
            this.WriteFeature("ug_web_express", 1);
            this.WriteFeature("ugopen_menuscript", 21);
            this.WriteFeature("ugopen_plus_plus", 1);
            this.WriteFeature("ui_styler", 1);
            this.WriteFeature("usr_defined_features", 1);
            this.WriteFeature("visview_base", 1);
            this.WriteFeature("server_id", 1);
            this.WriteFeature("UG13500_assemblies", 2);
            this.WriteFeature("UG13500_c_p_p_runtime", 2);
            this.WriteFeature("UG13500_dotnet_runtime", 2);

            this.WriteFeature("UG13500_features_modeling", 2, 1);
            this.WriteUser("steenwyk CAD9649D CAD9649D0.0", 1550, "10:25");
            this.WriteLine();

            this.WriteFeature("UG13500_gateway", 2, 1);
            this.WriteUser("steenwyk CAD9649D CAD9649D0.0", 6586, "7:13");
            this.WriteLine();

            this.WriteFeature("UG13500_grip_execute", 2);
            this.WriteFeature("UG13500_id_ext_fem_beam", 2);
            this.WriteFeature("UG13500_iges", 2);
            this.WriteFeature("UG13500_nx_freeform_1", 2);
            this.WriteFeature("UG13500_nx_freeform_2", 2);
            this.WriteFeature("UG13500_nx_ftk", 2);
            this.WriteFeature("UG13500_nx_masterfem", 2);
            this.WriteFeature("UG13500_nx_material_system", 2);
            this.WriteFeature("UG13500_nx_spsd_stress", 2);
            this.WriteFeature("UG13500_nx_spsd_vibration", 2);
            this.WriteFeature("UG13500_pcf_package_file", 2);
            this.WriteFeature("UG13500_pstudio_cons", 2);
            this.WriteFeature("UG13500_pv_ugdatagenerator", 2);
            this.WriteFeature("UG13500_sla_3d_systems", 2);

            this.WriteFeature("UG13500_solid_modeling", 2, 1);
            this.WriteUser("steenwyk CAD9649D CAD9649D0.0", 5653, "10:19");
            this.WriteLine();

            this.WriteFeature("UG13500_step_ap203", 2);
            this.WriteFeature("UG13500_step_ap214", 2);
            this.WriteFeature("UG13500_studio_render", 2);
            this.WriteFeature("UG13500_studio_visualize", 2);
            this.WriteFeature("UG13500_ufunc_execute", 2);
            this.WriteFeature("UG13500_ug_checkmate", 2);
            this.WriteFeature("UG13500_ug_collaborate", 2);
            this.WriteFeature("UG13500_ug_kf_checker", 2);
            this.WriteFeature("UG13500_ug_kf_execute", 2);
            this.WriteFeature("UG13500_ug_smart_models", 2);
            this.WriteFeature("UG13500_ug_web_express", 2);
            this.WriteFeature("UG13500_ugopen_menuscript", 2);
            this.WriteFeature("UG13500_usr_defined_features", 2);
            this.WriteFeature("NX13100N_3d_to_2d_flattener", 1);
            this.WriteFeature("NX13100N_adv_assemblies", 1);
            this.WriteFeature("NX13100N_assemblies", 1);
            this.WriteFeature("NX13100N_design_studio", 1);
            this.WriteFeature("NX13100N_dotnet_runtime", 1);
            this.WriteFeature("NX13100N_drafting", 1);
            this.WriteFeature("NX13100N_dxf_to_ug", 1);
            this.WriteFeature("NX13100N_dxfdwg", 1);
            this.WriteFeature("NX13100N_features_modeling", 1);
            this.WriteFeature("NX13100N_free_form_modeling", 1);
            this.WriteFeature("NX13100N_gateway", 1);
            this.WriteFeature("NX13100N_geometric_tol", 1);
            this.WriteFeature("NX13100N_grip_execute", 1);
            this.WriteFeature("NX13100N_iges", 1);
            this.WriteFeature("NX13100N_nc_external_program", 1);
            this.WriteFeature("NX13100N_nx_flexible_pcb", 1);
            this.WriteFeature("NX13100N_nx_freeform_1", 1);
            this.WriteFeature("NX13100N_nx_freeform_2", 1);
            this.WriteFeature("NX13100N_nx_nastran_export", 1);
            this.WriteFeature("NX13100N_nx_sheet_metal", 1);
            this.WriteFeature("NX13100N_nx_spsd_stress", 1);
            this.WriteFeature("NX13100N_nx_spsd_vibration", 1);
            this.WriteFeature("NX13100N_pcf_package_file", 1);
            this.WriteFeature("NX13100N_pstudio_cons", 1);
            this.WriteFeature("NX13100N_pv_ugdatagenerator", 1);
            this.WriteFeature("NX13100N_sla_3d_systems", 1);
            this.WriteFeature("NX13100N_solid_modeling", 1);
            this.WriteFeature("NX13100N_step_ap203", 1);
            this.WriteFeature("NX13100N_step_ap214", 1);
            this.WriteFeature("NX13100N_studio_analyze", 1);
            this.WriteFeature("NX13100N_studio_free_form", 1);
            this.WriteFeature("NX13100N_studio_render", 1);
            this.WriteFeature("NX13100N_studio_visualize", 1);
            this.WriteFeature("NX13100N_ug_checkmate", 1);
            this.WriteFeature("NX13100N_ug_collaborate", 1);
            this.WriteFeature("NX13100N_ug_kf_checker", 1);
            this.WriteFeature("NX13100N_ug_kf_execute", 1);
            this.WriteFeature("NX13100N_ug_nas_des", 1);
            this.WriteFeature("NX13100N_ug_opt_wizard", 1);
            this.WriteFeature("NX13100N_ug_prod_des_advisor", 1);
            this.WriteFeature("NX13100N_ug_smart_models", 1);
            this.WriteFeature("NX13100N_ug_to_dxf", 1);
            this.WriteFeature("NX13100N_ug_web_express", 1);
            this.WriteFeature("NX13100N_ugopen_menuscript", 1);
            this.WriteFeature("NX13100N_usr_defined_features", 1);
        }
    }
}
