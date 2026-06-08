Namespace Views
    Public Class Dashboard
        Public Sub New()
            InitializeComponent()
            LoadStatistics()
        End Sub

        Private Sub LoadStatistics()
            ' Placeholders for database calls to load stats
            txtUsbCount.Text = "12"
            txtSuspiciousCount.Text = "3"
            txtBrowserCount.Text = "1542"
            txtDeletedCount.Text = "48"
            txtEventsCount.Text = "310"
        End Sub
    End Class
End Namespace
