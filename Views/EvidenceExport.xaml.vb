Imports System.Windows
Imports Microsoft.Win32
Imports System.IO

Namespace Views
    Public Class EvidenceExport
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub btnExport_Click(sender As Object, e As RoutedEventArgs)
            If String.IsNullOrWhiteSpace(txtCaseID.Text) OrElse String.IsNullOrWhiteSpace(txtExaminer.Text) Then
                MessageBox.Show("Please fill in Case ID and Examiner details.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim format As String = TryCast(cmbFormat.SelectedItem, Controls.ComboBoxItem).Content.ToString()
            Dim sfd As New SaveFileDialog()
            
            If format.Contains("PDF") Then
                sfd.Filter = "PDF Files (*.pdf)|*.pdf"
            ElseIf format.Contains("HTML") Then
                sfd.Filter = "HTML Files (*.html)|*.html"
            ElseIf format.Contains("JSON") Then
                sfd.Filter = "JSON Files (*.json)|*.json"
            Else
                sfd.Filter = "CSV Files (*.csv)|*.csv"
            End If

            sfd.FileName = $"MASA_DFI_Report_{txtCaseID.Text}_{DateTime.Now.ToString("yyyyMMdd")}"

            If sfd.ShowDialog() = True Then
                Try
                    ' Mock export generation
                    File.WriteAllText(sfd.FileName, $"MASA DFI Report{vbCrLf}Case ID: {txtCaseID.Text}{vbCrLf}Examiner: {txtExaminer.Text}{vbCrLf}Date: {DateTime.Now}{vbCrLf}Conclusion: Analysis completed successfully.")
                    MessageBox.Show("Report exported successfully to: " & sfd.FileName, "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information)
                Catch ex As Exception
                    MessageBox.Show("Error exporting report: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            End If
        End Sub
    End Class
End Namespace
