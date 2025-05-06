Imports System.Configuration
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Net.Sockets

Public Class SignalRGBClient

    Private udpClient As UdpClient
    Private listenPort As Integer = 8123 ' Default port
    Private _isListening As Boolean = False

    Public Event SettingsChanged(sender As Object, e As SignalRGBSettingsChangedEventArgs)

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
                    Return New Size(32, 24)
                Case MatrixSizeType.Portrait4_3
                    Return New Size(24, 32)
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
                    Return New Size(63, 27)
                Case MatrixSizeType.Portrait21_9
                    Return New Size(27, 63)
                Case MatrixSizeType.Landscape32_9
                    Return New Size(96, 27)
                Case Else 'Portrait32_9
                    Return New Size(27, 96)
            End Select
        End Get
    End Property

    Private _smoothingMode As SmoothingMode
    Public ReadOnly Property SmoothingMode() As Drawing2D.SmoothingMode
        Get
            Return _smoothingMode
        End Get
    End Property

    Private _compositingQuality As CompositingQuality
    Public ReadOnly Property CompositingQuality() As Drawing2D.CompositingQuality
        Get
            Return _compositingQuality
        End Get
    End Property

    Private _interpolationMode As InterpolationMode
    Public ReadOnly Property InterpolationMode() As Drawing2D.InterpolationMode
        Get
            Return _interpolationMode
        End Get
    End Property

    Private _pixelOffsetMode As PixelOffsetMode
    Public ReadOnly Property PixelOffsetMode() As Drawing2D.PixelOffsetMode
        Get
            Return _pixelOffsetMode
        End Get
    End Property

    Private _ledShape As LEDShape
    Public ReadOnly Property LEDShape() As LEDShape
        Get
            Return _ledShape
        End Get
    End Property

    Private _roundedRectangleCornerRadius As Integer
    Public ReadOnly Property RoundedRectangleCornerRadius() As Integer
        Get
            Return _roundedRectangleCornerRadius
        End Get
    End Property

    Private _ledPadding As Single
    Public ReadOnly Property LEDPadding() As Single
        Get
            Return _ledPadding
        End Get
    End Property

    Private _ledUpdateInterval As Integer
    Public ReadOnly Property LEDUpdateInterval() As Integer
        Get
            Return _ledUpdateInterval
        End Get
    End Property

    Private _coverImageSizeMode As Integer
    Public ReadOnly Property CoverImageSizeMode() As Integer
        Get
            Return _coverImageSizeMode
        End Get
    End Property

    Private _backgroundColor As Color
    Public ReadOnly Property BackgroundColor() As Color
        Get
            Return _backgroundColor
        End Get
    End Property

    Private _cpuUsagePauseValue As Integer
    Public ReadOnly Property CPUUsagePauseValue() As Integer
        Get
            Return _cpuUsagePauseValue
        End Get
    End Property

    Public Sub New(save As UserSave, Optional port As Integer = 8123)
        listenPort = port
        _matrixSizeType = save.MatrixSizeType
        _smoothingMode = save.SmoothingMode
        _compositingQuality = save.CompositingQuality
        _interpolationMode = save.InterpolationMode
        _pixelOffsetMode = save.PixelOffsetMode
        _ledShape = save.LedShape
        _roundedRectangleCornerRadius = save.RoundedRectangleCornerRadius
        _ledPadding = save.LedPadding
        _ledUpdateInterval = save.LedUpdateInterval
        _coverImageSizeMode = save.CoverImageSizeMode
        _cpuUsagePauseValue = save.CpuUsagePauseValue
        _backgroundColor = save.BackgroundColor
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

    Private CombinedData As New List(Of Byte)

    Private Sub ProcessSignalRGBPacket(data As Byte(), sender As IPEndPoint)
        Try
            If data.Length < 8 Then
                Return
            End If

            Select Case CInt(data(0))
                Case 0
                    'Dim hexString As String = BitConverter.ToString(CombinedData.ToArray).Replace("-", " ")
                    'Logger.Capture($"Raw RGB data: {hexString}")

                    Dim currPacket As Integer = CInt(data(1))
                    Dim numPackets As Integer = CInt(data(2))
                    '_currentPacket = currPacket
                    '_numberOfPackets = numPackets

                    If currPacket = numPackets - 1 Then
                        CombinedData.AddRange(data.Skip(3))
                        ParseRGBCommands(CombinedData.ToArray)
                        CombinedData.Clear()
                    Else
                        CombinedData.AddRange(data.Skip(3))
                    End If
                Case 1
                    ParseSettingCommands(data)
            End Select
        Catch ex As Exception
            Logger.Log($"Error processing packet: {ex.Message} {ex.StackTrace}")
        End Try
    End Sub

    Private Sub ParseSettingCommands(data As Byte())
        'Dim hexString As String = BitConverter.ToString(data).Replace("-", " ")
        'Logger.Capture($"Raw settings data: {hexString}")

        If data.Length >= 8 Then
            Try
                _matrixSizeType = CInt(data(1))
                _smoothingMode = CInt(data(2))
                _compositingQuality = CInt(data(3))
                _interpolationMode = CInt(data(4))
                _pixelOffsetMode = CInt(data(5))
                _ledShape = CInt(data(6))
                _roundedRectangleCornerRadius = CInt(data(7))
                _ledPadding = CInt(data(8))
                _ledUpdateInterval = CInt(data(9))
                _coverImageSizeMode = CInt(data(10))
                _cpuUsagePauseValue = CInt(data(11))
                _backgroundColor = Color.FromArgb(data(12), data(13), data(14))

                Dim eventArgs = New SignalRGBSettingsChangedEventArgs(_matrixSizeType, _smoothingMode, _compositingQuality, _interpolationMode, _pixelOffsetMode, _ledShape,
                                                                      _roundedRectangleCornerRadius, _ledPadding, _ledUpdateInterval, _coverImageSizeMode, _backgroundColor, _cpuUsagePauseValue)
                RaiseEvent SettingsChanged(Me, eventArgs)
            Catch ex As Exception
                Logger.Log($"Error parsing packet: {ex.Message} {ex.StackTrace}")
            End Try
        End If
    End Sub

    Private Sub ParseRGBCommands(data As Byte())
        If data.Length >= 8 Then
            Try
                _colors.Clear()

                For i As Integer = 0 To data.Length - 1 Step 3
                    Dim r As Byte = data(i)
                    Dim g As Byte = data(i + 1)
                    Dim b As Byte = data(i + 2)
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

Public Class SignalRGBSettingsChangedEventArgs
    Inherits EventArgs

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

    Private _smoothingMode As SmoothingMode
    Public ReadOnly Property SmoothingMode() As Drawing2D.SmoothingMode
        Get
            Return _smoothingMode
        End Get
    End Property

    Private _compositingQuality As CompositingQuality
    Public ReadOnly Property CompositingQuality() As Drawing2D.CompositingQuality
        Get
            Return _compositingQuality
        End Get
    End Property

    Private _interpolationMode As InterpolationMode
    Public ReadOnly Property InterpolationMode() As Drawing2D.InterpolationMode
        Get
            Return _interpolationMode
        End Get
    End Property

    Private _pixelOffsetMode As PixelOffsetMode
    Public ReadOnly Property PixelOffsetMode() As Drawing2D.PixelOffsetMode
        Get
            Return _pixelOffsetMode
        End Get
    End Property

    Private _ledShape As LEDShape
    Public ReadOnly Property LEDShape() As LEDShape
        Get
            Return _ledShape
        End Get
    End Property

    Private _roundedRectangleCornerRadius As Integer
    Public ReadOnly Property RoundedRectangleCornerRadius() As Integer
        Get
            Return _roundedRectangleCornerRadius
        End Get
    End Property

    Private _ledPadding As Single
    Public ReadOnly Property LEDPadding() As Single
        Get
            Return _ledPadding
        End Get
    End Property

    Private _ledUpdateInterval As Integer
    Public ReadOnly Property LEDUpdateInterval() As Integer
        Get
            Return _ledUpdateInterval
        End Get
    End Property

    Private _coverImageSizeMode As Integer
    Public ReadOnly Property CoverImageSizeMode() As Integer
        Get
            Return _coverImageSizeMode
        End Get
    End Property

    Private _backgroundColor As Color
    Public ReadOnly Property BackgroundColor() As Color
        Get
            Return _backgroundColor
        End Get
    End Property

    Private _cpuUsagePauseValue As Integer
    Public ReadOnly Property CPUUsagePauseValue() As Integer
        Get
            Return _cpuUsagePauseValue
        End Get
    End Property

    Public Sub New(matrixSizeType As MatrixSizeType, smoothingMode As SmoothingMode, compositingQuality As CompositingQuality, interpolationMode As InterpolationMode, pixelOffsetMode As PixelOffsetMode,
                   ledShape As LEDShape, roundedRectangleCornerRadius As Integer, ledPadding As Single, ledUpdateInterval As Integer, coverImageSizeMode As Integer, backgroundColor As Color,
                   cpuUsagePauseValue As Integer)
        _matrixSizeType = matrixSizeType
        _smoothingMode = smoothingMode
        _compositingQuality = compositingQuality
        _interpolationMode = interpolationMode
        _pixelOffsetMode = pixelOffsetMode
        _ledShape = ledShape
        _roundedRectangleCornerRadius = roundedRectangleCornerRadius
        _ledPadding = ledPadding
        _ledUpdateInterval = ledUpdateInterval
        _coverImageSizeMode = coverImageSizeMode
        _backgroundColor = backgroundColor
        _cpuUsagePauseValue = cpuUsagePauseValue
    End Sub

End Class