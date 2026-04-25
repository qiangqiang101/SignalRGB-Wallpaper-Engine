Imports System.Net
Imports System.Net.Sockets

Public Class SignalRGBClient

    Private udpClient As UdpClient
    Private listenPort As Integer = 8133 ' Default port
    Private _isListening As Boolean = False
    Private _lastPacketTime As DateTime = DateTime.Now

    Public Event SettingsChanged(sender As Object, e As SignalRGBSettingsChangedEventArgs)

    Public ReadOnly Property IsListening As Boolean
        Get
            Return _isListening
        End Get
    End Property

    Public ReadOnly Property LastPacketTime As DateTime
        Get
            Return _lastPacketTime
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

    Private _matrixSizeTier As MatrixSizeTier
    Public ReadOnly Property MatrixSizeTier() As MatrixSizeTier
        Get
            Return _matrixSizeTier
        End Get
    End Property

    Public ReadOnly Property IsProtrait() As Boolean
        Get
            Select Case _matrixSizeType
                Case MatrixSizeType.Portrait4_1, MatrixSizeType.Portrait4_3, MatrixSizeType.Portrait5_4,
                     MatrixSizeType.Portrait16_9, MatrixSizeType.Portrait16_10, MatrixSizeType.Portrait21_9,
                     MatrixSizeType.Portrait32_9
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Public ReadOnly Property MatrixSize() As Size
        Get
            Select Case _matrixSizeType
                Case MatrixSizeType.Landscape4_1
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(8, 2)
                        Case MatrixSizeTier.Normal
                            Return New Size(16, 4)
                        Case MatrixSizeTier.Large
                            Return New Size(32, 8)
                        Case Else ' XLarge
                            Return New Size(64, 16)
                    End Select
                Case MatrixSizeType.Portrait4_1
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(2, 8)
                        Case MatrixSizeTier.Normal
                            Return New Size(4, 16)
                        Case MatrixSizeTier.Large
                            Return New Size(8, 32)
                        Case Else ' XLarge
                            Return New Size(16, 64)
                    End Select
                Case MatrixSizeType.Landscape4_3
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(8, 6)
                        Case MatrixSizeTier.Normal
                            Return New Size(16, 12)
                        Case MatrixSizeTier.Large
                            Return New Size(32, 24)
                        Case Else ' XLarge
                            Return New Size(64, 48)
                    End Select
                Case MatrixSizeType.Portrait4_3
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(6, 8)
                        Case MatrixSizeTier.Normal
                            Return New Size(12, 16)
                        Case MatrixSizeTier.Large
                            Return New Size(24, 32)
                        Case Else ' XLarge
                            Return New Size(48, 64)
                    End Select
                Case MatrixSizeType.Landscape5_4
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(10, 8)
                        Case MatrixSizeTier.Normal
                            Return New Size(20, 16)
                        Case MatrixSizeTier.Large
                            Return New Size(40, 32)
                        Case Else ' XLarge
                            Return New Size(80, 64)
                    End Select
                Case MatrixSizeType.Portrait5_4
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(8, 10)
                        Case MatrixSizeTier.Normal
                            Return New Size(16, 20)
                        Case MatrixSizeTier.Large
                            Return New Size(32, 40)
                        Case Else ' XLarge
                            Return New Size(64, 80)
                    End Select
                Case MatrixSizeType.Landscape16_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(32, 18)
                        Case MatrixSizeTier.Normal
                            Return New Size(48, 27)
                        Case MatrixSizeTier.Large
                            Return New Size(64, 36)
                        Case Else ' XLarge
                            Return New Size(128, 72)
                    End Select
                Case MatrixSizeType.Portrait16_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(18, 32)
                        Case MatrixSizeTier.Normal
                            Return New Size(27, 48)
                        Case MatrixSizeTier.Large
                            Return New Size(36, 64)
                        Case Else ' XLarge
                            Return New Size(72, 128)
                    End Select
                Case MatrixSizeType.Landscape16_10
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(32, 20)
                        Case MatrixSizeTier.Normal
                            Return New Size(48, 30)
                        Case MatrixSizeTier.Large
                            Return New Size(64, 40)
                        Case Else ' XLarge
                            Return New Size(128, 80)
                    End Select
                Case MatrixSizeType.Portrait16_10
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(20, 32)
                        Case MatrixSizeTier.Normal
                            Return New Size(30, 48)
                        Case MatrixSizeTier.Large
                            Return New Size(40, 64)
                        Case Else ' XLarge
                            Return New Size(80, 128)
                    End Select
                Case MatrixSizeType.Landscape21_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(42, 18)
                        Case MatrixSizeTier.Normal
                            Return New Size(63, 27)
                        Case MatrixSizeTier.Large
                            Return New Size(84, 36)
                        Case Else ' XLarge
                            Return New Size(168, 72)
                    End Select
                Case MatrixSizeType.Portrait21_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(18, 42)
                        Case MatrixSizeTier.Normal
                            Return New Size(27, 63)
                        Case MatrixSizeTier.Large
                            Return New Size(36, 48)
                        Case Else ' XLarge
                            Return New Size(72, 168)
                    End Select
                Case MatrixSizeType.Landscape32_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(64, 18)
                        Case MatrixSizeTier.Normal
                            Return New Size(96, 27)
                        Case MatrixSizeTier.Large
                            Return New Size(128, 36)
                        Case Else ' XLarge
                            Return New Size(256, 72)
                    End Select
                Case Else 'Portrait32_9
                    Select Case _matrixSizeTier
                        Case MatrixSizeTier.Small
                            Return New Size(18, 64)
                        Case MatrixSizeTier.Normal
                            Return New Size(27, 96)
                        Case MatrixSizeTier.Large
                            Return New Size(36, 128)
                        Case Else ' XLarge
                            Return New Size(72, 256)
                    End Select
            End Select
        End Get
    End Property

    Private _ShutdownEffect As ShutdownEffect
    Public ReadOnly Property ShutdownEffect() As ShutdownEffect
        Get
            Return _ShutdownEffect
        End Get
    End Property

    Private _showFps As Boolean
    Public ReadOnly Property ShowFps() As Boolean
        Get
            Return _showFps
        End Get
    End Property

    Private _BlurIntensity As Integer
    Public ReadOnly Property BlurIntensity() As Integer
        Get
            Return _BlurIntensity
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

    Private _FPS As Integer
    Public ReadOnly Property FPS() As Integer
        Get
            Return _FPS
        End Get
    End Property

    Private _CoverImageStretch As Stretch
    Public ReadOnly Property CoverImageStretch() As Stretch
        Get
            Return _CoverImageStretch
        End Get
    End Property

    Private _backgroundColor As Color
    Public ReadOnly Property BackgroundColor() As Color
        Get
            Return _backgroundColor
        End Get
    End Property

    Private _shutdownColor As Color
    Public ReadOnly Property ShutdownColor() As Color
        Get
            Return _shutdownColor
        End Get
    End Property

    Private _cpuUsagePauseValue As Integer
    Public ReadOnly Property CPUUsagePauseValue() As Integer
        Get
            Return _cpuUsagePauseValue
        End Get
    End Property

    Private _coverImage As String
    Public ReadOnly Property CoverImage() As String
        Get
            Return _coverImage
        End Get
    End Property

    Public Sub New(save As UserSave, Optional port As Integer = 8133)
        listenPort = port
        _matrixSizeType = save.MatrixSizeType
        _matrixSizeTier = save.MatrixSizeTier
        _ShutdownEffect = save.ShutdownEffect
        _showFps = save.ShowFps
        _BlurIntensity = save.BlurIntensity
        _ledShape = save.LedShape
        _roundedRectangleCornerRadius = save.RoundedRectangleCornerRadius
        _ledPadding = save.LedPadding
        _FPS = save.FPS
        _CoverImageStretch = save.CoverImageStretch
        _cpuUsagePauseValue = save.CpuUsagePauseValue
        _backgroundColor = save.BackgroundColor
        _shutdownColor = save.ShutdownColor
        _coverImage = save.CoverImage
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
                    _lastPacketTime = DateTime.Now
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
                _matrixSizeTier = CInt(data(2))
                _ShutdownEffect = CInt(data(3))
                _showFps = CBool(data(4))
                _BlurIntensity = CInt(data(5))
                _ledShape = CInt(data(6))
                _roundedRectangleCornerRadius = CInt(data(7))
                _ledPadding = CInt(data(8))
                _FPS = CInt(data(9))
                _CoverImageStretch = CInt(data(10))
                _cpuUsagePauseValue = CInt(data(11))
                _backgroundColor = Color.FromRgb(data(12), data(13), data(14))
                _shutdownColor = Color.FromRgb(data(15), data(16), data(17))
                _coverImage = Text.Encoding.UTF8.GetString(data.Skip(19).ToArray()).TrimEnd(Chr(0)) 'Skip 1 byte for length

                Dim eventArgs = New SignalRGBSettingsChangedEventArgs(_matrixSizeType, _matrixSizeTier, _ShutdownEffect, _showFps, _BlurIntensity, _ledShape, _roundedRectangleCornerRadius,
                                                                      _ledPadding, _FPS, _CoverImageStretch, _backgroundColor, _cpuUsagePauseValue, _coverImage, _shutdownColor)
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
                    _colors.Add(Color.FromRgb(r, g, b))
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

    Private _matrixSizeTier As MatrixSizeTier
    Public ReadOnly Property MatrixSizeTier() As MatrixSizeTier
        Get
            Return _matrixSizeTier
        End Get
    End Property

    Private _ShutdownEffect As ShutdownEffect
    Public ReadOnly Property ShutdownEffect() As ShutdownEffect
        Get
            Return _ShutdownEffect
        End Get
    End Property

    Private _showFps As Boolean
    Public ReadOnly Property ShowFps() As Boolean
        Get
            Return _showFps
        End Get
    End Property

    Private _BlurIntensity As Integer
    Public ReadOnly Property BlurIntensity() As Integer
        Get
            Return _BlurIntensity
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

    Private _FPS As Integer
    Public ReadOnly Property FPS() As Integer
        Get
            Return _FPS
        End Get
    End Property

    Private _CoverImageStretch As Stretch
    Public ReadOnly Property CoverImageStretch() As Stretch
        Get
            Return _CoverImageStretch
        End Get
    End Property

    Private _backgroundColor As Color
    Public ReadOnly Property BackgroundColor() As Color
        Get
            Return _backgroundColor
        End Get
    End Property

    Private _shutdownColor As Color
    Public ReadOnly Property ShutdownColor() As Color
        Get
            Return _shutdownColor
        End Get
    End Property

    Private _cpuUsagePauseValue As Integer
    Public ReadOnly Property CPUUsagePauseValue() As Integer
        Get
            Return _cpuUsagePauseValue
        End Get
    End Property

    Private _coverImage As String
    Public ReadOnly Property CoverImage() As String
        Get
            Return _coverImage
        End Get
    End Property

    Public Sub New(matrixSizeType As MatrixSizeType, matrixSizeTier As MatrixSizeTier, ShutdownEffect As ShutdownEffect, showFps As Boolean, BlurIntensity As Integer,
                   ledShape As LEDShape, roundedRectangleCornerRadius As Integer, ledPadding As Single, FPS As Integer, CoverImageStretch As Stretch, backgroundColor As Color,
                   cpuUsagePauseValue As Integer, coverImage As String, shutdownColor As Color)
        _matrixSizeType = matrixSizeType
        _matrixSizeTier = matrixSizeTier
        _ShutdownEffect = ShutdownEffect
        _showFps = showFps
        _BlurIntensity = BlurIntensity
        _ledShape = ledShape
        _roundedRectangleCornerRadius = roundedRectangleCornerRadius
        _ledPadding = ledPadding
        _FPS = FPS
        _CoverImageStretch = CoverImageStretch
        _backgroundColor = backgroundColor
        _cpuUsagePauseValue = cpuUsagePauseValue
        _coverImage = coverImage
        _shutdownColor = shutdownColor
    End Sub

End Class