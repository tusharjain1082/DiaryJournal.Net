/*
 * 
 * 
 * Tushar Jain's Open File System Database (OpenFileSystemDB.cs)
 * location: https://github.com/tusharjain1082/DiaryJournal.Net
 * version: 1.1.0.0
 * description: I myself invented this framework on my own without
 * copying any material from others. this framework is solely my own idea and invention.
 * what we can do with this open file system database is that we can retrieve 
 * and change and even delete database and it's files directly through windows
 * explorer. this database is not single file based database. it is
 * windows file system based database. all database's entries are
 * openly stored as ordinary windows .rtf and .xml files in the 
 * database's own folder. so we can directly in a single step read the files
 * and do whatever we want with the files. so there is no issue of import and export
 * of database because entire database is stored as a common structure of open folders and files
 * in the open like all the files on windows and hard disk. so we can directly pick the files and move/backup them.
 * license: open source and free. no license. i dedicate this work to the public. 
 * initial completion and release date: Saturday, 20 August, 2022.
 * 
 * 
 */

using System.Text;
using System.Xml;

namespace DiaryJournal.Net
{
    public class OpenFSDBContext
    {
        public bool dbLoaded = false;
        public String dbBaseParentPath = "";
        public String dbBasePath = "";
        public String dbEntryPath = "";
        public String dbEntryConfigPath = "";
        public String dbName = "";
        public String dbConfigFile = "";
        public DatabaseConfig dbConfig = new DatabaseConfig();
        public EntryType dbEntryType = EntryType.Rtf;
        public DatabaseIndexing dbIndexing = new DatabaseIndexing();
        public String dbIndexingFile = "";

        public bool isDBOpen()
        {
            return dbLoaded;
        }

        public void close()
        {
            dbLoaded = false;
            dbBasePath = dbEntryPath = dbName = dbBaseParentPath = dbEntryConfigPath = dbConfigFile = "";
            dbConfig = new DatabaseConfig();
                
        }
    }

    public static class OpenFileSystemDB
    {
        public static String defaultDBPath = Application.StartupPath;
        public static String defaultDBPath_factory = Application.StartupPath;
        public static String defaultDBName = "myJournal";
        public static String defaultDBName_factory = "myJournal";
        public static String defaultDBEntryDirName = "Entries";
        public static String defaultDBEntryCfgDirName = "EntryConfig";
        public const string configFileName = "OpenFSDBConfig.xml";

        // auto loads otherwise creates the db if it does not exists. overwrites if flag is set.
        public static bool CreateLoadDB(String parentPath, String dbName, ref OpenFSDBContext ctx, bool overwrite = false,
            bool create = false)
        {
            if (parentPath == "")
                return false;

            if (dbName == "")
                return false;

            if (!Directory.Exists(parentPath))
                return false;

            // prepare formatted paths set
            String dbBasePath = Path.Combine(parentPath, dbName);
            String dbConfigFile = Path.Combine(dbBasePath, configFileName);
            String dbEntryPath = Path.Combine(dbBasePath, defaultDBEntryDirName);
            String dbEntryCfgPath = Path.Combine(dbBasePath, defaultDBEntryCfgDirName);
            String dbIndexingFile = Path.Combine(dbBasePath, DatabaseIndexing.dbIndexingFileName);

            // this method securely erases all the files in the target directory.
            if (overwrite)
            {
                // if overwrite, then always create new directory set. if it exists, erase and delete them and create new.
                if (Directory.Exists(dbBasePath))
                    commonMethods.SecureDeleteDirectory(dbBasePath);

                try
                {
                    Directory.CreateDirectory(dbEntryPath);
                    Directory.CreateDirectory(dbEntryCfgPath);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            
            // if directory set does not exists, create them, but not overwrite.
            if (create)
            {
                if (!Directory.Exists(dbBasePath))
                {
                    try
                    {
                        Directory.CreateDirectory(dbEntryPath);
                        Directory.CreateDirectory(dbEntryCfgPath);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            else
            {
                // user does not requests new db creation. if it doesn't exists, return with error.
                if (!Directory.Exists(dbBasePath))
                    return false;
            }

            // validate
            if (!Directory.Exists(dbEntryPath))
                return false;

            if (!Directory.Exists(dbEntryCfgPath))
                return false;

            // initialize and write brand new config file. if it exists, load it.
            ctx.dbConfig = new DatabaseConfig();
            if (File.Exists(dbConfigFile))
                DatabaseConfig.fromXml(ref ctx.dbConfig, dbConfigFile);
            else
                DatabaseConfig.toXmlFile(ref ctx.dbConfig, dbConfigFile);

            // initialize and write brand new db indexing file. if it exists, load it.
            ctx.dbIndexing = new DatabaseIndexing();
            if (File.Exists(dbIndexingFile))
                DatabaseIndexing.fromFile(ref ctx.dbIndexing, dbIndexingFile);
            else
                DatabaseIndexing.toFile(ref ctx.dbIndexing, dbIndexingFile);

            // success
            ctx.dbBasePath = dbBasePath;
            ctx.dbBaseParentPath = parentPath;
            ctx.dbEntryPath = dbEntryPath;
            ctx.dbEntryConfigPath = dbEntryCfgPath;
            ctx.dbName = dbName;
            ctx.dbConfigFile = dbConfigFile;
            ctx.dbIndexingFile = dbIndexingFile;
            ctx.dbLoaded = true; // set marker db is loaded

            return true;
        }

        // this method loads the open file system db when the user inputs the complete db path which includes the db name part in the path string
        public static bool LoadDB(ref OpenFSDBContext ctx, String dbPath)
        {
            if (dbPath == "")
                return false;

            // prepare
            DirectoryInfo info0 = new DirectoryInfo(dbPath);
            String dbName = info0.Name;
            DirectoryInfo info1 = Directory.GetParent(dbPath);
            if (info1 == null)
                return false; // invalid db path

            String dbParentPath = info1.FullName;

            // load only.
            return CreateLoadDB(dbParentPath, dbName, ref ctx, false, false);
        }

        // this method auto creates and loads db, otherwise overwrites and creates and loads new db if user demands
        public static bool autoLoadCreateDefaultDB(ref OpenFSDBContext ctx, myConfig cfg, bool overwrite)
        {
            if (ctx.dbLoaded)
                ctx.close();

            if (cfg.chkCfgUseWinUserDocFolder)
                defaultDBPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                defaultDBPath = Application.StartupPath;

            // now create overwrite if demanded, else load existing
            return CreateLoadDB(defaultDBPath, defaultDBName, ref ctx, overwrite, true);
        }





        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Shell ***********************************************************
        //********************************************************************** 
        //**********************************************************************



        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Shell ***********************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core Framework **************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core Framework **************************************************
        //********************************************************************** 
        //**********************************************************************


        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core ************************************************************
        //********************************************************************** 
        //**********************************************************************

        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(OpenFSDBContext ctx, Int64 id, bool deleted = false,
            bool useDeletedParam = false)
        {
            List<myNode> nodes = new List<myNode>();

            // first find the current node
            String rtf = "";
            myNode? node = selectNode(ctx, id, ref rtf, false);
            if (node == null)
                return nodes; // error node not found

            // if user demands to find deleted nodes, then validate and add accordingly
            if (useDeletedParam)
            {
                if (node.chapter.IsDeleted == deleted)
                    nodes.Add(node); // matching node either deleted marked or not deleted marked as demanded by user
            }
            else
            {
                // add this node at index 0 of the list
                nodes.Add(node);
            }

            if (nodes.Count <= 0)
                return nodes;

            while (true)
            {
                // find and load all parent nodes recursively from bottom to root
                node = selectNode(ctx, node.chapter.parentId, ref rtf, false);
                if (node == null)
                    break; // no more parents found, this is end of loop

                // a parent found

                // if user demands to find deleted nodes, then validate and add accordingly
                if (useDeletedParam)
                {
                    if (node.chapter.IsDeleted == deleted)
                        nodes.Add(node); // matching node either deleted marked or not deleted marked as demanded by user
                }
                else
                {
                    // add this parent node in the list
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        // find all entry files by entry type which exist in a open file system db path.
        public static IEnumerable<String> findAllNodeFiles(OpenFSDBContext ctx)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(ctx.dbEntryType, ref ext, ref extComplete, ref extSearchPattern);

            // now find all valid files
            EnumerationOptions options = new EnumerationOptions();
            options.RecurseSubdirectories = false;
            return Directory.EnumerateFiles(ctx.dbEntryPath, extSearchPattern, options);
        }

        // find all entry files loaded as nodes by entry type which exist in a open file system db path.
        public static List<myNode> findAllNodes(OpenFSDBContext ctx, bool sort = true, bool descending = false)
        {
            IEnumerable<String> files = findAllNodeFiles(ctx);
            List<myNode> nodes = new List<myNode>();
            foreach (String file in files)
            {
                String rtf = "";
                myNode? node = OpenFileSystemDB.pathToNode(ctx, file, ref rtf, false);
                if (node != null)
                    nodes.Add(node);
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }

        // find all nodes by node type
        public static List<myNode> findNodesByNodeType(OpenFSDBContext ctx, NodeType nodeType)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode listNode in findAllNodes(ctx, true, false))
            {
                myNode node = listNode;
                if (node.chapter.nodeType == nodeType)
                    nodes.Add(node);
            }
            return nodes;
        }

        // find nodes by date and time
        public static List<myNode> findNodesByDateTime(OpenFSDBContext ctx, DateTime nodeDateTime, bool useNodeDateTime)
        {
            List<myNode> nodes = new List<myNode>();

            // validation
            if (!useNodeDateTime)
                return nodes; // exception, flag was not set, return with empty list.

            foreach (myNode listNode in findAllNodes(ctx, true, false))
            {
                myNode node = listNode;// new myNode();
                //if (!entryMethods.convertEntryFilenameToNode(ref node, file))
                //    continue; // some invalid file, skip

                // load whole entry config into node
                //if (!loadNodeConfig(ctx, node.chapter.guid, ref node))
                //    continue;

                // a valid format entry file found, process
                if (useNodeDateTime)
                {
                    // node date time flag set, find the node by date and time
                    if (node.chapter.chapterDateTime == nodeDateTime)
                    {
                        // found matching node, process

                        // add this found node to the list
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }
        // loads a node from a path and file string.
        public static myNode? pathToNode(OpenFSDBContext ctx, String file, ref String rtf, bool loadData = false)
        {
            myNode node = new myNode();
            if (!entryMethods.convertEntryFilenameToNode(ref node, file))
                return null; // some invalid file, skip

            // load whole entry config into node
            if (!loadNodeConfig(ctx, node.chapter.Id, ref node))
                return null;

            // load entry data if user asks
            if (loadData)
                rtf = loadNodeData(ctx, node.chapter.Id);

            return node;
        }

        // directly selects a node and it's files by it's guid without finding the node in the db's path
        public static myNode? selectNode(OpenFSDBContext ctx, Int64 id, ref String rtf, bool loadData = false)
        {
            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            if (!File.Exists(entryFile))
                return null;

            myNode node = new myNode();

            // load whole entry config into node
            if (!loadNodeConfig(ctx, id, ref node))
                return null;

            // load entry data if user asks
            if (loadData)
                rtf = loadNodeData(ctx, id);

            // return node
            return node;
        }

        // find & load a node by it's guid or it's parent guid
        public static myNode? findLoadNode(OpenFSDBContext ctx, Int64 id, ref String rtf, bool loadData = false)
        {
            // we directly select the node
            return selectNode(ctx, id, ref rtf, loadData);
        }

        // load the entry data through entry's node
        public static String loadNodeData(OpenFSDBContext ctx, Int64 id)
        {
            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            return File.ReadAllText(entryFile);
        }

        // load the entry config through entry's node 
        public static bool loadNodeConfig(OpenFSDBContext ctx, Int64 id, ref myNode node)
        {
            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, id, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            String output = "";

            // check if the node contains no chapter, then create a new chapter for loading the config into it.
            if (node.chapter == null)
                node.chapter = new Chapter();

            // load the config into the node's chapter.
            return xmlEntry.fromXml(ref node.chapter, entryConfigFile, ref output);
        }

        // this method generates factory default formatted node file names and strings
        public static void generateNodeFileNames(OpenFSDBContext ctx, Int64 id, 
            out String entryNameOut, out String entryCfgNameOut, out String entryFileOut, out String entryConfigFileOut)
        {
            entryFileOut = entryMethods.getFormattedPathFileName(ctx.dbEntryPath, id, 0,
                "", default(DateTime), 0, ctx.dbEntryType, out entryNameOut);
            entryConfigFileOut = entryMethods.getFormattedPathFileName(ctx.dbEntryConfigPath, id,
                0, "", default(DateTime), 0, EntryType.Xml, out entryCfgNameOut);
        }

        // this method generates factory default formatted node file names and strings
        public static void generateNodeFileNames(OpenFSDBContext ctx, ref myNode node,  
            out String entryNameOut, out String entryCfgNameOut, out String entryFileOut, out String entryConfigFileOut)
        {
            entryFileOut = entryMethods.getFormattedPathFileName(ctx.dbEntryPath, node.chapter.Id, 0,
                "", default(DateTime), 0, ctx.dbEntryType, out entryNameOut);
            entryConfigFileOut = entryMethods.getFormattedPathFileName(ctx.dbEntryConfigPath, node.chapter.Id,
                0, "", default(DateTime), 0, EntryType.Xml, out entryCfgNameOut);
        }

        // create a new node. guid used cannot be reused.
        public static bool createNode(OpenFSDBContext ctx, ref myNode node, String rtf, bool newId)
            
        {
            if (node == null)
                return false;

            // prepare

            if (newId)
            {
                // generate new id if demanded
                node.chapter.Id = entryMethods.CreateNodeID(ref ctx.dbIndexing.currentDBIndex);
            }

            // create entry's config xml string
            String entryConfigString = xmlEntry.toXml(ref node.chapter, "");

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, ref node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // now write files

            // write config file
            File.WriteAllText(entryConfigFile, entryConfigString);
            // write entry file. entry file is simply the richtext format .rtf file
            File.WriteAllText(entryFile, rtf);
            return true;
        }

        // this method purges the old unusable files and replaces them with new update, and updates the node accordingly. 
        public static bool updateNode(OpenFSDBContext ctx, ref myNode node, String rtf = "", bool storeData = false)
        {
            if (node == null)
                return false;

            // prepare

            // create entry's config xml string
            String entryConfigString = xmlEntry.toXml(ref node.chapter, "");

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, ref node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // if data flag is set, then user has given new data, so purge the old data file
            // if data flag is not set, then user has not given any new data, so rename the old data file to the new updated file name
            if (storeData)
                purgeNode(ctx, ref node, true, true); // new data so purge old data file
            else
                purgeNode(ctx, ref node, true, false); // data file must persist from the past to this update.

            // now write files

            // write config file
            File.WriteAllText(entryConfigFile, entryConfigString);

            if (storeData)
            {
                // write entry file. entry file is simply the richtext format .rtf file
                File.WriteAllText(entryFile, rtf);
            }
            return true;
        }

        // erases and purges the node's files
        public static bool purgeNode(OpenFSDBContext ctx, ref myNode node, bool purgeConfig = true, bool purgeData = true)
        {
            if (node == null)
                return false;

            // get formatted open file system db node file names
            String entryNameOut = "", entryCfgNameOut = "";
            String entryFile = "", entryConfigFile = "";
            generateNodeFileNames(ctx, ref node, out entryNameOut, out entryCfgNameOut, out entryFile, out entryConfigFile);

            // secure erase then purge
            if (purgeConfig)
                commonMethods.SecureEraseFile(entryConfigFile, 1, true);

            if (purgeData)
                commonMethods.SecureEraseFile(entryFile, 1, true);

            return true;
        }

        // this method counts the total number of valid/validated nodes found in the open file system database entries path.
        public static long NodesCount(OpenFSDBContext ctx)
        {
            long count = 0;
            //foreach (String file in findAllNodes(ctx))
            foreach (myNode listNode in findAllNodes(ctx, false, false))
            {
                myNode node = listNode;
                //myNode node = new myNode();
                //if (!entryMethods.convertEntryFilenameToNode(ref node, file))
                 //   continue; // some invalid file, skip

                // a valid format entry file found, process
                if (node.chapter.Id != 0)
                {
                    // found matching node, process
                    count++;
                }
            }
            return count;
        }

        public static myNode? newNode(ref OpenFSDBContext ctx,
            SpecialNodeType specialNodeType, NodeType nodeType, DomainType domainType,
            ref myNode? initialNode, DateTime nodeDateTime = default(DateTime), Int64 parentId = 0, bool DBImport = true,
            String title = "", String rtf = "", bool newId = true)
        {
            // prepare and configure
            myNode? node = initialNode;
            if (node == null)
                node = new myNode(true);

            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = nodeDateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.specialNodeType = specialNodeType;
            node.chapter.domainType = domainType;
            node.chapter.Title = title;

            // now when all setup is done, import the entry and it's data object into the db if required
            if (DBImport)
            {
                if (!createNode(ctx, ref node, rtf, newId))
                    return null;    
            }
            return node;
        }

        //**********************************************************************
        //********************************************************************** 
        //********************************************************************** 
        //**** Core ************************************************************
        //********************************************************************** 
        //**********************************************************************

    }
}
