Imports System.Windows
Imports System.Windows.Threading

Namespace Views
    Public Class SplashWindow
        Private dispatcherTimer As New DispatcherTimer()
        Private progress As Integer = 0

        Public Sub New()
            InitializeComponent()
            
            ' Setup loading timer
            AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
            dispatcherTimer.Interval = New TimeSpan(0, 0, 0, 0, 50) ' 50 milliseconds
            dispatcherTimer.Start()
        End Sub

        Private Sub dispatcherTimer_Tick(sender As Object, e As EventArgs)
            progress += 2
            pbLoading.Value = progress

            ' Update status text based on progress
            If progress = 20 Then
                txtStatus.Text = "Loading Database Manager..."
            ElseIf progress = 40 Then
                txtStatus.Text = "Loading Security Modules..."
            ElseIf progress = 60 Then
                txtStatus.Text = "Establishing Kernel Hooks..."
            ElseIf progress = 80 Then
                txtStatus.Text = "Verifying Forensic Integrity..."
            ElseIf progress >= 100 Then
                dispatcherTimer.Stop()
                
                ' Open Main Window
                Dim mainWin As New MasaDFI.MainWindow()
                mainWin.Show()
                
                ' Close Splash
                Me.Close()
            End If
        End Sub
    End Class
End Namespace
