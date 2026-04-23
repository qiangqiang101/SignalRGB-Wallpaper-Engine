Public Class LedCanvas
    Inherits ContentControl

    Public Property Client As SignalRGBClient

    Private _currentFpsDisplay As String = "0 FPS"
    Public ReadOnly Property CurrentFps As String
        Get
            Return _currentFpsDisplay
        End Get
    End Property

    Private _stableColorSnapshot As Color() = Array.Empty(Of Color)
    Private _brushCache As New Dictionary(Of Color, SolidColorBrush)
    Private fpsWatch As New Stopwatch()

    Private _lastFpsUpdateTime As Double = 0
    Private _frameCount As Integer = 0
    Private _lastRenderTime As Double = 0

    Private Sub LedCanvas_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AddHandler CompositionTarget.Rendering, AddressOf OnRendering
    End Sub

    Private Sub LedCanvas_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        RemoveHandler CompositionTarget.Rendering, AddressOf OnRendering
        _brushCache.Clear()
    End Sub

    Private Sub OnRendering(sender As Object, e As EventArgs)
        If Client Is Nothing OrElse Not Client.IsListening Then Return

        Dim targetFps As Integer = If(Client.FPS > 0, Client.FPS * 1.5, 60)
        Dim minElapsedMs As Double = 1000.0 / targetFps

        If Not fpsWatch.IsRunning Then
            fpsWatch.Start()
            _lastRenderTime = 0
        End If

        Dim currentTime = fpsWatch.Elapsed.TotalMilliseconds
        Dim timeSinceLastDraw = currentTime - _lastRenderTime

        If timeSinceLastDraw >= minElapsedMs Then
            _lastRenderTime = currentTime

            _frameCount += 1
            If currentTime - _lastFpsUpdateTime >= 500 Then
                Dim actualFps As Double = (_frameCount * 1000.0) / (currentTime - _lastFpsUpdateTime)
                _currentFpsDisplay = $"{Math.Ceiling(actualFps)}"

                _frameCount = 0
                _lastFpsUpdateTime = currentTime
            End If

            Me.InvalidateVisual()
        End If
    End Sub

    Private Function GetBrush(color As Color) As SolidColorBrush
        If Not _brushCache.ContainsKey(color) Then
            If _brushCache.Count > 2000 Then _brushCache.Clear()

            Dim b As New SolidColorBrush(color)
            b.Freeze()
            _brushCache(color) = b
        End If
        Return _brushCache(color)
    End Function

    Protected Overrides Sub OnRender(dc As DrawingContext)
        MyBase.OnRender(dc)

        If Client Is Nothing OrElse Not Client.IsListening Then Return

        Dim w = Client.MatrixSize.Width
        Dim h = Client.MatrixSize.Height
        If w < 1 OrElse h < 1 Then Return

        Dim targetList = Client.Colors
        If targetList Is Nothing Then Return

        SyncLock targetList
            Try
                Dim totalLeds = w * h
                If _stableColorSnapshot.Length <> totalLeds Then
                    _stableColorSnapshot = New Color(totalLeds - 1) {}
                End If

                If targetList.Count > 0 Then
                    Dim actualCount = Math.Min(targetList.Count, _stableColorSnapshot.Length)
                    targetList.CopyTo(0, _stableColorSnapshot, 0, actualCount)
                End If
            Catch ex As Exception
                Return
            End Try
        End SyncLock

        Dim cellW As Double = Me.ActualWidth / w
        Dim cellH As Double = Me.ActualHeight / h
        Dim count As Integer = 0
        Dim lastValidColor As Color = Colors.Black

        For j As Integer = 0 To h - 1
            For i As Integer = 0 To w - 1
                If count >= _stableColorSnapshot.Length Then Exit For

                Dim col = _stableColorSnapshot(count)

                If col.A > 0 OrElse col.R > 0 OrElse col.G > 0 OrElse col.B > 0 Then
                    DrawLed(dc, i, j, col, cellW, cellH)
                    lastValidColor = col
                End If
                count += 1
            Next
        Next
    End Sub

    Private Sub DrawLed(dc As DrawingContext, i As Integer, j As Integer, color As Color, cellW As Double, cellH As Double)
        Dim brush = GetBrush(color)
        Dim P As Double = If(Client IsNot Nothing, Client.LEDPadding, 0)

        Dim x As Double = (cellW * i) + (P / 2)
        Dim y As Double = (cellH * j) + (P / 2)
        Dim drawW As Double = Math.Max(0, cellW - P)
        Dim drawH As Double = Math.Max(0, cellH - P)

        Dim rect As New Rect(x, y, drawW, drawH)

        Select Case If(Client IsNot Nothing, Client.LEDShape, LEDShape.Rectangle)
            Case LEDShape.Rectangle
                dc.DrawRectangle(brush, Nothing, rect)
            Case LEDShape.RoundedRectangle
                Dim radius As Double = Client.RoundedRectangleCornerRadius
                dc.DrawRoundedRectangle(brush, Nothing, rect, radius, radius)
            Case LEDShape.Sphere
                Dim center As New Point(x + (drawW / 2), y + (drawH / 2))
                dc.DrawEllipse(brush, Nothing, center, drawW / 2, drawH / 2)
        End Select
    End Sub

End Class
