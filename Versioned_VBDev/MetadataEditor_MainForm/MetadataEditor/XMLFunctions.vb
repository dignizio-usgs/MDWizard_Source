Imports System.Xml

Module XMLFunctions

    'to get node value
    Public Function getNodeText(xDoc As XmlDocument, xpath As String)
        Try
            Return xDoc.SelectSingleNode(xpath).FirstChild.Value
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    'get all values from nodes at a specific xpath, when node element is repeating
    Public Function getMultiNodeValues(xdoc As XmlDocument, xpath As String) As Collection
        Try
            Dim multiNodeList As XmlNodeList = xdoc.SelectNodes(xpath)
            Dim multiValueList As New Collection

            Dim node As XmlNode
            For Each node In multiNodeList
                Dim iValue As String
                iValue = node.FirstChild.Value
                multiValueList.Add(iValue)
            Next node

            Return multiValueList

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    'check if node exists
    Public Function nodeExists(xDoc As XmlDocument, xPath As String) As Boolean
        Try
            Dim node As XmlNode
            node = xDoc.SelectSingleNode(xPath)

            If Not IsNothing(node) Then
                Return True
            End If

            If IsNothing(node) Then
                Return False
            End If
        Catch ex As Exception

        End Try
        Return Nothing

    End Function

    'to set node value
    Public Sub setNodeText(xDoc As XmlDocument, xPath As String, nodeText As String,
                                 Optional addIfMissing As Boolean = True)
        Dim node As XmlNode
        node = xDoc.SelectSingleNode(xPath)
        If IsNothing(node) And addIfMissing Then
            node = AddNode(xDoc, xPath, True)
        End If

        node.InnerText = nodeText
    End Sub

    'get parent node of an element
    Public Function getParentNode(xDoc As XmlDocument, xPath As String, Optional addMissingParents As Boolean = False)

        Dim parentXpath As String = xPath.Substring(0, InStrRev(xPath, "/") - 1)
        Dim parentNode As XmlNode = xDoc.SelectSingleNode(parentXpath)

        If IsNothing(parentNode) And addMissingParents Then
            parentNode = AddNode(xDoc, parentXpath, True)
        End If

        Return parentNode

    End Function

    'to add node
    Public Function AddNode(xDoc As XmlDocument, xPath As String, Optional addMissingParents As Boolean = True) As XmlNode

        Dim parentNode As XmlNode = Nothing
        Dim xpathTokens() As String = xPath.Split("/")

        Dim lastParentXpath As String = xpathTokens(0)
        Dim lastParentNode As XmlNode = xDoc.SelectSingleNode(xpathTokens(0))


        For i As Integer = 1 To xpathTokens.Length - 1
            lastParentXpath += "/" + xpathTokens(i)
            parentNode = xDoc.SelectSingleNode(lastParentXpath)
            If IsNothing(parentNode) Then
                Dim missingParent As XmlNode = xDoc.CreateElement(xpathTokens(i))
                lastParentNode.AppendChild(missingParent)
                parentNode = xDoc.SelectSingleNode(lastParentXpath)
            End If
            lastParentNode = parentNode
        Next

        Return parentNode
    End Function

    'to remove a node
    Public Sub removeNode(xDoc As XmlDocument, xpath As String)

        Dim node As XmlNode
        node = xDoc.SelectSingleNode(xpath)
        Try
            node.ParentNode.RemoveChild(node)
        Catch ex As Exception
        End Try
    End Sub

    Public Function getChildNodes(xDoc As XmlDocument, xpath As String) As XmlNodeList

        Try
            Dim childNodes As XmlNodeList = xDoc.SelectSingleNode(xpath).ChildNodes
            Return childNodes
        Catch ex As Exception
            Return Nothing
        End Try

    End Function


    Public Function getUpperLevelNode(xDoc As XmlDocument, xpath As String, Optional levelsUP As Integer = 1) As XmlNode

        Try
            Dim dgvNode As XmlNode = getParentNode(xDoc, xpath, True)

            If levelsUP >= 2 Then
                For i As Integer = 0 To (levelsUP - 1)
                    dgvNode = dgvNode.ParentNode

                Next
                Return dgvNode
            End If
            Return dgvNode

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function getNode(xDoc As XmlDocument, xpath As String) As XmlNode

        Try
            Dim Node As XmlNode = xDoc.SelectSingleNode(xpath)
            Return Node
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Sub deleteChildren(xDoc As XmlDocument, xpath As String, Optional matchName As String = "")

        Try
            Dim childList As XmlNodeList = getChildNodes(xDoc, xpath)
            Dim parentNode As XmlNode = childList(0).ParentNode

            Dim foundOne As Boolean = True
            While foundOne = True
                childList = getChildNodes(xDoc, xpath)
                foundOne = False
                For Each iChild In childList
                    If iChild.Name = matchName Or matchName = "" Then
                        foundOne = True
                        Try
                            parentNode.RemoveChild(iChild)
                        Catch ex As Exception
                        End Try
                    End If
                Next iChild
            End While

        Catch ex As Exception
        End Try
    End Sub


    '----------------------------------------------------------

    'FUNCTIONS THAT USE A SPECIFIC XML NODE INSTANCE (AND ELEMENTS WITHIN IT) RATHER THAN XPATH
    '   This handles scenarios like the "Source Info" element where there are multiple instances of a node (at the same xpath), each with sub content.

    'to get node value within a specific xml node instance
    Public Function getNodeTextAtNodeInstance(nodeInstance As XmlNode, xpath As String)
        Try
            Return nodeInstance.SelectSingleNode(xpath).FirstChild.Value
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    'get all values from specific node instance, when sub element is repeating
    Public Function getMultiNodeValuesAtNodeInstance(nodeInstance As XmlNode, xpath As String) As Collection
        Try
            Dim multiNodeList As XmlNodeList = nodeInstance.SelectNodes(xpath)
            Dim multiValueList As New Collection

            Dim node As XmlNode
            For Each node In multiNodeList
                Dim iValue As String
                iValue = node.FirstChild.Value
                multiValueList.Add(iValue)
            Next node

            Return multiValueList

        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    'check if node instance exists
    Public Function nodeInstanceExists(nodeInstance As XmlNode, xPath As String) As Boolean

        Dim node As XmlNode
        node = nodeInstance.SelectSingleNode(xPath)

        If Not IsNothing(node) Then
            Return True
        End If

        If IsNothing(node) Then
            Return False
        End If

        Return Nothing

    End Function



End Module

