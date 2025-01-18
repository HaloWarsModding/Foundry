using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    public static class ERA
    {
        public static void ExpandERA(string eraPath, string outputDir)
        {
//            using (KSoft.Phoenix.Resource.EraFileExpander expander = new KSoft.Phoenix.Resource.EraFileExpander(eraPath))
//            {
//                expander.Options = new KSoft.Collections.BitVector32();
//                expander.Options.Set(KSoft.Phoenix.Resource.EraFileUtilOptions.x64);

//                expander.ExpanderOptions = new KSoft.Collections.BitVector32();
//                expander.ExpanderOptions.Set(KSoft.Phoenix.Resource.EraFileExpanderOptions.Decrypt);
//                expander.ExpanderOptions.Set(KSoft.Phoenix.Resource.EraFileExpanderOptions.DontOverwriteExistingFiles);
//                //expander.ExpanderOptions.Set(KSoft.Phoenix.Resource.EraFileExpanderOptions.DontLoadEntireEraIntoMemory);
//                expander.ProgressOutput = null;
//                expander.VerboseOutput = null;
//                expander.DebugOutput = null;

//#if DEBUG
//                expander.ProgressOutput = Console.Out;
//#endif

//                expander.Read();
//                expander.ExpandTo(outputDir, Path.GetFileNameWithoutExtension(eraPath));
//            }

//            GC.Collect();
        }
    }
}
