Imports ESRI.ArcGIS.esriSystem

Namespace My

    Partial Friend Class MyApplication
        Private m_AOLicenseInitializer As LicenseInitializer = New StandAloneEAEditor.LicenseInitializer()

        Private Sub MyApplication_Startup(sender As Object, e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'ESRI License Initializer generated code.

            '#Arc 10 syntax
            'If (Not m_AOLicenseInitializer.InitializeApplication(New esriLicenseProductCode() {esriLicenseProductCode.esriLicenseProductCodeArcView, esriLicenseProductCode.esriLicenseProductCodeArcEditor, esriLicenseProductCode.esriLicenseProductCodeArcInfo}, _
            'New esriLicenseExtensionCode() {})) Then
            '    MsgBox(m_AOLicenseInitializer.LicenseMessage() + vbNewLine + vbNewLine _
            '    + "This application could not initialize with the correct ArcGIS license and will shutdown.")
            '    m_AOLicenseInitializer.ShutdownApplication()
            '    e.Cancel = True
            '    Return
            'End If

            '#Arc 10.1 Syntax
            'm_AOLicenseInitializer.InitializeApplication(New esriLicenseProductCode() {esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced}, _
            'New esriLicenseExtensionCode() {})

            '**DEVELOPMENT NOTES**
            'Syntax for license initialization is different for ArcGIS 10.0 and ArcGIS 10.1, however the syntax below works for 10.0, 10.1, and 10.2.
            'To get the EA tool to work in different versions of ArcGIS, it is necessary to have a development machine set up with that version of Arc installed
            'to compile the .NET code against the ESRI references, in the proper version.
            '1. Install ESRI of the appropiate version on a development machine.
            '2. Install Visual Studio. I have been using the Express version. All of the development for the MetadataWizard has been done in the 2010 version of Visual Studio.
            '3. Install the ESRI SDK for .NET, using the appropriate ESRI version. The connection between Visual Studio and the SDK appears to be automatic. 
            '4. Copy a version of the .NET project to new folder. Naming it something to indicate the version of ESRI it is intended to run with is helpful. Open up the .SLN file.
            '5. If necessary, 'References' may need to be added in the 'Solution Explorer' for the .NET code in visual studio to ESRI. I added:
            '-ESRI.ArcGIS.ADF.Local
            '-ESRI.ArcGIS.ADF.DataSourcesFile
            '-ESRI.ArcGIS.ADF.DataSourcesGDB
            '-ESRI.ArcGIS.ADF.DataSourcesNetCDF
            '-ESRI.ArcGIS.ADF.DataSourcesOleDB
            '-ESRI.ArcGIS.ADF.DataSourcesRaster
            '-ESRI.ArcGIS.ADF.Geodatabase
            '-ESRI.ArcGIS.ADF.Geoprocessing
            '-ESRI.ArcGIS.ADF.System
            '-ESRI.ArcGIS.ADF.Version
            '6. Run the code in visual studio. It should now work, having the appropriate SDK configured.
            '7. Build the application and the solution file from within the IDE.
            '8. Update the Python code used by the ArcCatalog tool accordingly to check for the ESRI version, then apply the appropriate switch for the 10.0, 10.1, or 10.2 executable.
            '9. Ensure all necessary versions of the .NET exectuable are provided with distribution materials.
            '10. Lastly, it may be necessary to create a new ArcToolbox tool using the version of ArcCatalog for which it is intened. Version 10.0 and 10.1 toolboxes were incompatible,
            'so this was performed.

            'Test Syntax to run EATool from CommandLine:
            'C:\Windows\System32>"N:\Metadata\NBII\TechnicalAssistance\OME_Tool\StandAloneEAEditor\StandAloneEAEditor2\bin\Debug\StandAloneEAEditor2.exe" N:\Metadata\MetadataWizard\MDWizardTestingQAQC\TestData\CO_NM_SolarFacilities.shp C:\Temp\EAtestDREW.txt

            Dim License1 As esriLicenseProductCode = 40 '"esriLicenseProductCodeArcView" in Arc 10, "esriLicenseProductCodeBasic" in Arc 10.1
            Dim License2 As esriLicenseProductCode = 50 '"esriLicenseProductCodeArcEditor" in Arc 10, "esriLicenseProductCodeStandard" in Arc 10.1
            Dim License3 As esriLicenseProductCode = 60 '"esriLicenseProductCodeArcInfo" in Arc 10, "esriLicenseProductCodeAdvanced" in Arc 10.1

            If (Not m_AOLicenseInitializer.InitializeApplication(New esriLicenseProductCode() {License1, License2, License3}, _
            New esriLicenseExtensionCode() {})) Then
                MsgBox(m_AOLicenseInitializer.LicenseMessage() + vbNewLine + vbNewLine _
                + "This application could not initialize with the correct ArcGIS license and will now shutdown.")
                m_AOLicenseInitializer.ShutdownApplication()
                e.Cancel = True
                Return
            End If
        End Sub

        Private Sub MyApplication_Shutdown(sender As Object, e As System.EventArgs) Handles Me.Shutdown
            'ESRI License Initializer generated code.
            'Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication()
        End Sub
    End Class


End Namespace

