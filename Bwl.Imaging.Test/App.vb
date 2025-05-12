Imports System.Windows.Forms

Module App

    Sub Main(args As String())
        Application.EnableVisualStyles()
        Dim [variant] = Int32.MaxValue
        While ([variant] > 0)
            Try
                Console.Clear()
                Console.WriteLine("Please select form for testing:")
                Console.WriteLine("0 - Exit app")
                Console.WriteLine("1 - HdrTestForm")
                Console.WriteLine("2 - MoveDetectorTest")
                Console.WriteLine("3 - TestForm")
                Console.WriteLine("4 - TestForm2")
                Console.WriteLine("5  - UnsafeTestForm")
                Console.Write("Your value: ")
                Dim consoleInput = Console.ReadLine()
                Dim input = Integer.TryParse(consoleInput, [variant])
                If Not input Then Throw New ArgumentException("Please input the correct string")
                Select Case [variant]
                    Case 0
                        Exit While
                    Case 1
                        Application.Run(New HdrTestForm)
                    Case 2
                        Application.Run(New MoveDetectorTest)
                    Case 3
                        Application.Run(New TestForm)
                    Case 4
                        Application.Run(New TestForm2)
                    Case 5
                        Application.Run(New UnsafeTestForm)
                    Case Else
                        Throw New ArgumentException("No such option is available")
                End Select
            Catch ex As Exception
                Console.WriteLine($"ERROR: {ex.Message}")
                [variant] = Int32.MaxValue
            End Try
            Console.WriteLine("Press Enter to continue...")
            Console.ReadLine()
        End While

        Application.Exit()

    End Sub
End Module
