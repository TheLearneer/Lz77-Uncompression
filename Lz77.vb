﻿Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Module Lz77
    
    Public Function Hexstringtobytearray(ByRef stringdata As String) As Byte()
        'converting string to byte array
        Dim bytearr As New List(Of Byte)
        Dim bytefinal As Byte()
        'the actual conversion
        For i As Integer = 1 To Len(stringdata) Step 2
            bytearr.Add(CByte(Int32.Parse((Mid(stringdata, i, 2)), Globalization.NumberStyles.HexNumber)))
        Next
        bytefinal = bytearr.ToArray
        Return bytefinal
    End Function

    Public Function ByteArrayToHexString(ByRef bytedata As Byte()) As String
        Dim strcon As String = ""
        For i As Integer = 0 To (bytedata.Length - 1)
            strcon = strcon & Right("00" & (Hex(bytedata(i))), 2)
        Next
        Return strcon
    End Function
    
Public Function Lz77Uncompression(ByRef CompressedHexstring As String) As String
        'declaring necessary variables
        Dim inputbyte As Byte() = Hexstringtobytearray(CompressedHexstring)
        Dim sizeofcompression As Integer = Int32.Parse((ReverseHEX(Mid(CompressedHexstring, 3, 6))), Globalization.NumberStyles.HexNumber)
        Dim outputbyte As New List(Of Byte)
        Dim headerposition As Integer = 4 'the position that shows the beginning of every block header
        Dim blockheaderbyte As Byte
        Dim checkingnum As Integer = &H80
        Dim currentposition As Integer = 0
        Dim bytedone As Integer = 0
        Dim additionpostion As Integer = 1
        Dim data As Integer
        Dim length As Integer
        Dim backpos As Integer

        'checking if the data provided is lz77 compressed or not....
        If inputbyte(0) <> &H10 Then
            GoTo errorr
        End If

        'the actual uncompress process
        Do While outputbyte.ToArray.Length < sizeofcompression
            blockheaderbyte = inputbyte(headerposition)

            additionpostion = 1
            checkingnum = &H80
            Do While (bytedone < 8)
                'MsgBox("bytedone - " & bytedone)
                If (blockheaderbyte And checkingnum) <> 0 Then 'this is the compressed byte and is to be copied by checking length and back position
                    'getting the length and position to go back to copy the data
                    data = ((inputbyte(headerposition + additionpostion) * (2 ^ 8)) Or inputbyte(headerposition + additionpostion + 1))
                    length = (data \ 4096) + 3
                    backpos = (data And &HFFF) + 1

                    'MsgBox("data - " & Hex(data))
                    Dim postfrombeg As Integer
                    postfrombeg = outputbyte.Count - backpos
                    Dim backbuffer As New List(Of Byte)
                    'copying the data

                    While (backbuffer.Count < length)
                        backbuffer.Add(outputbyte(postfrombeg))
                        postfrombeg = postfrombeg + 1

                        If (postfrombeg >= outputbyte.Count) Then
                            postfrombeg = outputbyte.Count - backpos
                        End If
                    End While
                    outputbyte.AddRange(backbuffer)

                    'doing this next line for skipping 2 compressed bytes
                    additionpostion = additionpostion + 2

                Else 'this is the uncompressed byte and is to be copied directly

                    'MsgBox("single byte - " & Hex(inputbyte(headerposition + additionpostion)))
                    If (headerposition + additionpostion) >= inputbyte.Length Then
                        GoTo result
                    End If

                    outputbyte.Add(inputbyte(headerposition + additionpostion))
                    additionpostion = additionpostion + 1

                End If

                checkingnum = checkingnum / 2
                bytedone = bytedone + 1
            Loop
            ' MsgBox("addition position - " & additionpostion)
            headerposition = headerposition + additionpostion
            bytedone = 0
        Loop

result:
        Return ByteArrayToHexString(outputbyte.ToArray)
        Exit Function

Errorr:
        MsgBox("The data provided isnt Lz77 compressed")
        Return ""
    End Function

End Module
