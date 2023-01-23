Imports DevExpress.Mvvm
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace FilterDropDown_AgregateOperators.ViewModels

    Public Class MainViewModel
        Inherits ViewModelBase

        Public Sub New()
            Genres = GridData.GetGenres()
            Items = GridData.GetDataItems(Genres)
        End Sub

        Protected _Items As ObservableCollection(Of ClassName)

        Public Property Items As ObservableCollection(Of ClassName)
            Get
                Return _Items
            End Get

            Set(ByVal value As ObservableCollection(Of ClassName))
                SetProperty(_Items, value, "Items")
            End Set
        End Property

        Protected _Genres As List(Of Object)

        Public Property Genres As List(Of Object)
            Get
                Return _Genres
            End Get

            Set(ByVal value As List(Of Object))
                SetProperty(_Genres, value, "Genres")
            End Set
        End Property
    End Class

    Public Class GridData

        Public Shared Function GetGenres() As List(Of Object)
            Return New List(Of Object)(Enumerable.Range(0, 10).[Select](Function(c) New Genre() With {.Value = c, .Name = "Genre #" & c}))
        End Function

        Public Shared Function GetDataItems(ByVal genres As IList(Of Object)) As ObservableCollection(Of ClassName)
            Dim data As ObservableCollection(Of ClassName) = New ObservableCollection(Of ClassName)()
            Dim r As Random = New Random()
            Dim i As Integer = -1
            While Threading.Interlocked.Increment(i) < 150
                Dim value As ClassName = New ClassName()
                value.Name = String.Format("Name #{0}", r.Next(100))
                value.Age = r.Next(40) + 20
                Dim index1 = r.Next(1, genres.Count)
                Dim index2 = r.Next(0, index1)
                value.Genres = New List(Of Object)() From {genres(index1), genres(index2)}
                value.DateTime = Date.Today.AddDays(r.Next(30) - 15)
                value.IsSelected = r.Next(2) > 0
                data.Add(value)
            End While

            Return data
        End Function
    End Class
End Namespace
