#!/usr/bin/env python3
"""Resumen de cobertura y cantidad de tests por capa.

Uso (desde tests/):
  python3 tools/reporte_por_capa.py coverage/raw/*/coverage.cobertura.xml coverage/raw/*.trx
Imprime una tabla Markdown lista para pegar en TESTING.md.
"""
import sys, glob, re, xml.etree.ElementTree as ET
from collections import defaultdict

CAPAS = [
    ("Dominio y servicios", [r"web/FuncionesPersonalizadas_base\.cs", r"generales/Numerica\.cs", r"web/cSubirpedido_base\.cs",
                             r"web/capaDatos/capaCAR_WebService\.cs", r"generales/Validaciones\.cs", r"web/generales/Validaciones_base\.cs",
                             r"web/generales/Texto_base\.cs", r"web/AccesoGrilla_base\.cs"]),
    ("Integraciones externas (HTTP, SMTP, ReCaptcha)", [r"web/capaDatos/capaAPI\.cs", r"web/capaDatos/capaDLL\.cs",
                             r"web/generales/ReCaptchaClass\.cs", r"web/generales/cMail_base\.cs"]),
    ("Acceso a datos (capa*, acceso, Util)", [r"web/capaDatos/", r"web/acceso\.cs", r"app/accesoApp\.cs", r"baseDatos/",
                             r"generales/BaseDataAccess\.cs", r"Util\.cs"]),
    ("Infraestructura (Log, Helper, Constantes, Serializador, Thumbnail)", [r"Helper\.cs", r"generales/", r"web/cThumbnail_base\.cs"]),
    ("Modelos y DTOs", [r"Business/", r"Entities/", r"Models/", r"dll/", r"web/Clases"]),
]
# Clase de test -> capa (las no listadas van por prefijo de namespace).
TESTS = {
    "Dominio y servicios": ["FuncionesPersonalizadas.", "NumericaTests", "cSubirpedido.", "TomarPedidoTests",
                            "ValidacionesTests", "Texto_baseTests", "GrillaYPermisosTests"],
    "Integraciones externas (HTTP, SMTP, ReCaptcha)": ["capaAPITests", "capaAPIContratoTests", "capaDLLTests", "capaDLLContratoTests", "ReCaptcha", "cMail"],
    "Acceso a datos (capa*, acceso, Util)": ["Contratos", "StoredProcedureTests", "BaseDataAccessTests"],
    "Infraestructura (Log, Helper, Constantes, Serializador, Thumbnail)": ["LogTests", "Serializador", "ConstantesYHelperTests"],
    "Modelos y DTOs": ["Modelos"],
}

def capa_de(archivo):
    archivo = archivo.replace("\\", "/")
    # el orden importa: CAR_WebService/capaAPI/capaDLL antes que el genérico web/capaDatos/
    for nombre, patrones in CAPAS:
        if any(re.search(p, archivo) for p in patrones):
            return nombre
    return "Otros"

def main(args):
    xmls = [a for p in args for a in glob.glob(p) if a.endswith(".xml")]
    trxs = [a for p in args for a in glob.glob(p) if a.endswith(".trx")]
    lineas = defaultdict(dict); ramas = defaultdict(dict)
    for x in xmls:
        raiz = ET.parse(x).getroot()
        fuentes = [s.text for s in raiz.iter("source")]
        for c in raiz.iter("class"):
            f = c.get("filename")
            capa = capa_de(f)
            for l in c.iter("line"):
                clave = (f, l.get("number"))
                hits = int(l.get("hits"))
                lineas[capa][clave] = max(lineas[capa].get(clave, 0), hits)
                cond = l.get("condition-coverage")
                if cond:
                    m = re.search(r"\((\d+)/(\d+)\)", cond)
                    cub, tot = int(m.group(1)), int(m.group(2))
                    prev = ramas[capa].get(clave, (0, tot))
                    ramas[capa][clave] = (max(prev[0], cub), tot)
    tests = defaultdict(int)
    for t in trxs:
        ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
        raiz = ET.parse(t).getroot()
        clases = {u.get("id"): u.find("t:TestMethod", ns).get("className") for u in raiz.iterfind(".//t:UnitTest", ns)}
        for r in raiz.iterfind(".//t:UnitTestResult", ns):
            nombre = clases.get(r.get("testId"), "") + "." + r.get("testName")
            asignada = next((c for c, claves in TESTS.items() if any(k in nombre for k in claves)), "Otros")
            tests[asignada] += 1
    print("| Capa | Tests | % líneas | % ramas | Mutation score |")
    print("|---|---:|---:|---:|---:|")
    tot_l = tot_lc = tot_r = tot_rc = 0
    for nombre, _ in CAPAS + [("Otros", [])]:
        if nombre not in lineas and nombre not in tests: continue
        l = lineas[nombre]; r = ramas[nombre]
        lc = sum(1 for h in l.values() if h > 0); lt = len(l)
        rc = sum(a for a, _ in r.values()); rt = sum(b for _, b in r.values())
        tot_l += lt; tot_lc += lc; tot_r += rt; tot_rc += rc
        pl = f"{100*lc/lt:.1f} % ({lc}/{lt})" if lt else "-"
        pr = f"{100*rc/rt:.1f} % ({rc}/{rt})" if rt else "-"
        print(f"| {nombre} | {tests.get(nombre, 0)} | {pl} | {pr} | |")
    print(f"| **Total** | **{sum(tests.values())}** | **{100*tot_lc/tot_l:.1f} %** | **{100*tot_rc/tot_r:.1f} %** | |")

if __name__ == "__main__":
    main(sys.argv[1:])
