Imports System.Windows
Imports System.IO
Imports System.Collections.ObjectModel

Namespace Views
    Public Class PrefetchAnalyzer
        Public Property PrefetchEntries As ObservableCollection(Of PrefetchEntry)

        Public Sub New()
            InitializeComponent()
            PrefetchEntries = New ObservableCollection(Of PrefetchEntry)()
            dgPrefetch.ItemsSource = PrefetchEntries
        End Sub

        Private Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            PrefetchEntries.Clear()
            Dim prefetchPath As String = "C:\Windows\Prefetch"
            
            Try
                If Directory.Exists(prefetchPath) Then
                    Dim files As String() = Directory.GetFiles(prefetchPath, "*.pf")
                    For Each file In files
                        Dim fi As New FileInfo(file)
                        Dim nameParts As String() = fi.Name.Split("-"c)
                        
                        Dim appName As String = fi.Name
                        Dim hash As String = "N/A"

                        If nameParts.Length > 1 Then
                            appName = nameParts(0)
                            hash = nameParts(1).Replace(".pf", "")
                        End If

                        PrefetchEntries.Add(New PrefetchEntry With {
                            .AppName = appName,
                            .Hash = hash,
                            .LastExecuted = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            .Size = fi.Length
                        })
                    Next
                    
                    ' Sort by most recently executed
                    Dim sortedList = PrefetchEntries.OrderByDescending(Function(x) x.LastExecuted).ToList()
                    PrefetchEntries.Clear()
                    For Each item In sortedList
                        PrefetchEntries.Add(item)
                    Next
                Else
                    MessageBox.Show("Prefetch directory not found or access denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            Catch ex As UnauthorizedAccessException
                MessageBox.Show("Access Denied to C:\Windows\Prefetch. Please run MASA DFI as Administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning)
            Catch ex As Exception
                MessageBox.Show("Error scanning prefetch: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub
    End Class

    Public Class PrefetchEntry
        Public Property AppName As String
        Public Property Hash As String
        Public Property LastExecuted As String
        Public Property Size As Long
    End Class
End Namespace
