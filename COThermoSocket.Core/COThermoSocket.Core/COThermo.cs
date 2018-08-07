using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAPEOPEN110;
using System.ComponentModel;

namespace COThermoSocket.Core
{
    public enum COVersion
    {
        //[ObsoleteAttribute("Not implemented Cape-Open 1.0.", true)]
        //V100,
        V110
    }

    public class PropPkgMgr
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string About { get; set; }
        public string VenderURL { get; set; }
        public string CapeVersion { get; set; }
        public string ComponentVersion { get; set; }
        public string ProgID { get; set; }

        public COVersion? Verson { get; set; }
    }

    public class COThermo
    {

        #region Singleton Pattern

        private COThermo(COVersion version)
        {
            Version = version;
        }

        private static COThermo _COThermo;
        public static COThermo GetCOThermo(COVersion version = COVersion.V110)
        {
            if (_COThermo == null)
            {
                _COThermo = new COThermo(version);
            }
            return _COThermo;
        }

        #endregion

        #region Properties

        #region Version
        private COVersion? version = null;
        public COVersion? Version
        {
            get
            {
                return version;
            }
            set
            {
                if (version != value & value != null)
                {
                    version = value;
                    RefreshPropPkgMgrs();
                }
            }
        }
        #endregion

        #region PropPkgMgrs
        public PropPkgMgr[] PropPkgMgrs { get; private set; }
        #endregion

        #region PropPkgMgr
        private PropPkgMgr propPkgMgr = null;
        public PropPkgMgr PropPkgMgr
        {
            get
            {
                return propPkgMgr;
            }
            set
            {
                if (propPkgMgr != value && value != null && PropPkgMgrs.Contains(value))
                {
                    propPkgMgr = value;
                    RefreshPropPkgNames();
                }
            }
        }
        #endregion

        #region PropPkgNames
        public string[] PropPkgNames { get; private set; }
        #endregion

        #region PropPkgName
        private string propPkgName = null;
        public string PropPkgName
        {
            get
            {
                return propPkgName;
            }
            set
            {
                if (propPkgName != value && value != null && PropPkgNames.Contains(value))
                {
                    propPkgName = value;
                    RefreshPropertyPackage(propPkgName);
                }
            }
        }
        #endregion

        #region MaterialObject
        private MaterialObjet materialObject;
        public MaterialObjet MaterialObject
        {
            get
            {
                if (materialObject == null)
                {
                    materialObject = new MaterialObjet(_pp);
                }
                return materialObject;
            }
        }
        #endregion

        #endregion

        #region Methods

        private void RefreshPropPkgMgrs()
        {
            
            Dictionary<COVersion, string> categoryIDs = new Dictionary<COVersion, string>
                {
                    //{ COThermoVerson.V100, "{678c09a3-7d66-11d2-a67d-00105a42887f}" },

                    //CO_Thermo_1.1_Specification_311.pdf, P35
                    { COVersion.V110, "{CF51E383-0110-4ed8-ACB7-B50CFDE6908E}" }
                };

            List<PropPkgMgr> pkgmgrs = new List<PropPkgMgr>();
            RegistryKey classesRoot = Registry.ClassesRoot;
            foreach (var subkey in classesRoot.OpenSubKey("CLSID", false).GetSubKeyNames())
            {
                if (classesRoot.OpenSubKey(string.Join(@"\", "CLSID", subkey, "Implemented Categories", categoryIDs[(COVersion)Version]), false) != null)
                {
                    RegistryKey propPkgMgr = classesRoot.OpenSubKey("CLSID", false).OpenSubKey(subkey, false);
                    RegistryKey capeDescription = propPkgMgr.OpenSubKey("CapeDescription", false);
                    pkgmgrs.Add(new PropPkgMgr()
                    {
                        Name = capeDescription.GetValue("Name").ToString(),
                        Description = capeDescription.GetValue("Description").ToString(),
                        About = capeDescription.GetValue("About").ToString(),
                        VenderURL = capeDescription.GetValue("VendorURL").ToString(),
                        CapeVersion = capeDescription.GetValue("CapeVersion").ToString(),
                        ComponentVersion = capeDescription.GetValue("ComponentVersion").ToString(),
                        ProgID = propPkgMgr.OpenSubKey("ProgID").GetValue("").ToString(),
                        Verson = Version
                    });
                }
            }
            PropPkgMgrs = pkgmgrs.ToArray();
        }

        private void RefreshPropPkgNames()
        {
            _ppm = Activator.CreateInstance(Type.GetTypeFromProgID(PropPkgMgr.ProgID));
            ICapeUtilities ppm = _ppm as ICapeUtilities;
            if (ppm != null)
            {
                try
                {
                    ppm.Initialize();
                }
                catch (Exception)
                {
                    ECapeUser eCapeUser = _ppm as ECapeUser;
                    throw new Exception("Failed to initialize ICapeUtilities: " + eCapeUser.description);
                }
            }

            //if (Version == COThermoVerson.V100)
            //{
            //    PropPkgNames = (_ppm as ICapeThermoSystem).GetPropertyPackages() as string[];
            //}
            if (Version == COVersion.V110)
            {
                PropPkgNames = (_ppm as ICapeThermoPropertyPackageManager).GetPropertyPackageList() as string[];
            }
        }

        private void RefreshPropertyPackage(string propPkgName)
        {
            //if (Version == COThermoVerson.V100)
            //{
            //    _pp = (_ppm as ICapeThermoSystem).ResolvePropertyPackage(propPkgName);
            //}
            if (Version == COVersion.V110)
            {
                _pp = (_ppm as ICapeThermoPropertyPackageManager).GetPropertyPackage(propPkgName);
            }
            //_mo = new MaterialObjet(_pp);
            //// SetMaterial calls _mo.GetCompoundList(o, o, o, o, o, o) to check compounds material can be indentified by the package
            //(_pp as ICapeThermoMaterialContext).SetMaterial(_mo);
        }

        #endregion

        private dynamic _ppm, _pp;




    }
}
