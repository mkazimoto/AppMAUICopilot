<#
.SYNOPSIS
  Pre check-in review hook for the CameraApp .NET MAUI project.

.DESCRIPTION
  Triggered by the PreToolUse hook before any run_in_terminal call.
  When the command is a `git commit` or `git push`, this script:
    1. Builds CameraApp targeting net10.0 (fast, no Android SDK required)
    2. Runs all unit tests in CameraApp.Test
  If either step fails, the commit/push is BLOCKED with an explanatory reason.

.NOTES
  Input:  JSON on stdin  { toolName, toolInput: { command, ... } }
  Output: JSON on stdout { hookSpecificOutput: { permissionDecision } }
  Exit 0 = success/allow, Exit 2 = blocking error/deny
#>

param()

# ── 1. Read and parse stdin ────────────────────────────────────────────────────
$inputJson = [Console]::In.ReadToEnd()
if (-not $inputJson.Trim()) { exit 0 }

try {
    $data = $inputJson | ConvertFrom-Json
} catch {
    exit 0  # Non-blocking: keep going if input is malformed
}

# ── 2. Filter: only intercept run_in_terminal ──────────────────────────────────
if ($data.toolName -ne "run_in_terminal") { exit 0 }

$command = $data.toolInput.command
if (-not $command) { exit 0 }

# ── 3. Filter: only intercept git commit / push ────────────────────────────────
if ($command -notmatch '(?i)^\s*git\s+(commit|push)\b') { exit 0 }

[Console]::Error.WriteLine("[pre-checkin-review] Intercepted: $command")

# ── 4. Locate projects relative to the working directory ──────────────────────
$repoRoot   = (Get-Location).Path
$mainProj   = Join-Path $repoRoot "CameraApp"      "CameraApp.csproj"
$testProj   = Join-Path $repoRoot "CameraApp.Test" "CameraApp.Test.csproj"
$reasons    = [System.Collections.Generic.List[string]]::new()

# ── 5. Build check (net10.0 — compiles as library, no Android toolchain) ──────
[Console]::Error.WriteLine("[pre-checkin-review] Building CameraApp (net10.0)...")

$buildResult = dotnet build $mainProj -f net10.0 --no-restore -v quiet 2>&1
if ($LASTEXITCODE -ne 0) {
    $reasons.Add("Build failed — fix errors before committing. Run: dotnet build CameraApp/CameraApp.csproj -f net10.0")
    [Console]::Error.WriteLine("[pre-checkin-review] BUILD FAILED")
} else {
    [Console]::Error.WriteLine("[pre-checkin-review] Build OK")
}

# ── 6. Unit test check ─────────────────────────────────────────────────────────
if (Test-Path $testProj) {
    [Console]::Error.WriteLine("[pre-checkin-review] Running unit tests...")

    $testResult = dotnet test $testProj --no-restore -v quiet 2>&1
    if ($LASTEXITCODE -ne 0) {
        $reasons.Add("Unit tests failed — fix tests before committing. Run: dotnet test CameraApp.Test/")
        [Console]::Error.WriteLine("[pre-checkin-review] TESTS FAILED")
    } else {
        [Console]::Error.WriteLine("[pre-checkin-review] Tests OK")
    }
} else {
    [Console]::Error.WriteLine("[pre-checkin-review] Test project not found, skipping tests.")
}

# ── 7. Return decision ─────────────────────────────────────────────────────────
if ($reasons.Count -eq 0) {
    [Console]::Error.WriteLine("[pre-checkin-review] All checks passed — allowing check-in.")
    @{
        hookSpecificOutput = @{
            hookEventName      = "PreToolUse"
            permissionDecision = "allow"
        }
    } | ConvertTo-Json -Depth 3
    exit 0
} else {
    $msg = $reasons -join " | "
    [Console]::Error.WriteLine("[pre-checkin-review] BLOCKED: $msg")
    @{
        hookSpecificOutput = @{
            hookEventName              = "PreToolUse"
            permissionDecision         = "deny"
            permissionDecisionReason   = $msg
        }
    } | ConvertTo-Json -Depth 3
    exit 2  # Blocking exit code
}
