#!/usr/bin/env bash
# בונה את כל פרויקטי ה-.NET בקורס (דמואים, מעבדות, פתרונות) ומדווח על כשלונות.
set -u
cd "$(dirname "$0")/.."
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
fail=0; ok=0
while IFS= read -r proj; do
  if dotnet build "$proj" -nologo -v q -clp:NoSummary >/tmp/build.log 2>&1; then
    ok=$((ok+1)); echo "OK    $proj"
  else
    fail=$((fail+1)); echo "FAIL  $proj"; grep -E "error" /tmp/build.log | head -5
  fi
done < <(find . -name "*.csproj" -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/node_modules/*" | sort)
echo "---- built: $ok  failed: $fail"
exit $fail
