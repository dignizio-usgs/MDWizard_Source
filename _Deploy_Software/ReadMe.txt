D. Ignizio
12/2013

Instructions for 'assembling/preparing' toolbox materials.

-Compile the VB code for the EA Editor against either the 10.0, 10.1, or 10.2 version of ESRI's ArcObjects with the appropiate SDK. A separate machine for each scenario with the properly configured Visual Basic IDE/SDK setup is required for this. After completing this step, copy the 'StandAloneEAEditor2.exe' file from the location where it is created (example: Versioned_VBDev\EABuilder_v10.1\StandAloneEAEditor2\bin\Debug\StandAloneEAEditor2.exe). Rename the .exe file, adding either "_V10", "_V101", or "_V102" distinction to the file name, as appropriate. Copy this file to the "...\MetadataWizard\MDWizard_PythonCode" folder. There is some duplication here, as the 10.2 exe should never be needed by the 10.0 tool, but it is good practice to keep the folders consistent. Also, the Python script looks for all versions, regardless of the version, so the files need to be there.

-Compile and run the VB Code for the MetadataEditor. (example: "...\Versioned_VBDev\MetadataEditor_MainForm\MetadataEditor\bin\Debug\MetadataEditor.exe") Copy the .exe file to the "...\MetadataWizard\MDWizard_PythonCode" folder.

-After all edits have been made to the Python code (using an IDE that points to the Python workspace listed above), all of the contents of the folder need to be copied to each deployment package. Copy all the contents of the folder, with the exception of the 2 following files (they are something Eclipse creates, but not necessary for deployment):
'.project'
'.pydevproject'
Paste the files into the deployment folders.

-All the Python Code and necessary secondary resources are maintained in this folder: "...\MetadataWizard\MDWizard_PythonCode"
(D. Ignizio Note: I am developing the Python (.py) files from this location in my IDE. Completed/updated .py files also need to be copied into the appropriate deploy bundles. These files are not ESRI version dependent; they are the same in all the deploy folders but will need to be replaced after updates are made).

For the 10.0 deployment package (at the time of this documentation, D. Ignizio was developing in the Arc 10.0 environment), the .tbx file located in the Python workspace is the same file that should be placed in the 10.0 deployment package.

For the 10.1 and 10.2 deployment packages, a separate ArcToolbox (.tbx file) actually needs to be created and maintained. The same toolbox works in both 10.1 and 10.2. 

On a 10.1 or 10.2 machine, create/edit this second version of the toolbox (new.tbx file). A dual monitor setup with a 10.1 VM and ArcCatalog pulled up and the 10.0 machine with ArcCatalog pulled up works well for this. Ensure the toolboxes are identically set up for consistency.
The parameter configurations, documentation, and validation scripts can be the same, they just must be set up for the toolboxes independently on machines using the different specific versions of ESRI. Editing the 10.0 toolbox and copying/pasting changes in documentation or the validation script to the different version toolbox on the VM in the second monitor seems to be an efficient way to do this.

-(A 10.0 toolbox added to ESRI 10.1/10.2 will erroneously show multiple intances of a tool and will not work properly).

-The 10.1 deployment folder and the 10.2 deployment folder are the same. These could be combined in the future, if desired.

--

Be sure to avoid 'importing' the script into a tool in a Python toolbox in ArcCatalog when editing the properties. This will result in the script getting hardcoded in such a way that changes to the stand-alone .py file that the tool points to will not be effected. Just point the tool to the python file: 'USGS_MetadataTool.py'.

--

The 'Resources' folder in the Python workspace and the deployment packages needs to contain: 
1. The config file used with MP (as of December 2013, I'm using the 'BDP_Mod.cfg' file)
2. The MP .exe file
3. The 'MetadataWizardStylesheet.xsl' for clean viewing of metadata records in Internet Explorer from the GUI.

--

Be consistent/systematic with updates, ensuring they are shared across the multiple versions of the software. I.e., updates to the EA Builder GUI (VB.NET) need to be made identically across all 3 versions of the VB code, then re-compiled in each, and finally copied over/renamed in the deployment packages. 

Python code changes or changes in the Main Form Wizard GUI (VB.NET) only need to be made in the respective code, but need to be copied/replaced in all 3 deployment folders.

--

FORT STAFF: 

The FORT team's ArcToolbox collection (...\GIS_LIBRARY\_FORT_Resources\ArcGIS_ToolBoxes) should also be updated with modified versions of the toolboxes. This also provides a nice final test space before re-posting any changed versions to Sciencebase. 

Post updated, robust versions of software to www.sciencebase.gov/metadatawizard