Imports System.Windows
Imports System.Windows.Controls

Class MainWindow
    Private _dbManager As DatabaseManager

    Public Sub New()
        InitializeComponent()
        InitializeDatabase()
        
        ' Load Dashboard by default
        NavigateTo("Dashboard")
    End Sub

    Private Sub InitializeDatabase()
        Try
            _dbManager = New DatabaseManager()
            _dbManager.InitializeDatabase()
        Catch ex As Exception
            MessageBox.Show("Error initializing database: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub NavButton_Click(sender As Object, e As RoutedEventArgs)
        Dim btn As Button = TryCast(sender, Button)
        If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
            Dim pageName As String = btn.Tag.ToString()
            NavigateTo(pageName)
        End If
    End Sub

    Private Sub NavigateTo(pageName As String)
        Select Case pageName
            Case "Dashboard"
                MainFrame.Content = New Views.Dashboard()
            Case "EventLogs"
                MainFrame.Content = New Views.EventLogs()
            Case "Browser"
                MainFrame.Content = New Views.BrowserHistory()
            Case "USB"
                MainFrame.Content = New Views.UsbHistory()
            Case "Timeline"
                MainFrame.Content = New Views.TimelineExplorer()
            Case "Recovery"
                MainFrame.Content = New Views.FileRecovery()
            Case "Suspicious"
                MainFrame.Content = New Views.SuspiciousScanner()
            Case "Prefetch"
                MainFrame.Content = New Views.PrefetchAnalyzer()
            Case "AdsScanner"
                MainFrame.Content = New Views.AdsScanner()
            Case "JumpLists"
                MainFrame.Content = New Views.JumpListsAnalyzer()
            Case "DnsCache"
                MainFrame.Content = New Views.DnsCacheViewer()
            Case "Services"
                MainFrame.Content = New Views.ServicesAnalyzer()
            Case "System"
                MainFrame.Content = New Views.SystemAnalyzers()
            Case "Export"
                MainFrame.Content = New Views.EvidenceExport()
            Case Else
                Dim textBlock As New TextBlock() With {
                    .Text = pageName & " Module loaded successfully.",
                    .FontSize = 24,
                    .Foreground = New Media.SolidColorBrush(Media.Colors.White),
                    .HorizontalAlignment = HorizontalAlignment.Center,
                    .VerticalAlignment = VerticalAlignment.Center
                }
                MainFrame.Content = textBlock
        End Select
    End Sub
End Class
