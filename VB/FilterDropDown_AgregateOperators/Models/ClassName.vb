Imports DevExpress.Mvvm

Namespace FilterDropDown_AgregateOperators

    Public Class ClassName
        Inherits BindableBase

        Protected _Name As String

        Public Property Name As String
            Get
                Return _Name
            End Get

            Set(ByVal value As String)
                SetProperty(_Name, value, "Name")
            End Set
        End Property

        Protected _Age As Integer

        Public Property Age As Integer
            Get
                Return _Age
            End Get

            Set(ByVal value As Integer)
                SetProperty(_Age, value, "Age")
            End Set
        End Property

        Protected _Genres As Object

        Public Property Genres As Object
            Get
                Return _Genres
            End Get

            Set(ByVal value As Object)
                SetProperty(_Genres, value, "Genres")
            End Set
        End Property

        Protected _DateTime As Date

        Public Property DateTime As Date
            Get
                Return _DateTime
            End Get

            Set(ByVal value As Date)
                SetProperty(_DateTime, value, "DateTime")
            End Set
        End Property

        Protected _IsSelected As Boolean

        Public Property IsSelected As Boolean
            Get
                Return _IsSelected
            End Get

            Set(ByVal value As Boolean)
                SetProperty(_IsSelected, value, "IsSelected")
            End Set
        End Property
    End Class
End Namespace
