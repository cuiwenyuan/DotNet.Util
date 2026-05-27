<#
PowerShell script to replace C# usages of string.IsNullOrEmpty(...) / String.IsNullOrEmpty(...) with (expr).IsNullOrEmpty()

This variant preserves the original file encoding to avoid corrupting files with non-UTF8 encodings (for example files that contain Chinese characters in ANSI/GBK encoding).

Usage:
  powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\replace-string-isnullorempty3.ps1

Options:
  -WhatIf : preview changes (will list files and matched fragments, no file write)
  -Include : glob pattern for files to include (default: **/*.cs)
  -Backup  : when set, create a .bak copy of modified files (default: $true)

Note:
 - This script performs best-effort encoding detection: it checks for BOMs (UTF-8/16/32) and also heuristically detects UTF-8 without BOM. If neither applies it will treat the file as the system ANSI encoding (Encoding.Default) and write back using that encoding, preserving Chinese characters.
 - Review changes and run a build after applying.
#>

[CmdletBinding()]
param(
    [switch]$WhatIf,
    [string]$Include = "*.cs",
    [bool]$Backup = $false
)

Write-Host "Searching files matching: $Include"

$files = Get-ChildItem -Path . -Include $Include -Recurse -File
if ($files.Count -eq 0) {
    Write-Host "No files found. Exiting."
    exit 0
}

function Get-FileEncoding {
    param([byte[]]$bytes)

    # BOM checks
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        return @{ Encoding = [System.Text.Encoding]::UTF8; HasBom = $true }
    }
    if ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
        return @{ Encoding = [System.Text.Encoding]::Unicode; HasBom = $true } # UTF-16 LE
    }
    if ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
        return @{ Encoding = [System.Text.Encoding]::BigEndianUnicode; HasBom = $true } # UTF-16 BE
    }
    if ($bytes.Length -ge 4 -and $bytes[0] -eq 0x00 -and $bytes[1] -eq 0x00 -and $bytes[2] -eq 0xFE -and $bytes[3] -eq 0xFF) {
        return @{ Encoding = [System.Text.Encoding]::GetEncoding('utf-32'); HasBom = $true }
    }

    # Heuristic: detect UTF8 without BOM by round-trip
    try {
        $utf8 = [System.Text.Encoding]::UTF8
        $decoded = $utf8.GetString($bytes)
        $reencoded = $utf8.GetBytes($decoded)
        if ($reencoded.Length -eq $bytes.Length) {
            $same = $true
            for ($i = 0; $i -lt $bytes.Length; $i++) {
                if ($bytes[$i] -ne $reencoded[$i]) { $same = $false; break }
            }
            if ($same) { return @{ Encoding = $utf8; HasBom = $false } }
        }
    } catch {
        # ignore
    }

    # fallback to system default (ANSI) to preserve legacy encodings like GBK
    return @{ Encoding = [System.Text.Encoding]::Default; HasBom = $false }
}

function Replace-OneFile {
    param($filePath)

    # read raw bytes and detect encoding
    $bytes = [System.IO.File]::ReadAllBytes($filePath)
    $encInfo = Get-FileEncoding -bytes $bytes
    $encoding = $encInfo.Encoding
    $hasBom = $encInfo.HasBom

    $text = $encoding.GetString($bytes)
    $original = $text
    $offset = 0
    $modified = $false

    while ($true) {
        # find next occurrence of string.IsNullOrEmpty( or String.IsNullOrEmpty(
        $idx1 = $text.IndexOf('string.IsNullOrEmpty(', $offset, [System.StringComparison]::Ordinal)
        $idx2 = $text.IndexOf('String.IsNullOrEmpty(', $offset, [System.StringComparison]::Ordinal)
        if ($idx1 -lt 0) { $idx1 = [int]::MaxValue }
        if ($idx2 -lt 0) { $idx2 = [int]::MaxValue }
        $idx = [math]::Min($idx1, $idx2)
        if ($idx -eq [int]::MaxValue) { break }

        # the position of the opening parenthesis
        $startArg = $idx + ("string.IsNullOrEmpty(".Length)
        # if matched String variant, adjust startArg
        if ($idx -eq $idx2) { $startArg = $idx + ("String.IsNullOrEmpty(".Length) }

        # find matching closing parenthesis taking nested parentheses into account
        $depth = 1
        $i = $startArg
        while ($i -lt $text.Length -and $depth -gt 0) {
            $ch = $text[$i]
            if ($ch -eq '(') { $depth++ }
            elseif ($ch -eq ')') { $depth-- }
            $i++
        }
        if ($depth -ne 0) {
            Write-Warning "Unmatched parentheses in $filePath at position $idx. Skipping this occurrence."
            $offset = $startArg
            continue
        }

        $argLength = $i - $startArg - 1
        if ($argLength -lt 0) { $offset = $i; continue }
        $arg = $text.Substring($startArg, $argLength)

        # Trim surrounding whitespace
        $argTrimmed = $arg.Trim()

        # Decide whether we need parentheses around the expression.
        function IsSafeToUnparenthesize($s) {
            if ([string]::IsNullOrEmpty($s)) { return $false }
            # whitespace inside expression => keep parentheses
            if ($s -match '\s') { return $false }
            # common operator characters that make expression complex
            if ($s -match '[+\-*/%&|\^=<>?:]') { return $false }
            # return true for simple identifiers, member access, indexers and method calls
            if ($s -match '^[A-Za-z_][A-Za-z0-9_]*(?:[.\[][^^\]]*\]|\([^)]*\))*$') { return $true }
            return $false
        }

        if (IsSafeToUnparenthesize($argTrimmed)) {
            $replacement = $argTrimmed + ".IsNullOrEmpty()"
        } else {
            $replacement = "(" + $argTrimmed + ").IsNullOrEmpty()"
        }

        # Replace the span from $idx to $i (inclusive of closing paren)
        $before = $text.Substring(0, $idx)
        $after = $text.Substring($i)
        $text = $before + $replacement + $after

        $modified = $true
        $offset = $idx + $replacement.Length
    }

    if ($modified) {
        Write-Host "Changes prepared for: $filePath"
        if ($WhatIf) {
            Write-Host "[WhatIf] Would update $filePath"
        } else {
            if ($Backup) {
                $bak = $filePath + ".bak"
                if (-not (Test-Path $bak)) {
                    Copy-Item -LiteralPath $filePath -Destination $bak -ErrorAction SilentlyContinue
                }
            }

            # write preserving encoding and BOM (when originally present)
            try {
                $outBytes = @()
                if ($hasBom) {
                    $preamble = $encoding.GetPreamble()
                    if ($preamble -ne $null -and $preamble.Length -gt 0) {
                        $outBytes = $preamble + $encoding.GetBytes($text)
                    } else {
                        $outBytes = $encoding.GetBytes($text)
                    }
                } else {
                    # no BOM originally: for UTF8 specifically, we avoid emitting BOM
                    if ($encoding -eq [System.Text.Encoding]::UTF8) {
                        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
                        $outBytes = $utf8NoBom.GetBytes($text)
                    } else {
                        $outBytes = $encoding.GetBytes($text)
                    }
                }
                [System.IO.File]::WriteAllBytes($filePath, $outBytes)
                Write-Host "Updated: $filePath"
                } catch {
                    Write-Warning ("Failed to write file {0}: {1}" -f $filePath, $_)
                }
        }
    }
}

foreach ($f in $files) {
    try {
        Replace-OneFile -filePath $f.FullName
    } catch {
        Write-Warning ("Failed to process {0}: {1}" -f $f.FullName, $_)
    }
}

Write-Host "Done. Review changes and run your solution build."
