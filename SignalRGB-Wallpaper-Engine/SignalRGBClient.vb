Imports System.Net
Imports System.Net.Sockets

Public Class SignalRGBClient

    Private udpClient As UdpClient
    Private listenPort As Integer = 8123 ' Default port
    Private _isListening As Boolean = False

    Public ReadOnly Property IsListening As Boolean
        Get
            Return _isListening
        End Get
    End Property

    Private _colors As New List(Of Color)
    Public ReadOnly Property Colors() As List(Of Color)
        Get
            Return _colors
        End Get
    End Property

    Public Sub New(Optional port As Integer = 8123)
        listenPort = port
    End Sub

    Public Sub StartListening()
        If IsListening Then
            Return
        End If

        Try
            udpClient = New UdpClient(listenPort)
            _isListening = True
            Dim remoteEP As New IPEndPoint(Net.IPAddress.Any, 0)

            While _isListening
                Try
                    Dim packetData As Byte() = udpClient.Receive(remoteEP)
                    ProcessSignalRGBPacket(packetData, remoteEP)
                Catch ex As Exception
                    If IsListening Then ' Only show error if we're still supposed to be listening
                        Logger.Log($"Error receiving data: {ex.Message}")
                    End If
                End Try
            End While
        Catch ex As Exception
            Logger.Log($"Error starting UDP listener: {ex.Message}")
        End Try
    End Sub

    Private Sub ProcessSignalRGBPacket(data As Byte(), sender As IPEndPoint)
        Try
            If data.Length < 8 Then
                Return
            End If

            ParseRGBCommands(data)
        Catch ex As Exception
            Logger.Log($"Error processing packet: {ex.Message}")
        End Try
    End Sub

    Private Sub ParseRGBCommands(data As Byte())
        If data.Length >= 8 Then
            _colors.Clear()

            For i As Integer = 0 To data.Length - 1 Step 3
                Dim r As Byte = data(i)
                Dim g As Byte = data(i + 1)
                Dim b As Byte = data(i + 2)
                _colors.Add(Color.FromArgb(r, g, b))
            Next
        End If
    End Sub

    Public Sub StopListening()
        _isListening = False
        If udpClient IsNot Nothing Then
            udpClient.Close()
            udpClient = Nothing
        End If
    End Sub

End Class
