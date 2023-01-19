Imports DevExpress.Data.Filtering
Imports DevExpress.Data.Filtering.Helpers
Imports System.Linq

Namespace FilterDropDown_AgregateOperators.Behaviours

    Public Class InToAggregatePatcher
        Inherits ClientCriteriaLazyPatcherBase.AggregatesCommonProcessingBase

        Public Shared FieldName As String

        Public Shared DataItemId As String

        Public Shared Function Patch(ByVal source As CriteriaOperator) As CriteriaOperator
            Return New InToAggregatePatcher().Process(source)
        End Function

        Public Overrides Function Visit(ByVal theOperator As InOperator) As CriteriaOperator
            Dim result = CType(MyBase.Visit(theOperator), InOperator)
            Dim [property] = TryCast(result.LeftOperand, OperandProperty)
            If Equals([property]?.PropertyName, FieldName) AndAlso result.Operands.All(Function(c) TypeOf c Is OperandValue) Then
                Dim items = result.Operands.Cast(Of OperandValue)().[Select](Function(c) c.Value)
                Dim ItemValues = items.[Select](Function(item) New OperandValue(item.GetType().GetProperty(DataItemId).GetValue(item)))
                Dim inOperator = New InOperator(New OperandProperty(DataItemId), ItemValues)
                Dim newOperator = CriteriaOperator.Parse(FieldName & "[" & inOperator.ToString() & "]")
                Return newOperator
            End If

            Return result
        End Function
    End Class
End Namespace
