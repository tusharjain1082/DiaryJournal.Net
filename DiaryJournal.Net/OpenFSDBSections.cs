using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    public class OpenFSDBSection
    {
        public long sectionId = 0;
        public long totalNodes = 0;


    }

    public class OpenFSDBSections
    {
        public long totalSections = 0;
        public List<OpenFSDBSection> sections = new List<OpenFSDBSection>();
        public String dbBaseParentPath = "";
        public String dbBasePath = "";
        public String dbEntryPath = "";
        public String dbEntryConfigPath = "";

        public const long maxNodesInSection = 1000; // max 1000 nodes per section

        public OpenFSDBSections()
        {

        }

        public OpenFSDBSections(String dbBaseParentPath, String dbBasePath, String dbEntryPath, String dbEntryConfigPath)
        {
            this.dbBaseParentPath = dbBaseParentPath;
            this.dbBasePath = dbBasePath;
            this.dbEntryPath = dbEntryPath;
            this.dbEntryConfigPath = dbEntryConfigPath;
        }

        // find a section
        public static OpenFSDBSection? findSection(OpenFSDBContext ctx, long id)
        {
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
                if (section.sectionId == id)
                    return section;

            return null;
        }

        // this method auto finds-generates a new available section id which requires no further incrementation
        public static long findNewSectionId(OpenFSDBContext ctx)
        {
            if (ctx.dbSections.sections.Count > 0)
                return (ctx.dbSections.sections.Max(s => s.sectionId) + 1);
            else
                return 1;
        }
        // get auto formatted paths for section in both entry and entry config directories
        public static void getFormattedSectionPaths(OpenFSDBContext ctx, long id, ref String EntrySectionOut, ref String EntryConfigSectionOut)
        {
            String EntrySectionPath = Path.Combine(ctx.dbSections.dbEntryPath, id.ToString());
            String EntryConfigSectionPath = Path.Combine(ctx.dbSections.dbEntryConfigPath, id.ToString());
            EntrySectionOut = EntrySectionPath;
            EntryConfigSectionOut = EntryConfigSectionPath;
        }
        // this method creates a new section with one step ahead incremented id
        public static OpenFSDBSection? createSection(OpenFSDBContext ctx, long id)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, id, ref EntrySectionPath, ref EntryConfigSectionPath);

            // this section already exists, so we must return error and not create a new one
            // so that the user defines a new nonexistent section with new available id
            if (Directory.Exists(EntryConfigSectionPath))
                return null;

            // section does not exists, so create it
            Directory.CreateDirectory(EntrySectionPath);
            Directory.CreateDirectory(EntryConfigSectionPath);

            // return new section
            OpenFSDBSection section = new OpenFSDBSection();
            section.sectionId = id;
            ctx.dbSections.sections.Add(section);
            return section;
        }
        // this method finds the first section in list having an available slot for a node
        public static OpenFSDBSection? findFirstSectionWithFreeSlot(OpenFSDBContext ctx)
        {
            foreach (OpenFSDBSection section in ctx.dbSections.sections)
            {
                if (section.totalNodes < maxNodesInSection)
                    return section;
            }
            return null;
        }
        // this method auto finds an available section, otherwise auto creates a new section in list and returns it
        public static OpenFSDBSection? autoFindCreateAvailableSection(OpenFSDBContext ctx)
        {
            // find and return if we have an available section
            OpenFSDBSection? section = findFirstSectionWithFreeSlot(ctx);
            if (section != null)
                return section;

            // no available section, so create a new one with a new available id
            return createSection(ctx, findNewSectionId(ctx));
        }

        // this method loads all sections into list until no more section is found
        public static void loadReloadSections(OpenFSDBContext ctx)
        {
            ctx.dbSections.sections.Clear();
            long id = 1; // 1 is valid section id and 0 is invalid
            while (true)
            {
                OpenFSDBSection section = new OpenFSDBSection();
                if (!loadSection(ctx, id, ref section))
                    break;

                // section found, add it
                ctx.dbSections.sections.Add(section);

                id++;
            }    

            if (id == 0)
            {
                // no sections found, so create the initial first one with index 1
                autoFindCreateAvailableSection(ctx);
            }
        }

        // this method loads the section and validates it
        public static bool loadSection(OpenFSDBContext ctx, long id, ref OpenFSDBSection section)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, id, ref EntrySectionPath, ref EntryConfigSectionPath);

            if (!Directory.Exists(EntryConfigSectionPath))
                return false;

            section.sectionId = id;
            refreshSection(ctx, ref section);
            return true;
        }

        // this method refreshes the section with new latest config assuming the section already exists
        public static void refreshSection(OpenFSDBContext ctx, ref OpenFSDBSection section)
        {
            String EntrySectionPath = "";
            String EntryConfigSectionPath = "";
            getFormattedSectionPaths(ctx, section.sectionId, ref EntrySectionPath, ref EntryConfigSectionPath);

            DirectoryInfo directoryInfo = new DirectoryInfo(EntryConfigSectionPath);
            long totalNodes = directoryInfo.GetFiles().LongCount();
            section.totalNodes = totalNodes;
        }


    }
}
