Imports System.Windows
Imports System.IO
Imports System.Collections.ObjectModel

Namespace Views
    Public Class FileRecovery
        Public Property RecoveredFiles As ObservableCollection(Of Models.RecoveredFileEntry)

        Public Sub New()
            InitializeComponent()
            RecoveredFiles = New ObservableCollection(Of Models.RecoveredFileEntry)()
            dgRecovery.ItemsSource = RecoveredFiles
        End Sub

        Private Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            RecoveredFiles.Clear()
            Try
                ' Mock Recycle Bin Scan for Architecture setup
                ' A real implementation parses $I files in C:\$Recycle.Bin
                Dim drive As New DriveInfo("C")
                Dim recycleBinPath As String = Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin")

                If Directory.Exists(recycleBinPath) Then
                    RecoveredFiles.Add(New Models.RecoveredFileEntry With {
                        .FileName = "secret_document.pdf",
                        .Size = 450212,
                        .DeletedDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss"),
                        .OriginalPath = "C:\Users\Admin\Documents\secret_document.pdf"
                    })
                    RecoveredFiles.Add(New Models.RecoveredFileEntry With {
                        .FileName = "passwords.txt",
                        .Size = 1024,
                        .DeletedDate = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                        .OriginalPath = "C:\Users\Admin\Desktop\passwords.txt"
                    })
                Else
                    MessageBox.Show("Could not access Recycle Bin. Run as Administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning)
                End If
            Catch ex As Exception
                MessageBox.Show("Error scanning: " & ex.Message)
            End Try
        End Sub
    End Class
End Namespace
