using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SharpConfig;

namespace DiaryJournal.Net
{
    public class myConfig
    {
        public bool chkCfgAutoLoadCreateDefaultDB = false;
        public int cmbCfgRtbEntryRMValue = 1500;
        public int cmbCfgRtbViewEntryRMValue = 1500;
        public bool radioCfgUseDocumentsPath = true;
        public bool radioCfgUseAppPath = false;
    }

    public static class myConfigMethods
    {
        public const string myConfigFileName = "myConfig.cfg";

        public static String getConfigPathFile()
        {
            return Path.Combine(Application.StartupPath, Path.GetFileName(myConfigFileName));
        }
        public static bool saveConfigFile(String file, myContext ctx, bool initNewConfig = false)
        {
            if (initNewConfig)
                ctx.config = new myConfig();

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
                config1V1000.Add(new Setting("chkCfgAutoLoadCreateDefaultDB", ctx.config.chkCfgAutoLoadCreateDefaultDB));
                config1V1000.Add(new Setting("cmbCfgRtbEntryRMValue", ctx.config.cmbCfgRtbEntryRMValue));
                config1V1000.Add(new Setting("cmbCfgRtbViewEntryRMValue", ctx.config.cmbCfgRtbViewEntryRMValue));
                config1V1000.Add(new Setting("radioCfgUseDocumentsPath", ctx.config.radioCfgUseDocumentsPath));
                config1V1000.Add(new Setting("radioCfgUseAppPath", ctx.config.radioCfgUseAppPath));
                myConfig.Add(config1V1000);
                myConfig.SaveToFile(file);
                ctx.configFilePath = file;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool loadConfigFile(String file, myContext ctx)
        {
            ctx.config = new myConfig();

            try
            {
                // Create the configuration.
                Configuration myConfig = Configuration.LoadFromFile(file);
                if (myConfig == null)
                    return false;

                Section config1V1000 = myConfig["Config1Version1.0.0.0"];
                ctx.config.chkCfgAutoLoadCreateDefaultDB = config1V1000["chkCfgAutoLoadCreateDefaultDB"].BoolValue;
                ctx.config.cmbCfgRtbEntryRMValue = config1V1000["cmbCfgRtbEntryRMValue"].IntValue;
                ctx.config.cmbCfgRtbViewEntryRMValue = config1V1000["cmbCfgRtbViewEntryRMValue"].IntValue;
                ctx.config.radioCfgUseDocumentsPath = config1V1000["radioCfgUseDocumentsPath"].BoolValue;
                ctx.config.radioCfgUseAppPath = config1V1000["radioCfgUseAppPath"].BoolValue;
                ctx.configFilePath = file;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool autoCreateLoadConfigFile(myContext ctx, bool initNewConfig = false)
        {
            String file = getConfigPathFile();

            if (initNewConfig)
            {
                if (!saveConfigFile(file, ctx, true))
                    return false;

                if (!loadConfigFile(file, ctx))
                    return false;
            }
            else
            {
                if (!File.Exists(file))
                {
                    if (!saveConfigFile(file, ctx, false))
                        return false;
                }

                if (!loadConfigFile(file, ctx))
                    return false;
            }
            return true;
        }
    }
}
