Imports System.Runtime.CompilerServices
Imports NAudio.CoreAudioApi
Imports NAudio.Dsp
Imports NAudio.Wave

Module OfflineAudioEffects



End Module

Public Module AudioEngine
    Private capture As WasapiLoopbackCapture
    Public Property AudioBins As Single() = New Single(199) {}
    Public Property IsActive As Boolean = False

    Private Const FFT_SIZE As Integer = 1024
    Private fftBuffer As Complex() = New Complex(FFT_SIZE - 1) {}
    Private fftPos As Integer = 0

    Public Sub Start()
        If IsActive Then Return ' Already running
        Try
            Dim enumerator As New MMDeviceEnumerator()
            Dim device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
            capture = New WasapiLoopbackCapture(device)
            AddHandler capture.DataAvailable, AddressOf ProcessAudio
            capture.StartRecording()
            IsActive = True
        Catch ex As Exception
            Debug.Print("Audio Engine Error: " & ex.Message)
            IsActive = False
        End Try
    End Sub

    Public Sub [Stop]()
        IsActive = False
        If capture IsNot Nothing Then
            capture.StopRecording()
            RemoveHandler capture.DataAvailable, AddressOf ProcessAudio
            capture.Dispose()
            capture = Nothing
        End If
        ' Clear bins so lights don't get stuck on
        Array.Clear(AudioBins, 0, AudioBins.Length)
    End Sub

    Private Sub ProcessAudio(sender As Object, e As WaveInEventArgs)
        If Not IsActive Then Return

        ' Copy data quickly to a local array to free up the NAudio buffer
        Dim bytesToProcess = e.BytesRecorded
        Dim localBuffer = New Byte(bytesToProcess - 1) {}
        Array.Copy(e.Buffer, localBuffer, bytesToProcess)

        ' Offload the heavy FFT math to a background thread to keep FPS at 60
        Task.Run(Sub()
                     Dim waveBuffer = New WaveBuffer(localBuffer)
                     Dim samplesCount = bytesToProcess / 4

                     For i As Integer = 0 To samplesCount - 1
                         Dim sample As Single = waveBuffer.FloatBuffer(i)
                         Dim windowed As Double = sample * FastFourierTransform.HammingWindow(fftPos, FFT_SIZE)

                         fftBuffer(fftPos).X = CSng(windowed)
                         fftBuffer(fftPos).Y = 0
                         fftPos += 1

                         If fftPos >= FFT_SIZE Then
                             FastFourierTransform.FFT(True, 10, fftBuffer)

                             Dim halfSize As Integer = FFT_SIZE \ 2
                             Dim binsPerIndex As Double = halfSize / 200.0

                             For j As Integer = 0 To 199
                                 Dim startIdx = CInt(j * binsPerIndex)
                                 Dim endIdx = Math.Min(CInt((j + 1) * binsPerIndex), halfSize - 1)
                                 Dim maxMag As Single = 0

                                 For k As Integer = startIdx To endIdx
                                     ' Complex.X^2 + Y^2 is faster than Math.Sqrt if you just need a relative level
                                     Dim mag = CSng(Math.Sqrt(fftBuffer(k).X ^ 2 + fftBuffer(k).Y ^ 2))
                                     If mag > maxMag Then maxMag = mag
                                 Next

                                 ' Update the public bins (Thread-safe-ish for visualization)
                                 AudioBins(j) = Math.Max(maxMag, AudioBins(j) * 0.8F)
                             Next
                             fftPos = 0
                         End If
                     Next
                 End Sub)
    End Sub
End Module