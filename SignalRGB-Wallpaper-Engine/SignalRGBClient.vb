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

    Private _matrixSizeType As MatrixSizeType
    Public ReadOnly Property MatrixSizeType() As MatrixSizeType
        Get
            Return _matrixSizeType
        End Get
    End Property

    Public ReadOnly Property MatrixSize() As Size
        Get
            Select Case _matrixSizeType
                Case MatrixSizeType.Landscape4_3
                    Return New Size(36, 27)
                Case MatrixSizeType.Portrait4_3
                    Return New Size(27, 36)
                Case MatrixSizeType.Landscape5_4
                    Return New Size(40, 32)
                Case MatrixSizeType.Portrait5_4
                    Return New Size(32, 40)
                Case MatrixSizeType.Landscape16_9
                    Return New Size(48, 27)
                Case MatrixSizeType.Portrait16_9
                    Return New Size(27, 48)
                Case MatrixSizeType.Landscape16_10
                    Return New Size(48, 30)
                Case MatrixSizeType.Portrait16_10
                    Return New Size(30, 48)
                Case MatrixSizeType.Landscape21_9
                    Return New Size(52, 22)
                Case MatrixSizeType.Portrait21_9
                    Return New Size(22, 52)
                Case MatrixSizeType.Landscape32_9
                    Return New Size(64, 18)
                Case Else 'Portrait32_9
                    Return New Size(18, 64)
            End Select
        End Get
    End Property

    Public ReadOnly Property LEDCount() As Integer
        Get
            Return MatrixSize.Width * MatrixSize.Height
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
        'Dim hexString As String = BitConverter.ToString(data).Replace("-", " ")
        'Logger.Capture($"Raw data: {hexString}")

        Try
            If data.Length < 8 Then
                Return
            End If

            ParseRGBCommands(data)
        Catch ex As Exception
            Logger.Log($"Error processing packet: {ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Private Sub ParseRGBCommands(data As Byte())
        If data.Length >= 8 Then
            Try
                _colors.Clear()

                _matrixSizeType = CInt(data(0))
                Dim newData = data.Skip(3).ToArray

                For i As Integer = 0 To newData.Length - 1 Step 3
                    Dim r As Byte = newData(i)
                    Dim g As Byte = newData(i + 1)
                    Dim b As Byte = newData(i + 2)
                    _colors.Add(Color.FromArgb(r, g, b))
                Next
            Catch ex As Exception
                Logger.Log($"Error parsing packet: {ex.Message} {ex.StackTrace}")
            End Try
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
