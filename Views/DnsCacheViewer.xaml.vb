Imports System.Windows
Imports System.Diagnostics

Namespace Views
    Public Class DnsCacheViewer
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Async Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            btnScan.IsEnabled = False
            txtDnsOutput.Text = "Retrieving DNS Cache... Please wait."

            Try
                Dim output As String = Await System.Threading.Tasks.Task.Run(Function()
                    Dim proc As New Process()
                    proc.StartInfo.FileName = "ipconfig"
                    proc.StartInfo.Arguments = "/displaydns"
                    proc.StartInfo.UseShellExecute = False
                    proc.StartInfo.RedirectStandardOutput = True
                    proc.StartInfo.CreateNoWindow = True
                    proc.Start()
                    Dim result As String = proc.StandardOutput.ReadToEnd()
                    proc.WaitForExit()
                    Return result
                End Function)

                txtDnsOutput.Text = output
            Catch ex As Exception
                txtDnsOutput.Text = "Error retrieving DNS Cache: " & ex.Message
            Finally
                btnScan.IsEnabled = True
            End Try
        End Sub

        Private Async Sub btnFlush_Click(sender As Object, e As RoutedEventArgs)
            Dim confirmResult = MessageBox.Show("Are you sure you want to flush the DNS cache? This action cannot be undone.", "Confirm Flush", MessageBoxButton.YesNo, MessageBoxImage.Warning)
            If confirmResult = MessageBoxResult.Yes Then
                Try
                    Dim output As String = Await System.Threading.Tasks.Task.Run(Function()
                        Dim proc As New Process()
                        proc.StartInfo.FileName = "ipconfig"
                        proc.StartInfo.Arguments = "/flushdns"
                        proc.StartInfo.UseShellExecute = False
                        proc.StartInfo.RedirectStandardOutput = True
                        proc.StartInfo.CreateNoWindow = True
                        proc.Start()
                        Dim result As String = proc.StandardOutput.ReadToEnd()
                        proc.WaitForExit()
                        Return result
                    End Function)
                    
                    MessageBox.Show(output, "Flush DNS Result", MessageBoxButton.OK, MessageBoxImage.Information)
                    btnScan_Click(Nothing, Nothing)
                Catch ex As Exception
                    MessageBox.Show("Error flushing DNS Cache: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End Try
            End If
        End Sub
    End Class
End Namespace
