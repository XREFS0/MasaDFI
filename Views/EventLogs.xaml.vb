Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Controls
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Namespace Views
    Public Class EventLogs
        Public Property LogEntries As ObservableCollection(Of EventLogData)

        Public Sub New()
            InitializeComponent()
            LogEntries = New ObservableCollection(Of EventLogData)()
            dgLogs.ItemsSource = LogEntries
        End Sub

        Private Async Sub btnLoadLogs_Click(sender As Object, e As RoutedEventArgs)
            Dim logType As String = TryCast(cmbLogType.SelectedItem, ComboBoxItem).Content.ToString()
            LogEntries.Clear()
            btnLoadLogs.IsEnabled = False
            
            Try
                Await System.Threading.Tasks.Task.Run(Sub() LoadLogs(logType))
            Catch ex As Exception
                MessageBox.Show("Error reading logs. You may need to run as Administrator to read Security logs. Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                btnLoadLogs.IsEnabled = True
            End Try
        End Sub

        Private Sub LoadLogs(logName As String)
            Try
                Dim log As New EventLog(logName)
                Dim entries = log.Entries.Cast(Of EventLogEntry)().OrderByDescending(Function(x) x.TimeGenerated).Take(500).ToList()

                Application.Current.Dispatcher.Invoke(Sub()
                    For Each entry In entries
                        LogEntries.Add(New EventLogData With {
                            .TimeGenerated = entry.TimeGenerated.ToString("yyyy-MM-dd HH:mm:ss"),
                            .EventID = entry.InstanceId And &HFFFF,
                            .EntryType = entry.EntryType.ToString(),
                            .Source = entry.Source,
                            .Message = entry.Message.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
                        })
                    Next
                End Sub)
            Catch ex As Security.SecurityException
                Application.Current.Dispatcher.Invoke(Sub()
                    MessageBox.Show("Access denied to " & logName & " log. Please run as Administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning)
                End Sub)
            Catch ex As Exception
                Application.Current.Dispatcher.Invoke(Sub()
                    Throw ex
                End Sub)
            End Try
        End Sub
    End Class

    Public Class EventLogData
        Public Property TimeGenerated As String
        Public Property EventID As Long
        Public Property EntryType As String
        Public Property Source As String
        Public Property Message As String
    End Class
End Namespace
