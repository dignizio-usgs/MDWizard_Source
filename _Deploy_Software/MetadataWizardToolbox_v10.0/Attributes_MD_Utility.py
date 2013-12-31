#!/usr/bin/env python
# Module     : Metadata_attributes.py
# Synopsis   : Compute information for metadata content with regard to entity and attribute information.
# Programmer : Michael O'Donnell
# Date       : 01/13/2009
# Outputs    : Information printed to screen as well as an ASCII file. Contents may not all be applicable
#            :   to metadata. Therefore it is up to the user to know how to use this information.
#            :   Vector attribute information will be inserted in: of the metadata.
#            :   Raster attribute information will be inserted in: of the metadata.
# Notes      : Vector data: does not handle all field types. Refer to field type evaluation section
#            :   for specific information. Because integer fields could be either categorical or numeric
#            :   they are evaluated for both.
#            : Raster data: Currently this examines the value field and no others. I am not sure if other
#            :   fields can be evaluated or how this is done. The other issue is that it is only handling
#            :   a single band. I am not sure how to obtain info on separate bands.
#            :
# Arguments  :
#
# This program will work on the following:
#  multi-band and single band raster datasets that ESRI can read
#  Feature classes (The feature class can be nested within a feature dataset, but
#    do not specify a feature dataset because metadata needs to exist at the feature class level
#    You may need to create a feature dataset metadata, but I am not sure how this is supposed to be handled
#    in metadata.
########################################################################################################
# Standard python modules
import os, string, math, sys, subprocess
from datetime import datetime

# Non-standard modules: ESRI
import arcpy
from arcpy.sa import *
from arcpy import env

#######################  Check/Get ESRI licence
try:
    if arcpy.CheckProduct("ArcInfo") == "Available":
        arcpy.SetProduct("ArcInfo")
except:
    print "Possible problem with ESRI license."
    print arcpy.GetMessages()
    sys.exit(1)



#####################   Arguments via GUI (All strings)
'''
                                         # All arguments are required but some can accept "#" for first run
InXML = sys.argv[1]                      # string: Input XMl template
OutXMLws = sys.argv[2]                   # string: Output XML WS (Defaults to either InDS basename or with _1.xml)
InDS = sys.argv[3]                       # string: Input dataset
FieldListMeta = sys.argv[4]              # string: Attribution assignment. Use "#" on first run then populate with content
                                         #   provided in notepad--will convert to GUI at some point
DataScale = int(sys.argv[5])             # Integer: Scale of vector dataset (not applicable to raster and will be ignored)
                                         #   Used for calculating lat/long res
Vertical_CS_Switch = sys.argv[6]         # string: "Present" | "Absent" (If data is a DEM use Present, otherwise use Absent)
'''

#'''
# Hardcoded testing
#InXML = r"K:\USERS\ISB\GIS\ODONNELL\AK_LatRes.shp.xml"
InXML = r"\\Igskbacbfs001\gis$\Active\ODONNELL\Habitat_SG\Outsource_data\NGS\Shapefiles\Albany\Albany.shp.xml"
OutXMLws = r"C:\temp"
#InDS = r"K:\USERS\ISB\GIS\ODONNELL\AK_LatRes.shp"
InDS = r"\\Igskbacbfs001\gis$\Active\ODONNELL\Habitat_SG\Outsource_data\NGS\Shapefiles\Albany\Albany.shp"
#FieldListMeta = "[('FID', 'OID', 'RANGE'), ('Shape', 'Geometry', 'ENUM'), ('Id', 'Integer', 'CODESET')]"
FieldListMeta = "[('FID', 'OID', 'RANGE'), ('Shape', 'Geometry', 'ENUM'), ('FeatureId', 'SmallInteger', 'CODESET'), ('DATA_DATE', 'String', 'CODESET'), ('DATA_SRCE', 'String', 'CODESET'), ('DEC_LONG', 'String', 'CODESET'), ('DEC_LAT', 'String', 'CODESET'), ('PID', 'String', 'CODESET'), ('NAME', 'String', 'CODESET'), ('STATE', 'String', 'CODESET'), ('COUNTY', 'String', 'CODESET'), ('QUAD', 'String', 'CODESET'), ('LATITUDE', 'String', 'CODESET'), ('LONGITUDE', 'String', 'CODESET'), ('POS_DATUM', 'String', 'CODESET'), ('DATUM_TAG', 'String', 'CODESET'), ('POS_SRCE', 'String', 'CODESET'), ('ELEVATION', 'String', 'CODESET'), ('ELEV_DATUM', 'String', 'CODESET'), ('ELEV_SRCE', 'String', 'CODESET'), ('ELLIP_HT', 'String', 'CODESET'), ('ELLIP_SRCE', 'String', 'CODESET'), ('POS_ORDER', 'String', 'CODESET'), ('POS_CHECK', 'String', 'CODESET'), ('ELEV_ORDER', 'String', 'CODESET'), ('ELEV_CLASS', 'String', 'CODESET'), ('ELEV_CHECK', 'String', 'CODESET'), ('DIST_RATE', 'String', 'CODESET'), ('ELLP_ORDER', 'String', 'CODESET'), ('ELLP_CLASS', 'String', 'CODESET'), ('FIRST_RECV', 'String', 'CODESET'), ('LAST_RECV', 'String', 'CODESET'), ('LAST_COND', 'String', 'CODESET'), ('LAST_RECBY', 'String', 'CODESET'), ('SAT_USE', 'String', 'CODESET'), ('SAT_DATE', 'String', 'CODESET'), ('STABILITY', 'String', 'CODESET')]"
DataScale = 24000
Vertical_CS_Switch = "Absent"
#'''

### List of unique fields for table or feature class table
# [("FieldName", FieldType", "MetaDomain"), (..., ..., ...), ...]
# ENUM, CODESET, RANGE, UNREP
#FieldListMeta = "[('OBJECTID', 'OID', 'ESRI'), ('Shape', 'Geometry', 'ESRI'),
#   ('AREA', 'Double', 'RANGE'), ('PERIMETER', 'Double', 'RANGE'), ('BLMLANDG_', 'Double', 'RANGE'),
#   ('BLMLANDG_I', 'Double', 'RANGE'), ('OWNER', 'String', 'ENUM'), ('STATE', 'String', 'ENUM'),
#   ('Shape_Length', 'Double', 'RANGE'), ('Shape_Area', 'Double', 'RANGE')]"
try:
    FieldListMeta = eval(FieldListMeta) # convert string to list object
except:
    print "Use recommended format: [(\"FieldName\", \"FieldType\", \"MetaDomain\"), (..., ..., ...), ...]"
    FieldListMeta = []


### ============================================================================
### =========================== HradCoded ======================================
### Validation of passed arguments
if not os.path.exists(InXML):
    print "\tError passing argument.", InXML
    sys.exit(1)
if not os.path.exists(OutXMLws):
    print "\tError passing argument.", OutXMLws
    sys.exit(1)
try: int(DataScale)
except:
    print "\tError passing argument.", DataScale
    sys.exit(1)
if not arcpy.Exists(InDS):
    print "\tError passing argument.", InDS
    sys.exit(1)
if Vertical_CS_Switch not in ["Absent", "Absent"]:
    print "\tError passing argument.", Vertical_CS_Switch
    sys.exit(1)

### Metadata Element Tabls
ElemTab_1 = "\n\t"
ElemTab_2 = "\n\t\t"
ElemTab_3 = "\n\t\t\t"
ElemTab_4 = "\n\t\t\t\t"
ElemTab_5 = "\n\t\t\t\t\t"
ElemTab_6 = "\n\t\t\t\t\t\t"
ElemTab_7 = "\n\t\t\t\t\t\t\t"
ElemTab_8 = "\n\t\t\t\t\t\t\t\t"
ElemTab_9 = "\n\t\t\t\t\t\t\t\t\t"

### Digitizer precision for calculating coordinate resolution--User may want to change this
DigPrecision = 0.001

### SDTS -----------------------------------------------------------------------
# The system of objects used to represent space in the dataset.
Direct_Spatial_Reference_Method = ["Point", "Vector", "Raster"]


### Spatial Reference ----------------------------------------------------------
### If projection does not meet one of these then use free text
GeogCoordUnits = ["Decimal degrees", "Decimal minutes", "Decimal seconds",
    "Degrees and decimal minutes", "Degrees, minutes, and decimal seconds",
    "Radians", "Grads"]


# Use this to get lat/long bounding coordinates--taken from ESRI, but store locally
GCS_PrjFile = os.path.join(os.path.dirname(sys.argv[0]), "WGS 1984.prj")
if not os.path.exists(GCS_PrjFile):
    print "\tMissing file which should be stored in folder where script is located.", GCS_PrjFile
    sys.exit(1)

### Spatial reference objects set at global level
try:
    desc = arcpy.Describe(InDS)
except:
    print "\tError trying to execute an ESRI Describe on input dataset."
    print arcpy.GetMessages()
    sys.exit(1)
#-----------
if desc.DatasetType != "Table":
    try:
        SR_InDS = desc.SpatialReference
    except:
        print "\tError trying to define spatial reference for input dataset."
        print arcpy.GetMessages()
        sys.exit(1)
else:
    print "\tTable passed and will not be creating spatial reference of dataset."
#-----------
try:
    SR_GCS = arcpy.SpatialReference(GCS_PrjFile)
except:
    print "\tError trying to define spatial reference for geographic file."
    print arcpy.GetMessages()
    sys.exit(1)


### -----------------------------------------------------------------------------
### Metadata elements which this program will strip and then populate from an
###   existing template or metadata file
# Remove detailed entity and attribute information (<detailed>)
# Remove spatial reference if it exists in template (<spref>)
# Remove SDTS or Spatial Data Transfer Standards
BeginSearchList = ["<spdom", "<spdoinfo", "<spref", "<eainfo"]
EndSearchList = ["</spdom", "</spdoinfo", "</spref", "</eainfo"]


### Raster Type
# The types and numbers of raster spatial objects in the dataset. This metadata element
#   contains the following sub-elements
# Raster spatial objects used to locate zero-, two-, or three-dimensional locations
#   in the dataset.
Raster_Object_Type = ["Point", "Pixel", "Grid Cell", "Voxel"]


### Cut off for the Number of unique values allowed to consider as CODESET
###   Abblicable for assigning default metadata domains, but overridden
###   with FieldListMeta after user passes
if len(FieldListMeta) == 0:
    UniqCountThreshold = 50
else:
    UniqCountThreshold = 1000


### Temporary files ------------------------------------------------------------
# Scratch workspace default for storing temporary files
ScratchWS_1 = r"c:\temp"
ScratchWS = os.path.join(ScratchWS_1, "temp_meta.gdb")
if not os.path.exists(ScratchWS_1):
    try:
        os.mkdir(ScratchWS_1)
    except:
        print "\tError trying to create a temporary workspace.",
        sys.exit(1)

if not arcpy.Exists(ScratchWS):
    # CreateFileGDB_management (out_folder_path, out_name, {out_version})
    try:
        arcpy.CreateFileGDB_management(ScratchWS_1, os.path.basename(ScratchWS))
    except:
        print "\tError trying to create scratch file geodatabase."
        arcpy.GetMessages()
        sys.exit(1)

# Use to convert multi-part to single-part if required (necessary for entity info
InDS2 = os.path.join(ScratchWS, "temdDS")

# temp text file for FieldListMeta string
Tmp_FieldListMeta = os.path.join(ScratchWS_1, "temp_FieldListMeta.txt")

# Use for getting lat/long bounding box
tmp_CSV = os.path.join(ScratchWS_1, "temp.csv")
tmp_CSV_shp = os.path.join(ScratchWS, "tempCSV")

# Temp raster VAT
MYout_table = os.path.join(ScratchWS, "tempTbl")

# Temp vector table
MYout_table2 = os.path.join(ScratchWS, "tempTbl2")

# Temp metadata file
OutXML_Tmp = os.path.join(ScratchWS_1, "temp_update.xml")
OutXML_b = os.path.join(ScratchWS_1, "temp_parse.xml")

# Final output XML file
#tds = os.path.basename(os.path.splitext(InDS)[0])
tds = os.path.basename(InDS)
OutXML = os.path.join(OutXMLws, tds + ".xml")
# If input XML template has the same name as the output XML
#   (Input dataset + .xml) then append _1.xml. IF this exists
#   and this is not the same as the input then delete
if os.path.exists(OutXML):
    try: os.remove(OutXML)
    except:
        OutXML = os.path.join(OutXMLws, tds + "_1.xml")
        if os.path.exists(OutXML):
            try: os.remove(OutXML)
            except:
                print "\tError trying to delete output XML--please close or try again.", OutXML
                sys.exit(1)


############################################################################################
############################################################################################
############################################################################################

#====================================================================
# Delete any interim files used while modifiying metadata for this program
#====================================================================
def Delete_InterimFiles():

    ### Check for output and delete if these exist
    try:
        if InDS != InDS2:
            if arcpy.Exists(InDS2):
                arcpy.Delete_management(InDS2)
    except: pass
    try:
        if arcpy.Exists(MYout_table):
            arcpy.Delete_management(MYout_table)
    except: pass
    try:
        if arcpy.Exists(MYout_table2):
            arcpy.Delete_management(MYout_table2)
    except: pass
    try:
        if os.path.exists(FileOut):
            os.remove(FileOut)
    except: pass
    try:
        if os.path.exists(tmp_CSV):
            os.remove(tmp_CSV)
    except: pass
    try:
        if arcpy.Exists(tmp_CSV_shp):
            arcpy.Delete_management(tmp_CSV_shp)
    except: pass
    try:
        if os.path.exists(OutXML_Tmp):
            os.remove(OutXML_Tmp)
    except: pass
    try:
        if os.path.exists(OutXML_b):
            os.remove(OutXML_b)
    except: pass
    try:
        if os.path.exists(Tmp_FieldListMeta):
            os.remove(Tmp_FieldListMeta)
    except: pass



#====================================================================
# Pre-processing of XML file
#   Strip contents that this program will insert
#   Preserve any information user may have within metadata file
#     (this may be the original intended template or modified template)
#   The template must meet XML standards for this to work well
#====================================================================
def XML_Parse():

    ### Strip metadata compounds and any child elements from templates.
    ### These will be re-populated with this program if the program does not crash
    count = 0 # Positional index for seleting list items
    for iString in BeginSearchList:
        FileIntR = open(InXML, 'r')
        FileOutW = open(OutXML_b, 'w') # Starts new temp xml file
        Start = False # Boolean declaring that first meta child located
        for line in FileIntR:
            if line.find(iString) >= 0:
                # Found starting compound element
                # Do not write subsequent lines to output XML but incldue this line
                Start = True
                FileOutW.write(line)
            elif line.find(EndSearchList[count]) >= 0:
                # Found end compound element
                # Note that if there are errors in the metadata where no XML
                #   begin and end elements end this will fail
                # We could search for next child element, but it is possible these will
                #   not exist so for now assume the user has not messed anything up
                Start = False
                FileOutW.write(line)
            else:
                Start = False
                # Write these to output xml
                FileOutW.write(line)
        count += 1

    # Clean up
    FileOutW.close()
    del FileOutW


#====================================================================
# Determine what type of data set is being evaluated
#====================================================================
def Data_Type():

    ### Define type of data set
    if desc.DatasetType == "RasterDataset":
        myDataType = "Raster"
    if desc.DatasetType == "FeatureClass":
        myDataType = "Vector"
    if desc.DatasetType == "ShapeFile": # This does not seem to occur any more, but keep for now
        myDataType = "Vector"
    if desc.DatasetType == "Table":
        myDataType = "Table"
    if desc.DatasetType == "FeatureDataset":
        print "This is a feature dataset and we are requiring feature classes to have individual metadata."
        sys.exit(1)
    if desc.DatasetType == "GeometricNetwork":
        myDataType = "GeometricNetwork"

    ### Define type of shape for non raster datasets
    if myDataType not in ["Raster", "Table", "FeatureDataset", "GeometricNetwork"]:
        if desc.shapeType == "Polygon":
            myFeatType = "Polygon"
        if desc.shapeType == "Polyline":
            myFeatType = "Polyline"
        if desc.shapeType == "Point":
            myFeatType = "Point"
        if desc.shapeType == "MultiPoint":
            myFeatType = "Point"
    elif myDataType == "Raster":
        myFeatType = "None"
    elif myDataType == "Table":
        myFeatType = "None"
    elif myDataType == "FeatureDataset":
        myFeatType = "None"
    elif myDataType == "GeometricNetwork":
        myFeatType = "None"

    ### Return desired objects
    return myDataType, myFeatType


#====================================================================
# Determine lat/long bounding coordinates for input dataset
#   (if necessary)
#====================================================================
def Get_LatLon_BndBox():

    '''

    '''


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

        # Instead of projecting point project polygon
        OutSR = arcpy.SpatialReference(GCS_PrjFile)
        arcpy.env.outputCoordinateSystem = arcpy.SpatialReference(GCS_PrjFile)

        # Return a list of geometry objects (we only have one polygon) using a geographic coordinate system
        boundaryPolygon2 = arcpy.CopyFeatures_management(boundaryPolygon, arcpy.Geometry())

        # Each feature is list item which has its own geemetry (therefore we pull the first and only polygon)
        GCSextent = boundaryPolygon2[0].extent

        # Get boundary extent
        GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax = float(GCSextent.XMin), float(GCSextent.YMin), \
            float(GCSextent.XMax), float(GCSextent.YMax)
        #print "GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax:", GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax
        GCS_ExtentList = [GCS_XMin, GCS_YMin, GCS_XMax, GCS_YMax]
        del pointGeometry, PtList, point, OutSR, GCSextent, boundaryPolygon, boundaryPolygon2, array

    return Local_ExtentList, GCS_ExtentList



#====================================================================
# Vincenty's Inverse (Vincenty, 1975)
# For a given latitude and longitude calculate the ellipsoidal
#   distance between two points
# May be used for arcs ranging from a few cm to nearly 20,000 km, with
#   millimetre accuracy.
#
# Passed args:
#   f: flattening ratio for specific ellipsoid in meters
#   a: semimajor axis for specific ellipsoid in meters
#   GCS_ExtentList: Bounding coordinates in GCS for dataset in degrees
#
# Return:
#   dist: [distance (m) for 1 second at any given latitude,
#          distance (m) for 1 second of latitude (will be a constant)]
# Online calculators:
#   Cannot use: http://www.ga.gov.au/geodesy/datums/vincenty_inverse.jsp
#   http://geographiclib.sourceforge.net/scripts/geod-calc.html
#====================================================================
def vinc_dist(f, a, GCS_ExtentList):

    ### Find GCS bounding coordinates of DS
    # GCS_ExtentList = [extent.XMin, extent.YMin, extent.XMax, extent.YMax]
    min_lon = GCS_ExtentList[0]
    min_lat = GCS_ExtentList[1]
    max_lon = GCS_ExtentList[2]
    max_lat = GCS_ExtentList[3]
    #print "min_lon, min_lat, max_lon, max_lat:", min_lon, min_lat, max_lon, max_lat


    ### Find mid-latitude position while handling Hemisphere and convert result
    ###   to radians
    mid = 0
    if max_lat >= min_lat:
        mid = ((max_lat - min_lat)/2) + min_lat
    if max_lat < min_lat:
        mid = ((min_lat - max_lat)/2) + max_lat
    mid_lat = mid


    ### Find mid-longitude position while handling Greenwich and convert result
    ###   to radians
    mid = 0
    if max_lon >= min_lon:
        mid = ((max_lon - min_lon)/2) + min_lon
    if max_lon < min_lon:
        mid = ((min_lon - max_lon)/2) + max_lon
    mid_lon = mid
    #print "mid_lon, mid_lat (degrees):", mid_lon, mid_lat


    ### 1 second equals 0.00027777778 decimal degrees, so 1/2 second equals 0.0001388889
    ###  For half second, convert to radians for below calculations
    HalfSec_length = 0.0001388889


    ### Calculate distances for 1 (or 1/2) sec lat (i==1) and 1 (or 1/2) sec long (i==2)
    dist = []
    for i in range(1, 3):

        # Default
        lat1, lon1, lat2, lon2 = 0.0,0.0,0.0,0.0

        if mid_lat > 0: HalfSec_length1 = HalfSec_length * 1.0
        else: HalfSec_length1 = HalfSec_length * -1.0
        #
        if mid_lon > 0: HalfSec_length2 = HalfSec_length * -1.0
        else: HalfSec_length2 = HalfSec_length * 1.0

        if i == 1:
            # Used for calculating lon res
            # Calculate distance for 1 second of longitude when latitude changes
            # Largest difference will occur here
            # 1 second of longitude at a given lat will have x distance
            lat1, lon1, lat2, lon2 = \
                mid_lat, (mid_lon - HalfSec_length2), \
                mid_lat, (mid_lon + HalfSec_length2)
        if i == 2:
            # Used for calculating lat res
            # Calculate distance for 1 second of latitude
            # Any differences here are a result of the flattening ratio and not
            #   changes in longitude
            # 1 second of latitude at a given lat will have x distance
            #'''
            # This was throwing odd results but if I changed longitude it
            #   seemed to work. I divide by 4 so change in lon is extremely
            #   small and this seems to fix problem with getting negative values.
            #   Not sure why the first method resulted in negative distances, but
            #   the uncommented section works and should be ok to use.
            '''
            # This results in neg values in many cases
            lat1, lon1, lat2, lon2 = \
                (mid_lat - HalfSec_length), mid_lon, \
                (mid_lat + HalfSec_length), mid_lon
            '''
            lat1, lon1, lat2, lon2 = \
                (mid_lat - HalfSec_length1), (mid_lon - (HalfSec_length2/4)), \
                (mid_lat + HalfSec_length1), (mid_lon + (HalfSec_length2/4))
        #print "Longitude dist (deg): lat1, lon1, lat2, lon2:", lat1, lon1, lat2, lon2


        ### Longitude at poles will have a distance of 0
        ###   I am not sure if this is the correct way to handle this but we should
        ###   almost never run into this (arctic vegetation maps might be one
        ###   scenario we would encounter).
        if i == 1 and mid_lat > 89 or mid_lat < -89:
            s = 0.0

        else:
            '''
             - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
             Vincenty Inverse Solution of Geodesics on the Ellipsoid (c) Chris Veness 2002-2012

             from: Vincenty inverse formula - T Vincenty, "Direct and Inverse Solutions of Geodesics on the
                   Ellipsoid with application of nested equations", Survey Review, vol XXII no 176, 1975
                   http://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf

            http://www.movable-type.co.uk/scripts/latlong-vincenty.html
            http://www.geomidpoint.com/destination/calculation.html
            http://trac.osgeo.org/proj/wiki/GeodesicCalculations
            http://cpan.uwinnipeg.ca/htdocs/GIS-Distance/GIS/Distance/Formula/Vincenty.html
             - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
             * Calculates geodetic distance between two points specified by latitude/longitude using
             * Vincenty inverse formula for ellipsoids
             *
             * param   {Number} lat1, lon1: first point in decimal degrees
             * param   {Number} lat2, lon2: second point in decimal degrees
             * returns (Number} distance in metres between points
            '''
            # This requires decimal degree inputs
            b = float(a * (1.0 - f))
            L = math.radians(float(lon2-lon1))
            U1 = float(math.atan((float(1.0-f)) * math.tan(math.radians(lat1))))
            U2 = float(math.atan((float(1.0-f)) * math.tan(math.radians(lat2))))
            sinU1 = math.sin(U1)
            cosU1 = math.cos(U1)
            sinU2 = math.sin(U2)
            cosU2 = math.cos(U2)

            lembda = L
            lembdaP = 2 * math.pi
            iterLimit = 10000
            s = 1.000 # Default
            while (abs(lembda-lembdaP) > 1e-12 and iterLimit>0):
                sinlembda = math.sin(lembda)
                coslembda = math.cos(lembda)
                sinSigma = math.sqrt(pow(cosU2 * sinlembda, 2) + pow(cosU1 * sinU2 - sinU1 * cosU2 * coslembda, 2))
                if (sinSigma==0):
                    s = 0.000  # co-incident points
                    break
                cosSigma = float(sinU1 * sinU2 + cosU1 * cosU2 * coslembda)
                sigma = math.atan2(sinSigma, cosSigma)
                sinAlpha = float(cosU1 * cosU2 * sinlembda / sinSigma)
                cosSqAlpha = float(1.0 - sinAlpha * sinAlpha)
                if cosSqAlpha == 0:
                    cos2SigmaM = 0 # Two points on equator
                else:
                    cos2SigmaM = float(cosSigma - 2.0 * sinU1 * sinU2)/cosSqAlpha

                C = float(f/16.0 * cosSqAlpha * (4.0 + f * (4.0 - 3.0 * cosSqAlpha)))
                lembdaP = lembda

                lembda = float(L + (1.0 - C) * f * sinAlpha * (sigma + \
                    C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * pow(cos2SigmaM, 2)))))

            if s != 0:
                uSq = float(cosSqAlpha * (a * a - b * b)) / float((b * b))
                A = float(1.0 + uSq)/float(16384.0 * (4096.0 + uSq * (-768.0 + uSq * (320.0 - 175.0 * uSq))))
                B = uSq/1024.0 * (256.0 + uSq * (-128.0 + uSq * (74.0 - 47.0 * uSq)))
                deltaSigma = B * sinSigma * (cos2SigmaM + B / 4.0 * \
                    (cosSigma * (-1.0 + 2.0 * pow(cos2SigmaM, 2)) - B / 6.0 * cos2SigmaM * \
                    (-3.0 + 4.0 * pow(sinSigma, 2)) * (-3.0 + 4.0 * pow(cos2SigmaM, 2))))

                # initial bearings (degrees)
                fwdAz = math.atan2(cosU2 * sinlembda, cosU1 * sinU2 - sinU1 * cosU2 * coslembda)
                # final bearings  (degrees)
                revAz = math.atan2(cosU1 * sinlembda, -sinU1 * cosU2 + cosU1 * sinU2 * coslembda)

                # distance
                s = b * A * (sigma - deltaSigma)
                s = round(s, 3) # round distance (meters) to 1mm precision

        dist.append(s)

    ### Return Ellipsoidal distance(meters): [long, lat]
    return dist



#====================================================================
# Retrieve information about the spatial reference / coordinate sys.
#====================================================================
def Get_SpatialRef(SR_InDS, myDataType, myFeatType, GCS_ExtentList):

    ### Default units for GCS and PCS
    PCSname = "[Unknown]"
    PrjName = "[Unknown]"
    GCSname = "[Unknown]"
    PCS_Units = "[Unknown]"
    GCS_Units = "[Unknown]"
    Azimuth = "[Unknown]"
    latOf1stPt = "[Unknown]"
    latOf2ndPt = "[Unknown]"
    longOf1stPt = "[Unknown]"
    longOf2ndPt = "[Unknown]"
    DatumName = "[Unknown]"
    SpheroidName = "[Unknown]"
    PrimeMeridName = "[Unknown]"
    PrimeMeridDeg = "[Unknown]"
    SP1 = "[Unknown]"
    SP2 = "[Unknown]"
    LongCM = "[Unknown]"
    ProjCM = "[Unknown]"
    LatPrjOrigin = "[Unknown]"
    SF = "[Unknown]"
    FE = "[Unknown]"
    FN = "[Unknown]"
    UTM_Zone = "[Unknown]"
    SPCS_Zone = "[Unknown]"
    UPS_Zone = "[Unknown]"
    Arc_Zone = "[Unknown]"
    a = "[Unknown]"
    f = "[Unknown]"
    Absc_res = "[Unknown]"
    Ord_res = "[Unknown]"
    Lat_res = "[Unknown]"
    Lon_res = "[Unknown]"
    Height = "[Unknown]"
    VCSname = "[Unknown]"
    VCSdatum = "[Unknown]"
    VCS_Units = "[Unknown]"
    VCS_res = "[Unknown]"


    # http://www.geoapi.org/2.0/javadoc/org/opengis/referencing/doc-files/WKT.html
    # -------------------------------------------------------------------------
    # VERTCS['National_Geodetic_Vertical_Datum_1929',
    #   VDATUM['NGVD_1929'],
    #   PARAMETER['Vertical_Shift',0.0],
    #   PARAMETER['Direction',1.0],
    #   UNIT['Meter',1.0]
    # ]
    # -------------------------------------------------------------------------
    # "PROJCS['Albers_Conical_Equal_Area',
    #   GEOGCS['GCS_WGS_1984',
    #       DATUM['D_WGS_1984', SPHEROID['WGS_1984',6378137.0,298.257223563]]
    #       PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]
    #   ]
    #   PROJECTION['Albers'],
    #   PARAMETER['False_Easting',0.0],
    #   PARAMETER['False_Northing',0.0],
    #   PARAMETER['central_meridian',-96.0],
    #   PARAMETER['Standard_Parallel_1',29.5],
    #   PARAMETER['Standard_Parallel_2',45.5],
    #   PARAMETER['latitude_of_origin',23.0],
    #   UNIT['Meter',1.0]
    # ]
    # -------------------------------------------------------------------------
    # -------------------------------------------------------------------------
    # -------------------------------------------------------------------------
    # Not sure what these precisions are for yet
    # ;-16901100 -6972200 266467840.990852;
    # -100000 10000;
    # -100000 10000;
    # 0.001;
    # 0.001;
    # 0.001;
    # IsHighPrecision"

    ### Break the string into pieces so each item will be something we can work with
    SR_string = SR_InDS.exportToString()
    # tmp_CSV = re.split('\[*\]', CRS)
    try: CRS = str(SR_string.split(";")[0])
    except: CRS = SR_string

    ### Determine if data uses map projection or geographic coord sys
    if str(SR_InDS.PCSname) != "":
        PCSname = str(SR_InDS.PCSname)
    if str(SR_InDS.GCSname) != "":
        GCSname = str(SR_InDS.GCSname)
    if str(SR_InDS.GCSname) != "":
        PrjName = "[Unknown]"
    else:
        PrjName = str(SR_InDS.name)
    if "VERTCS[" in SR_string:
        try:
            VCS = CRS.split(",VERTCS[")[1]
            VCSname = VCS.split(",")[0].strip("'")
        except:
            VCSname = "[Unknown]"
    else:
        VCSname = "[Unknown]"


    ### Map Projection information ---------------------------------------------
    if PCSname != "[Unknown]":
        ### Alter projection name for certain map projections
        if "van_der_grinten" in PrjName.lower():
            PrjName = "van der Grinten"
        if "Vertical_Near_Side_Perspective" == PrjName:
            PrjName = "General Vertical Near-sided Perspective"
        PrjName = PrjName.replace("_", " ")
        if PrjName == "":
            PrjName = "[Unknown]"

        try: PROJCS = CRS.split(",PROJECTION[")[0]
        except: PROJCS = "[Unknown]"
        try: PROJCS2 = PROJCS.split(",GEOGCS[")[0]
        except: PROJCS2 = "[Unknown]"
        try:
            GEOGCS = PROJCS.split(",GEOGCS[")[1]
        except: GEOGCS = "[Unknown]"
        try:
            PROJECTION = CRS.split(",PROJECTION")[1]
            PROJECTION2 = PROJECTION.split("],")
        except: PROJECTION2 = "[Unknown]"

        try:
            Azimuth = str(float(SR_InDS.azimuth))
            if float(Azimuth) < 0.00000001e-25: Azimuth = "[Unknown]"
        except: pass
        #
        try:
            latOf1stPt = str(float(SR_InDS.latitudeOf1st))
            if float(latOf1stPt) < 0.00000001e-25: latOf1stPt = "[Unknown]"
        except: pass
        #
        try:
            latOf2ndPt = str(float(SR_InDS.latitudeOf2nd))
            if float(latOf2ndPt) < 0.00000001e-25: latOf2ndPt = "[Unknown]"
        except: pass
        #
        try:
            longOf1stPt = str(float(SR_InDS.longitudeOf1st))
            if float(longOf1stPt) < 0.00000001e-25: longOf1stPt = "[Unknown]"
        except: pass
        #
        try:
            longOf2ndPt = str(float(SR_InDS.longitudeOf2nd))
            if float(longOf2ndPt) < 0.00000001e-25: longOf2ndPt = "[Unknown]"
        except: pass
        #
        try:
            LongCM = str(float(SR_InDS.centralMeridian))
        except: pass
        #
        try: SF = str(float(SR_InDS.scaleFactor))
        except: pass
        #
        try:
            SP1 = str(float(SR_InDS.standardParallel1))
            SP2 = str(float(SR_InDS.standardParallel2))
        except: pass
        #
        try: FE = str(float(SR_InDS.falseEasting))
        except: pass
        try: FN = str(float(SR_InDS.falseNorthing))
        except: pass
        #
        try: PCS_Units = str(SR_InDS.linearUnitName)
        except:
            if PCS_Units == "": PCS_Units = "[Unknown]"
        #
        try:
            ProjCM = str(float(SR_InDS.longitudeOfOrigin))
            if float(ProjCM) < 0.00000001e-25: ProjCM = "[Unknown]"
        except: pass
        #
        try:
            PROJECTION_tmp = PROJECTION.lower() # Discrepencies how ESRI writes this
            tmp = PROJECTION_tmp.split("PARAMETER['latitude_of_origin".lower())[1]
            tmp2 = tmp.split("],")[0]
            LatPrjOrigin = str(float(tmp2.strip("\',")))
        except: pass

        ### Extract Geographic info from SRS string for map projections
        ###   ESRI objects will not work when map projection exists, and
        ###   therefore this method is required to populate metadata
        try:
            GCSname = GEOGCS.split(",")[0].strip("'")
        except: pass
        #
        try:
            tmp = GEOGCS.split("DATUM[")[1]
            DatumName = tmp.split(",")[0].strip("'")
        except: pass
        #
        try:
            tmp = GEOGCS.split("SPHEROID[")[1]
            SpheroidName = tmp.split(",")[0].strip("'")
        except: pass
        #
        try:
            tmp = GEOGCS.split("PRIMEM[")[1]
            PrimeMeridName = tmp.split(",")[0].strip("'")
        except: pass
        #
        try:
            # ??????????????????????????????????
            LongPM = "[Unknown]"
        except: pass
        #
        try:
            tmp = GEOGCS.split("UNIT[")[1]
            tmp2 = tmp.split(",")[0]
            GCS_Units = tmp2.strip("'")
        except: pass
        #
        try:
            tmp = GEOGCS.split("PRIMEM[")[1]
            tmp2 = tmp.split("],")[0]
            PrimeMeridDeg = str(float(tmp2.split(",")[1]))
        except: pass

        ### Get zone when applicable -----------------------------------------------
        # Zones--Caution: if a custom projection is used, but name was not changed or
        #   if name changed and does not follow the ESRI protocol, this will not work.
        # We could derive from CM (UTM) and lat (UPS), but not sure how to derive for
        #   ARC or SPCS
        # For now, we will use this method since FORT does not generally use these
        #   map projections
        Name = str(SR_InDS.name)
        Name2 = Name.split("_")
        len_Name = len(Name2)
        if "UTM" in Name2:
            try:
                UTM_Zone = Name2[len_Name-1]
                UTM_Zone = UTM_Zone.replace("N", "")
                UTM_Zone = UTM_Zone.replace("S", "")
            except:pass
        if "StatePlane" in Name2:
            try:
                SPCS_Zone = Name2[len_Name-2]
            except: pass
        if "UPS" in Name2:
            try:
                # "A", "B", "Y", "Z" -- How do I get this info
                UPS_Zone = Name2[len_Name-1] # North or south
            except:pass
        if "ARC" in Name2:
            try:
                Arc_Zone = Name2[len_Name-1]
            except:pass


    ### Geographic coordinate system information -------------------------------
    if GCSname != "[Unknown]":
        try:
            if str(SR_InDS.GCSname) != "": GCSname = str(SR_InDS.GCSname)
        except: pass
        #
        try:
            if str(SR_InDS.datumName) != "": DatumName = str(SR_InDS.datumName)
        except: pass
        #
        try:
            if str(SR_InDS.spheroidName) != "": SpheroidName = str(SR_InDS.spheroidName)
        except: pass
        #
        try:
            if str(SR_InDS.primeMeridianName) != "": PrimeMeridName = str(SR_InDS.primeMeridianName)
        except: pass
        #
        try:
            LongPM = str(float(SR_InDS.longitude))
            if float(LongPM) < 0.00000001e-25: LongPM = "[Unknown]"
        except: pass
        #
        try:
            GEOGCS = CRS.split("GEOGCS[")[1]
        except: GEOGCS = "[Unknown]"
        #
        try:
            tmp = GEOGCS.split("UNIT[")[1]
            tmp2 = tmp.split(",")[0]
            GCS_Units = tmp2.strip("\'")
        except: pass
        #
        try:
            tmp = GEOGCS.split("PRIMEM[")[1]
            tmp2 = tmp.split("],")[0]
            PrimeMeridDeg = str(float(tmp2.split(",")[1]))
        except: pass


    ### Vertical coordinate system information ---------------------------------
    if VCSname != "[Unknown]":
        try:
            tmp = str(VCS.split("VDATUM[")[1])
            VCSdatum = tmp.split("],")[0].strip("'")
        except: pass
        #
        try:
            tmp = str(VCS.split("UNIT[")[1])
            VCS_Units = tmp.split(",")[0].strip("'")
        except: pass
        #
        try:
            # ?????????????????????????
            tmp = str(VCS.split("PARAMETER['Direction")[1])
            tmp2 = tmp.split("],")[0].strip("'")
            VCS_res = str(float(tmp2.strip(",")))
        except: pass
        #
        try:
            tmp = str(VCS.split("PARAMETER['Vertical_Shift")[1])
            tmp2 = tmp.split("],")[0].strip("'")
            Height = str(float(tmp2.strip(",")))
        except: pass


    ### Flattening ratio -------------------------------------------------------
    # Make sure these units are always meters, otherwise convert--ESRI will
    #   always use meters I believe so do not need to worry about this.
    # This will work for both map projection and GCS because we extract 'GEOGCS'
    #   differently using code.
    # a = semimajor axis
    # b = semiminor axis
    # Flattening ratio: f = (a-b)/a
    #try: f = float(SR_InDS.flattening)
    #except: pass
    try:
        tmp = GEOGCS.split("SPHEROID[")[1]
        tmp2 = tmp.split("]],")[0]
        tmp3 = tmp2.split(",")
        a = float(tmp3[1])
        b = float(tmp3[2])
        if a != 0.0:
            f = float((a - b) / a)
        else: f = 0.0

        a = str(a)
        f = str(f)
    except:
        f = "0.0"
        a = "0.0"


    ### Coordinate resolution for all coord sys and data types
    # Planar coordinate info
    #   Coord encoding method
    #   Abscissa res
    #   Ordinate res
    # The minimum difference between X (abscissa) and Y (ordinate) values in the
    #   planar data set
    # The values usually indicate the ?fuzzy tolerance? or ?clustering? setting
    #   that establishes the minimum distance at which two points will NOT be
    #   automatically converged by the data collection device (digitizer,
    #   GPS, etc.). NOTE: units of measures are provided under element Planar
    #   Distance Units
    # Raster data: Abscissa/ordinate res equals cell resolution
    # Vector data: Abscissa/ordinate res is the smallest measurable distance between
    #   coordinates
    if myDataType == "Raster":
        # Would need to loop this, but do not know how to handle metadata
        if int(str(arcpy.GetRasterProperties_management(InDS, "BANDCOUNT"))) == 1:
            # works on single band otherwise need to use different syntax
            Absc_res = str(float(desc.meanCellWidth))
            Ord_res = str(float(desc.meanCellHeight))
        else:
            # Works on multi-band as well as single band
            Absc_res = str(arcpy.GetRasterProperties_management(InDS, "CELLSIZEX"))
            Ord_res = str(arcpy.GetRasterProperties_management(InDS, "CELLSIZEX"))
    else:
        if PCSname != "[Unknown]":
            # Industry-standard digitizer precision of 0.002"
            if PCS_Units == "Feet":
                Absc_res = str(float(DataScale) * float(DigPrecision)/12.0)
                Ord_res = str(float(DataScale) * float(DigPrecision)/12.0)
            if PCS_Units == "Meter":
                Absc_res = str(float(DataScale) * (float(DigPrecision)/12.0) * 0.3048)
                Ord_res = str(float(DataScale) * (float(DigPrecision)/12.0) * 0.3048)
        if GCSname != "[Unknown]":
            # Use the distance of 1/2 second at given latitude and ellipsoid and then calculate
            # Units in decimal seconds
            dist = vinc_dist(float(f), float(a), GCS_ExtentList)
            #print str(dist)
            Lat_res = float(float(1.0/float(dist[0])) * float(float(DataScale)/1.0) * float(1.0/39.37007874) * float(DigPrecision))
            Lat_res = str(format(Lat_res, '.10f'))
            #
            Lon_res = float(float(1.0/float(dist[1])) * float(float(DataScale)/1.0) * float(1.0/39.37007874) * float(DigPrecision))
            Lon_res = str(format(Lon_res, '.10f'))

            #print "Lon_res, Lat_res:", Lon_res, Lat_res


    ### Return dictionary object for populating metadata
    SR_List = \
        {"PCSname": PCSname,
        "PrjName": PrjName,
        "GCSname": GCSname,
        "GCS_Units": GCS_Units,
        "PCS_Units": PCS_Units,
        "Azimuth": Azimuth,
        "latOf1stPt": latOf1stPt,
        "latOf2ndPt": latOf2ndPt,
        "longOf1stPt": longOf1stPt,
        "longOf2ndPt": longOf2ndPt,
        "DatumName": DatumName,
        "SpheroidName": SpheroidName,
        "SP1": SP1,
        "SP2": SP2,
        "LongCM": LongCM,
        "ProjCM": ProjCM,
        "LatPrjOrigin": LatPrjOrigin,
        "SF": SF,
        "FE": FE,
        "FN": FN,
        "UTM_Zone": UTM_Zone,
        "SPCS_Zone": SPCS_Zone,
        "UPS_Zone": UPS_Zone,
        "Arc_Zone": Arc_Zone,
        "a": a,
        "f": f,
        "Absc_res": Absc_res,
        "Ord_res": Ord_res,
        "Lat_res": Lat_res,
        "Lon_res": Lon_res,
        "Height": Height,
        "VCSname": VCSname,
        "VCSdatum": VCSdatum,
        "VCS_Units": VCS_Units,
        "VCS_res": VCS_res}

    #print str(SR_List)
    return SR_List




#====================================================================
# Spatial Reference for updating XML metadata templates
#====================================================================
### Spatial Reference Open
def Open_SpatialRef():
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_1 + "<spref>")
    FileOutW.close()
    del FileOutW

def Close_SpatialRef():
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_1 + "</spref>")
    FileOutW.close()
    del FileOutW

### Horizontal coordinate system
def Open_Horizontal():
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_2 + "<horizsys>")
    FileOutW.close()
    del FileOutW
def Close_Horizontal():
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_2 + "</horizsys>")
    FileOutW.close()
    del FileOutW


#====================================================================
# Geographic Coordinate Systems for updating XML metadata templates
#====================================================================
def Geographic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<geograph>")
    FileOutW.write(ElemTab_4 + "<latres>" + str(SR_List["Lat_res"]) + "</latres>")
    FileOutW.write(ElemTab_4 + "<longres>" + str(SR_List["Lon_res"]) + "</longres>")
    #FileOutW.write(ElemTab_4 + "<geogunit>" + SR_List["GCS_Units"] + "</geogunit>")
    # Calculation for res will always return a value in Decimal Seconds
    FileOutW.write(ElemTab_4 + "<geogunit>Decimal seconds</geogunit>")
    FileOutW.write(ElemTab_3 + "</geograph>")
    FileOutW.close()
    del FileOutW




#====================================================================
# Planar Map Projections for updating XML metadata templates
#====================================================================
def Albers_Conical_Equal_Area(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    #FileOutW.write(ElemTab_5 + "<mapprojn>Albers Conical Equal Area (" + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<albers>")
    # Std Paral 1
    # Std Paral 2 (if exists)
    # Long of central Meridian
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    if SR_List["SP2"] != "[Unknown]":
        FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP2"] + "</stdparll>")
    else: pass
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</albers>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Azimuthal_Equidistant(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<azimequi>")
    # Long of central Meridian
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</azimequi>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Equidistant_Conic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<equicon>")
    # Std Paral 1
    # Std Paral 2 (if exists)
    # Long of central Meridian
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    if SR_List["SP2"] != "[Unknown]":
        FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP2"] + "</stdparll>")
    else: pass
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</equicon>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Equirectangular(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<equirect>")
    # Std Paral 1
    # Long of central Meridian
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</equirect>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def General_Vertical_Near_sided_Perspective(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<gvnsp>")
    # Height of perspective point above surface
    # Longitude of Projection Center
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<heightpt>" + SR_List["Height"] + "</heightpt>")
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</gvnsp>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Gnomonic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<gnomonic>")
    # Long of Projection Center
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</gnomonic>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Lambert_Azimuthal_Equal_Area(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<lamberta>")
    # Long of Projection Center
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</lamberta>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Lambert_Conformal_Conic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<lambertc>")
    # Std paral 1
    # Std paral 2 (if exists)
    # Long of Central Meridian
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    if SR_List["SP2"] != "[Unknown]":
        FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP2"] + "</stdparll>")
    else: pass
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</lambertc>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Mercator(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<mercator>")
    # Std paral 1 (or scale factor at equator)
    # Long of Central Meridian
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</mercator>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Modified_Stereographic_for_Alaska(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<modsak>")
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</modsak>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Miller_Cylindrical(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<miller>")
    FileOutW.write(ElemTab_5 + "<mapprojp>")
    # Long of Central Meridian
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</miller>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Oblique_Mercator(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<obqmerc>")
    # Scale Factor at Center Line
    FileOutW.write(ElemTab_6 + "<sfctrlin>" + SR_List["SF"] + "</sfctrlin>")

    # --
    # Oblique Line Azimuth
    #   Azimuthal Angle
    #   Azimuth Measure Point Longitude
    FileOutW.write(ElemTab_6 + "<obqlazim>")
    FileOutW.write(ElemTab_7 + "<azimangl>[Unknown]</azimangl>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_7 + "<azimptl>[Unknown]</azimptl>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_6 + "</obqlazim>")
    # OR
    # Oblique Line Point
    #   (two occurrences of both)??
    #   Oblique Line Latitude
    #   Oblique Line Longitude
    FileOutW.write(ElemTab_6 + "<obqlpt>")
    FileOutW.write(ElemTab_7 + "<obqllat>[Unknown]</obqllat>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_7 + "<obqllat>[Unknown]</obqllat>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_7 + "<obqllong>[Unknown]</obqllong>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_7 + "<obqllong>[Unknown]</obqllong>") #????????????????????????????????????????????????
    FileOutW.write(ElemTab_6 + "</obqlpt>")
    # ---

    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")

    FileOutW.write(ElemTab_5 + "</obqmerc>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Orthographic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<orthogr>")
    # Longitude of Projection Center
    # Latitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</orthogr>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Polar_Stereographic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<polarst>")
    # Straight-Vertical Longitude from Pole
    FileOutW.write(ElemTab_6 + "<svlong>[Unknown]</svlong>") #????????????????????????????????????????????????
    # ---
    # Standard Parallel
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")

    # OR
    # Scale Factor at Projection Origin
    FileOutW.write(ElemTab_6 + "<sfprjorg>" + SR_List["SF"] + "</sfprjorg>")
    # ---
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")

    FileOutW.write(ElemTab_5 + "</polarst>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Polyconic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<polycon>")
    # Longitude of Central Meridian
    # Latitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</polycon>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Robinson(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<robinson>")
    # Longitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</robinson>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Sinusoidal(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<sinusoid>")
    # Longitude of Central Meridian
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</sinusoid>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Space_Oblique_Mercator_Landsat(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<spaceobq>")
    # Landsat Number
    # Path Number
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<landsat>[Landsat Platform Number]</landsat>")
    FileOutW.write(ElemTab_6 + "<pathnum>[Landsat Path Number]</pathnum>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</spaceobq>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Stereographic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<stereo>")
    # Longitude of Projection Center
    # Latitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjc>" + SR_List["LatPrjOrigin"] + "</latprjc>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</stereo>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Transverse_Mercator(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<transmer>")
    # Scale Factor at Central Meridian
    # Longitude of Projection Center
    # Latitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<sfctrmer>" + SR_List["SF"] + "</sfctrmer>")
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<latprjc>" + SR_List["LatPrjOrigin"] + "</latprjc>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</transmer>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def van_der_Grinten(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<vdgrin>")
    # Longitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_5 + "</vdgrin>")
    FileOutW.write(ElemTab_4 + "</mapproj>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Other_MapProjections(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<mapproj>")
    FileOutW.write(ElemTab_5 + "<mapprojn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</mapprojn>")
    FileOutW.write(ElemTab_5 + "<mapprojp>[Need to define name, parameter and reference]</mapprojp>")
    # Enumerate parameters and include if defined (in correct order) ??????????????????????????????????????????????????????????? Not complete
    FileOutW.write(ElemTab_4 + "</mapproj>")
    FileOutW.close()
    del FileOutW



#====================================================================
# Grid Map Projections for updating XML metadata templates
#====================================================================
def Universal_Transverse_Mercator(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<gridsys>")
    FileOutW.write(ElemTab_5 + "<gridsysn>Universal Transverse Mercator" + " (ESRI Full Name: " + SR_List["PCSname"] + ")</gridsysn>")
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_5 + "<utm>")
    FileOutW.write(ElemTab_6 + "<utmzone>" + SR_List["UTM_Zone"] + "</utmzone>")
    # Transverse Mercator
    # Scale Factor at Central Meridian
    # Longitude of Projection Center
    # Latitude of Projection Center
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_7 + "<transmer>")
    FileOutW.write(ElemTab_8 + "<sfctmer>" + SR_List["SF"] + "</sfctmer>")
    FileOutW.write(ElemTab_8 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
    FileOutW.write(ElemTab_8 + "<latprjc>" + SR_List["LatPrjOrigin"] + "</latprjc>")
    FileOutW.write(ElemTab_8 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_8 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_7 + "</transmer>")
    FileOutW.write(ElemTab_5 + "</utm>")
    FileOutW.write(ElemTab_4 + "</gridsys>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW



def Universal_Polar_Stereographic(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<gridsys>")
    FileOutW.write(ElemTab_5 + "<gridsysn>Universal Polar Stereographic" + " (ESRI Full Name: " + SR_List["PCSname"] + ")</gridsysn>")
    #FileOutW.write(ElemTab_5 + "<gridsysn>" + SR_List["PrjName"] + " (ESRI Full Name: " + SR_List["PCSname"] + ")</gridsysn>")
    FileOutW.write(ElemTab_5 + "<ups>")
    FileOutW.write(ElemTab_6 + "<upszone>" + SR_List["UPS_Zone"] + "</upszone>")

    # Straight-Vertical Longitude from Pole
    FileOutW.write(ElemTab_6 + "<svlong>[Unknown]</svlong>") #????????????????????????????????????????

    # ---
    # Standard Parallel
    FileOutW.write(ElemTab_6 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
    # OR --
    # Scale Factor at Projection Origin
    #FileOutW.write(ElemTab_6 + "<sfprjorg>" + x + "</sfprjorg>") #????????????????????????????????????????
    # ---

    # False Easting
    # False Northing
    FileOutW.write(ElemTab_6 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_6 + "<fnorth>" + SR_List["FN"] + "</fnorth>")

    FileOutW.write(ElemTab_5 + "</ups>")
    FileOutW.write(ElemTab_4 + "</gridsys>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def State_Plane_Coordinate_System(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<gridsys>")
    FileOutW.write(ElemTab_5 + "<gridsysn>State Plane Coordinate System" + " (ESRI Full Name: " + SR_List["PCSname"] + ")</gridsysn>")
    FileOutW.write(ElemTab_5 + "<spcs>")
    FileOutW.write(ElemTab_6 + "<spcszone>" + SR_List["SPCS_Zone"] + "</spcszone>")

    if SR_List["PrjName"] == "Lambert Conformal Conic":
        # Lambert Conformal Conic
        # Std paral 1
        # Std paral 2 (if exists)
        # Long of Central Meridian
        # Latitude of Projection Origin
        # False Easting
        # False Northing
        FileOutW.write(ElemTab_7 + "<lambertc>")
        FileOutW.write(ElemTab_8 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
        if SR_List["SP2"] != "[Unknown]":
            FileOutW.write(ElemTab_8 + "<stdparll>" + SR_List["SP2"] + "</stdparll>")
        else: pass
        FileOutW.write(ElemTab_8 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
        FileOutW.write(ElemTab_8 + "<latprjo></latprjo>")
        FileOutW.write(ElemTab_8 + "<feast>" + SR_List["FE"] + "</feast>")
        FileOutW.write(ElemTab_8 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
        FileOutW.write(ElemTab_7 + "</lambertc>")


    # OR ----------------------------
    if SR_List["PrjName"] == "Transverse Mercator":
        # Transverse Mercator
        # Scale Factor at Central Meridian
        # Longitude of Projection Center
        # Latitude of Projection Center
        # False Easting
        # False Northing
        FileOutW.write(ElemTab_7 + "<transmer>")
        FileOutW.write(ElemTab_8 + "<sfctmer>" + SR_List["SF"] + "</sfctmer>")
        FileOutW.write(ElemTab_8 + "<longpc>" + SR_List["LongCM"] + "</longpc>")
        FileOutW.write(ElemTab_8 + "<latprjc>" + SR_List["LatPrjOrigin"] + "</latprjc>")
        FileOutW.write(ElemTab_8 + "<feast>" + SR_List["FE"] + "</feast>")
        FileOutW.write(ElemTab_8 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
        FileOutW.write(ElemTab_7 + "</transmer>")


    # OR ----------------------------
    if SR_List["PrjName"] == "Oblique Mercator":
        # Oblique Mercator
        # Scale Factor at Center Line
        FileOutW.write(ElemTab_7 + "<obqmerc>")
        FileOutW.write(ElemTab_8 + "<sfctrlin>" + SR_List["SF"] + "</sfctrlin>")
    # --
    if SR_List["PrjName"] == "Oblique Line Azimuth": #???????????????????????????????????????
        # Oblique Line Azimuth
        #   Azimuthal Angle
        #   Azimuth Measure Point Longitude
        FileOutW.write(ElemTab_8 + "<obqlazim>")
        FileOutW.write(ElemTab_9 + "<azimangl>[Unknown]</azimangl>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_9 + "<azimptl>[Unknown]</azimptl>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_8 + "</obqlazim>")
    # OR
    if SR_List["PrjName"] == "Oblique Line Point": #???????????????????????????????????????
        # Oblique Line Point
        #   (two occurrences of both)??
        #   Oblique Line Latitude
        #   Oblique Line Longitude
        FileOutW.write(ElemTab_8 + "<obqlpt>")
        FileOutW.write(ElemTab_9 + "<obqllat>[Unknown]</obqllat>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_9 + "<obqllat>[Unknown]</obqllat>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_9 + "<obqllong>[Unknown]</obqllong>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_9 + "<obqllong>[Unknown]</obqllong>") #????????????????????????????????????????????????
        FileOutW.write(ElemTab_8 + "</obqlpt>")
    # Longitude of Projection Origin
    # False Easting
    # False Northing
    FileOutW.write(ElemTab_8 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
    FileOutW.write(ElemTab_8 + "<feast>" + SR_List["FE"] + "</feast>")
    FileOutW.write(ElemTab_8 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
    FileOutW.write(ElemTab_7 + "</obqmerc>")
    # ---


    # OR ----------------------------
    if SR_List["PrjName"] == "Polyconic":
        # Polyconic
        # Longitude of Central Meridian
        # Latitude of Projection Origin
        # False Easting
        # False Northing
        FileOutW.write(ElemTab_7 + "<polycon>")
        FileOutW.write(ElemTab_8 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
        FileOutW.write(ElemTab_8 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
        FileOutW.write(ElemTab_8 + "<feast>" + SR_List["FE"] + "</feast>")
        FileOutW.write(ElemTab_8 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
        FileOutW.write(ElemTab_7 + "</polycon>")

    FileOutW.write(ElemTab_5 + "</spcs>")
    FileOutW.write(ElemTab_4 + "</gridsys>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def ARC_Coordinate_System(SR_List):
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<gridsys>")
    FileOutW.write(ElemTab_5 + "<gridsysn>Arc Coordinate System" + " (ESRI Full Name: " + SR_List["PCSname"] + ")</gridsysn>")
    FileOutW.write(ElemTab_5 + "<arcsys>")
    FileOutW.write(ElemTab_6 + "<arczone>" + SR_List["Arc_Zone"] + "</arczone>")

    if SR_List["PrjName"] == "Equirectangular":
        # Equirectangular
        # Std Paral 1
        # Long of central Meridian
        # False Easting
        # False Northing
        FileOutW.write(ElemTab_6 + "<equirect>")
        FileOutW.write(ElemTab_7 + "<stdparll>" + SR_List["SP1"] + "</stdparll>")
        FileOutW.write(ElemTab_7 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
        FileOutW.write(ElemTab_7 + "<feast>" + SR_List["FE"] + "</feast>")
        FileOutW.write(ElemTab_7 + "<fnorth>" + SR_List["FN"] + "</fnorth>")
        FileOutW.write(ElemTab_6 + "</equirect>")


    # OR ---
    if SR_List["PrjName"] == "Azimuthal Equidistant":
        # Azimuthal Equidistant
        # Long of central Meridian
        # Latitude of Projection Origin
        # False Easting
        # False Northing
        FileOutW.write(ElemTab_6 + "<azimequi>")
        FileOutW.write(ElemTab_7 + "<longcm>" + SR_List["LongCM"] + "</longcm>")
        FileOutW.write(ElemTab_7 + "<latprjo>" + SR_List["LatPrjOrigin"] + "</latprjo>")
        FileOutW.write(ElemTab_7 + "<feast>" + SR_List["FE"] + "</feast>")
        FileOutW.write(ElemTab_7 + "<fnorth>" + SR_List["FN"] + "</fnorth>")

    FileOutW.write(ElemTab_6 + "</azimequi>")
    FileOutW.write(ElemTab_5 + "</arcsys>")
    FileOutW.write(ElemTab_4 + "</gridsys>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


def Other_Grid_System(SR_List):
    # We do not have a way to differentiate between grids and map projections so we have no way to
    #   capture this here--therefore, this will not be handled
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<planar>")
    FileOutW.write(ElemTab_4 + "<gridsys>")
    FileOutW.write(ElemTab_5 + "<othergrd>")
    FileOutW.write(ElemTab_5 + "[Include name, parameters, and values and citation]")
    FileOutW.write(ElemTab_5 + "</othergrd>")
    FileOutW.write(ElemTab_4 + "</gridsys>")
    #FileOutW.write(ElemTab_3 + "</planar>")
    FileOutW.close()
    del FileOutW


#====================================================================
# Map Projection Planar Coordinate Info for updating XML metadata templates
#====================================================================
def Planar_CoordInfo(myDataType, SR_List):

    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_4 + "<planci>")
    if myDataType == "Raster":
        FileOutW.write(ElemTab_5 + "<plance>row and column</plance>")
    else:
        FileOutW.write(ElemTab_5 + "<plance>coordinate pair</plance>")
    # Not supporting distance and bearing
    FileOutW.write(ElemTab_5 + "<coordrep>")
    FileOutW.write(ElemTab_6 + "<absres>" + SR_List["Absc_res"] + "</absres>")
    FileOutW.write(ElemTab_6 + "<ordres>" + SR_List["Ord_res"] + "</ordres>")
    FileOutW.write(ElemTab_5 + "</coordrep>")
    FileOutW.write(ElemTab_5 + "<plandu>" + SR_List["PCS_Units"] + "</plandu>")
    FileOutW.write(ElemTab_4 + "</planci>")
    FileOutW.write(ElemTab_3 + "</planar>")

    FileOutW.close()
    del FileOutW



#====================================================================
# Map Projection Geodetic Model for updating XML metadata templates
#====================================================================
def Geodetic(SR_List):

    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_3 + "<geodetic>")
    if SR_List["DatumName"] != "[Unknown]":
        FileOutW.write(ElemTab_4 + "<horizdn>" + SR_List["DatumName"] + "</horizdn>")
    else: pass
    FileOutW.write(ElemTab_4 + "<ellips>" + SR_List["SpheroidName"] + "</ellips>")
    FileOutW.write(ElemTab_4 + "<semiaxis>" + SR_List["a"] + "</semiaxis>")
    FileOutW.write(ElemTab_4 + "<denflat>" + SR_List["f"] + "</denflat>")
    FileOutW.write(ElemTab_3 + "</geodetic>")
    FileOutW.close()
    del FileOutW


#====================================================================
# Local Map Projections for updating XML metadata templates
#====================================================================
### Local Planar: we will rarely run into this and therefore this program
###   will not handle it. These are for coordinate systems where the z-axix
###   coincides with a plumb line though the origin that locally is aligned
###   with the Earth's surface
### We are not tracking any information of metadata elements for this


#====================================================================
# Vertical Map Projections for updating XML metadata templates
#====================================================================
def Vertical_CS(SR_List):
    # Currently, ArcGIS 10 does not do any vertical coordinate system conversions.
    #   You can define a vertical coordinate system on a dataset, but the software
    #   isn't using the information except when defining the spatial reference
    #   (min/max z, z resolution/tolerance values).
    # A vertical coordinate system (vcs) can be referenced to two different types of surfaces:
    #   spheroidal (ellipsoidal) OR gravity-related (geoidal).
    # Most vertical coordinate systems are gravity-related.
    #
    # VCSdatum = [National Geodetic Vertical Datum of 1929", "North American Vertical Datum of 1988", free text]
    # "Explicit elevation coordinate included with horizontal coordinates", "Implicit coordinate", "Attribute values"


    FileOutW = open(OutXML_Tmp, 'a')

    FileOutW.write(ElemTab_2 + "<vertdef>")
    FileOutW.write(ElemTab_3 + "<altsys>")

    FileOutW.write(ElemTab_4 + "<altdatum>" + SR_List["VCSdatum"] + "</altdatum>")
    FileOutW.write(ElemTab_4 + "<altres>" + SR_List["VCS_res"] + "</altres>")
    FileOutW.write(ElemTab_4 + "<altunits>" + SR_List["VCS_Units"] + "</altunits>")
    FileOutW.write(ElemTab_4 + "<altenc>Explicit elevation coordinate included with horizontal coordinates</altenc>")

    FileOutW.write(ElemTab_3 + "</altsys>")
    FileOutW.write(ElemTab_2 + "</vertdef>")

    FileOutW.close()
    del FileOutW


#====================================================================
# Get unique values for each field so the program can make the best 'guess'
#   on the type of domain code (based on threshold) to evaluate the data
#====================================================================
def GetUniqueValues(InDS2, myDataType, Field, FieldType):

    ### Default
    UniqueCriteria = ""

    ### Retrieve the a list of unique values for specified field within table
    # SearchCursor (dataset, {where_clause}, {spatial_reference}, {fields}, {sort_fields})
    rows = arcpy.SearchCursor(InDS2)
    Uniquelist = []
    uniqueCount = 0
    for irow in rows:
        try:
            myValue = irow.getValue(Field)
            if myValue == "": myValue = "Null"
            if myValue not in Uniquelist:
                uniqueCount += 1
                Uniquelist.append(myValue)
            if uniqueCount <= UniqCountThreshold:
                UniqueCriteria = "CODESET"
                break
        except: print arcpy.GetMessages()
    del rows, irow

    if UniqueCriteria == "":
        if FieldType in ("Text", "String"):
            UniqueCriteria = "UNREP"
        else:
            UniqueCriteria = "RANGE"
    return UniqueCriteria


#====================================================================
# Determine if fields and domains passed by user match the data
# If this information was not passed, a list of fields and domain values
#   are passed back to the user to help the user formulate the appropriate
#   list.
#====================================================================
def CheckFields(InDS2, myDataType, FieldListMeta):

    ### Verify the list object has the correct format
    for iFieldListMeta in FieldListMeta:
        try:
            field_name = iFieldListMeta[0]
            field_type = iFieldListMeta[1]
            meta_type = iFieldListMeta[2]
            Validation = False
            for i2_FieldListMeta in FieldListMeta:
                if i2_FieldListMeta[0] == field_name: Validation = True
            if Validation != True:
                print "Error in format of string passed to program for field and metadata definitions."
                print "Fields do not match."
                print "Format: [(\"FieldName\", \"FieldType\", \"MetaDomain\"), (..., ..., ...), ...]"
                sys.exit(1)
        except:
            print "Error in format of string passed to program for field and metadata definitions."
            print "Format: [(\"FieldName\", \"FieldType\", \"MetaDomain\"), (..., ..., ...), ...]"
            sys.exit(1)


    ### Check domain list for vector and table data ----------------------------
    if myDataType == "Vector" or myDataType == "Table":

        ### Enumerate fields and determine unique values
        # object.ListFields(InputValue As String, {wildCard} As String, {FieldType} As
        #   String) As Python List
        try: fieldList = arcpy.ListFields(InDS2)
        except:
            print "Error trying to get list of fields for data."
            print arcpy.GetMessages()
            sys.exit(1)

        ### Use this list for comparing string passed by user with dataset field definitions
        fieldList2 = []
        for iField in fieldList:
            fieldList2.append((str(iField.name), str(iField.type)))


        ### If user did not provide a string of field names, field types, metadata class
        ###   then attempt to create one for the user to start with
        if len(FieldListMeta) == 0:
            ### Return list of fields
            ### If string, default to "ENUM"
            ### if float, default to "RANGE"
            ### if int and more than 25 unique itmes, devault to "CODESET"
            ### If values are symbols/characters which are impossible to represent, use "UNREP"
            for iField in fieldList:
                FieldName = str(iField.name).lower()
                FieldType = str(iField.type).lower()

                ### Field name criteria; L.count(value)
                if ["objectid", "oid", "fid", "shape_length", "shape_area"].count(FieldName) == 1:
                    UniqueCriteria = "RANGE"
                if ["fnode_", "tnode_", "lpoly_", "rpoly_", "length"].count(FieldName) == 1:
                    if str(desc.dataType) != "Coverage":
                        print "\t\tUser should delete unnecessary field:", iField.name
                    UniqueCriteria = "RANGE"
                if ["shape"].count(FieldName) == 1:
                    UniqueCriteria = "ENUM"


                ### Field types
                if ["objectid", "oid", "fid", "shape_length", "shape_area", "length", \
                    "shape", "fnode_", "tnode_", "lpoly_", "rpoly_"].count(FieldName) == 0:

                    # Either range or unique
                    if ["short integer", "long integer", "integer", "small integer", "smallinteger"].count(FieldType) == 1:
                        UniqueCriteria = GetUniqueValues(InDS2, myDataType, str(iField.name), str(iField.type))
                    # Range
                    elif ["float", "double", "date"].count(FieldType) == 1:
                        UniqueCriteria = "RANGE"
                    # Unique
                    elif ["text", "string"].count(FieldType) == 1:
                        #UniqueCriteria = "ENUM"
                        UniqueCriteria = GetUniqueValues(InDS2, myDataType, str(iField.name), str(iField.type))
                    # Not sure about
                    elif ["blob", "guid", "raster"].count(FieldType) == 1:
                        UniqueCriteria = "UNREP"
                    else:
                        UniqueCriteria = "Unknown"

                if not UniqueCriteria:
                    UniqueCriteria = "Unknown"

                ### Add to list
                FieldListMeta.append((str(iField.name), str(iField.type), UniqueCriteria))


            print "\n\nField definitions not provided--attempting to define for user." + \
                  "\n\tRetun a string using this syntax:" + \
                  "\n\t\"[('fieldname', 'FieldType', 'ENUM'),('fieldname',  'FieldType', 'CODESET'), ...]"
            print "\n\t...and use these domain values:"
            print "\t\tENUM: Unique text values (characters or numeric) listed."
            print "\t\tRANGE: Minimum and Maximum record values listed in metadata."
            print "\t\tUNREP: Record values represent symbols/characters or BLOBs that" + \
                  " otherwise have no meaning or difficult to represent."
            print "\t\tCODESET: Integers represent classification scheme."
            print "\n\tRefer to notepad, which opened on your machine, for an example with your dataset."
            #print "\n", str(FieldListMeta)
            FileOutW = open(Tmp_FieldListMeta, 'w')
            FileOutW.write("\"" + str(FieldListMeta) + "\"")
            FileOutW.close()
            del FileOutW
            ret = subprocess.Popen(["notepad.exe", Tmp_FieldListMeta])
            sys.exit(1)



        ### If user did provide a string of field info then verify it matches the data passed to the program
        ###   Cannot verfiy metadata class, but can verify field names and field types
        else:
            for iFieldListMeta in FieldListMeta:
                field_name = False
                field_type = False
                meta_type = False
                for ifieldList2 in fieldList2:
                    if field_name == True or field_type == True or meta_type == True:
                        break
                    # Check field name
                    if iFieldListMeta[0] == ifieldList2[0]:
                        field_name = True

                        # Check field type
                        if iFieldListMeta[1] == ifieldList2[1]:
                            field_type = True

                            # Check metadata domain value
                            if iFieldListMeta[2] in ["RANGE", "ENUM", "CODESET", "UNREP"]:
                                meta_type = True


                ### Exit if field name or field type not found in user list of data field list
                if field_name == False:
                    print str(FieldListMeta)
                    print "\nError, Field does not exist in dataset:", iFieldListMeta[0]
                    sys.exit(1)
                elif field_type == False:
                    print str(FieldListMeta)
                    print "\nError, Field Type does not match dataset (Field, Field type):", \
                        iFieldListMeta[0], iFieldListMeta[1]
                    sys.exit(1)
                elif meta_type == False:
                    print str(FieldListMeta)
                    print "\nError, Metadata Domain Type does not match available selection (\"RANGE\", \"ENUM\", \"CODESET\", \"UNREP\"):", \
                        iFieldListMeta[0], iFieldListMeta[1], iFieldListMeta[2]
                    sys.exit(1)

                else:
                    return FieldListMeta

    ### Check domain list for integer data -------------------------------------
    if myDataType == "Raster":
        if FieldListMeta == []:

            ### Determine unique value count and alert user if necessary
            try:
                # this works when there is a raster attribute table only
                myUniqueNum = int(str(arcpy.GetRasterProperties_management(InDS, "UNIQUEVALUECOUNT")))
            except:
                print arcpy.GetMessages()
                myUniqueNum = 1000000

            if myUniqueNum > UniqCountThreshold:
                # FieldListMeta = "[('FID', 'OID', 'RANGE')
                FieldListMeta = [("VAT", "VAT", "RANGE")]
            if myUniqueNum < UniqCountThreshold:
                FieldListMeta = [("VAT", "VAT", "ENUM")]
            return FieldListMeta
        else:
            ### Verify values passed to program are correct
            for i in FieldListMeta:
                if FieldListMeta[0][0] == "VAT" and FieldListMeta[0][1] == "VAT":
                    if FieldListMeta[0][2] in ["RANGE", "ENUM", "CODESET", "UNREP"]:
                        pass
                    else:
                        print "\n\nField definitions not provided--attempting to define for user." + \
                              "\n\tRetun a string using this syntax:" + \
                              "\n\t\t[('fieldname', 'FieldType', 'ENUM'),('fieldname',  'FieldType', 'CODESET'), ...]"
                        print "\n\t...and use these domain values:"
                        print "\t\tENUM: Unique text values (characters or numeric) listed."
                        print "\t\tRANGE: Minimum and Maximum record values listed in metadata."
                        print "\t\tUNREP: Record values represent symbols/characters or BLOBs that" + \
                              " otherwise have no meaning or difficult to represent."
                        print "\t\tCODESET: Integers represent classification scheme."
                        print "\n\tRefer to notepad, which opened on your machine, for an example with your dataset."
                        #print "\n", str(FieldListMeta)
                        FileOutW = open(Tmp_FieldListMeta, 'w')
                        FileOutW.write("\"" + str(FieldListMeta) + "\"")
                        FileOutW.close()
                        del FileOutW
                        ret = subprocess.Popen(["notepad.exe", Tmp_FieldListMeta])
                        sys.exit(1)



#====================================================================
# Retrieve vector attribute information
#====================================================================
def Retrieve_VectorAttributes(InDS2, myDataType, FieldListMeta):

    ### For each field, create a list of items for metadata records.
    ###   These lists will vary depending on the type of metadata notation
    ###   so include the type of metadata to be used in the list:
    ### [(field, "Meta_Type"), (...)]

    ### List object that will contain all relevant info for detailed attribute info
    AttribList_DET = []

    try:
        # Describe data set
        dsc = arcpy.Describe(InDS2)
        GetCount = int(str(arcpy.GetCount_management(InDS2)))
    except:
        print "Error retrieving feature count."
        print arcpy.GetMessages()
        sys.exit(1)

    ### Enumerate fields and determine unique values
    # object.ListFields(InputValue As String, {wildCard} As String, {FieldType} As
    #   String) As Python List
    fieldList = arcpy.ListFields(InDS2)

    for iFieldListMeta in FieldListMeta:
        ### Pull out info so we can extract data based on field type and metadata domain constraint
        FieldName = iFieldListMeta[0]
        FieldType = iFieldListMeta[1]
        MetaDomain = iFieldListMeta[2]

        if ["objectid", "oid", "fid", "shape_length", "shape_area", "shape"].count(FieldName.lower()) == 1:
            if FieldName.lower() == "shape":
                # Use Enum
                AttribList_DET.append(("DOMAIN", "ENUM"))
                AttribList_DET.append(("FieldName", FieldName))
                AttribList_DET.append(("VALUE", str(dsc.shapeType)))
            elif FieldName.lower() in ("objectid", "oid", "fid"):
                # Cannot retrieve values for these and do not need to
                AttribList_DET.append(("DOMAIN", "RANGE"))
                AttribList_DET.append(("FieldName", FieldName))
                AttribList_DET.append(("Min", "0"))
                AttribList_DET.append(("Max", str(GetCount-1)))
            else:
                # Use Range Domain
                AttribList_DET.append(("DOMAIN", "RANGE"))
                AttribList_DET.append(("FieldName", FieldName))
                min, max = GetRange(InDS2, myDataType, FieldType, FieldName)
                AttribList_DET.append(("Min", min))
                AttribList_DET.append(("Max", max))
        #elif FieldName in ("FNODE_", "TNODE_", "LPOLY_", "RPOLY_", "LENGTH"):
        else:
            if MetaDomain == "ENUM":
                Results = Get_Unique(InDS2, FieldName)
                AttribList_DET.append(("DOMAIN", "ENUM"))
                AttribList_DET.append(("FieldName", FieldName))
                for iResults in Results:
                    AttribList_DET.append(iResults)
            if MetaDomain == "CODESET":
                Results = Get_Unique(InDS2, FieldName)
                AttribList_DET.append(("DOMAIN", "CODESET"))
                AttribList_DET.append(("FieldName", FieldName))
                for iResults in Results:
                    AttribList_DET.append(iResults)
            if MetaDomain == "UNREP":
                AttribList_DET.append(("DOMAIN", "UNREP"))
                AttribList_DET.append(("FieldName", FieldName))
            if MetaDomain == "RANGE":
                AttribList_DET.append(("DOMAIN", "RANGE"))
                AttribList_DET.append(("FieldName", FieldName))
                min, max = GetRange(InDS2, myDataType, FieldType, FieldName)
                AttribList_DET.append(("Min", min))
                AttribList_DET.append(("Max", max))

    return AttribList_DET


### Retrieve the minimum and maximum value for specified field within table
def GetRange(InDS2, myDataType, FieldType, FieldName):


    try:
        # SearchCursor (dataset, {where_clause}, {spatial_reference}, {fields},
        #   {sort_fields})
        rows = arcpy.SearchCursor(InDS2, "", "", FieldName, FieldName + " A")
    except:
        print "Unable to create a search cursor on dataset table."
        print arcpy.GetMessages()
        print myDataType, FieldName
        sys.exit(1)

    count = 1
    min, max = 0, 0
    for irow in rows:
        if FieldType.lower() != "date":
            if count == 1:
                min = int(irow.getValue(FieldName))
            elif int(irow.getValue(FieldName)) > min:
                max = int(irow.getValue(FieldName))
        else: # http://gis.stackexchange.com/questions/17097/arcpy-cursors-where-clauses-and-date-time-fields
            # datetime.strptime(str(irow.getValue(FieldName)), '%m/%d/%Y')
            if count == 1:
                min = str(irow.getValue(FieldName))
                #min = str(datetime.strptime(str(irow.getValue(FieldName)), '%m/%d/%Y'))
            max = str(irow.getValue(FieldName))
            #max = str(datetime.strptime(str(irow.getValue(FieldName)), '%m/%d/%Y'))
        count += 1
    del rows, irow

    return str(min), str(max)


### Retrieve the a list of unique values for specified field within table
def Get_Unique(InDS2, FieldName):

    # Create a frequency table of the field of interest
    # FREQUENCY: field with count
    # field_name: Unique text values
    try:
        # Delete output table if it exists
        if arcpy.Exists(MYout_table2):
            arcpy.Delete_management(MYout_table2)

        # Frequency_analysis (in_table, out_table, frequency_fields, {summary_fields})
        arcpy.Frequency_analysis(InDS2, MYout_table2, FieldName)
    except:
        print arcpy.GetMessages()
        sys.exit(1)

    try:
        # SearchCursor (dataset, {where_clause}, {spatial_reference}, {fields}, {sort_fields})
        rows = arcpy.SearchCursor(MYout_table2, "", "", FieldName, FieldName + " A")
    except:
        print arcpy.GetMessages()
        sys.exit(1)

    Results = []
    for irow in rows:
        try:
            Val = str(irow.getValue(FieldName))
            if Val == "": Val = "Null"
            ValCnt = str(int(irow.getValue("FREQUENCY")))
            Results.append((Val, ValCnt))
        except:
            print "Error using search cursor on field.", "Get_Unique:", FieldName
            print arcpy.GetMessages()
            #sys.exit(1)
    del rows, irow

    # Delete output table if it exists
    if arcpy.Exists(MYout_table2):
        arcpy.Delete_management(MYout_table2)

    return Results


#====================================================================
# Retrieve Raster attribute information
#====================================================================
def Retrieve_RasterAttributes(Domain_Raster):

    ### Raster Properties
    # GetRasterProperties_management (in_raster, property_type)
    # BuildRasterAttributeTable_management(in_raster, overwrite)
    # CalculateStatistics_management (in_raster_dataset, x_skip_factor, y_skip_factor,
    #   ignore_values)
    # Frequency_analysis (in_table, out_table, frequency_fields, summary_fields)
    # MakeTableView_management(in_table, out_view, where_clause, workspace, field_info)

    ### Default list assignment
    Raster_DetailEnty = []
    Raster_DetailEnty_VAT = []
    myTable = "RasterAttTblLYR"

    ### Get band count
    try:
        RasBandCount = int(str(arcpy.GetRasterProperties_management(InDS, "BANDCOUNT")))
    except: print arcpy.GetMessages()

    '''
    ### Update the statistics and VAT--not sure we want to automate this in case table has additional info
    try:
        arcpy.CalculateStatistics_management(InDS)
    except: print arcpy.GetMessages()

    ### Assume raster attribute table already built because we do not want to overwrite
    try: arcpy.BuildRasterAttributeTable_management(InDS, "OVERWRITE")
    except: pass
    '''


    # Raster_DetailEnty = [("Meta_Type", "RANGE"), ("BandName", BandName), \
    #   ("NoData", myNullValue), ("Min", myMin), ("Max", myMax)]
    if Domain_Raster == "RANGE":
        print "\n\tThe number of unique values in your raster dataset exceeds your threshold and"
        print "\ttherefore if you need an Enumerated_Domain_Value increase your threshold parameter."
        try: print "\n\tUnique value count:", myUniqueNum
        except: pass
        print "\tThreshold value:", UniqCountThreshold

        Raster_DetailEnty = [("Meta_Type", "RANGE")]

        ### Enumerate bands and get information
        for i in range(1, RasBandCount+1):
            ### Band name
            RasterBand = os.path.join(InDS, "Band_" + str(i))
            BandName = "Band_" + str(i)


            if arcpy.Exists(RasterBand):
                try:
                    myNullValue = str(arcpy.Describe(RasterBand).noDataValue)
                    myMin = str(arcpy.GetRasterProperties_management(RasterBand, "MINIMUM"))
                    myMax = str(arcpy.GetRasterProperties_management(RasterBand, "MAXIMUM"))
                except: print arcpy.GetMessages()
            else:
                try:
                    #myNullValue = str(arcpy.Describe(InDS).noDataValue) # Bug
                    myNullValue = str(arcpy.Raster(InDS).noDataValue) # works
                    myMin = str(arcpy.GetRasterProperties_management(InDS, "MINIMUM"))
                    myMax = str(arcpy.GetRasterProperties_management(InDS, "MAXIMUM"))
                except: print arcpy.GetMessages()

            ### Raster_DetailEnty = [(Meta_Type, "RANGE"), (min, x), (max, x)]
            Raster_DetailEnty.append(("BandName", BandName))
            Raster_DetailEnty.append(("NoData", myNullValue))
            Raster_DetailEnty.append(("Min", myMin))
            Raster_DetailEnty.append(("Max", myMax))

        return Raster_DetailEnty, Raster_DetailEnty_VAT

    else:
        # Raster_DetailEnty_VAT = [("Meta_Type", "ENUM"), \
        # ("BandName", BandName), ("NoData", myNullValue)
        # ("Class", str(ClassVal)), ("ClassCount", str(ClassCount))]

        Raster_DetailEnty_VAT = [("Meta_Type", Domain_Raster)]

        ### Enumerate bands and get information
        for i in range(1, RasBandCount+1):
            ### Band name
            RasterBand = os.path.join(InDS, "Band_" + str(i))
            BandName = "Band_" + str(i)

            try:
                # MakeRasterLayer_management (in_raster, out_rasterlayer, {where_clause},
                #   {envelope}, {band_index})
                if arcpy.Exists(RasterBand):
                    myNullValue = str(arcpy.Describe(RasterBand).noDataValue)
                else:
                    #myNullValue = str(arcpy.Describe(InDS).noDataValue) # Bug
                    myNullValue = str(arcpy.Raster(InDS).noDataValue) # works
            except: print arcpy.GetMessages()

            ### Store band number
            Raster_DetailEnty_VAT.append(("BandName", BandName))
            Raster_DetailEnty_VAT.append(("NoData", myNullValue))


            ### Enumerate table view
            Unique_classList = []
            # SearchCursor (dataset, {where_clause}, {spatial_reference}, {fields}, {sort_fields})
            rows = arcpy.SearchCursor(InDS, "", "", "", "Value A")
            for irow in rows: # What if there is more than one field for raster
                try:
                    ClassVal = str(int(irow.getValue("Value")))
                    ClassCount = str(int(irow.getValue("Count")))
                except:
                    pass
                    #print arcpy.GetMessages()

                ### Raster_DetailEnty = [(Class, ClassVal), (min, x), (max, x)]
                Raster_DetailEnty_VAT.append(("Class", str(ClassVal)))
                Raster_DetailEnty_VAT.append(("ClassCount", str(ClassCount)))
            try: del rows
            except: pass

        return Raster_DetailEnty, Raster_DetailEnty_VAT


#====================================================================
# Spatial extent for XML
#====================================================================
def XML_Bounding_Box(GCS_ExtentList):
    FileOutW = open(OutXML_Tmp, 'w')
    # GCS_ExtentList = [extent.XMin, extent.YMin, extent.XMax, extent.YMax]

    FileOutW.write("\nSPATIAL_DOMAIN")
    FileOutW.write(ElemTab_2 + "<spdom>")
    FileOutW.write(ElemTab_3 + "<descgeog>[Latitude/Longitude Bounding Extent]</descgeog>")
    FileOutW.write(ElemTab_3 + "<bounding>")
    FileOutW.write(ElemTab_4 + "<westbc>" + str(GCS_ExtentList[0]) + "</westbc>")
    FileOutW.write(ElemTab_4 + "<eastbc>" + str(GCS_ExtentList[2]) + "</eastbc>")
    FileOutW.write(ElemTab_4 + "<northbc>" + str(GCS_ExtentList[3]) + "</northbc>")
    FileOutW.write(ElemTab_4 + "<southbc>" + str(GCS_ExtentList[1]) + "</southbc>")
    FileOutW.write(ElemTab_3 + "</bounding>")
    FileOutW.write(ElemTab_2 + "</spdom>")

    FileOutW.close()
    del FileOutW


#====================================================================
# Spatial Data Organization Information
#   We will not handle VPF data, becuase not commonly used at FORT
#   Follows Data quality: </dataqual>
#   Proceeds Spatial reference: <spref>
# -------------------------------------------------------------------
def XML_Spatial_Data_Organization(myDataType, myFeatType):

    ### Open Parent Compound ==========================================
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write("\nSPATIAL_DATA_ORGANIZATION")
    FileOutW.write(ElemTab_1 + "<spdoinfo>")


    ### Indirect ==========================================
    FileOutW.write(ElemTab_2 + "<indspref>[Insert a descriptive location reference here]</indspref>")


    ### Direct ==========================================
    # Direct_Spatial_Reference_Method = ["Point", "Vector", "Raster"] (Possible types, list exists elsewhere).
    # myDataType = ["Raster", "Vector"] (Possible types, list exists elsewhere).
    # myFeatType = ["Polygon", "Polyline", "Point", "None"] # None: raster, table, feature DS (Possible types, list exists elsewhere).
    if myDataType == "Vector":
        if myFeatType == "Point":
            FileOutW.write(ElemTab_2 + "<direct>" + Direct_Spatial_Reference_Method[0] + "</direct>")
        else:
            FileOutW.write(ElemTab_2 + "<direct>" + Direct_Spatial_Reference_Method[1] + "</direct>")
    if myDataType == "Raster":
        FileOutW.write(ElemTab_2 + "<direct>" + Direct_Spatial_Reference_Method[2] + "</direct>")


    ### Point and Vector object dinformation
    
    if myFeatType in ["Point", "Polyline"]:
        if myFeatType == "Point":
            SDTS_Type = "Entity point" # Usually this versus "Point"--see meta standard
        if myFeatType == "Polyline":
            # In most cases these will be accurate, but in some cases the user may want to change
            #   to a different type that is more specific
            if os.path.splitext(InDS)[1] == ".shp":
                SDTS_Type = "String" # shapefiles can never have topology
            else:
                SDTS_Type = "Link" # other feature classes MAY have topology

        try: ObjCount = str(arcpy.GetCount_management(InDS2))
        except:
            print "\t\tError obtaining object count for point dataset.", InDS2
            print "\t\t", arcpy.GetMessages(2)
            ObjCount = "0"

        FileOutW.write(ElemTab_2 + "<ptvctinf>")
        # ------ These are repeatable
        FileOutW.write(ElemTab_3 + "<sdtsterm>")
        FileOutW.write(ElemTab_4 + "<sdtstype>" + SDTS_Type + "</sdtstype>")
        FileOutW.write(ElemTab_4 + "<ptvctcnt>" + ObjCount + "</ptvctcnt>")
        FileOutW.write(ElemTab_3 + "</sdtsterm>")
        # ------
        FileOutW.write(ElemTab_2 + "</ptvctinf>")


    elif myFeatType == "Polygon":
        SDTS_Type = "G-polygon"
        try: ObjCount = str(arcpy.GetCount_management(InDS2))
        except:
            print "\t\tError obtaining object count for vector (polygon) dataset.", InDS
            print "\t\t", arcpy.GetMessages(2)
            ObjCount = "0"
        FileOutW.write(ElemTab_2 + "<ptvctinf>")
        # ------ These are repeatable
        FileOutW.write(ElemTab_3 + "<sdtsterm>")
        FileOutW.write(ElemTab_4 + "<sdtstype>" + SDTS_Type + "</sdtstype>")
        FileOutW.write(ElemTab_4 + "<ptvctcnt>" + ObjCount + "</ptvctcnt>")
        FileOutW.write(ElemTab_3 + "</sdtsterm>")
        # ------
        # ------ These are repeatable
        FileOutW.write(ElemTab_3 + "<sdtsterm>")
        FileOutW.write(ElemTab_4 + "<sdtstype>Label point</sdtstype>")
        FileOutW.write(ElemTab_4 + "<ptvctcnt>" + ObjCount + "</ptvctcnt>")
        FileOutW.write(ElemTab_3 + "</sdtsterm>")
        # ------
        FileOutW.write(ElemTab_2 + "</ptvctinf>")


    elif myDataType == "GeometricNetwork":
        SDTS_Type = "Network chain, nonplanar graph"

        # Locate Polyline feature class within network data
        NetDS = "" # Clear
        NetWS = os.path.dirname(InDS)
        desc = arcpy.Describe(InDS)
        FClist = desc.featureClassNames
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
                    print "\t\tError obtaining object count for vector (line) dataset.", InDS
                    print "\t\t", arcpy.GetMessages(2)
                    ObjCount = "0"
            else:
                print "\t\tError returning information about Geonetwork.", InDS
                print "\t\t", arcpy.GetMessages(2)
                sys.exit(1)

            FileOutW.write(ElemTab_2 + "</ptvctinf>") #??????????????????????????????????????????????????????????????
            # ------ These are repeatable
            FileOutW.write(ElemTab_3 + "<sdtsterm>")
            FileOutW.write(ElemTab_4 + "<sdtstype>" + SDTS_Type + "</sdtstype>")
            FileOutW.write(ElemTab_4 + "<ptvctcnt>" + ObjCount + "</ptvctcnt>")
            FileOutW.write(ElemTab_3 + "</sdtsterm>")
            # ------
            FileOutW.write(ElemTab_2 + "</ptvctinf>")


    ### Raster object information ==========================================
    elif myDataType == "Raster":
        # Raster_Object_Type = ["Point", "Pixel", "Grid Cell", "Voxel"]
        RasterType = "Grid Cell" # This is most probable answer
        try:
            RowCount = str(arcpy.GetRasterProperties_management(InDS, "ROWCOUNT"))
            ColCount = str(arcpy.GetRasterProperties_management(InDS, "COLUMNCOUNT"))
            BandCount = str(arcpy.GetRasterProperties_management(InDS, "BANDCOUNT"))
        except:
            print "Error running raster dataset properties tool.", InDS
            print arcpy.GetMessages(2)
            sys.exit(1)

        # Write results to XML
        FileOutW.write(ElemTab_2 + "<rastinfo>")
        FileOutW.write(ElemTab_3 + "<rasttype>" + RasterType + "</rasttype>")
        FileOutW.write(ElemTab_3 + "<rowcount>" + RowCount + "</rowcount>")
        FileOutW.write(ElemTab_3 + "<colcount>" + ColCount + "</colcount>")
        FileOutW.write(ElemTab_3 + "<vrtcount>" + BandCount + "</vrtcount>")
        FileOutW.write(ElemTab_2 + "</rastinfo>")



    ### Close Parent Compound
    FileOutW.write(ElemTab_1 + "</spdoinfo>")
    FileOutW.close()
    del FileOutW



#====================================================================
# Spatial Reference for XML
#   Follows xx: </spref>
# -------------------------------------------------------------------
def XML_SpatialReference(SR_List):

    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write("\nSPATIAL_REFERENCE")
    FileOutW.close()
    del FileOutW

    ### ----------------------------------------------------------------------------
    ### Create coumpound element for spatial reference
    Open_SpatialRef() #-- This will be left in the metadata template after cleanup
    Open_Horizontal()

    ### ------------------------------------- Horizontal Coordinate System
    # Geographic coordinate system--"GCSname" is defined for map projections so
    #   check whether PCSname is unknown
    if SR_List["PCSname"] == "[Unknown]":
        Geographic(SR_List)

        # Add geodetic Model now; this is used if using an ellipsoid or spheroid
        Geodetic(SR_List)

    ### ------------------------------------- Include this in addition to horizontal when applicable
    # Vertical coordinate system
    if Vertical_CS_Switch == "Present" or SR_List["VCSname"] != "[Unknown]":
        Vertical_CS(SR_List)

    # Projected map projection and Grid map projection
    elif SR_List["PCSname"] != "[Unknown]":
        if SR_List["UTM_Zone"] == "[Unknown]" and SR_List["SPCS_Zone"] == "[Unknown]" and \
                SR_List["UPS_Zone"] == "[Unknown]" and SR_List["Arc_Zone"] == "[Unknown]":
            
            # Map Projection
            ProjName = SR_List["PrjName"].lower().replace("_", " ")
            
            if "albers" in ProjName:
                Albers_Conical_Equal_Area(SR_List)
            elif "azimuthal" and "equidistant" in ProjName:
                Azimuthal_Equidistant(SR_List)
            elif "equidistant" and "conic" in ProjName:
                Equidistant_Conic(SR_List)
            elif "equirectangular" in ProjName:
                Equirectangular(SR_List)
            elif "general" and "vertical" and "near" and "perspective" in ProjName:
                General_Vertical_Near_sided_Perspective(SR_List)
            elif "gnomonic" in ProjName:
                Gnomonic(SR_List)
            elif "lambert" and "azimuthal" in ProjName:
                Lambert_Azimuthal_Equal_Area(SR_List)
            elif "lambert" and "conformal" and "conic" in ProjName:
                Lambert_Conformal_Conic(SR_List)
            elif "modified" and "stereographic" and "alaska" in ProjName:
                Modified_Stereographic_for_Alaska()
            elif "miller" and "cylindrical" in ProjName:
                Miller_Cylindrical(SR_List)
            
            elif "space" and "oblique" and "mercator" and "landsat" in ProjName:
                Space_Oblique_Mercator_Landsat(SR_List)            
            elif "transverse" and "mercator" in ProjName:
                Transverse_Mercator(SR_List)           
            elif "oblique" and "mercator" in ProjName:
                Oblique_Mercator(SR_List)                        
            elif "mercator" in ProjName:
                Mercator(SR_List)


            elif "polar" and "stereographic" in ProjName:
                Polar_Stereographic(SR_List)
            elif "stereographic" in ProjName:
                Stereographic(SR_List)            
            
            
            elif "orthographic" in ProjName:    
                Orthographic(SR_List)
            elif "polyconic" in ProjName:
                Polyconic(SR_List)
            elif "robinson" in ProjName:
                Robinson(SR_List)
            elif "sinusoidal" in ProjName:
                Sinusoidal(SR_List)
            elif "van" and "der" and "grinten" in ProjName:
                van_der_Grinten(SR_List)
            else:
                Other_MapProjections(SR_List)

        else:
            if SR_List["UTM_Zone"] != "[Unknown]":
                Universal_Transverse_Mercator(SR_List)
            elif SR_List["SPCS_Zone"] != "[Unknown]":
                State_Plane_Coordinate_System(SR_List)
            elif SR_List["UPS_Zone"] != "[Unknown]":
                Universal_Polar_Stereographic(SR_List)
            elif SR_List["Arc_Zone"] != "[Unknown]":
                ARC_Coordinate_System(SR_List)
            else:
                # This will not work because do not have a mechanism to determine
                #   whether using a grid or not (instead it will go into Other_MapProjections())
                Other_Grid_System(SR_List)

        # Planar coordinate Information
        Planar_CoordInfo(myDataType, SR_List)

        # Add geodetic Model now; this is used if using an ellipsoid or spheroid
        Geodetic(SR_List)

    ### ------------------------------------- Local -- not including
    else:
        pass


    ### Close horizon compound element for spatial reference
    Close_Horizontal()

    ### ------------------------------------- Include this in addition to horizontal when applicable
    # Vertical coordinate system
    if Vertical_CS_Switch == "Present" or SR_List["VCSname"] != "[Unknown]":
        Vertical_CS(SR_List)


    ### Close compound element for spatial reference
    Close_SpatialRef() # this will be left in template so do not include here


#====================================================================
# Entity and Attribute Information for XML
#   Follows spatial reference: </spref>
# -------------------------------------------------------------------
# Enumerated_Domain: string values
# Range_Domain: Sequence, series, scale of numeric values
# Unrepresentable_Domain: Values cannot be represented because of character
#   set or form that is impossible to represent
# String values represented as numeric
#====================================================================
def XML_Entity_and_Attribute_Info():

    ### Open Parent Compound
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write("\nENTITY_ATTRIBUTE_INFO")
    FileOutW.write(ElemTab_1 + "<eainfo>")
    FileOutW.write(ElemTab_2 + "<detailed>")
    FileOutW.write(ElemTab_3 + "<enttyp>")
    FileOutW.write(ElemTab_4 + "<enttypl>Attribute Table for Dataset</enttypl>")
    FileOutW.write(ElemTab_4 + "<enttypd>Attribute descriptions</enttypd>")
    FileOutW.write(ElemTab_4 + "<enttypds>Producer defined</enttypds>")
    FileOutW.write(ElemTab_3 + "</enttyp>")
    FileOutW.close()
    del FileOutW


def XML_Attribute(AttList):

    ### Open file
    FileOutW = open(OutXML_Tmp, 'a')

    ### Enumerate list
    FieldName = ''
    FieldNewCount = 0
    for iAttListCnt in range(0, len(AttList)):
        ### Set up data type and domain type
        if AttList[iAttListCnt][0] == "DOMAIN":
            DataType = "Vector"                      # "DOMAIN" or "Meta_Type"
            Domain = AttList[iAttListCnt][1]         # "ENUM", "CODESET", "UNREP", "RANGE"
        elif AttList[iAttListCnt][0] == "Meta_Type":
            DataType = "Raster"
            Domain = AttList[iAttListCnt][1]

        ### Determine if we are evaluaing a new field
        if AttList[iAttListCnt][0] == "FieldName":
            FieldName = AttList[iAttListCnt][1]
            FieldNew = True # Allows repeats
            FieldNewCount += 1
        elif AttList[iAttListCnt][0] != "FieldName":
            FieldNew = False # Allows repeats
            FieldTmp = AttList[iAttListCnt][0]


        ### Vector and point data --------------------------------------------------
        if DataType == "Vector":
            # AttribList_DET= [("DOMAIN", "ENUM"), ("FieldName", FieldName), (Val, ValCnt)]
            # AttribList_DET= [("DOMAIN", "CODESET"), ("FieldName", FieldName), (Val, ValCnt)]
            # AttribList_DET= [("DOMAIN", "UNREP"), ("FieldName", FieldName), (Val, ValCnt)]
            # AttribList_DET= [("DOMAIN", "RANGE"), ("FieldName", FieldName), ("Min", min), ("Max", max)]

            ### Do this once for each new field
            if len(FieldName) > 0 and FieldNew == True:
                if FieldName.lower() == "objectid": Def = "ESRI Record Identifier"
                elif FieldName.lower() == "oid": Def = "ESRI Record Identifier"
                elif FieldName.lower() == "fid": Def = "ESRI Record Identifier"
                elif FieldName.lower() == "shape_length": Def = "ESRI Shape Length"
                elif FieldName.lower() == "shape_area": Def = "ESRI Shape Area"
                elif FieldName.lower() == "shape": Def = "ESRI Shape Type"
                else: Def = "[User needs to define]"
                #
                if Def == "[User needs to define]":
                    DefSource = "Producer defined"
                else: DefSource = "ESRI"

                # Close previous attribute
                if FieldNewCount > 1: FileOutW.write(ElemTab_3 + "</attr>")

                # Start new attribute
                FileOutW.write(ElemTab_3 + "<attr>")
                FileOutW.write(ElemTab_4 + "<attrlabl>" + FieldName + "</attrlabl>")
                FileOutW.write(ElemTab_4 + "<attrdef>" + Def + "</attrdef>")
                FileOutW.write(ElemTab_4 + "<attrdefs>" + DefSource + "</attrdefs>")

            if Domain == "UNREP" and AttList[iAttListCnt][0] != "DOMAIN" and AttList[iAttListCnt][0] != "FieldName":
                # Captures any failures
                FileOutW.write(ElemTab_4 + "<attrdomv>")
                FileOutW.write(ElemTab_5 + "<udom>[User Must Define Here]</udom>")
                FileOutW.write(ElemTab_4 + "</attrdomv>")


            ### Repeat these when necessary (will vary depending on type of domain)
            if FieldNew == False and AttList[iAttListCnt][0] != "DOMAIN" and AttList[iAttListCnt][0] != "FieldName":
                # Write attribute values using appropriate domain values once we pass fieldname list item
                if Domain == "UNREP": # This is not writing to output because of above if statement
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<udom>[User Must Define Here]</udom>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                elif Domain == "ENUM":
                    if FieldName.lower() == "shape":
                        Val = AttList[iAttListCnt][1]
                    else:
                        Freq = AttList[iAttListCnt][1]
                        Val = AttList[iAttListCnt][0] + " (Freq: " + Freq + ")"
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<edom>")
                    FileOutW.write(ElemTab_6 + "<edomv>" + Val + "</edomv>")
                    FileOutW.write(ElemTab_6 + "<edomvd>" + Def + "</edomvd>")
                    FileOutW.write(ElemTab_6 + "<edomvds>" + DefSource + "</edomvds>")
                    FileOutW.write(ElemTab_5 + "</edom>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                elif Domain == "RANGE":
                    if AttList[iAttListCnt][0] == "Min" and AttList[iAttListCnt+1][0] == "Max":
                        Min = AttList[iAttListCnt][1]
                        Max = AttList[iAttListCnt+1][1]
                        FileOutW.write(ElemTab_4 + "<attrdomv>")
                        FileOutW.write(ElemTab_5 + "<rdom>")
                        FileOutW.write(ElemTab_6 + "<rdommin>" + Min + "</rdommin>")
                        FileOutW.write(ElemTab_6 + "<rdommax>" + Max + "</rdommax>")
                        FileOutW.write(ElemTab_5 + "</rdom>")
                        FileOutW.write(ElemTab_4 + "</attrdomv>")
                elif Domain == "CODESET":
                    Freq = AttList[iAttListCnt][1]
                    Val = AttList[iAttListCnt][0] + " (Freq: " + Freq + ")"
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<codesetd>")
                    FileOutW.write(ElemTab_6 + "<codesetn>" + Val + "</codesetn>")
                    FileOutW.write(ElemTab_6 + "<codesets>[User Required to Provide Pub Ref]</codesets>")
                    FileOutW.write(ElemTab_5 + "</codesetd>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                else:
                    # Captures any failures
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<udom>[User Must Define Here]</udom>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")

    ### Close attribute compound when completed enumerating all attribute fields
    FileOutW.write(ElemTab_3 + "</attr>")
    #if iAttListCnt == len(AttList)-1:
    #    FileOutW.write(ElemTab_3 + "</attr>")

    ### Raster data ------------------------------------------------------------
    # This was indented one more level but I do not think it should have been
    if DataType == "Raster":
        if Domain == "RANGE":
            # Raster_DetailEnty = [("Meta_Type", "RANGE"), ("BandName", BandName), \
            #     ("NoData", myNullValue), ("Min", myMin), ("Max", myMax)]
            ListCounter = 0
            for iBand in AttList:
                if iBand[0] == "BandName":
                    iField = AttList[ListCounter][1] # Band number
                    BandMin = AttList[ListCounter + 2][1]
                    BandMax = AttList[ListCounter + 3][1]

                    FileOutW.write(ElemTab_3 + "<attr>")
                    FileOutW.write(ElemTab_4 + "<attrlabl>Value (" + iField + ")</attrlabl>")
                    FileOutW.write(ElemTab_4 + "<attrdef>Raster Value Information</attrdef>")
                    FileOutW.write(ElemTab_4 + "<attrdefs>User Defined</attrdefs>")
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<rdom>")
                    FileOutW.write(ElemTab_6 + "<rdommin>" + BandMin + "</rdommin>")
                    FileOutW.write(ElemTab_6 + "<rdommax>" + BandMax + "</rdommax>")
                    FileOutW.write(ElemTab_5 + "</rdom>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                    FileOutW.write(ElemTab_3 + "</attr>")
                ListCounter += 1
        # Raster table values referenced in a publication or report and therefore
        #   we do not need to define these as ENUM
        if Domain == "CODESET":
            # Raster_DetailEnty_VAT = [("Meta_Type", "CODESET"), \
            #    ("BandName", BandName), ("NoData", myNullValue)
            #    ("Class", str(ClassVal)), ("ClassCount", str(ClassCount))]
            ListCounter = 0
            for iBand in AttList:
                if iBand[0] == "BandName":
                    iField = AttList[ListCounter][1] # Band number
                    BandMin = AttList[ListCounter + 2][1]
                    BandMax = AttList[ListCounter + 3][1]
                    Source = "[User Needs to Define Pub Ref]"
                    FileOutW.write(ElemTab_3 + "<attr>")
                    FileOutW.write(ElemTab_4 + "<attrlabl>Value: (" + iField + ")</attrlabl>")
                    FileOutW.write(ElemTab_4 + "<attrdef>Value Attribute Table</attrdef>")
                    FileOutW.write(ElemTab_4 + "<attrdefs>" + Source + "</attrdefs>")
                    #FileOutW.write(ElemTab_4 + "<attrdomv>")
                if iBand[0] == "Class":
                    Freq = AttList[ListCounter + 1][1]
                    Name = AttList[ListCounter][0] + " (Freq: " + Freq + ")"
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<codesetd>")
                    FileOutW.write(ElemTab_6 + "<codesetn>" + Name + "</codesetn>")
                    FileOutW.write(ElemTab_6 + "<codesets>Publication Reference</codesets>")
                    FileOutW.write(ElemTab_5 + "</codesetd>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                FileOutW.write(ElemTab_3 + "</attr>")
                ListCounter += 1


        # Raster table values are not referenced in a publication or report and therefore
        #   we need to define these as ENUM
        if Domain == "ENUM":
            # Raster_DetailEnty_VAT = [("Meta_Type", "ENUM"), \
            #    ("BandName", BandName), ("NoData", myNullValue)
            #    ("Class", str(ClassVal)), ("ClassCount", str(ClassCount))]
            ListCounter = 0
            for iBand in AttList:
                if iBand[0] == "BandName":
                    iField = AttList[ListCounter][1]
                    BandMin = AttList[ListCounter + 2][1]
                    BandMax = AttList[ListCounter + 3][1]
                    FileOutW.write(ElemTab_3 + "<attr>")
                    FileOutW.write(ElemTab_4 + "<attrlabl>Value: (" + iField + ")</attrlabl>")
                    FileOutW.write(ElemTab_4 + "<attrdef>Value Attribute Table</attrdef>")
                    FileOutW.write(ElemTab_4 + "<attrdefs>User Defined</attrdefs>")
                    #FileOutW.write(ElemTab_4 + "<attrdomv>")
                if iBand[0] == "Class":
                    Freq = AttList[ListCounter + 1][1]
                    Name = AttList[ListCounter][1] + " (Freq: " + Freq + ")"
                    FileOutW.write(ElemTab_4 + "<attrdomv>")
                    FileOutW.write(ElemTab_5 + "<edom>")
                    FileOutW.write(ElemTab_6 + "<edomv>" + Name + "</edomv>")
                    FileOutW.write(ElemTab_6 + "<edomvd>[User Definition]</edomvd>")
                    FileOutW.write(ElemTab_6 + "<edomvds>Producer defined</edomvds>")
                    FileOutW.write(ElemTab_5 + "</edom>")
                    FileOutW.write(ElemTab_4 + "</attrdomv>")
                FileOutW.write(ElemTab_3 + "</attr>")
                ListCounter += 1

    # This was indented one more level but I do not think it should have been
    ### ------------------------------------------------------------

    # Close
    FileOutW.close()
    del FileOutW


def XML_Attribute_Close():
    FileOutW = open(OutXML_Tmp, 'a')
    FileOutW.write(ElemTab_2 + "</detailed>")
    FileOutW.write(ElemTab_2 + "<overview>")
    FileOutW.write(ElemTab_3 + "<eaover>[Insert general description of Attributes. If not desired, delete 'overview' element]</eaover>")
    FileOutW.write(ElemTab_3 + "<eadetcit>Producer Defined</eadetcit>")
    FileOutW.write(ElemTab_2 + "</overview>")
    FileOutW.write(ElemTab_1 + "</eainfo>\n") # Not sure why I need to add the "\n" but screwed something up somewhere
    FileOutW.close()
    del FileOutW



#====================================================================
# Create final XML file by incorporating XML sections created with script
#====================================================================
def Create_Final_XML():

    ### Defaults for locating metdata elements in template
    SPDOM_Write, SPDOINFO_Write, SPREF_Write, EAINFO_Write = False, False, False, False

    FileInR_XML_Parsed = open(OutXML_b, 'r') # Parsed xml template
    FileInR_XML_tmp = open(OutXML_Tmp, 'r') # xml generated for metadata sections using this script
    FileInW_XML = open(OutXML, 'w') # integrate the above two scripts: final xml file for user

    # ["SPATIAL_DOMAIN", "SPATIAL_DATA_ORGANIZATION", "SPATIAL_REFERENCE", "ENTITY_ATTRIBUTE_INFO"]
    for iXML_Parse_Line in FileInR_XML_Parsed:
        if iXML_Parse_Line.find("<spdom>") > 0:
            FileInR_XML_tmp.seek(0,0)
            Start = False
            for iXML_tmp_Line in FileInR_XML_tmp:
                if iXML_tmp_Line.find("SPATIAL_DATA_ORGANIZATION") == 0:
                    break
                if Start:
                    FileInW_XML.write(iXML_tmp_Line)
                if iXML_tmp_Line.find("SPATIAL_DOMAIN") == 0:
                    Start = True
            SPDOM_Write = True
        elif iXML_Parse_Line.find("<spdoinfo>") > 0:
            FileInR_XML_tmp.seek(0,0)
            Start = False
            for iXML_tmp_Line in FileInR_XML_tmp:
                if iXML_tmp_Line.find("SPATIAL_REFERENCE") == 0:
                    break
                if Start:
                    FileInW_XML.write(iXML_tmp_Line)
                if iXML_tmp_Line.find("SPATIAL_DATA_ORGANIZATION") == 0:
                    Start = True
            SPDOINFO_Write = True
        elif iXML_Parse_Line.find("<spref>") > 0:
            FileInR_XML_tmp.seek(0,0)
            Start = False
            for iXML_tmp_Line in FileInR_XML_tmp:
                if iXML_tmp_Line.find("ENTITY_ATTRIBUTE_INFO") == 0:
                    break
                if Start:
                    FileInW_XML.write(iXML_tmp_Line)
                if iXML_tmp_Line.find("SPATIAL_REFERENCE") == 0:
                    Start = True
            SPREF_Write = True
        elif iXML_Parse_Line.find("<eainfo>") > 0:
            FileInR_XML_tmp.seek(0,0)
            Start = False
            for iXML_tmp_Line in FileInR_XML_tmp:
                if Start:
                    FileInW_XML.write(iXML_tmp_Line)
                if iXML_tmp_Line.find("ENTITY_ATTRIBUTE_INFO") == 0:
                    Start = True
            EAINFO_Write = True
        else:
            if iXML_Parse_Line.find("</spdom>") <= 0 and iXML_Parse_Line.find("</spdoinfo>") <= 0 and \
                iXML_Parse_Line.find("</spref>") <= 0 and iXML_Parse_Line.find("</eainfo>") <= 0:
                FileInW_XML.write(iXML_Parse_Line)

    ### Clean up
    FileInR_XML_tmp.close()
    del FileInR_XML_tmp
    FileInR_XML_Parsed.close()
    del FileInR_XML_Parsed
    FileInW_XML.close()
    del FileInW_XML


    ### If any section was not written to the final XML then append to the end of the file
    ###   and the user can move to the correct location.
    ### This will only happen if there was an error with the input template
    if SPDOM_Write == False:
        FileInR_XML_tmp = open(OutXML_Tmp, 'r') # xml generated for metadata sections using this script
        FileInW_XML = open(OutXML, 'w') # integrate the above two scripts: final xml file for user
        FileInW_XML.write("\n\n++++++++++++ Fix")

        Start = False
        for iXML_tmp_Line in FileInR_XML_tmp:
            if iXML_tmp_Line.find("SPATIAL_DATA_ORGANIZATION") == 0:
                break
            if Start:
                FileInW_XML.write(iXML_tmp_Line)
            if iXML_tmp_Line.find("SPATIAL_DOMAIN") == 0:
                Start = True

        FileInR_XML_tmp.close()
        del FileInR_XML_tmp
        FileInW_XML.close()
        del FileInW_XML
    if SPDOINFO_Write == False:
        FileInR_XML_tmp = open(OutXML_Tmp, 'r') # xml generated for metadata sections using this script
        FileInW_XML = open(OutXML, 'w') # integrate the above two scripts: final xml file for user
        FileInW_XML.write("\n\n++++++++++++ Fix")

        Start = False
        for iXML_tmp_Line in FileInR_XML_tmp:
            if iXML_tmp_Line.find("SPATIAL_REFERENCE") == 0:
                break
            if Start:
                FileInW_XML.write(iXML_tmp_Line)
            if iXML_tmp_Line.find("SPATIAL_DATA_ORGANIZATION") == 0:
                Start = True

        FileInR_XML_tmp.close()
        del FileInR_XML_tmp
        FileInW_XML.close()
        del FileInW_XML
    if SPREF_Write == False:
        FileInR_XML_tmp = open(OutXML_Tmp, 'r') # xml generated for metadata sections using this script
        FileInW_XML = open(OutXML, 'w') # integrate the above two scripts: final xml file for user
        FileInW_XML.write("\n\n++++++++++++ Fix")

        Start = False
        for iXML_tmp_Line in FileInR_XML_tmp:
            if iXML_tmp_Line.find("ENTITY_ATTRIBUTE_INFO") == 0:
                break
            if Start:
                FileInW_XML.write(iXML_tmp_Line)
            if iXML_tmp_Line.find("SPATIAL_REFERENCE") == 0:
                Start = True

        FileInR_XML_tmp.close()
        del FileInR_XML_tmp
        FileInW_XML.close()
        del FileInW_XML
    if EAINFO_Write == False:
        FileInR_XML_tmp = open(OutXML_Tmp, 'r') # xml generated for metadata sections using this script
        FileInW_XML = open(OutXML, 'w') # integrate the above two scripts: final xml file for user
        FileInW_XML.write("\n\n++++++++++++ Fix")

        Start = False
        for iXML_tmp_Line in FileInR_XML_tmp:
            if Start:
                FileInW_XML.write(iXML_tmp_Line)
            if iXML_tmp_Line.find("ENTITY_ATTRIBUTE_INFO") == 0:
                Start = True

        FileInR_XML_tmp.close()
        del FileInR_XML_tmp
        FileInW_XML.close()
        del FileInW_XML


#====================================================================
# Clean Up
#====================================================================
def CleanUp():

    ### Delete all remaining variables
    for v in vars().copy(): del v



####################################################################################
####################################################################################
####################################################################################
if __name__ == '__main__':

    # --------------------------------------------------------------------------
    # SETUP --------------------------------------------------------------------
    # Print input parameters
    print "\nParameters ========================================================"
    print "Input Dataset:", InDS
    print "Data refraction scale:", DataScale
    print "Digitizer resolution: (Not specified by User)", DigPrecision
    print "XML Template (Provided by user):", InXML
    print "Modified XML Output (Created for user):", OutXML
    print "Field list:", FieldListMeta
    print "Vertical Coord Sys:", Vertical_CS_Switch
    print "=====================================================================\n"

    print "\n\n====================================================================="
    print "SETUP"
    print "\tRemove interim files..."
    Delete_InterimFiles()
    if os.path.exists(OutXML):
        os.remove(OutXML)

    print "\tSetup for XML output file (remove metadata elements we will be updating)..."
    XML_Parse()


    print "\nDetermine what type of data we are working with..."
    myDataType, myFeatType = Data_Type()
    print "\tData type being evaluated:", myDataType
    print "\tFeature type being evaluated:", myFeatType

    ### ------------------------------------------------------------------------
    if myDataType in ["Vector", "Table"]:
        print "\tVerifying if field names and metadata domain for Detailed Entity Attribute match data."
        if myDataType == "Vector":
            try: arcpy.MultipartToSinglepart(InDS, InDS2) # Check if multipart
            except: InDS2 = InDS
        if myDataType == "Table":
            InDS2 = InDS
        # Verify list of fields, field type and metadata domain type provided by user
        FieldListMeta = CheckFields(InDS2, myDataType, FieldListMeta)

    ### ------------------------------------------------------------------------
    if myDataType in ["Raster"]:
        ### This applies to all bands and not individual bands
        try:
            # this works when there is a raster attribute table only
            myUniqueNum = int(str(arcpy.GetRasterProperties_management(InDS, "UNIQUEVALUECOUNT")))
        except:
            print arcpy.GetMessages()
            myUniqueNum = 1000000
        #
        if myUniqueNum < UniqCountThreshold:
            # Integer data only
            FieldListMeta = CheckFields(InDS, myDataType, FieldListMeta)
        else:
            # Floating point data
            FieldListMeta = [("VAT", "VAT", "RANGE")]


    # --------------------------------------------------------------------------
    # COLLECT METADATA INFO AND POPULATE ---------------------------------------
    # This will handle all feature types and raster data, but will exclude tables
    print "\n\n\t====================================================================="
    if myDataType != "Table":
        print "\tRetrieve bounding coordinates and update metadata..."
        Local_ExtentList, GCS_ExtentList = Get_LatLon_BndBox()
        XML_Bounding_Box(GCS_ExtentList)

        print "\tRetrieve spatial data organization information and update metadata..."
        XML_Spatial_Data_Organization(myDataType, myFeatType)

        print "\tGet spatial reference and update metadata..."
        SR_List = Get_SpatialRef(SR_InDS, myDataType, myFeatType, GCS_ExtentList)
        XML_SpatialReference(SR_List)


    print "\tRetrieve entity and attribute information and update metadata..."
    # This will handle all data, but for now we have not programmed this for envelop
    #   data types (feature datasets, networks, etc)
    if myDataType == "Vector" or myDataType == "Table":
        ### Get information about each field based on the list and metadata domain choices
        AttribList_DET = Retrieve_VectorAttributes(InDS2, myDataType, FieldListMeta)

    if myDataType == "Raster":
        try:
            Domain_Raster = FieldListMeta[0][2]
            if not Domain_Raster in ["RANGE", "ENUM", "CODESET", "UNREP"]:
                Domain_Raster = ""
        except:
            print "Error validating entity domain values.", str(FieldListMeta), Domain_Raster
            sys.exit(1)
        Raster_DetailEnty, Raster_DetailEnty_VAT = Retrieve_RasterAttributes(Domain_Raster)

    if myDataType in ["Vector", "Table", "Raster"]:
        XML_Entity_and_Attribute_Info()

        if myDataType == "Raster":
            if Raster_DetailEnty != []:
                # Attribute info
                XML_Attribute(Raster_DetailEnty)
            if Raster_DetailEnty_VAT != []:
                XML_Attribute(Raster_DetailEnty_VAT)
        else:
            # Attribute info
            XML_Attribute(AttribList_DET)

        # Close XML syntax for eainfo
        XML_Attribute_Close() #--Not using use because in template
    print "\t====================================================================="



    # --------------------------------------------------------------------------
    # CREATE FINAL XML ---------------------------------------------------------
    # Will need to read though the lines in this cleaned up XML version, write
    #   this to a final output while also inserting the info collected here
    print "\n\n====================================================================="
    print "CREATE FINAL XML"
    Create_Final_XML()


    # --------------------------------------------------------------------------
    print "\n\n====================================================================="
    print "CLEAN UP"
    Delete_InterimFiles()
    try:
        if arcpy.Exists(ScratchWS):
            arcpy.Delete_management(ScratchWS)
    except: pass
    CleanUp()
    print "\n\tProgram Completed."





