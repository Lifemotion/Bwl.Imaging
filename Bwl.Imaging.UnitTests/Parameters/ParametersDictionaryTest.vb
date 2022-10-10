<TestClass()> Public Class ParametersDictionaryTest

    <TestMethod()> Public Sub ParametersDictionaryTest1()
        Dim params1 = New ParametersDictionary()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionary)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryTest2()
        Dim params1 = New ParametersDictionary()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionaryList)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub

    <TestMethod()> Public Sub ParametersDictionaryTest3()
        Dim params1 = New ParametersDictionaryList()

        params1.Add(New Parameter("key1", "value1"))
        params1.Add(New Parameter("key2", "value2"))
        params1.Add(New Parameter("key3", "value3"))

        Dim paramsJson1 = Serializer.SaveObjectToJsonString(params1)

        Dim params2 = Serializer.LoadObjectFromJsonString(Of ParametersDictionary)(paramsJson1)
        Dim paramsJson2 = Serializer.SaveObjectToJsonString(params2)

        Assert.AreEqual(paramsJson1, paramsJson2)
    End Sub
End Class
