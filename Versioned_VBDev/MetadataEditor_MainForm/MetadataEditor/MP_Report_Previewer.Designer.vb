<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MP_Report_Previewer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MP_Report_Previewer))
        Me.WebBrowser_ErrorReportPreview = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'WebBrowser_ErrorReportPreview
        '
        Me.WebBrowser_ErrorReportPreview.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser_ErrorReportPreview.Location = New System.Drawing.Point(0, 0)
        Me.WebBrowser_ErrorReportPreview.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser_ErrorReportPreview.Name = "WebBrowser_ErrorReportPreview"
        Me.WebBrowser_ErrorReportPreview.Size = New System.Drawing.Size(1156, 667)
        Me.WebBrowser_ErrorReportPreview.TabIndex = 1
        '
        'MP_Report_Previewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1156, 667)
        Me.Controls.Add(Me.WebBrowser_ErrorReportPreview)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MP_Report_Previewer"
        Me.Text = "Metadata Parser (MP) Error Report"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents WebBrowser_ErrorReportPreview As System.Windows.Forms.WebBrowser
End Class
