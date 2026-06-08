Imports System.Windows
Imports Microsoft.Win32
Imports System.Collections.ObjectModel

Namespace Views
    Public Class UsbHistory
        Public Property UsbDevices As ObservableCollection(Of Models.USBDeviceEntry)

        Public Sub New()
            InitializeComponent()
            UsbDevices = New ObservableCollection(Of Models.USBDeviceEntry)()
            dgUsbDevices.ItemsSource = UsbDevices
        End Sub

        Private Sub btnScan_Click(sender As Object, e As RoutedEventArgs)
            ScanUsbRegistry()
        End Sub

        Private Sub ScanUsbRegistry()
            UsbDevices.Clear()
            Try
                ' This key usually requires admin rights to read fully, but basic info might be accessible.
                Dim keyPath As String = "SYSTEM\CurrentControlSet\Enum\USBSTOR"
                Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(keyPath)
                    If key IsNot Nothing Then
                        For Each deviceType In key.GetSubKeyNames()
                            Using deviceKey As RegistryKey = key.OpenSubKey(deviceType)
                                If deviceKey IsNot Nothing Then
                                    For Each serialNumber In deviceKey.GetSubKeyNames()
                                        Using infoKey As RegistryKey = deviceKey.OpenSubKey(serialNumber)
                                            If infoKey IsNot Nothing Then
                                                Dim friendlyName As String = TryCast(infoKey.GetValue("FriendlyName"), String)
                                                If String.IsNullOrEmpty(friendlyName) Then
                                                    friendlyName = deviceType
                                                End If
                                                
                                                ' Extract dates (this requires parsing properties or setupapi.dev.log for exact first/last use)
                                                ' For this architecture, we will put placeholders for dates until advanced parsing is added.
                                                Dim entry As New Models.USBDeviceEntry() With {
                                                    .DeviceName = friendlyName,
                                                    .SerialNumber = serialNumber.Replace("&0", ""),
                                                    .FirstUse = "Unknown (Check Logs)",
                                                    .LastUse = "Unknown (Check Logs)"
                                                }
                                                UsbDevices.Add(entry)
                                            End If
                                        End Using
                                    Next
                                End If
                            End Using
                        Next
                    Else
                        MessageBox.Show("USBSTOR key not found. Ensure you are running as Administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning)
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("Error reading registry: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub
    End Class
End Namespace
