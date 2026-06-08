Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Namespace Views
    Public Class AdsScanner
        Public Property AdsEntries As New Collections.ObjectModel.ObservableCollection(Of AdsEntry)

        Public Sub New()
            InitializeComponent()
            dgAds.ItemsSource = AdsEntries
        End Sub

        Private Async Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            ' Simple folder picker using FolderBrowserDialog via Forms, or just ask user for path
            Dim dialog As New System.Windows.Forms.FolderBrowserDialog()
            dialog.Description = "Select a folder to scan for hidden Alternate Data Streams (ADS)"
            If dialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Dim selectedPath As String = dialog.SelectedPath
                AdsEntries.Clear()
                btnScan.IsEnabled = False
                
                Try
                    Await ScanForAds(selectedPath)
                    If AdsEntries.Count = 0 Then
                        MessageBox.Show("No hidden streams found in the selected directory.", "Scan Complete", MessageBoxButton.OK, MessageBoxImage.Information)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error scanning: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                Finally
                    btnScan.IsEnabled = True
                End Try
            End If
        End Sub

        Private Async Function ScanForAds(folderPath As String) As System.Threading.Tasks.Task
            ' Use PowerShell to find ADS: Get-Item -Path "folder\*" -Stream * | Where-Object Stream -ne ':$DATA'
            Dim script As String = $"Get-ChildItem -Path '{folderPath}' -Recurse -File -ErrorAction SilentlyContinue | Get-Item -Stream * -ErrorAction SilentlyContinue | Where-Object Stream -ne ':$DATA' | Select-Object FileName, Stream, Length | ConvertTo-Json"
            
            Dim output As String = Await System.Threading.Tasks.Task.Run(Function()
                Dim proc As New Process()
                proc.StartInfo.FileName = "powershell"
                proc.StartInfo.Arguments = $"-NoProfile -Command ""{script}"""
                proc.StartInfo.UseShellExecute = False
                proc.StartInfo.RedirectStandardOutput = True
                proc.StartInfo.CreateNoWindow = True
                proc.Start()
                Dim result As String = proc.StandardOutput.ReadToEnd()
                proc.WaitForExit()
                Return result
            End Function)

            If Not String.IsNullOrWhiteSpace(output) Then
                Try
                    ' The output is JSON. We can parse it using Newtonsoft.Json.
                    Dim items = Newtonsoft.Json.Linq.JToken.Parse(output)
                    If items.Type = Newtonsoft.Json.Linq.JTokenType.Array Then
                        For Each item In items
                            AdsEntries.Add(New AdsEntry With {
                                .FilePath = item("FileName")?.ToString(),
                                .StreamName = item("Stream")?.ToString(),
                                .StreamSize = Convert.ToInt64(item("Length"))
                            })
                        Next
                    ElseIf items.Type = Newtonsoft.Json.Linq.JTokenType.Object Then
                        AdsEntries.Add(New AdsEntry With {
                            .FilePath = items("FileName")?.ToString(),
                            .StreamName = items("Stream")?.ToString(),
                            .StreamSize = Convert.ToInt64(items("Length"))
                        })
                    End If
                Catch ex As Exception
                    ' Parse error or no ads
                End Try
            End If
        End Function
    End Class

    Public Class AdsEntry
        Public Property FilePath As String
        Public Property StreamName As String
        Public Property StreamSize As Long
    End Class
End Namespace
