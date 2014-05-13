Imports System.IO
Imports System.Xml
Imports System.Windows.Forms


Public Class frmMetadataEditor

    Public xmlMD As New XmlDocument
    Public xmlMDOutput As New XmlDocument

    Private sPath As String
    Private sInFile As String
    Private sOutFile As String
    'Private sIExplorerPath As String

    Private sPreviewCount As Integer

    Dim txtSourceTitle As TextBox
    Dim txtSourcePublisher As Object
    Dim cboSourceDataType As ComboBox
    Dim txtSourcePubDate As TextBox
    Dim txtSourceOriginator As TextBox
    Dim txtSourcePubPlace As TextBox
    Dim dgvSourceOnlineLink As DataGridView
    Dim gbxSourceTimePeriodInfo As GroupBox
    Dim txtSourceAbbreviation As TextBox
    Dim txtSourceContribution As TextBox
    Dim txtSourceBeginDate As TextBox
    Dim txtSourceEndDate As TextBox
    Dim cboSourceCurrentnessRef As ComboBox
    Dim labSourceBeginDate As Label
    Dim labSourceCurrentnessRef As Label
    Dim labSourceEndDate As Label

    Dim txtProcessStep As TextBox
    Dim txtProcessDate As TextBox
    Dim txtPlaceKeyThesaurus As Object
    Dim labPlaceKeyTip As Object
    Dim gbxPlaceKeyReqNote As GroupBox
    Dim labAdditionalPlaceKeyReqNote1 As Label
    Dim labAdditionalPlaceKeyReqNote2 As Label
    Dim dgvPlaceKeywords As Object
    Dim txtTopicKeyThesaurus As TextBox
    Dim dgvTopicKeywords As Object
    Dim labTopicKeyTip As Label
    Dim lab101 As Object
    Dim lab102 As Object
    Dim lab103 As Object



#Region "Radio Buttons Tab 1"

    'Data Set Has a Larger Work Citation Yes/No
    Private Sub LargerWorkYes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSLargerWorkYes.CheckedChanged
        If rbDSLargerWorkYes.Checked Then

            txtDSLargerWorkTitle.Enabled = True
            dgvDSLargerWorkOriginator.Enabled = True
            cboDSLargerWorkFormat.Enabled = True
            txtDSLargerWorkPubDate.Enabled = True
            txtDSLargerWorkPublisher.Enabled = True
            txtDSLargerWorkPubPlace.Enabled = True

            txtDSLargerWorkSeriesName.Enabled = True
            txtDSLargerWorkSeriesNum.Enabled = True

            txtDSLargerWorkOnlineLink.Enabled = True '(not a required FGDC element)
            txtDSLargerWorkEdition.Enabled = True '(not a required FGDC element)

            dgvDSLargerWorkOriginator.DefaultCellStyle.BackColor = Color.White

            'Load Routine
            Try
                Dim DSLargerWorkOrigin As Collection
                DSLargerWorkOrigin = getMultiNodeValues(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/origin")

                If DSLargerWorkOrigin IsNot Nothing Then
                    For Each iOrigin In DSLargerWorkOrigin
                        dgvDSLargerWorkOriginator.Rows.Add(iOrigin)
                    Next iOrigin
                End If

            Catch ex As Exception
        End Try

        Dim DSLargerWorkTitle As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/title")
        txtDSLargerWorkTitle.Text = DSLargerWorkTitle

        Dim DSLargerWorkFormat As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/geoform")
        cboDSLargerWorkFormat.Text = DSLargerWorkFormat

        Dim DSLargerWorkPubDate As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/pubdate")
        txtDSLargerWorkPubDate.Text = DSLargerWorkPubDate

        Dim DSLargerWorkPublisher As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/pubinfo/publish")
        txtDSLargerWorkPublisher.Text = DSLargerWorkPublisher

        Dim DSLargerWorkPubPlace As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/pubinfo/pubplace")
        txtDSLargerWorkPubPlace.Text = DSLargerWorkPubPlace

        Dim DSLargerWorkSeriesName As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/serinfo/sername")
        txtDSLargerWorkSeriesName.Text = DSLargerWorkSeriesName

        Dim DSLargerWorkSeriesNum As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/serinfo/issue")
        txtDSLargerWorkSeriesNum.Text = DSLargerWorkSeriesNum

        Dim DSLargerWorkOnlineLink As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/onlink")
        txtDSLargerWorkOnlineLink.Text = DSLargerWorkOnlineLink

        Dim DSLargerWorkEdition As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo/edition")
        txtDSLargerWorkEdition.Text = DSLargerWorkEdition

        End If
    End Sub

    Private Sub LargerWorkNo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSLargerWorkNo.CheckedChanged
        If rbDSLargerWorkNo.Checked Then

            txtDSLargerWorkTitle.Enabled = False
            txtDSLargerWorkTitle.Text = ""

            dgvDSLargerWorkOriginator.Enabled = False
            dgvDSLargerWorkOriginator.Rows.Clear()
            dgvDSLargerWorkOriginator.DefaultCellStyle.BackColor = Color.WhiteSmoke

            cboDSLargerWorkFormat.Enabled = False
            cboDSLargerWorkFormat.Text = ""

            txtDSLargerWorkPubDate.Enabled = False
            txtDSLargerWorkPubDate.Text = ""

            txtDSLargerWorkPublisher.Enabled = False
            txtDSLargerWorkPublisher.Text = ""

            txtDSLargerWorkPubPlace.Enabled = False
            txtDSLargerWorkPubPlace.Text = ""

            txtDSLargerWorkSeriesName.Enabled = False
            txtDSLargerWorkSeriesName.Text = ""

            txtDSLargerWorkSeriesNum.Enabled = False
            txtDSLargerWorkSeriesNum.Text = ""

            txtDSLargerWorkOnlineLink.Enabled = False '(not a required FGDC element)
            txtDSLargerWorkOnlineLink.Text = ""

            txtDSLargerWorkEdition.Enabled = False '(not a required FGDC element)
            txtDSLargerWorkEdition.Text = ""

            Try
                removeNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo/lworkcit")
            Catch ex As Exception
            End Try

        End If
    End Sub

    'Data Set Is a Data Series Yes/No
    Private Sub SeriesYes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSSeriesYes.CheckedChanged
        If rbDSSeriesYes.Checked = True Then

            txtDSSeriesName.Enabled = True
            txtDSSeriesNumber.Enabled = True

            Dim DSSeriesName As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/serinfo/sername")
            txtDSSeriesName.Text = DSSeriesName

            Dim DSSeriesNumber As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/serinfo/issue")
            txtDSSeriesNumber.Text = DSSeriesNumber

        End If
    End Sub

    Private Sub SeriesNo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSSeriesNo.CheckedChanged
        If rbDSSeriesNo.Checked = True Then

            txtDSSeriesName.Enabled = False
            txtDSSeriesName.Text = ""

            txtDSSeriesNumber.Enabled = False
            txtDSSeriesNumber.Text = ""

            Try
                removeNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo/serinfo")
            Catch ex As Exception
            End Try
        End If
    End Sub


    'Data Set Publication Info Yes/No
    Private Sub DSPublicationYes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSPubInfoYes.CheckedChanged
        If rbDSPubInfoYes.Checked = True Then

            txtDSPubPlace.Enabled = True
            txtDSPublisher.Enabled = True

            Dim DSPubPlace As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/pubinfo/pubplace")
            txtDSPubPlace.Text = DSPubPlace

            Dim DSPublisher As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/pubinfo/publish")
            txtDSPublisher.Text = DSPublisher

        End If
    End Sub

    Private Sub DSPublicationNo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSPubInfoNo.CheckedChanged
        If rbDSPubInfoNo.Checked = True Then

            txtDSPubPlace.Enabled = False
            txtDSPubPlace.Text = ""

            txtDSPublisher.Enabled = False
            txtDSPublisher.Text = ""

            Try
                removeNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo/pubinfo")
            Catch ex As Exception
            End Try
        End If
    End Sub


    'Data Set Place Keywords Yes/No
    Private Sub rbPlaceKeywordsYes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbPlaceKeywordsYes.CheckedChanged
        If rbPlaceKeywordsYes.Checked = True Then

            tabCtrlDSPlaceKeywords.SelectedIndex = 0
            tabCtrlDSPlaceKeywords.SelectedTab.Enabled = True

            tabCtrlDSPlaceKeywords.Enabled = True

            dgvPlaceKeywords1.Enabled = True
            dgvPlaceKeywords1.DefaultCellStyle.BackColor = Color.White

            btnAddPlaceKeywordSet.Enabled = True
            btnDeletePlaceKeywordSet.Enabled = True
            txtPlaceKeyThesaurus1.Enabled = True
            txtPlaceKeyThesaurus1.Text = "None"

            'Load Routine

            Try
                Dim tabCt As Integer = 1
                Dim multiNodeList As XmlNodeList = xmlMD.SelectNodes("metadata/idinfo/keywords/place")
                Dim node As XmlNode
                If multiNodeList IsNot Nothing Then
                    For Each node In multiNodeList

                        If tabCt = 1 Then
                            'For first instance of a place thesaurus/place keywords pairing, populate the existing tab.

                            populatePlaceKeywords1(node, tabCt)

                            tabCt = tabCt + 1

                        ElseIf tabCt > 1 Then
                            'For instance #2 and beyond, create new tabs as needed.

                            clonePlaceKeywordsTab(tabCt)
                            populateAdditionalPlaceKeywords(node, tabCt)

                            tabCt = tabCt + 1

                        End If

                    Next node
                End If
            Catch ex As Exception
            End Try

        End If
    End Sub

    Private Sub rbPlaceKeywordsNo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbPlaceKeywordsNo.CheckedChanged
        If rbPlaceKeywordsNo.Checked = True Then

            tabCtrlDSPlaceKeywords.Enabled = False

            dgvPlaceKeywords1.Enabled = False
            dgvPlaceKeywords1.Rows.Clear()
            dgvPlaceKeywords1.DefaultCellStyle.BackColor = Color.WhiteSmoke

            btnAddPlaceKeywordSet.Enabled = False
            btnDeletePlaceKeywordSet.Enabled = False
            txtPlaceKeyThesaurus1.Enabled = False
            txtPlaceKeyThesaurus1.Text = ""

            'Eliminate all other thesaurus/keyword pair tabs, if any exist.
            For Each page As TabPage In tabCtrlDSPlaceKeywords.TabPages
                If page.Name = "tabPlaceKeywords1" Then Continue For
                tabCtrlDSPlaceKeywords.TabPages.Remove(page)
            Next page

            Try
                deleteChildren(xmlMDOutput, "metadata/idinfo/keywords", "place")
            Catch ex As Exception
            End Try

        End If
    End Sub


    'Data Set Contact Yes/No
    Private Sub rbDSContactYes_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSContactYes.CheckedChanged

        If rbDSContactYes.Checked Then

            txtDSContactOrg.Enabled = True
            txtDSContactPerson.Enabled = True
            txtDSContactPersonTitle.Enabled = True
            txtDSContactPhone.Enabled = True
            txtDSContactEmail.Enabled = True
            txtDSContactFax.Enabled = True
            txtDSContactAddress1.Enabled = True
            txtDSContactAddress2.Enabled = True
            txtDSContactAddress3.Enabled = True
            txtDSContactCity.Enabled = True
            txtDSContactState.Enabled = True
            txtDSContactZip.Enabled = True
            txtDSContactCountry.Enabled = True
            cboDSContactAddressType.Enabled = True
            txtUSGSLookupDSContact.Enabled = True

            'Handle the "Contact Organization Primary" possibility
            If nodeExists(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg") Then
                Try
                    Dim DSContactOrg1 As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg")
                    txtDSContactOrg.Text = DSContactOrg1
                    txtDSContactOrg.Tag = "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg"

                    Dim DSContactPerson1 As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntper")
                    txtDSContactPerson.Text = DSContactPerson1
                    txtDSContactPerson.Tag = "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntper"

                Catch ex As Exception
                End Try
            End If

            'Handle the "Contact Person Primary" possibility
            If nodeExists(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper") Then
                Try
                    Dim DSContactPerson2 As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper")
                    txtDSContactPerson.Text = DSContactPerson2
                    txtDSContactPerson.Tag = "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper"

                    Dim DSContactOrg2 As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntorg")
                    txtDSContactOrg.Text = DSContactOrg2
                    txtDSContactOrg.Tag = "metadata/idinfo/ptcontac/cntinfo/cntperp/cntorg"

                Catch ex As Exception
                End Try
            End If

            Dim DSContactEmail As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntemail")
            txtDSContactEmail.Text = DSContactEmail

            Dim DSContactPersonTitle As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntpos")
            txtDSContactPersonTitle.Text = DSContactPersonTitle

            Dim DSContactPhone As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntvoice")
            txtDSContactPhone.Text = DSContactPhone

            Dim DSContactFax As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntfax")
            txtDSContactFax.Text = DSContactFax

            Dim DSContactAddresses As Collection
            DSContactAddresses = getMultiNodeValues(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/address")

            If DSContactAddresses.Count > 0 Then
                Try
                    Dim DSContactAddress1 As String = DSContactAddresses(1)
                    txtDSContactAddress1.Text = DSContactAddress1
                    Dim DSContactAddress2 As String = DSContactAddresses(2)
                    txtDSContactAddress2.Text = DSContactAddress2
                    Dim DSContactAddress3 As String = DSContactAddresses(3)
                    txtDSContactAddress3.Text = DSContactAddress3
                Catch ex As Exception
                End Try
            End If


            Dim DSContactAddressType As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/addrtype")
            cboDSContactAddressType.Text = DSContactAddressType

            Dim DSContactCity As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/city")
            txtDSContactCity.Text = DSContactCity

            Dim DSContactState As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/state")
            txtDSContactState.Text = DSContactState

            Dim DSContactZip As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/postal")
            txtDSContactZip.Text = DSContactZip

            Dim DSContactCountry As String = getNodeText(xmlMD, "metadata/idinfo/ptcontac/cntinfo/cntaddr/country")
            txtDSContactCountry.Text = DSContactCountry

        End If
    End Sub

    Private Sub rbDSContactNo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSContactNo.CheckedChanged

        If rbDSContactNo.Checked Then

            txtDSContactOrg.Enabled = False
            txtDSContactOrg.Text = ""

            txtDSContactPerson.Enabled = False
            txtDSContactPerson.Text = ""

            txtDSContactPersonTitle.Enabled = False
            txtDSContactPersonTitle.Text = ""

            txtDSContactPhone.Enabled = False
            txtDSContactPhone.Text = ""

            txtDSContactEmail.Enabled = False
            txtDSContactEmail.Text = ""

            txtDSContactFax.Enabled = False
            txtDSContactFax.Text = ""

            txtDSContactAddress1.Enabled = False
            txtDSContactAddress1.Text = ""

            txtDSContactAddress2.Enabled = False
            txtDSContactAddress2.Text = ""

            txtDSContactAddress3.Enabled = False
            txtDSContactAddress3.Text = ""

            txtDSContactCity.Enabled = False
            txtDSContactCity.Text = ""

            txtDSContactState.Enabled = False
            txtDSContactState.Text = ""

            txtDSContactZip.Enabled = False
            txtDSContactZip.Text = ""

            txtDSContactCountry.Enabled = False
            txtDSContactCountry.Text = ""

            cboDSContactAddressType.Enabled = False
            cboDSContactAddressType.Text = ""

            txtUSGSLookupDSContact.Enabled = False
            txtUSGSLookupDSContact.Text = ""

            Try
                removeNode(xmlMDOutput, "metadata/idinfo/ptcontac")
            Catch ex As Exception
            End Try

        End If
    End Sub

#End Region

#Region "Radio Buttons Tab 2"

    'Source Inputs Were Used Yes/No
    Private Sub rbNoSourceInputs_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbNoSourceInputs.CheckedChanged
        If rbNoSourceInputs.Checked = True Then

            'Clean out and disable source input tab #1
            tabCtrlDSSourceInputs.Enabled = False
            btnAddNewSourceInput.Enabled = False
            btnAddNewSourceInput.BackColor = Color.WhiteSmoke
            btnDeleteSourceInput.Enabled = False
            btnDeleteSourceInput.BackColor = Color.WhiteSmoke

            txtSource1Title.Text = ""
            txtSource1Publisher.Text = ""
            cboSource1DataType.Text = ""
            txtSource1PubDate.Text = ""
            txtSource1Originator.Text = ""
            txtSource1PubPlace.Text = ""

            dgvSource1OnlineLink.Rows.Clear()
            dgvSource1OnlineLink.DefaultCellStyle.BackColor = Color.WhiteSmoke

            txtSource1BeginDate.Text = ""
            txtSource1EndDate.Text = ""
            cboSource1CurrentnessRef.Text = ""
            txtSource1Abbreviation.Text = ""
            txtSource1Contribution.Text = ""

            'Eliminate all other source input tabs, if any exist.
            For Each page As TabPage In tabCtrlDSSourceInputs.TabPages
                If page.Name = "tabSourceInfo1" Then Continue For
                tabCtrlDSSourceInputs.TabPages.Remove(page)
            Next page

            Try
                deleteChildren(xmlMDOutput, "metadata/dataqual/lineage", "srcinfo")
            Catch ex As Exception
            End Try

        End If
    End Sub

    Private Sub rbSourceInputs_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbSourceInputs.CheckedChanged
        If rbSourceInputs.Checked = True Then

            tabCtrlDSSourceInputs.SelectedIndex = 0
            tabCtrlDSSourceInputs.SelectedTab.Enabled = True

            tabCtrlDSSourceInputs.Enabled = True
            btnAddNewSourceInput.Enabled = True
            btnAddNewSourceInput.BackColor = Color.DeepSkyBlue
            btnDeleteSourceInput.Enabled = True
            btnDeleteSourceInput.BackColor = Color.Silver
            dgvSource1OnlineLink.DefaultCellStyle.BackColor = Color.White

            Dim tabCt As Integer = 1

            Dim multiNodeList As XmlNodeList = xmlMD.SelectNodes("metadata/dataqual/lineage/srcinfo")
            Dim node As XmlNode

            If multiNodeList.Count = 0 Then
                txtSource1Abbreviation.ForeColor = Color.SlateGray
                txtSource1Abbreviation.Text = ("Source Input" & Str(tabCt))
                txtSource1Contribution.ForeColor = Color.SlateGray
                txtSource1Contribution.Text = "Source information used in support of the development of the data set."
                tabCt = tabCt + 1
            End If

            If multiNodeList.Count > 0 Then
                For Each node In multiNodeList

                    If tabCt = 1 Then
                        'For first instance of a source input, populate the existing tab.
                        'This section should really allow for strings to be converted variables (akin to Python 'eval': "Source" & str(tabCt) & "Title")...

                        populateSourceInput1(node, tabCt)
                        tabCt = tabCt + 1

                    ElseIf tabCt > 1 Then
                        'For instance #2 and beyond, create new tabs as needed.

                        cloneSourceInputTab(tabCt)
                        populateAdditionalSourceInput(node, tabCt)
                        tabCt = tabCt + 1

                    End If

                Next node
            End If
        End If
    End Sub


#End Region

#Region "Radio Buttons Tab 4"

    'Data Set Has Distribution Information Yes/No
    Private Sub rbDSYesDistributionInfo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSYesDistributionInfo.CheckedChanged
        If rbDSYesDistributionInfo.Checked = True Then

            txtDSDistOrg.Enabled = True
            txtDSDistPerson.Enabled = True
            txtDSDistPersonTitle.Enabled = True
            txtDSDistPhone.Enabled = True
            txtDSDistFax.Enabled = True
            txtDSDistEmail.Enabled = True

            txtDSDistAddress1.Enabled = True
            txtDSDistAddress2.Enabled = True
            txtDSDistAddress3.Enabled = True
            txtDSDistCity.Enabled = True
            txtDSDistState.Enabled = True
            txtDSDistZip.Enabled = True
            txtDSDistCountry.Enabled = True
            cboDSDistAddressType.Enabled = True

            txtDSNetworkResource.Enabled = True
            txtDSCustomDistOrder.Enabled = True
            txtDSDistLiability.Enabled = True
            txtDSDistFees.Enabled = True

            btnImportDSContactToDistContact.Enabled = True
            btnImportMetaContactToDistContact.Enabled = True

            txtUSGSLookupDistContact.Enabled = True
            btnUSGSLookupDistContact.Enabled = True

            rbOnlineDistribution.Enabled = True
            rbStockDistributionText.Enabled = True
            rbCustomDistribution.Enabled = True


            'Handle the "Contact Person Primary" possibility
            If nodeExists(xmlMD, "metadata/distinfo/distrib/cntinfo/cntperp/cntper") Then
                Try
                    Dim DSDistPerson As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntperp/cntper")
                    txtDSDistPerson.Text = DSDistPerson
                    txtDSDistPerson.Tag = "metadata/distinfo/distrib/cntinfo/cntperp/cntper"

                    Dim DSDistOrg As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntperp/cntorg")
                    txtDSDistOrg.Text = DSDistOrg
                    txtDSDistOrg.Tag = "metadata/distinfo/distrib/cntinfo/cntperp/cntorg"

                Catch ex As Exception
                End Try
            End If

            'Handle the "Contact Organization Primary" possibility
            If nodeExists(xmlMD, "metadata/distinfo/distrib/cntinfo/cntorgp/cntorg") Then
                Try
                    Dim DSDistOrg As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntorgp/cntorg")
                    txtDSDistOrg.Text = DSDistOrg
                    txtDSDistOrg.Tag = "metadata/distinfo/distrib/cntinfo/cntorgp/cntorg"

                    Dim DSDistPerson As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntorgp/cntper")
                    txtDSDistPerson.Text = DSDistPerson
                    txtDSDistPerson.Tag = "metadata/distinfo/distrib/cntinfo/cntorgp/cntper"

                Catch ex As Exception
                End Try
            End If

            Dim DSDistPersonTitle As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntpos")
            txtDSDistPersonTitle.Text = DSDistPersonTitle

            Dim DSDistPhone As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntvoice")
            txtDSDistPhone.Text = DSDistPhone

            Dim DSDistFax As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntfax")
            txtDSDistFax.Text = DSDistFax

            Dim DSDistEmail As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntemail")
            txtDSDistEmail.Text = DSDistEmail

            Dim DSDistAddresses As Collection
            DSDistAddresses = getMultiNodeValues(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/address")

            If DSDistAddresses.Count > 0 Then
                Try
                    Dim DSDistAddress1 As String = DSDistAddresses(1)
                    txtDSDistAddress1.Text = DSDistAddress1
                    Dim DSDistAddress2 As String = DSDistAddresses(2)
                    txtDSDistAddress2.Text = DSDistAddress2
                    Dim DSDistAddress3 As String = DSDistAddresses(3)
                    txtDSDistAddress3.Text = DSDistAddress3
                Catch ex As Exception
                End Try
            End If


            Dim DSDistCity As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/city")
            txtDSDistCity.Text = DSDistCity

            Dim DSDistState As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/state")
            txtDSDistState.Text = DSDistState

            Dim DSDistZip As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/postal")
            txtDSDistZip.Text = DSDistZip

            Dim DSDistCountry As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/country")
            txtDSDistCountry.Text = DSDistCountry

            Dim DSDistAddressType As String = getNodeText(xmlMD, "metadata/distinfo/distrib/cntinfo/cntaddr/addrtype")
            cboDSDistAddressType.Text = DSDistAddressType

            'Data Distribution Liability
            If nodeExists(xmlMD, "metadata/distinfo/distliab") Then
                Dim DSDistLiability As String = getNodeText(xmlMD, "metadata/distinfo/distliab")
                txtDSDistLiability.ForeColor = Color.Black
                txtDSDistLiability.Text = DSDistLiability
            Else
                txtDSDistLiability.ForeColor = Color.SlateGray
                txtDSDistLiability.Text = "Distributor assumes no liability for misuse of data."
            End If

            'Data Distribution Fees
            If nodeExists(xmlMD, "metadata/distinfo/stdorder/fees") Then
                Dim DSDistFees As String = getNodeText(xmlMD, "metadata/distinfo/stdorder/fees")
                txtDSDistFees.ForeColor = Color.Black
                txtDSDistFees.Text = DSDistFees
            Else
                txtDSDistFees.ForeColor = Color.SlateGray
                txtDSDistFees.Text = "None. No fees are applicable for obtaining the data set."
            End If

            'Data Distribution - Order Process

            '**We check these in this order so that if both a network resource and a custom order process are present in an existing record,
            '**we will show the network resource (the custom order instructions will be kept as well, just not visible). If a user switches
            '**from the network resource option to 'other', the network resource info and custom instructions will both be erased and replaced. 
            '**The output XML will then only contain the custom instructions they provided.

            If nodeExists(xmlMD, "metadata/distinfo/custom") Then
                Dim DSCustomDistOrder As String = getNodeText(xmlMD, "metadata/distinfo/custom")
                txtDSCustomDistOrder.Text = DSCustomDistOrder
                rbCustomDistribution.Checked = True
            End If

            '**Try to grab the first online linkage and use it for the Network Resource element.
            If nodeExists(xmlMD, "metadata/citation/citeinfo/onlink") Then
                Dim OnlineLinkage As String = getNodeText(xmlMD, "metadata/citation/citeinfo/onlink[1]")
                txtDSNetworkResource.Text = OnlineLinkage
                rbOnlineDistribution.Checked = True
            End If

            '**Here we look for the actual network resource element. If present, we will retain it (overriding the online linkage that may have been used).
            If nodeExists(xmlMD, "metadata/distinfo/stdorder/digform/digtopt/onlinopt/computer/networka/networkr") Then
                Dim DSNetworkResource As String = getNodeText(xmlMD, "metadata/distinfo/stdorder/digform/digtopt/onlinopt/computer/networka/networkr")
                txtDSNetworkResource.Text = DSNetworkResource
                rbOnlineDistribution.Checked = True
            End If

        End If
    End Sub

    Private Sub rbDSNoDistributionInfo_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDSNoDistributionInfo.CheckedChanged
        If rbDSNoDistributionInfo.Checked = True Then

            txtDSDistOrg.Enabled = False
            txtDSDistOrg.Text = ""

            txtDSDistPerson.Enabled = False
            txtDSDistPerson.Text = ""

            txtDSDistPersonTitle.Enabled = False
            txtDSDistPersonTitle.Text = ""

            txtDSDistPhone.Enabled = False
            txtDSDistPhone.Text = ""

            txtDSDistFax.Enabled = False
            txtDSDistFax.Text = ""

            txtDSDistEmail.Enabled = False
            txtDSDistEmail.Text = ""

            txtDSDistAddress1.Enabled = False
            txtDSDistAddress1.Text = ""

            txtDSDistAddress2.Enabled = False
            txtDSDistAddress2.Text = ""

            txtDSDistAddress3.Enabled = False
            txtDSDistAddress3.Text = ""

            txtDSDistCity.Enabled = False
            txtDSDistCity.Text = ""

            txtDSDistState.Enabled = False
            txtDSDistState.Text = ""

            txtDSDistZip.Enabled = False
            txtDSDistZip.Text = ""

            txtDSDistCountry.Enabled = False
            txtDSDistCountry.Text = ""

            cboDSDistAddressType.Enabled = False
            cboDSDistAddressType.Text = ""

            txtDSNetworkResource.Enabled = False
            txtDSNetworkResource.Text = ""

            txtDSCustomDistOrder.Enabled = False
            txtDSCustomDistOrder.Text = ""

            txtDSDistLiability.Enabled = False
            txtDSDistLiability.Text = ""

            txtDSDistFees.Enabled = False
            txtDSDistFees.Text = ""

            btnImportDSContactToDistContact.Enabled = False
            btnImportMetaContactToDistContact.Enabled = False

            txtUSGSLookupDistContact.Enabled = False
            btnUSGSLookupDistContact.Enabled = False

            rbOnlineDistribution.Enabled = False
            rbStockDistributionText.Enabled = False
            rbCustomDistribution.Enabled = False

            Try
                removeNode(xmlMDOutput, "metadata/distinfo")
            Catch ex As Exception
            End Try

        End If
    End Sub

    Private Sub rbDistributionMethod_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbOnlineDistribution.CheckedChanged, rbStockDistributionText.CheckedChanged, rbCustomDistribution.CheckedChanged

        If rbOnlineDistribution.Checked = True Then

            txtDSNetworkResource.Enabled = True
            txtDSCustomDistOrder.Enabled = False
            txtDSCustomDistOrder.Text = ""
        End If

        If rbCustomDistribution.Checked = True Then

            txtDSCustomDistOrder.Enabled = True
            txtDSNetworkResource.Enabled = False
            txtDSNetworkResource.Text = ""
            txtDSDistFees.Text = "" '"Fees" are not an allowed element in FGDC if the 'custom' order option is selected.
        End If

        If rbStockDistributionText.Checked = True Then

            txtDSCustomDistOrder.Text = "The data set is not available online. Interested parties should contact the distributor for details on acquiring the data."
            rbCustomDistribution.Checked = True
        End If
    End Sub


#End Region


#Region "Loading"

    'Load existing XML Values into fields'

    Public Sub frmMetadataEditor_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        sInFile = My.Application.CommandLineArgs(0)
        sOutFile = My.Application.CommandLineArgs(1)
        'sIExplorerPath = My.Application.CommandLineArgs(2)

        xmlMD.Load(sInFile)
        xmlMDOutput.Load(sInFile)
        PopulateForm()

        sPreviewCount = 0

    End Sub


    Private Sub PopulateForm()
        loadTab1()
        loadTab2()
        loadTab3()
        loadTab4()
    End Sub

    Private Sub loadTab1()
        'POPULATE TAB 1 (IDENTIFICATION INFORMATION)"

        'Data Set Title
        Dim Title As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/title")
        txtDSTitle.Text = Title

        'Data Set Publication Date
        Dim DSPubDate As String = getNodeText(xmlMD, "metadata/idinfo/citation/citeinfo/pubdate")
        txtDSPubDate.Text = DSPubDate

        'Data SetTime Period Information

        '   Single Date'
        If nodeExists(xmlMD, "metadata/idinfo/timeperd/timeinfo/sngdate/caldate") Then
            tabCtrlDSTimePeriod.SelectedTab = tabDSSingleDate
            Dim DSTimePeriodSingleDate As String = getNodeText(xmlMD, "metadata/idinfo/timeperd/timeinfo/sngdate/caldate")
            txtDSTimePeriodSingleDate.Text = DSTimePeriodSingleDate
        End If

        '   Multiple Dates'
        If nodeExists(xmlMD, "metadata/idinfo/timeperd/timeinfo/mdattim/sngdate/caldate") Then
            tabCtrlDSTimePeriod.SelectedTab = tabDSMultiDate
            Dim dateList As Collection = getMultiNodeValues(xmlMD, "metadata/idinfo/timeperd/timeinfo/mdattim/sngdate/caldate")
            If dateList IsNot Nothing Then
                For Each iDate In dateList
                    dgvDSTimePeriodMultiDate.Rows.Add(iDate)
                Next iDate
            End If
        End If

        '   Range of Dates
        If nodeExists(xmlMD, "metadata/idinfo/timeperd/timeinfo/rngdates/begdate") Then
            Dim DSTimePeriodBegDate As String = getNodeText(xmlMD, "metadata/idinfo/timeperd/timeinfo/rngdates/begdate")
            txtDSTimePeriodBegDate.Text = DSTimePeriodBegDate
            tabCtrlDSTimePeriod.SelectedTab = tabDSDateRange
            txtDSTimePeriodBegDate.SelectionStart = 0
        End If
        If nodeExists(xmlMD, "metadata/idinfo/timeperd/timeinfo/rngdates/enddate") Then
            Dim DSTimePeriodEndDate As String = getNodeText(xmlMD, "metadata/idinfo/timeperd/timeinfo/rngdates/enddate")
            txtDSTimePeriodEndDate.Text = DSTimePeriodEndDate
            tabCtrlDSTimePeriod.SelectedTab = tabDSDateRange
            txtDSTimePeriodEndDate.SelectionStart = 0
        End If

        'Data Set Originator(s)
        Dim originatorList As Collection = getMultiNodeValues(xmlMD, "metadata/idinfo/citation/citeinfo/origin")

        If originatorList IsNot Nothing Then
            For Each iOriginator In originatorList
                dgvDSOriginator.Rows.Add(iOriginator)
            Next iOriginator
        End If

        'Data Set Data Status Progress
        Dim DSDataStatus As String = getNodeText(xmlMD, "metadata/idinfo/status/progress")
        cboDSDataStatus.Text = DSDataStatus

        'Data Set Data Updates
        Dim DSDataUpdates As String = getNodeText(xmlMD, "metadata/idinfo/status/update")
        cboDSDataUpdates.Text = DSDataUpdates

        'Data Set Supplemental Info
        Dim DSsuppInfo As String = getNodeText(xmlMD, "metadata/idinfo/descript/supplinf")
        txtDSSuppInfo.Text = DSsuppInfo

        'Data Set Abstract
        Dim DSAbstract As String = getNodeText(xmlMD, "metadata/idinfo/descript/abstract")
        txtDSAbstract.Text = DSAbstract

        'Data Set Purpose
        Dim DSPurpose As String = getNodeText(xmlMD, "metadata/idinfo/descript/purpose")
        txtDSPurpose.Text = DSPurpose

        'Data Set Access Constraints
        If nodeExists(xmlMD, "metadata/idinfo/accconst") Then
            Dim DSAccessCon As String = getNodeText(xmlMD, "metadata/idinfo/accconst")
            txtDSAccessCon.ForeColor = Color.Black
            txtDSAccessCon.Text = DSAccessCon
        End If

        'Data Set Use Constraints
        If nodeExists(xmlMD, "metadata/idinfo/useconst") Then
            Dim DSUseCon As String = getNodeText(xmlMD, "metadata/idinfo/useconst")
            txtDSUseCon.ForeColor = Color.Black
            txtDSUseCon.Text = DSUseCon
        End If

        'Data Set - Data Set Credit
        Dim DSCredit As String = getNodeText(xmlMD, "metadata/idinfo/datacred")
        txtDSCredit.Text = DSCredit

        'Data Set Online Linkage(s)
        If nodeExists(xmlMD, "metadata/idinfo/citation/citeinfo/onlink") Then
            Dim onLinkList As Collection
            onLinkList = getMultiNodeValues(xmlMD, "metadata/idinfo/citation/citeinfo/onlink")
            If onLinkList IsNot Nothing Then
                For Each iLink In onLinkList
                    dgvDSOnlineLink.Rows.Add(iLink)
                Next iLink
            End If
        End If

        'Data Set Currentness Reference
        Dim DSTimePeriodCurrentRef As String = getNodeText(xmlMD, "metadata/idinfo/timeperd/current")
        cboDSTimePeriodCurrentRef.Text = DSTimePeriodCurrentRef

        'Data Set Place Keywords
        If nodeExists(xmlMD, "metadata/idinfo/keywords/place") Then
            rbPlaceKeywordsYes.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If

        'Data Set Topic Keywords
        If nodeExists(xmlMD, "metadata/idinfo/keywords/theme") Then
            Try
                Dim tabCt As Integer = 1
                Dim multiNodeList As XmlNodeList = xmlMD.SelectNodes("metadata/idinfo/keywords/theme")
                Dim node As XmlNode

                For Each node In multiNodeList

                    If tabCt = 1 Then
                        'For first instance of a topic thesaurus/place keywords pairing, populate the existing tab.

                        populateTopicKeywords1(node, tabCt)

                        tabCt = tabCt + 1

                    ElseIf tabCt > 1 Then
                        'For instance #2 and beyond, create new tabs as needed.

                        cloneTopicKeywordsTab(tabCt)
                        populateAdditionalTopicKeywords(node, tabCt)

                        tabCt = tabCt + 1

                    End If

                Next node

            Catch ex As Exception
        End Try
        End If

        loadTab1_LargerWork()
        loadTab1_DSContact()
        loadTab1_DSDataSeriesInfo()
        loadTab1_DSPublisherInfo()

    End Sub

    Private Sub loadTab1_LargerWork()

        'Larger Work Citation Info
        If nodeExists(xmlMD, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo") Then

            rbDSLargerWorkYes.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If
    End Sub

    Private Sub loadTab1_DSContact()

        'Data Set Contact Organization

        If nodeExists(xmlMD, "metadata/idinfo/ptcontac/cntinfo") Then

            rbDSContactYes.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If

    End Sub

    Private Sub loadTab1_DSDataSeriesInfo()

        'Data Set Data Series Info
        If nodeExists(xmlMD, "metadata/idinfo/citation/citeinfo/serinfo") Then

            rbDSSeriesYes.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If

    End Sub

    Private Sub loadTab1_DSPublisherInfo()

        'Data Set Publisher Info

        If nodeExists(xmlMD, "metadata/idinfo/citation/citeinfo/pubinfo") Then

            rbDSPubInfoYes.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If

    End Sub

    Private Sub loadTab2()

        'POPULATE TAB 2 INFORMATION (Data Quality)

        'Data Set Attribute Accuracy
        If nodeExists(xmlMD, "metadata/dataqual/attracc/attraccr") Then
            Dim DSAttributeAccuracy As String = getNodeText(xmlMD, "metadata/dataqual/attracc/attraccr")
            txtDSAttributeAccuracy.ForeColor = Color.Black
            txtDSAttributeAccuracy.Text = DSAttributeAccuracy
        End If

        'Data Set Logical Accuracy
        If nodeExists(xmlMD, "metadata/dataqual/logic") Then
            Dim DSLogicalAccuracy As String = getNodeText(xmlMD, "metadata/dataqual/logic")
            txtDSLogicalAccuracy.ForeColor = Color.Black
            txtDSLogicalAccuracy.Text = DSLogicalAccuracy
        End If

        'Data Set Completeness
        If nodeExists(xmlMD, "metadata/dataqual/complete") Then
            Dim DSCompletenessReport As String = getNodeText(xmlMD, "metadata/dataqual/complete")
            txtDSCompletenessReport.ForeColor = Color.Black
            txtDSCompletenessReport.Text = DSCompletenessReport
        End If

        'Horizontal Accuracy Report
        If nodeExists(xmlMD, "metadata/dataqual/posacc/horizpa/horizpar") Then
            Dim DSHorizAccReport As String = getNodeText(xmlMD, "metadata/dataqual/posacc/horizpa/horizpar")
            txtDSHorizAccReport.ForeColor = Color.Black
            txtDSHorizAccReport.Text = DSHorizAccReport
        End If

        'Vertical Accuracy Report
        If nodeExists(xmlMD, "metadata/dataqual/posacc/vertacc/vertaccr") Then
            Dim DSVertAccReport As String = getNodeText(xmlMD, "metadata/dataqual/posacc/vertacc/vertaccr")
            txtDSVertAccReport.ForeColor = Color.Black
            txtDSVertAccReport.Text = DSVertAccReport
        End If

        'Data Set Source Inputs
        If nodeExists(xmlMD, "metadata/dataqual/lineage/srcinfo") Then

            rbSourceInputs.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If

        'Processing Steps
        Try
            Dim tabCt As Integer = 1
            Dim multiNodeList As XmlNodeList = xmlMD.SelectNodes("metadata/dataqual/lineage/procstep")
            Dim node As XmlNode

            If multiNodeList IsNot Nothing Then
                For Each node In multiNodeList

                    If tabCt = 1 Then
                        'For first instance of a processing step, populate the existing tab.

                        populateProcessStep1(node, tabCt)

                        tabCt = tabCt + 1

                    ElseIf tabCt > 1 Then
                        'For instance #2 and beyond, create new tabs as needed.

                        cloneProcessStepTab(tabCt)
                        populateAdditionalProcessStep(node, tabCt)

                        tabCt = tabCt + 1

                    End If
                Next node
            End If
        Catch ex As Exception
        End Try


    End Sub

    Private Sub loadTab3()

        'POPULATE TAB 3 (METADATA REFERENCE)

        'Handle the "Contact Person Primary" possibility
        If nodeExists(xmlMD, "metadata/metainfo/metc/cntinfo/cntperp/cntper") Then
            Try
                Dim MetaContactPerson As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntperp/cntper")
                txtMetaContactPerson.Text = MetaContactPerson
                txtMetaContactPerson.Tag = "metadata/metainfo/metc/cntinfo/cntperp/cntper"

                Dim MetaContactOrg As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntperp/cntorg")
                txtMetaContactOrg.Text = MetaContactOrg
                txtMetaContactOrg.Tag = "metadata/metainfo/metc/cntinfo/cntperp/cntorg"

            Catch ex As Exception
            End Try
        End If

        'Handle the "Contact Organization Primary" possibility
        If nodeExists(xmlMD, "metadata/metainfo/metc/cntinfo/cntorgp/cntorg") Then
            Try
                Dim MetaContactOrg As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntorgp/cntorg")
                txtMetaContactOrg.Text = MetaContactOrg
                txtMetaContactOrg.Tag = "metadata/metainfo/metc/cntinfo/cntorgp/cntorg"

                Dim MetaContactPerson As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntorgp/cntper")
                txtMetaContactPerson.Text = MetaContactPerson
                txtMetaContactPerson.Tag = "metadata/metainfo/metc/cntinfo/cntorgp/cntper"

            Catch ex As Exception
            End Try
        End If

        Dim MetaContactPersonTitle As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntpos")
        txtMetaContactPersonTitle.Text = MetaContactPersonTitle

        Dim MetaContactPhone As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntvoice")
        txtMetaContactPhone.Text = MetaContactPhone

        Dim MetaContactFax As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntfax")
        txtMetaContactFax.Text = MetaContactFax

        Dim MetaContactEmail As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntemail")
        txtMetaContactEmail.Text = MetaContactEmail

        Dim MetaContactAddresses As Collection
        MetaContactAddresses = getMultiNodeValues(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/address")

        If MetaContactAddresses.Count > 0 Then
            Try
                Dim MetaContactAddress1 As String = MetaContactAddresses(1)
                txtMetaContactAddress1.Text = MetaContactAddress1
                Dim MetaContactAddress2 As String = MetaContactAddresses(2)
                txtMetaContactAddress2.Text = MetaContactAddress2
                Dim MetaContactAddress3 As String = MetaContactAddresses(3)
                txtMetaContactAddress3.Text = MetaContactAddress3
            Catch ex As Exception
            End Try
        End If


        Dim MetaContactCity As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/city")
        txtMetaContactCity.Text = MetaContactCity

        Dim MetaContactState As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/state")
        txtMetaContactState.Text = MetaContactState

        Dim MetaContactZip As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/postal")
        txtMetaContactZip.Text = MetaContactZip

        Dim MetaContactCountry As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/country")
        txtMetaContactCountry.Text = MetaContactCountry

        Dim MetaContactAddressType As String = getNodeText(xmlMD, "metadata/metainfo/metc/cntinfo/cntaddr/addrtype")
        cboMetaContactAddressType.Text = MetaContactAddressType

        If nodeExists(xmlMD, "metadata/metainfo/metstdn") Then
            Dim MetaStandardName As String = getNodeText(xmlMD, "metadata/metainfo/metstdn")
            cboMetaStandardName.Text = MetaStandardName
        Else
            cboMetaStandardName.Text = "FGDC Content Standard for Digital Geospatial Metadata"
        End If

        If nodeExists(xmlMD, "metadata/metainfo/metstdv") Then
            Dim MetaStandardVersion As String = getNodeText(xmlMD, "metadata/metainfo/metstdv")
            cboMetaStandardVersion.Text = MetaStandardVersion
        Else
            cboMetaStandardVersion.Text = "FGDC-STD-001-1998"
        End If
    End Sub

    Private Sub loadTab4()

        'POPULATE TAB 4 (DISTRIBUTION INFORMATION)
        If nodeExists(xmlMD, "metadata/distinfo") Then

            rbDSYesDistributionInfo.Checked = True
            'The load routine is handled with the button check. See Radio Button section.
        End If
    End Sub

#End Region


#Region "Save/Save and Close/Preview Metadata (Buttons)"

    'Save routine
    Private Sub Save()

        saveToInMemory()
        xmlMDOutput.Save(sOutFile)

        ''''''
        'Harness the power of MP to do a little element order cleanup.
        Dim startUpPath As String = Application.StartupPath
        Dim mpPath As String = startUpPath & "\Resources\mp.exe"

        Dim configFile As String = startUpPath & "\Resources\BDP_Mod.cfg"
        'This will ensure any optional BDP elements are also ordered properly. 
        'This config file also has a 'prune' line which removes empty nodes/branches.

        Dim p As New ProcessStartInfo
        'Pad with quotes to handle any spaces in file names or paths when calling MP.
        p.FileName = Chr(34) & mpPath & Chr(34)
        p.Arguments = "-x " & Chr(34) & sOutFile & Chr(34) & " -c " & Chr(34) & configFile & Chr(34) & " " & Chr(34) & sOutFile & Chr(34)
        'p.Arguments = "-x " & Chr(34) & sOutFile & Chr(34) & " " & Chr(34) & sOutFile & Chr(34)
        Try
            Process.Start(p)
            System.Threading.Thread.Sleep(500) 'Wait a moment for MP to run.
        Catch ex As Exception
        End Try
        ''''''
    End Sub

    '"Save" Button
    Private Sub SaveXML(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        Save()

    End Sub

    'Save to In Memory Routine
    Private Sub saveToInMemory()

        'Testing - delete these elements to ensure they are being rebuilt correctly...
        'deleteChildren(xmlMDOutput, "metadata", "idinfo")
        'deleteChildren(xmlMDOutput, "metadata", "dataqual")
        'deleteChildren(xmlMDOutput, "metadata/metainfo", "metc")
        'deleteChildren(xmlMDOutput, "metadata", "distinfo")

        Dim DSAddressList As New Collection
        Dim MDAddressList As New Collection
        Dim DistributorAddressList As New Collection

        Dim ctl As Control = Me

        ' Handle Text Boxes and Combo Box Elements
        Do
            ctl = Me.GetNextControl(ctl, True)

            If ctl IsNot Nothing Then
                If ctl.Enabled Then

                    If TypeOf ctl Is TextBox Or TypeOf ctl Is ComboBox Then

                        ''''''''''''''''
                        'Handle the TextBox/ComboBox elements that repeat at a single Xpath
                        If ctl.Name = "txtDSContactAddress1" Or ctl.Name = "txtDSContactAddress2" Or ctl.Name = "txtDSContactAddress3" Then
                            If ctl.Text <> "" Then
                                DSAddressList.Add(ctl.Text)
                            End If
                        End If

                        If ctl.Name = "txtMetaContactAddress1" Or ctl.Name = "txtMetaContactAddress2" Or ctl.Name = "txtMetaContactAddress3" Then
                            If ctl.Text <> "" Then
                                MDAddressList.Add(ctl.Text)
                            End If
                        End If

                        If ctl.Name = "txtDSDistAddress1" Or ctl.Name = "txtDSDistAddress2" Or ctl.Name = "txtDSDistAddress3" Then
                            If ctl.Text <> "" Then
                                DistributorAddressList.Add(ctl.Text)
                            End If
                        End If
                        ''''''''''''''''

                        ''''''''''''''''
                        'Handle all the other TextBox/ComboBox elements
                        Try
                            If ctl.Text <> "" Then
                                setNodeText(xmlMDOutput, CStr(ctl.Tag), ctl.Text, True)
                            End If

                            '*DI: 4/4/13 Consider removing the above 'If' to only write out elements if they contain content. This seems to be problematic
                            'for deleting element content and the 'Import Contact' features from tab to tab?
                            'Should be accounted for by the 'If' that checks that an element is enabled (above) and the 'prune' option when MP is applied.
                            'Leave in for now.

                        Catch ex As Exception
                            Debug.Print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!missing tag:  " & CStr(ctl.Name))
                        End Try
                        ''''''''''''''''

                    End If

                Else
                    'Not enabled.... eliminate node.
                    'Check "Kill Paths"... some elements need to be removed higher up from their tags...
                    'OR simply kill all empty or disabled controls and write/use a removeDanglingNodesRoutine    

                End If
            End If

        Loop Until ctl Is Nothing

        ''''''''''''''''
        deleteChildren(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr", "address")
        Dim dsStub As XmlNode = AddNode(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr", True)
        For Each iAddress In DSAddressList
            Dim addressNode As XmlNode = xmlMDOutput.CreateElement("address")
            addressNode.InnerText = CStr(iAddress)
            dsStub.AppendChild(addressNode)
        Next

        deleteChildren(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr", "address")
        Dim mdStub As XmlNode = AddNode(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr", True)
        For Each iAddress In MDAddressList
            Dim addressNode As XmlNode = xmlMDOutput.CreateElement("address")
            addressNode.InnerText = CStr(iAddress)
            mdStub.AppendChild(addressNode)
        Next

        deleteChildren(xmlMDOutput, "metadata/distinfo/distrib/cntinfo/cntaddr", "address")
        Dim distStub As XmlNode = AddNode(xmlMDOutput, "metadata/distinfo/distrib/cntinfo/cntaddr", True)
        For Each iAddress In DistributorAddressList
            Dim addressNode As XmlNode = xmlMDOutput.CreateElement("address")
            addressNode.InnerText = CStr(iAddress)
            distStub.AppendChild(addressNode)
        Next

        '''''''''''''''
        Do
            ctl = Me.GetNextControl(ctl, True)

            If ctl IsNot Nothing Then
                If ctl.Enabled Then

                    '''''''''''''''''''''''''''''
                    If ctl.Name = "dgvDSOriginator" Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/citation/citeinfo", "origin")

                        If nodeExists(xmlMDOutput, "metadata/idinfo/citation/citeinfo") = False Then
                            AddNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo", True)
                        End If

                        Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo")
                        Dim RowList As List(Of String) = getDGVList(ctl)

                        Dim reverseRowList As List(Of String) = New List(Of String)(RowList)
                        reverseRowList.Reverse()

                        For Each row In reverseRowList

                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("origin")
                            newNode.InnerText = row
                            stub.InsertBefore(newNode, stub.FirstChild)
                        Next
                    End If
                    '''''''''''''''''''''''''''''
                    If ctl.Name = "dgvDSLargerWorkOriginator" Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo", "origin")

                        If nodeExists(xmlMDOutput, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo") = False Then
                            AddNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo", True)
                        End If

                        Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo/lworkcit/citeinfo")
                        Dim RowList As List(Of String) = getDGVList(ctl)

                        Dim reverseRowList As List(Of String) = New List(Of String)(RowList)
                        reverseRowList.Reverse()

                        For Each row In reverseRowList

                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("origin")
                            newNode.InnerText = row
                            stub.InsertBefore(newNode, stub.FirstChild)
                        Next
                    End If
                    ''''''''''''''''''''''''''''''
                    If ctl.Name = "dgvDSTimePeriodMultiDate" Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/timeperd/timeinfo", "mdattim")

                        Dim RowList As List(Of String) = getDGVList(ctl)
                        If RowList.Count > 0 Then

                            If nodeExists(xmlMDOutput, "metadata/idinfo/timeperd/timeinfo/mdattim") = False Then
                                AddNode(xmlMDOutput, "metadata/idinfo/timeperd/timeinfo/mdattim", True)
                            End If

                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/timeperd/timeinfo/mdattim")

                            Dim reverseRowList As List(Of String) = New List(Of String)(RowList)
                            reverseRowList.Reverse()

                            For Each row In reverseRowList

                                Dim newNode1 As XmlNode = xmlMDOutput.CreateElement("sngdate")
                                Dim newNode2 As XmlNode = xmlMDOutput.CreateElement("caldate")
                                stub.InsertBefore(newNode1, stub.FirstChild)
                                newNode1.InsertBefore(newNode2, newNode1.FirstChild)
                                newNode2.InnerText = row
                            Next
                        End If
                    End If

                    '''''''''''''''''''''''''''''
                    If ctl.Name = "dgvDSOnlineLink" Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/citation/citeinfo", "onlink")

                        Dim RowList As List(Of String) = getDGVList(ctl)

                        If RowList.Count > 0 Then

                            If nodeExists(xmlMDOutput, "metadata/idinfo/citation/citeinfo") = False Then
                                AddNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo", True)
                            End If

                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/citation/citeinfo")

                            For Each row In RowList
                                Dim newNode As XmlNode = xmlMDOutput.CreateElement("onlink")
                                newNode.InnerText = row
                                stub.AppendChild(newNode)
                            Next
                        End If
                    End If

                    ''''''''''''''''''''''''''''
                    If ctl.Name.Contains("tabCtrlDSTopicKeywords") Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/keywords", "theme")

                        Dim tabList As New List(Of TabPage)
                        For Each iTab In tabCtrlDSTopicKeywords.TabPages
                            If iTab.enabled Then
                                tabList.Add(iTab)
                            End If
                        Next

                        For Each iTab In tabList
                            If nodeExists(xmlMDOutput, "metadata/idinfo/keywords") = False Then
                                AddNode(xmlMDOutput, "metadata/idinfo/keywords")
                            End If
                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/keywords")

                            For Each subCtl As Control In iTab.Controls
                                If subCtl.Name.Contains("txtTopicKeyThesaurus") Then
                                    If subCtl.Text <> "" Then
                                        Dim themeNode As XmlNode = xmlMDOutput.CreateElement("theme")
                                        Dim themeKT As XmlNode = xmlMDOutput.CreateElement("themekt")

                                        stub.AppendChild(themeNode)
                                        themeNode.PrependChild(themeKT)
                                        themeKT.InnerText = subCtl.Text

                                        For Each iSubCtl As Control In iTab.Controls
                                            If iSubCtl.Name.Contains("dgvTopicKeywords") Then

                                                Dim RowList As List(Of String) = getDGVList(iSubCtl)
                                                If RowList.Count > 0 Then

                                                    For Each row In RowList
                                                        Dim themeKey As XmlNode = xmlMDOutput.CreateElement("themekey")
                                                        themeKey.InnerText = row
                                                        themeNode.AppendChild(themeKey)
                                                    Next
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        Next
                    End If

                    '''''''''''''''''''''
                    If ctl.Name.Contains("tabCtrlDSPlaceKeywords") Then
                        deleteChildren(xmlMDOutput, "metadata/idinfo/keywords", "place")

                        Dim tabList As New List(Of TabPage)
                        For Each iTab In tabCtrlDSPlaceKeywords.TabPages
                            If iTab.enabled Then
                                tabList.Add(iTab)
                            End If
                        Next

                        For Each iTab In tabList
                            If nodeExists(xmlMDOutput, "metadata/idinfo/keywords") = False Then
                                AddNode(xmlMDOutput, "metadata/idinfo/keywords")
                            End If
                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/idinfo/keywords")

                            For Each subCtl As Control In iTab.Controls
                                If subCtl.Name.Contains("txtPlaceKeyThesaurus") Then

                                    If subCtl.Text <> "" Then
                                        Dim placeNode As XmlNode = xmlMDOutput.CreateElement("place")
                                        Dim placeKT As XmlNode = xmlMDOutput.CreateElement("placekt")

                                        stub.AppendChild(placeNode)
                                        placeNode.PrependChild(placeKT)
                                        placeKT.InnerText = subCtl.Text

                                        For Each iSubCtl As Control In iTab.Controls
                                            If iSubCtl.Name.Contains("dgvPlaceKeywords") Then

                                                Dim RowList As List(Of String) = getDGVList(iSubCtl)
                                                If RowList.Count > 0 Then

                                                    For Each row In RowList
                                                        Dim placeKey As XmlNode = xmlMDOutput.CreateElement("placekey")
                                                        placeKey.InnerText = row
                                                        placeNode.AppendChild(placeKey)
                                                    Next
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        Next
                    End If

                    ''''''''''''''''''''''''''''
                    If ctl.Name = "tabCtrlDSSourceInputs" Then
                        deleteChildren(xmlMDOutput, "metadata/dataqual/lineage", "srcinfo")

                        Dim tabList As New List(Of TabPage)
                        For Each iTab In tabCtrlDSSourceInputs.TabPages
                            If iTab.enabled Then
                                tabList.Add(iTab)
                            End If
                        Next

                        Dim tabNum As Integer
                        tabNum = 0 'This will be used to grab multi-dates from input XML, when applicable. Count starts at '0' for Xpath index. UPDATE: Tag used instead.
                        For Each iTab In tabList

                            'Debug.Print(CStr(tabNum))
                            'Debug.Print(CStr(iTab.Tag))

                            If nodeExists(xmlMDOutput, "metadata/dataqual/lineage") = False Then
                                AddNode(xmlMDOutput, "metadata/dataqual/lineage")
                            End If
                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/dataqual/lineage")


                            Dim srcInfoNode As XmlNode = xmlMDOutput.CreateElement("srcinfo")
                            stub.AppendChild(srcInfoNode)

                            Dim srcCiteNode As XmlNode = xmlMDOutput.CreateElement("srccite")
                            srcInfoNode.AppendChild(srcCiteNode)

                            Dim typeSrcNode As XmlNode = xmlMDOutput.CreateElement("typesrc")
                            typeSrcNode.InnerText = "Digital and/or Hardcopy Resources"
                            'This will be used for all records. This element is antiquated, to say the least.
                            srcInfoNode.AppendChild(typeSrcNode)

                            Dim citeInfoNode As XmlNode = xmlMDOutput.CreateElement("citeinfo")
                            srcCiteNode.AppendChild(citeInfoNode)

                            Dim pubInfoNode As XmlNode = xmlMDOutput.CreateElement("pubinfo")
                            citeInfoNode.AppendChild(pubInfoNode)

                            Dim srcTimeNode As XmlNode = xmlMDOutput.CreateElement("srctime")
                            srcInfoNode.AppendChild(srcTimeNode)

                            Dim timeInfoNode As XmlNode = xmlMDOutput.CreateElement("timeinfo")
                            srcTimeNode.AppendChild(timeInfoNode)

                            For Each subCtl As Control In iTab.Controls
                                If TypeOf subCtl Is TextBox Or TypeOf subCtl Is ComboBox Then

                                    If subCtl.Text <> "" Then
                                        If subCtl.Name.Contains("Originator") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("origin")
                                            citeInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("PubDate") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("pubdate")
                                            citeInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("Title") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("title")
                                            citeInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("DataType") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("geoform")
                                            citeInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("PubPlace") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("pubplace")
                                            pubInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("Publisher") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("publish")
                                            pubInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("Abbreviation") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("srccitea")
                                            srcInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If

                                        If subCtl.Name.Contains("Contribution") Then
                                            Dim newNode As XmlNode = xmlMDOutput.CreateElement("srccontr")
                                            srcInfoNode.AppendChild(newNode)
                                            newNode.InnerText = subCtl.Text
                                        End If
                                    End If

                                End If

                                If subCtl.Name.Contains("dgvSource") Then
                                    Dim RowList As List(Of String) = getDGVList(subCtl)
                                    For Each row In RowList
                                        Dim newNode As XmlNode = xmlMDOutput.CreateElement("onlink")
                                        newNode.InnerText = row
                                        citeInfoNode.AppendChild(newNode)
                                    Next
                                End If

                                '---------
                                If subCtl.Name.Contains("gbxSource") Then

                                    Dim BeginDate As String = ""
                                    Dim EndDate As String = ""

                                    For Each gbxSubCtl As Control In subCtl.Controls

                                        If TypeOf gbxSubCtl Is TextBox Or TypeOf gbxSubCtl Is ComboBox Then

                                            'Debug.Print(CStr(gbxSubCtl.Name))

                                            If gbxSubCtl.Name.Contains("BeginDate") Then
                                                BeginDate = gbxSubCtl.Text
                                            End If

                                            If gbxSubCtl.Name.Contains("EndDate") Then
                                                EndDate = gbxSubCtl.Text
                                            End If

                                            If gbxSubCtl.Name.Contains("CurrentnessRef") Then
                                                Dim newNode As XmlNode = xmlMDOutput.CreateElement("srccurr")
                                                newNode.InnerText = gbxSubCtl.Text
                                                srcTimeNode.AppendChild(newNode)
                                            End If

                                        End If
                                    Next


                                    If BeginDate = "Multiple Dates. XML will not be changed" Then
                                        Try
                                            Dim targetNode As XmlNode = getNode(xmlMD, "metadata/dataqual/lineage")
                                            'Dim subTargetNode As XmlNode = targetNode.ChildNodes.Item(tabNum)
                                            Dim subTargetNode As XmlNode = targetNode.ChildNodes.Item(iTab.Tag) 'Should be robust to dictionary-style ordering in "For each" loop.
                                            Dim multiDateNode As XmlNode = xmlMDOutput.CreateElement("mdattim")
                                            timeInfoNode.AppendChild(multiDateNode)

                                            Dim multiDateList As Collection = getMultiNodeValuesAtNodeInstance(subTargetNode, "srctime/timeinfo/mdattim/sngdate/caldate")

                                            If multiDateList.Count > 0 Then
                                                For Each iDate In multiDateList
                                                    Dim sngDateNode As XmlNode = xmlMDOutput.CreateElement("sngdate")
                                                    Dim calDateNode As XmlNode = xmlMDOutput.CreateElement("caldate")
                                                    multiDateNode.AppendChild(sngDateNode)
                                                    sngDateNode.AppendChild(calDateNode)
                                                    calDateNode.InnerText = iDate
                                                Next
                                            End If

                                        Catch ex As Exception
                                        End Try

                                    ElseIf BeginDate = EndDate Then
                                        Dim sngDateNode As XmlNode = xmlMDOutput.CreateElement("sngdate")
                                        Dim calDateNode As XmlNode = xmlMDOutput.CreateElement("caldate")
                                        timeInfoNode.AppendChild(sngDateNode)
                                        sngDateNode.AppendChild(calDateNode)
                                        calDateNode.InnerText = BeginDate

                                    Else
                                        Dim rngDateNode As XmlNode = xmlMDOutput.CreateElement("rngdates")
                                        Dim begDateNode As XmlNode = xmlMDOutput.CreateElement("begdate")
                                        Dim endDateNode As XmlNode = xmlMDOutput.CreateElement("enddate")
                                        timeInfoNode.AppendChild(rngDateNode)
                                        rngDateNode.AppendChild(begDateNode)
                                        rngDateNode.AppendChild(endDateNode)
                                        begDateNode.InnerText = BeginDate
                                        endDateNode.InnerText = EndDate

                                    End If
                                End If
                                '--------
                            Next

                            tabNum = tabNum + 1

                        Next
                    End If

                    '''''''''''''''''''''''''''
                    If ctl.Name = "tabCtrlDSProcessSteps" Then
                        deleteChildren(xmlMDOutput, "metadata/dataqual/lineage", "procstep")

                        Dim tabList As New List(Of TabPage)
                        For Each iTab In tabCtrlDSProcessSteps.TabPages
                            If iTab.enabled Then
                                tabList.Add(iTab)
                            End If
                        Next

                        For Each iTab In tabList

                            If nodeExists(xmlMDOutput, "metadata/dataqual/lineage") = False Then
                                AddNode(xmlMDOutput, "metadata/dataqual/lineage")
                            End If
                            Dim stub As XmlNode = getNode(xmlMDOutput, "metadata/dataqual/lineage")

                            Dim procStepNode As XmlNode = xmlMDOutput.CreateElement("procstep")
                            stub.AppendChild(procStepNode)


                            For Each subCtl As Control In iTab.Controls
                                If TypeOf subCtl Is TextBox Or TypeOf subCtl Is ComboBox Then

                                    If subCtl.Name.Contains("txtProcessStep") Then
                                        Dim processDescNode As XmlNode = xmlMDOutput.CreateElement("procdesc")
                                        processDescNode.InnerText = subCtl.Text
                                        procStepNode.AppendChild(processDescNode)
                                    End If

                                    If subCtl.Name.Contains("txtProcessDate") Then
                                        Dim processDateNode As XmlNode = xmlMDOutput.CreateElement("procdate")
                                        processDateNode.InnerText = subCtl.Text
                                        procStepNode.AppendChild(processDateNode)
                                    End If

                                End If
                            Next

                        Next
                    End If

                    '''''''''''''''''''''''''''

                Else
                    'Not enabled.... eliminate node.
                    'Check "Kill Paths"... some elements need to be removed higher up from their tags...
                    'OR simply kill all empty or disabled controls and write/use a removeDanglingNodesRoutine

                    'DI UPDATE: Issue of empty nodes should be handled by not writing out any VB.NET txt or cbo form element that is not enabled or
                    'is empty. DGV's should be handled by only writing out content for each row (empty DGVs will have no rows).
                    'In the case of the 'Source Inputs' tab control, a user could activate it and leave it empty; this would cause empty
                    'XML branches to be created. Using the BDP config file with MP on the Save routine erases these. A 'no prune' version is also included
                    'in the Resources location and could be used if needed.
                End If
            End If

        Loop Until ctl Is Nothing


    End Sub

    '"Save and Close" Button
    Private Sub SaveAndCloseXML(sender As System.Object, e As System.EventArgs) Handles btnSaveAndClose.Click

        Save()
        Me.Dispose()

    End Sub

    'Preview Metadata Record Button
    Private Sub btnPreviewMetadata_Click(sender As System.Object, e As System.EventArgs) Handles btnPreviewMetadata.Click

        Save()

        Dim startUpPath As String = Application.StartupPath
        Dim styleSheetPath As String = startUpPath & "\Resources\MetadataWizardStylesheet.xsl"

        Dim styleSheetReference As String = "href=" & Chr(34) & styleSheetPath & Chr(34) & " type=" & Chr(34) & "text/xsl" & Chr(34)


        'Add Stylesheet to Ouput XML when previewing for first time.
        Dim newPI As XmlProcessingInstruction

        If sPreviewCount = 0 Then
            newPI = xmlMDOutput.CreateProcessingInstruction("xml-stylesheet", styleSheetReference)
            xmlMDOutput.InsertBefore(newPI, xmlMDOutput.SelectSingleNode("metadata"))
            sPreviewCount = sPreviewCount + 1
        End If

        xmlMDOutput.Save(sOutFile) 'Save the file with a direct system save. The other 'Save()' routine calls MP and removes the stylesheet, which we don't want to do in this case.

        Dim previewForm As New MD_previewer
        previewForm.WebBrowser_Preview.Navigate(New Uri(sOutFile))
        previewForm.Show()



        'DI: The code below uses Internet Explorer or user-provided browser location to open the XML Preview.
        'The code above that uses the built-in VB web browser is more consistently reliable.

        'Dim p As New ProcessStartInfo
        'p.FileName = sIExplorerPath
        'p.Arguments = sOutFile

        'Try
        '    Process.Start(p)
        'Catch ex As Exception
        'End Try

    End Sub

#End Region


    Private Sub btnImportDSContactToDistContact_Click(sender As System.Object, e As System.EventArgs) Handles btnImportDSContactToDistContact.Click

        txtDSDistOrg.Text = ""
        txtDSDistPerson.Text = ""
        txtDSDistPersonTitle.Text = ""
        txtDSDistPhone.Text = ""
        txtDSDistFax.Text = ""
        txtDSDistEmail.Text = ""

        txtDSDistAddress1.Text = ""
        txtDSDistAddress2.Text = ""
        txtDSDistAddress3.Text = ""
        txtDSDistCity.Text = ""
        txtDSDistState.Text = ""
        txtDSDistZip.Text = ""
        txtDSDistCountry.Text = ""

        Save()

        'Handle the "Contact Person Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper") Then
            Dim DSDistPerson As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper")
            txtDSDistPerson.Text = DSDistPerson
            Dim DSDistOrg As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntorg")
            txtDSDistOrg.Text = DSDistOrg
        End If

        'Handle the "Contact Organization Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg") Then
            Dim DSDistOrg As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg")
            txtDSDistOrg.Text = DSDistOrg
            Dim DSDistPerson As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntper")
            txtDSDistPerson.Text = DSDistPerson
        End If

        Dim DSDistPersonTitle As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntpos")
        txtDSDistPersonTitle.Text = DSDistPersonTitle

        Dim DSDistPhone As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntvoice")
        txtDSDistPhone.Text = DSDistPhone

        Dim DSDistFax As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntfax")
        txtDSDistFax.Text = DSDistFax

        Dim DSDistEmail As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntemail")
        txtDSDistEmail.Text = DSDistEmail

        Dim DSDistAddresses As Collection
        DSDistAddresses = getMultiNodeValues(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/address")

        If DSDistAddresses.Count > 0 Then
            Try
                Dim DSDistAddress1 As String = DSDistAddresses(1)
                txtDSDistAddress1.Text = DSDistAddress1
                Dim DSDistAddress2 As String = DSDistAddresses(2)
                txtDSDistAddress2.Text = DSDistAddress2
                Dim DSDistAddress3 As String = DSDistAddresses(3)
                txtDSDistAddress3.Text = DSDistAddress3
            Catch ex As Exception
            End Try
        End If


        Dim DSDistCity As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/city")
        txtDSDistCity.Text = DSDistCity

        Dim DSDistState As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/state")
        txtDSDistState.Text = DSDistState

        Dim DSDistZip As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/postal")
        txtDSDistZip.Text = DSDistZip

        Dim DSDistCountry As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/country")
        txtDSDistCountry.Text = DSDistCountry

        Dim DSDistAddressType As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/addrtype")
        cboDSDistAddressType.Text = DSDistAddressType

    End Sub

    Private Sub btnImportMetaContactToDistContact_Click(sender As System.Object, e As System.EventArgs) Handles btnImportMetaContactToDistContact.Click

        txtDSDistOrg.Text = ""
        txtDSDistPerson.Text = ""
        txtDSDistPersonTitle.Text = ""
        txtDSDistPhone.Text = ""
        txtDSDistFax.Text = ""
        txtDSDistEmail.Text = ""

        txtDSDistAddress1.Text = ""
        txtDSDistAddress2.Text = ""
        txtDSDistAddress3.Text = ""
        txtDSDistCity.Text = ""
        txtDSDistState.Text = ""
        txtDSDistZip.Text = ""
        txtDSDistCountry.Text = ""

        Save()

        'Handle the "Contact Person Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntperp/cntper") Then
            Dim DSDistPerson As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntperp/cntper")
            txtDSDistPerson.Text = DSDistPerson
            Dim DSDistOrg As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntperp/cntorg")
            txtDSDistOrg.Text = DSDistOrg
        End If

        'Handle the "Contact Organization Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntorgp/cntorg") Then
            Dim DSDistOrg As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntorgp/cntorg")
            txtDSDistOrg.Text = DSDistOrg
            Dim DSDistPerson As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntorgp/cntper")
            txtDSDistPerson.Text = DSDistPerson
        End If

        Dim DSDistPersonTitle As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntpos")
        txtDSDistPersonTitle.Text = DSDistPersonTitle

        Dim DSDistPhone As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntvoice")
        txtDSDistPhone.Text = DSDistPhone

        Dim DSDistFax As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntfax")
        txtDSDistFax.Text = DSDistFax

        Dim DSDistEmail As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntemail")
        txtDSDistEmail.Text = DSDistEmail

        Dim DSDistAddresses As Collection
        DSDistAddresses = getMultiNodeValues(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/address")

        If DSDistAddresses.Count > 0 Then
            Try
                Dim DSDistAddress1 As String = DSDistAddresses(1)
                txtDSDistAddress1.Text = DSDistAddress1
                Dim DSDistAddress2 As String = DSDistAddresses(2)
                txtDSDistAddress2.Text = DSDistAddress2
                Dim DSDistAddress3 As String = DSDistAddresses(3)
                txtDSDistAddress3.Text = DSDistAddress3
            Catch ex As Exception
            End Try
        End If


        Dim DSDistCity As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/city")
        txtDSDistCity.Text = DSDistCity

        Dim DSDistState As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/state")
        txtDSDistState.Text = DSDistState

        Dim DSDistZip As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/postal")
        txtDSDistZip.Text = DSDistZip

        Dim DSDistCountry As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/country")
        txtDSDistCountry.Text = DSDistCountry

        Dim DSDistAddressType As String = getNodeText(xmlMDOutput, "metadata/metainfo/metc/cntinfo/cntaddr/addrtype")
        cboDSDistAddressType.Text = DSDistAddressType

    End Sub

    Private Sub btnImportDSContactToMetaContact_Click(sender As System.Object, e As System.EventArgs) Handles btnImportDSContactToMetaContact.Click

        txtMetaContactPerson.Text = ""
        txtMetaContactOrg.Text = ""
        txtMetaContactPersonTitle.Text = ""
        txtMetaContactPhone.Text = ""
        txtMetaContactFax.Text = ""
        txtMetaContactEmail.Text = ""

        txtMetaContactAddress1.Text = ""
        txtMetaContactAddress2.Text = ""
        txtMetaContactAddress3.Text = ""
        txtMetaContactCity.Text = ""
        txtMetaContactState.Text = ""
        txtMetaContactZip.Text = ""
        txtMetaContactCountry.Text = ""
        cboMetaContactAddressType.Text = ""

        Save()

        'Handle the "Contact Person Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper") Then
            Dim MetaContactPerson As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntper")
            txtMetaContactPerson.Text = MetaContactPerson
            Dim MetaContactOrg As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntperp/cntorg")
            txtMetaContactOrg.Text = MetaContactOrg
        End If

        'Handle the "Contact Organization Primary" possibility
        If nodeExists(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg") Then
            Dim MetaContactOrg As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntorg")
            txtMetaContactOrg.Text = MetaContactOrg
            Dim MetaContactPerson As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntorgp/cntper")
            txtMetaContactPerson.Text = MetaContactPerson
        End If

        Dim MetaContactPersonTitle As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntpos")
        txtMetaContactPersonTitle.Text = MetaContactPersonTitle

        Dim MetaContactPhone As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntvoice")
        txtMetaContactPhone.Text = MetaContactPhone

        Dim MetaContactFax As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntfax")
        txtMetaContactFax.Text = MetaContactFax

        Dim MetaContactEmail As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntemail")
        txtMetaContactEmail.Text = MetaContactEmail

        Dim MetaContactAddresses As Collection
        MetaContactAddresses = getMultiNodeValues(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/address")

        If MetaContactAddresses.Count > 0 Then
            Try
                Dim MetaContactAddress1 As String = MetaContactAddresses(1)
                txtMetaContactAddress1.Text = MetaContactAddress1
                Dim MetaContactAddress2 As String = MetaContactAddresses(2)
                txtMetaContactAddress2.Text = MetaContactAddress2
                Dim MetaContactAddress3 As String = MetaContactAddresses(3)
                txtMetaContactAddress3.Text = MetaContactAddress3
            Catch ex As Exception
            End Try
        End If


        Dim MetaContactCity As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/city")
        txtMetaContactCity.Text = MetaContactCity

        Dim MetaContactState As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/state")
        txtMetaContactState.Text = MetaContactState

        Dim MetaContactZip As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/postal")
        txtMetaContactZip.Text = MetaContactZip

        Dim MetaContactCountry As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/country")
        txtMetaContactCountry.Text = MetaContactCountry

        Dim MetaContactAddressType As String = getNodeText(xmlMDOutput, "metadata/idinfo/ptcontac/cntinfo/cntaddr/addrtype")
        cboMetaContactAddressType.Text = MetaContactAddressType

    End Sub


    Private Sub btnUSGSLookupDSContact_Click(sender As System.Object, e As System.EventArgs) Handles btnUSGSLookupDSContact.Click
        Dim oHTTP As Object
        Dim bGetAsAsync As Boolean
        Dim xmlContact As New XmlDocument

        'Try to obtain contact info from P. Schweitzer's USGS database...

        oHTTP = CreateObject("Microsoft.XMLHTTP")

        bGetAsAsync = False
        Dim inputEmailAddress As String = txtUSGSLookupDSContact.Text
        Dim lookupContact As String = inputEmailAddress.Replace("@usgs.gov", "")

        'oHTTP.open("GET", "http://geo-nsdi.er.usgs.gov/contact-xml.php?email=dignizio", bGetAsAsync) ...EXAMPLE...
        oHTTP.open("GET", ("http://geo-nsdi.er.usgs.gov/contact-xml.php?email=" & lookupContact), bGetAsAsync)
        oHTTP.send()

        Try
            Dim xmlString As String = (oHTTP.responseText)
            xmlContact.LoadXml(xmlString)

            If getNodeText(xmlContact, "cntinfo/cntemail") = "" Or getNodeText(xmlContact, "cntinfo/cntemail") Is Nothing Then
                Throw New ApplicationException
            End If

            'Clear any existing content...
            txtDSContactPerson.Text = ""
            txtDSContactOrg.Text = ""
            txtDSContactPersonTitle.Text = ""
            txtDSContactPhone.Text = ""
            txtDSContactFax.Text = ""
            txtDSContactEmail.Text = ""
            txtDSContactAddress1.Text = ""
            txtDSContactAddress2.Text = ""
            txtDSContactAddress3.Text = ""
            txtDSContactCity.Text = ""
            txtDSContactState.Text = ""
            txtDSContactZip.Text = ""
            txtDSContactCountry.Text = ""
            cboDSContactAddressType.Text = ""

            'Parse the new contact info...
            txtDSContactPerson.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntper")
            txtDSContactOrg.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntorg")
            txtDSContactPersonTitle.Text = getNodeText(xmlContact, "cntinfo/cntpos")
            txtDSContactPhone.Text = getNodeText(xmlContact, "cntinfo/cntvoice")
            txtDSContactFax.Text = getNodeText(xmlContact, "cntinfo/cntfax")
            txtDSContactEmail.Text = getNodeText(xmlContact, "cntinfo/cntemail")

            Dim DSContactAddresses As Collection
            DSContactAddresses = getMultiNodeValues(xmlContact, "cntinfo/cntaddr/address")

            If DSContactAddresses.Count > 0 Then
                Try
                    Dim DSContactAddress1 As String = DSContactAddresses(1)
                    txtDSContactAddress1.Text = DSContactAddress1
                    Dim DSContactAddress2 As String = DSContactAddresses(2)
                    txtDSContactAddress2.Text = DSContactAddress2
                    Dim DSContactAddress3 As String = DSContactAddresses(3)
                    txtDSContactAddress3.Text = DSContactAddress3
                Catch ex As Exception
                End Try
            End If


            txtDSContactCity.Text = getNodeText(xmlContact, "cntinfo/cntaddr/city")
            txtDSContactState.Text = getNodeText(xmlContact, "cntinfo/cntaddr/state")
            txtDSContactZip.Text = getNodeText(xmlContact, "cntinfo/cntaddr/postal")
            txtDSContactCountry.Text = getNodeText(xmlContact, "cntinfo/cntaddr/country")
            cboDSContactAddressType.Text = getNodeText(xmlContact, "cntinfo/cntaddr/addrtype")

        Catch ex As Exception
            MsgBox("The Metadata Wizard was unable to locate the provided email address in the USGS directory. Please ensure the email contact is valid before importing the contact or enter the information manually.")

        End Try

        'MsgBox(xmlString)
        txtUSGSLookupDSContact.Text = ""

    End Sub

    Private Sub btnUSGSLookupMetaContact_Click(sender As System.Object, e As System.EventArgs) Handles btnUSGSLookupMetaContact.Click

        Dim oHTTP As Object
        Dim bGetAsAsync As Boolean
        Dim xmlContact As New XmlDocument

        'Try to obtain contact info from P. Schweitzer's USGS database...

        oHTTP = CreateObject("Microsoft.XMLHTTP")

        bGetAsAsync = False
        Dim inputEmailAddress As String = txtUSGSLookupMetaContact.Text
        Dim lookupContact As String = inputEmailAddress.Replace("@usgs.gov", "")

        'oHTTP.open("GET", "http://geo-nsdi.er.usgs.gov/contact-xml.php?email=dignizio", bGetAsAsync) ...EXAMPLE...
        oHTTP.open("GET", ("http://geo-nsdi.er.usgs.gov/contact-xml.php?email=" & lookupContact), bGetAsAsync)
        oHTTP.send()
        Try
            Dim xmlString As String = (oHTTP.responseText)
            xmlContact.LoadXml(xmlString)

            If getNodeText(xmlContact, "cntinfo/cntemail") = "" Or getNodeText(xmlContact, "cntinfo/cntemail") Is Nothing Then
                Throw New ApplicationException
            End If

            'Clear any existing content...
            txtMetaContactPerson.Text = ""
            txtMetaContactOrg.Text = ""
            txtMetaContactPersonTitle.Text = ""
            txtMetaContactPhone.Text = ""
            txtMetaContactFax.Text = ""
            txtMetaContactEmail.Text = ""
            txtMetaContactAddress1.Text = ""
            txtMetaContactAddress2.Text = ""
            txtMetaContactAddress3.Text = ""
            txtMetaContactCity.Text = ""
            txtMetaContactState.Text = ""
            txtMetaContactZip.Text = ""
            txtMetaContactCountry.Text = ""
            cboMetaContactAddressType.Text = ""

            'Parse the new contact info...
            txtMetaContactPerson.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntper")
            txtMetaContactOrg.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntorg")
            txtMetaContactPersonTitle.Text = getNodeText(xmlContact, "cntinfo/cntpos")
            txtMetaContactPhone.Text = getNodeText(xmlContact, "cntinfo/cntvoice")
            txtMetaContactFax.Text = getNodeText(xmlContact, "cntinfo/cntfax")
            txtMetaContactEmail.Text = getNodeText(xmlContact, "cntinfo/cntemail")

            Dim MetaContactAddresses As Collection
            MetaContactAddresses = getMultiNodeValues(xmlContact, "cntinfo/cntaddr/address")

            If MetaContactAddresses.Count > 0 Then
                Try
                    Dim MetaContactAddress1 As String = MetaContactAddresses(1)
                    txtMetaContactAddress1.Text = MetaContactAddress1
                    Dim MetaContactAddress2 As String = MetaContactAddresses(2)
                    txtMetaContactAddress2.Text = MetaContactAddress2
                    Dim MetaContactAddress3 As String = MetaContactAddresses(3)
                    txtMetaContactAddress3.Text = MetaContactAddress3
                Catch ex As Exception
                End Try
            End If


            txtMetaContactCity.Text = getNodeText(xmlContact, "cntinfo/cntaddr/city")
            txtMetaContactState.Text = getNodeText(xmlContact, "cntinfo/cntaddr/state")
            txtMetaContactZip.Text = getNodeText(xmlContact, "cntinfo/cntaddr/postal")
            txtMetaContactCountry.Text = getNodeText(xmlContact, "cntinfo/cntaddr/country")
            cboMetaContactAddressType.Text = getNodeText(xmlContact, "cntinfo/cntaddr/addrtype")

        Catch ex As Exception
            MsgBox("The Metadata Wizard was unable to locate the provided email address in the USGS directory. Please ensure the email contact is valid before importing the contact or enter the information manually.")

        End Try

        'MsgBox(xmlString)
        txtUSGSLookupMetaContact.Text = ""

    End Sub

    Private Sub btnUSGSLookupDistContact_Click(sender As System.Object, e As System.EventArgs) Handles btnUSGSLookupDistContact.Click

        Dim oHTTP As Object
        Dim bGetAsAsync As Boolean
        Dim xmlContact As New XmlDocument

        'Try to obtain contact info from P. Schweitzer's USGS database...

        oHTTP = CreateObject("Microsoft.XMLHTTP")

        bGetAsAsync = False
        Dim inputEmailAddress As String = txtUSGSLookupDistContact.Text
        Dim lookupContact As String = inputEmailAddress.Replace("@usgs.gov", "")

        'oHTTP.open("GET", "http://geo-nsdi.er.usgs.gov/contact-xml.php?email=dignizio", bGetAsAsync) ...EXAMPLE...
        oHTTP.open("GET", ("http://geo-nsdi.er.usgs.gov/contact-xml.php?email=" & lookupContact), bGetAsAsync)
        oHTTP.send()
        Try
            Dim xmlString As String = (oHTTP.responseText)
            xmlContact.LoadXml(xmlString)

            If getNodeText(xmlContact, "cntinfo/cntemail") = "" Or getNodeText(xmlContact, "cntinfo/cntemail") Is Nothing Then
                Throw New ApplicationException
            End If

            'Clear any existing content...
            txtDSDistPerson.Text = ""
            txtDSDistOrg.Text = ""
            txtDSDistPersonTitle.Text = ""
            txtDSDistPhone.Text = ""
            txtDSDistFax.Text = ""
            txtDSDistEmail.Text = ""
            txtDSDistAddress1.Text = ""
            txtDSDistAddress2.Text = ""
            txtDSDistAddress3.Text = ""
            txtDSDistCity.Text = ""
            txtDSDistState.Text = ""
            txtDSDistZip.Text = ""
            txtDSDistCountry.Text = ""
            cboDSDistAddressType.Text = ""

            'Parse the new contact info...
            txtDSDistPerson.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntper")
            txtDSDistOrg.Text = getNodeText(xmlContact, "cntinfo/cntperp/cntorg")
            txtDSDistPersonTitle.Text = getNodeText(xmlContact, "cntinfo/cntpos")
            txtDSDistPhone.Text = getNodeText(xmlContact, "cntinfo/cntvoice")
            txtDSDistFax.Text = getNodeText(xmlContact, "cntinfo/cntfax")
            txtDSDistEmail.Text = getNodeText(xmlContact, "cntinfo/cntemail")

            Dim DSDistAddresses As Collection
            DSDistAddresses = getMultiNodeValues(xmlContact, "cntinfo/cntaddr/address")

            If DSDistAddresses.Count > 0 Then
                Try
                    Dim DSDistAddress1 As String = DSDistAddresses(1)
                    txtDSDistAddress1.Text = DSDistAddress1
                    Dim DSDistAddress2 As String = DSDistAddresses(2)
                    txtDSDistAddress2.Text = DSDistAddress2
                    Dim DSDistAddress3 As String = DSDistAddresses(3)
                    txtDSDistAddress3.Text = DSDistAddress3
                Catch ex As Exception
                End Try
            End If


            txtDSDistCity.Text = getNodeText(xmlContact, "cntinfo/cntaddr/city")
            txtDSDistState.Text = getNodeText(xmlContact, "cntinfo/cntaddr/state")
            txtDSDistZip.Text = getNodeText(xmlContact, "cntinfo/cntaddr/postal")
            txtDSDistCountry.Text = getNodeText(xmlContact, "cntinfo/cntaddr/country")
            cboDSDistAddressType.Text = getNodeText(xmlContact, "cntinfo/cntaddr/addrtype")
        Catch ex As Exception
            MsgBox("The Metadata Wizard was unable to locate the provided email address in the USGS directory. Please ensure the email contact is valid before importing the contact or enter the information manually.")

        End Try

        'MsgBox(xmlString)
        txtUSGSLookupDistContact.Text = ""

    End Sub


#Region "Notes/Scratch"
    Public Sub showHelp(xPath)
        MsgBox(xPath)
    End Sub

    Public Sub changeValue(xPath, value)
        MsgBox("insert code to change " + xPath + " for " + value)
    End Sub

    Public Sub changeValueList(xPath, value)
        MsgBox("insert code to change " + xPath)
    End Sub

    Public Function checkDate(value) As Boolean
        If IsNumeric(value) Then
            Return True
        Else
            Return False

        End If
    End Function

    'Public Function loadData()
    '    For Each cntrl As Control In Me.Controls

    '#If cntrl.enabled = true

    '        If TypeOf (cntrl) Is TextControl Or TypeOf (cntrl) Is comboControl Then
    '            cntrl.value = getNodeText(xmlMD, cntrl.xpath)
    '        End If
    '    Next

    '    For Each cntrl As Control In Me.Controls
    '        If TypeOf (cntrl) Is TextControl Or TypeOf (cntrl) Is Data Then
    '            cntrl.value = getNodeText(xmlMD, cntrl.xpath)
    '        End If
    '    Next

    '   if the world is square then
    '        txtCrazy.text = getNodeText(xmlMD, "some/random/xpath")
    '   end if

    'End Function

    'Public Function saveData()
    '    For Each cntrl As Control In Me.Controls
    '        If TypeOf (cntrl) Is TextControl Or TypeOf (cntrl) Is comboControl Then
    '            If cntrl.validit Then
    '                setNodeText(xmlMD, cntrl.xpath, cntrl.value)
    '            End If
    '    Next
    'End Function




    ''something like this eliminate having to have two separate populate tab routines
    'MsgBox(Me.Controls.ContainsKey("txtTopicKeyThesaurus 2"))
    'Dim tmpCtrl As Control
    ''write a custom getControlByName routine... then call it
    'tmpCtrl = getControlByName("name") 
    'tmpCtrl.Text = "blah"



#End Region

#Region "Utilities"


    Private Sub cloneSourceInputTab(tabCt As Integer)

        'tabCtrlDSSourceInputs.Controls.Add(New TabPage("Source Input " & Str(tabCt)))
        'Option below solves issue of numbers being out of order if user has multiple tabs and deletes several, then adds more
        tabCtrlDSSourceInputs.Controls.Add(New TabPage("Source Input"))

        Dim tp As TabPage = tabCtrlDSSourceInputs.TabPages(tabCt - 1)
        tp.Tag = (tabCt - 1) 'Keep track of which item, in order (1-n) in the input XML the particular source info element corresponds with.

        txtSourceTitle = New TextBox()
        With txtSourceTitle
            .Name = ("txtSource" & CStr(tabCt) & "Title")
            .Location = txtSource1Title.Location
            .Size = txtSource1Title.Size
            .Tag = txtSource1Title.Tag
        End With

        txtSourcePublisher = New TextBox()
        With txtSourcePublisher
            .Name = ("txtSource" & CStr(tabCt) & "Publisher")
            .Location = txtSource1Publisher.Location
            .Size = txtSource1Publisher.Size
            .Tag = txtSource1Publisher.Tag
        End With

        cboSourceDataType = New ComboBox()
        With cboSourceDataType
            .Name = ("cboSource" & CStr(tabCt) & "DataType")
            .Location = cboSource1DataType.Location
            .Size = cboSource1DataType.Size
            .Tag = cboSource1DataType.Tag
            For Each dType As Object In cboSource1DataType.Items
                .Items.Add(dType.ToString())
            Next dType
        End With

        txtSourcePubDate = New TextBox()
        With txtSourcePubDate
            .Name = ("txtSource" & CStr(tabCt) & "PubDate")
            .Location = txtSource1PubDate.Location
            .Size = txtSource1PubDate.Size
            .Tag = txtSource1PubDate.Tag
        End With

        txtSourceOriginator = New TextBox()
        With txtSourceOriginator
            .Name = ("txtSource" & CStr(tabCt) & "Originator")
            .Location = txtSource1Originator.Location
            .Size = txtSource1Originator.Size
            .Tag = txtSource1Originator.Tag
        End With

        txtSourcePubPlace = New TextBox()
        With txtSourcePubPlace
            .Name = ("txtSource" & CStr(tabCt) & "PubPlace")
            .Location = txtSource1PubPlace.Location
            .Size = txtSource1PubPlace.Size
            .Tag = txtSource1PubPlace.Tag
        End With

        dgvSourceOnlineLink = New DataGridView()
        With dgvSourceOnlineLink
            .Name = ("dgvSource" & CStr(tabCt) & "OnlineLink")
            .Location = dgvSource1OnlineLink.Location
            .Size = dgvSource1OnlineLink.Size
            .Tag = dgvSource1OnlineLink.Tag
            .BackgroundColor = System.Drawing.SystemColors.Control
            .DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Window
            .DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlText
            .BorderStyle = BorderStyle.None
            .RowHeadersVisible = False
            .Columns.Add("OnlineLink", "URL(s) List individually on each line.")
            .Columns(0).Width = 220
        End With



        '------------------------------------------
        gbxSourceTimePeriodInfo = New GroupBox()
        txtSourceBeginDate = New TextBox()
        txtSourceEndDate = New TextBox()
        cboSourceCurrentnessRef = New ComboBox
        labSourceBeginDate = New Label()
        labSourceEndDate = New Label()
        labSourceCurrentnessRef = New Label()


        With txtSourceBeginDate
            .Name = ("txtSource" & CStr(tabCt) & "BeginDate")
            .Location = txtSource1BeginDate.Location
            .Size = txtSource1BeginDate.Size
            .Tag = txtSource1BeginDate.Tag
        End With

        With txtSourceEndDate
            .Name = ("txtSource" & CStr(tabCt) & "EndDate")
            .Location = txtSource1EndDate.Location
            .Size = txtSource1EndDate.Size
            .Tag = txtSource1EndDate.Tag
        End With

        With cboSourceCurrentnessRef
            .Name = ("cboSource" & CStr(tabCt) & "CurrentnessRef")
            .Location = cboSource1CurrentnessRef.Location
            .Size = cboSource1CurrentnessRef.Size
            .Tag = cboSource1CurrentnessRef.Tag
            For Each cReference As Object In cboSource1CurrentnessRef.Items
                .Items.Add(cReference.ToString())
            Next cReference
        End With

        With labSourceBeginDate
            .Text = labSource1BeginDate.Text
            .Location = labSource1BeginDate.Location
            .Size = labSource1BeginDate.Size
        End With

        With labSourceEndDate
            .Text = labSource1EndDate.Text
            .Location = labSource1EndDate.Location
            .Size = labSource1EndDate.Size
        End With

        With labSourceCurrentnessRef
            .Text = labSource1CurrentnessRef.Text
            .Location = labSource1CurrentnessRef.Location
            .Size = labSource1CurrentnessRef.Size
        End With

        lab101 = New Label()
        With lab101
            .Text = "*"
            .Location = labSourceInputReq101.Location
            .Size = labSourceInputReq101.Size
            .ForeColor = labSourceInputReq101.ForeColor
            .Font = labSourceInputReq101.Font
        End With

        lab102 = New Label()
        With lab102
            .Text = "*"
            .Location = labSourceInputReq102.Location
            .Size = labSourceInputReq102.Size
            .ForeColor = labSourceInputReq102.ForeColor
            .Font = labSourceInputReq102.Font
        End With

        lab103 = New Label()
        With lab103
            .Text = "*"
            .Location = labSourceInputReq103.Location
            .Size = labSourceInputReq103.Size
            .ForeColor = labSourceInputReq103.ForeColor
            .Font = labSourceInputReq103.Font
        End With

        With gbxSourceTimePeriodInfo
            .Location = gbxSource1TimePeriodInfo.Location
            .Size = gbxSource1TimePeriodInfo.Size
            .Name = ("gbxSource" & CStr(tabCt) & "TimePeriodInfo")
            .Text = gbxSource1TimePeriodInfo.Text

            .Controls.Add(txtSourceBeginDate)
            .Controls.Add(txtSourceEndDate)
            .Controls.Add(cboSourceCurrentnessRef)

            .Controls.Add(labSourceBeginDate)
            .Controls.Add(labSourceEndDate)
            .Controls.Add(labSourceCurrentnessRef)
            .Controls.Add(lab101)
            .Controls.Add(lab102)
            .Controls.Add(lab103)
        End With
        '------------------------------------------



        txtSourceAbbreviation = New TextBox()
        With txtSourceAbbreviation
            .Name = ("txtSource" & CStr(tabCt) & "Abbreviation")
            .Location = txtSource1Abbreviation.Location
            .Size = txtSource1Abbreviation.Size
            .Tag = txtSource1Abbreviation.Tag
            .ForeColor = Color.SlateGray
            .Text = ("Source Input" & Str(tabCt))
        End With

        txtSourceContribution = New TextBox()
        With txtSourceContribution
            .Name = ("txtSource" & CStr(tabCt) & "Contribution")
            .Location = txtSource1Contribution.Location
            .Size = txtSource1Contribution.Size
            .Tag = txtSource1Contribution.Tag
            .ForeColor = Color.SlateGray
            .Text = "Source information used in support of the development of the data set."
        End With



        Label1 = New Label()
        With Label1
            .Text = "Title of Source Input"
            .Location = labSource1Title.Location
            .Size = labSource1Title.Size
        End With

        Label2 = New Label()
        With Label2
            .Text = "Publisher / Data Provider"
            .Location = labSource1Publisher.Location
            .Size = labSource1Publisher.Size
        End With

        Label3 = New Label()
        With Label3
            .Text = "Type of Data"
            .Location = labSource1DataType.Location
            .Size = labSource1DataType.Size
        End With

        Label4 = New Label()
        With Label4
            .Text = "Publication Date (YYYYMMDD)"
            .Location = labSource1PubDate.Location
            .Size = labSource1PubDate.Size
        End With

        Label5 = New Label()
        With Label5
            .Text = "Author / Originator of Source Input"
            .Location = labSource1Originator.Location
            .Size = labSource1Originator.Size
        End With

        Label6 = New Label()
        With Label6
            .Text = "Publication Place"
            .Location = labSource1PubPlace.Location
            .Size = labSource1PubPlace.Size
        End With

        Label7 = New Label()
        With Label7
            .Text = "URL Link or GIS Service to Data"
            .Location = labSource1OnlineLink.Location
            .Size = labSource1OnlineLink.Size
        End With

        Label8 = New Label()
        With Label8
            .Text = "Abbreviation / Short Name for the Input Data     (Update as Needed)"
            .Location = labSource1Abbreviation.Location
            .Size = labSource1Abbreviation.Size
        End With

        Label9 = New Label()
        With Label9
            .Text = "Contribution of Source Input     (Update as Needed)"
            .Location = labSource1Contribution.Location
            .Size = labSource1Contribution.Size
        End With

        Label10 = New Label()
        With Label10
            .Text = "*"
            .Location = labSourceInputReq1.Location
            .Size = labSourceInputReq1.Size
            .ForeColor = labSourceInputReq1.ForeColor
            .Font = labSourceInputReq1.Font
        End With

        Label11 = New Label()
        With Label11
            .Text = "*"
            .Location = labSourceInputReq2.Location
            .Size = labSourceInputReq2.Size
            .ForeColor = labSourceInputReq2.ForeColor
            .Font = labSourceInputReq2.Font
        End With

        Label12 = New Label()
        With Label12
            .Text = "*"
            .Location = labSourceInputReq3.Location
            .Size = labSourceInputReq3.Size
            .ForeColor = labSourceInputReq3.ForeColor
            .Font = labSourceInputReq3.Font
        End With

        Label13 = New Label()
        With Label13
            .Text = "*"
            .Location = labSourceInputReq4.Location
            .Size = labSourceInputReq4.Size
            .ForeColor = labSourceInputReq4.ForeColor
            .Font = labSourceInputReq4.Font
        End With

        Label14 = New Label()
        With Label14
            .Text = "*"
            .Location = labSourceInputReq5.Location
            .Size = labSourceInputReq5.Size
            .ForeColor = labSourceInputReq5.ForeColor
            .Font = labSourceInputReq5.Font
        End With

        Label15 = New Label()
        With Label15
            .Text = "*"
            .Location = labSourceInputReq6.Location
            .Size = labSourceInputReq6.Size
            .ForeColor = labSourceInputReq6.ForeColor
            .Font = labSourceInputReq6.Font
        End With

        Label16 = New Label()
        With Label16
            .Text = "*"
            .Location = labSourceInputReq7.Location
            .Size = labSourceInputReq7.Size
            .ForeColor = labSourceInputReq7.ForeColor
            .Font = labSourceInputReq7.Font
        End With

        Label17 = New Label()
        With Label17
            .Text = "*"
            .Location = labSourceInputReq8.Location
            .Size = labSourceInputReq8.Size
            .ForeColor = labSourceInputReq8.ForeColor
            .Font = labSourceInputReq8.Font
        End With

        tp.Controls.Add(txtSourceTitle)
        tp.Controls.Add(txtSourcePublisher)
        tp.Controls.Add(cboSourceDataType)
        tp.Controls.Add(txtSourcePubDate)
        tp.Controls.Add(txtSourceOriginator)
        tp.Controls.Add(txtSourcePubPlace)
        tp.Controls.Add(dgvSourceOnlineLink)

        tp.Controls.Add(gbxSourceTimePeriodInfo)

        tp.Controls.Add(txtSourceAbbreviation)
        tp.Controls.Add(txtSourceContribution)

        tp.Controls.Add(Label1)
        tp.Controls.Add(Label2)
        tp.Controls.Add(Label3)
        tp.Controls.Add(Label4)
        tp.Controls.Add(Label5)
        tp.Controls.Add(Label6)
        tp.Controls.Add(Label7)
        tp.Controls.Add(Label8)
        tp.Controls.Add(Label9)
        tp.Controls.Add(Label10)
        tp.Controls.Add(Label11)
        tp.Controls.Add(Label12)
        tp.Controls.Add(Label13)
        tp.Controls.Add(Label14)
        tp.Controls.Add(Label15)
        tp.Controls.Add(Label16)
        tp.Controls.Add(Label17)
    End Sub

    Private Sub populateSourceInput1(node As XmlNode, tabCt As Integer)
        Dim Source1Title As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/title")
        txtSource1Title.Text = Source1Title

        Dim Source1Publisher As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubinfo/publish")
        txtSource1Publisher.Text = Source1Publisher


        Dim Source1DataType As String = getNodeTextAtNodeInstance(node, "typesrc")
        cboSource1DataType.Text = Source1DataType


        Dim Source1PubDate As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubdate")
        txtSource1PubDate.Text = Source1PubDate


        Dim Source1Originator As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/origin")
        txtSource1Originator.Text = Source1Originator


        Dim Source1PubPlace As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubinfo/pubplace")
        txtSource1PubPlace.Text = Source1PubPlace

        dgvSource1OnlineLink.DefaultCellStyle.BackColor = Color.White
        Try
            Dim Source1OnlineLinkList As Collection = getMultiNodeValuesAtNodeInstance(node, "srccite/citeinfo/onlink")

            For Each iLink In Source1OnlineLinkList
                dgvSource1OnlineLink.Rows.Add(iLink)
            Next iLink
        Catch ex As Exception
        End Try

        If nodeInstanceExists(node, "srctime/timeinfo/sngdate/caldate") Then
            Dim Source1BeginDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/sngdate/caldate")
            txtSource1BeginDate.Text = Source1BeginDate
            Dim Source1EndDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/sngdate/caldate")
            txtSource1EndDate.Text = Source1EndDate
        End If
        If nodeInstanceExists(node, "srctime/timeinfo/rngdates") Then
            Dim Source1BeginDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/rngdates/begdate")
            txtSource1BeginDate.Text = Source1BeginDate
            Dim Source1EndDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/rngdates/enddate")
            txtSource1EndDate.Text = Source1EndDate
        End If
        If nodeInstanceExists(node, "srctime/timeinfo/mdattim") Then
            txtSource1BeginDate.Text = "Multiple Dates. XML will not be changed"
            txtSource1BeginDate.Enabled = False
            txtSource1EndDate.Text = "Multiple Dates. XML will not be changed"
            txtSource1EndDate.Enabled = False
        End If

        Dim Source1CurrentnessRef As String = getNodeTextAtNodeInstance(node, "srctime/srccurr")
        cboSource1CurrentnessRef.Text = Source1CurrentnessRef

        Try
            Dim Source1Abbreviation As String = node.SelectSingleNode("srccitea").FirstChild.Value
            txtSource1Abbreviation.Text = Source1Abbreviation
            txtSource1Abbreviation.ForeColor = Color.Black
        Catch ex As Exception
            txtSource1Abbreviation.ForeColor = Color.SlateGray
            txtSource1Abbreviation.Text = ("Source Input" & Str(tabCt))
        End Try

        Try
            Dim Source1Contribution As String = node.SelectSingleNode("srccontr").FirstChild.Value
            txtSource1Contribution.Text = Source1Contribution
            txtSource1Contribution.ForeColor = Color.Black
        Catch ex As Exception
            txtSource1Contribution.ForeColor = Color.SlateGray
            txtSource1Contribution.Text = "Source information used in support of the development of the data set."
        End Try
    End Sub

    Private Sub populateAdditionalSourceInput(node As XmlNode, tabCt As Integer)

        Dim SourceTitle As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/title")
        txtSourceTitle.Text = SourceTitle

        Dim SourcePublisher As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubinfo/publish")
        txtSourcePublisher.Text = SourcePublisher


        Dim SourceDataType As String = getNodeTextAtNodeInstance(node, "typesrc")
        cboSourceDataType.Text = SourceDataType


        Dim SourcePubDate As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubdate")
        txtSourcePubDate.Text = SourcePubDate


        Dim SourceOriginator As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/origin")
        txtSourceOriginator.Text = SourceOriginator


        Dim SourcePubPlace As String = getNodeTextAtNodeInstance(node, "srccite/citeinfo/pubinfo/pubplace")
        txtSourcePubPlace.Text = SourcePubPlace

        dgvSourceOnlineLink.DefaultCellStyle.BackColor = Color.White
        Try
            Dim SourceOnlineLinkList As Collection = getMultiNodeValuesAtNodeInstance(node, "srccite/citeinfo/onlink")

            For Each iLink In SourceOnlineLinkList
                dgvSourceOnlineLink.Rows.Add(iLink)
            Next iLink
        Catch ex As Exception
        End Try

        If nodeInstanceExists(node, "srctime/timeinfo/sngdate/caldate") Then
            Dim SourceBeginDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/sngdate/caldate")
            txtSourceBeginDate.Text = SourceBeginDate
            Dim SourceEndDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/sngdate/caldate")
            txtSourceEndDate.Text = SourceEndDate
        End If
        If nodeInstanceExists(node, "srctime/timeinfo/rngdates") Then
            Dim SourceBeginDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/rngdates/begdate")
            txtSourceBeginDate.Text = SourceBeginDate
            Dim SourceEndDate As String = getNodeTextAtNodeInstance(node, "srctime/timeinfo/rngdates/begdate")
            txtSourceEndDate.Text = SourceEndDate
        End If
        If nodeInstanceExists(node, "srctime/timeinfo/mdattim") Then
            txtSourceBeginDate.Text = "Multiple Dates. XML will not be changed"
            txtSourceBeginDate.Enabled = False
            txtSourceEndDate.Text = "Multiple Dates. XML will not be changed"
            txtSourceEndDate.Enabled = False
        End If

        Dim SourceCurrentnessRef As String = getNodeTextAtNodeInstance(node, "srctime/srccurr")
        cboSourceCurrentnessRef.Text = SourceCurrentnessRef

        Try
            Dim SourceAbbreviation As String = node.SelectSingleNode("srccitea").FirstChild.Value
            txtSourceAbbreviation.Text = SourceAbbreviation
            txtSourceAbbreviation.ForeColor = Color.Black
        Catch ex As Exception
            txtSourceAbbreviation.ForeColor = Color.SlateGray
            txtSourceAbbreviation.Text = ("Source Input" & Str(tabCt))
        End Try

        Try
            Dim SourceContribution As String = node.SelectSingleNode("srccontr").FirstChild.Value
            txtSourceContribution.Text = SourceContribution
            txtSourceContribution.ForeColor = Color.Black
        Catch ex As Exception
            txtSourceContribution.ForeColor = Color.SlateGray
            txtSourceContribution.Text = "Source information used in support of the development of the data set."
        End Try
    End Sub


    Private Sub cloneProcessStepTab(tabCt As Integer)
        'tabCtrlDSProcessSteps.Controls.Add(New TabPage("Process Step " & Str(tabCt)))
        tabCtrlDSProcessSteps.Controls.Add(New TabPage("Process Step"))

        Dim tp As TabPage = tabCtrlDSProcessSteps.TabPages(tabCt - 1)

        txtProcessStep = New TextBox()
        With txtProcessStep
            .Name = ("txtProcessStep" & CStr(tabCt))
            .Location = txtProcessStep1.Location
            .Size = txtProcessStep1.Size
            .ScrollBars = ScrollBars.Vertical
            .Multiline = True
        End With

        txtProcessDate = New TextBox()
        With txtProcessDate
            .Name = ("txtProcessDate" & CStr(tabCt))
            .Location = txtProcessDate1.Location
            .Size = txtProcessDate1.Size
        End With

        Label1 = New Label()
        With Label1
            .Text = "Describe the processing step or method below:"
            .Location = labProcessStep1.Location
            .Size = labProcessStep1.Size
        End With

        Label2 = New Label()
        With Label2
            .Text = "Date (YYYYMMDD)"
            .Location = labProcessDate1.Location
            .Size = labProcessDate1.Size
        End With

        Label3 = New Label()
        With Label3
            .Text = "*"
            .Location = labProcessStepReq1.Location
            .Size = labProcessStepReq1.Size
            .ForeColor = labProcessStepReq1.ForeColor
            .Font = labProcessStepReq1.Font
        End With

        Label4 = New Label()
        With Label4
            .Text = "*"
            .Location = labProcessStepReq2.Location
            .Size = labProcessStepReq2.Size
            .ForeColor = labProcessStepReq2.ForeColor
            .Font = labProcessStepReq2.Font
        End With

        tp.Controls.Add(txtProcessStep)
        tp.Controls.Add(txtProcessDate)
        tp.Controls.Add(Label1)
        tp.Controls.Add(Label2)
        tp.Controls.Add(Label3)
        tp.Controls.Add(Label4)

    End Sub

    Private Sub populateProcessStep1(node As XmlNode, tabCt As Integer)

        Dim ProcessStep As String = getNodeTextAtNodeInstance(node, "procdesc")
        If ProcessStep IsNot Nothing Then
            txtProcessStep1.Text = ProcessStep
            txtProcessStep1.ForeColor = Color.Black
        End If

        Dim ProcessDate As String = getNodeTextAtNodeInstance(node, "procdate")
        If ProcessDate IsNot Nothing Then
            txtProcessDate1.Text = ProcessDate
            txtProcessDate1.ForeColor = Color.Black
        End If

    End Sub

    Private Sub populateAdditionalProcessStep(node As XmlNode, tabCt As Integer)

        Dim ProcessStep As String = getNodeTextAtNodeInstance(node, "procdesc")
        txtProcessStep.Text = ProcessStep
        txtProcessStep.ForeColor = Color.Black

        Dim ProcessDate As String = getNodeTextAtNodeInstance(node, "procdate")
        txtProcessDate.Text = ProcessDate
        txtProcessDate.ForeColor = Color.Black

    End Sub



    Private Sub clonePlaceKeywordsTab(tabCt As Integer)

        'tabCtrlDSPlaceKeywords.Controls.Add(New TabPage("Place Keywords"))
        'Dim tp As TabPage = tabCtrlDSPlaceKeywords.TabPages(tabCt - 1)

        Dim tp As New TabPage
        tp.Name = "tabPlaceKeywords" & CStr(tabCt)
        tp.Text = "Place Keywords"

        txtPlaceKeyThesaurus = New TextBox()
        With txtPlaceKeyThesaurus
            .Name = ("txtPlaceKeyThesaurus" & CStr(tabCt))
            .Location = txtPlaceKeyThesaurus1.Location
            .Size = txtPlaceKeyThesaurus1.Size
        End With

        labPlaceKeyTip = New Label()
        With labPlaceKeyTip
            .Text = labPlaceKeyTip1.Text
            .Location = labPlaceKeyTip1.Location
            .Size = labPlaceKeyTip1.Size
            .ForeColor = labPlaceKeyTip1.ForeColor
            .Font = labPlaceKeyTip1.Font
        End With

        Label1 = New Label()
        With Label1
            .Text = "List place keywords below:"
            .Location = labListPlaceKeywords1.Location
            .Size = labListPlaceKeywords1.Size
            .ForeColor = labListPlaceKeywords1.ForeColor
            .Font = labListPlaceKeywords1.Font
        End With

        Label2 = New Label()
        With Label2
            .Text = "Keyword Thesaurus"
            .Location = labPlaceKeyThesaurus1.Location
            .Size = labPlaceKeyThesaurus1.Size
            .ForeColor = labPlaceKeyThesaurus1.ForeColor
            .Font = labPlaceKeyThesaurus1.Font
        End With

        Label3 = New Label()
        With Label3
            .Text = "*"
            .Location = labPlaceKeywordReq1.Location
            .Size = labPlaceKeywordReq1.Size
            .ForeColor = labPlaceKeywordReq1.ForeColor
            .Font = labPlaceKeywordReq1.Font
        End With

        Label4 = New Label()
        With Label4
            .Text = "*"
            .Location = labPlaceKeywordReq2.Location
            .Size = labPlaceKeywordReq2.Size
            .ForeColor = labPlaceKeywordReq2.ForeColor
            .Font = labPlaceKeywordReq2.Font
        End With

        '-------------------------

        gbxPlaceKeyReqNote = New GroupBox()
        labAdditionalPlaceKeyReqNote1 = New Label()
        labAdditionalPlaceKeyReqNote2 = New Label()


        With labAdditionalPlaceKeyReqNote1
            .Text = labPlaceKeyReqNote1.Text
            .Location = labPlaceKeyReqNote1.Location
            .Size = labPlaceKeyReqNote1.Size
            .ForeColor = labPlaceKeyReqNote1.ForeColor
            .Font = labPlaceKeyReqNote1.Font
        End With

        With labAdditionalPlaceKeyReqNote2
            .Text = labPlaceKeyReqNote2.Text
            .Location = labPlaceKeyReqNote2.Location
            .Size = labPlaceKeyReqNote2.Size
            .ForeColor = labPlaceKeyReqNote2.ForeColor
            .Font = labPlaceKeyReqNote2.Font
        End With

        With gbxPlaceKeyReqNote
            .Location = gbxPlaceKeyReqNote1.Location
            .Size = gbxPlaceKeyReqNote1.Size
            .Text = gbxPlaceKeyReqNote1.Text

            .Controls.Add(labAdditionalPlaceKeyReqNote1)
            .Controls.Add(labAdditionalPlaceKeyReqNote2)
        End With
        '-------------------------

        dgvPlaceKeywords = New DataGridView()
        With dgvPlaceKeywords
            .Name = ("dgvPlaceKeywords" & Str(tabCt))
            .Location = dgvPlaceKeywords1.Location
            .Size = dgvPlaceKeywords1.Size
            .BackgroundColor = System.Drawing.SystemColors.Control
            .DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Window
            .DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlText
            .BorderStyle = dgvPlaceKeywords1.BorderStyle
            .RowHeadersVisible = False
            .Columns.Add("PlaceKeywords", "Enter one keyword per line.")
            .Columns(0).Width = dgvPlaceKeywords1.Columns(0).Width
        End With

        tp.Controls.Add(labPlaceKeyTip)

        tp.Controls.Add(txtPlaceKeyThesaurus)
        tp.Controls.Add(Label1)
        tp.Controls.Add(Label2)
        tp.Controls.Add(Label3)
        tp.Controls.Add(Label4)
        tp.Controls.Add(gbxPlaceKeyReqNote)
        tp.Controls.Add(dgvPlaceKeywords)

        tabCtrlDSPlaceKeywords.Controls.Add(tp)

    End Sub

    Private Sub populatePlaceKeywords1(node As XmlNode, tabCt As Integer)

        If nodeInstanceExists(node, "placekt") Then
            Dim PlaceKeyThesaurus As String = getNodeTextAtNodeInstance(node, "placekt")
            txtPlaceKeyThesaurus1.Text = PlaceKeyThesaurus
        Else
            txtPlaceKeyThesaurus1.Text = "None"
        End If

        'dgvPlaceKeywords1.DefaultCellStyle.BackColor = Color.White
        Try
            Dim PlaceKeywordsList As Collection = getMultiNodeValuesAtNodeInstance(node, "placekey")

            For Each iWord In PlaceKeywordsList
                dgvPlaceKeywords1.Rows.Add(iWord)
            Next iWord

        Catch ex As Exception
        End Try

    End Sub

    Private Sub populateAdditionalPlaceKeywords(node As XmlNode, tabCt As Integer)

        If nodeInstanceExists(node, "placekt") Then
            Dim PlaceKeyThesaurus As String = getNodeTextAtNodeInstance(node, "placekt")
            txtPlaceKeyThesaurus.Text = PlaceKeyThesaurus
        Else
            txtPlaceKeyThesaurus.Text = "None"
        End If

        'dgvPlaceKeywords1.DefaultCellStyle.BackColor = Color.White
        Try
            Dim PlaceKeywordsList As Collection = getMultiNodeValuesAtNodeInstance(node, "placekey")

            For Each iWord In PlaceKeywordsList
                dgvPlaceKeywords.Rows.Add(iWord)
            Next iWord

        Catch ex As Exception
        End Try
    End Sub



    Private Sub cloneTopicKeywordsTab(tabCt As Integer)

        'tabCtrlDSTopicKeywords.Controls.Add(New TabPage("Theme Keywords"))
        'Dim tp As TabPage = tabCtrlDSTopicKeywords.TabPages(tabCt - 1)

        Dim tp As New TabPage
        tp.Name = "tabTopicKeywords" & CStr(tabCt)
        tp.Text = "Theme Keywords"


        txtTopicKeyThesaurus = New TextBox()
        With txtTopicKeyThesaurus
            .Name = ("txtTopicKeyThesaurus" & CStr(tabCt))
            .Location = txtTopicKeyThesaurus1.Location
            .Size = txtTopicKeyThesaurus1.Size
        End With

        labTopicKeyTip = New Label()
        With labTopicKeyTip
            .Text = labTopicKeyTip1.Text
            .Location = labTopicKeyTip1.Location
            .Size = labTopicKeyTip1.Size
            .ForeColor = labTopicKeyTip1.ForeColor
            .Font = labTopicKeyTip1.Font
        End With

        Label1 = New Label()
        With Label1
            .Text = "List topic keywords below:"
            .Location = labListTopicKeywords1.Location
            .Size = labListTopicKeywords1.Size
            .ForeColor = labListTopicKeywords1.ForeColor
            .Font = labListTopicKeywords1.Font
        End With

        Label2 = New Label()
        With Label2
            .Text = "Keyword Thesaurus"
            .Location = labTopicKeyThesaurus1.Location
            .Size = labTopicKeyThesaurus1.Size
            .ForeColor = labTopicKeyThesaurus1.ForeColor
            .Font = labTopicKeyThesaurus1.Font
        End With

        Label3 = New Label()
        With Label3
            .Text = "*"
            .Location = labTopicKeywordReq1.Location
            .Size = labTopicKeywordReq1.Size
            .ForeColor = labTopicKeywordReq1.ForeColor
            .Font = labTopicKeywordReq1.Font
        End With

        Label4 = New Label()
        With Label4
            .Text = "*"
            .Location = labTopicKeywordReq2.Location
            .Size = labTopicKeywordReq2.Size
            .ForeColor = labTopicKeywordReq2.ForeColor
            .Font = labTopicKeywordReq2.Font
        End With


        dgvTopicKeywords = New DataGridView()
        With dgvTopicKeywords
            .Name = ("dgvTopicKeywords" & CStr(tabCt))
            .Location = dgvTopicKeywords1.Location
            .Size = dgvTopicKeywords1.Size
            .BackgroundColor = System.Drawing.SystemColors.Control
            .BorderStyle = dgvTopicKeywords1.BorderStyle
            .DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Window
            .DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlText
            .RowHeadersVisible = False
            .Columns.Add("TopicKeywords", "Enter one keyword per line.")
            .Columns(0).Width = dgvTopicKeywords1.Columns(0).Width
        End With

        tp.Controls.Add(labTopicKeyTip)

        tp.Controls.Add(txtTopicKeyThesaurus)
        tp.Controls.Add(Label1)
        tp.Controls.Add(Label2)
        tp.Controls.Add(Label3)
        tp.Controls.Add(Label4)

        tp.Controls.Add(dgvTopicKeywords)

        tabCtrlDSTopicKeywords.Controls.Add(tp)

    End Sub

    Private Sub populateTopicKeywords1(node As XmlNode, tabCt As Integer)

        If nodeInstanceExists(node, "themekt") Then
            Dim TopicKeyThesaurus As String = getNodeTextAtNodeInstance(node, "themekt")
            txtTopicKeyThesaurus1.Text = TopicKeyThesaurus
        Else
            txtTopicKeyThesaurus1.Text = "None"
        End If

        Try
            Dim TopicKeywordsList As Collection = getMultiNodeValuesAtNodeInstance(node, "themekey")

            For Each iWord In TopicKeywordsList
                dgvTopicKeywords1.Rows.Add(iWord)
            Next iWord

        Catch ex As Exception
        End Try

    End Sub

    Private Sub populateAdditionalTopicKeywords(node As XmlNode, tabCt As Integer)

        If nodeInstanceExists(node, "themekt") Then
            Dim TopicKeyThesaurus As String = getNodeTextAtNodeInstance(node, "themekt")
            txtTopicKeyThesaurus.Text = TopicKeyThesaurus
        Else
            txtTopicKeyThesaurus.Text = "None"
        End If

        Try
            Dim TopicKeywordsList As Collection = getMultiNodeValuesAtNodeInstance(node, "themekey")

            For Each iWord In TopicKeywordsList
                dgvTopicKeywords.Rows.Add(iWord)
            Next iWord

        Catch ex As Exception
        End Try

    End Sub


    Public Function getDGVList(ctl As DataGridView)
        Try
            Dim RowList As New List(Of String)
            For Each row As DataGridViewRow In CType(ctl, DataGridView).Rows

                If Not row.IsNewRow And Not CStr((row.Cells(0).Value)) = "" Then
                    RowList.Add(row.Cells(0).Value)
                End If
            Next
            Return RowList
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


#End Region





    Private Sub btnAddProcessStep_Click(sender As System.Object, e As System.EventArgs) Handles btnAddProcessStep.Click

        Dim currentTabNumber As Integer = (tabCtrlDSProcessSteps.TabCount + 1)
        cloneProcessStepTab(currentTabNumber)

    End Sub

    Private Sub btnDeleteProcessStep_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteProcessStep.Click
        Dim targetPage As Integer = tabCtrlDSProcessSteps.SelectedIndex

        If tabCtrlDSProcessSteps.TabCount = 1 Then
            Dim tab As TabPage = tabCtrlDSProcessSteps.SelectedTab
            ClearTabContents(tab)
            tab.Enabled = False

        Else
            tabCtrlDSProcessSteps.TabPages.RemoveAt(targetPage)
        End If
    End Sub


    Private Sub btnAddNewSourceInput_Click(sender As System.Object, e As System.EventArgs) Handles btnAddNewSourceInput.Click

        Dim currentTabNumber As Integer = (tabCtrlDSSourceInputs.TabCount + 1)
        cloneSourceInputTab(currentTabNumber)

    End Sub

    Private Sub btnDeleteSourceInput_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteSourceInput.Click
        Dim targetPage As Integer = tabCtrlDSSourceInputs.SelectedIndex

        If tabCtrlDSSourceInputs.SelectedTab.Name = "tabSourceInfo1" Then
            Dim tab As TabPage = tabCtrlDSSourceInputs.SelectedTab
            ClearTabContents(tab)
            dgvSource1OnlineLink.DefaultCellStyle.BackColor = Color.WhiteSmoke
            tab.Enabled = False

        Else
            tabCtrlDSSourceInputs.TabPages.RemoveAt(targetPage)
        End If

    End Sub


    Private Sub btnAddPlaceKeywordSet_Click(sender As System.Object, e As System.EventArgs) Handles btnAddPlaceKeywordSet.Click

        Dim currentTabNumber As Integer = (tabCtrlDSPlaceKeywords.TabCount + 1)
        clonePlaceKeywordsTab(currentTabNumber)

    End Sub

    Private Sub btnDeletePlaceKeywordSet_Click(sender As System.Object, e As System.EventArgs) Handles btnDeletePlaceKeywordSet.Click

        Dim targetPage As Integer = tabCtrlDSPlaceKeywords.SelectedIndex

        If tabCtrlDSPlaceKeywords.SelectedTab.Name = "tabPlaceKeywords1" Then
            Dim tab As TabPage = tabCtrlDSPlaceKeywords.SelectedTab
            ClearTabContents(tab)
            dgvPlaceKeywords1.DefaultCellStyle.BackColor = Color.WhiteSmoke
            tab.Enabled = False

        Else
            tabCtrlDSPlaceKeywords.TabPages.RemoveAt(targetPage)
        End If
    End Sub


    Private Sub btnAddTopicKeywordSet_Click(sender As System.Object, e As System.EventArgs) Handles btnAddTopicKeywordSet.Click
        Dim currentTabNumber As Integer = (tabCtrlDSTopicKeywords.TabCount + 1)
        cloneTopicKeywordsTab(currentTabNumber)
    End Sub

    Private Sub btnDeleteTopicKeywordSet_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteTopicKeywordSet.Click
        Dim targetPage As Integer = tabCtrlDSTopicKeywords.SelectedIndex

        If tabCtrlDSTopicKeywords.TabCount = 1 Then

            Dim tab As TabPage = tabCtrlDSTopicKeywords.SelectedTab
            ClearTabContents(tab)
            For Each dgv In tab.Controls.OfType(Of DataGridView)()
                dgv.DefaultCellStyle.BackColor = Color.WhiteSmoke
                tab.Enabled = False
            Next
        Else
            tabCtrlDSTopicKeywords.TabPages.RemoveAt(targetPage)
        End If
    End Sub


    Private Sub ClearTabContents(tab As TabPage)

        For Each tb In tab.Controls.OfType(Of TextBox)()
            tb.Clear()
        Next
        For Each cb In tab.Controls.OfType(Of ComboBox)()
            cb.Text = ""
        Next
        For Each dgv In tab.Controls.OfType(Of DataGridView)()
            dgv.Rows.Clear()
        Next

        For Each gbx In tab.Controls.OfType(Of GroupBox)()
            For Each tb In gbx.Controls.OfType(Of TextBox)()
                tb.Clear()
            Next
            For Each cb In gbx.Controls.OfType(Of ComboBox)()
                cb.Text = ""
            Next
            For Each dgv In gbx.Controls.OfType(Of DataGridView)()
                dgv.Rows.Clear()
            Next
        Next
    End Sub

End Class

