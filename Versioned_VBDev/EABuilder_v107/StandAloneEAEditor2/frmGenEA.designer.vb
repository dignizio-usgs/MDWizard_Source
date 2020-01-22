<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGenEA
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGenEA))
        Me.cmdSaveAndClose = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.lblLocation = New System.Windows.Forms.Label()
        Me.splitContainer_Main = New System.Windows.Forms.SplitContainer()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtEadetcit = New System.Windows.Forms.TextBox()
        Me.lstFields = New System.Windows.Forms.ListView()
        Me.Attribute = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TypeX = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.txtOverview = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.frmDomain = New System.Windows.Forms.GroupBox()
        Me.optEnum = New System.Windows.Forms.RadioButton()
        Me.optUnrep = New System.Windows.Forms.RadioButton()
        Me.optRange = New System.Windows.Forms.RadioButton()
        Me.optCodeset = New System.Windows.Forms.RadioButton()
        Me.txtAttDefSource = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtAttDef = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.pnlRange = New System.Windows.Forms.Panel()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.labelVATwarning = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtRngResolution = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtRngUnits = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtRngMax = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtRngMin = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.pnlUnrepresentable = New System.Windows.Forms.Panel()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtUnrep = New System.Windows.Forms.TextBox()
        Me.pnlCodeset = New System.Windows.Forms.Panel()
        Me.txtCodesetSource = New System.Windows.Forms.TextBox()
        Me.cboCodesetName = New System.Windows.Forms.ComboBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlEnumerated = New System.Windows.Forms.Panel()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtValDefSource = New System.Windows.Forms.TextBox()
        Me.txtValDef = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lstUniqueVals = New System.Windows.Forms.ListBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.cmdSave = New System.Windows.Forms.Button()
        CType(Me.splitContainer_Main, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer_Main.Panel1.SuspendLayout()
        Me.splitContainer_Main.Panel2.SuspendLayout()
        Me.splitContainer_Main.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.frmDomain.SuspendLayout()
        Me.pnlRange.SuspendLayout()
        Me.pnlUnrepresentable.SuspendLayout()
        Me.pnlCodeset.SuspendLayout()
        Me.pnlEnumerated.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdSaveAndClose
        '
        Me.cmdSaveAndClose.Location = New System.Drawing.Point(359, 595)
        Me.cmdSaveAndClose.Name = "cmdSaveAndClose"
        Me.cmdSaveAndClose.Size = New System.Drawing.Size(212, 23)
        Me.cmdSaveAndClose.TabIndex = 0
        Me.cmdSaveAndClose.Text = "Save and Close (Continue)"
        Me.cmdSaveAndClose.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(693, 594)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(212, 23)
        Me.cmdCancel.TabIndex = 1
        Me.cmdCancel.Text = "Cancel   (Close without saving)"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'lblLocation
        '
        Me.lblLocation.AutoSize = True
        Me.lblLocation.Location = New System.Drawing.Point(72, 18)
        Me.lblLocation.Name = "lblLocation"
        Me.lblLocation.Size = New System.Drawing.Size(0, 13)
        Me.lblLocation.TabIndex = 3
        '
        'splitContainer_Main
        '
        Me.splitContainer_Main.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.splitContainer_Main.Location = New System.Drawing.Point(12, 8)
        Me.splitContainer_Main.Name = "splitContainer_Main"
        '
        'splitContainer_Main.Panel1
        '
        Me.splitContainer_Main.Panel1.Controls.Add(Me.Label23)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.PictureBox1)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.Label14)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.txtEadetcit)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.lstFields)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.txtOverview)
        Me.splitContainer_Main.Panel1.Controls.Add(Me.Label2)
        '
        'splitContainer_Main.Panel2
        '
        Me.splitContainer_Main.Panel2.Controls.Add(Me.frmDomain)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.txtAttDefSource)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.Label4)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.txtAttDef)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.Label3)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.pnlCodeset)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.pnlEnumerated)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.pnlRange)
        Me.splitContainer_Main.Panel2.Controls.Add(Me.pnlUnrepresentable)
        Me.splitContainer_Main.Size = New System.Drawing.Size(942, 578)
        Me.splitContainer_Main.SplitterDistance = 314
        Me.splitContainer_Main.TabIndex = 7
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(92, 11)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(184, 30)
        Me.Label23.TabIndex = 7
        Me.Label23.Text = "Click Through Each Attribute and" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Review/Update Its Description"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(31, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(55, 58)
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(4, 490)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(255, 13)
        Me.Label14.TabIndex = 5
        Me.Label14.Text = "Citation (required if Overview Description is provided)"
        '
        'txtEadetcit
        '
        Me.txtEadetcit.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEadetcit.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtEadetcit.Location = New System.Drawing.Point(3, 506)
        Me.txtEadetcit.Multiline = True
        Me.txtEadetcit.Name = "txtEadetcit"
        Me.txtEadetcit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtEadetcit.Size = New System.Drawing.Size(304, 65)
        Me.txtEadetcit.TabIndex = 4
        Me.txtEadetcit.Text = resources.GetString("txtEadetcit.Text")
        '
        'lstFields
        '
        Me.lstFields.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstFields.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Attribute, Me.TypeX})
        Me.lstFields.FullRowSelect = True
        Me.lstFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstFields.HideSelection = False
        Me.lstFields.Location = New System.Drawing.Point(3, 67)
        Me.lstFields.MultiSelect = False
        Me.lstFields.Name = "lstFields"
        Me.lstFields.Size = New System.Drawing.Size(304, 286)
        Me.lstFields.TabIndex = 3
        Me.lstFields.UseCompatibleStateImageBehavior = False
        Me.lstFields.View = System.Windows.Forms.View.Details
        '
        'Attribute
        '
        Me.Attribute.Text = "Attribute (Field)"
        Me.Attribute.Width = 158
        '
        'TypeX
        '
        Me.TypeX.Text = "Type"
        Me.TypeX.Width = 142
        '
        'txtOverview
        '
        Me.txtOverview.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOverview.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtOverview.Location = New System.Drawing.Point(3, 375)
        Me.txtOverview.Multiline = True
        Me.txtOverview.Name = "txtOverview"
        Me.txtOverview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtOverview.Size = New System.Drawing.Size(304, 103)
        Me.txtOverview.TabIndex = 2
        Me.txtOverview.Text = resources.GetString("txtOverview.Text")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 359)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(157, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Overview Description  (optional)"
        '
        'frmDomain
        '
        Me.frmDomain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.frmDomain.Controls.Add(Me.optEnum)
        Me.frmDomain.Controls.Add(Me.optUnrep)
        Me.frmDomain.Controls.Add(Me.optRange)
        Me.frmDomain.Controls.Add(Me.optCodeset)
        Me.frmDomain.Location = New System.Drawing.Point(6, 158)
        Me.frmDomain.Name = "frmDomain"
        Me.frmDomain.Size = New System.Drawing.Size(611, 40)
        Me.frmDomain.TabIndex = 11
        Me.frmDomain.TabStop = False
        Me.frmDomain.Text = "Field Type"
        '
        'optEnum
        '
        Me.optEnum.AutoSize = True
        Me.optEnum.Location = New System.Drawing.Point(42, 16)
        Me.optEnum.Name = "optEnum"
        Me.optEnum.Size = New System.Drawing.Size(82, 17)
        Me.optEnum.TabIndex = 7
        Me.optEnum.TabStop = True
        Me.optEnum.Tag = "enum"
        Me.optEnum.Text = "Enumerated"
        Me.optEnum.UseVisualStyleBackColor = True
        '
        'optUnrep
        '
        Me.optUnrep.AutoSize = True
        Me.optUnrep.Location = New System.Drawing.Point(464, 16)
        Me.optUnrep.Name = "optUnrep"
        Me.optUnrep.Size = New System.Drawing.Size(103, 17)
        Me.optUnrep.TabIndex = 10
        Me.optUnrep.TabStop = True
        Me.optUnrep.Tag = "unrep"
        Me.optUnrep.Text = "Unrepresentable"
        Me.optUnrep.UseVisualStyleBackColor = True
        '
        'optRange
        '
        Me.optRange.AutoSize = True
        Me.optRange.Location = New System.Drawing.Point(197, 16)
        Me.optRange.Name = "optRange"
        Me.optRange.Size = New System.Drawing.Size(57, 17)
        Me.optRange.TabIndex = 8
        Me.optRange.TabStop = True
        Me.optRange.Tag = "range"
        Me.optRange.Text = "Range"
        Me.optRange.UseVisualStyleBackColor = True
        '
        'optCodeset
        '
        Me.optCodeset.AutoSize = True
        Me.optCodeset.Location = New System.Drawing.Point(327, 16)
        Me.optCodeset.Name = "optCodeset"
        Me.optCodeset.Size = New System.Drawing.Size(64, 17)
        Me.optCodeset.TabIndex = 9
        Me.optCodeset.TabStop = True
        Me.optCodeset.Tag = "codeset"
        Me.optCodeset.Text = "Codeset"
        Me.optCodeset.UseVisualStyleBackColor = True
        '
        'txtAttDefSource
        '
        Me.txtAttDefSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAttDefSource.Location = New System.Drawing.Point(140, 110)
        Me.txtAttDefSource.Multiline = True
        Me.txtAttDefSource.Name = "txtAttDefSource"
        Me.txtAttDefSource.Size = New System.Drawing.Size(477, 26)
        Me.txtAttDefSource.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 116)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(130, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Attribute Definition Source"
        '
        'txtAttDef
        '
        Me.txtAttDef.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAttDef.Location = New System.Drawing.Point(6, 26)
        Me.txtAttDef.Multiline = True
        Me.txtAttDef.Name = "txtAttDef"
        Me.txtAttDef.Size = New System.Drawing.Size(611, 78)
        Me.txtAttDef.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 11)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Attribute Definition"
        '
        'pnlRange
        '
        Me.pnlRange.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlRange.Controls.Add(Me.Label24)
        Me.pnlRange.Controls.Add(Me.labelVATwarning)
        Me.pnlRange.Controls.Add(Me.Label22)
        Me.pnlRange.Controls.Add(Me.Label21)
        Me.pnlRange.Controls.Add(Me.txtRngResolution)
        Me.pnlRange.Controls.Add(Me.Label12)
        Me.pnlRange.Controls.Add(Me.txtRngUnits)
        Me.pnlRange.Controls.Add(Me.Label11)
        Me.pnlRange.Controls.Add(Me.txtRngMax)
        Me.pnlRange.Controls.Add(Me.Label10)
        Me.pnlRange.Controls.Add(Me.txtRngMin)
        Me.pnlRange.Controls.Add(Me.Label8)
        Me.pnlRange.Location = New System.Drawing.Point(6, 208)
        Me.pnlRange.Name = "pnlRange"
        Me.pnlRange.Size = New System.Drawing.Size(611, 366)
        Me.pnlRange.TabIndex = 8
        Me.pnlRange.Tag = "range"
        Me.pnlRange.Visible = False
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(15, 125)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(430, 39)
        Me.Label24.TabIndex = 13
        Me.Label24.Text = resources.GetString("Label24.Text")
        '
        'labelVATwarning
        '
        Me.labelVATwarning.AutoSize = True
        Me.labelVATwarning.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelVATwarning.ForeColor = System.Drawing.SystemColors.Control
        Me.labelVATwarning.Location = New System.Drawing.Point(397, 26)
        Me.labelVATwarning.Name = "labelVATwarning"
        Me.labelVATwarning.Size = New System.Drawing.Size(211, 64)
        Me.labelVATwarning.TabIndex = 12
        Me.labelVATwarning.Text = "If this raster contains categorical " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "data please close this tool, build " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "a rast" & _
    "er attribute table for the data, " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "and then re-run this tool."
        '
        'Label22
        '
        Me.Label22.Location = New System.Drawing.Point(15, 214)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(524, 111)
        Me.Label22.TabIndex = 11
        Me.Label22.Text = resources.GetString("Label22.Text")
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(15, 192)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(102, 13)
        Me.Label21.TabIndex = 10
        Me.Label21.Text = """Range Domain"""
        '
        'txtRngResolution
        '
        Me.txtRngResolution.Location = New System.Drawing.Point(232, 93)
        Me.txtRngResolution.Name = "txtRngResolution"
        Me.txtRngResolution.Size = New System.Drawing.Size(159, 20)
        Me.txtRngResolution.TabIndex = 9
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(15, 96)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(172, 13)
        Me.Label12.TabIndex = 8
        Me.Label12.Text = "Measurement Resolution (Optional)"
        '
        'txtRngUnits
        '
        Me.txtRngUnits.Location = New System.Drawing.Point(232, 67)
        Me.txtRngUnits.Name = "txtRngUnits"
        Me.txtRngUnits.Size = New System.Drawing.Size(159, 20)
        Me.txtRngUnits.TabIndex = 7
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(15, 72)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(87, 13)
        Me.Label11.TabIndex = 6
        Me.Label11.Text = "Units of Measure"
        '
        'txtRngMax
        '
        Me.txtRngMax.Location = New System.Drawing.Point(232, 41)
        Me.txtRngMax.Name = "txtRngMax"
        Me.txtRngMax.Size = New System.Drawing.Size(159, 20)
        Me.txtRngMax.TabIndex = 5
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(15, 46)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(172, 13)
        Me.Label10.TabIndex = 4
        Me.Label10.Text = "Range Maximum (Numeric or Date)"
        '
        'txtRngMin
        '
        Me.txtRngMin.Location = New System.Drawing.Point(232, 15)
        Me.txtRngMin.Name = "txtRngMin"
        Me.txtRngMin.Size = New System.Drawing.Size(159, 20)
        Me.txtRngMin.TabIndex = 1
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(15, 20)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(169, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "Range Minimum (Numeric or Date)"
        '
        'pnlUnrepresentable
        '
        Me.pnlUnrepresentable.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.pnlUnrepresentable.Controls.Add(Me.Label18)
        Me.pnlUnrepresentable.Controls.Add(Me.Label16)
        Me.pnlUnrepresentable.Controls.Add(Me.Label15)
        Me.pnlUnrepresentable.Controls.Add(Me.txtUnrep)
        Me.pnlUnrepresentable.Location = New System.Drawing.Point(6, 208)
        Me.pnlUnrepresentable.Name = "pnlUnrepresentable"
        Me.pnlUnrepresentable.Size = New System.Drawing.Size(611, 366)
        Me.pnlUnrepresentable.TabIndex = 13
        Me.pnlUnrepresentable.Tag = "unrep"
        '
        'Label18
        '
        Me.Label18.Location = New System.Drawing.Point(12, 245)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(582, 80)
        Me.Label18.TabIndex = 9
        Me.Label18.Text = resources.GetString("Label18.Text")
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(12, 223)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(158, 13)
        Me.Label16.TabIndex = 8
        Me.Label16.Text = """Unrepresentable Domain"""
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(12, 10)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(266, 13)
        Me.Label15.TabIndex = 6
        Me.Label15.Text = "Enter a Description of the Values Recorded in the Field"
        '
        'txtUnrep
        '
        Me.txtUnrep.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUnrep.Location = New System.Drawing.Point(15, 30)
        Me.txtUnrep.Multiline = True
        Me.txtUnrep.Name = "txtUnrep"
        Me.txtUnrep.Size = New System.Drawing.Size(582, 175)
        Me.txtUnrep.TabIndex = 7
        '
        'pnlCodeset
        '
        Me.pnlCodeset.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.pnlCodeset.Controls.Add(Me.txtCodesetSource)
        Me.pnlCodeset.Controls.Add(Me.cboCodesetName)
        Me.pnlCodeset.Controls.Add(Me.Label17)
        Me.pnlCodeset.Controls.Add(Me.Label9)
        Me.pnlCodeset.Controls.Add(Me.Label13)
        Me.pnlCodeset.Controls.Add(Me.Label1)
        Me.pnlCodeset.Location = New System.Drawing.Point(6, 208)
        Me.pnlCodeset.Name = "pnlCodeset"
        Me.pnlCodeset.Size = New System.Drawing.Size(611, 366)
        Me.pnlCodeset.TabIndex = 12
        Me.pnlCodeset.Tag = "codeset"
        Me.pnlCodeset.Visible = False
        '
        'txtCodesetSource
        '
        Me.txtCodesetSource.Location = New System.Drawing.Point(113, 43)
        Me.txtCodesetSource.Name = "txtCodesetSource"
        Me.txtCodesetSource.Size = New System.Drawing.Size(412, 20)
        Me.txtCodesetSource.TabIndex = 14
        '
        'cboCodesetName
        '
        Me.cboCodesetName.FormattingEnabled = True
        Me.cboCodesetName.Items.AddRange(New Object() {"FIPS Code (2-Digit State ID)", "FIPS Code (3-Digit County ID)", "FIPS Code (5-Digit State & County ID)", "US Postal Zip Codes", "US Postal 2-Letter State", "MAF / TIGER Feature Class Codes (MTFCC)"})
        Me.cboCodesetName.Location = New System.Drawing.Point(113, 17)
        Me.cboCodesetName.Name = "cboCodesetName"
        Me.cboCodesetName.Size = New System.Drawing.Size(412, 21)
        Me.cboCodesetName.TabIndex = 12
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(24, 85)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(111, 13)
        Me.Label17.TabIndex = 11
        Me.Label17.Text = """Codeset Domain"""
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(24, 45)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "Codeset Source"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(24, 19)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(77, 13)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Codeset Name"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(24, 109)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(501, 161)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'pnlEnumerated
        '
        Me.pnlEnumerated.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.pnlEnumerated.Controls.Add(Me.Label19)
        Me.pnlEnumerated.Controls.Add(Me.txtValDefSource)
        Me.pnlEnumerated.Controls.Add(Me.txtValDef)
        Me.pnlEnumerated.Controls.Add(Me.Label7)
        Me.pnlEnumerated.Controls.Add(Me.Label6)
        Me.pnlEnumerated.Controls.Add(Me.Label5)
        Me.pnlEnumerated.Controls.Add(Me.lstUniqueVals)
        Me.pnlEnumerated.Controls.Add(Me.Label20)
        Me.pnlEnumerated.Location = New System.Drawing.Point(6, 208)
        Me.pnlEnumerated.Name = "pnlEnumerated"
        Me.pnlEnumerated.Size = New System.Drawing.Size(611, 366)
        Me.pnlEnumerated.TabIndex = 12
        Me.pnlEnumerated.Tag = "enum"
        Me.pnlEnumerated.Visible = False
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(210, 179)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(132, 13)
        Me.Label19.TabIndex = 8
        Me.Label19.Text = """Enumerated Domain"""
        '
        'txtValDefSource
        '
        Me.txtValDefSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtValDefSource.Location = New System.Drawing.Point(213, 128)
        Me.txtValDefSource.Multiline = True
        Me.txtValDefSource.Name = "txtValDefSource"
        Me.txtValDefSource.Size = New System.Drawing.Size(390, 36)
        Me.txtValDefSource.TabIndex = 7
        '
        'txtValDef
        '
        Me.txtValDef.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtValDef.Location = New System.Drawing.Point(213, 26)
        Me.txtValDef.Multiline = True
        Me.txtValDef.Name = "txtValDef"
        Me.txtValDef.Size = New System.Drawing.Size(390, 83)
        Me.txtValDef.TabIndex = 6
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(210, 112)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Definition source"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(210, 9)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(111, 13)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "Definition of this value"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(15, 4)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(76, 13)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Unique Values"
        '
        'lstUniqueVals
        '
        Me.lstUniqueVals.FormattingEnabled = True
        Me.lstUniqueVals.Location = New System.Drawing.Point(15, 26)
        Me.lstUniqueVals.Name = "lstUniqueVals"
        Me.lstUniqueVals.Size = New System.Drawing.Size(182, 290)
        Me.lstUniqueVals.TabIndex = 0
        '
        'Label20
        '
        Me.Label20.Location = New System.Drawing.Point(213, 196)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(390, 120)
        Me.Label20.TabIndex = 9
        Me.Label20.Text = resources.GetString("Label20.Text")
        '
        'cmdSave
        '
        Me.cmdSave.BackColor = System.Drawing.Color.LightSteelBlue
        Me.cmdSave.Location = New System.Drawing.Point(45, 595)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(212, 23)
        Me.cmdSave.TabIndex = 8
        Me.cmdSave.Text = "Save"
        Me.cmdSave.UseVisualStyleBackColor = False
        '
        'frmGenEA
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(963, 632)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdSaveAndClose)
        Me.Controls.Add(Me.cmdSave)
        Me.Controls.Add(Me.splitContainer_Main)
        Me.Controls.Add(Me.lblLocation)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmGenEA"
        Me.Padding = New System.Windows.Forms.Padding(5)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Metadata Wizard - Entity and Attribute Builder"
        Me.splitContainer_Main.Panel1.ResumeLayout(False)
        Me.splitContainer_Main.Panel1.PerformLayout()
        Me.splitContainer_Main.Panel2.ResumeLayout(False)
        Me.splitContainer_Main.Panel2.PerformLayout()
        CType(Me.splitContainer_Main, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer_Main.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.frmDomain.ResumeLayout(False)
        Me.frmDomain.PerformLayout()
        Me.pnlRange.ResumeLayout(False)
        Me.pnlRange.PerformLayout()
        Me.pnlUnrepresentable.ResumeLayout(False)
        Me.pnlUnrepresentable.PerformLayout()
        Me.pnlCodeset.ResumeLayout(False)
        Me.pnlCodeset.PerformLayout()
        Me.pnlEnumerated.ResumeLayout(False)
        Me.pnlEnumerated.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdSaveAndClose As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents lblLocation As System.Windows.Forms.Label
    Friend WithEvents splitContainer_Main As System.Windows.Forms.SplitContainer
    Friend WithEvents txtOverview As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtAttDef As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents pnlRange As System.Windows.Forms.Panel
    Friend WithEvents txtRngResolution As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtRngUnits As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtRngMax As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtRngMin As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents pnlEnumerated As System.Windows.Forms.Panel
    Friend WithEvents txtValDefSource As System.Windows.Forms.TextBox
    Friend WithEvents txtValDef As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lstUniqueVals As System.Windows.Forms.ListBox
    Friend WithEvents frmDomain As System.Windows.Forms.GroupBox
    Friend WithEvents optEnum As System.Windows.Forms.RadioButton
    Friend WithEvents optUnrep As System.Windows.Forms.RadioButton
    Friend WithEvents optRange As System.Windows.Forms.RadioButton
    Friend WithEvents optCodeset As System.Windows.Forms.RadioButton
    Friend WithEvents txtAttDefSource As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents pnlUnrepresentable As System.Windows.Forms.Panel
    Friend WithEvents pnlCodeset As System.Windows.Forms.Panel
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtUnrep As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents lstFields As System.Windows.Forms.ListView
    Friend WithEvents Attribute As System.Windows.Forms.ColumnHeader
    Friend WithEvents TypeX As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtEadetcit As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents cboCodesetName As System.Windows.Forms.ComboBox
    Friend WithEvents txtCodesetSource As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents labelVATwarning As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
End Class
