#!/usr/bin/env bash
# Mutation testing con Stryker.NET sin tocar el repositorio.
#
# Stryker analiza DKbase.csproj directamente, y global.json fija el SDK 5.0.408
# (no instalado en entornos con SDK 8). Por eso el script copia el repo a una carpeta
# temporal y quita global.json de la COPIA. El repositorio no cambia.
#
# Uso (desde cualquier carpeta):  tests/tools/mutacion.sh
# Reporte: tests/StrykerOutput/reports/mutation-report.html
set -euo pipefail
REPO="$(cd "$(dirname "$0")/../.." && pwd)"
TMP="$(mktemp -d)/DKbase"
mkdir -p "$TMP"
rsync -a --exclude .git --exclude 'bin/' --exclude 'obj/' --exclude 'tests/StrykerOutput' --exclude 'tests/coverage' "$REPO/" "$TMP/"
rm -f "$TMP/global.json"
cd "$TMP/tests"
"${STRYKER:-dotnet-stryker}" --config-file stryker-config.json --output StrykerOutput "$@"
rm -rf "$REPO/tests/StrykerOutput"
cp -r "$TMP/tests/StrykerOutput" "$REPO/tests/StrykerOutput"
echo "Reporte: $REPO/tests/StrykerOutput/reports/mutation-report.html"
