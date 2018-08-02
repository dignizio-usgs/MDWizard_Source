Metadata Wizard
===============

##### Metadata Wizard version Beta release (Updated 122013)
- Updated help tips and default values.
- Ability to load from a template XML file.
- Auto import contact now works for all USGS personnel.
- Updated ESRI ArcToolbox interface parameter configuration.
- Updated Python routine to calculate Longitudinal and Latitudinal resolution.
- Updated default values for EntityAttribute overview.
- Dedicated link in ScienceBase where the tool can be downloaded.
 
##### Metadata Wizard version 1.0 (Updated 122014)
- Handle null values and empty fields in input data sets.
- Handles feature classes within file and personal GDB.
- Internal VB.Net metadata preview capacity (no longer dependent on IE).
- Updated ESRI ArcToolbox interface parameter configuration.
- Users can specify any metadata template of their choice.
- Updated toolbox documentation and tool input validation.
 
##### Metadata Wizard version 1.1 (Updated 01272014)
- Key element check implemented. Wizard now checks for and ensures the presence of the following ['idinfo', 'dataqual', 'spdoinfo', 'spref', 'eainfo', 'distinfo', 'metainfo'].
This resolves the issue of the tool hanging up on "Updating Digital Transfer Info - Format Name" (and other instances resulting from missing nodes).
- Microsoft Excel file provided as input will prompt user to export to .dbf.
- Handling of State Plane coordinate systems has been updated.
- XML output will now be well-formatted when viewed in raw XML. Thanks to P. Schweitzer for updates to the MP utility.

##### Metadata Wizard version 1.2 (Updated 01292014)
- Corrected error in ArcToolbox validation script to allow for proper specification of template metadata file.

##### Metadata Wizard version 1.2.1 (Updated 0242014)
- Added a 'multi-try' sequence to return statistics on ArcObjects table. This should resolve the 'Attempted to readwrite corrupted memory' error upon opening the EntityAttribute builder.
- Updated image in ArcCatalog 'Description' tab to illustrate Metadata Wizard workflow.

##### Metadata Wizard version 1.2.2 (Updated 0312014) - Internal
- Minor updates to in-tool help (parameter descriptions) for the ArcToolbox tool.

##### Metadata Wizard version 1.2.3 (Updated 0552014)
- Minor updates to in-tool help (parameter descriptions) for the ArcToolbox tool.
- Updated image in ArcCatalog 'Description' tab to illustrate Metadata Wizard workflow with slightly more detail to match the image provided in the Metadata Wizard publication.
- The 'Overview Description' text box is now properly referenced by the 'Citation' element in the EntityAttribute.
- The 'Originator' for the Larger Work Citation is now properly saved to the output XML file. Previous versions of the tool would drop this element. Thanks to Roland Viger for identifying this issue.
- Updated the provided 'Custom Starter Template' file to allow for proper behavior when using the Import From Template option. To work properly, template FGDC-CSDGM XML files must contain at least 'idinfocitationciteinfo' and 'metainfometstdn' elements, and ESRI metadata XML files must have at least 'idPurp, idAbs, idCredit, and searchKeys' elements. This is so the Metadata Wizard can determine the metadata format and apply the appropriate transformation during export for continued editing. The elements may be empty (i.e., contain no text) or populated with dummy values, but they must be present. If these elements are not present, the Wizard will be unable to identify and use the template and will instead begin building a metadata record using a blank FGDC-CSDGM template. Thanks to VeeAnn Cross for identifying this issue.
- Added GUI tip to remind users to populate only one 'Time Period' option for the input dataset.

##### Metadata Wizard version 1.2.4 (Updated 05132014)
- Added a count check for certain node instances to avoid an 'Index out of Range' error in the VB code. This should resolve problems when loading the second GUI and using importcopy features within the form itself resulting from empty (repeating) nodes.

##### Metadata Wizard version 1.2.5 (Updated 06172014)
- Updated the VB.Net forms so that the 'AutoScaleMode' is set to 'DPI' and not 'Font'. This should resolve the issue of the form looking strange if a user has something other than the default magnification/size selected for their profile in 
Windows. 
- Included the updated 'starter template' .XML file with the toolbox bundle. This file properly labels the dummy content in 
the "Originator" field.

##### Metadata Wizard version 1.3 (Updated 08012014)
- The 'Source Scale' input has been added to the Source Inputs tab in the Main Metadata Editor form.
- Single toolbox should now work across all versions of ESRI ArcGIS Desktop 10.x.
- Added new "Metadata Validation" tab. This now allows a user to run MP and view metadata errors from directly within the Metadata Wizard tool. 

- Thanks to Curtis Price for the following edits:
- Built "clean" tbx file using ArcGIS 10.1 (saved as 10.0) to avoid double tool display. This clean tbx should work with all ArcGIS 10.x.
- Various tweaks to USGS_MetadataTool.py to improve maintainability (minor re-coding)
- "WGS 1984.prj" file always used to avoid differences between prj files between ArcGIS versions
- Used GetInstallInfo to populate ArcGIS versions (Get_NativeEnvironment)
- Native Environment string: Set and use string "ArcGIS 10.x" instead of "ArcCatalog 10.x" (changes to Get_NativeEnvironment, GetESRIVersion_WriteNativeEnv). 
- *Modified validation code in tbx:*
- Create working folder in user TEMP (USGS STIG does not allow creation of C:\TEMP folder)
- Modified validation to use type "Folder" not "Workspace" for metadata working folder, also did some cleanup of the validation code and added a check to make sure the template file is entered when the box is checked. 
- If specified folder is not available, standard ArcGIS validation used (no longer populating instruction text into path variable -- to be consistent with other ArcGIS tools).
- Made minor edits to the tool documentation (mostly formatting, using bullets, consistency)

##### Metadata Wizard version 1.4 (Updated 10022014)
- Python files now check for 'Multipoint' and 'MultiPoint' (capitalization) to avoid problems assigning feature type. Thanks to T. Chesley-Preston for identifying this issue.
- A sleep statement was introduced in the VB.Net to ensure that the XML error report generated by MP has time to save completely to avoid problems. Thanks to A. Freeman for identifying this issue.
- C:\temp is no longer used when creating the 'ArcpyTranslate.xml' file when converting from ESRI metadata to FGDC. The 'WorkingDir' is now passed and used instead. Thanks to S. Miller for identifying this issue.
- Updates were made to the way 'Distribution Info' is handled to accommodate for MP validation. The 'stdorder>digform>digtinfo>formname' (which the Wizard auto-populates) will now be removed, along with 'fees' if a user selects the 'Custom' option. Thanks to P. Haggerty for identifying this issue.

##### Metadata Wizard version 1.5 (Updated 02052015)
- New toolbox will run against ESRI ArcGIS Version 10.3.
- More robust error checking and element handling when running MP through the 'Metadata Validation' tab. The Distribution Info section should be handled properly now.
- Added new field, 'Description of Geographic Extent' that will activate when a user selects the BDP option to allow entry of this information.
- Resolved a bug related to beginning/end dates of a source input being confused on a record loads into the Main Editor. Thanks to D. Hockman-Wert for identifying this issue.
- Fixed help tips on Source Input and Process Step tabs (tab numbers 2-n). Thanks to D. Hockman-Wert for identifying this issue.
- Main Editor GUI will now pull values and write values from the 'metadata/dataqual/lineage/srccite/citeinfo/geoform' element for the 'Type of Data' field in the Source Input tab. Thanks to D. Hockman-Wert for identifying this issue.
- Updated the handling of the metadata preview. MP will now be run to prune off and avoid showing empty nodes. Stylesheet will still be applied for clean viewing.
- Updated the handling of the metadata preview. Upon subsequent user clicks of the 'Preview Metadata' button, the same pop-up window will be used, rather than spawning a new window each time.

##### Metadata Wizard version 1.6 (Updated 06112015)
- Bug fix to resolve an issue with the E/A Builder GUI crashing on raster datasets residing within an ArcGIS file geodatabase. Thanks to C. Jarchow for identifying this issue.

##### Metadata Wizard version 1.7 (Updated 10152015)
- Updated the 'Keywords' section. Worked with Peter Schweitzer to connect the keywords section to his keywords JSON API service. Users can now search/choose from a standardized list of keywords for both 'topic/theme' and 'place' keywords. We also strongly encourage (but don't require) the user to supply at least one ISO keyword. There is a new button which allows users to easily browse and select ISO keywords. An Internet connection is required to communicate with the keywords service. Users can still use the traditional method of creating their own custom keywords.

##### Metadata Wizard version 1.7.1 (Updated 10292015)
- Minor updates for error handling and application UI after the revision to the Keywords section. Updates should resolve unhandled application errors related to clicking the Add or Delete buttons when no keyword or thesaurus had been selected. User help tips have also been added for the new UI fields. Additionally, clicking No Place Keywords radio button will now properly remove any place keywords from the XML if toggled.

##### Metadata Wizard version 1.7.2 (Updated 04302016)
- Compiled a version of the EA builder for ESRI ArcDesktop Version 10.4 Developer note: a bug in Visual Studio prevented the ability to do this with the Community 2015 edition. Visual Studio 2013 was used.
- Updated the URL that the controlled vocabulary thesaurus uses to point to 'www2.usgs.gov' after a bureau-wide change to thesaurus resources. This feature of the tool should now work correctly.
- Resolved an issue that prevented a user from providing a custom Codeset Name and Codeset Source in the EA builder. At v 1.7.2 this fix will only be available for the 10.4 version of the tool. Thanks to N. Nakagaki for identifying this issue.
- There is a known issue in the EA Builder when the tool is run on an ESRI Feature Class inside of an ESRI Feature Dataset with the same exact name in an ESRI Geodatabase. The tool will run successfully if the respective names of the Feature Class and Feature Dataset are not identical. Thanks to K. Kovacs and J.C. Nelson for identifying this issue.
- Minor updates to Python script and validation to streamline ESRI version compatibility. Thanks to C. Price for collaboration with this effort.

##### Metadata Wizard version 1.7.3 (Updated 06272016)
- Updated the URL that the controlled vocabulary thesaurus uses to implement the 'https' protocol after another bureau-wide change to thesaurus resources. This feature of the tool should now work correctly.

##### Metadata Wizard version 1.8 (Updated 01052017)
- Compiled a version of the EA builder for ESRI ArcDesktop Version 10.5 Developer note: a bug in Visual Studio prevented the ability to do this with the Community 2015 edition. Visual Studio 2013 was used.
- Updated the URL that the controlled vocabulary thesaurus uses to implement the 'https' protocol after another bureau-wide change to thesaurus resources. This feature of the tool should now work correctly.
- Replaced MP with version 2.9.26 (slightly older version) to retain white space in certain elements after running the parser.

##### Metadata Wizard version 1.8.1 (Updated 04102017)
- Downsaved the toolbox bundle within ESRI as a version 10.0 toolbox to ensure backwards compatibility with earlier versions of ArcDesktop.

##### Metadata Wizard version 1.8.2 (Updated 04252017)
- Again, downsaved the toolbox bundle within ESRI as a version 10.0 toolbox to ensure backwards compatibility with earlier versions of ArcDesktop to resolve issue.

##### Metadata Wizard version 1.8.3 (Updated 03062018)
- Compiled a version of the EA builder for ESRI ArcDesktop Version 10.6.
- Downsaved the toolbox bundle within ESRI as a version 10.0 toolbox to ensure backwards compatibility with earlier versions of ArcDesktop.

##### Metadata Wizard version 1.8.4 (Updated 08022018)
- Updated process for capturing 'geogunit' element for a raster file in GCS. Metadata Wizard will use the cell size for both Lat Res / Lon Res and for abscissa res / ordinate res. The unit will be the linear unit for the GCS. Thanks to D. Hockman-Wert for feedback.