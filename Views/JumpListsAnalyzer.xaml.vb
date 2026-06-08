Imports System.Windows
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Collections.ObjectModel

Namespace Views
    Public Class JumpListsAnalyzer
        Public Property JumpListEntries As New ObservableCollection(Of JumpListEntry)

        Public Sub New()
            InitializeComponent()
            dgJumpLists.ItemsSource = JumpListEntries
        End Sub

        Private Async Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            btnScan.IsEnabled = False
            JumpListEntries.Clear()

            Try
                Await System.Threading.Tasks.Task.Run(Sub()
                    Dim jumpListDir As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\Windows\Recent\AutomaticDestinations")
                    
                    If Directory.Exists(jumpListDir) Then
                        Dim files As String() = Directory.GetFiles(jumpListDir, "*.automaticDestinations-ms")
                        
                        For Each file In files
                            Dim fi As New FileInfo(file)
                            Dim appId As String = fi.Name.Replace(".automaticDestinations-ms", "")
                            
                            ' Read binary content and try to extract paths (C:\...)
                            Try
                                Dim contentBytes As Byte() = System.IO.File.ReadAllBytes(file)
                                Dim contentStr As String = System.Text.Encoding.Unicode.GetString(contentBytes)
                                
                                ' Simple regex to find drive paths in Unicode
                                Dim matches As MatchCollection = Regex.Matches(contentStr, "[a-zA-Z]:\\[^\0]+")
                                
                                Dim uniquePaths As New HashSet(Of String)()
                                For Each match As Match In matches
                                    If match.Value.Length > 5 AndAlso Not match.Value.Contains("?") Then
                                        uniquePaths.Add(match.Value)
                                    End If
                                Next

                                If uniquePaths.Count > 0 Then
                                    For Each p In uniquePaths
                                        Application.Current.Dispatcher.Invoke(Sub()
                                            JumpListEntries.Add(New JumpListEntry With {
                                                .AppId = appId,
                                                .ExtractedPath = p,
                                                .LastModified = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                                            })
                                        End Sub)
                                    Next
                                Else
                                    Application.Current.Dispatcher.Invoke(Sub()
                                        JumpListEntries.Add(New JumpListEntry With {
                                            .AppId = appId,
                                            .ExtractedPath = "[No clear paths extracted - Binary Data]",
                                            .LastModified = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                                        })
                                    End Sub)
                                End If
                            Catch ex As Exception
                                ' Ignore read errors for locked files
                            End Try
                        Next
                    End If
                End Sub)
                
            Catch ex As Exception
                MessageBox.Show("Error scanning Jump Lists: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                btnScan.IsEnabled = True
            End Try
        End Sub
    End Class

    Public Class JumpListEntry
        Public Property AppId As String
        Public Property ExtractedPath As String
        Public Property LastModified As String
    End Class
End Namespace
