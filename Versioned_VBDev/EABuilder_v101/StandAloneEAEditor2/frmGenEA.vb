Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.DataSourcesGDB
Imports ESRI.ArcGIS.DataSourcesFile
Imports System.IO
Imports ESRI.ArcGIS.Geoprocessing


Public Class frmGenEA
    Private pMD As IMetadata
    'Private pGXObject As IGxObject
    'Private pGxDataset As IGxDataset
    Dim pXML As IXmlPropertySet2
    Dim pPs As IPropertySet

    Private sPath As String
    Private sWorkspacePath As String
    Private sFileName As String
    Private sFileNameWithExtension As String
    Private sExt As String
    Private sOutFile As String

    'Private validDataset As Boolean = True
    Private dataType As String = "table" 'Table = any standard table based form, or 'singleband' or 'multiband'
    Public Property filePath() As String
        Get
            Return sPath
        End Get
        Set(ByVal value As String)
            sPath = value
            'pGxDataset = pGXObject
            'pMD = pGXObject
            'pPs = pMD.Metadata
            'pXML = pPs

        End Set
    End Property

    Private pDataset As IDataset
    Private pTable As ITable
    Private pRasterDataSet As IRasterDataset
    Private pRasterBandCollection As IRasterBandCollection
    Private pField As IField
    Private pTableSort As ITableSort
    Private intType As Integer
    Private pFeatureClass As IFeatureClass
    Private strCovName As String
    Private strExtension As String
    Private pFields As IFields

    Private attribs As New Dictionary(Of Integer, attr)

    Public i As Long

    Private curEA As New DetailedDescription

#Region "FGDC_EA_classStructure"

    Private Class DetailedDescription
        Public ET As New EntityType
        Public attributesDict As New Dictionary(Of String, attr)
    End Class

    Private Class EntityType
        Public EntityTypeLabel As String
        Public entityTypeDescription As String
        Public entityTypeDefinitionSource As String

        Public Sub New()
            EntityTypeLabel = "Attribute Table"
            entityTypeDescription = "Table containing attribute information associated with the data set."
            entityTypeDefinitionSource = "Producer defined"
        End Sub
    End Class

    Private Class attr
        Public attrlabl As String
        Public attrdef As String
        Public attrdefs As String
        Public c_attribdomv As attribdomv
    End Class

    Private Class attribdomv
        Public attributeDomainValues() As domain

    End Class

    Private Class domain

    End Class

    Private Class edom
        Inherits domain
        Public edomv As String
        Public edomvd As String
        Public edomvds As String
    End Class

    Private Class rdom
        Inherits domain
        Public rdommin As String
        Public rdommax As String
        Public attrunit As String
        Public attrmes As String
    End Class

    Private Class codesetd
        Inherits domain
        Public codesetn As String
        Public codesets As String
    End Class

    Private Class udom
        Inherits domain
        Public udom As String
    End Class

#End Region

#Region "Loading"

    Private Sub load_dataset()
        sWorkspacePath = System.IO.Path.GetDirectoryName(sPath)
        sFileName = System.IO.Path.GetFileNameWithoutExtension(sPath)
        sFileNameWithExtension = System.IO.Path.GetFileName(sPath)
        sExt = System.IO.Path.GetExtension(sPath)

        Dim pWorkspaceFactory As IWorkspaceFactory
        Dim workspaceType As String = getWorkspaceType()
        If workspaceType = "personalGDB" Then
            pWorkspaceFactory = New AccessWorkspaceFactory
            loadFromGDB(pWorkspaceFactory)
        ElseIf workspaceType = "fileGDB" Then
            pWorkspaceFactory = New FileGDBWorkspaceFactory
            loadFromGDB(pWorkspaceFactory)
        ElseIf workspaceType = "shapefile" Then
            pWorkspaceFactory = New ShapefileWorkspaceFactory
            loadFromGDB(pWorkspaceFactory)
        ElseIf workspaceType = "raster" Then
            loadRaster()
            pMD = pRasterDataSet
        ElseIf workspaceType = "dbf" Then
            pWorkspaceFactory = New ShapefileWorkspaceFactory
            loadFromGDB(pWorkspaceFactory)
        ElseIf workspaceType = "arcTable" Then
            load_arcinfo_table()
        ElseIf workspaceType = "coverage" Then
            loadCoverage()
            'ElseIf workspaceType = "SDEdata" Then
            '    loadSDEdata()
        End If


        pPs = pMD.Metadata
        pXML = pPs

        If dataType = "multiband" Then
            loadMultiBand()
        ElseIf dataType = "singleband" Then
            loadSingleBand()
        Else
            load_strEA(pFields)
        End If

        Me.lstFields.Focus()
        Me.lstFields.Items(0).Selected = True

        Me.txtOverview.Text = existingXMLEntry("eaover")
        Me.txtEadetcit.Text = existingXMLEntry("eadetcit")

        If Me.txtOverview.Text = "The entity and attribute information provided here describes the tabular data associated with the data set. Please review the detailed descriptions that are provided (the individual attribute descriptions) for information on the values that appear as fields/table entries of the data set." Then
            Me.txtOverview.ForeColor = Color.SlateGray
        End If
        If Me.txtEadetcit.Text = "The entity and attribute information was generated by the individual and/or agency identified as the originator of the data set. Please review the rest of the metadata record for additional details and information." Then
            Me.txtEadetcit.ForeColor = Color.SlateGray
        End If

        AddHandler lstFields.SelectedIndexChanged, AddressOf lstFields_SelectedIndexChanged
        AddHandler lstUniqueVals.SelectedIndexChanged, AddressOf lstUniqueVals_SelectedIndexChanged

        RecordChange()

    End Sub

    Private Function getWorkspaceType() As String

        If sExt.ToLower = ".shp" Then
            Return "shapefile"
        ElseIf sExt.ToLower = ".tif" Or _
            sExt.ToLower = ".img" Or _
            sExt.ToLower = ".bmp" Or _
            sExt.ToLower = ".gif" Or _
            sExt.ToLower = ".png" Or _
            sExt.ToLower = ".jpg" Then
            Return "raster"

        ElseIf File.Exists(Path.Combine(sPath, "hdr.adf")) Then
            Return "raster"
        ElseIf sWorkspacePath.Contains(".mdb") Then
            Return "personalGDB"
        ElseIf sWorkspacePath.Contains(".gdb") Then
            Return "fileGDB"
        ElseIf sExt.ToLower = ".dbf" Then
            Return "dbf"

        ElseIf isCoverage(sWorkspacePath, sFileName) Then
            Return "coverage"
        ElseIf isInfoTable(sWorkspacePath, sFileName) Then
            Return "arcTable"

            'ElseIf sPath.Contains("Connection") And sPath.Contains(".sde") Then
            '    Return "SDEdata"

        End If
        Return "Unhandled workspace type (function=getWorkspaceType())"
    End Function

    Private Function isInfoTable(ByVal workspacePath As String, ByVal name As String) As Boolean

        'Try
        '    Dim GP As GeoProcessor = New GeoProcessor()
        '    Dim ds As ESRI.ArcGIS.Geoprocessing.IGpEnumList
        '    GP.SetEnvironmentValue("workspace", sWorkspacePath)
        '    ds = GP.ListTables("*", "info")
        '    Dim fc As String = ds.Next
        '    Do While fc <> ""
        '        If fc = name Then Return True
        '        fc = ds.Next()
        '    Loop
        'Catch ex As Exception
        'End Try
        'Return False
        Try
            Dim covWorkspace As String = System.IO.Path.GetDirectoryName(workspacePath)
            Dim covName As String = System.IO.Path.GetFileNameWithoutExtension(workspacePath)
            Dim propertySet As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySetClass()
            ' path to coverage workspace directory        
            propertySet.SetProperty("DATABASE", workspacePath)
            'open the coverage workspace        
            Dim workspaceFactory As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesFile.ArcInfoWorkspaceFactoryClass()
            Dim workspace As IWorkspace = workspaceFactory.Open(propertySet, 0)
            Dim featureWorkspace As IArcInfoWorkspace = DirectCast(workspace, IFeatureWorkspace)

            Dim pFeatureWorkspace As IFeatureWorkspace
            pFeatureWorkspace = workspace

            Dim pTableTmp As ITable
            Dim pFieldsTmp As IFields
            pTableTmp = pFeatureWorkspace.OpenTable(sFileNameWithExtension)
            pFieldsTmp = getFields(pTableTmp)

            pTableTmp = Nothing
            pFieldsTmp = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function isCoverage(ByVal workspacePath As String, ByVal name As String) As Boolean
        'Try
        '    Dim covWorkspace As String = System.IO.Path.GetDirectoryName(sWorkspacePath)
        '    Dim covName As String = System.IO.Path.GetFileNameWithoutExtension(sWorkspacePath)
        '    Dim GP As GeoProcessor = New GeoProcessor()
        '    Dim ds As ESRI.ArcGIS.Geoprocessing.IGpEnumList
        '    GP.SetEnvironmentValue("workspace", covWorkspace)
        '    ds = GP.ListDatasets("*", "Coverage")
        '    Dim fc As String = ds.Next
        '    Do While fc <> ""
        '        If fc = covName Then Return True
        '        fc = ds.Next()
        '    Loop
        'Catch ex As Exception
        'End Try
        'Return False

        Try
            Dim covWorkspace As String = System.IO.Path.GetDirectoryName(workspacePath)
            Dim covName As String = System.IO.Path.GetFileNameWithoutExtension(workspacePath)

            Dim pFeatureClassTmp As IFeatureClass
            pFeatureClassTmp = ArcInfoWorkspaceFactory_Example(covWorkspace, covName, sFileNameWithExtension)
            Dim pCoverageFeatureClass As ICoverageFeatureClass
            pCoverageFeatureClass = pFeatureClass
            Dim pDatasetTmp As IDataset
            pDatasetTmp = pFeatureClass
            Dim pTableTmp As ITable
            pTableTmp = pDatasetTmp

            pFeatureClassTmp = Nothing
            pDatasetTmp = Nothing
            pTableTmp = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub loadFromGDB(ByVal pWorkspaceFactory As IWorkspaceFactory)

        Dim pWorkspace As IWorkspace
        Dim pFeatureWorkspace As IFeatureWorkspace

        'if this is geodatabase make sure we have the root gdb and not a feature dataset
        If pWorkspaceFactory.WorkspaceType = esriWorkspaceType.esriLocalDatabaseWorkspace And _
            (Not sWorkspacePath.EndsWith(".mdb") And Not sWorkspacePath.EndsWith(".gdb")) Then
            sWorkspacePath = System.IO.Path.GetDirectoryName(sWorkspacePath)
        End If

        pWorkspace = pWorkspaceFactory.OpenFromFile(sWorkspacePath, 0)
        pFeatureWorkspace = pWorkspace

        Dim c As Collection
        c = GetContents(pWorkspace)

        Dim pDSName As IDatasetName = Nothing
        For Each pDSName In c
            If pDSName.Name = sFileName Then Exit For
        Next

        If pDSName Is Nothing Then
            MsgBox("Unable to find pDSName in loadFromGDB sub" & vbCrLf & vbCrLf & "An unexpected error was encountered. Please report this to the application developers.")
            Me.Close()
        End If

        If TypeOf pDSName Is IFeatureClassName Then
            pFeatureClass = pFeatureWorkspace.OpenFeatureClass(sFileName)
            pDataset = pFeatureClass
            pTable = pDataset
            pFields = getFields(pTable)
        ElseIf TypeOf pDSName Is ITableName Then
            pTable = pFeatureWorkspace.OpenTable(sFileName)
            pFields = getFields(pTable)
        ElseIf TypeOf pDSName Is IRasterDatasetName Then
            pTable = openRasterTablefromGDB(sWorkspacePath, sFileName)
            pFields = getFields(pTable)
        Else
            dataType = "invalid"
        End If

        pMD = pFeatureClass
        If pMD Is Nothing Then
            pMD = pTable
        End If

    End Sub

    '#This is a preliminary attempt to get the Wizard to work against SDE data. This is still not working due to a problem with the 'TableSort' against the SDE table object.
    'Private Function loadSDEdata()
    '    Dim factoryType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory")
    '    Dim workspaceFactory As IWorkspaceFactory = CType(Activator.CreateInstance(factoryType), IWorkspaceFactory)
    '    Dim pWorkspace As IWorkspace
    '    Dim pFeatureWorkspace As IFeatureWorkspace
    '    sFileName = sFileName + sExt 'The earlier code splits on the '.' For the SDE datasets, we have to put the string pieces back together.

    '    Dim sdeConnectionFile As String
    '    sdeConnectionFile = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(sPath))

    '    sWorkspacePath = "C:\Users\dignizio\AppData\Roaming\ESRI\Desktop10.0\ArcCatalog\" + sdeConnectionFile
    '    pWorkspace = workspaceFactory.OpenFromFile(sWorkspacePath, 0)
    '    pFeatureWorkspace = pWorkspace

    '    Dim c As Collection
    '    c = GetContents(pWorkspace)

    '    Dim pDSName As IDatasetName = Nothing
    '    For Each pDSName In c
    '        If pDSName.Name = sFileName Then Exit For
    '    Next

    '    If pDSName.Type = "5" Then 'Type #5 is esriDTFeatureClass (see: http://resources.arcgis.com/en/help/arcobjects-net/componenthelp/index.html#//002500000042000000)
    '        pFeatureClass = pFeatureWorkspace.OpenFeatureClass(sFileName)
    '        pDataset = pFeatureClass
    '        pTable = pDataset
    '        pFields = getFields(pTable)
    '    ElseIf TypeOf pDSName Is ITableName Then
    '        pTable = pFeatureWorkspace.OpenTable(sFileName)
    '        pFields = getFields(pTable)
    '    ElseIf TypeOf pDSName Is IRasterDatasetName Then
    '        pTable = openRasterTablefromGDB(sWorkspacePath, sFileName)
    '        pFields = getFields(pTable)
    '    Else
    '        dataType = "invalid"
    '    End If

    '    pMD = pFeatureClass
    '    If pMD Is Nothing Then
    '        pMD = pTable
    '    End If

    'End Function


    Private Function openRasterTablefromGDB(ByVal filePath As String, ByVal name As String) As ITable
        Dim factoryType As Type = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory")
        Dim wsFactory As IWorkspaceFactory = CType(Activator.CreateInstance(factoryType), IWorkspaceFactory)
        Dim rasterWS As IRasterWorkspaceEx = CType(wsFactory.OpenFromFile(filePath, 0), IRasterWorkspaceEx)
        Dim rasterDS As IRasterDataset = rasterWS.OpenRasterDataset(name)
        Dim rasterBandCollection As IRasterBandCollection = CType(rasterDS, IRasterBandCollection)
        Dim rasterBand As IRasterBand = rasterBandCollection.Item(0)

        pTable = rasterBand.AttributeTable
        Return pTable
    End Function

    Private Sub loadRaster()
        Dim pWorkspaceFactory As IWorkspaceFactory
        Dim pWorkspace As IWorkspace
        Dim pRasterWorkspace As IRasterWorkspace

        pWorkspaceFactory = New RasterWorkspaceFactory
        pWorkspace = pWorkspaceFactory.OpenFromFile(sWorkspacePath, 0)
        pRasterWorkspace = pWorkspace
        pRasterDataSet = pRasterWorkspace.OpenRasterDataset(sFileName & sExt)
        pRasterBandCollection = pRasterDataSet

        'Count the number of Raster Bands
        Dim intRasterBandCount As Integer = pRasterBandCollection.Count
        Dim pRasterBand As IRasterBand = pRasterBandCollection.Item(0)
        pTable = pRasterBand.AttributeTable


        If intRasterBandCount > 1 Then
            dataType = "multiband"
        ElseIf pTable Is Nothing Then
            dataType = "singleband"
        Else
            dataType = "table"
            pFields = getFields(pTable)
        End If

    End Sub


    Private Sub loadCoverage()
        Dim covWorkspace As String = System.IO.Path.GetDirectoryName(sWorkspacePath)
        Dim covName As String = System.IO.Path.GetFileNameWithoutExtension(sWorkspacePath)

        pFeatureClass = ArcInfoWorkspaceFactory_Example(covWorkspace, covName, sFileNameWithExtension)
        Dim pCoverageFeatureClass As ICoverageFeatureClass
        pCoverageFeatureClass = pFeatureClass
        pDataset = pFeatureClass
        pTable = pDataset
        pFields = getFields(pTable)
        pMD = pCoverageFeatureClass
    End Sub

    Private Sub load_arcinfo_table()
        Dim covWorkspace As String = System.IO.Path.GetDirectoryName(sWorkspacePath)
        Dim covName As String = System.IO.Path.GetFileNameWithoutExtension(sWorkspacePath)
        Dim propertySet As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySetClass()
        ' path to coverage workspace directory        
        propertySet.SetProperty("DATABASE", sWorkspacePath)
        'open the coverage workspace        
        Dim workspaceFactory As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesFile.ArcInfoWorkspaceFactoryClass()
        Dim workspace As IWorkspace = workspaceFactory.Open(propertySet, 0)
        Dim featureWorkspace As IArcInfoWorkspace = DirectCast(workspace, IFeatureWorkspace)

        Dim pFeatureWorkspace As IFeatureWorkspace
        pFeatureWorkspace = workspace

        pTable = pFeatureWorkspace.OpenTable(sFileNameWithExtension)
        pFields = getFields(pTable)
        pMD = pTable
    End Sub

    Public Function ArcInfoWorkspaceFactory_Example(ByVal database As String, ByVal nameOfCoverage As String, ByVal nameOfFeature As String) As IFeatureClass
        'this function opens an ArcInfo coverage using a property set.        
        Dim propertySet As ESRI.ArcGIS.esriSystem.IPropertySet = New ESRI.ArcGIS.esriSystem.PropertySetClass()
        ' path to coverage workspace directory        
        propertySet.SetProperty("DATABASE", database)
        'open the coverage workspace        
        Dim workspaceFactory As IWorkspaceFactory = New ESRI.ArcGIS.DataSourcesFile.ArcInfoWorkspaceFactoryClass()
        Dim workspace As IWorkspace = workspaceFactory.Open(propertySet, 0)
        Dim featureWorkspace As IFeatureWorkspace = DirectCast(workspace, IFeatureWorkspace)
        Dim featureClass As IFeatureClass
        featureClass = featureWorkspace.OpenFeatureClass(nameOfCoverage & ":" & nameOfFeature)
        Return featureClass
    End Function

    Function GetContents(ByVal pWorkspace As IWorkspace) As Collection
        Dim c As New Collection
        Dim pEnumDSName As IEnumDatasetName
        pEnumDSName = pWorkspace.DatasetNames(esriDatasetType.esriDTAny)
        Dim pDSName As IDatasetName
        pDSName = pEnumDSName.Next
        Do Until pDSName Is Nothing
            If TypeOf pDSName Is IFeatureClassName Or _
                TypeOf pDSName Is ITableName Or _
                TypeOf pDSName Is IRasterDatasetName Then
                c.Add(pDSName)
            ElseIf TypeOf pDSName Is IFeatureDatasetName Then
                c.Add(pDSName)
                AddSubNames(pDSName, c)
            End If
            pDSName = pEnumDSName.Next
        Loop
        GetContents = c
    End Function

    Sub AddSubNames(ByVal pDSName1 As IDatasetName, ByVal c As Collection)
        Dim pEnumDSName As IEnumDatasetName
        pEnumDSName = pDSName1.SubsetNames
        pEnumDSName.Reset()
        Dim pDSName2 As IDatasetName
        pDSName2 = pEnumDSName.Next
        Do Until pDSName2 Is Nothing
            If TypeOf pDSName2 Is IFeatureClassName Then
                c.Add(pDSName2)
            End If
            pDSName2 = pEnumDSName.Next
        Loop
    End Sub

#End Region


    Private Function getFields(ByVal pTable As ITable) As IFields2
        Dim pFields As IFields2
        Try
            pFields = pTable.Fields
        Catch ex As Exception
            MsgBox("The Entity/Attribute builder tool only works on datasets with valid tables, attribute tables, VATs, etc.", MsgBoxStyle.Information)
            dataType = "invalid"
            Return Nothing
        End Try

        Return pFields
    End Function

    Private Sub InvalidDataType()
        For Each cntrl As Windows.Forms.Control In Me.Controls
            cntrl.Enabled = False
        Next
        Me.cmdCancel.Enabled = True

    End Sub


    Private Sub configureFixedForm()
        InvalidDataType()
        Me.splitContainer_Main.Enabled = True
        Me.splitContainer_Main.Panel1.Enabled = True
        Me.frmDomain.Enabled = False
        Me.txtOverview.Enabled = True
        Me.txtEadetcit.Enabled = True
        Me.labelVATwarning.ForeColor = Color.CornflowerBlue
        txtAttDef.Enabled = True
        txtAttDefSource.Enabled = True
        cmdSaveAndClose.Enabled = True
        cmdSave.Enabled = True

        optRange.Enabled = True
        RemoveHandler optRange.CheckedChanged, AddressOf optRange_CheckedChanged
        optRange.Checked = True
        AddHandler optRange.CheckedChanged, AddressOf optRange_CheckedChanged

        txtRngMin.Enabled = True
        txtRngMax.Enabled = True
        txtRngUnits.Enabled = True
        txtRngResolution.Enabled = True

    End Sub

    Private Sub load_strEA(ByVal pFields As IFields2)
        Dim strDataType As String
        Dim n As Integer
        n = 0

        Dim intDataType As String
        curEA = New DetailedDescription
        'ReDim strEA(0)
        For i As Integer = 0 To pFields.FieldCount - 1
            pField = pFields.Field(i)
            'Debug.Print(pField.Name)
            'Populate the FlexGrid with the attribute name and the attribute type
            intDataType = pField.Type
            'Convert the Datatype to a string using the DataTypeConvert Sub
            ' Used to show text rather than integers in the ListView
            strDataType = DataTypeConvert(intDataType)

            'Do not populate the ListView with the FID or OID
            ' no sense in doing Statistics for these fields
            Dim index As Integer = 0
            If Not (pField.Type = esriFieldType.esriFieldTypeOID Or pField.Type = esriFieldType.esriFieldTypeGeometry) Then
                Dim curAtt As New attr
                curAtt.attrlabl = pField.Name
                curAtt.attrdef = existingXMLEntry("attdef", pField.Name)
                curAtt.attrdefs = existingXMLEntry("attdefs", pField.Name)

                If pField.Type = esriFieldType.esriFieldTypeSmallInteger Or _
                    pField.Type = esriFieldType.esriFieldTypeInteger Or _
                    pField.Type = esriFieldType.esriFieldTypeSingle Or _
                    pField.Type = esriFieldType.esriFieldTypeInteger Or _
                    pField.Type = esriFieldType.esriFieldTypeDouble Then
                    curAtt.c_attribdomv = NumericField(intDataType, n)
                ElseIf pField.Type = esriFieldType.esriFieldTypeString Then
                    curAtt.c_attribdomv = CharacterField(intDataType, n)
                ElseIf pField.Type = esriFieldType.esriFieldTypeDate Then
                    curAtt.c_attribdomv = NumericField(intDataType, n)
                ElseIf pField.Type = esriFieldType.esriFieldTypeGUID Or pField.Type = esriFieldType.esriFieldTypeGlobalID Then
                    'its a guid
                    Dim unrep As New udom
                    unrep.udom = "Globally Unique Identifier or GUID "

                    Dim attributeDomainValue As New attribdomv
                    ReDim attributeDomainValue.attributeDomainValues(0)
                    attributeDomainValue.attributeDomainValues(0) = unrep
                    curAtt.c_attribdomv = attributeDomainValue

                Else
                    'Unhandled data type
                    Dim unrep As New udom
                    unrep.udom = "Unknown Data Type"
                    Dim attributeDomainValue As New attribdomv
                    ReDim attributeDomainValue.attributeDomainValues(0)
                    attributeDomainValue.attributeDomainValues(0) = unrep
                    curAtt.c_attribdomv = attributeDomainValue
                End If

                curEA.attributesDict(pField.Name) = curAtt
                Dim item As Windows.Forms.ListViewItem
                item = lstFields.Items.Add(pField.Name)
                item.SubItems.Add(strDataType)
                n = n + 1
            End If
        Next
    End Sub

    Function existingXMLEntry(ByVal strType As String, Optional ByVal strLbl As String = Nothing, Optional ByVal strEdomv As String = Nothing)
        Dim xpath As String = "Not set"
        Dim xpathTry1 As String = "Not set"
        Dim xpathTry2 As String = "Not set"

        Dim ans As String
        Dim strDefault As String = "Unknown"

        Select Case strType
            'We try querying for case-sensitivity. This should handle issues associated with files created in/exported from geodatabases in ESRI.

            Case "attdef"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdef"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdef"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdef"

            Case "attdefs"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdefs"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdefs"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdefs"
                strDefault = "Producer defined"

            Case "edomvd"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/edom[edomv='" & strEdomv & "']/edomvd"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/edom[edomv='" & strEdomv & "']/edomvd"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/edom[edomv='" & strEdomv & "']/edomvd"

            Case "edomvds"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/edomvds"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/edomvds"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/edomvds"
                strDefault = "Producer defined"

            Case "udom"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/udom"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/udom"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/udom"

            Case "eaover"
                xpath = "eainfo/overview/eaover"
                strDefault = "The entity and attribute information provided here describes the tabular data associated with the data set. Please review the detailed descriptions that are provided (the individual attribute descriptions) for information on the values that appear as fields/table entries of the data set."

            Case "eadetcit"
                xpath = "eainfo/overview/eadetcit"
                strDefault = "The entity and attribute information was generated by the individual and/or agency identified as the originator of the data set. Please review the rest of the metadata record for additional details and information."

            Case "codesetn"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/codesetd/codesetn"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/codesetd/codesetn"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/codesetd/codesetn"

            Case "codesets"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/codesetd/codesets"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/codesetd/codesets"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/codesetd/codesets"
                strDefault = "Provide Codeset Definition Reference (Citatation/URL)"

            Case "attrunit"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/rdom/attrunit"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/rdom/attrunit"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/rdom/attrunit"

            Case "attrmres"
                xpath = "eainfo/detailed/attr[attrlabl='" & strLbl & "']/attrdomv/rdom/attrmres"
                xpathTry1 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToLower & "']/attrdomv/rdom/attrmres"
                xpathTry2 = "eainfo/detailed/attr[attrlabl='" & strLbl.ToUpper & "']/attrdomv/rdom/attrmres"
            Case Else
                xpath = "Unhandled case in function=existingXMLEntry"
                xpathTry1 = "Unhandled case in function=existingXMLEntry"
                xpathTry2 = "Unhandled case in function=existingXMLEntry"
        End Select

        Try
            'Debug.Print("XPath:" & xpath)
            ans = pXML.GetXml(xpath)
            'Debug.Print("Value:" & ans)

            If (CStr(strType) <> "eaover" And CStr(strType) <> "eadetcit") Then
                If ans = Nothing Then
                    Try
                        ans = pXML.GetXml(xpathTry1)
                    Catch ex As Exception

                    End Try
                End If
            End If

            If (CStr(strType) <> "eaover" And CStr(strType) <> "eadetcit") Then
                If ans = Nothing Then
                    Try
                        ans = pXML.GetXml(xpathTry2)
                    Catch ex As Exception
                    End Try
                End If
            End If
            If ans Is Nothing Then
                ans = strDefault
            Else
                ans = Mid(ans, ans.IndexOf(">") + 2)
                ans = Mid(ans, 1, InStrRev(ans, "<") - 1)
            End If

        Catch ex As Exception
            ans = strDefault
        End Try

        Return ans

    End Function
    Function DataTypeConvert(ByRef intDataType As Integer) As String
        On Error GoTo ErrorHandler

        'Given the integer, change to string
        Select Case intDataType
            Case 0
                DataTypeConvert = "Integer"
            Case 1
                DataTypeConvert = "Long Integer"
            Case 2
                DataTypeConvert = "Single-precision floating-point"
            Case 3
                DataTypeConvert = "Double-precision floating-point"
            Case 4
                DataTypeConvert = "Character string"
            Case 5
                DataTypeConvert = "Date"
            Case 6
                DataTypeConvert = "Long Integer representing an object identifier"
            Case 7
                DataTypeConvert = "Geometry"
            Case 8
                DataTypeConvert = "Binary Large Object"
            Case 11
                DataTypeConvert = "Global Unique Identifier field"
            Case Else
                DataTypeConvert = "Unknown field Type"
        End Select

        Exit Function 'avoid error handler

ErrorHandler:  'error handling routine
        MsgBox("DataTypeConvert " & Err.Description, vbCritical)
    End Function

    Sub loadSingleBand()
        Dim pRasterBand As IRasterBand = pRasterBandCollection.Item(0)

        If pRasterBand.Statistics Is Nothing Then
            pRasterBand.RasterDataset.PrecalculateStats(0)
        End If

        curEA = New DetailedDescription
        Dim curAtt As New attr
        curAtt.attrlabl = "Value"
        curAtt.attrdef = existingXMLEntry("attdef", "Value")
        curAtt.attrdefs = existingXMLEntry("attdefs", "Value")

        Dim m_rdom As New rdom

        m_rdom.rdommin = CStr(pRasterBand.Statistics.Minimum)
        m_rdom.rdommax = CStr(pRasterBand.Statistics.Maximum)

        m_rdom.attrunit = existingXMLEntry("attrunit", "Value")
        m_rdom.attrmes = existingXMLEntry("attrmres", "Value")

        Dim attributeDomainValue As New attribdomv
        ReDim attributeDomainValue.attributeDomainValues(0)
        attributeDomainValue.attributeDomainValues(0) = m_rdom
        curAtt.c_attribdomv = attributeDomainValue
        curEA.attributesDict("Value") = curAtt

        RemoveHandler txtRngMax.TextChanged, AddressOf txtRngMax_TextChanged
        RemoveHandler txtRngMin.TextChanged, AddressOf txtRngMin_TextChanged
        txtRngMax.Text = CStr(pRasterBand.Statistics.Maximum)
        txtRngMin.Text = CStr(pRasterBand.Statistics.Minimum)
        AddHandler txtRngMax.TextChanged, AddressOf txtRngMax_TextChanged
        AddHandler txtRngMin.TextChanged, AddressOf txtRngMin_TextChanged

        Dim item As Windows.Forms.ListViewItem
        item = lstFields.Items.Add("Value")
        item.SubItems.Add("Continuous raster value")

        configureFixedForm()

    End Sub

    Sub loadMultiBand()

        'Count the number of Raster Bands
        Dim intRasterBandCount As Integer = pRasterBandCollection.Count
        curEA = New DetailedDescription

        Dim currBand As Integer = 0
        For Each band As Integer In Enumerable.Range(0, intRasterBandCount)

            Dim pRasterBand As IRasterBand = pRasterBandCollection.Item(currBand)

            If pRasterBand.Statistics Is Nothing Then
                pRasterBand.RasterDataset.PrecalculateStats(currBand)
            End If

            Dim curAtt As New attr
            Dim curValue As String = ("Band_" & CStr(currBand + 1))

            curAtt.attrlabl = curValue
            curAtt.attrdef = existingXMLEntry("attdef", curValue)
            curAtt.attrdefs = existingXMLEntry("attdefs", curValue)

            Dim m_rdom As New rdom

            m_rdom.rdommin = CStr(pRasterBand.Statistics.Minimum)
            m_rdom.rdommax = CStr(pRasterBand.Statistics.Maximum)

            m_rdom.attrunit = existingXMLEntry("attrunit", curValue)
            m_rdom.attrmes = existingXMLEntry("attrmres", curValue)

            Dim attributeDomainValue As New attribdomv
            ReDim attributeDomainValue.attributeDomainValues(0)
            attributeDomainValue.attributeDomainValues(0) = m_rdom
            curAtt.c_attribdomv = attributeDomainValue
            curEA.attributesDict(curValue) = curAtt

            RemoveHandler txtRngMax.TextChanged, AddressOf txtRngMax_TextChanged
            RemoveHandler txtRngMin.TextChanged, AddressOf txtRngMin_TextChanged
            txtRngMax.Text = CStr(pRasterBand.Statistics.Maximum)
            txtRngMin.Text = CStr(pRasterBand.Statistics.Minimum)
            AddHandler txtRngMax.TextChanged, AddressOf txtRngMax_TextChanged
            AddHandler txtRngMin.TextChanged, AddressOf txtRngMin_TextChanged

            Dim item As Windows.Forms.ListViewItem
            item = lstFields.Items.Add(curValue)
            item.SubItems.Add("Continuous raster value")

            currBand += 1
        Next

        configureFixedForm()


    End Sub


    Function NumericField(ByRef intEsriDataType As Integer, ByVal intIndex As Integer) As Object

        Dim pStats As IStatisticsResults = Nothing
        'Dim intMean As Double
        Dim strMin As String
        Dim strMax As String
        'Dim intSTDV As Double
        Dim sFieldName As String
        sFieldName = pField.Name
        'strFieldComplete = ""

        pTableSort = New TableSort

        With pTableSort
            .Fields = sFieldName
            .Ascending(sFieldName) = True
            .CaseSensitive(sFieldName) = False
            .QueryFilter = Nothing
            If intType = 10 Then
                .Table = pTable
            ElseIf intType = 12 Then
                .Table = pTable
            ElseIf intType = 5 Then
                .Table = pTable
            ElseIf intType = 0 Then
                .Table = pTable
            Else
                .Table = pFeatureClass
            End If
        End With
        pTableSort.Sort(Nothing)
        Dim pDataStatistics As IDataStatistics

        Dim pCursor As ICursor
        Dim pFeatureCursor As IFeatureCursor

        If intType = 10 Or intType = 12 Or intType = 5 Or intType = 0 Then
            pCursor = pTableSort.Rows
            pDataStatistics = New DataStatistics

            pDataStatistics.Field = sFieldName
            pDataStatistics.Cursor = pCursor
        Else
            pFeatureCursor = pTableSort.Rows
            pDataStatistics = New DataStatistics

            pDataStatistics.Field = sFieldName
            pDataStatistics.Cursor = pFeatureCursor

        End If

        'Getting the stats seems to fail sometimes but work others... This is admittedly strange. Anyway, we'll try 10 times.
        Dim trys As Integer = 0
        Do While trys < 10
            Try
                pStats = pDataStatistics.Statistics
            Catch x As Exception
                trys += 1
            End Try
            If Not pStats Is Nothing Then
                trys = 10
            End If
        Loop
        If pStats Is Nothing Then
            MsgBox("A problem was encountered while retrieving the statistics from the dataset's table.")
        End If


        If pField.Type = esriFieldType.esriFieldTypeDate Then
            Try
                strMin = CDate(Date.FromOADate(pStats.Minimum))
            Catch ex As Exception
                strMin = "{Null Value / Empty Field Entry}"
            End Try
            Try
                strMax = CDate(Date.FromOADate(pStats.Maximum))
            Catch ex As Exception
                strMax = "{Null Value / Empty Field Entry}"
            End Try

        Else
            strMin = pStats.Minimum
            strMax = pStats.Maximum
        End If

        If pStats.Count = 0 Then
            strMin = "{Null Value / Empty Field Entry}"
            strMax = "{Null Value / Empty Field Entry}"
        End If

        Dim m_rdom As New rdom
        m_rdom.rdommin = strMin
        m_rdom.rdommax = strMax


        m_rdom.attrunit = existingXMLEntry("attrunit", sFieldName)
        m_rdom.attrmes = existingXMLEntry("attrmres", sFieldName)


        Dim attributeDomainValue As New attribdomv
        ReDim attributeDomainValue.attributeDomainValues(0)
        attributeDomainValue.attributeDomainValues(0) = m_rdom
        Return attributeDomainValue


        Exit Function

    End Function

    Function CharacterField(ByRef intEsriDataType As Integer, ByVal intIndex As Integer) As Object
        On Error GoTo ErrorHandler

        Dim sFieldName As String
        sFieldName = pField.Name
        'strFieldComplete = ""

        pTableSort = New TableSort
        With pTableSort
            .Fields = sFieldName
            .Ascending(sFieldName) = True
            .CaseSensitive(sFieldName) = False
            .QueryFilter = Nothing
            If intType = 10 Or intType = 12 Or intType = 0 Then
                .Table = pTable
            Else
                .Table = pFeatureClass
            End If

        End With
        pTableSort.Sort(Nothing)

        Dim pDataStatistics As IDataStatistics

        Dim pCursor As ICursor
        Dim pFeatureCursor As IFeatureCursor
        If intType = 10 Or intType = 12 Or intType = 0 Then
            pCursor = pTableSort.Rows
            pDataStatistics = New DataStatistics

            pDataStatistics.Field = sFieldName
            pDataStatistics.Cursor = pCursor
        Else
            pFeatureCursor = pTableSort.Rows
            pDataStatistics = New DataStatistics

            pDataStatistics.Field = sFieldName
            pDataStatistics.Cursor = pFeatureCursor

        End If

        Dim pEnumVar As System.Collections.IEnumerator
        Dim strUniqueCharacter As String
        Dim value As Object

        Dim strAttribute As String

        'Check to see if strAttribute exists, bail if it does not
        Dim strEaInfo As String
        Dim strDetailed As String
        Dim strAttr As String
        Dim n As Integer

        'DI: This line seems to be a bottleneck....
        pEnumVar = pDataStatistics.UniqueValues

        Dim strValue As String
        Dim varVal

        Dim lngUnique As Long = 0

        Dim m_domainvals As New attribdomv
        ReDim m_domainvals.attributeDomainValues(0)
        'Dim rowCount As Integer = 0
        While pEnumVar.MoveNext
            If lngUnique > 20 Then
                'Too many unique values default to an unrep
                Dim unrep As New udom
                unrep.udom = existingXMLEntry("udom", sFieldName)

                Dim attributeDomainValue As New attribdomv
                ReDim attributeDomainValue.attributeDomainValues(0)
                attributeDomainValue.attributeDomainValues(0) = unrep
                Return attributeDomainValue
            Else

                Dim curEdom As New edom
                If pEnumVar.Current = "" Or pEnumVar.Current = " " Then
                    curEdom.edomv = "{Null Value / Empty Field Entry}"
                Else
                    curEdom.edomv = pEnumVar.Current
                End If

                curEdom.edomvd = existingXMLEntry("edomvd", sFieldName, pEnumVar.Current)
                curEdom.edomvds = existingXMLEntry("edomvds", sFieldName, pEnumVar.Current)
                m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues)) = curEdom
                ReDim Preserve m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues) + 1)
                'If rowCount > 1000 Then
                '    'DI: Default to UNREP if there are many features... speed up tool initialization
                '    Dim unrep As New udom
                '    unrep.udom = existingXMLEntry("udom", sFieldName)

                '    Dim attributeDomainValue As New attribdomv
                '    ReDim attributeDomainValue.attributeDomainValues(0)
                '    attributeDomainValue.attributeDomainValues(0) = unrep
                '    Return attributeDomainValue
                '    Exit While
                'End If
            End If
            lngUnique += 1
            'rowCount += 1
        End While

        If UBound(m_domainvals.attributeDomainValues) = 0 Then
            'There were no values to iterate over. (This probably means that all values were null.)
            Dim unrep As New udom
            unrep.udom = existingXMLEntry("udom", sFieldName)

            Dim attributeDomainValue As New attribdomv
            ReDim attributeDomainValue.attributeDomainValues(0)
            attributeDomainValue.attributeDomainValues(0) = unrep
            Return attributeDomainValue
        Else
            ReDim Preserve m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues) - 1)
        End If

        Return m_domainvals

        Exit Function 'avoid error handler
ErrorHandler:  'error handling routine
        MsgBox("CharacterField  " & Err.Description, vbCritical)
    End Function

    Private Sub RecordChange()

        RemoveHandler lstUniqueVals.SelectedIndexChanged, AddressOf lstUniqueVals_SelectedIndexChanged
        RemoveHandler optEnum.CheckedChanged, AddressOf optRange_CheckedChanged
        RemoveHandler optRange.CheckedChanged, AddressOf optRange_CheckedChanged
        RemoveHandler optCodeset.CheckedChanged, AddressOf optRange_CheckedChanged
        RemoveHandler optUnrep.CheckedChanged, AddressOf optRange_CheckedChanged


        Dim curFieldKey As String
        If Me.lstFields.SelectedItems.Count = 0 Then
            Exit Sub
        Else
            curFieldKey = Me.lstFields.SelectedItems(0).Text
        End If


        Me.txtAttDef.Text = curEA.attributesDict(curFieldKey).attrdef
        Me.txtAttDefSource.Text = curEA.attributesDict(curFieldKey).attrdefs


        Dim curType As String
        curType = returnType(curFieldKey)

        Dim curDomain As domain
        'DI: Updated with Try/Catch here to handle 'all fields empty' scenario in a column.
        Try
            curDomain = curEA.attributesDict(curFieldKey).c_attribdomv.attributeDomainValues(0)
        Catch ex As Exception
            curDomain = Nothing
        End Try

        If curType = "udom" Then
            Me.optUnrep.Checked = True
            Dim curUnrep As udom
            curUnrep = curDomain
            Me.txtUnrep.Text = curUnrep.udom
        ElseIf curType = "rdom" Then
            Me.optRange.Checked = True
            Dim curRange As rdom
            curRange = curDomain
            Me.optRange.Checked = True

            Me.txtRngMin.Text = curRange.rdommin
            Me.txtRngMax.Text = curRange.rdommax
            Me.txtRngResolution.Text = curRange.attrmes
            Me.txtRngUnits.Text = curRange.attrunit
        ElseIf curType = "edom" Then
            Me.optEnum.Checked = True
            Dim j As Integer
            Me.lstUniqueVals.Items.Clear()
            For j = 0 To curEA.attributesDict(curFieldKey).c_attribdomv.attributeDomainValues.Length - 1
                Dim curEdom As edom
                curEdom = curEA.attributesDict(curFieldKey).c_attribdomv.attributeDomainValues(j)
                Me.lstUniqueVals.Items.Add(curEdom.edomv)
            Next
            Me.lstUniqueVals.SelectedIndex = 0
            Call lstUniqueVals_AfterUpdate()
        ElseIf curType = "codesetd" Then
            Me.optCodeset.Checked = True
            Dim curCodeset As codesetd
            curCodeset = curDomain
            Me.cboCodesetName.Text = curCodeset.codesetn
            Me.txtCodesetSource.Text = curCodeset.codesets

        Else
            'DI: If we can't match a field type, set to Unrepresentable.
            Me.optUnrep.Checked = True
            curType = "udom"

        End If

        changePanel(curType)


        AddHandler lstUniqueVals.SelectedIndexChanged, AddressOf lstUniqueVals_SelectedIndexChanged
        AddHandler optEnum.CheckedChanged, AddressOf optRange_CheckedChanged
        AddHandler optRange.CheckedChanged, AddressOf optRange_CheckedChanged
        AddHandler optCodeset.CheckedChanged, AddressOf optRange_CheckedChanged
        AddHandler optUnrep.CheckedChanged, AddressOf optRange_CheckedChanged
    End Sub

    Private Function returnType(ByVal curField As String) As String

        Dim dmn As domain
        'DI: Updated with Try/Catch here to handle 'all fields empty' scenario in a column.
        Try
            dmn = curEA.attributesDict(curField).c_attribdomv.attributeDomainValues(0)

            Dim t As Type = curEA.attributesDict(curField).c_attribdomv.attributeDomainValues(0).GetType
            Return t.Name

        Catch ex As Exception
            Return Nothing

        End Try

    End Function

    Private Sub ChangeDomain()
        'Changes the current fields attribute type to correspond to the new option check
        Dim pRow As IRow
        Dim pCursor As ICursor
        Dim intField As Integer

        Dim sFieldName As String

        sFieldName = Me.lstFields.SelectedItems(0).Text

        intField = pFields.FindField(sFieldName)
        pField = pFields.Field(intField)

        ReDim curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)

        If Me.optUnrep.Checked = True Then
            Dim curUnrep As New udom
            curUnrep.udom = existingXMLEntry("udom", sFieldName)
            curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0) = curUnrep

        ElseIf Me.optRange.Checked = True Then

            Dim strMin As String
            Dim strMax As String

            pTableSort = New TableSort

            With pTableSort
                .Fields = sFieldName
                .Ascending(sFieldName) = True
                .CaseSensitive(sFieldName) = False
                .QueryFilter = Nothing
                If intType = 10 Then
                    .Table = pTable
                ElseIf intType = 12 Then
                    .Table = pTable
                ElseIf intType = 5 Then
                    .Table = pTable
                ElseIf intType = 0 Then
                    .Table = pTable
                Else
                    .Table = pFeatureClass
                End If
            End With


            pTableSort.Sort(Nothing)
            pCursor = pTableSort.Rows
            pRow = pCursor.NextRow
            'strMin = pRow.Value(intField)

            If pRow Is Nothing Then
                strMin = "{Null Value / Empty Field Entry}"
            ElseIf pRow.Value(intField) Is DBNull.Value Then
                strMin = "{Null Value / Empty Field Entry}"
            ElseIf CStr(pRow.Value(intField)) = "" Then
                strMin = "{Null Value / Empty Field Entry}"
            Else
                strMin = pRow.Value(intField)
            End If


            pTableSort.Ascending(sFieldName) = False
            pTableSort.Sort(Nothing)
            pCursor = pTableSort.Rows
            pRow = pCursor.NextRow
            'strMax = pRow.Value(intField)

            If pRow Is Nothing Then
                strMax = "{Null Value / Empty Field Entry}"
            ElseIf pRow.Value(intField) Is DBNull.Value Then
                strMax = "{Null Value / Empty Field Entry}"
            ElseIf CStr(pRow.Value(intField)) = "" Then
                strMax = "{Null Value / Empty Field Entry}"
            Else
                strMax = pRow.Value(intField)
            End If


            Dim m_rdom As New rdom
            m_rdom.rdommin = strMin
            m_rdom.rdommax = strMax
            m_rdom.attrunit = existingXMLEntry("attrunit", sFieldName)
            m_rdom.attrmes = existingXMLEntry("attrmres", sFieldName)
            curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0) = m_rdom


        ElseIf Me.optEnum.Checked = True Then

            Dim intDType As Integer
            Me.lstUniqueVals.Items.Clear()
            intDType = pField.Type
            Dim strDataType As String
            strDataType = DataTypeConvert(intDType)

            pTableSort = New TableSort

            With pTableSort
                .Fields = sFieldName 'Me.lstFields.value
                .Ascending(sFieldName) = True
                .CaseSensitive(sFieldName) = False
                .QueryFilter = Nothing
                If intType = 10 Then
                    .Table = pTable
                ElseIf intType = 12 Then
                    .Table = pTable
                ElseIf intType = 5 Then
                    .Table = pTable
                ElseIf intType = 0 Then
                    .Table = pTable
                Else
                    .Table = pFeatureClass
                End If
            End With
            pTableSort.Sort(Nothing)
            pCursor = pTableSort.Rows

            Dim m_domainvals As New attribdomv
            ReDim m_domainvals.attributeDomainValues(0)

            Dim curVal As String
            pRow = pCursor.NextRow
            curVal = Nothing
            Dim curRowVal As String
            Do Until pRow Is Nothing
                If pRow.Value(intField) Is DBNull.Value Then
                    curRowVal = "{Null Value / Empty Field Entry}"
                ElseIf CStr(pRow.Value(intField)) = "" Then
                    curRowVal = "{Null Value / Empty Field Entry}"
                Else
                    curRowVal = pRow.Value(intField)
                End If
                If curVal <> curRowVal Then
                    curVal = curRowVal
                    Dim curEdom As New edom
                    curEdom.edomv = curRowVal

                    curEdom.edomvd = existingXMLEntry("edomvd", sFieldName, curVal)
                    curEdom.edomvds = existingXMLEntry("edomvds", sFieldName, curVal)
                    m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues)) = curEdom
                    ReDim Preserve m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues) + 1)
                End If
                pRow = pCursor.NextRow
            Loop

            ReDim Preserve m_domainvals.attributeDomainValues(UBound(m_domainvals.attributeDomainValues) - 1)
            curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv = m_domainvals

        ElseIf Me.optCodeset.Checked Then
            Dim curCodeset As New codesetd
            curCodeset.codesetn = existingXMLEntry("codesetn", sFieldName)
            curCodeset.codesets = existingXMLEntry("codesets", sFieldName)
            curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0) = curCodeset
        End If

        Call RecordChange()
    End Sub

    Private Function returnFormType() As Integer
        If Me.optEnum.Checked = True Then
            returnFormType = 1
        ElseIf Me.optRange.Checked = True Then
            returnFormType = 2
        ElseIf Me.optCodeset.Checked = True Then
            returnFormType = 3
        ElseIf Me.optUnrep.Checked = True Then
            returnFormType = 4
        End If

    End Function


    Private Sub changePanel(ByVal tag As String)
        pnlEnumerated.Hide()
        pnlCodeset.Hide()
        pnlRange.Hide()
        pnlUnrepresentable.Hide()

        If tag = "edom" Then pnlEnumerated.Show()
        If tag = "codesetd" Then pnlCodeset.Show()
        If tag = "rdom" Then pnlRange.Show()
        If tag = "udom" Then pnlUnrepresentable.Show()
    End Sub


    Private Sub lstUniqueVals_AfterUpdate()
        Dim curEdom As edom
        curEdom = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(Me.lstUniqueVals.SelectedIndex)

        Me.txtValDef.Text = curEdom.edomvd
        Me.txtValDefSource.Text = curEdom.edomvds
    End Sub


    Private Sub frmGenEA_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        sPath = My.Application.CommandLineArgs(0)
        sOutFile = My.Application.CommandLineArgs(1)

        load_dataset()
        'RecordChange()
    End Sub


#Region "Form Events"
    Private Sub optRange_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optEnum.CheckedChanged, optUnrep.CheckedChanged, optRange.CheckedChanged, optCodeset.CheckedChanged
        If optRange.Checked Then changePanel("rdom")
        If optEnum.Checked Then changePanel("edom")
        If optCodeset.Checked Then changePanel("codesetd")
        If optUnrep.Checked Then changePanel("udom")

        ChangeDomain()
    End Sub


    Private Sub lstFields_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RecordChange()
    End Sub

    Private Sub lstUniqueVals_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim curEdom As edom
        curEdom = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(Me.lstUniqueVals.SelectedIndex)

        Me.txtValDef.Text = curEdom.edomvd
        Me.txtValDefSource.Text = curEdom.edomvds
    End Sub


#End Region


    Private Sub txtValDef_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtValDef.TextChanged
        Dim curEdom As edom
        curEdom = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(Me.lstUniqueVals.SelectedIndex)

        curEdom.edomvd = Me.txtValDef.Text

    End Sub
    Private Sub txtValDefSource_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtValDefSource.TextChanged
        Dim curEdom As edom
        curEdom = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(Me.lstUniqueVals.SelectedIndex)

        curEdom.edomvds = Me.txtValDefSource.Text

    End Sub


    Private Sub txtAttDefSource_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAttDefSource.TextChanged
        curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).attrdefs = Me.txtAttDefSource.Text
    End Sub
    Private Sub txtAttDef_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtAttDef.TextChanged
        curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).attrdef = Me.txtAttDef.Text
    End Sub


    Private Sub txtRngMin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRngMin.TextChanged

        Dim curRange As rdom
        curRange = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curRange.rdommin = Me.txtRngMin.Text

    End Sub

    Private Sub txtRngMax_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRngMax.TextChanged

        Dim curRange As rdom
        curRange = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curRange.rdommax = Me.txtRngMax.Text

    End Sub

    Private Sub txtRngUnits_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRngUnits.TextChanged

        Dim curRange As rdom
        curRange = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curRange.attrunit = Me.txtRngUnits.Text

    End Sub

    Private Sub txtRngResolution_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRngResolution.TextChanged

        Dim curRange As rdom
        curRange = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curRange.attrmes = Me.txtRngResolution.Text

    End Sub

    'Private Sub txtCodeset_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim curCodeset As codesetd
    '    curCodeset = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
    '    curCodeset.codesetn = cboCodesetName.Text
    '    curCodeset.codesets = txtCodesetSource.Text

    'End Sub

    Private Sub txtUnrep_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtUnrep.TextChanged
        Dim curUnrep As udom
        curUnrep = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curUnrep.udom = Me.txtUnrep.Text

    End Sub


    Private Sub saveEA()
        pXML.SetPropertyX("eainfo/detailed/enttyp/enttypl", curEA.ET.EntityTypeLabel, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
        pXML.SetPropertyX("eainfo/detailed/enttyp/enttypd", curEA.ET.entityTypeDescription, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
        pXML.SetPropertyX("eainfo/detailed/enttyp/enttypds", curEA.ET.entityTypeDefinitionSource, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)

        'Delete the previous Attributes
        While pXML.CountX("eainfo/detailed/attr")
            pXML.DeleteProperty("eainfo/detailed/attr")
        End While

        Dim curType As String
        Dim i As Integer = 0
        For Each kvp As KeyValuePair(Of String, attr) In curEA.attributesDict
            Dim att As attr = kvp.Value
            pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrlabl", att.attrlabl, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddIfNotExists, False)
            pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdef", att.attrdef, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddIfNotExists, False)
            pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdefs", att.attrdefs, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddIfNotExists, False)


            curType = returnType(att.attrlabl)
            If curType = "udom" Then
                Dim curUdom As udom
                curUdom = att.c_attribdomv.attributeDomainValues(0)
                pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/udom", curUdom.udom, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
            ElseIf curType = "rdom" Then
                Dim curRdom As rdom
                curRdom = att.c_attribdomv.attributeDomainValues(0)
                pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/rdom/rdommin", curRdom.rdommin, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/rdom/rdommax", curRdom.rdommax, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                If curRdom.attrmes <> "Unknown" Then
                    pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/rdom/attrmes", curRdom.attrmes, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                End If
                If curRdom.attrunit <> "Unknown" Then
                    pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/rdom/attrunit", curRdom.attrunit, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                End If
            ElseIf curType = "codesetd" Then
                Dim curCodeset As codesetd
                curCodeset = att.c_attribdomv.attributeDomainValues(0)
                pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/codesetd/codesetn", curCodeset.codesetn, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv/codesetd/codesets", curCodeset.codesets, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
            ElseIf curType = "edom" Then
                'The hard one
                Dim j As Integer
                For j = 0 To curEA.attributesDict(att.attrlabl).c_attribdomv.attributeDomainValues.Length - 1

                    Dim curEdom As edom
                    curEdom = curEA.attributesDict(att.attrlabl).c_attribdomv.attributeDomainValues(j)
                    pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv[" & Str(j) & "]/edom/edomv", curEdom.edomv, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                    pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv[" & Str(j) & "]/edom/edomvd", curEdom.edomvd, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                    pXML.SetPropertyX("eainfo/detailed/attr[" & Str(i) & "]/attrdomv[" & Str(j) & "]/edom/edomvds", curEdom.edomvds, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
                Next
            End If
            i += 1
        Next


        If txtOverview.Text <> "Unknown" Then
            pXML.SetPropertyX("eainfo/overview/eaover", txtOverview.Text, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)
            pXML.SetPropertyX("eainfo/overview/eadetcit", txtEadetcit.Text, esriXmlPropertyType.esriXPTText, esriXmlSetPropertyAction.esriXSPAAddOrReplace, False)

        End If

        Dim file As System.IO.FileStream
        file = System.IO.File.Create(sOutFile)
        file.Close()

        Dim outEAInfo As String = pXML.GetXml("eainfo")
        Dim objWriter As New System.IO.StreamWriter(sOutFile)
        objWriter.Write(outEAInfo)
        objWriter.Close()

        Try
            pMD.Metadata = pXML
        Catch ex As Exception
            'DI Update 6/9/15. Not doing anything for the exception but the tool was bombing here on Raster datasets in a File GDB, which this should catch.
            'Re-defining pMD.Metdata = pXML should not be necessary since we only load the form the first time. No updates are 
            'being made to the actual dataset, we're simply saving the form's contents out to a .txt file for use by the Metadata Wizard.
        End Try

    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        saveEA()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.Dispose()
    End Sub

    Private Sub cmdSaveAndClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveAndClose.Click
        saveEA()
        Me.Dispose()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCodesetName.SelectedIndexChanged

        'Dim codesetKey As Integer
        If cboCodesetName.Text = "FIPS Code (2-Digit State ID)" Then
            txtCodesetSource.Text = "FIPS / ANSI (http://www.itl.nist.gov/fipspubs/co-codes/states.txt)"
        End If
        If cboCodesetName.Text = "FIPS Code (3-Digit County ID)" Then
            txtCodesetSource.Text = "FIPS / ANSI (http://www.itl.nist.gov/fipspubs/co-codes/states.txt)"
        End If
        If cboCodesetName.Text = "FIPS Code (5-Digit State & County ID)" Then
            txtCodesetSource.Text = "FIPS / ANSI (http://www.itl.nist.gov/fipspubs/co-codes/states.txt)"
        End If
        If cboCodesetName.Text = "US Postal Zip Codes" Then
            txtCodesetSource.Text = "US Census / US Postal Service (http://www.census.gov/tiger/tms/gazetteer/zips.txt)"
        End If
        If cboCodesetName.Text = "US Postal 2-Letter State" Then
            txtCodesetSource.Text = "US Census / US Postal Service (http://www.census.gov/tiger/tms/gazetteer/zips.txt)"
        End If
        If cboCodesetName.Text = "MAF / TIGER Feature Class Codes (MTFCC)" Then
            txtCodesetSource.Text = "MAF / TIGER (http://www.census.gov/geo/www/tiger/tgrshp2009/TGRSHP09AF.pdf)"
        End If

        Dim curCodeset As codesetd
        curCodeset = curEA.attributesDict(Me.lstFields.SelectedItems(0).Text).c_attribdomv.attributeDomainValues(0)
        curCodeset.codesetn = cboCodesetName.Text
        curCodeset.codesets = txtCodesetSource.Text

    End Sub

    Private Sub ComboBox1_TextUpdate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCodesetName.TextUpdate

    End Sub


    Private Sub txtOverview_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOverview.TextChanged

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine)
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
End Class