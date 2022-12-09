# DiaryJournal.Net
an open source and free desktop/laptop diary and journal software project from Tushar Jain, for latest Visual Studio 2022, Windows 10/11 and .Net 6.0. includes precompiled program files to use as your diary/journal application.

# version 1.7.5.0
published 09 December 2022. major upgrade. major improvements. new additions. some bugs also fixed. added formal printing support. some project files were missing and solution file was older obsolete version, i have corrected everything. the entire solution is now compiling and binary files are being produced in bin\ directory as expected.

# requirements
A Desktop or Laptop PC, .Net 6.0 and .Net Desktop 6.0 is the basic requirement. You need latest version of .Net 6, not the first version 6.0. You can run the executable software files in any Windows where .Net 6.0 and .Net 6.0 Desktop Runtime (both latest version not first version) properly installs and functions.

i coded the entire software on a native 64-bit Windows 11, therefore native 64-bit Windows is probably the other requirement. except for the requirements mentioned, there is no other dependency or requirement.

# important notes
it is very important for you to be on caution that this software and source code is not completely perfect yet. "not perfect" doesn't means the software is incomplete. it is complete. please excuse the exception of printing support, i do not know how to add printing support. what you can do for printing an entry is to export the entry as .html file and print it from web browser like chrome edge or firefox. as soon as i find printing support, i shall add it. there may exist multiple bugs in the software because the software is new and not perfect. i fix the bug when i find it but it takes many days to weeks time even months time. so you must cautiously use this software for professional and real life work and journaling of any kind yet. you must agree to use this software and the source code at your own risk and responsibility. you must always create multiple full backups of database in various places before doing any operation. when the software becomes perfect, i will remove this statement and publish a notice to confirm that the software is perfect and faultless.

this software is completely memory dependent like all the multimedia, data entry, 3d and imaging, banking software, and therefore can fail resulting in database corruption if your computer has less memory and the software overflows the memory while doing any database operation. as many nodes/entries exist and load in/from the database, as much memory it requires. only the entry's node is loaded with all the rest of nodes. node is the complete configuration of the entry. entry's body which is the rich text media, is not loaded unless the entry is actually edited or processed. if each node is loaded and processed individually from the database and the database is operated every time for every individual node, then everything will definately become darn slow and tedius for example while searching the entire database, importing and exporting entries. this can be witnessed when you have aquired tens of thousands of entries, or hundred thousand, or more. to prevent this issue, i have configured everything which makes the software dependent on computer memory. if you have at least 16 GB RAM and SSD and latest technologies powerful desktop/laptop with a large 20+ GB windows paging file, you should be able to load a wooping 100,000 nodes/entries in seconds to minutes without any issue. because i developed this software on the same configuration.

you can find and use executable software files in /bin/Release/net6.0-windows/ directory.

to download the binary program/software executable files or to download the source code, you need to download both as a single zip file. please click on "Code" and then click on "Download Zip" to download both the binary program/software executable files and the source code all included into one single zip file.

DiaryJournal.Net is upgradable to .Net 7.0, so you can retarget entire DiaryJournal.Net Solution to .Net 7.0 and run DiaryJournal.Net in .Net 7.0 without issues.

if you discover a bug, defect, issue, please report it to me.

if you find this software project eligible for a star, please do star it.

# features
2 types of database used. 1. single file based database (LiteDB). 2. open file system based database (my own code) in which the entire database is stored in open as ordinary files on windows and hard disk and we can read and manage any database file.

# screenshots
![Alt text](/screenshot4.png?raw=false "DiaryJournal.Net screenshot")

![Alt text](/screenshot5.png?raw=false "DiaryJournal.Net screenshot")

![Alt text](/screenshot6.png?raw=false "DiaryJournal.Net screenshot")



