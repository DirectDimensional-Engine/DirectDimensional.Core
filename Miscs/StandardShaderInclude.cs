using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using DirectDimensional.Bindings.D3DCompiler;
using System.Text;

namespace DirectDimensional.Core.Miscs {
    internal sealed class StandardShaderInclude : Include {
        public static StandardShaderInclude? Instance { get; private set; }

        public static void UpdateInstance(string? local, string? system) {
            Instance = new(local, system);
        }

        public string? LocalDirectory { get; private set; }
        public string? SystemDirectory { get; private set; }

        public StandardShaderInclude(string? localDir, string? systemDir) {
            if (!Directory.Exists(localDir)) {
                throw new DirectoryNotFoundException("Cannot use shader local include directory '" + localDir + "' as the directory doesn't exists");
            }

            if (!Directory.Exists(systemDir)) {
                throw new DirectoryNotFoundException("Cannot use shader system include directory '" + systemDir + "' as the directory doesn't exists");
            }

            LocalDirectory = localDir;
            SystemDirectory = systemDir;
        }

        public override HRESULT Open(D3D_INCLUDE_TYPE includeType, string fileName, IntPtr pParentData, IntPtr ppData, IntPtr pBytes) {
            try {
                string path = includeType switch {
                    D3D_INCLUDE_TYPE.System => Path.Combine(SystemDirectory ?? string.Empty, fileName),
                    _ => Path.Combine(LocalDirectory ?? string.Empty, fileName),
                };

                var content = File.ReadAllText(path);

                Marshal.WriteInt32(pBytes, content.Length);
                Marshal.WriteIntPtr(ppData, Marshal.StringToHGlobalAnsi(content));

                return HRESULTCodes.S_OK;
            } catch (Exception e) {
                return e.HResult;
            }
        }

        public override HRESULT Close(IntPtr pData) {
            Marshal.FreeHGlobal(pData);

            return HRESULTCodes.S_OK;
        }
    }
}
