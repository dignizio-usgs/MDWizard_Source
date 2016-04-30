Imports ESRI.ArcGIS

Partial Friend Class LicenseInitializer

  Public Sub New()

  End Sub

  Private Sub BindingArcGISRuntime(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResolveBindingEvent
    '
    ' TODO: Modify ArcGIS runtime binding code as needed
    '
    If Not RuntimeManager.Bind(ProductCode.Desktop) Then
      ' Failed to bind, announce and force exit
      MsgBox("ArcGIS runtime binding failed. Application will shut down.")
      End
    End If
  End Sub

End Class