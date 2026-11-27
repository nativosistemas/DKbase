#!/usr/bin/env bash
# Mutation testing con Stryker.NET sin tocar el repositorio.
#
# Stryker analiza DKbase.csproj directamente, y eso choca con dos cosas del repo:
#   1. global.json fija el SDK 5.0.408 (no instalado en entornos con SDK 8).
#   2. DKbase.csproj compila todos los .cs del árbol, incluidos los de tests/.
# Por eso el script copia el repo a una carpeta temporal, quita global.json de la COPIA y agrega
# <Compile Remove="tests\**" /> al csproj de la COPIA. El código de producción no cambia.
#
# Uso (desde cualquier carpeta):  tests/tools/mutacion.sh
# Reporte: tests/StrykerOutput/reports/mutation-report.html
set -euo pipefail
REPO="$(cd "$(dirname "$0")/../.." && pwd)"
TMP="$(mktemp -d)/DKbase"
mkdir -p "$TMP"
rsync -a --exclude .git --exclude 'bin/' --exclude 'obj/' --exclude 'tests/StrykerOutput' --exclude 'tests/coverage' "$REPO/" "$TMP/"
rm -f "$TMP/global.json"
sed -i 's|</Project>|  <ItemGroup><Compile Remove="tests\\**" /><None Remove="tests\\**" /><EmbeddedResource Remove="tests\\**" /></ItemGroup>\n</Project>|' "$TMP/DKbase.csproj"
cd "$TMP/tests"
"${STRYKER:-dotnet-stryker}" --config-file stryker-config.json --output StrykerOutput "$@"
rm -rf "$REPO/tests/StrykerOutput"
cp -r "$TMP/tests/StrykerOutput" "$REPO/tests/StrykerOutput"
echo "Reporte: $REPO/tests/StrykerOutput/reports/mutation-report.html"
