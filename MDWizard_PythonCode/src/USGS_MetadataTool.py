"""
Authors:  Drew Ignizio, Michael O'Donnell, Colin Talbert
Created:  04/26/2012
Script Purpose/Notes:

This tool is designed as a resource to help geospatial data users with the creation and editing of metadata compliant
with the Federal Geographic Data Committee's 'Content Standard for Digital Geospatial Metadata' (FGDC-CSDGM).

After any existing metadata has been extracted from the input data set, the tool will supplement the spatial domain
information and certain components of the spatial data organization, spatial reference, and entity/attribute sections
of the metadata record with information inherent in the data set. Users will then be provided with a graphical user
interface (GUI) to enter additional metadata information. While some defaults are provided, a user must ensure that
all required fields are populated with valid information to produce a fully compliant metadata record of quality.

When the tool finishes running, a completed copy of the metadata file will be re-associated with the original input data set.

A copy of the original, unmodified metadata associated with a data set will also be saved to the 'Working Directory' as a
stand-alone XML file.

"""
import traceback
import os, sys, subprocess, time, datetime
import arcpy
from arcpy import env
from arcpy.sa import *
import _winreg

import ExportFGDC_MD_Utility
import MDTools
import SpatialRefTools
import winsound

# TO ADD A NEW VERSION (say, for 10.5)
# - create file StandAloneEAEditor2_V105.exe
# - add "10.5" to this list
# Updates from C. Price. 

# check for ArcGIS supported version
arcvers = ["10.0", "10.1", "10.2", "10.3", "10.4", "10.5", "10.6", "10.7"]
arcver = arcpy.GetInstallInfo("Desktop")["Version"][:4] # "10.3"
if arcver[:4] not in arcvers:
    arcpy.AddError(("This version of ArcGIS (%s) is not supported "
                    "by the metadata wizard.") % arcver)
    sys.exit()

InputData = arcpy.GetParameterAsText(0)
WorkingDir = arcpy.GetParameterAsText(1)
CreateStandAloneXML = arcpy.GetParameterAsText(2)#Toggle to delete/keep final modified stand-alone XML after it is re-imported into data set.
UseStartTemplate = arcpy.GetParameterAsText(3)#Toggle to run MD Wizard using the custom template saved by the user as the starting point.
CustomStarterTemplate= arcpy.GetParameterAsText(4)
GenericTemplate = os.path.join(os.path.dirname(sys.argv[0]), "GenericFGDCTemplate.xml")

#'Entity and Attribute Builder' tool and 'Metadata Editor' will be shipped with Toolbox.
# A new version of StandaloneEAEditor must be created for each supported version.
installDir = os.path.dirname(os.path.realpath(__file__))
# Standalone EA Editor exe filename tag list: ["10", "101", ...]
arctags = ["10"] + [v.replace(".", "") for v in arcvers[1:]]
SEAEditors = [os.path.join(installDir, "StandAloneEAEditor2_V%s.exe" % arctag) for arctag in arctags]
# exe for for this version
arctag = arctags[arcvers.index(arcver)]
SEAEditor = os.path.join(installDir, "StandAloneEAEditor2_V%s.exe" % arctag)
# other files to check for
MetadataEditor = os.path.join(installDir, "MetadataEditor.exe")
WGS84file = os.path.join(installDir, "WGS 1984.prj")

#Check that the necessary install files are present
for ifile in SEAEditors + [MetadataEditor, WGS84file]:
    if not os.path.exists(ifile):
        arcpy.AddError("\nInstall file missing: %s" % ifile)
        sys.exit(1)

#Check/create the working directory at the user-specified location. This is also checked for in the toolbox validation script.
if not os.path.exists(WorkingDir):
    try:
        os.makedirs(WorkingDir)
    except:
        arcpy.AddMessage("The user-specified working directory could not be located or created. Ensure that write access is granted.")
        sys.exit(1)

###Spatial Reference (reference objects set at global level)-----------------------------------------------
GeogCoordUnits = ["Decimal degrees", "Decimal minutes", "Decimal seconds",
    "Degrees and decimal minutes", "Degrees, minutes, and decimal seconds",
    "Radians", "Grads"]

###WGS 84 GCS File used to get lat/long bounding coordinates. Shipped with ToolBox.
GCS_PrjFile = WGS84file

#Check for Excel spreadsheet, and prompt to export.
if os.path.splitext(InputData)[-1] == ".xls" or os.path.splitext(InputData)[-1] == ".xlsx":
    arcpy.AddWarning("!!!!!!!")
    arcpy.AddWarning("The Metadata Wizard does not operate on Excel files. Please try exporting to a .dbf and re-running the tool.")
    arcpy.AddWarning("!!!!!!!")
    sys.exit(1)

#Determine if input is a stand-alone XML. If so, no attempt will be made to extract spatial data, etc. 'Re-import' to data set won't apply.
if os.path.splitext(InputData)[-1] == ".xml":
    InputIsXML = True
    desc = "XML File"

else:
    InputIsXML = False

    try:
        desc = arcpy.Describe(InputData)#**This 'desc' object is used extensively throughout program.**

    except:
        arcpy.AddMessage("Error trying to execute an ESRI-Describe on input data set.")
        arcpy.AddMessage(arcpy.GetMessages())
        sys.exit(1)

    #-----------
    if desc.DatasetType != "Table":
        try:
            SR_InDS = desc.SpatialReference
        except:
            arcpy.AddMessage("Error trying to define spatial reference for input data set. Check that the data set has a defined projection.")
            arcpy.AddMessage(arcpy.GetMessages())
            sys.exit(1)
    else:
        arcpy.AddMessage("\nA table was passed as the input. No spatial reference will be defined for the data set.")
    #-----------
    try:
        SR_GCS = arcpy.SpatialReference(GCS_PrjFile)
    except:
        arcpy.AddMessage("Error trying to define spatial reference for geographic file.")
        arcpy.AddMessage(arcpy.GetMessages())
        sys.exit(1)

def ProcessRoutine(ArgVariables):
    """Main Function that operates the logic of the script."""
    try:

        arcpy.AddMessage("\nInputData: " + InputData)
        arcpy.AddMessage("WorkingDir: " + WorkingDir)
        arcpy.AddMessage("CreateStandAloneXML: " + CreateStandAloneXML)
        arcpy.AddMessage("UseStartTemplate: " + UseStartTemplate)
        cst_note = CustomStarterTemplate
        if not cst_note:
            cst_note = "(none)"
        arcpy.AddMessage("StarterTemplate: " + cst_note)

        myDataType, myFeatType = Get_Data_Type()#Determine data type, and feature type if applicable
        arcpy.AddMessage("Data type being evaluated: " + myDataType)
        arcpy.AddMessage("Feature type being evaluated: " + myFeatType + "\n")


        SourceFile = os.path.split(os.path.splitext(InputData)[0])[1] #The name of the input file. No extension. No full path.
        OriginalMDRecord = os.path.join(WorkingDir, SourceFile + "_Original.xml")#File pointer to unmodified original.
        FGDCXML = os.path.join(WorkingDir, SourceFile + "_FGDC.xml")#File pointer to the copy we will modify/update.

        #Create and keep 'Original' metadata copy in working directory.
        try:
            MDTools.CreateCopyMDRecord(InputData, OriginalMDRecord)
        except:
            pass

        #After we made a copy of the input's original MD, start process from custom template if it is toggled.
        if str(UseStartTemplate) == "true":
            try:
                arcpy.MetadataImporter_conversion(CustomStarterTemplate, InputData) # This imports only: does not convert and does not sync
                arcpy.AddMessage("The user's custom starter record is now being imported into the input data set...\n")
            except:
                arcpy.AddWarning("!!!!!!!")
                arcpy.AddWarning("There was a problem importing from the Custom Starter Template. Please ensure that the file is here: (" + CustomStarterTemplate + ")")
                arcpy.AddWarning("!!!!!!!\n")
                sys.exit(1)

        try:#Extract any existing metadata, and translate to FGDC format if necessary.
            ExportFGDC_MD_Utility.GetMDContent(InputData, FGDCXML, WorkingDir)#Export (translate if necessary) input metadata to FGDC format. Remove ESRI 'sync' & 'reminder' elements.
        except:
            arcpy.AddMessage("No metadata could be found for this record. A new file will be created.\n")
            MDTools.CreateCopyMDRecord(GenericTemplate, FGDCXML)

        MDTools.RemoveNameSpace(FGDCXML)#Eliminate namespace tags from root element in xml if present (appear when tool is run on spatial data sets).
        MDTools.CheckMasterNodes(FGDCXML)#Ensure all the key FGDC-CSDGM nodes are present in the record.


        if InputIsXML == False and desc.DatasetType != "Table": #Only attempt to extract/update spatial properties from spatial data sets.

            try:
                GCS_ExtentList = Get_LatLon_BndBox()[1]
            except:
                arcpy.AddWarning("!!!!!!!")
                arcpy.AddWarning("A problem was encountered when attempting to retrieve the spatial extent of the input data set. Please review the tool documentation and ensure the data set is a valid input and ENSURE THAT A COORDINATE SYSTEM HAS BEEN DEFINED.")
                arcpy.AddWarning("!!!!!!!\n")
                sys.exit()

            #Get/Update Bounding Coordinates
            GCS_ExtentList = Get_LatLon_BndBox()[1]
            Local_ExtentList = Get_LatLon_BndBox()[0]
            if "nan" in str(Local_ExtentList):
                arcpy.AddWarning("No spatial extent could be found for the input spatial data set. Please review the 'Bounding Extent' in the final metadata record. (Values will be set to maximum global extent).\n")
            arcpy.AddMessage("Bounding Coordinates (Local):        " + ("{:15.5f} " * 4).format(*Local_ExtentList) + "\n")
            arcpy.AddMessage("Bounding Coordinates (Geographic):   " + ("{:15.5f} " * 4).format(*GCS_ExtentList) + "\n")

            WestBC = Get_LatLon_BndBox()[1][0]
            EastBC = Get_LatLon_BndBox()[1][2]
            NorthBC = Get_LatLon_BndBox()[1][3]
            SouthBC = Get_LatLon_BndBox()[1][1]
            MDTools.WriteBoundingInfo(FGDCXML, WestBC, EastBC, NorthBC, SouthBC)

            #Get/Update Spatial Data Organization
            SpatialDataOrgInfo = Get_Spatial_Data_OrgInfo(InputData, myDataType, myFeatType)
            MDTools.WriteSpatialDataOrgInfo(FGDCXML, SpatialDataOrgInfo)

            #Get/Update Spatial Reference Information
            SpatialReferenceInfo = SpatialRefTools.SpatialRefInfo(GCS_PrjFile, InputData, WorkingDir, GCS_ExtentList)
            MDTools.WriteSpatialRefInfo(FGDCXML, SpatialReferenceInfo)
            #Handle vertical coordinate system?

        #Get/Update Geospatial Presentation Form. Also updates Format Name (within Distribution Info).
        #(Skip this step and leave existing content if tool input is XML).
        if InputIsXML == False:
            MDTools.WriteGeospatialForm(FGDCXML, myDataType, myFeatType)

        #Get/Update Native Environment Details
        #This will be used as a switch to determine which .exe for the EA builder needs to be run (for either 10.0, 10.1, or 10.2).
        #The version info is also written out to the XML record in the 'Native Environment' section.
        ESRIVersion = GetESRIVersion_WriteNativeEnv(FGDCXML)

        #Get/Update Metadata Date of Editing
        Now = datetime.datetime.now()
        MDDate = Now.strftime("%Y%m%d")
        MDTools.WriteMDDate(FGDCXML, MDDate)

        #Update Entity/Attribute Section
        if InputIsXML == False:
            EAtextfile = os.path.join(WorkingDir, "EAInfo.txt")

##            if ESRIVersion == "10.0":
##                ftag = "10"
##            else:
##                ftag = ESRIVersion.replace(".", "") # 10.1 -> 101
##            EATool = os.path.join(installDir,
##                                  "StandAloneEAEditor2_V%s.exe" % ftag)

            Arg = '"%s" "%s" "%s"' % (SEAEditor, InputData, EAtextfile) #Start and end quotes are necessary to handle spaces in file names when passing to Command Prompt.

            arcpy.AddMessage("*************************")
            arcpy.AddMessage("\nPLEASE UPDATE THE ENTITY/ATTRIBUTE INFORMATION IN THE POP-UP WINDOW.")
            arcpy.AddMessage("(Allow a moment for the window to open-- this may take several minutes for larger data sets).\n")
            arcpy.AddMessage("*************************")
            try:
                winsound.PlaySound(r"C:\Windows\Media\Cityscape\Windows Exclamation.wav", winsound.SND_FILENAME)
            except:
                pass
            #os.popen(Arg)
            p = subprocess.Popen(Arg)
            p.wait()


            try:
                EAInfo = MDTools.RetrieveEAInfo(EAtextfile)
                MDTools.WriteEAInfo(FGDCXML, EAInfo)
            except:
                arcpy.AddWarning("No content was saved in the Entity / Attribute tool. The metadata element was not updated.\n")

        #Rerun FGDC Translator tool to handle newly-added elements that are out of order in XML tree.
        MDTools.ReRunFGDCTranslator(FGDCXML)

        #Re-import new metadata to the data set to capture E/A tool changes. If input file is a stand alone .xml this step is skipped
        if InputIsXML == False:
            try:
                arcpy.MetadataImporter_conversion(FGDCXML, InputData) # This imports only: does not convert and does not sync
            except:
                print "There was a problem during the metadata importation process."


        #Open up Metadata Editor and allow user to review/update
        outXML = os.path.splitext(FGDCXML)[0] + "temp.xml"
        #Arg = '"' + MetadataEditor + '"' + " " + '"' + FGDCXML + '"' + " " + '"' + outXML + '"' + " " + '"' + Browser + '"' #Start and end quotes are necessary to handle spaces in file names and IE Path when passing to Command Prompt.
        #Arg = '"' + MetadataEditor + '"' + " " + '"' + FGDCXML + '"' + " " + '"' + outXML + '"' + " "
        Arg = '"%s" "%s" "%s"' % (MetadataEditor, FGDCXML, outXML)
        arcpy.AddMessage("*************************")
        arcpy.AddMessage("\nPLEASE UPDATE/REVIEW THE METADATA INFO IN THE POP-UP WINDOW.")
        arcpy.AddMessage("(Allow a moment for the window to open).\n")
        arcpy.AddMessage("*************************")
        try:
            winsound.PlaySound(r"C:\Windows\Media\Cityscape\Windows Exclamation.wav", winsound.SND_FILENAME)
        except:
            pass
        #os.popen(Arg)
        p = subprocess.Popen(Arg)
        p.wait()


        try:
            MDTools.RemoveStyleSheet(outXML)#MP actually removes the stylesheet in VB.NET app... this is a redundancy here.
            MDTools.ReplaceXML(FGDCXML, outXML)
        except:
            arcpy.AddWarning("No content was saved in the Metadata Editor window. The metadata record was not updated.\n")



        #Re-import new metadata to the data set to capture user edits from the Metadata Editor window.
        try:
            arcpy.MetadataImporter_conversion(FGDCXML, InputData) # This imports only: does not convert and does not sync
            arcpy.AddMessage("The updated metadata record is now being re-imported into the input data set...\n")
        except:
            arcpy.AddMessage("There was a problem during the metadata importation process!")

        #Remove the Error Report file generated by MP from the Main Metadata Editor.
        MP_ErrorReport = os.path.splitext(FGDCXML)[0] + "temp_MP_ErrorReport.xml"
        try:
            os.remove(MP_ErrorReport)
        except:
            pass

        #Remove FGDC XML file if the toggle to preserve 'stand-alone' file is configured to FALSE. This appears to be passed as a string rather than boolean.
        if str(CreateStandAloneXML) == "false":
            try:
                arcpy.Delete_management(FGDCXML)
                arcpy.AddMessage("The Wizard will now remove the stand-alone FGDC XML, as requested in the tool interface...\n")
            except:
                arcpy.AddMessage("There was a problem removing the stand-alone XML file. Try removing the file (%s) manually from the working directory.\n" % FGDCXML)

        #Remove the 'ArcpyTranslate.xml' temp file that gets created when exporting from ESRI metadata to FGDC.
        try:
            os.remove(os.path.join(WorkingDir, 'ArcpyTranslate.xml'))
        except:
            pass


    except arcpy.ExecuteError:
        arcpyError()
    except:
        pythonError()


def Get_Data_Type():#Determine what type of data set is being evaluated

    ### Define type of data set

    if InputIsXML == True:
        myDataType = "XML File"
        myFeatType = "None"
        return myDataType, myFeatType

    else:
        if desc.DatasetType == "RasterDataset":
            myDataType = "Raster"
        elif desc.DatasetType == "FeatureClass":
            myDataType = "Vector"
        elif desc.DatasetType == "ShapeFile": # This does not seem to occur any more, but keep for now
            myDataType = "Vector"
        elif desc.DatasetType == "Table":
            myDataType = "Table"
        elif desc.DatasetType == "FeatureDataset":
            arcpy.AddWarning("!!!!!!!")
            arcpy.AddWarning("This is a feature dataset (e.g., a coverage or multiple feature classes).  This tool can only be run on feature classes.")
            arcpy.AddWarning("!!!!!!!")
            sys.exit(1)
        elif desc.DatasetType == "GeometricNetwork":
            myDataType = "GeometricNetwork"
        else:
            arcpy.AddWarning("The provided data set does not appear to be a valid input. Please review the tool documentation.")
            sys.exit(1)

        ### Define type of shape for non raster datasets
        if myDataType not in ["Raster", "Table", "FeatureDataset", "GeometricNetwork"]:
            if desc.shapeType == "Polygon":
                myFeatType = "Polygon"
            elif desc.shapeType == "Polyline":
                myFeatType = "Polyline"
            elif desc.shapeType == "Point":
                myFeatType = "Point"
            elif desc.shapeType == "MultiPoint" or desc.shapeType == "Multipoint":
                myFeatType = "Point"
            else:
                arcpy.AddWarning("The feature type for the provided data set could not be determined. It will be set to 'None'.")
                myFeatType = "None"
        elif myDataType == "Raster":
            myFeatType = "None"
        elif myDataType == "Table":
            myFeatType = "None"
        elif myDataType == "FeatureDataset":
            myFeatType = "None"
        elif myDataType == "GeometricNetwork":
            myFeatType = "None"
        else:
            arcpy.AddWarning("!!!!!!!")
            arcpy.AddWarning("The provided data set does not appear to be a valid input. Please review the tool documentation.")
            arcpy.AddWarning("!!!!!!!")
            sys.exit(1)

        ### Return desired objects
        return myDataType, myFeatType

def GetESRIVersion_WriteNativeEnv(FGDCXML):
    """
    Gets the version of ESRI being run on a user's machine.
    Additionally, this information is written out to the new XML record that is being created.
    """
    ESRIVersion = ""
    NativeInfo = Get_NativeEnvironment()
    arcpy.AddMessage(NativeInfo + "\n")
    MDTools.WriteNativeEnvInfo(FGDCXML, NativeInfo)
    # Identify version of ArcGIS
    Esri_version = "UNKNOWN"
    # arcvers list was set up at the beginning of the script
    for arcver in  arcvers:
        if "ArcGIS %s" % arcver in NativeInfo:
            ESRIVersion = arcver
            break
    return ESRIVersion

def Get_LatLon_BndBox(): # Determine lat/long bounding coordinates for input dataset, when applicable

    ### Get extent and spatial reference of input dataset
    extent = desc.Extent

    Local_ExtentList = [float(extent.XMin), float(extent.YMin), \
        float(extent.XMax), float(extent.YMax)]

    if float(extent.XMin) >= -180 and float(extent.XMax) <= 180 and \
        float(extent.YMin) >= -90 and float(extent.YMax) <= 90:
        GCS_ExtentList = Local_ExtentList
        # Local extent is GCS so do not need to project coords in order to obtain GCS values
        return Local_ExtentList, GCS_ExtentList
    else:
        ### Create geographic bounding coordinates
        # Create 2 point list for LL and UR
        # For each axis, create a center point by (xmin + xmax)/2 and this would be coord (do not need to add or subtract)
        #   And same for lat: (ymin + ymax)/2 and this would be coord (do not need to add or subtract)
        # Need total of 8 points
        x_mid6 = float(float(extent.XMin) + float(extent.XMax)/2*0.25)
        x_mid7 = float(float(extent.XMin) + float(extent.XMax)/2)
        x_mid8 = float(float(extent.XMin) + float(extent.XMax)/2*0.75)
        y_mid2 = float(float(extent.YMin) + float(extent.YMax)/2*0.25)
        y_mid3 = float(float(extent.YMin) + float(extent.YMax)/2)
        y_mid4 = float(float(extent.YMin) + float(extent.YMax)*0.75)
        # Point ID in list
        # 5   6   7   8   9
        #
        # 4               10
        #
        # 3               11
        #
        # 2               12
        #
        # 1  16  15  14   13
        PtList = [[float(extent.XMin), float(extent.YMin)],
                [float(extent.XMin), y_mid2],
                [float(extent.XMin), y_mid3],
                [float(extent.XMin), y_mid4],
                [float(extent.XMin), float(extent.YMax)],
                [x_mid6, float(extent.YMax)],
                [x_mid7, float(extent.YMax)],
                [x_mid8, float(extent.YMax)],
                [float(extent.XMax), float(extent.YMax)],
                [float(extent.XMax), y_mid4],
                [float(extent.XMax), y_mid3],
                [float(extent.XMax), y_mid3],
                [float(extent.XMax), float(extent.YMin)],
                [x_mid8, float(extent.YMin)],
                [x_mid7, float(extent.YMin)],
                [x_mid6, float(extent.YMin)],
                [float(extent.XMin), float(extent.YMin)]]

        # Create an empty Point object
        point = arcpy.Point()
        array = arcpy.Array()

        # For each coordinate pair, populate the Point object and create
        #  a new PointGeometry
        for pt in PtList:
            point.X = pt[0]
            point.Y = pt[1]
            pointGeometry = arcpy.PointGeometry(point, SR_InDS)
            array.add(point)

        # Create a Polygon object based on the array of points
        boundaryPolygon  = arcpy.Polygon(array, SR_InDS)
        array.removeAll()

        # Instead of projecting point, project polygon
        OutSR = arcpy.SpatialReference(GCS_PrjFile)
        arcpy.env.outputCoordinateSystem = arcpy.SpatialReference(GCS_PrjFile)

        # Return a list of geometry objects (we only have one polygon) using a geographic coordinate system
        boundaryPolygon2 = arcpy.CopyFeatures_management(boundaryPolygon, arcpy.Geometry())

        # Each feature is list item which has its own geometry (therefore we pull the first and only polygon)
        GCSextent = boundaryPolygon2[0].extent

        # Get boundary extent
        GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax = float(GCSextent.XMin), float(GCSextent.YMin), \
            float(GCSextent.XMax), float(GCSextent.YMax)
        #print "GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax:", GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax
        GCS_ExtentList = [GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax]
        del pointGeometry, PtList, point, OutSR, GCSextent, boundaryPolygon, boundaryPolygon2, array

    return Local_ExtentList, GCS_ExtentList

def Get_Spatial_Data_OrgInfo(InputDS, myDataType, myFeatType):

    #Does not handle VPF data as this is rare/unseen.

    ### Indirect ==========================================
    #Consider adding indirect spatial reference? Leave this out for now.
    #indspref = "<indspref>[Insert a descriptive location reference here]</indspref>"

    ### Direct Spatial Reference===========================
    Direct_Spatial_Reference_Method = ["Point", "Vector", "Raster"]

    # myDataType = ["Raster", "Vector"]
    # myFeatType = ["Polygon", "Polyline", "Point", "None"] # 'None' will apply to: Raster, Table, feature DS, XML File

    if myDataType == "Vector":
        if myFeatType == "Point":
            DirectSpatialRef = "<direct>" + Direct_Spatial_Reference_Method[0] + "</direct>"
        else:
            DirectSpatialRef = "<direct>" + Direct_Spatial_Reference_Method[1] + "</direct>"
    if myDataType == "Raster":
        DirectSpatialRef = "<direct>" + Direct_Spatial_Reference_Method[2] + "</direct>"



    ### Point and Vector object information=================

    if myFeatType in ["Point", "Polyline"]:
        if myFeatType == "Point":
            SDTS_Type = "Entity point" # Usually the type should be this versus "Point"--see meta standard
        if myFeatType == "Polyline":
            # In most cases these will be accurate, but in some cases the user may want to change
            #   to a different type that is more specific
            if os.path.splitext(InputDS)[1] == ".shp":
                SDTS_Type = "String" # shapefiles can never have topology
            else:
                SDTS_Type = "Link" # other feature classes MAY have topology

        try: ObjCount = str(arcpy.GetCount_management(InputDS))
        except:
            arcpy.AddMessage("Error obtaining object count for the vector data set. The count information will be left blank. \n")
            ObjCount = 0

        if ObjCount != 0:
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "<ptvctcnt>" + ObjCount + "</ptvctcnt>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo

        elif ObjCount == 0:#Omit object count if we cound't obtain it.
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo


    elif myFeatType == "Polygon":
        SDTS_Type = "G-polygon"
        try: ObjCount = str(arcpy.GetCount_management(InputDS))
        except:
            arcpy.AddMessage("Error obtaining object count for vector (polygon) data set. The count information will be left blank. \n")
            ObjCount = 0

        if ObjCount != 0:
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "<ptvctcnt>" + ObjCount + "</ptvctcnt>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo

        elif ObjCount == 0:#Omit object count if we cound't obtain it.
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo


    elif myDataType == "GeometricNetwork":
        SDTS_Type = "Network chain, nonplanar graph"

        # Locate Polyline feature class within network data
        NetDS = "" # Clear
        NetWS = os.path.dirname(InputDS)
        desc = arcpy.Describe(InputDS)
        FClist = desc.featureClassNames #Returns the names of all features participating in topology.
        for iFClist in FClist:
            try:
                desc2 = arcpy.Describe(os.path.join(NetWS, iFClist))
                if desc2.shapeType == "Polyline":
                    NetDS = os.path.join(NetWS, iFClist)
            except:
                pass
            if arcpy.Exists(NetDS):
                try: ObjCount = str(arcpy.GetCount_management(NetDS))
                except:
                    arcpy.AddMessage("Error obtaining object count for vector (line) data set. The count information will be left blank.\n")
                    ObjCount = 0
            else:
                arcpy.AddMessage("Error obtaining object count for vector (line) data set. The count information will be left blank.\n")
                ObjCount = 0

        if ObjCount != 0:
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "<ptvctcnt>" + ObjCount + "</ptvctcnt>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo

        elif ObjCount == 0:#Omit object count if we cound't obtain it.
            PVOI = \
            "<ptvctinf><sdtsterm>" + \
            "<sdtstype>" + SDTS_Type + "</sdtstype>" + \
            "</sdtsterm></ptvctinf>"

            SpatialDataOrgInfo = DirectSpatialRef + PVOI
            return SpatialDataOrgInfo


    ### Raster object information ================================
    elif myDataType == "Raster":
        # Raster_Object_Type = ["Point", "Pixel", "Grid Cell", "Voxel"](options)
        RasterType = "Grid Cell" # This is the most probable answer
        try:
            RowCount = str(arcpy.GetRasterProperties_management(InputDS, "ROWCOUNT"))
            ColCount = str(arcpy.GetRasterProperties_management(InputDS, "COLUMNCOUNT"))
            BandCount = str(arcpy.GetRasterProperties_management(InputDS, "BANDCOUNT"))

            ROI = \
            "<rastinfo>" + \
            "<rasttype>" + RasterType + "</rasttype>" + \
            "<rowcount>" + RowCount + "</rowcount>" + \
            "<colcount>" + ColCount + "</colcount>" + \
            "<vrtcount>" + BandCount + "</vrtcount>" + \
            "</rastinfo>"

            SpatialDataOrgInfo = DirectSpatialRef + ROI
            return SpatialDataOrgInfo

        except:
            arcpy.AddMessage("Error obtaining row/column count information for the raster data set. The count information will be left blank.\n")

            #Omit row, column, and band count if unable to extract.
            ROI = \
            "<rastinfo>" + \
            "<rasttype>" + RasterType + "</rasttype>" + \
            "</rastinfo>"

            SpatialDataOrgInfo = DirectSpatialRef + ROI
            return SpatialDataOrgInfo

def Get_NativeEnvironment():
    """
    ------------------------------------
    winver = sys.getwindowsversion()
          tuple containing five components, describing the Windows version
          major, minor, build, platform, and SP as text
          Constant Platform
          0 (VER_PLATFORM_WIN32s) Win32s on Windows 3.1
          1 (VER_PLATFORM_WIN32_WINDOWS) Windows 95/98/ME
          2 (VER_PLATFORM_WIN32_NT) Windows NT/2000/XP
          3 (VER_PLATFORM_WIN32_CE) Windows CE

        (6, 1, 7601, 2, 'Service Pack 1')
    ------------------------------------

    Windows 7    6.1
    Windows Server 2008 R2    6.1
    Windows Server 2008    6.0
    Windows Vista    6.0
    Windows Server 2003 R2    5.2
    Windows Server 2003    5.2
    Windows XP 64-Bit Edition    5.2
    Windows XP    5.1
    Windows 2000    5.0

    ------------------------------------
    sys.platform
        'win32'
    ------------------------------------
    Linux (2.x and 3.x)     'linux2'
    Windows     'win32'
    Windows/Cygwin     'cygwin'
    Mac OS X     'darwin'
    OS/2     'os2'
    OS/2 EMX     'os2emx'
    RiscOS     'riscos'
    AtheOS     'atheos'

    ------------------------------------
    os.name
    nt
    ------------------------------------
    """


    if os.name == 'nt':

        try:
            osVer = os.sys.getwindowsversion()

            OS_Ver = osVer[3], osVer[0], osVer[1]
            OS_VerStr = str(osVer[0]) + "." + str(osVer[1])
            OS_Build = str(osVer[2])
            OS_SP = str(osVer[4])

            if OS_Ver == (2, 6, 1):
                OS_Str = "Windows 7"
                #OS_Str = "Windows Server 2008 R2"
            elif OS_Ver == (2, 6, 0):
                OS_Str = "Windows Server 2008"
                #OS_Str = "Windows Vista"
            elif OS_Ver == (2, 5, 2):
                OS_Str = "Windows Server 2003 R2"
                #OS_Str = "Windows Server 2003"
                #OS_Str = "Windows XP 64-Bit Edition"
            elif OS_Ver == (2, 5, 1):
                OS_Str = "Windows XP"
            elif OS_Ver == (2, 5, 0):
                OS_Str = "Windows 2000"
                #OS_Str = "Windows 2k"
            elif OS_Ver == (2, 4, 0):
                OS_Str = "Windows NT"
            elif OS_Ver == (1, 4, 90):
                OS_Str = "Windows ME"
            elif OS_Ver == (1, 4, 10):
                OS_Str = "Windows 98"
            elif OS_Ver == (1, 4, 0):
                OS_Str = "Windows 95"
            else:
                OS_Str = "[Unknown]"
        except:
            OS_Str = "[Unknown]"
            OS_VerStr = "[Unknown]"
            OS_Build = "[Unknown]"
            OS_SP = "[Unknown]"
    else:
        OS_Str = "[Unknown]"
        OS_VerStr = "[Unknown]"
        OS_Build = "[Unknown]"
        OS_SP = "[Unknown]"



    # ESRI version
    try:
        agis = arcpy.GetInstallInfo()
        desktopVer = agis["Version"]
        desktopBuildVer = agis["BuildNumber"]

        try:
            ESRI_SP = agis["SPNumber"]
            ESRI_SP_Build = agis["SPBuild"]
        except:
            ESRI_SP = "[N/A]"
            ESRI_SP_Build = "[N/A]"
    except:
            desktopVer = "[Unknown]"
            desktopBuildVer = "[Unknown]"
            ESRI_SP = "[Unknown]"
            ESRI_SP_Build = "[Unknown]"

    NativeStr = "Environment as of Metadata Creation: Microsoft %s Version %s (Build %s) %s; " % (
        OS_Str, OS_VerStr, OS_Build, OS_SP)
    NativeStr += "Esri ArcGIS %s (Build %s) Service Pack %s (Build %s)" % (
            desktopVer, desktopBuildVer, ESRI_SP, ESRI_SP_Build)

    return NativeStr

def msg(message, is_error):
    """Print a message to console and ArcGIS
    Args: message - the message to print
          is_error - Add Message or Add Error
    Returns: None"""

    if is_error:
        arcpy.AddError(message)
    else:
        arcpy.AddMessage(message)
    print message

def arcpyError():
    """Adds arcpy error messages to a print statement and ArcGIS window."""

    msg("Script Failed", 1)
    msg(arcpy.GetMessages(2), 1)
    sys.exit(1)

def pythonError():
    """Adds python error messages to a print statement and ArcGIS window."""
    msg("Non-Arcpy error occured", 1)
    # Get the traceback object
    tb = sys.exc_info()[2]
    tbinfo = traceback.format_tb(tb)[0]
    # Concatenate information together concerning the error into a string
    pymsg = "PYTHON ERRORS:\nTraceback info:\n" + tbinfo + \
             "\nError Info:\n" + str(sys.exc_info()[1])
    msgs = "Arcpy Errors:\n" + arcpy.GetMessages(2) + "\n"
    # Return python error messages for use in script tool or Python Window
    msg(pymsg, 1)
    msg(msgs, 1)
    sys.exit(1)

if __name__ == '__main__':
    ProcessRoutine(sys.argv)
#    ProcessRoutine(["N:\Active\FORT_WaddleLidar\DerivedData\bc_int",
#                   "C:\temp\MetadataWizard",
#                    True,
#                    "C:\Program Files (x86)\Internet Explorer",
#                    False,
#                    " "" ""])
    print "Tool has completed running."

