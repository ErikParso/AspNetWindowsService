using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace AspWinService.Services
{
    public sealed class RuntimeVersionDetectorService
    {
        // zdroj informací: http://support.microsoft.com/kb/318785

        // Dodatek co microsoft neříká!
        // FW 2.0 SP 1 na WindowsXP a má jako verzi .net 2.2.30729
        // FW 2.0 SP 1 na Windows Vista a Windows 7 má jako verzi .net 2.0.50727.1433
        // FW 2.0 je na Windows 2003 nainstalován bez příznaku Install=1  !

        public sealed class RuntimeVersion
        {
            /// <summary>
            /// CZ: Zjištěná verze .net runtime
            /// </summary>
            public readonly Version Version;
            /// <summary>
            /// CZ: Aplikovaný ServicePack
            /// </summary>
            public readonly int? ServicePack;
            /// <summary>
            /// CZ: Instalovaný klientský profile?
            /// </summary>
            public readonly bool IsClientProfile;
            /// <summary>
            /// CZ: Instalovaný plný profil?
            /// </summary>
            public readonly bool IsFullProfile;

            public RuntimeVersion(Version version, int? servicePack, bool isClientProfile, bool isFullProfile)
            {
                Version = version;
                ServicePack = servicePack;
                IsClientProfile = isClientProfile;
                IsFullProfile = isFullProfile;
            }
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována některá z kompatibilních verzí.</returns>
        public bool CheckRuntimeVersion(Version version)
        {
            RuntimeVersion rv = new RuntimeVersion(version, null, true, true); //kontroluji jen verzi
            return _CheckRuntimeVersion(rv);
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework s aplikovaným, nebo vyšším service packem
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <param name="servicePack">Požadována určitá verze instalovaného ServicePack</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována některá z kompatibilních verzí. Zároveň je stejný a nebo vyšší ServicePack.</returns>
        public bool CheckRuntimeVersion(Version version, int servicePack)
        {
            RuntimeVersion rv = new RuntimeVersion(version, servicePack, true, true); //kontroluji verzi a instalovaný SP
            return _CheckRuntimeVersion(rv);
        }

        /// <summary>
        /// CZ: Ověření zda je nainstalována podporovaná verze .NET Framework s aplikovaným, nebo vyšším service packem a yda je instalov8n jen klientský profil namísto plného.
        /// </summary>
        /// <param name="version">Podporovaná verze rozhraní</param>
        /// <param name="servicePack">Požadována určitá verze instalovaného ServicePack</param>
        /// <param name="needOnlyClientProfile">True pokud požaduji jen instalovaný klientský profil. False pokud stačí, aby byl nainstalován jen plný profil.</param>
        /// <returns>True pokud je platforma nainstalována, nebo je nainstalována, některá z kompatibilních verzí. Zároveň je stejný a nebo vyšší ServicePack a je instalován požadovaný profil.</returns>
        public bool CheckRuntimeVersion(Version version, int servicePack, bool needOnlyClientProfile)
        {
            RuntimeVersion rv = new RuntimeVersion(version, servicePack, needOnlyClientProfile, !needOnlyClientProfile); //kontroluji verzi a instalovaný SP a vyzaduji pouze klientský profil
            return _CheckRuntimeVersion(rv);
        }

        private bool _CheckRuntimeVersion(RuntimeVersion rv)
        {
            bool versionFound = false;
            List<RuntimeVersion> installedVersions = GetRuntimeInstalledVersions();
            foreach (RuntimeVersion v in installedVersions)
            {
                versionFound =
                    (v.Version.Major == rv.Version.Major && v.Version.Minor >= rv.Version.Minor) && // shodnost Major a Minor stejný a větší
                    (rv.Version.Build == -1 || v.Version.Minor > rv.Version.Minor || v.Version.Build >= rv.Version.Build) && // pokud je zadaný build a nebo je vetsi minor nebo je build vetsi nez pozadovany
                    (rv.Version.Revision == -1 || v.Version.Minor > rv.Version.Minor || v.Version.Revision >= rv.Version.Revision) && // pokud je zadaná revize a nebo je vetsi minor a nebo je revize vetsi nez pozadovana
                    (rv.ServicePack == null || v.ServicePack >= rv.ServicePack) && // service pack je zadaný a instalovaný je stejný a nebo větší
                    (rv.IsClientProfile == false || rv.IsClientProfile == v.IsClientProfile) && // je pozadovana instalace klient profile (FW 4.0 a vyšší) a opravdu tomu tak je
                    (rv.IsFullProfile == false || rv.IsFullProfile == v.IsFullProfile);// je pozadovaná instalace full profile (FW 4.0 a vyšší) a opravdu tomu tak je
                if (versionFound) // uz jsem nasel verzi, kterou hledam
                    break;
            }
            return versionFound;
        }

        /// <summary>
        /// CZ: Zjištění instalovaných verzí runtime .NET Framework
        /// </summary>
        /// <returns></returns>
        public List<RuntimeVersion> GetRuntimeInstalledVersions()
        {
            // algoritmus neumi zpracovat FW 1.0 a FW 1.1 nicméně ty nepotřebne takže to nevadí
            // zdroj informací: http://support.microsoft.com/kb/318785/en-us

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(
                "Software\\Microsoft\\NET Framework Setup\\NDP", false); // jen pro cteni, abych nepotreboval UAC elevation
            List<RuntimeVersion> versions = new List<RuntimeVersion>();
            foreach (string key in regKey.GetSubKeyNames())
            {
                if (key.StartsWith("v") && key.Length > 1 && char.IsNumber(key[1]))
                {
                    Microsoft.Win32.RegistryKey fwRegKey = regKey.OpenSubKey(key);
                    bool installed = (int)fwRegKey.GetValue("Install", 0) == 1;
                    if (installed)
                    {// daný framework je nainstalován
                        string vString = (string)fwRegKey.GetValue("Version", string.Empty);
                        if (!string.IsNullOrEmpty(vString))
                        {
                            Version v = new Version(vString);
                            int? sp = (int)fwRegKey.GetValue("SP", -1);
                            if (sp.Value == -1)
                                sp = null;

                            RuntimeVersion version = new RuntimeVersion(v, sp, true, true);
                            versions.Add(version);
                        }
                        else
                        {// nektere instalace nemaji vyplnenu verzi, ale jen increment a nekdy ani to, proto je tu tento blok
                            int? sp = (int)fwRegKey.GetValue("SP", -1);
                            if (sp.Value == -1)
                                sp = null;

                            string incrementString = (string)fwRegKey.GetValue("increment", string.Empty);
                            Version v = new Version(key.Substring(1) + (string.IsNullOrEmpty(incrementString) ? string.Empty : "." + incrementString));
                            RuntimeVersion version = new RuntimeVersion(v, sp, true, true);
                            versions.Add(version);
                        }
                    }
                    else
                    {// pro framework 4.0 a vyšší platí, že je distribuován jako dvě podverze Client profile a Full profile. V systému může nastat situace, že bude k dispozici jen jeden profil
                        Microsoft.Win32.RegistryKey fwRegClientKey = fwRegKey.OpenSubKey("Client");
                        Microsoft.Win32.RegistryKey fwRegFullKey = fwRegKey.OpenSubKey("Full");

                        bool installedClient = fwRegClientKey != null ? (int)fwRegClientKey.GetValue("Install", 0) == 1 : false;
                        bool installedFull = fwRegFullKey != null ? (int)fwRegFullKey.GetValue("Install", 0) == 1 : false;
                        if (installedClient || installedFull)
                        {
                            fwRegKey = installedClient ? fwRegClientKey : fwRegFullKey;
                            string vString = (string)fwRegClientKey.GetValue("Version", string.Empty);
                            if (!string.IsNullOrEmpty(vString))
                            {
                                Version v = new Version(vString);
                                int? sp = (int)fwRegKey.GetValue("SP", -1);
                                if (sp.Value == -1)
                                    sp = null;

                                RuntimeVersion version = new RuntimeVersion(v, sp, installedClient, installedFull);
                                versions.Add(version);
                            }
                        }
                    }
                }
            }
            return versions;
        }
    }
}
