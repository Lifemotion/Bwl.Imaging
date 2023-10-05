<TestClass()> Public Class ParametersDictionaryTest

    <TestMethod()> Public Sub ParametersDictionaryEqualityTest1()
        Dim params1 = New ParametersDictionary()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionary)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryEqualityTest2()
        Dim params1 = New ParametersDictionary()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionaryList)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryEqualityTest3()
        Dim params1 = New ParametersDictionaryList()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionary)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryEqualityTest4()
        Dim params1 = New ParametersDictionaryList() 'Словарь первой версии (на основе списка)
        Dim params2 = New ParametersDictionary() 'Словарь второй версии (на основе словаря)

        'Заполнение словарей идентичными данными
        Dim param = New Parameter($"ParameterKey{1}", $"ParameterKey{1}")
        params1.Add(param)
        params2.Add(param)

        'Сравнение идентичности сериализации
        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)
        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryPerfomanceTest()
        Dim NItems = 20
        Dim NIters = 10000
        Dim rnd As New Random(Now.Ticks Mod Integer.MaxValue)

        Dim params1 = New ParametersDictionaryList() 'Словарь первой версии (на основе списка)
        Dim params2 = New ParametersDictionary() 'Словарь второй версии (на основе словаря)

        'Заполнение словарей идентичными данными
        Dim paramKeys As New List(Of String)
        For i = 1 To NItems
            Dim param = New Parameter($"ParameterKey{i}", $"ParameterKey{i}")
            params1.Add(param)
            params2.Add(param)
            paramKeys.Add(param.Key)
        Next

        'Тест словаря первой версии
        Dim sw1 As New Stopwatch()
        sw1.Start()
        For i = 1 To NIters
            Dim rndIdx = rnd.Next Mod paramKeys.Count
            Dim rndKey = paramKeys(rndIdx)
            Dim rndValue = params1.ItemValue(rndKey)
        Next
        sw1.Stop()
        Dim t1 = sw1.Elapsed.TotalMilliseconds

        'Тест словаря второй версии
        Dim sw2 As New Stopwatch()
        sw2.Start()
        For i = 1 To NIters
            Dim rndIdx = rnd.Next Mod paramKeys.Count
            Dim rndKey = paramKeys(rndIdx)
            Dim rndValue = params2.ItemValue(rndKey)
        Next
        sw2.Stop()
        Dim t2 = sw2.Elapsed.TotalMilliseconds

        'Коэффициент ускорения словаря второй версии
        Dim spdX = t1 / t2
        If spdX < 10 Then
            Throw New Exception("spdX < 10")
        End If
    End Sub
End Class
