using System;
using System.Collections.Generic;
using System.IO;
using LuaSTGEditorSharp.EditorData.Document;

namespace LuaSTGEditorSharp.EditorData.Compile
{
    /// <summary>
    ///     The <see cref="CompileProcess" /> of part of a luastg project.
    /// </summary>
    internal class PartialProjectProcess : CompileProcess
    {
        /// <summary>
        ///     The reference of parent <see cref="ProjectProcess" /> of parent <see cref="ProjectData" />.
        /// </summary>
        internal ProjectProcess parentProcess;

        public override string TargetPath => parentProcess.TargetPath;

        /// <summary>
        ///     Execute the <see cref="CompileProcess" />.
        /// </summary>
        /// <param name="SCDebug">Whether SCDebug is switched on.</param>
        /// <param name="StageDebug">Whether Stage Debug is switched on.</param>
        /// <param name="appSettings">App that contains settings</param>
        public override void ExecuteProcess(bool SCDebug, bool StageDebug, IAppSettings appSettings)
        {
            ExecuteProcess(SCDebug, StageDebug, appSettings, "");
        }

        /// <summary>
        ///     Execute the <see cref="CompileProcess" />.
        /// </summary>
        /// <param name="SCDebug">Whether SCDebug is switched on.</param>
        /// <param name="StageDebug">Whether Stage Debug is switched on.</param>
        /// <param name="appSettings">App that contains settings</param>
        public override void ExecuteProcess(bool SCDebug, bool StageDebug, IAppSettings appSettings,
            string directory = "", string filename = "")
        {
            GetPacker(appSettings, directory, filename);

            GenerateCode(SCDebug, StageDebug);

            //Gather file need to pack
            var resNeedToPack = new Dictionary<string, string>();
            var resPathToMD5 = new Dictionary<string, Tuple<string, string>>();

            if (appSettings.PackResources)
            {
                if (appSettings.SaveResMeta)
                {
                    if (File.Exists(projMetaPath) && Packer.TargetExists())
                        GatherResByResMeta(resNeedToPack, resPathToMD5);
                    else
                        GatherResAndSaveMeta(resNeedToPack);
                }
                else
                {
                    GatherAllRes(resNeedToPack);
                }
            }
            else
            {
                if (File.Exists(projMetaPath)) File.Delete(projMetaPath);
            }

            PackFileUsingInfo(appSettings, resNeedToPack, resPathToMD5, false, true);
        }
    }
}