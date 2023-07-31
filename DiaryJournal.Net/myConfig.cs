using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SharpConfig;
using System.IO;
using RtfPipe.Model;

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

        // other config
        public bool radCfgLMNode = true;
        public bool radCfgLCNode = false;
        public bool radCfgTCNode = false;

        public bool chkCfgAutoLoadCreateDefaultDB = true;
        public int cmbCfgRtbViewEntryRMValue = 800;//1500;
        public static int default_cmbCfgRtbViewEntryRMValue = 800;//1500;
        public bool chkCfgUseWinUserDocFolder = true;
        public String configFilePath = "";

        public const int defaultDocumentWidth = 800;

        public int tvEntriesItemHeight = 20;
        public static int default_tvEntriesItemHeight = 20;
        public int tvEntriesIndent = 20;
        public static int default_tvEntriesIndent = 20;
        public Font? tvEntriesFont = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
        public static Font default_tvEntriesFont = new System.Drawing.Font("Arial", 11, FontStyle.Regular);
        public Color tvEntriesBackColor = Color.White;
        public Color tvEntriesForeColor = Color.Black;
        public static Color default_tvEntriesBackColor = Color.White;
        public static Color default_tvEntriesForeColor = Color.Black;

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
                Configuration config = new Configuration();
                Section config1V1000 = new Section("Config1Version1.0.0.0");

                // prepare default config wherever required
                if (cfg.tvEntriesItemHeight <= 0) cfg.tvEntriesItemHeight = myConfig.default_tvEntriesItemHeight;
                if (cfg.tvEntriesIndent <= 0) cfg.tvEntriesIndent = myConfig.default_tvEntriesIndent;
                if (cfg.tvEntriesFont == null) cfg.tvEntriesFont = myConfig.default_tvEntriesFont;
                if (cfg.tvEntriesBackColor == Color.Empty) cfg.tvEntriesBackColor = myConfig.default_tvEntriesBackColor;
                if (cfg.tvEntriesForeColor == Color.Empty) cfg.tvEntriesForeColor = myConfig.default_tvEntriesForeColor;

                config1V1000.Add(new Setting("chkCfgAutoLoadCreateDefaultDB", cfg.chkCfgAutoLoadCreateDefaultDB));
                config1V1000.Add(new Setting("cmbCfgRtbViewEntryRMValue", cfg.cmbCfgRtbViewEntryRMValue));
                config1V1000.Add(new Setting("chkCfgUseWinUserDocFolder", cfg.chkCfgUseWinUserDocFolder));
                config1V1000.Add(new Setting("radCfgUseOpenFileSystemDB", cfg.radCfgUseOpenFileSystemDB));
                config1V1000.Add(new Setting("radCfgUseSingleFileDB", cfg.radCfgUseSingleFileDB));
                config1V1000.Add(new Setting("radCfgLMNode", cfg.radCfgLMNode));
                config1V1000.Add(new Setting("radCfgLCNode", cfg.radCfgLCNode));
                config1V1000.Add(new Setting("radCfgTCNode", cfg.radCfgTCNode));
                config1V1000.Add(new Setting("tvEntriesItemHeight", cfg.tvEntriesItemHeight));
                config1V1000.Add(new Setting("tvEntriesIndent", cfg.tvEntriesIndent));
                config1V1000.Add(new Setting("tvEntriesFont", commonMethods.FontToString(cfg.tvEntriesFont)));
                config1V1000.Add(new Setting("tvEntriesBackColor", commonMethods.ColorToString(cfg.tvEntriesBackColor)));
                config1V1000.Add(new Setting("tvEntriesForeColor", commonMethods.ColorToString(cfg.tvEntriesForeColor)));

                config.Add(config1V1000);
                config.SaveToFile(file);
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
                Configuration config = Configuration.LoadFromFile(file);
                if (config == null)
                    return false;

                Section config1V1000 = config["Config1Version1.0.0.0"];
                cfg.chkCfgAutoLoadCreateDefaultDB = config1V1000["chkCfgAutoLoadCreateDefaultDB"].BoolValue;
                cfg.cmbCfgRtbViewEntryRMValue = config1V1000["cmbCfgRtbViewEntryRMValue"].IntValue;
                cfg.chkCfgUseWinUserDocFolder = config1V1000["chkCfgUseWinUserDocFolder"].BoolValue;
                cfg.radCfgUseSingleFileDB = config1V1000["radCfgUseSingleFileDB"].BoolValue;
                cfg.radCfgUseOpenFileSystemDB = config1V1000["radCfgUseOpenFileSystemDB"].BoolValue;
                cfg.radCfgLMNode = config1V1000["radCfgLMNode"].BoolValue;
                cfg.radCfgLCNode = config1V1000["radCfgLCNode"].BoolValue;
                cfg.radCfgTCNode = config1V1000["radCfgTCNode"].BoolValue;
                cfg.tvEntriesItemHeight = config1V1000["tvEntriesItemHeight"].IntValue;
                cfg.tvEntriesIndent = config1V1000["tvEntriesIndent"].IntValue;
                cfg.tvEntriesFont =  commonMethods.StringToFont(config1V1000["tvEntriesFont"].StringValue);
                cfg.tvEntriesBackColor = commonMethods.StringToColor(config1V1000["tvEntriesBackColor"].StringValue);
                cfg.tvEntriesForeColor = commonMethods.StringToColor(config1V1000["tvEntriesForeColor"].StringValue);
                cfg.configFilePath = file;

                if (cfg.tvEntriesItemHeight <= 0) cfg.tvEntriesItemHeight = myConfig.default_tvEntriesItemHeight;
                if (cfg.tvEntriesIndent <= 0) cfg.tvEntriesIndent = myConfig.default_tvEntriesIndent;
                if (cfg.tvEntriesFont == null) cfg.tvEntriesFont = myConfig.default_tvEntriesFont;
                if (cfg.tvEntriesBackColor == Color.Empty) cfg.tvEntriesBackColor = myConfig.default_tvEntriesBackColor;
                if (cfg.tvEntriesForeColor == Color.Empty) cfg.tvEntriesForeColor = myConfig.default_tvEntriesForeColor;


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
