#! pwsh

$ROOT_PATH = $PSScriptRoot + '\'
$BIN_PATH = ($ROOT_PATH + 'bin\')
$PROJECT_NAME = 'RemoteKeycard'
$CONFIGS = ('RELEASE', 'DEBUG')

dotnet restore $ROOT_PATH

foreach ($CONFIG in $CONFIGS) {
    $RELATIVE_OUTPUT_PATH = ($PROJECT_NAME + '-' + $CONFIG)
    $OUTPUT_PATH = ($ROOT_PATH + $RELATIVE_OUTPUT_PATH)
    dotnet build $ROOT_PATH -c $CONFIG -o $OUTPUT_PATH

    Remove-Item -Path ($OUTPUT_PATH + '\*') -Recurse -Exclude *$PROJECT_NAME* -Force

    Write-Warning "Bin path: $BIN_PATH"
    Write-Warning "Output path: $OUTPUT_PATH"
    Write-Warning "Relative output path: $RELATIVE_OUTPUT_PATH"

    $TAR_ARCHIVE_NAME = ($RELATIVE_OUTPUT_PATH + '.tar')
    Compress-7Zip -ArchiveFileName $TAR_ARCHIVE_NAME -Path $OUTPUT_PATH -OutputPath $ROOT_PATH -Format 'Tar'
    Compress-7Zip -ArchiveFileName ($RELATIVE_OUTPUT_PATH + '.tar.gz') -Path ($ROOT_PATH + $TAR_ARCHIVE_NAME) -OutputPath $BIN_PATH -Format 'GZip' -CompressionLevel 'High'
    Remove-Item $OUTPUT_PATH -Recurse -Force
}

Remove-Item -Path ($ROOT_PATH + '\*') -Include '*.tar' -Recurse -Force
