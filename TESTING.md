# Tests de caracterización de DKbase

Esta suite congela el comportamiento **actual** de DKbase antes de refactorizar.
Los tests no dicen qué *debería* hacer el código: dicen qué *hace hoy*.
Si un test falla después de un cambio, el cambio alteró un comportamiento observable.
Ese cambio puede ser intencional (se actualiza el test) o una regresión (se corrige el código).

No se modificó código de producción. Todo vive en `tests/`, más este archivo, `.gitignore` y la exclusión de `tests/` en `DKbase.csproj`.

## Cómo ejecutar

Requisito: .NET SDK 8 o superior. El `global.json` de la raíz fija el SDK 5.0.408 para la librería.
`tests/global.json` permite usar el SDK 8 para los tests, por eso **los comandos se corren desde `tests/`**.

```bash
cd tests

# Todos los tests
dotnet test DKbase.Tests

# Cobertura (Cobertura XML + reporte HTML y resumen de texto)
dotnet test DKbase.Tests --settings coverage.runsettings --results-directory coverage/raw --logger "trx;LogFileName=corrida.trx"
reportgenerator -reports:"coverage/raw/*/coverage.cobertura.xml" -targetdir:coverage/report -reporttypes:"TextSummary;Html"
python3 tools/reporte_por_capa.py "coverage/raw/*/coverage.cobertura.xml" "coverage/raw/*.trx"   # tabla por capa

# Mutation testing (Stryker.NET 4.4.1) sobre las capas críticas
dotnet tool install -g dotnet-stryker --version 4.4.1
tools/mutacion.sh          # reporte en tests/StrykerOutput/reports/mutation-report.html
```

`reportgenerator` se instala con `dotnet tool install -g dotnet-reportgenerator-globaltool`.

### Linux con glibc 2.41 o superior

En distribuciones con glibc 2.41 o superior (por ejemplo Ubuntu 25.04 en adelante), **todos** los tests fallan en el
constructor de `TestBase` con `DllNotFoundException ... mm-exhelper.so ... cannot enable executable stack`.
Pasa porque `Lib.Harmony` 2.3.3 carga, vía MonoMod, una librería auxiliar que pide *executable stack*, y esas
versiones de glibc ya no lo permiten por defecto. No es un problema del código de DKbase.
Se resuelve habilitándolo solo para el proceso de los tests:

```bash
export GLIBC_TUNABLES=glibc.rtld.execstack=2
dotnet test DKbase.Tests
```

Se puede ver la versión con `ldd --version`. En Windows no hace falta.

### Por qué hay un script para Stryker

Stryker analiza `DKbase.csproj` por su cuenta y choca con el SDK 5 fijado en el `global.json` de la raíz.
`tools/mutacion.sh` copia el repo a una carpeta temporal y, **solo en la copia**, quita `global.json`.

### El csproj de la librería y la carpeta `tests/`

`DKbase.csproj` compila todos los `.cs` del árbol. Para que no incluya los tests tiene un `ItemGroup` con
`<Compile Remove="tests\**" />`, `<None Remove="tests\**" />` y `<EmbeddedResource Remove="tests\**" />`.
Es el único cambio fuera de `tests/` (configuración de build, aprobado por Pablo; no cambia código).

## Cómo está armada la suite

| Pieza | Qué hace |
|---|---|
| `TestInfra/Interceptores.cs` | Con Lib.Harmony intercepta los límites externos sin tocar producción: `SqlConnection.Open/State/Close`, `SqlCommand.Execute*`, `DataTable.Load`, `DbDataAdapter.Fill`, `DateTime.Now/Today`, `SmtpClient.Send`, `WebClient.DownloadString` y `Thread.Sleep`. |
| `TestInfra/FakeDb.cs` | SQL Server falso: registra cada SP con sus parámetros, devuelve tablas, escalares, filas afectadas, parámetros de salida o errores. Tiene un **modo universal** que devuelve todas las columnas conocidas con valores tipados. |
| `TestInfra/FakeHttp.cs` | Reemplaza el `HttpMessageHandler` del `HttpClient` estático de `capaAPI`. |
| `TestInfra/TestBase.cs` | Reloj fijo (viernes 15/03/2024 10:00), cultura invariante, carpeta temporal por test, `Helper` restaurado a sus valores por defecto y consola silenciada. |
| `TestInfra/Builders.cs` | Builders reutilizables: `Crear.Cliente()`, `Crear.Producto()`, `Crear.ProductoGenerico()`, `Crear.Usuario()`, `Tabla.De(...)`. |
| `TestInfra/Excel.cs`, `FakeFormFile.cs` | Generan .xlsx en memoria y archivos subidos (`IFormFile`). |
| `TestInfra/ColumnasConocidas.g.cs` | Generado por `tools/generar_columnas.py`: las 376 columnas que el código lee de los `DataRow`, con su tipo inferido. |
| `Contratos/ContratoBaseDeDatosTests.cs` | Golden master de 20 clases de datos: invoca cada método público con la base vacía, caída y con datos, y congela SPs, parámetros, resultados y excepciones (`*.verified.txt`). |

Reglas que cumplen todos los tests: AAA, un comportamiento por test, nombres `deberia_<resultado>_cuando_<condicion>`, parametrizados con `[Theory]`, sin red, sin reloj real y sin orden entre tests.
La paralelización está desactivada porque DKbase usa estado estático global (`Helper`, `HttpClient` estático).

Los snapshots usan [Verify](https://github.com/VerifyTests/Verify). Si un cambio es intencional, se revisa el `.received.txt` y se renombra a `.verified.txt`.

## Resultados por capa

Última corrida: 27/11/2026, .NET SDK 8.0, Linux. Cobertura con coverlet (formato Cobertura). Mutation score de Stryker.NET 4.4.1.

| Capa | Tests | % líneas | % ramas | Mutation score |
|---|---:|---:|---:|---:|
| Dominio y servicios | 548 | 96,0 % (1802/1877) | 95,3 % (811/851) | 89,8 % (ver detalle) |
| Integraciones externas (HTTP, SMTP, ReCaptcha) | 61 | 98,9 % (707/715) | 100 % (14/14) | no medido |
| Acceso a datos (capa\*, acceso, Util) | 72 | 85,2 % (7542/8855) | 74,3 % (2058/2768) | no medido |
| Infraestructura (Log, Helper, Constantes, Serializador) | 44 | 90,5 % (210/232) | 83,3 % (25/30) | no medido |
| Infraestructura: cThumbnail / cThumbnail_base | 0 | 2,1 % (6/292) | 3,6 % (3/84) | no testeable (ver abajo) |
| Modelos y DTOs | 18 | 100 % (123/123) | 100 % (6/6) | no medido |
| **Total** | **743** | **85,9 %** (10390/12094) | **77,7 %** (2917/3753) | |

Cobertura de métodos: 98,9 % (655 de 662).

Las capas se definen por archivo en `tools/reporte_por_capa.py`. "Dominio y servicios" incluye precios, transfers, horarios de cierre, buscador, archivos de pedido, toma de pedidos, formatos numéricos, validaciones y grillas.
Los modelos que viven dentro de los archivos `capa*.cs` (por ejemplo `cClientes`) cuentan en "Acceso a datos".

### Mutation testing (Stryker.NET)

Se corrió sobre los cuatro archivos con más lógica de negocio. Los mutantes "CompileError" no cuentan para el score.

| Archivo | Muertos | Sobrevivientes | Sin cobertura | Score |
|---|---:|---:|---:|---:|
| `web/FuncionesPersonalizadas_base.cs` | 489 | 23 | 10 | 93,7 % |
| `web/cSubirpedido_base.cs` | 150 | 18 | 3 | 87,7 % |
| `generales/Numerica.cs` | 54 | 8 | 0 | 87,1 % |
| `web/capaDatos/capaCAR_WebService.cs` | 249 | 24 | 21 | 84,7 % |
| **Total** | **942** | **73** | **34** | **89,8 %** |

La primera corrida dio 78,7 %. Con los sobrevivientes se agregaron tests de límites (cierre fijo de las 22:15:30, líneas S de 22 caracteres),
del buscador por tipo de cliente, de los productos no encontrados al subir archivos y del armado de carritos.

La mayoría de los sobrevivientes que quedan son **mutantes equivalentes**: no cambian ningún resultado observable. Por ejemplo:
inicializar un `string` con `"Stryker was here!"` cuando se pisa en la línea siguiente, cambiar `<` por `<=` en un `if` cuyo caso de igualdad ya lo atrapa
la condición anterior, o quitar un `break` de un bucle que igual termina. Los que no son equivalentes están en ramas que solo escriben en el log.

### Por qué algunas capas no llegan al objetivo

Objetivo pedido: ≥ 90 % líneas / 85 % ramas en dominio y servicios, ≥ 80 % en el resto.

- **Acceso a datos (85,2 % líneas / 74,3 % ramas)**: llega a la meta de líneas, no a la de ramas. Cada mapeador `DataRow → objeto` tiene un `if (fila["x"] != DBNull.Value)` por columna,
  y muchas columnas son opcionales (`Table.Columns.Contains`). El golden master cubre "todas las columnas con valor" y "base vacía";
  las combinaciones de nulos columna por columna se dejaron para cuando se toque cada mapeador. Las clases más bajas son `accesoApp` (62 %), `capaCV_base` (59 %) y `Util` (71 %).
- **Infraestructura**: sin `cThumbnail*` está en 90,5 % / 83,3 %. Las dos clases de thumbnails usan GDI+ (`System.Drawing`), que no existe fuera de Windows en .NET 8.
- **Integraciones externas (98,9 % líneas)**: las líneas que faltan son ramas de `capaAPI` que solo se alcanzan con errores de red muy específicos.

## Comportamientos sospechosos (posibles bugs)

Ninguno se corrigió. Cada uno tiene un test que confirma el comportamiento actual, marcado en el código con `// CARACTERIZACIÓN: posible bug - ...`.

| # | Área | Comportamiento actual | Test |
|---|---|---|---|
| 1 | Modelos | new cProductosGenerico(cProductos) NO copia 15 propiedades del producto, entre ellas pro_troquel, pro_isCadenaFrio, pro_requierevale, pro_canmaxima, pro_vtasolotransfer, pro_entransfer, pro_AltoCosto y PrecioConDescuentoOferta (ver "NO copiada" en el snapshot). | [`deberia_mantener_las_propiedades_copiadas_por_los_constructores_de_copia`](tests/DKbase.Tests/Modelos/ModelosTests.cs) |
| 2 | Modelos | CargarTransferYTransferDetalle no copia tde_DescripcionDeProducto ni tfr_provincia. | [`deberia_mantener_las_propiedades_copiadas_por_los_constructores_de_copia`](tests/DKbase.Tests/Modelos/ModelosTests.cs) |
| 3 | Modelos | la "copia" es superficial: modificar la lista de la copia modifica el original. | [`deberia_compartir_las_listas_cuando_copia_un_buscador`](tests/DKbase.Tests/Modelos/ModelosTests.cs) |
| 4 | Modelos | CoreService y CoreServiceTest son implementaciones de prueba ("Hola Mundo") | [`deberia_devolver_pedido_de_prueba_hola_mundo_cuando_se_usa_core_service`](tests/DKbase.Tests/Modelos/ModelosTests.cs) |
| 5 | Modelos | web_Error_StackTrace lee y escribe el mismo campo que web_Error. | [`deberia_compartir_el_campo_entre_web_error_y_su_stack_trace`](tests/DKbase.Tests/Modelos/ModelosTests.cs) |
| 6 | Log | LogError ignora la fecha recibida (pFechaActual) y usa DateTime.Now. | [`deberia_registrar_error_con_mensaje_y_parametros_en_la_base`](tests/DKbase.Tests/generales/LogTests.cs) |
| 7 | Log | con menos valores que parámetros, getParameters lanza IndexOutOfRange y se registra un error extra "getParameters". | [`deberia_loguear_error_extra_y_parametros_parciales_cuando_faltan_valores`](tests/DKbase.Tests/generales/LogTests.cs) |
| 8 | Numerica | la parte decimal se obtiene quitando la parte entera TRUNCADA del texto redondeado; si F2 redondea hacia arriba (0.995 → "1.00") el resultado queda mal. | [`deberia_corromper_decimales_cuando_el_redondeo_sube_la_parte_entera`](tests/DKbase.Tests/generales/NumericaTests.cs) |
| 9 | Numerica | los negativos quedan con el "-" entre ceros y con largo variable. | [`deberia_poner_el_signo_en_el_medio_cuando_el_numero_es_negativo`](tests/DKbase.Tests/generales/NumericaTests.cs) |
| 10 | Numerica | -0,5 se formatea como "-,50" (falta el 0 entero). | [`deberia_perder_el_cero_entero_cuando_es_negativo_menor_a_uno`](tests/DKbase.Tests/generales/NumericaTests.cs) |
| 11 | BuscadorGeneral | si la base devuelve menos de 3 tablas, Tables[2] lanza IndexOutOfRangeException sin manejar. | [`deberia_usar_el_sp_de_oferta_cuando_se_informa_id_de_oferta`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/BuscadorGeneralTests.cs) |
| 12 | BuscadorYConversiones | inyección SQL: el texto del buscador se concatena sin escapar. | [`deberia_concatenar_texto_sin_escapar_cuando_tiene_comillas`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/BuscadorYConversionesTests.cs) |
| 13 | BuscadorYConversiones | el código de producto se concatena en DataTable.Select sin escapar comillas. | [`deberia_lanzar_excepcion_cuando_codigo_de_producto_tiene_comilla`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/BuscadorYConversionesTests.cs) |
| 14 | CantidadTransfer | tde_muluni = 0 divide por cero (Infinity) y Convert.ToInt32 lanza OverflowException sin manejar. | [`deberia_lanzar_OverflowException_cuando_unidad_multiplo_es_cero`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/CantidadTransferTests.cs) |
| 15 | CantidadTransfer | facturación directa, transfer más barato y sin reglas de unidades: no se marca "pasar directamente" y la cantidad pedida se pierde ([0, 0]). | [`deberia_devolver_cero_y_cero_cuando_conviene_transfer_pero_no_hay_reglas`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/CantidadTransferTests.cs) |
| 16 | HorariosCierre | un horario con formato "ab:cd" lanza FormatException sin manejar. | [`deberia_lanzar_FormatException_cuando_horario_no_es_numerico`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/HorariosCierreTests.cs) |
| 17 | HorariosCierre | si hay dos filas para el mismo día, solo se mira la primera (break); el horario de las 18:00 de la segunda fila se ignora y se informa el del día siguiente. | [`deberia_usar_solo_la_primera_fila_del_dia_cuando_hay_filas_repetidas`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/HorariosCierreTests.cs) |
| 18 | HorariosCierre | si la consulta de horarios falla, el método de datos devuelve null y aquí se hace .Where() sobre null sin control. | [`deberia_lanzar_ArgumentNullException_cuando_falla_la_base`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/HorariosCierreTests.cs) |
| 19 | HorariosCierre | cuando el día es otro, la hora queda con 50 segundos; cuando es hoy, con 0. | [`deberia_calcular_proxima_fecha_del_dia_indicado`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/HorariosCierreTests.cs) |
| 20 | HorariosCierre | arma la fecha con hoy.Day + sumaDia: el sábado 30/03 buscando el lunes intenta crear el 32/03, lanza ArgumentOutOfRangeException, se traga sin log y devuelve "". | [`deberia_devolver_vacio_cuando_el_dia_cae_en_el_mes_siguiente`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/HorariosCierreTests.cs) |
| 21 | Precios | un producto neto con tipo distinto de M/P/A/V queda con precio final 0. | [`deberia_devolver_cero_cuando_producto_neto_tiene_tipo_desconocido`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/PreciosTests.cs) |
| 22 | Precios | no se acota el descuento a 100 %, el precio puede quedar negativo. | [`deberia_devolver_precio_negativo_cuando_descuento_supera_100`](tests/DKbase.Tests/web/FuncionesPersonalizadas_base/PreciosTests.cs) |
| 23 | GrillaYPermisos | un campo cuyo nombre termina en "ASC" (p. ej. "cod_BASC") pierde esas letras. | [`deberia_tratar_como_ascendente_un_campo_que_termina_en_ASC`](tests/DKbase.Tests/web/GrillaYPermisosTests.cs) |
| 24 | GrillaYPermisos | RecuperarTodasAccionesRol devuelve null ante un error y el foreach lanza NullReferenceException. | [`deberia_lanzar_excepcion_cuando_la_base_de_acciones_falla`](tests/DKbase.Tests/web/GrillaYPermisosTests.cs) |
| 25 | LeerArchivoPedido | se eliminan los puntos intermedios del nombre del archivo subido. | [`deberia_quitar_puntos_intermedios_del_nombre_cuando_tiene_varios`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 26 | LeerArchivoPedido | un archivo sin extensión se guarda como ".<nombre>" y se rechaza como formato desconocido. | [`deberia_usar_el_nombre_como_extension_cuando_no_tiene_punto`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 27 | LeerArchivoPedido | las líneas F exigen 24 caracteres aunque el formato usa 22. | [`deberia_ignorar_lineas_F_de_22_o_23_caracteres_aunque_sean_parseables`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 28 | LeerArchivoPedido | un archivo vacío de formato desconocido se acepta como correcto. | [`deberia_devolver_resultado_correcto_cuando_formato_desconocido_esta_vacio`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 29 | LeerArchivoPedido | los códigos de barra guardados como texto compartido (shared string) en Excel se ignoran. | [`deberia_leer_solo_celdas_numericas_y_saltear_encabezado_cuando_es_excel`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 30 | LeerArchivoPedido | un Excel corrupto envía una tabla null a la base y el resultado queda como correcto. | [`deberia_enviar_tabla_nula_cuando_excel_esta_corrupto`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 31 | LeerArchivoPedido | el resultado queda como correcto aunque falle la base. | [`deberia_marcar_correcto_aunque_falle_la_base`](tests/DKbase.Tests/web/cSubirpedido_base/LeerArchivoPedidoTests.cs) |
| 32 | LeerRenglones | el lector de archivos acepta líneas S desde 22 caracteres, pero el parseo necesita 38. | [`deberia_devolver_null_y_loguear_cuando_linea_S_tiene_menos_de_38_caracteres`](tests/DKbase.Tests/web/cSubirpedido_base/LeerRenglonesTests.cs) |
| 33 | BuscadorYCarritos | con la base vacía o caída se accede a Tables[1] sin validar y se lanza excepción. | [`deberia_lanzar_excepcion_cuando_la_base_no_devuelve_la_tabla_de_carritos`](tests/DKbase.Tests/web/capaDatos/BuscadorYCarritosTests.cs) |
| 34 | TomarPedido | si el SP de faltantes no devuelve la tabla esperada, InsertarFaltantesProblemasCrediticios lanza excepción; el pedido ya fue tomado en DKdll pero no se guarda en CAR.spCargarPedido ni se borra el carrito. | [`deberia_devolver_el_pedido_sin_guardarlo_ni_borrar_el_carrito_cuando_falla_el_registro_de_faltantes`](tests/DKbase.Tests/web/capaDatos/TomarPedidoTests.cs) |
| 35 | TomarPedido | el XML de guardarPedido no admite codProducto null: el pedido queda tomado en DKdll y el carrito borrado, pero CAR.spCargarPedido nunca se ejecuta (solo queda un log). | [`deberia_devolver_el_pedido_sin_guardarlo_cuando_un_producto_del_carrito_no_tiene_codigo`](tests/DKbase.Tests/web/capaDatos/TomarPedidoTests.cs) |
| 36 | TomarPedido | sin transfers de la sucursal, car_id_aux queda en 0 y se llama igual a DKdll con una lista vacía. | [`deberia_enviar_un_pedido_vacio_con_carrito_cero_cuando_ningun_transfer_es_de_la_sucursal`](tests/DKbase.Tests/web/capaDatos/TomarPedidoTests.cs) |
| 37 | TomarPedido | a diferencia de TomarPedidoCarrito_generico, aquí no se valida Items != null ni hay try/catch: un pedido OK sin Items lanza NullReferenceException al llamador. | [`deberia_lanzar_excepcion_cuando_el_pedido_habitual_vuelve_sin_lista_de_items`](tests/DKbase.Tests/web/capaDatos/TomarPedidoTests.cs) |
| 38 | TomarPedido | si el borrado nunca funciona, el hilo queda bloqueado con Thread.Sleep más de un minuto (10 reintentos). | [`deberia_reintentar_diez_veces_con_esperas_crecientes_y_devolver_false_cuando_nunca_se_puede_borrar`](tests/DKbase.Tests/web/capaDatos/TomarPedidoTests.cs) |
| 39 | capaAPI | una respuesta que no es JSON válido lanza JsonException sin capturar a nivel API. | [`deberia_lanzar_JsonException_cuando_la_respuesta_no_es_json_valido`](tests/DKbase.Tests/web/capaDatos/capaAPITests.cs) |
| 40 | capaAPI | ModificarPasswordWEB siempre devuelve "", haya funcionado o no. | [`deberia_devolver_vacio_siempre_cuando_modifica_password`](tests/DKbase.Tests/web/capaDatos/capaAPITests.cs) |
| 41 | capaAPI | ImprimirComprobante devuelve true aunque la API falle. | [`deberia_devolver_true_siempre_cuando_imprime_comprobante`](tests/DKbase.Tests/web/capaDatos/capaAPITests.cs) |
| 42 | capaAPI | url_DKcore/url_DKdll se copian de Helper en el inicializador estático; si Helper se configura después de tocar capaAPI, las URLs quedan viejas (o null). | [`deberia_mantener_url_cargada_al_iniciar_la_clase_aunque_cambie_Helper`](tests/DKbase.Tests/web/capaDatos/capaAPITests.cs) |
| 43 | capaDLL | el resultado de InicioCarritoEnProceso se ignora; el pedido se envía igual. | [`deberia_tomar_pedido_aunque_no_se_pueda_bloquear_el_carrito`](tests/DKbase.Tests/web/capaDatos/capaDLLTests.cs) |
| 44 | capaDLL | el catch vuelve a usar pCliente.cli_login para loguear y lanza otra NullReferenceException. | [`deberia_lanzar_NullReferenceException_desde_el_catch_cuando_cliente_es_null`](tests/DKbase.Tests/web/capaDatos/capaDLLTests.cs) |
| 45 | ReCaptchaYMail | se acepta cualquier respuesta de Google que contenga el texto "true" (no se parsea el JSON). | [`deberia_aceptar_cuando_la_respuesta_contiene_true`](tests/DKbase.Tests/web/generales/ReCaptchaYMailTests.cs) |
| 46 | ReCaptchaYMail | la clave secreta se copia de Helper en el inicializador estático; cambios posteriores de Helper.getReCAPTCHA_ClaveSecreta no se ven. | [`deberia_usar_la_clave_leida_al_cargar_la_clase_aunque_cambie_la_configuracion`](tests/DKbase.Tests/web/generales/ReCaptchaYMailTests.cs) |
| 47 | ReCaptchaYMail | cualquier error de envío se traga sin registrar nada. | [`deberia_devolver_false_sin_log_cuando_falla_el_servidor_smtp`](tests/DKbase.Tests/web/generales/ReCaptchaYMailTests.cs) |
| 48 | Texto_base | la tabla de origen tiene "u" en lugar de "ü" minúscula, así que "ü" no se convierte. | [`deberia_no_quitar_dieresis_de_u_minuscula`](tests/DKbase.Tests/web/generales/Texto_baseTests.cs) |
| 49 | Validaciones | isMail(null) lanza excepción en vez de devolver false. | [`deberia_lanzar_ArgumentNullException_cuando_mail_es_null`](tests/DKbase.Tests/web/generales/ValidacionesTests.cs) |

Otros hallazgos sin test propio:

- **Código duplicado**: `generales/Validaciones` y `web/generales/Validaciones_base` son copias (los tests corren contra las dos); lo mismo `cThumbnail` y `cThumbnail_base`, y `Util.GetUsuariosDeCliente` y `AccesoGrilla_base.GetUsuariosDeCliente` (también probadas las dos).
- **`BaseDataAccess.GetDataReader`** devuelve un `DbDataReader` después de cerrar la conexión que lo creó (`using`), así que con una base real el lector no se puede usar.

## Código no testeable (o no testeado) y sugerencias

| Código | Por qué | Sugerencia |
|---|---|---|
| `generales/cThumbnail`, `web/cThumbnail_base` | Usan `System.Drawing` (GDI+), que en .NET 6+ solo existe en Windows. | Aislar el redimensionado detrás de una interfaz y usar una librería multiplataforma (por ejemplo ImageSharp). |
| `Util.GenerateDocument` | Abre plantillas OpenXML desde rutas fijas en disco. | Recibir la plantilla como `Stream` para poder testearla con un archivo de fixture. |
| `BaseDataAccess.GetDataReader` | El valor de retorno solo tiene sentido con una conexión real abierta. | Tests de integración contra SQL Server (por ejemplo Testcontainers) o devolver un `DataTable`. |
| Lógica dentro de los stored procedures | Vive en SQL Server, fuera del repo. Los tests solo fijan el contrato (nombre de SP, parámetros, tipos). | Tests de integración de los SP en una base de prueba. |
| Inicializadores estáticos de `capaAPI` y `ReCaptchaClass` | Copian `Helper` una sola vez al cargar la clase; los tests los ajustan por reflexión. | Inyectar configuración (`IOptions`) y un `HttpClient` por DI. |
| `capaAPI.setDatosLogin` | Lanza la autenticación en una tarea en segundo plano sin esperar; el test tiene que sondear. | Exponer un método `async` que se pueda esperar. |
| Ramas `catch` que solo loguean y bloques defensivos de columnas opcionales | Muchas combinaciones dependen de esquemas de SP que el repo no documenta. | Cubrirlas a medida que se refactoricen, con datos reales de cada SP. |

## Resumen de la última ejecución

- Fecha: 27/11/2026. Entorno: Linux, .NET SDK 8.0, xUnit 2.9.2.
- **743 tests, 743 en verde, 0 fallidos, 0 omitidos.**
- Se ejecutó la suite completa **3 veces seguidas** (05:08:55, 05:09:00 y 05:09:06 UTC): 743/743 en verde las tres veces, unos 4 segundos cada una. No hubo tests inestables.
- Durante el armado apareció un test inestable (la autenticación en segundo plano de `capaAPI.setDatosLogin`); se corrigió en el test (credenciales por reflexión y espera activa del header), no en producción. Después pasó 8 corridas seguidas.
- Cobertura total: 85,9 % líneas, 77,7 % ramas, 98,9 % métodos.
- Mutation testing: 89,8 % sobre las capas críticas (1.049 mutantes evaluados).
- Comportamientos sospechosos documentados: 49 (tabla de arriba).
