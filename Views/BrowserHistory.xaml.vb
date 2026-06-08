Imports System.Windows
Imports System.IO
Imports System.Data.SQLite
Imports System.Collections.ObjectModel

Namespace Views
    Public Class BrowserHistory
        Public Property HistoryEntries As ObservableCollection(Of Models.BrowserHistoryEntry)

        Public Sub New()
            InitializeComponent()
            HistoryEntries = New ObservableCollection(Of Models.BrowserHistoryEntry)()
            dgHistory.ItemsSource = HistoryEntries
        End Sub

        Private Sub btnLoad_Click(sender As Object, e As RoutedEventArgs)
            Dim browser As String = TryCast(cmbBrowser.SelectedItem, Controls.ComboBoxItem).Content.ToString()
            LoadBrowserHistory(browser)
        End Sub

        Private Sub LoadBrowserHistory(browser As String)
            HistoryEntries.Clear()
            Dim appData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim historyPath As String = ""

            If browser = "Google Chrome" Then
                historyPath = Path.Combine(appData, "Google\Chrome\User Data\Default\History")
            ElseIf browser = "Microsoft Edge" Then
                historyPath = Path.Combine(appData, "Microsoft\Edge\User Data\Default\History")
            End If

            If Not File.Exists(historyPath) Then
                MessageBox.Show(browser & " history file not found. It might not be installed or used.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information)
                Return
            End If

            ' Copy file to temp to avoid lock issues
            Dim tempFile As String = Path.Combine(Path.GetTempPath(), "masadfi_history.db")
            Try
                File.Copy(historyPath, tempFile, True)
                
                Dim connectionString As String = $"Data Source={tempFile};Version=3;"
                Using connection As New SQLiteConnection(connectionString)
                    connection.Open()
                    Dim query As String = "SELECT url, title, visit_count, datetime(last_visit_time/1000000-11644473600, 'unixepoch', 'localtime') as last_visit FROM urls ORDER BY last_visit DESC LIMIT 500"
                    Using command As New SQLiteCommand(query, connection)
                        Using reader As SQLiteDataReader = command.ExecuteReader()
                            While reader.Read()
                                HistoryEntries.Add(New Models.BrowserHistoryEntry With {
                                    .Browser = browser,
                                    .URL = reader("url").ToString(),
                                    .Title = reader("title").ToString(),
                                    .VisitTime = reader("last_visit").ToString(),
                                    .VisitCount = Convert.ToInt32(reader("visit_count"))
                                })
                            End While
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error reading browser history: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Finally
                If File.Exists(tempFile) Then
                    Try
                        File.Delete(tempFile)
                    Catch
                    End Try
                End If
            End Try
        End Sub
    End Class
End Namespace
