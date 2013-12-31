<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MD_previewer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MD_previewer))
        Me.WebBrowser_Preview = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'WebBrowser_Preview
        '
        Me.WebBrowser_Preview.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser_Preview.Location = New System.Drawing.Point(0, 0)
        Me.WebBrowser_Preview.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser_Preview.Name = "WebBrowser_Preview"
        Me.WebBrowser_Preview.Size = New System.Drawing.Size(1156, 667)
        Me.WebBrowser_Preview.TabIndex = 0
        '
        'MD_previewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1156, 667)
        Me.Controls.Add(Me.WebBrowser_Preview)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MD_previewer"
        Me.Text = "Metadata Wizard - Record Preview          (Right Click in Form for Options)"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents WebBrowser_Preview As System.Windows.Forms.WebBrowser
End Class
