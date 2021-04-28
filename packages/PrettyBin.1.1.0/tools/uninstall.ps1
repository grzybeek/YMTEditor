param($installPath, $toolsPath, $package, $project)


Try
{
     $path =  $project.FullName
     Write-Host 'Uninstall PrettifyBin; '
     [ xml ]$doc =  [ xml ](Get-Content $path)


    # If a Namespace URI was not given, use the Xml document's default namespace.
    $NamespaceURI = $doc.DocumentElement.NamespaceURI 
     Write-Host $NamespaceURI
    # In order for SelectSingleNode() to actually work, we need to use the fully qualified node path along with an Xml Namespace Manager, so set them up.
    $xmlNsManager = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
    $xmlNsManager.AddNamespace("ns", $NamespaceURI)


    #  ============        This script attemps to remove this tags in csproj: ========
    #
    # <Target Name="AfterBuild">
    #      <Move SourceFiles="@(MoveToLibFolder)" DestinationFolder="$(OutputPath)lib" OverwriteReadOnlyFiles="true" />
    # </Target>


  	$moveNode =  $doc.SelectSingleNode("//ns:Project/ns:Target[@Name='AfterBuild']/ns:Move[contains(@SourceFiles,'@(MoveToLibFolder)')]", $xmlNsManager)
	 
    if ($moveNode  -ne $null)
	{
        Write-Host 'We will delete move node'
		$moveNode.ParentNode.RemoveChild($moveNode)
	}


    $doc.Save($path)
}
Catch
{
    $ErrorMessage = $_.Exception.Message
    $FailedItem = $_.Exception.ItemName
    Write-Host  $ErrorMessage
	 Write-Host   $FailedItem
    Break
}

