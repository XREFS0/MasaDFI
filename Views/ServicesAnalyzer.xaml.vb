Imports System.Windows
Imports System.Diagnostics
Imports Newtonsoft.Json.Linq

Namespace Views
    Public Class ServicesAnalyzer
        Public Property ServicesEntries As New Collections.ObjectModel.ObservableCollection(Of ServiceEntry)

        Public Sub New()
            InitializeComponent()
            dgServices.ItemsSource = ServicesEntries
        End Sub

        Private Async Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            btnScan.IsEnabled = False
            ServicesEntries.Clear()

            Try
                Dim script As String = "Get-CimInstance Win32_Service | Select-Object Name, DisplayName, State, PathName | ConvertTo-Json -Compress"
                
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
                        Dim items = JToken.Parse(output)
                        If items.Type = JTokenType.Array Then
                            For Each item In items
                                ServicesEntries.Add(New ServiceEntry With {
                                    .Name = item("Name")?.ToString(),
                                    .DisplayName = item("DisplayName")?.ToString(),
                                    .State = item("State")?.ToString(),
                                    .PathName = item("PathName")?.ToString()
                                })
                            Next
                        End If
                    Catch ex As Exception
                        MessageBox.Show("Error parsing services: " & ex.Message)
                    End Try
                End If
            Catch ex As Exception
                MessageBox.Show("Error scanning services: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                btnScan.IsEnabled = True
            End Try
        End Sub
    End Class

    Public Class ServiceEntry
        Public Property Name As String
        Public Property DisplayName As String
        Public Property State As String
        Public Property PathName As String
    End Class
End Namespace
