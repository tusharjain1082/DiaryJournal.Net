using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SharpConfig;
using System.IO;

namespace DiaryJournal.Net
{
    public class myConfig
    {
        // both db contexts must be stored right here.
        public SingleFileDBContext ctx0 = new SingleFileDBContext();
        public OpenFSDBContext ctx1 = new OpenFSDBContext();
        public long totalNodes = 0;

        // 2 flags by which the active opened db is identified.
        public bool radCfgUseOpenFileSystemDB = false;
        public bool radCfgUseSingleFileDB = true;

        public bool chkCfgAutoLoadCreateDefaultDB = true;
        public int cmbCfgRtbEntryRMValue = 800;//1500;
        public int cmbCfgRtbViewEntryRMValue = 800;//1500;
        public bool chkCfgUseWinUserDocFolder = true;
        public String configFilePath = "";

        public void close()
        {
            ctx0.close();
            ctx1.close();
        }
    }
    public enum DatabaseType : byte
    {
        SingleFileDB = 0,
        OpenFSDB = 1
    }

    public static class myConfigMethods
    {
        public const string myConfigFileName = "myConfig.cfg";

        public static String getConfigPathFile()
        {
            return Path.Combine(Application.StartupPath, Path.GetFileName(myConfigFileName));
        }
        public static bool saveConfigFile(String file, ref myConfig cfg, bool initNewConfig = false)
        {
            if (initNewConfig)
                cfg = new myConfig();

            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch
            {
                return false;
            }

            try
            {
                // Create the configuration.
                Configuration myConfig = new Configuration();
                Section config1V1000 = new Section("Config1Version1.0.0.0");
                config1V1000.Add(new Setting("chkCfgAutoLoadCreateDefaultDB", cfg.chkCfgAutoLoadCreateDefaultDB));
                config1V1000.Add(new Setting("cmbCfgRtbEntryRMValue", cfg.cmbCfgRtbEntryRMValue));
                config1V1000.Add(new Setting("cmbCfgRtbViewEntryRMValue", cfg.cmbCfgRtbViewEntryRMValue));
                config1V1000.Add(new Setting("chkCfgUseWinUserDocFolder", cfg.chkCfgUseWinUserDocFolder));
                config1V1000.Add(new Setting("radCfgUseOpenFileSystemDB", cfg.radCfgUseOpenFileSystemDB));
                config1V1000.Add(new Setting("radCfgUseSingleFileDB", cfg.radCfgUseSingleFileDB));
                myConfig.Add(config1V1000);
                myConfig.SaveToFile(file);
                cfg.configFilePath = file;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool loadConfigFile(String file, ref myConfig cfg)
        {
            cfg = new myConfig();

            try
            {
                // Create the configuration.
                Configuration myConfig = Configuration.LoadFromFile(file);
                if (myConfig == null)
                    return false;

                Section config1V1000 = myConfig["Config1Version1.0.0.0"];
                cfg.chkCfgAutoLoadCreateDefaultDB = config1V1000["chkCfgAutoLoadCreateDefaultDB"].BoolValue;
                cfg.cmbCfgRtbEntryRMValue = config1V1000["cmbCfgRtbEntryRMValue"].IntValue;
                cfg.cmbCfgRtbViewEntryRMValue = config1V1000["cmbCfgRtbViewEntryRMValue"].IntValue;
                cfg.chkCfgUseWinUserDocFolder = config1V1000["chkCfgUseWinUserDocFolder"].BoolValue;
                cfg.radCfgUseSingleFileDB = config1V1000["radCfgUseSingleFileDB"].BoolValue;
                cfg.radCfgUseOpenFileSystemDB = config1V1000["radCfgUseOpenFileSystemDB"].BoolValue;
                cfg.configFilePath = file;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool autoCreateLoadConfigFile(ref myConfig cfg, bool initNewConfig = false)
        {
            String file = getConfigPathFile();

            if (initNewConfig)
            {
                if (!saveConfigFile(file, ref cfg, true))
                    return false;

                if (!loadConfigFile(file, ref cfg))
                    return false;
            }
            else
            {
                if (!File.Exists(file))
                {
                    if (!saveConfigFile(file, ref cfg, false))
                        return false;
                }

                if (!loadConfigFile(file, ref cfg))
                    return false;
            }
            return true;
        }
    }
}
