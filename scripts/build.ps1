$ROOT_PATH = (Resolve-Path -Path ($PSScriptRoot + '\..\')).Path
$BIN_PATH = ($ROOT_PATH + '\bin')
$PROJECT_NAME = 'RemoteKeycard'
$CONFIGS = ('RELEASE', 'DEBUG')

dotnet restore $ROOT_PATH

foreach ($CONFIG in $CONFIGS) {
    $OUTPUT_PATH = ($ROOT_PATH + "\$PROJECT_NAME" + '-' + $CONFIG + '')
    dotnet build $ROOT_PATH -c $CONFIG -o $OUTPUT_PATH

    Remove-Item -Path ($OUTPUT_PATH + '\*') -Recurse -Exclude *$PROJECT_NAME* -Force
    Compress-7Zip -ArchiveFileName ($OUTPUT_PATH + '.tar') -Path $OUTPUT_PATH -OutputPath $ROOT_PATH -Format 'Tar'
    Compress-7Zip -ArchiveFileName ($OUTPUT_PATH + '.tar.gz') -Path ($OUTPUT_PATH + '.tar') -OutputPath $ROOT_PATH -Format 'GZip' -CompressionLevel 'High'
    Remove-Item $OUTPUT_PATH -Recurse -Force

    # I literally don't know why `OutputPath` doesn't work correctly, I'll debug it later
    if (-not (Test-Path ($BIN_PATH))) { New-Item -Path $BIN_PATH -ItemType Container }
    Move-Item -Path ($OUTPUT_PATH + '.tar.gz') -Destination $BIN_PATH -Force
}

#Remove-Item -Path ($ROOT_PATH + '\*') -Include '*.tar' -Recurse -Force
