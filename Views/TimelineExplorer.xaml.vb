Imports System.Windows
Imports System.Collections.ObjectModel

Namespace Views
    Public Class TimelineExplorer
        Public Property TimelineEvents As ObservableCollection(Of Models.TimelineEvent)

        Public Sub New()
            InitializeComponent()
            TimelineEvents = New ObservableCollection(Of Models.TimelineEvent)()
            dgTimeline.ItemsSource = TimelineEvents
        End Sub

        Private Sub btnGenerate_Click(sender As Object, e As RoutedEventArgs)
            TimelineEvents.Clear()
            ' Mocked Data for Architecture Demonstration
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-2).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "System", .Description = "PC Started"})
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-1.9).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "USB", .Description = "USB Device Inserted (SanDisk Cruzer)"})
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-1.8).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "Browser", .Description = "Chrome Opened"})
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-1.7).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "Browser", .Description = "Visited: file-sharing-site.com/download/setup.exe"})
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-1.5).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "File System", .Description = "Created File: C:\Users\Admin\Downloads\setup.exe"})
            TimelineEvents.Add(New Models.TimelineEvent With {.EventTime = DateTime.Now.AddHours(-1.0).ToString("yyyy-MM-dd HH:mm:ss"), .Category = "USB", .Description = "USB Device Removed"})
        End Sub
    End Class
End Namespace
