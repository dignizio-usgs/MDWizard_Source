Metadata Wizard
===============

##### Metadata Wizard version Beta release (Updated 122013)
- Updated help tips and default values.
- Ability to load from a template XML file.
- Auto import contact now works for all USGS personnel.
- Updated ESRI ArcToolbox interfaceparameter configuration.
- Updated Python routine to calculate Longitudinal and Latitudinal resolution.
- Updated default values for EntityAttribute overview.
- Dedicated link in ScienceBase where the tool can be downloaded.
 
##### Metadata Wizard version 1.0 (Updated 122014)
- Handle null valuesall empty fields in input data sets.
- Handles feature classes within file and personal GDB.
- Internal VB.Net metadata preview capacity (no longer dependent on IE).
- Updated ESRI ArcToolbox interfaceparameter configuration.
- Users can specify any metadata template of their choice.
- Updated toolbox documentationtool input validation.
 
##### Metadata Wizard version 1.1 (Updated 01272014)
- Key element check implemented. Wizard now checks forensures the presence of the following ['idinfo', 'dataqual', 'spdoinfo', 'spref', 'eainfo', 'distinfo', 'metainfo'].
This resolves the issue of the tool hanging up on �Updating Digital Transfer Info � Format Name� (and other instances resulting from missing nodes).
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

##### Metadata Wizard tweaks by Curtis Price (06112014)
- Built "clean" tbx file using ArcGIS 10.1 (saved as 10.0) to avoid double tool display
- Various tweaks to USGS_MetadataTool.py to improve maintainability
- **Modified validation code in tbx:**
- Create working folder in user TEMP (not C:\temp)
- Modified validation to use type "Folder" not "Workspace" for metadata working folder
- If folder is not available, standard ArcGIS error message is displayed
