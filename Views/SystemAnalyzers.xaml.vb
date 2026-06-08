Imports System.Windows
Imports System.Diagnostics
Imports Microsoft.Win32
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Media.Imaging

Namespace Views
    Public Class SystemAnalyzers
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub btnLoadProcesses_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim processList As New List(Of ProcessInfo)()
                For Each p As Process In Process.GetProcesses()
                    Try
                        processList.Add(New ProcessInfo With {
                            .PID = p.Id,
                            .ProcessName = p.ProcessName,
                            .MemoryMB = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 2)
                        })
                    Catch ex As Exception
                        ' Access denied to some processes
                    End Try
                Next
                dgProcesses.ItemsSource = processList.OrderByDescending(Function(x) x.MemoryMB)
            Catch ex As Exception
                MessageBox.Show("Error loading processes: " & ex.Message)
            End Try
        End Sub

        Private Async Sub btnLoadNetwork_Click(sender As Object, e As RoutedEventArgs)
            btnLoadNetwork.IsEnabled = False
            txtNetworkOut.Text = "Scanning netstat -ano..."
            Try
                Dim output As String = Await System.Threading.Tasks.Task.Run(Function()
                    Dim proc As New Process()
                    proc.StartInfo.FileName = "netstat"
                    proc.StartInfo.Arguments = "-ano"
                    proc.StartInfo.UseShellExecute = False
                    proc.StartInfo.RedirectStandardOutput = True
                    proc.StartInfo.CreateNoWindow = True
                    proc.Start()
                    Dim result As String = proc.StandardOutput.ReadToEnd()
                    proc.WaitForExit()
                    Return result
                End Function)
                txtNetworkOut.Text = output
            Catch ex As Exception
                txtNetworkOut.Text = "Error: " & ex.Message
            Finally
                btnLoadNetwork.IsEnabled = True
            End Try
        End Sub

        Private Sub btnLoadStartup_Click(sender As Object, e As RoutedEventArgs)
            Dim startupList As New List(Of StartupInfo)()

            ' Scan Current User Run key
            Try
                Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run")
                    If key IsNot Nothing Then
                        For Each valName In key.GetValueNames()
                            startupList.Add(New StartupInfo With {.Name = valName, .Path = key.GetValue(valName).ToString(), .Location = "HKCU\...\Run"})
                        Next
                    End If
                End Using
            Catch
            End Try

            ' Scan Local Machine Run key
            Try
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run")
                    If key IsNot Nothing Then
                        For Each valName In key.GetValueNames()
                            startupList.Add(New StartupInfo With {.Name = valName, .Path = key.GetValue(valName).ToString(), .Location = "HKLM\...\Run"})
                        Next
                    End If
                End Using
            Catch
            End Try

            dgStartup.ItemsSource = startupList
        End Sub

        Private Sub btnTakeScreenshot_Click(sender As Object, e As RoutedEventArgs)
            Try
                ' Hide window to capture screen
                Dim mainWindow As Window = Application.Current.MainWindow
                mainWindow.WindowState = WindowState.Minimized
                System.Threading.Thread.Sleep(500) ' Wait for minimize animation

                ' Capture screen
                Dim bounds As System.Drawing.Rectangle = System.Windows.Forms.Screen.PrimaryScreen.Bounds
                Using bmp As New Bitmap(bounds.Width, bounds.Height)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size)
                    End Using

                    ' Save to temp memory stream and load into WPF Image control
                    Using ms As New MemoryStream()
                        bmp.Save(ms, ImageFormat.Png)
                        ms.Position = 0
                        Dim bitmapImage As New BitmapImage()
                        bitmapImage.BeginInit()
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad
                        bitmapImage.StreamSource = ms
                        bitmapImage.EndInit()
                        imgScreenshot.Source = bitmapImage
                    End Using
                End Using

                mainWindow.WindowState = WindowState.Normal
            Catch ex As Exception
                MessageBox.Show("Error taking screenshot: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                Application.Current.MainWindow.WindowState = WindowState.Normal
            End Try
        End Sub
    End Class

    Public Class ProcessInfo
        Public Property PID As Integer
        Public Property ProcessName As String
        Public Property MemoryMB As Double
    End Class

    Public Class StartupInfo
        Public Property Name As String
        Public Property Path As String
        Public Property Location As String
    End Class
End Namespace
