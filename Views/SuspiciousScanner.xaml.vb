Imports System.Windows
Imports Microsoft.Win32
Imports System.IO
Imports System.Security.Cryptography

Namespace Views
    Public Class SuspiciousScanner
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub btnSelectFile_Click(sender As Object, e As RoutedEventArgs)
            Dim ofd As New OpenFileDialog()
            If ofd.ShowDialog() = True Then
                AnalyzeFile(ofd.FileName)
            End If
        End Sub

        Private Sub AnalyzeFile(filePath As String)
            txtResults.Text = "Analyzing " & filePath & vbCrLf & vbCrLf

            Try
                Dim fileInfo As New FileInfo(filePath)
                txtResults.Text &= $"Size: {fileInfo.Length} bytes" & vbCrLf
                txtResults.Text &= $"Extension: {fileInfo.Extension}" & vbCrLf
                txtResults.Text &= $"Created: {fileInfo.CreationTime}" & vbCrLf
                txtResults.Text &= $"Modified: {fileInfo.LastWriteTime}" & vbCrLf & vbCrLf

                Dim bytes As Byte() = File.ReadAllBytes(filePath)

                ' Hashes
                Using md5 As MD5 = MD5.Create()
                    Dim hash = md5.ComputeHash(bytes)
                    txtResults.Text &= $"MD5: {BitConverter.ToString(hash).Replace("-", "").ToLower()}" & vbCrLf
                End Using

                Using sha1 As SHA1 = SHA1.Create()
                    Dim hash = sha1.ComputeHash(bytes)
                    txtResults.Text &= $"SHA1: {BitConverter.ToString(hash).Replace("-", "").ToLower()}" & vbCrLf
                End Using

                Using sha256 As SHA256 = SHA256.Create()
                    Dim hash = sha256.ComputeHash(bytes)
                    txtResults.Text &= $"SHA256: {BitConverter.ToString(hash).Replace("-", "").ToLower()}" & vbCrLf
                End Using

                ' Entropy (Basic calculation)
                Dim entropy As Double = CalculateEntropy(bytes)
                txtResults.Text &= vbCrLf & $"Entropy: {Math.Round(entropy, 4)}" & vbCrLf
                If entropy > 7.2 Then
                    txtResults.Text &= "WARNING: High entropy detected. File is likely packed or encrypted." & vbCrLf
                Else
                    txtResults.Text &= "Entropy is within normal range." & vbCrLf
                End If

            Catch ex As Exception
                txtResults.Text &= "Error reading file: " & ex.Message
            End Try
        End Sub

        Private Function CalculateEntropy(bytes As Byte()) As Double
            Dim counts(255) As Integer
            For Each b As Byte In bytes
                counts(b) += 1
            Next

            Dim entropy As Double = 0
            Dim length As Double = bytes.Length

            For Each count As Integer In counts
                If count > 0 Then
                    Dim p As Double = count / length
                    entropy -= p * Math.Log(p, 2)
                End If
            Next

            Return entropy
        End Function
    End Class
End Namespace
